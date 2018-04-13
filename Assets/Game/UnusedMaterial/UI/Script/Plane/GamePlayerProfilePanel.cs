using UnityEngine;
using System.Collections;

public class GamePlayerProfilePanel : MonoBehaviour
{
    public Vector2 BackgroundXY;
    public UIPanel WindowPanel;
    public UIPanel PlayerProfilePanel; // this is the main thing   
    public UISprite BoxBackground;
    public UISprite HeadImage;
    public UILabel PlayerName;
    public UISprite Star;
    public UILabel Profilepopuptext1;
    public UILabel Profilepopuptext2;
    public UILabel Profilepopuptext3;
    public UILabel Profilepopuptext4;
    public UILabel Profilepopuptext5;
    public UILabel Profilepopuptext6;
    public UILabel Profilepopuptextvalue1;
    public UILabel Profilepopuptextvalue2;
    public UILabel Profilepopuptextvalue3;
    public UILabel Profilepopuptextvalue4;
    public UILabel Profilepopuptextvalue5;
    public UILabel Profilepopuptextvalue6;
    ProfilePopupInfo vProfilePopupInfo;

    public void PlayerProfileUI(int ProfileID)
    {
        if (ProfileID == 1)
        {
             vProfilePopupInfo = GameManager_script.Instance().selfGameProfileInfo;
        }
        
        if (ProfileID == 2)
        {
             vProfilePopupInfo = GameManager_script.Instance().otherGameProfileInfo;
        }

        gameObject.transform.parent.GetComponent<UISprite>().alpha = 0.925f;

        if (vProfilePopupInfo == null)
        {
            HeadImage.gameObject.SetActive(false);
            Star.gameObject.SetActive(false);
            PlayerName.gameObject.SetActive(false);

            Profilepopuptext1.gameObject.SetActive(false);
            Profilepopuptextvalue1.gameObject.SetActive(false);
            Profilepopuptext2.gameObject.SetActive(false);
            Profilepopuptextvalue2.gameObject.SetActive(false);
            Profilepopuptext3.gameObject.SetActive(false);
            Profilepopuptextvalue3.gameObject.SetActive(false);
            Profilepopuptext4.gameObject.SetActive(false);
            Profilepopuptextvalue4.gameObject.SetActive(false);
            Profilepopuptext5.gameObject.SetActive(false);
            Profilepopuptextvalue5.gameObject.SetActive(false);
            Profilepopuptext6.gameObject.SetActive(false);
            Profilepopuptextvalue6.gameObject.SetActive(false);
        }
        else
        {
            //头像
            HeadImage.width = (int)(BackgroundXY.x * 0.28f);
            HeadImage.height = (int)(BackgroundXY.x * 0.28f); // 0.38f is what we want to give the left side (golden ratio)
            HeadImage.transform.localPosition = new Vector3(-BackgroundXY.x * 0.5f + BackgroundXY.x * 0.14f + BackgroundXY.x * 0.05f, BackgroundXY.x * 0.05f, 0);
            HeadImage.spriteName = "" + vProfilePopupInfo.HeadImage;

            //Star
            Star.height = Mathf.CeilToInt(HeadImage.height * 0.4f);
            Star.width = Mathf.CeilToInt(HeadImage.height * 0.4f);
            Star.transform.localPosition = new Vector3(HeadImage.transform.localPosition.x - HeadImage.width * 0.4f, HeadImage.transform.localPosition.y + HeadImage.height * 0.4f, 0);

            Star.GetComponent<UIStar>().Text.width = GameManager_script.Instance().StarTextFineTuneWidth(Star.width, GameManager_script.Instance().CurrentLevel);
            Star.GetComponent<UIStar>().Text.height = GameManager_script.Instance().StarTextFineTuneHeight(Star.height, GameManager_script.Instance().CurrentLevel);
            Star.GetComponent<UIStar>().Text.transform.localPosition = GameManager_script.Instance().StarTextFineTunePosition(Star.GetComponent<UIStar>().Text.transform.localPosition, Star.GetComponent<UIStar>().Text.width, Star.GetComponent<UIStar>().Text.height, GameManager_script.Instance().CurrentLevel);
            Star.GetComponent<UIStar>().StartUI(vProfilePopupInfo.Star);

            //PlayerName
            PlayerName.width = (int)(BackgroundXY.x * 0.28f);
            PlayerName.height = (int)(HeadImage.height * 0.168f);
            PlayerName.transform.localPosition = new Vector3(HeadImage.transform.localPosition.x, HeadImage.transform.localPosition.y - HeadImage.height / 2 - PlayerName.height / 2 - HeadImage.height * 0.15f, 0);
            PlayerName.text = Localization.Get(vProfilePopupInfo.PlayerName);

            //Profilepopuptext1
            Profilepopuptext1.width = (int)(BackgroundXY.x * 0.425f);
            Profilepopuptext1.height = (int)(BackgroundXY.y * 0.075f);
            Profilepopuptext1.transform.localPosition = new Vector3(BackgroundXY.x * -0.135f, BackgroundXY.y * 0.3625f, 0);
            Profilepopuptext1.text = Localization.Get("InGameProfileTPAScore");

            //Profilepopuptext1
            Profilepopuptextvalue1.width = (int)(BackgroundXY.x * 0.30f);
            Profilepopuptextvalue1.height = (int)(BackgroundXY.y * 0.075f);
            Profilepopuptextvalue1.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * 0.3625f, 0);
            Profilepopuptextvalue1.text = GameManager_script.convertNumberIntoGoodStringFormat(vProfilePopupInfo.TPAScore, "percentage");

            //Profilepopuptext2
            Profilepopuptext2.width = (int)(BackgroundXY.x * 0.425f);
            Profilepopuptext2.height = (int)(BackgroundXY.y * 0.075f);
            Profilepopuptext2.transform.localPosition = new Vector3(BackgroundXY.x * -0.135f, BackgroundXY.y * 0.2175f, 0);
            Profilepopuptext2.text = Localization.Get("InGameProfileWinPercentage");

            //Profilepopuptext2
            Profilepopuptextvalue2.width = (int)(BackgroundXY.x * 0.30f);
            Profilepopuptextvalue2.height = (int)(BackgroundXY.y * 0.075f);
            Profilepopuptextvalue2.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * 0.2175f, 0);
            Profilepopuptextvalue2.text = GameManager_script.convertNumberIntoGoodStringFormat(100.0f * (vProfilePopupInfo.gamesPlayed == 0.0f ? 0.0f : vProfilePopupInfo.gamesWon / vProfilePopupInfo.gamesPlayed), "percentage");

            //Profilepopuptext3
            Profilepopuptext3.width = (int)(BackgroundXY.x * 0.425f);
            Profilepopuptext3.height = (int)(BackgroundXY.y * 0.075f);
            Profilepopuptext3.transform.localPosition = new Vector3(BackgroundXY.x * -0.135f, BackgroundXY.y * 0.0725f, 0);
            Profilepopuptext3.text = Localization.Get("InGameProfileGamesWon");

            //Profilepopuptext3
            Profilepopuptextvalue3.width = (int)(BackgroundXY.x * 0.30f);
            Profilepopuptextvalue3.height = (int)(BackgroundXY.y * 0.075f);
            Profilepopuptextvalue3.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * 0.0725f, 0);
            Profilepopuptextvalue3.text = GameManager_script.convertNumberIntoGoodStringFormat(vProfilePopupInfo.gamesWon, "number") + " / " + GameManager_script.convertNumberIntoGoodStringFormat(vProfilePopupInfo.gamesPlayed, "number");

            //Profilepopuptext4
            Profilepopuptext4.width = (int)(BackgroundXY.x * 0.425f);
            Profilepopuptext4.height = (int)(BackgroundXY.y * 0.075f);
            Profilepopuptext4.transform.localPosition = new Vector3(BackgroundXY.x * -0.135f, BackgroundXY.y * -0.0725f, 0);
            Profilepopuptext4.text = Localization.Get("InGameProfileWinStreak");

            //Profilepopuptext4
            Profilepopuptextvalue4.width = (int)(BackgroundXY.x * 0.30f);
            Profilepopuptextvalue4.height = (int)(BackgroundXY.y * 0.075f);
            Profilepopuptextvalue4.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * -0.0725f, 0);
            Profilepopuptextvalue4.text = GameManager_script.convertNumberIntoGoodStringFormat(vProfilePopupInfo.streak, "number");

            //Profilepopuptext5
            Profilepopuptext5.width = (int)(BackgroundXY.x * 0.425f);
            Profilepopuptext5.height = (int)(BackgroundXY.y * 0.075f);
            Profilepopuptext5.transform.localPosition = new Vector3(BackgroundXY.x * -0.135f, BackgroundXY.y * -0.2175f, 0);
            Profilepopuptext5.text = Localization.Get("InGameProfileMilitaryRank");

            //Profilepopuptext5
            Profilepopuptextvalue5.width = (int)(BackgroundXY.x * 0.345f);
            Profilepopuptextvalue5.height = (int)(BackgroundXY.y * 0.075f);
            Profilepopuptextvalue5.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * -0.2175f, 0);
            Profilepopuptextvalue5.text = Localization.Get(GameManager_script.MilitaryRankName[vProfilePopupInfo.Star.text]);

            //Profilepopuptext6
            Profilepopuptext6.width = (int)(BackgroundXY.x * 0.425f);
            Profilepopuptext6.height = (int)(BackgroundXY.y * 0.075f);
            Profilepopuptext6.transform.localPosition = new Vector3(BackgroundXY.x * -0.135f, BackgroundXY.y * -0.3625f, 0);
            Profilepopuptext6.text = Localization.Get("InGameProfileSkillRank");

            //Profilepopuptext6
            Profilepopuptextvalue6.width = (int)(BackgroundXY.x * 0.30f);
            Profilepopuptextvalue6.height = (int)(BackgroundXY.y * 0.075f);
            Profilepopuptextvalue6.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * -0.3625f, 0);
            Profilepopuptextvalue6.text = Localization.Get(GameManager_script.StarRankName[(int)vProfilePopupInfo.Star.starType]);
        }
    }

    public void ChangeInsideDepth(int vDepth)
    {
        gameObject.GetComponent<UIPanel>().depth = (vDepth);
    }
}
