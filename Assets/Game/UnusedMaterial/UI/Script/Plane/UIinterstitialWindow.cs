using UnityEngine;
using System.Collections;

public class UIinterstitialWindow : MonoBehaviour
{
    public UIPanel RootPanel;
    public Vector2 BackgroundXY;
    public UIPanel WindowPanel;
    public UIPanel PlayerProfilePanel; // this is the main thing   

    public UISprite HeadImage;
    public UILabel PlayerName;
    public UISprite Star;
    public UISprite MoneyImage;
    public UILabel MoneyText;
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
    public UISprite BoxBackground;

    [System.NonSerialized]
    public bool TotalScoreBool = false;
    [System.NonSerialized]
    public float CurrentMoney = 0;
    [System.NonSerialized]
    public float UpdateMoney = 0;

    public void interstitialWindowUI()
    {
        //BackgroundXY
        BoxBackground.height = (int)(RootPanel.height * 0.565f); // this value is sort of set
        BoxBackground.width = (int)(Mathf.Min(Mathf.Max(1024.0f * 0.85f, RootPanel.width * 0.70f), BoxBackground.height * 2.0f)); // cannot be smaller than 870.0f and cannot be larger than 1.90X height
        BoxBackground.transform.localPosition = new Vector3(0.0f, BackgroundXY.y * -0.10f, 0.0f);
        BoxBackground.color = new Color(0.0f, 0.0f, 0.0f, 1);
        BoxBackground.alpha = 0.925f;

        BackgroundXY.x = BoxBackground.width;
        BackgroundXY.y = BoxBackground.height;

        // MoneyImage
        MoneyImage.width = (int)(RootPanel.height * 0.20f);
        MoneyImage.height = (int)(RootPanel.height * 0.20f);
        MoneyImage.transform.localPosition = new Vector3(0, RootPanel.height * 0.5f - MoneyImage.height * 0.625f, 0);

        // money text
        MoneyText.width = (int)(MoneyImage.height * 6);
        MoneyText.height = (int)(MoneyImage.height * 0.30f);
        MoneyText.transform.localPosition = new Vector3(0, MoneyImage.transform.localPosition.y - MoneyImage.height / 2 - MoneyText.height / 2, 0);

        //头像
        HeadImage.width = (int)(BackgroundXY.x * 0.28f);
        HeadImage.height = (int)(BackgroundXY.x * 0.28f); // 0.38f is what we want to give the left side (golden ratio)
        HeadImage.transform.localPosition = new Vector3(-BackgroundXY.x * 0.5f + BackgroundXY.x * 0.14f + BackgroundXY.x * 0.05f, BackgroundXY.x * 0.05f + BoxBackground.transform.localPosition.y, 0);

        //Star
        Star.height = Mathf.CeilToInt(HeadImage.height * 0.4f);
        Star.width = Mathf.CeilToInt(HeadImage.height * 0.4f);
        Star.transform.localPosition = new Vector3(HeadImage.transform.localPosition.x - HeadImage.width * 0.4f, HeadImage.transform.localPosition.y + HeadImage.height * 0.4f, 0);

        Star.GetComponent<UIStar>().Text.width = GameManager_script.Instance().StarTextFineTuneWidth(Star.width, GameManager_script.Instance().CurrentLevel);
        Star.GetComponent<UIStar>().Text.height = GameManager_script.Instance().StarTextFineTuneHeight(Star.height, GameManager_script.Instance().CurrentLevel);
        Star.GetComponent<UIStar>().Text.transform.localPosition = GameManager_script.Instance().StarTextFineTunePosition(Star.GetComponent<UIStar>().Text.transform.localPosition, Star.GetComponent<UIStar>().Text.width, Star.GetComponent<UIStar>().Text.height, GameManager_script.Instance().CurrentLevel);

        //PlayerName
        PlayerName.width = (int)(BackgroundXY.x * 0.28f);
        PlayerName.height = (int)(HeadImage.height * 0.168f);
        PlayerName.transform.localPosition = new Vector3(HeadImage.transform.localPosition.x, HeadImage.transform.localPosition.y - HeadImage.height / 2 - PlayerName.height / 2 - HeadImage.height * 0.15f, 0);

        //Profilepopuptext1
        Profilepopuptext1.width = (int)(BackgroundXY.x * 0.425f);
        Profilepopuptext1.height = (int)(BackgroundXY.y * 0.075f);
        Profilepopuptext1.transform.localPosition = new Vector3(BackgroundXY.x * -0.135f, BackgroundXY.y * 0.3625f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext1
        Profilepopuptextvalue1.width = (int)(BackgroundXY.x * 0.30f);
        Profilepopuptextvalue1.height = (int)(BackgroundXY.y * 0.075f);
        Profilepopuptextvalue1.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * 0.3625f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext2 (special case we can extend the box coz the right side is very short and left side is very long)
        Profilepopuptext2.width = (int)(BackgroundXY.x * 0.425f);
        Profilepopuptext2.height = (int)(BackgroundXY.y * 0.075f);
        Profilepopuptext2.transform.localPosition = new Vector3(BackgroundXY.x * -0.135f, BackgroundXY.y * 0.2175f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext2
        Profilepopuptextvalue2.width = (int)(BackgroundXY.x * 0.30f);
        Profilepopuptextvalue2.height = (int)(BackgroundXY.y * 0.075f);
        Profilepopuptextvalue2.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * 0.2175f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext3
        Profilepopuptext3.width = (int)(BackgroundXY.x * 0.425f);
        Profilepopuptext3.height = (int)(BackgroundXY.y * 0.075f);
        Profilepopuptext3.transform.localPosition = new Vector3(BackgroundXY.x * -0.135f, BackgroundXY.y * 0.0725f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext3
        Profilepopuptextvalue3.width = (int)(BackgroundXY.x * 0.30f);
        Profilepopuptextvalue3.height = (int)(BackgroundXY.y * 0.075f);
        Profilepopuptextvalue3.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * 0.0725f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext4
        Profilepopuptext4.width = (int)(BackgroundXY.x * 0.425f);
        Profilepopuptext4.height = (int)(BackgroundXY.y * 0.075f);
        Profilepopuptext4.transform.localPosition = new Vector3(BackgroundXY.x * -0.135f, BackgroundXY.y * -0.0725f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext4
        Profilepopuptextvalue4.width = (int)(BackgroundXY.x * 0.30f);
        Profilepopuptextvalue4.height = (int)(BackgroundXY.y * 0.075f);
        Profilepopuptextvalue4.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * -0.0725f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext5
        Profilepopuptext5.width = (int)(BackgroundXY.x * 0.425f);
        Profilepopuptext5.height = (int)(BackgroundXY.y * 0.075f);
        Profilepopuptext5.transform.localPosition = new Vector3(BackgroundXY.x * -0.135f, BackgroundXY.y * -0.2175f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext5
        Profilepopuptextvalue5.width = (int)(BackgroundXY.x * 0.345f);
        Profilepopuptextvalue5.height = (int)(BackgroundXY.y * 0.075f);
        Profilepopuptextvalue5.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * -0.2175f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext5
        Profilepopuptext6.width = (int)(BackgroundXY.x * 0.425f);
        Profilepopuptext6.height = (int)(BackgroundXY.y * 0.075f);
        Profilepopuptext6.transform.localPosition = new Vector3(BackgroundXY.x * -0.135f, BackgroundXY.y * -0.3625f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext5
        Profilepopuptextvalue6.width = (int)(BackgroundXY.x * 0.30f);
        Profilepopuptextvalue6.height = (int)(BackgroundXY.y * 0.075f);
        Profilepopuptextvalue6.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * -0.3625f + BoxBackground.transform.localPosition.y, 0);

        // set all the things!
        if (GameManager_script.Instance().interstitialPageInfo != null)
        {
            // this is incoming, mostly your own stats
            HeadImage.spriteName = "" + GameManager_script.Instance().interstitialPageInfo.HeadImage_one;
            Star.GetComponent<UIStar>().StartUI(GameManager_script.Instance().interstitialPageInfo.Star_one);
            PlayerName.text = Localization.Get(GameManager_script.Instance().interstitialPageInfo.PlayerName_one);

            CalculateUpdateMoney(0.0f, GameManager_script.Instance().CurrentWager * 2.0f);
            MoneyImage.spriteName = GetChipTypeBasedOnChips(GameManager_script.Instance().CurrentWager * 2.0f);

            Profilepopuptext1.text = Localization.Get("IinterstitialProfilepopuptext2");
            Profilepopuptextvalue1.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().interstitialPageInfo.Total_TPAScore, "percentage");

            Profilepopuptext2.text = Localization.Get("IinterstitialProfilepopuptext3");
            Profilepopuptextvalue2.text = GameManager_script.convertNumberIntoGoodStringFormat(100.0f * (GameManager_script.Instance().interstitialPageInfo.Total_GamesPlayed == 0.0f ? 0.0f : GameManager_script.Instance().interstitialPageInfo.Total_GamesWon / GameManager_script.Instance().interstitialPageInfo.Total_GamesPlayed), "percentage");

            Profilepopuptext3.text = Localization.Get("IinterstitialProfilepopuptext4");
            Profilepopuptextvalue3.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().interstitialPageInfo.Total_GamesWon, "number") + " / " + GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().interstitialPageInfo.Total_GamesPlayed, "number");

            Profilepopuptext4.text = Localization.Get("IinterstitialProfilepopuptext5");
            Profilepopuptextvalue4.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().interstitialPageInfo.streak, "number");

            Profilepopuptext5.text = Localization.Get("IinterstitialProfilepopuptext6");
            Profilepopuptextvalue5.text = Localization.Get(GameManager_script.MilitaryRankName[GameManager_script.Instance().interstitialPageInfo.Star_one.text]);

            Profilepopuptext6.text = Localization.Get("IinterstitialProfilepopuptext7");
            Profilepopuptextvalue6.text = Localization.Get(GameManager_script.StarRankName[(int)GameManager_script.Instance().interstitialPageInfo.Star_one.starType]);
        }
    }

    public void Update()
    {
        if (TotalScoreBool)
        {
            CurrentMoney += UpdateMoney;

            MoneyText.text = Localization.Get("IinterstitialProfilepopuptext1") + GameManager_script.convertNumberIntoGoodStringFormat(CurrentMoney, "gamecoinz");

            if (CurrentMoney > GameManager_script.Instance().CurrentWager * 2.0f)
            {
                MoneyText.text = Localization.Get("IinterstitialProfilepopuptext1") + GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().CurrentWager * 2.0f, "gamecoinz");

                TotalScoreBool = false;
            }
        }
    }

    public void CalculateUpdateMoney(float inStart, float inEnd)
    {
        float diff = inEnd - inStart;

        if (Mathf.Abs(diff) > 1000000)
        {
            UpdateMoney = diff / (60 * 5.00f); // 6 seconds
        }
        else if (Mathf.Abs(diff) > 100000)
        {
            UpdateMoney = diff / (60 * 4.00f); // 5 seconds
        }
        else if (Mathf.Abs(diff) > 10000)
        {
            UpdateMoney = diff / (60 * 3.00f); // 4 seconds
        }
        else if (Mathf.Abs(diff) > 1000)
        {
            UpdateMoney = diff / (60 * 2.25f); // 3 seconds
        }
        else if (Mathf.Abs(diff) > 100)
        {
            UpdateMoney = diff / (60 * 1.50f); // 2 seconds
        }
        else
        {
            UpdateMoney = diff / (60 * 1.00f); // 1 second
        }
    }

    public string GetChipTypeBasedOnChips(float inAmount)
    {
        if (inAmount <= GameManager_script.WagerLevels[0] * 2.0f)
        {
            return ChipType.Chip3.ToString();
        }
        else if (inAmount <= GameManager_script.WagerLevels[1] * 2.0f)
        {
            return ChipType.Chip4.ToString();
        }
        else if (inAmount <= GameManager_script.WagerLevels[2] * 2.0f)
        {
            return ChipType.Chip4.ToString();
        }
        else if (inAmount <= GameManager_script.WagerLevels[3] * 2.0f)
        {
            return ChipType.Chip5.ToString();
        }
        else if (inAmount <= GameManager_script.WagerLevels[4] * 2.0f)
        {
            return ChipType.Chip5.ToString();
        }
        else if (inAmount <= GameManager_script.WagerLevels[5] * 2.0f)
        {
            return ChipType.Chip6.ToString();
        }
        else if (inAmount <= GameManager_script.WagerLevels[6] * 2.0f)
        {
            return ChipType.Chip6.ToString();
        }
        else if (inAmount <= GameManager_script.WagerLevels[7] * 2.0f)
        {
            return ChipType.Chip7.ToString();
        }
        else if (inAmount <= GameManager_script.WagerLevels[8] * 2.0f)
        {
            return ChipType.Chip7.ToString();
        }
        else if (inAmount <= GameManager_script.WagerLevels[9] * 2.0f) // 10 for now
        {
            return ChipType.Chip8.ToString();
        }
        else
        {
            return ChipType.Chip8.ToString();
        }
    }
}
