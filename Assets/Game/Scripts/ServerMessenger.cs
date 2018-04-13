using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerMessenger : MonoBehaviour 
{
	public CueController cueController;

	void Awake ()
	{
		Application.runInBackground = true;
	}

	void OnEnable() 
	{
		MenuControllerGenerator.controller.OnLoadLevel += OnFindCueController; // this chain is ONLY called for network games
	}

    void OnDisable()
    {
        MenuControllerGenerator.controller.OnLoadLevel -= OnFindCueController;
    }

	void OnFindCueController(MenuController menuController)
	{
		StartCoroutine(WaitAndFindCueController(menuController.preloader));
	}

	IEnumerator WaitAndFindCueController(Preloader preloader)
	{
		cueController = null;

		while(!preloader.isDone)
		{
			yield return null;
		}

		while(cueController == null)
		{
			cueController = CueController.FindObjectOfType<CueController>();

			yield return null;
		}

        yield return new WaitForEndOfFrame();

        cueController.enabled = true; // this is the golden line that shows the game to our players

        StartCoroutine(UpdateSlaveStillPositions(0)); // 0th shot
    }

    // this chain is pretty ugly
    IEnumerator UpdateSlaveStillPositions(int inShotCount)
    {
        while (cueController.stillBallPocketed == null || cueController.stillBallPositions == null)
        {
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        // slave would sprodically supply some user info and other info to master. but this doesn't change our fail scenario, in which slaves are strictly listening and master talking
        // we need to make sure that game should be able to carry on without any functional glitches even when the slave is completely slient
        if (cueController.NetworkSlaveInControl())
        {
            cueController.updateSlaveStillPositions(inShotCount);
        }
    }

    public void OnHelpfulTipReceived(string inText)
    {
        if (cueController)
        {
            string nameText = ""; // dajiang hack, special cases, special cases everywhere, and "Skip/Push/Break" are left untouched here coz they already work, LOLz.

            // get some name bullshits
            if (inText == "OutOfTime" || inText == "PocketCueBall" || inText == "FourBallRail" || inText == "RightFirstBall" || inText == "Disconnected")
            {
                nameText = Localization.Get("YourOpponent");
            }

            // see if we should kill everything else
            if (inText == "Disconnected" || inText == "OppoLeftGame")
            {
                cueController.ShowHelpfulTooltipPopup(nameText, inText, true, true);
            }
            else
            {
                cueController.ShowHelpfulTooltipPopup(nameText, inText, true, false);
            }

            // show tips

            // dajiang hack, increment foul scratches on the opponent, really kinda sketchy
            if (inText == "PocketCueBall" || inText == "FourBallRail" || inText == "RightFirstBall" || inText == "BallRailContact")
            {
                GameManager_script.IncrementScratch(cueController.TrackingShotAsPlayerOne, cueController.TrackingShotAsPlayerTwo);
            }
        }
    }

    public void OnGameFinished(bool inSelfWin)
    {
        if (cueController && !cueController.LoadMainMenuAlreadyCalled)
        {
            // reward self with chips (this happens only when opponent explicitly disconnects from the game)
            if (inSelfWin)
            {
                GameManager_script.Instance().UpdateCoinCount(2.0f * GameManager_script.Instance().CurrentWager);
            }
            else
            {
                GameManager_script.Instance().UpdateCoinCount(0.0f);
            }

            // load main menu for next game
            StartCoroutine(cueController.OnLoadMainMenu(inSelfWin, true)); // will show animations
        }
    }

    public void OnStatusUpdateStillPositions(int inShotCount, bool inPlayerInControl, bool inPushoutable, bool inSkippable, int inStatus, bool[] inPocketed, Vector3[] inPositions, bool inFirstTime)
    {
        StartCoroutine(StatusUpdateStillPositions(inShotCount, inPlayerInControl, inPushoutable, inSkippable, inStatus, inPocketed, inPositions, inFirstTime));
    }

    IEnumerator StatusUpdateStillPositions(int inShotCount, bool inPlayerInControl, bool inPushoutable, bool inSkippable, int inStatus, bool[] inPocketed, Vector3[] inPositions, bool inFirstTime)
    {
        while (cueController == null)
        {
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        bool ControlReallyChanged = ServerController.serverController.playerInControl != inPlayerInControl;

        ServerController.serverController.playerInControl = inPlayerInControl;

        if (ControlReallyChanged && !inFirstTime)
        {
            float transfer_volume = Mathf.Clamp01(1.0f);

            if (ServerController.serverController.playerInControl)
            {
                GameManager_script.Instance().PlaySound((int)MusicClip.Good_Transfer, false, transfer_volume); // getting control always cheers
            }
            else
            {
                GameManager_script.Instance().PlaySound((int)MusicClip.Bad_Transfer, false, transfer_volume); // losing control always sad
            }
        }

        CueControllerUpdater.current_control_status = inStatus;

        cueController.pushoutAllowed = inPushoutable;
        cueController.skipAllowed = inSkippable;

        cueController.stillBallPositions = inPositions;
        cueController.stillBallPocketed = inPocketed;

        StartCoroutine(UpdateStillPositions(inShotCount));

        cueController.ConditionalEnableForceSlider(); // update force slider control

        // activate hand cursor
        if (CueControllerUpdater.current_control_status == CueControllerUpdater.CUE_BALL_MOVABLE_ON_TABLE && inPlayerInControl && !inFirstTime)
        {
           cueController.ActivateHandCursor();
        }

        // increment balls missed (when we transfer control over to player while it being on table)
        if (CueControllerUpdater.current_control_status == CueControllerUpdater.CUE_BALL_FIXED_ON_TABLE && inPlayerInControl && !inSkippable)
        {
            GameManager_script.IncrementShotsMissed(cueController.TrackingShotAsPlayerOne, cueController.TrackingShotAsPlayerTwo);
        }

        // show breakz pushout
        if (inFirstTime)
        {
            cueController.ShowHelpfulTooltipPopup("", "YourBreak", true, false);

            if (cueController.BotInControl() || cueController.NetworkSlaveInControl() || cueController.NetworkBotInControl()) // set for rematch breaks
            {
                GameManager_script.Instance().rematchYouAreThePrevBreaker = false;
            }
            else
            {
                GameManager_script.Instance().rematchYouAreThePrevBreaker = true;
            }
        }
    }

    IEnumerator UpdateStillPositions(int inShotCount)
    {
        while (cueController == null || cueController.stillBallPositions == null || cueController.stillBallPocketed == null || !cueController.allIsSleeping)
        {
            yield return null;
        }

        cueController.updateSlaveStillPositions(inShotCount);

        cueController.CheckForSnookerSelf();

        cueController.EndOfShotHouseKeeping();
    }

    public void OnPersonalInfoReceived(string inName, int inCue, int inAvatar, int starColor, int starNumber, float TPAScore, int gamesPlayed, int gamesWon, float inCoins, int inStreak)
    {
        StartCoroutine(PersonalInfoReceived(inName, inCue, inAvatar, starColor, starNumber, TPAScore, gamesPlayed, gamesWon, inCoins, inStreak));
    }

    IEnumerator PersonalInfoReceived(string inName, int inCue, int inAvatar, int starColor, int starNumber, float TPAScore, int gamesPlayed, int gamesWon, float inCoins, int inStreak)
    {
        while (cueController == null)
        {
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        // finally we get to populate info about the other player
        GameManager_script.Instance().PopulateOtherGameProfileInfo(inName, inCue, inAvatar, starColor, starNumber, TPAScore, gamesPlayed, gamesWon, inCoins, inStreak);

        // change player head
        cueController.gamecenter.GetComponent<GameCenter>().ChangeHead2Name();
        cueController.gamecenter.GetComponent<GameCenter>().ChangeHead2Image();

        // change star and other shiites
        cueController.gamecenter.GetComponent<GameCenter>().changeStarImage2(GameManager_script.Instance().otherGameProfileInfo.Star.starType);
        cueController.gamecenter.GetComponent<GameCenter>().changeStarText2("" + GameManager_script.Instance().otherGameProfileInfo.Star.text);
    }

    public void OnFriendPersonalInfoReceived(string inName, int inCue, int inAvatar, int starColor, int starNumber, float TPAScore, int gamesPlayed, int gamesWon, float inCoins, int inStreak, float inWager, int inTableTexture)
    {
        StartCoroutine(FriendPersonalInfoReceived(inName, inCue, inAvatar, starColor, starNumber, TPAScore, gamesPlayed, gamesWon, inCoins, inStreak, inWager, inTableTexture));
    }

    IEnumerator FriendPersonalInfoReceived(string inName, int inCue, int inAvatar, int starColor, int starNumber, float TPAScore, int gamesPlayed, int gamesWon, float inCoins, int inStreak, float inWager, int inTableTexture)
    {
        while (cueController == null)
        {
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        // finally we get to populate info about the other player
        GameManager_script.Instance().PopulateOtherGameProfileInfo(inName, inCue, inAvatar, starColor, starNumber, TPAScore, gamesPlayed, gamesWon, inCoins, inStreak);

        // change player head
        cueController.gamecenter.GetComponent<GameCenter>().ChangeHead2Name();
        cueController.gamecenter.GetComponent<GameCenter>().ChangeHead2Image();

        // change star and other shiites
        cueController.gamecenter.GetComponent<GameCenter>().changeStarImage2(GameManager_script.Instance().otherGameProfileInfo.Star.starType);
        cueController.gamecenter.GetComponent<GameCenter>().changeStarText2("" + GameManager_script.Instance().otherGameProfileInfo.Star.text);

        // see if we need to change up some shit
        if (GameManager_script.Instance().TableTextureIndex > inTableTexture)
        {
            // change table color
            GameReplaceTableTexture.SingleTon().SetTableTexture(inTableTexture);

            // refund excess wager
            GameManager_script.Instance().UpdateCoinCount(GameManager_script.Instance().CurrentWager - inWager);

            // reset current wager
            GameManager_script.Instance().CurrentWager = (float)inWager;
            GameManager_script.Instance().CurrentWagerLevel = (float)inTableTexture;

            // reset center label
            cueController.gamecenter.GetComponent<GameCenter>().ChangeCenterLabel("Red", GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().CurrentWager * 2.0f, "gamecoinz"));
        }
    }

    public void OnUpdateAllBallStatus(int inShotCount, float inTimeLapsed, bool inMandatory, bool[] inPocketed, Vector3[] inPosition, Vector3[] inVelocity, Vector3[] inAngular, Vector3 ballShotVelocity, Vector3 hitBallVelocity, Vector3 ballShotAngularVelocity)
    {
        if (cueController)
        {
            cueController.UpdateAllBallStatusFromNetwork(inShotCount, inTimeLapsed, inMandatory, inPocketed, inPosition, inVelocity, inAngular, ballShotVelocity, hitBallVelocity, ballShotAngularVelocity);
        }
    }

    public void OnUpdateCueControl(float cueNumber, bool isinHand, Vector3 physicalPosition, Quaternion localRotation, Vector3 localPosition, Vector3 rotationDisplacement) // i am using this for now, will change to tethering later
	{
		if(cueController)
        {
            cueController.SetCueControlFromNetwork(cueNumber, isinHand, physicalPosition, localRotation, localPosition, new Vector2(rotationDisplacement.x, rotationDisplacement.y));
        }
	}

    public void OnRematchCapabilityCheckReceived(string inKey)
    {
        if (inKey != "")
        {
            GameManager_script.Instance().rematchPasscodeKey = inKey;
        }
    }

    public void OnRematchConfirmationReceived(bool inWantToRematch)
    {
        if (inWantToRematch && GameManager_script.Instance().CoinCount >= GameManager_script.Instance().CurrentWager)
        {
            GameManager_script.Instance().rematchOppoWantToRematch = true;
            GameManager_script.Instance().rematchSmartBotSeries = GameManager_script.Instance().SmartBotInActionGame;

            if (GameManager_script.Instance().rematchSelfWantToRematch)
            {
                // it is time to hide the room for all other players forever
                GameManager_script.KillRoomAndDisconnect();

                // for photon view bug?
                GameManager_script.DestroyServerController();

                GameManager_script.Instance().rematchCurrentMatchIsRematch = true;
                GameManager_script.Instance().rematchSelfWantToRematch = false;
                GameManager_script.Instance().rematchOppoWantToRematch = false;

                GameManager_script.Instance().StartingOutAsANetWorkGame = true;
                GameManager_script.Instance().CurrentlyInANetWorkGame = true;
                GameManager_script.Instance().SmartBotInActionGame = false;
                GameManager_script.Instance().StupidBotInActionGame = false;
                GameManager_script.Instance().TrulySelfInActionGame = false;
                GameManager_script.Instance().FTUEInActionGame = false;

                GameManager_script.Instance().TableTextureIndex = (int)GameManager_script.Instance().CurrentWagerLevel;

                GameManager_script.Instance().PopulateSelfGameProfileInfo();

                GameManager_script.Instance().PopulateInterstitialStartScreen
                (
                    true,
                    GameManager_script.Instance().GetMaxTPAScore(),
                    GameManager_script.Instance().Total_Games_Played,
                    GameManager_script.Instance().Total_Games_Won,
                    GameManager_script.Instance().CoinCount - GameManager_script.Instance().CurrentWager,
                    GameManager_script.Instance().Current_Win_Streak
                );

                GameManager_script.Instance().resetSingleGameStats();

                GameManager_script.Instance().StartCoroutine(GameManager_script.Instance().PlayLookingForPlayerMusic());

                GameManager_script.Instance().PopupCurrentlyVisible = false;

                GameManager_script.Instance().NetworkGameSceneCurrentLoad = true;

                Application.LoadLevel("GameStart");
            }
            else
            {
                // display to self that opponent wants to rematch!
                GameManager_script.Instance().ChangeRematchToolTipAbsolutePath(true, "RematchYes");
            }
        }
        else
        {
            GameManager_script.Instance().rematchOppoWantToRematch = false;

            // show self that the other person don't want to rematch (only when we are waiting)
            GameManager_script.Instance().ChangeRematchToolTipAbsolutePath(true, "RematchNo");
        }
    }

    public void OnRematchPersonalInfoReceived(string inName, int inCue, int inAvatar, int starColor, int starNumber, float TPAScore, int gamesPlayed, int gamesWon, float inCoins, int inStreak)
    {
        StartCoroutine(RematchPersonalInfoReceived(inName, inCue, inAvatar, starColor, starNumber, TPAScore, gamesPlayed, gamesWon, inCoins, inStreak));
    }

    IEnumerator RematchPersonalInfoReceived(string inName, int inCue, int inAvatar, int starColor, int starNumber, float TPAScore, int gamesPlayed, int gamesWon, float inCoins, int inStreak)
    {
        while (cueController == null)
        {
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        // finally we get to populate info about the other player
        GameManager_script.Instance().PopulateOtherGameProfileInfo(inName, inCue, inAvatar, starColor, starNumber, TPAScore, gamesPlayed, gamesWon, inCoins, inStreak);

        // change player head
        cueController.gamecenter.GetComponent<GameCenter>().ChangeHead2Name();
        cueController.gamecenter.GetComponent<GameCenter>().ChangeHead2Image();

        // change star and other shiites
        cueController.gamecenter.GetComponent<GameCenter>().changeStarImage2(GameManager_script.Instance().otherGameProfileInfo.Star.starType);
        cueController.gamecenter.GetComponent<GameCenter>().changeStarText2("" + GameManager_script.Instance().otherGameProfileInfo.Star.text);

        // stringz ya...
        string centerLabelString = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().rematchYourWinCount, "number") + Localization.Get("SpacedMaoHao") + GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().rematchOppoWinCount, "number");

        // reset some middle shiite with the 1:0 or 0:1 shiite
        cueController.gamecenter.GetComponent<GameCenter>().ChangeCenterLabel("Red", centerLabelString);
    }

    public void OnChatMessageReceived(string inString)
    {
        if (cueController)
        {
            cueController.gamecenter.GetComponent<GameCenter>().ShowRiteChatBubble(inString);
        }
    }
}
