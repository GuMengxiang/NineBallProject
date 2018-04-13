using UnityEngine;
using System.Collections;

public class UIFTUE_Button : MonoBehaviour
{
    void OnClick()
    {
        // load the game up
        if (GameManager_script.Instance().CoinCount >= 0)
        {
            GameManager_script.Instance().rematchCurrentMatchIsRematch = false;
            GameManager_script.Instance().rematchSelfWantToRematch = false;
            GameManager_script.Instance().rematchOppoWantToRematch = false;

            GameManager_script.Instance().StartingOutAsANetWorkGame = false;
            GameManager_script.Instance().CurrentlyInANetWorkGame = false;
            GameManager_script.Instance().StupidBotInActionGame = false;
            GameManager_script.Instance().SmartBotInActionGame = false;
            GameManager_script.Instance().TrulySelfInActionGame = false;
            GameManager_script.Instance().FTUEInActionGame = true;

            GameManager_script.Instance().PopulateInterstitialStartScreen
            (
                true,
                GameManager_script.Instance().GetMaxTPAScore(),
                GameManager_script.Instance().Total_Games_Played,
                GameManager_script.Instance().Total_Games_Won,
                GameManager_script.Instance().CoinCount - GameManager_script.Instance().CurrentWager,
                GameManager_script.Instance().Current_Win_Streak
            );

            GameManager_script.Instance().CurrentWager = 0.0f;

            GameManager_script.Instance().PopulateSelfGameProfileInfo();

            GameManager_script.Instance().otherGameProfileInfo = GameManager_script.Instance().selfGameProfileInfo;

            GameManager_script.Instance().TableTextureIndex = 1; // green

            GameManager_script.Instance().resetSingleGameStats();

            GameManager_script.Instance().PopupCurrentlyVisible = false;

            GameManager_script.Instance().NetworkGameSceneCurrentLoad = false;

            Application.LoadLevel("GameStart");
        }

        // music
        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
    }
}
