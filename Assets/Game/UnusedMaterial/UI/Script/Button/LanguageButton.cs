using UnityEngine;
using System.Collections;

public class LanguageButton : MonoBehaviour
{
    public UIPanel WindowPanel;
    public int buttonId = 0;
    public UISprite Background;
    public UISprite MaskBackground;
    public UISprite MaskLanguageButton;
    public UILabel ButtonName;

    public void UI()
    {
        Background.GetComponent<BoxCollider>().size = new Vector3(Background.width , Background.height , 0);

        MaskBackground.width = Background.width;
        MaskBackground.height = Background.height;

        // MaskShopMainButton
        MaskLanguageButton.width = Background.width;
        MaskLanguageButton.height = Background.height;

        // dajiang hack, picture's ratio is 2.16f measured, and problems start to appear below the 1.75f button ratio line
        float buttonBackgroundRatio = 1.0f * Background.width / Background.height;
        float buttonRatioSubtraction = buttonBackgroundRatio < 1.80f ? buttonBackgroundRatio : 1.80f;
        float buttonImageNaturalRatio = 2.16f - (1.80f - buttonRatioSubtraction);


        //ButtonName
        ButtonName.width = (int)(Background.width );
        ButtonName.height = (int)(Background.height * 0.225f);
        ButtonName.transform.localPosition = new Vector3(0, -Background.height * 0.50f - ButtonName.height * 0.50f - Background.height * 0.05f, 0);

    }


    public void OnClick()
    {
        // sound
        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);

        // dajiang hack, absolute path here.
        if (GameObject.Find("GameManager/Localization"))
        {
            GameObject.Find("GameManager/Localization").GetComponent<SelectionLanguage>().SetLanguageFromStrings(ButtonName.text);

            Localization.language = ButtonName.text; // no need to save language again, already did
        }

        // name changez
        if (GameManager_script.Instance().First_Name == "" ||
            GameManager_script.Instance().First_Name == "Player Name" ||
            GameManager_script.Instance().First_Name == "玩家昵称" ||
            GameManager_script.Instance().First_Name == "Su Nombre" ||
            GameManager_script.Instance().First_Name == "Spielername" ||
            GameManager_script.Instance().First_Name == "Votre Nom" ||
            GameManager_script.Instance().First_Name == "プレイヤー名")
        {
            GameManager_script.Instance().First_Name = Localization.Get("FirstName");

            PlayerPrefs.SetString("First_Name", GameManager_script.Instance().First_Name);
        }

        // clear popup
        GameManager_script.Instance().PopupCurrentlyVisible = false;

        // no network lolz
        GameManager_script.Instance().NetworkGameSceneCurrentLoad = false;

        Application.LoadLevel("NGUI_MENU");
    }
}
