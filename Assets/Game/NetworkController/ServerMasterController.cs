using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerMasterController : MonoBehaviour
{
    [System.NonSerialized]
    public bool WeAreLookingForNetWorkGamez = false;

    [System.NonSerialized]
    public bool DisplayAwesomeSearchText = false;
    [System.NonSerialized]
    public List<float> DisplayAwesomeSearchTextTimer;
    [System.NonSerialized]
    public int DisplayAwesomeSearchTextCounter = 0;

    [System.NonSerialized]
    public bool LookingForSmartBotGames = true;
    [System.NonSerialized]
    public bool LookingToRefundAfterNothingToPlayWith = true;    
    [System.NonSerialized]
    public bool RunningGameUpdates = false;
    [System.NonSerialized]
    public bool GameAlreadyStarted = false;

    [System.NonSerialized]
    public bool SmartRobotIsValidCandidate = false;
    [System.NonSerialized]
    public float SmartRobotKoolOffPeriod = 3.5f; // wait for 3 seconds after we find the smart bot

    public void Awake()
    {
        GameManager_script.Instance().TotalWaitForRoomTime = Time.realtimeSinceStartup;
        GameManager_script.Instance().TimeSpendPausingGame = Time.realtimeSinceStartup;
        GameManager_script.Instance().SmartBotFreezeDueToInternet = false;
        GameManager_script.Instance().SmartBotFreezeDueToFinish = false;
    }

    public void Update()
    {
        if (WeAreLookingForNetWorkGamez)
        {
            if (DisplayAwesomeSearchText)
            {
                if (DisplayAwesomeSearchTextCounter < DisplayAwesomeSearchTextTimer.Count && Time.realtimeSinceStartup - GameManager_script.Instance().TotalWaitForRoomTime > DisplayAwesomeSearchTextTimer[DisplayAwesomeSearchTextCounter] * GameManager_script.Instance().TotalWaitForRoomTimeLimit)
                {
                    DisplayAwesomeSearchTextCounter += 1;

                    if (PhotonNetwork.connected)
                    {
                        if (GameManager_script.Instance().rematchCurrentMatchIsRematch)
                        {
                            switch (DisplayAwesomeSearchTextCounter)
                            {
                                case 1: GameManager_script.Instance().connectionStatus = Localization.Get("RematchInterstitialLabel1"); break;
                                case 2: GameManager_script.Instance().connectionStatus = Localization.Get("RematchInterstitialLabel2"); break;
                            }
                        }
                        else if (GameManager_script.Instance().isEverythingFocusedOnFrdsSelector)
                        {
                            switch (DisplayAwesomeSearchTextCounter)
                            {
                                case 1: GameManager_script.Instance().connectionStatus = Localization.Get("SearchFrd1"); break;
                                case 2: GameManager_script.Instance().connectionStatus = Localization.Get("SearchFrd2"); break;
                                case 3: GameManager_script.Instance().connectionStatus = Localization.Get("SearchFrd3"); break;
                                case 4: GameManager_script.Instance().connectionStatus = Localization.Get("SearchFrd4"); break;
                                case 5: GameManager_script.Instance().connectionStatus = Localization.Get("SearchFrd5"); break;
                            }
                        }
                        else
                        {
                            switch (DisplayAwesomeSearchTextCounter)
                            {
                                case 1: GameManager_script.Instance().connectionStatus = Localization.Get("SearchBestMatch"); break;
                                case 2: GameManager_script.Instance().connectionStatus = Localization.Get("SearchLocalServer1"); break;
                                case 3: GameManager_script.Instance().connectionStatus = Localization.Get("SearchLocalServer2"); break;
                                case 4: GameManager_script.Instance().connectionStatus = Localization.Get("SearchLocalServer3"); break;
                                case 5: GameManager_script.Instance().connectionStatus = Localization.Get("SearchLocalServer4"); break;
                                case 6: GameManager_script.Instance().connectionStatus = Localization.Get("SearchGlobalServer"); break;
                            }
                        }

                        // set rotating offset
                        LoadingText.charOffset = 4;

                        // just go ahead and kill the exit button all together for now
                        if (transform && transform.parent && transform.parent.GetComponentInChildren<UIInterstitialCenter>())
                        {
                            transform.parent.GetComponentInChildren<UIInterstitialCenter>().ChangeExitButton(false);
                        }
                    }
                    else
                    {
                        GameManager_script.Instance().connectionStatus = Localization.Get("SearchReconnect");
                        LoadingText.charOffset = 4;

                        // just go ahead and kill the exit button all together for now
                        if (transform && transform.parent && transform.parent.GetComponentInChildren<UIInterstitialCenter>())
                        {
                            transform.parent.GetComponentInChildren<UIInterstitialCenter>().ChangeExitButton(true);
                        }
                    }
                }
            }

            if (LookingForSmartBotGames)
            {
                float randomConnectionValue = (GameManager_script.Instance().Total_Games_Played == 0.0f ? 0.5f : 1.0f) * Random.Range(0.0f, 1.0f); // if first game, we find dudes faster
                float chanceOfSucceeding = GameManager_script.Instance().SmartBotChanceOfFinding.Evaluate(Mathf.Clamp01((Time.realtimeSinceStartup - GameManager_script.Instance().TotalWaitForRoomTime) / GameManager_script.Instance().TotalWaitForRoomTimeLimit));

                if (randomConnectionValue < Time.deltaTime * chanceOfSucceeding && PhotonNetwork.connected)
                {
                    // this doesn't belong to the other 4 bools, this is a special case bool
                    SmartRobotIsValidCandidate = true;

                    // make sure other parts of the game don't fuck up
                    LookingForSmartBotGames = false;
                    LookingToRefundAfterNothingToPlayWith = false;
                    RunningGameUpdates = false;
                    GameAlreadyStarted = false;

                    // if we are "looking for players" we need to stop all further advertisement and wait for a kool off period. "looking for room", "creating room" and "connecting to lobby" are taken care of elsewhere
                    if (PhotonNetwork.connected && PhotonNetwork.inRoom)
                    {
						PhotonNetwork.room.IsOpen = false;
						PhotonNetwork.room.IsVisible = false;
						PhotonNetwork.room.MaxPlayers = 2; // don't need to room
                    }

                    // wait x seconds for everything to clean out
                    StartCoroutine(WaitForSomeTimeBeforeRunningSmartBotInit());
                }
            }

            if (LookingToRefundAfterNothingToPlayWith)
            {
                // total refund when we didn't find anyone
                if (Time.realtimeSinceStartup - GameManager_script.Instance().TotalWaitForRoomTime > GameManager_script.Instance().TotalWaitForRoomTimeLimit)
                {
                    SwitchToStupidRefundModeAfterCannotFindBitchFrdz();
                }
            }

            if (RunningGameUpdates)
            {
                // everyone needs thizzzz
                CueController cc = ServerController.serverController ? ServerController.serverController.serverMessenger.cueController : null;

                if (!GameManager_script.Instance().SmartBotInActionGame)
                {
                    if (ServerController.serverController)
                    {
                        // real game pop the msg whenever we stop doing messages
                        if (cc != null)
                        {
                            if (Time.realtimeSinceStartup - ServerController.serverController.lastMessageTimeStamp > GameManager_script.Instance().PutUpBannerAndWarnPlayerTime)
                            {
                                // last signal over 3 seconds ago, if this is no active banner, erect it
                                if (!(cc.GC.ToolTipCurrentActive && cc.GC.ToolTipCurrentType == "help"))
                                {
                                    cc.ShowHelpfulTooltipPopup("", "Connecting", false, false);
                                }
                            }
                            else
                            {
                                // last signal less than 3 seconds, if the current banner IS connection banner, remove it
                                if (cc.GC.ToolTipCurrentActive && cc.GC.ToolTipCurrentType == "help" && cc.GC.ToolTipMainText == Localization.Get("Connecting"))
                                {
                                    cc.GC.CleanUpAllToolTipRelatedUI();
                                }
                            }
                        }

                        // real game we need to leave now...
                        if (Time.realtimeSinceStartup - ServerController.serverController.lastMessageTimeStamp > GameManager_script.Instance().TotalWaitForResponseTime)
                        {
                            // if you are playing a real real real game, then shuts up when we gg
                            SwitchToStupidRefundModeAfterDisconnect();
                        }
                    }
                }
                else
                {
                    // gotta have the cc'z
                    if (cc != null)
                    {
                        // smart bot gotta pause the game when we stuck
                        if (cc.NetworkBotInControl() && !PhotonNetwork.connected) // dajiang hack, as soon as we fuck up
                        {
                            // network is no good and bot is in control, we wait
                            GameManager_script.Instance().SmartBotFreezeDueToInternet = true;
                        }
                        else
                        {
                            // this is always good
                            GameManager_script.Instance().SmartBotFreezeDueToInternet = false;
                        }

                        // smart bot game pop the msg whenever internet is slowing down
                        if (cc.NetworkBotInControl() && (!PhotonNetwork.connected && GameManager_script.Instance().timeSinceDisconnected > GameManager_script.Instance().timeToDisplayReconnectBanner)) // dajiang hack, this is like 3 seconds
                        {
                            // erect the banner only when we are stuck under botzzz
                            if (cc.NetworkBotInControl() && !(cc.GC.ToolTipCurrentActive && cc.GC.ToolTipCurrentType == "help"))
                            {
                                cc.ShowHelpfulTooltipPopup("", "Connecting", false, false);
                            }
                        }
                        else
                        {
                            // last signal less than 3 seconds, if the current banner IS connection banner, remove it
                            if (cc.GC.ToolTipCurrentActive && cc.GC.ToolTipCurrentType == "help" && cc.GC.ToolTipMainText == Localization.Get("Connecting"))
                            {
                                cc.GC.CleanUpAllToolTipRelatedUI();
                            }
                        }

                        // smart bot gotta leave game now...
                        if (cc.NetworkBotInControl() && (!PhotonNetwork.connected && GameManager_script.Instance().timeSinceDisconnected > GameManager_script.Instance().timeToBreakOffConnection)) // this is a prolonged disconnection
                        {
                            // if you are playing a real real real game, then shuts up when we gg
                            SwitchToStupidRefundModeAfterDisconnect();
                        }
                    }
                }

                // all kill fail safe (when we turn off phone for X seconds or something)
                if (Time.realtimeSinceStartup - GameManager_script.Instance().TimeSpendPausingGame > GameManager_script.Instance().TimeSpendPausingGameLimit)
                {
                    // immediately get out of the thing no matter which client you are in, coz you paused for too long and your opponent is "gone"
                    SwitchToStupidRefundModeAfterDisconnect();
                }

                // all kill fail safe update
                GameManager_script.Instance().TimeSpendPausingGame = Time.realtimeSinceStartup;
            }
        }
    }

    public void SearchTextTimerHelperFunction()
    {
        // clean up?
        DisplayAwesomeSearchTextCounter = 0;

        // do fuckin' search text thing
        DisplayAwesomeSearchTextTimer = new List<float>(0);
        DisplayAwesomeSearchTextTimer.Add(0.0f);

        // vars
        float individualTime = 0.0f;

        if (GameManager_script.Instance().rematchCurrentMatchIsRematch)
        {
            // give them some probabilities (1 is just a hard code)
            for (int i = 0; i < 1; i++)
            {
                individualTime += 0.20f;

                DisplayAwesomeSearchTextTimer.Add(individualTime);
            }
        }
        else if (GameManager_script.Instance().isEverythingFocusedOnFrdsSelector)
        {
            // give them some probabilities (4 is just a hard code)
            for (int i = 0; i < 4; i++)
            {
                individualTime += 0.20f;

                DisplayAwesomeSearchTextTimer.Add(individualTime);
            }
        }
        else
        {
            // give them some probabilities (5 is just a hard code)
            for (int i = 0; i < 5; i++)
            {
                individualTime += Random.Range(0.06f, 0.18f);

                DisplayAwesomeSearchTextTimer.Add(individualTime);
            }
        }

        // gotta hold the place...
        GameManager_script.Instance().connectionStatus = Localization.Get("SearchBestMatch");
        LoadingText.charOffset = 4;

        // ok we ready now
        DisplayAwesomeSearchText = true;
    }

    public IEnumerator WaitForSomeTimeBeforeRunningSmartBotInit()
    {
        // random short time waiting for bitchz to kool off
        yield return new WaitForSeconds(SmartRobotKoolOffPeriod);

        // if no one else started a (real) game and network is sort of up, we can start gaming. we cannot "wait" for network to become good, what if its never good and we already killed LookingToRefundAfterNothingToPlayWith.
        if (!GameAlreadyStarted)
        {
            // system is clean, nothing joined, nothing is interfering, we can start a good bot game (else do nothing)
            GameManager_script.Instance().StartingOutAsANetWorkGame = true;
            GameManager_script.Instance().CurrentlyInANetWorkGame = false;
            GameManager_script.Instance().SmartBotInActionGame = true;
            GameManager_script.Instance().StupidBotInActionGame = false;
            GameManager_script.Instance().TrulySelfInActionGame = false;
            GameManager_script.Instance().FTUEInActionGame = false;

            GameManager_script.Instance().PopulateSelfGameProfileInfo();

            // we need to build a smart profile for these bots
            string opponentName = GameManager_script.SmartBotPlayerNameArray[Random.Range(0, GameManager_script.SmartBotPlayerNameArray.Length)];
            float TPAScore = GameManager_script.Instance().GenerateSmartBotTPAScore();
            int level = GameManager_script.Instance().GenerateSmartBotLevel();
            int cueIndex = GameManager_script.Instance().GenerateCueIndex();
            int avatarIndex = GameManager_script.Instance().GenerateAvatarIndex();
            int starColor = GameManager_script.Instance().GetBotStarColor(TPAScore);
            int gamesPlayed = GameManager_script.Instance().GenerateTotalGamesPlayed();
            int gamesWon = GameManager_script.Instance().GenerateTotalGamesWon();
            float coinz = GameManager_script.Instance().GenerateCoinCount();
            int streakz = GameManager_script.Instance().GenerateTotalStreak();

            // populate them in game fields
            GameManager_script.Instance().PopulateOtherGameProfileInfo(opponentName, cueIndex, avatarIndex, starColor, level, TPAScore, gamesPlayed, gamesWon, coinz, streakz);

            // put in table texture
            GameManager_script.Instance().TableTextureIndex = (int)GameManager_script.Instance().CurrentWagerLevel;

            // assign control
            ServerController.serverController.playerInControl = GameManager_script.DetermineLotteryResult(0.50f) ? true : false;

            // i am listening for stuff, hmm... stuff....
            ServerController.serverController.lastMessageTimeStamp = Time.realtimeSinceStartup;

            // DETERMINE if we want to slay some playerz (after we definitively deducted the coinz)
            GameManager_script.Instance().GenerateOverallIntentOnThisGame();

            // at this point, true anyways dajiang data 2
            ServerController.serverController.NetworkGameStarted = true;

            // deduct coinz from player
            GameManager_script.Instance().UpdateCoinCount(-1.0f * GameManager_script.Instance().CurrentWager);

            // exit from the network
            GameManager_script.KillRoomAndDisconnect();

            GameManager_script.Instance().StartCoroutine(GameManager_script.Instance().KillLookingForPlayerMusicAndPlayFoundOppoDing());

            GameManager_script.Instance().TimeSpendPausingGame = Time.realtimeSinceStartup;

            GameManager_script.Instance().timeSinceDisconnected = 0.0f;

            LookingForSmartBotGames = false;
            LookingToRefundAfterNothingToPlayWith = false;
            RunningGameUpdates = true;
            GameAlreadyStarted = true;

            // loading content
            GameManager_script.Instance().connectionStatus = GameManager_script.Instance().ShowLoadGameScreen(true);
            LoadingText.charOffset = 4;

            // disable text updating flag
            DisplayAwesomeSearchText = false;

            // load game, if you want delays do them here
            MenuControllerGenerator.controller.LoadLevel("Game");
        }
    }

    public IEnumerator WaitForSomeTimeBeforeRunningSmartBotRematchInit()
    {
        // random short time waiting for bitchz to kool off
        yield return new WaitForSeconds(SmartRobotKoolOffPeriod);

        // if no one else started a (real) game and network is sort of up, we can start gaming. we cannot "wait" for network to become good, what if its never good and we already killed LookingToRefundAfterNothingToPlayWith.
        if (!GameAlreadyStarted)
        {
            // system is clean, nothing joined, nothing is interfering, we can start a good bot game (else do nothing)
            GameManager_script.Instance().StartingOutAsANetWorkGame = true;
            GameManager_script.Instance().CurrentlyInANetWorkGame = false;
            GameManager_script.Instance().SmartBotInActionGame = true;
            GameManager_script.Instance().StupidBotInActionGame = false;
            GameManager_script.Instance().TrulySelfInActionGame = false;
            GameManager_script.Instance().FTUEInActionGame = false;

            GameManager_script.Instance().PopulateSelfGameProfileInfo();

            // preps
            float TPAPercentage = Random.Range(0.010f, 0.050f); // 1.0% to 5.0%
            bool ShouldAddLevel = (GameManager_script.DetermineLotteryResult(0.04f) && !GameManager_script.Instance().rematchSmartBotLevelAdded);
            GameManager_script.Instance().rematchSmartBotLevelAdded = ShouldAddLevel ? true : GameManager_script.Instance().rematchSmartBotLevelAdded;

            // we need to build a smart profile for these bots
            string opponentName = GameManager_script.Instance().otherGameProfileInfo.PlayerName; // done
            float TPAScore = TPAPercentage * GameManager_script.Instance().rematchPrevTPAScore + (1.0f - TPAPercentage) * GameManager_script.Instance().otherGameProfileInfo.TPAScore; // done
            int level = ShouldAddLevel ? (GameManager_script.Instance().otherGameProfileInfo.Star.text + 1) : (GameManager_script.Instance().otherGameProfileInfo.Star.text); // done
            int cueIndex = (int)GameManager_script.Instance().otherGameProfileInfo.cueEquipped; // done
            int avatarIndex = (int)GameManager_script.Instance().otherGameProfileInfo.HeadImage; // done
            int starColor = GameManager_script.Instance().GetBotStarColor(TPAScore); // done
            int gamesPlayed = (int)GameManager_script.Instance().otherGameProfileInfo.gamesPlayed + 1; // done
            int gamesWon = (int)GameManager_script.Instance().otherGameProfileInfo.gamesWon + (GameManager_script.Instance().rematchYouAreThePrevWinner ? 0 : 1); // done
            float coinz = GameManager_script.Instance().GenerateCoinCount(); // done, this is not shown anyways
            int streakz = (int)GameManager_script.Instance().otherGameProfileInfo.streak + (GameManager_script.Instance().rematchYouAreThePrevWinner ? -(int)GameManager_script.Instance().otherGameProfileInfo.streak : 1); // done

            // populate them in game fields
            GameManager_script.Instance().PopulateOtherGameProfileInfo(opponentName, cueIndex, avatarIndex, starColor, level, TPAScore, gamesPlayed, gamesWon, coinz, streakz);

            // put in table texture
            GameManager_script.Instance().TableTextureIndex = (int)GameManager_script.Instance().CurrentWagerLevel;

            // assign control
            ServerController.serverController.playerInControl = !GameManager_script.Instance().rematchYouAreThePrevBreaker;

            // i am listening for stuff, hmm... stuff....
            ServerController.serverController.lastMessageTimeStamp = Time.realtimeSinceStartup;

            // DETERMINE if we want to slay some playerz (after we definitively deducted the coinz)
            GameManager_script.Instance().GenerateOverallIntentOnThisGame();

            // at this point, true anyways dajiang data 2
            ServerController.serverController.NetworkGameStarted = true;

            // deduct coinz from player
            GameManager_script.Instance().UpdateCoinCount(-1.0f * GameManager_script.Instance().CurrentWager);

            // exit from the network
            GameManager_script.KillRoomAndDisconnect();

            GameManager_script.Instance().StartCoroutine(GameManager_script.Instance().KillLookingForPlayerMusicAndPlayFoundOppoDing());

            GameManager_script.Instance().TimeSpendPausingGame = Time.realtimeSinceStartup;

            GameManager_script.Instance().timeSinceDisconnected = 0.0f;

            LookingForSmartBotGames = false;
            LookingToRefundAfterNothingToPlayWith = false;
            RunningGameUpdates = true;
            GameAlreadyStarted = true;

            // loading content
            GameManager_script.Instance().connectionStatus = GameManager_script.Instance().ShowLoadGameScreen(true);
            LoadingText.charOffset = 4;

            // disable text updating flag
            DisplayAwesomeSearchText = false;

            // load game, if you want delays do them here
            MenuControllerGenerator.controller.LoadLevel("Game");
        }
    }

    // this function is when player disconnected from the game
    // this is for BOTH smart bot internet thing and real player photon thing
    public void SwitchToStupidRefundModeAfterDisconnect()
    {
        CueController cc = ServerController.serverController ? ServerController.serverController.serverMessenger.cueController : null;

        if (cc && !cc.LoadMainMenuAlreadyCalled)
        {
            bool yourFault = true;

            // it is always your fault if its a smart bot game
            if (GameManager_script.Instance().SmartBotInActionGame)
            {
                yourFault = true;
            }
            
            if (GameManager_script.Instance().CurrentlyInANetWorkGame)
            {
                // if its a real real game and you are not in your room, its your fault
                yourFault = PhotonNetwork.connected && PhotonNetwork.inRoom ? false : true;
            }

            if (yourFault) // if you are not in the room, this means you fucked up
            {
                GameManager_script.Instance().UpdateCoinCount(0.0f);

                cc.ShowHelpfulTooltipPopup(Localization.Get("YourSelf"), "Disconnected", true, true);

                StartCoroutine(cc.OnLoadMainMenu(false, true, 0, false, true, false));
            }
            else // you have to be in a room to prove that you didn't fuck up
            {
                GameManager_script.Instance().UpdateCoinCount(2.0f * GameManager_script.Instance().CurrentWager);

                cc.ShowHelpfulTooltipPopup(Localization.Get("YourOpponent"), "Disconnected", true, true);

                StartCoroutine(cc.OnLoadMainMenu(true, true, 0, true, false, false));
            }

            LookingForSmartBotGames = false;
            LookingToRefundAfterNothingToPlayWith = false;
            RunningGameUpdates = false;
            GameAlreadyStarted = false;
        }
    }

    // we refund the players if we cannot find any one to play with during the 20 something seconds period
    // actuall no refund anymore since we don't deduct up front
    public void SwitchToStupidRefundModeAfterCannotFindBitchFrdz()
    {
        if (ServerController.serverController && !ServerController.serverController.NetworkGameStarted)
        {
            // clean up selector profile stuff
            GameManager_script.Instance().selfGameProfileInfo = null;
            GameManager_script.Instance().otherGameProfileInfo = null;

            // clean up rematch consts
            GameManager_script.Instance().CleanUpRematchConsts();

            // send signal to friend saying we don't want to rematch
            ServerController.serverController.SendRPCToNetworkViewOthers("OnRematchConfirmationReceived", false);

            // exit from the network
            GameManager_script.KillRoomAndDisconnect();

            // destroy server controller
            GameManager_script.DestroyServerController();

            // special kill self lolz
            gameObject.SetActive(false);

            // disables menu whatever
            MenuControllerGenerator.controller.gameObject.SetActive(false);

            // kill music
            GameManager_script.Instance().StartCoroutine(GameManager_script.Instance().KillLookingForPlayerMusicAndPlayFoundOppoDing(false));

            // clear popup
            GameManager_script.Instance().PopupCurrentlyVisible = false;

            // we are not in any sort of network game no more
            GameManager_script.Instance().NetworkGameSceneCurrentLoad = false;

            // exits and load main menu
            Application.LoadLevel("NGUI_MENU");
        }
    }

    // if we are not part of lobby initially, this will be eventually called.
    // we should try to join a random game when this happens (when we are in lobby)
    public void OnJoinedLobby()
    {
        if (!GameAlreadyStarted && !SmartRobotIsValidCandidate && WeAreLookingForNetWorkGamez)
        {
            StartCoroutine(WaitForSomeTimeBeforeSearchForRoom());
        }
    }

    public IEnumerator WaitForSomeTimeBeforeSearchForRoom()
    {
        if (GameManager_script.Instance().rematchCurrentMatchIsRematch)
        {
            if (GameManager_script.Instance().rematchSmartBotSeries)
            {
                // just start a fake smart bot game right here
                StartCoroutine(WaitForSomeTimeBeforeRunningSmartBotRematchInit());
            }
            else
            {
                // wait for a period with a wider span
                yield return new WaitForSeconds(Random.Range(0.0f, 2.0f));

                // the limit is an arbitrary number, search for room if the ping is OK, otherwise don't even start any of the process
                if (PhotonNetwork.GetPing() < GameManager_script.Instance().targetPingHigherLimit)
                {
					RoomOptions roomOptions = new RoomOptions() { IsVisible = false, MaxPlayers = 2 };

                    PhotonNetwork.JoinOrCreateRoom(GameManager_script.Instance().rematchPasscodeKey, roomOptions, TypedLobby.Default);

                    ServerController.serverController.playerInControl = !GameManager_script.Instance().rematchYouAreThePrevBreaker;
                }
            }
        }
        else if (GameManager_script.Instance().isEverythingFocusedOnFrdsSelector)
        {
            // wait for a period with a wider span
            yield return new WaitForSeconds(Random.Range(0.0f, 2.0f));

            // the limit is an arbitrary number, search for room if the ping is OK, otherwise don't even start any of the process
            if (PhotonNetwork.GetPing() < GameManager_script.Instance().targetPingHigherLimit)
            {
				RoomOptions roomOptions = new RoomOptions() { IsVisible = false, MaxPlayers = 2 };

                PhotonNetwork.JoinOrCreateRoom(GameManager_script.Instance().SelectorFrdKey, roomOptions, TypedLobby.Default);

                ServerController.serverController.playerInControl = GameManager_script.DetermineLotteryResult(0.50f) ? true : false;
            }
        }
        else
        {
            // random short time waiting for bitchz to kool off
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

            // the limit is an arbitrary number, search for room if the ping is OK, otherwise don't even start any of the process
            if (PhotonNetwork.GetPing() < GameManager_script.Instance().targetPingHigherLimit)
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }
    }

    // this is when we join a room, if we are master client, we wait
    // if we are slave client, we prepare to start the game
    public void OnJoinedRoom()
    {
        if (GameManager_script.Instance().isEverythingFocusedOnFrdsSelector || GameManager_script.Instance().rematchCurrentMatchIsRematch)
        {
            if (PhotonNetwork.connected && PhotonNetwork.inRoom)
            {
				PhotonNetwork.room.IsOpen = true; // we do want the rooms to stay open
				PhotonNetwork.room.IsVisible = false;
				PhotonNetwork.room.MaxPlayers = 2;
            }
        }

        if (PhotonNetwork.isMasterClient)
        {
            if (GameAlreadyStarted || SmartRobotIsValidCandidate)
            {
                GameManager_script.KillRoomAndDisconnect();
            }
        }
        else
        {
            if (GameAlreadyStarted)
            {
                GameManager_script.KillRoomAndDisconnect();
            }
            else
            {
                // i am listening for stuff, hmm... stuff....
                ServerController.serverController.lastMessageTimeStamp = Time.realtimeSinceStartup;

                // dajiang data 2, slave is commited at this point (cannot voluntarily turn back) and this is also the checkpoint (doesn't get here, refunds)
                ServerController.serverController.NetworkGameStarted = true;

                // deduct coinz from player
                GameManager_script.Instance().UpdateCoinCount(-1.0f * GameManager_script.Instance().CurrentWager);

                if (GameManager_script.Instance().rematchCurrentMatchIsRematch)
                {
                    // send personal information to the other dude
                    ServerController.serverController.SendRPCToNetworkViewOthers("OnRematchPersonalInfoReceived",
                        (string)GameManager_script.Instance().First_Name,
                        (int)GameManager_script.Instance().CueEquipped,
                        (int)GameManager_script.Instance().AvatarEquipped,
                        (int)GameManager_script.Instance().GetSelfStarType(),
                        (int)GameManager_script.Instance().CurrentLevel,
                        (float)GameManager_script.Instance().GetMaxTPAScore(),
                        (int)GameManager_script.Instance().Total_Games_Played,
                        (int)GameManager_script.Instance().Total_Games_Won,
                        (float)GameManager_script.Instance().CoinCount,
                        (int)GameManager_script.Instance().Current_Win_Streak
                        );
                }
                else if (GameManager_script.Instance().isEverythingFocusedOnFrdsSelector)
                {
                    // send personal information to the other dude
                    ServerController.serverController.SendRPCToNetworkViewOthers("OnFriendPersonalInfoReceived",
                        (string)GameManager_script.Instance().First_Name,
                        (int)GameManager_script.Instance().CueEquipped,
                        (int)GameManager_script.Instance().AvatarEquipped,
                        (int)GameManager_script.Instance().GetSelfStarType(),
                        (int)GameManager_script.Instance().CurrentLevel,
                        (float)GameManager_script.Instance().GetMaxTPAScore(),
                        (int)GameManager_script.Instance().Total_Games_Played,
                        (int)GameManager_script.Instance().Total_Games_Won,
                        (float)GameManager_script.Instance().CoinCount,
                        (int)GameManager_script.Instance().Current_Win_Streak,
                        (float)GameManager_script.Instance().CurrentWager,
                        (int)GameManager_script.Instance().TableTextureIndex
                        );
                }
                else
                {
                    // send personal information to the other dude
                    ServerController.serverController.SendRPCToNetworkViewOthers("OnPersonalInfoReceived",
                        (string)GameManager_script.Instance().First_Name,
                        (int)GameManager_script.Instance().CueEquipped,
                        (int)GameManager_script.Instance().AvatarEquipped,
                        (int)GameManager_script.Instance().GetSelfStarType(),
                        (int)GameManager_script.Instance().CurrentLevel,
                        (float)GameManager_script.Instance().GetMaxTPAScore(),
                        (int)GameManager_script.Instance().Total_Games_Played,
                        (int)GameManager_script.Instance().Total_Games_Won,
                        (float)GameManager_script.Instance().CoinCount,
                        (int)GameManager_script.Instance().Current_Win_Streak
                        );
                }

                // i HAZ connected, it is time to hide the room for all other players forever
                if (PhotonNetwork.connected && PhotonNetwork.inRoom)
                {
                    PhotonNetwork.room.open = false;
                    PhotonNetwork.room.visible = false;
                    PhotonNetwork.room.maxPlayers = 2;
                }

                GameManager_script.Instance().StartCoroutine(GameManager_script.Instance().KillLookingForPlayerMusicAndPlayFoundOppoDing());

                GameManager_script.Instance().TimeSpendPausingGame = Time.realtimeSinceStartup;

                GameManager_script.Instance().timeSinceDisconnected = 0.0f;

                LookingForSmartBotGames = false;
                LookingToRefundAfterNothingToPlayWith = false;
                RunningGameUpdates = true;
                GameAlreadyStarted = true;

                // disable text updating flag
                DisplayAwesomeSearchText = false;

                // loading content
                GameManager_script.Instance().connectionStatus = GameManager_script.Instance().ShowLoadGameScreen(true);
                LoadingText.charOffset = 4;

                // load game, if you want delays do them here
                MenuControllerGenerator.controller.LoadLevel("Game");
            }
        }
    }

    // as soon as we cannot find a game online, we should switch to hosting immediately
    // this is because we want as MANY hosts as soon as possible
    public void OnPhotonRandomJoinFailed()
    {
        // we can move onto creating rooms now
        if (!GameAlreadyStarted && !SmartRobotIsValidCandidate)
        {
            PhotonNetwork.CreateRoom(null, new RoomOptions() { isVisible = true, isOpen = true, maxPlayers = 2 }, null);

            ServerController.serverController.playerInControl = GameManager_script.DetermineLotteryResult(0.50f) ? true : false;
        }
    }
    
    // if a player connected to your photon room, try to start the game
    // but if the game is already started, kick this unwanted player of course
    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        if (!GameAlreadyStarted)
        {
            // dajiang data 1, master's checkpoint (doesn't get here, refunds) is here, different from the point he or she is commited
            ServerController.serverController.NetworkGameStarted = true;

            // deduct coinz from player
            GameManager_script.Instance().UpdateCoinCount(-1.0f * GameManager_script.Instance().CurrentWager);

            // This should be a one stop shop for kick starting the client, so want to change it to no quotation marks...
            StopCoroutine("SendInitialSignalWhenReady");
            StartCoroutine("SendInitialSignalWhenReady");

            if (GameManager_script.Instance().rematchCurrentMatchIsRematch)
            {
                // send personal information to the other dude
                ServerController.serverController.SendRPCToNetworkViewOthers("OnRematchPersonalInfoReceived",
                    (string)GameManager_script.Instance().First_Name,
                    (int)GameManager_script.Instance().CueEquipped,
                    (int)GameManager_script.Instance().AvatarEquipped,
                    (int)GameManager_script.Instance().GetSelfStarType(),
                    (int)GameManager_script.Instance().CurrentLevel,
                    (float)GameManager_script.Instance().GetMaxTPAScore(),
                    (int)GameManager_script.Instance().Total_Games_Played,
                    (int)GameManager_script.Instance().Total_Games_Won,
                    (float)GameManager_script.Instance().CoinCount,
                    (int)GameManager_script.Instance().Current_Win_Streak
                    );
            }
            else if (GameManager_script.Instance().isEverythingFocusedOnFrdsSelector)
            {
                // send personal information to the other dude
                ServerController.serverController.SendRPCToNetworkViewOthers("OnFriendPersonalInfoReceived",
                    (string)GameManager_script.Instance().First_Name,
                    (int)GameManager_script.Instance().CueEquipped,
                    (int)GameManager_script.Instance().AvatarEquipped,
                    (int)GameManager_script.Instance().GetSelfStarType(),
                    (int)GameManager_script.Instance().CurrentLevel,
                    (float)GameManager_script.Instance().GetMaxTPAScore(),
                    (int)GameManager_script.Instance().Total_Games_Played,
                    (int)GameManager_script.Instance().Total_Games_Won,
                    (float)GameManager_script.Instance().CoinCount,
                    (int)GameManager_script.Instance().Current_Win_Streak,
                    (float)GameManager_script.Instance().CurrentWager,
                    (int)GameManager_script.Instance().TableTextureIndex
                    );
            }
            else
            {
                // send personal information to the other dude
                ServerController.serverController.SendRPCToNetworkViewOthers("OnPersonalInfoReceived",
                    (string)GameManager_script.Instance().First_Name,
                    (int)GameManager_script.Instance().CueEquipped,
                    (int)GameManager_script.Instance().AvatarEquipped,
                    (int)GameManager_script.Instance().GetSelfStarType(),
                    (int)GameManager_script.Instance().CurrentLevel,
                    (float)GameManager_script.Instance().GetMaxTPAScore(),
                    (int)GameManager_script.Instance().Total_Games_Played,
                    (int)GameManager_script.Instance().Total_Games_Won,
                    (float)GameManager_script.Instance().CoinCount,
                    (int)GameManager_script.Instance().Current_Win_Streak
                    );
            }

            // the other player has connected, it is time to hide the room for all other players forever
            if (PhotonNetwork.connected && PhotonNetwork.inRoom)
            {
                PhotonNetwork.room.open = false;
                PhotonNetwork.room.visible = false;
                PhotonNetwork.room.maxPlayers = 2;
            }

            // send rematch info over if this is NOT already a rematched event
            if (!GameManager_script.Instance().rematchCurrentMatchIsRematch)
            {
                GameManager_script.Instance().rematchPasscodeKey = GameManager_script.Instance().CreateRandomUniqueKey();

                if (ServerController.serverController)
                {
                    ServerController.serverController.SendRPCToNetworkViewOthers("OnRematchCapabilityCheckReceived", GameManager_script.Instance().rematchPasscodeKey);
                }
            }

            GameManager_script.Instance().StartCoroutine(GameManager_script.Instance().KillLookingForPlayerMusicAndPlayFoundOppoDing());

            GameManager_script.Instance().TimeSpendPausingGame = Time.realtimeSinceStartup;

            GameManager_script.Instance().timeSinceDisconnected = 0.0f;

            LookingForSmartBotGames = false;
            LookingToRefundAfterNothingToPlayWith = false;
            RunningGameUpdates = true;
            GameAlreadyStarted = true;

            // disable text updating flag
            DisplayAwesomeSearchText = false;

            // loading content
            GameManager_script.Instance().connectionStatus = GameManager_script.Instance().ShowLoadGameScreen(true);
            LoadingText.charOffset = 4;

            // this is loading the game, we can pre-load just not show, if you want delays do them here
            MenuControllerGenerator.controller.LoadLevel("Game");
        }
        else
        {
            // kick this person immediately if you are the master
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.CloseConnection(player);
            }
        }
    }

    // this function helps start the game and update positions in the client
    // this function is good
    public IEnumerator SendInitialSignalWhenReady()
    {
        while (!ServerController.serverController || !ServerController.serverController.serverMessenger)
        {
            yield return null;
        }

        while (!ServerController.serverController.serverMessenger.cueController)
        {
            yield return null;
        }

        while (ServerController.serverController.serverMessenger.cueController.stillBallPocketed == null || ServerController.serverController.serverMessenger.cueController.stillBallPositions == null)
        {
            yield return null;
        }

        CueController cc = ServerController.serverController.serverMessenger.cueController;

        ServerController.serverController.SendRPCToNetworkViewOthers("OnStatusUpdateStillPositions", cc.shotCount, !ServerController.serverController.playerInControl, false, false, CueControllerUpdater.current_control_status, cc.stillBallPocketed, cc.stillBallPositions, true);

        cc.ShowHelpfulTooltipPopup("", "YourBreak", true, false);

        if (cc.BotInControl() || cc.NetworkSlaveInControl() || cc.NetworkBotInControl()) // set for rematch breaks
        {
            GameManager_script.Instance().rematchYouAreThePrevBreaker = false;
        }
        else
        {
            GameManager_script.Instance().rematchYouAreThePrevBreaker = true;
        }
    }

    public IEnumerator DelayStartStupidBotOrSoloGame()
    {
        yield return new WaitForSeconds(GameManager_script.Instance().FTUEInActionGame ? GameManager_script.Instance().StartOfFtueScreenTime : GameManager_script.Instance().StartOfGameScreenTime);

        // exit from the network
        GameManager_script.KillRoomAndDisconnect();

        // kill server part
        GameManager_script.DestroyServerController();

        // sleep self
        gameObject.SetActive(false);

        // load level as solo, if you want delays do them here
        MenuControllerGenerator.controller.LoadLevel("Game"); // this is loading the game
    }

    public void OnEnable()
    {
        if (GameManager_script.Instance().StartingOutAsANetWorkGame)
        {
            // specifically declare we are indeed looking for anything on the network
            WeAreLookingForNetWorkGamez = true;

            // specifically declare if we are looking for bots games
            LookingForSmartBotGames = !(GameManager_script.Instance().isEverythingFocusedOnFrdsSelector || GameManager_script.Instance().rematchCurrentMatchIsRematch);

            // set time limit
            GameManager_script.Instance().TotalWaitForRoomTimeLimit = (GameManager_script.Instance().isEverythingFocusedOnFrdsSelector || GameManager_script.Instance().rematchCurrentMatchIsRematch) ? 16.0f : 24.0f;

            // you either have already passed the 320 test as a stranger and now rematching, or you guys are already frds
            GameManager_script.Instance().targetPingHigherLimit = (GameManager_script.Instance().isEverythingFocusedOnFrdsSelector || GameManager_script.Instance().rematchCurrentMatchIsRematch) ? 1000.0f : 333.0f;

            // grab object
            ServerController.serverController = gameObject.GetComponent<ServerController>();

            // make sure we set them to false to begin with
            ServerController.serverController.NetworkGameStarted = false;

            // text related stuffz
            SearchTextTimerHelperFunction();

            // if we happen to already be in lobby, we attempt to join right away
            if (PhotonNetwork.insideLobby)
            {
                if (!GameAlreadyStarted && !SmartRobotIsValidCandidate && WeAreLookingForNetWorkGamez)
                {
                    StartCoroutine(WaitForSomeTimeBeforeSearchForRoom());
                }
            }
        }
        else
        {
            // specifically declare we are NOT looking for anything on the network
            WeAreLookingForNetWorkGamez = false;

            // disable text updating flag
            DisplayAwesomeSearchText = false;

            // kill server part
            ServerController.serverController = gameObject.GetComponent<ServerController>();

            // loading content
            GameManager_script.Instance().connectionStatus = GameManager_script.Instance().ShowLoadGameScreen(false);
            LoadingText.charOffset = 4;

            // load level as solo, if you want delays do them here
            StartCoroutine(DelayStartStupidBotOrSoloGame());
        }
    }
}
