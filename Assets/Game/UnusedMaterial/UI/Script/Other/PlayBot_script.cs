using UnityEngine;
using System.Collections;

public class PlayBot_script : MonoBehaviour
{
    public void OnClick()
    {
        if (GameManager_script.Instance().CoinCount >= GameManager_script.Instance().StupidBotCoinWager)
        {
            GameManager_script.Instance().UpdateCoinCount(-1.0f * GameManager_script.Instance().StupidBotCoinWager);

            GameManager_script.Instance().rematchCurrentMatchIsRematch = false;
            GameManager_script.Instance().rematchSelfWantToRematch = false;
            GameManager_script.Instance().rematchOppoWantToRematch = false;

            GameManager_script.Instance().StartingOutAsANetWorkGame = false;
            GameManager_script.Instance().CurrentlyInANetWorkGame = false;
            GameManager_script.Instance().StupidBotInActionGame = true;
            GameManager_script.Instance().SmartBotInActionGame = false;
            GameManager_script.Instance().TrulySelfInActionGame = false;
            GameManager_script.Instance().FTUEInActionGame = false;

            GameManager_script.Instance().CurrentWager = 0.0f;

            GameManager_script.Instance().PopulateInterstitialStartScreen
            (
                true,
                GameManager_script.Instance().GetMaxTPAScore(),
                GameManager_script.Instance().Total_Games_Played,
                GameManager_script.Instance().Total_Games_Won,
                GameManager_script.Instance().CoinCount - GameManager_script.Instance().CurrentWager,
                GameManager_script.Instance().Current_Win_Streak
            );

            GameManager_script.Instance().PopulateSelfGameProfileInfo();

            int opponentIndex = Random.Range(0, GameManager_script.BotHeadImageArray.Length);

            GameManager_script.Instance().PopulateOtherGameProfileInfo(
                Localization.Get(GameManager_script.BotPlayerNameArray[opponentIndex]),
                GameManager_script.BotCueEquippedArray[opponentIndex],
                GameManager_script.BotHeadImageArray[opponentIndex],
                GameManager_script.Instance().GetBotStarColor(GameManager_script.BotTPAScoreArray[opponentIndex]),
                GameManager_script.BotStarLevelArray[opponentIndex],
                GameManager_script.BotTPAScoreArray[opponentIndex],
                GameManager_script.StupidBotInActionGamesPlayedArray[opponentIndex],
                GameManager_script.StupidBotInActionGamesWonArray[opponentIndex],
                GameManager_script.StupidBotInActionGamesCoinsArray[opponentIndex],
                GameManager_script.StupidBotInActionGamesStreakArray[opponentIndex]
            );

            GameManager_script.Instance().TableTextureIndex = 1; // green

            GameManager_script.Instance().resetSingleGameStats();

            GameManager_script.Instance().PopupCurrentlyVisible = false;

            GameManager_script.Instance().NetworkGameSceneCurrentLoad = false;

            Application.LoadLevel("GameStart");
        }

        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
    }
}
