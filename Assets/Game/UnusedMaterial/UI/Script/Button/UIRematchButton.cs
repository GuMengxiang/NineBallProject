using UnityEngine;
using System.Collections;

public class UIRematchButton : MonoBehaviour
{
    [System.NonSerialized]
    public bool HasBeenClicked = false;

    void OnClick()
    {
        // music
        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);

        // set locking var
        GameManager_script.Instance().rematchSelfWantToRematch = true;
        GameManager_script.Instance().rematchSmartBotSeries = GameManager_script.Instance().SmartBotInActionGame;

        // do stuff
        if (GameManager_script.Instance().CoinCount >= GameManager_script.Instance().CurrentWager) // we have to have enough coins to set self to true, to send, and to rematch
        {
            if (!HasBeenClicked)
            {
                HasBeenClicked = true;

                if (GameManager_script.Instance().rematchSmartBotSeries)
                {
                    // check intent, and re-calculate if its fucked
                    if (!GameManager_script.Instance().rematchSmartBotWantToRematch)
                    {
                        GameManager_script.Instance().GenerateSmartBotIntentToRespondToRematch();
                    }

                    // for real this time
                    if (GameManager_script.Instance().rematchOppoWantToRematch)
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
                        // ya bot doesn't want to rematch
                        GameManager_script.Instance().ChangeRematchToolTipAbsolutePath(true, "RematchWait");

                        GameManager_script.Instance().rematchSmartBotClickPhase = 2; // we are ready to 

                        GameManager_script.Instance().StartCoroutine(GameManager_script.Instance().SmartBotRematchFakeButtonClick(Random.Range(GameManager_script.Instance().EndOfGameSmartBotDecideTimeLong * 0.40f, GameManager_script.Instance().EndOfGameSmartBotDecideTimeLong), GameManager_script.Instance().rematchSmartBotClickPhase));
                    }
                }
                else
                {
                    // send signal
                    if (ServerController.serverController)
                    {
                        ServerController.serverController.SendRPCToNetworkViewOthers("OnRematchConfirmationReceived", true);
                    }

                    // see if we should just go ahead and do it
                    if (GameManager_script.Instance().rematchOppoWantToRematch && GameManager_script.Instance().rematchPasscodeKey != "")
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
                        // display all the excuses or a waiting banner
                        GameManager_script.Instance().ChangeRematchToolTipAbsolutePath(true, "RematchWait");
                    }
                }
            }
        }
        else
        {
            // not enough coins
            GameManager_script.Instance().ChangeRematchToolTipAbsolutePath(true, "RematchNoCoin");
        }
    }
}
