using UnityEngine;
using System.Collections;

public class PlaySolo_script : MonoBehaviour 
{
    public void OnClick()
    {
        if (GameManager_script.Instance().CoinCount >= GameManager_script.Instance().SoloCoinWager)
        {
            GameManager_script.Instance().UpdateCoinCount(-1.0f * GameManager_script.Instance().SoloCoinWager);

            GameManager_script.Instance().rematchCurrentMatchIsRematch = false;
            GameManager_script.Instance().rematchSelfWantToRematch = false;
            GameManager_script.Instance().rematchOppoWantToRematch = false;

            GameManager_script.Instance().StartingOutAsANetWorkGame = false;
            GameManager_script.Instance().CurrentlyInANetWorkGame = false;
            GameManager_script.Instance().StupidBotInActionGame = false;
            GameManager_script.Instance().SmartBotInActionGame = false;
            GameManager_script.Instance().TrulySelfInActionGame = true;
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

            GameManager_script.Instance().otherGameProfileInfo = GameManager_script.Instance().selfGameProfileInfo;

            GameManager_script.Instance().TableTextureIndex = 0; // blue

            GameManager_script.Instance().resetSingleGameStats();

            GameManager_script.Instance().PopupCurrentlyVisible = false;

            GameManager_script.Instance().NetworkGameSceneCurrentLoad = false;

            Application.LoadLevel("GameStart");
        }

        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
    }
}
