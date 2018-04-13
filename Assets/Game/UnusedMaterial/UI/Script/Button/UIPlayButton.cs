using UnityEngine;
using System.Collections;

public class UIPlayButton : MonoBehaviour
{
    void OnClick()
    {
        bool CanProceedWithoutFrdsKeyHassle = true; // we can almost always proceed

        if (GameManager_script.Instance().CoinCount >= GameManager_script.Instance().CurrentWager) // hack
        {
            if (GameManager_script.Instance().isEverythingFocusedOnFrdsSelector && (GameManager_script.Instance().SelectorFrdKey == "" || GameManager_script.Instance().SelectorFrdKey == "Enter Passcode Here" || GameManager_script.Instance().SelectorFrdKey == "这里输入验证码" || GameManager_script.Instance().SelectorFrdKey == "Introduce el Código Aquí" || GameManager_script.Instance().SelectorFrdKey == "Gib hier deinen PIN-Code ein" || GameManager_script.Instance().SelectorFrdKey == "Saisissez votre mot de passe" ||  GameManager_script.Instance().SelectorFrdKey == "パスコードを入力してください"))
            {
                CanProceedWithoutFrdsKeyHassle = false;
            }

            if (CanProceedWithoutFrdsKeyHassle)
            {
                GameManager_script.Instance().rematchCurrentMatchIsRematch = false;
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
        }

        // music
        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
    }
}
