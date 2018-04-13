using UnityEngine;
using System.Collections;

public class UIinterstitialWindowEnd : MonoBehaviour
{
    public UIPanel RootPanel;
    public Vector2 BackgroundXY;
    public UIPanel WindowPanel;
    public UIPanel PlayerProfilePanel; // this is the main thing   

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
    public UILabel LeftProfilepopuptextvalue1;
    public UILabel LeftProfilepopuptextvalue2;
    public UILabel LeftProfilepopuptextvalue3;
    public UILabel LeftProfilepopuptextvalue4;
    public UILabel LeftProfilepopuptextvalue5;
    public UILabel LeftProfilepopuptextvalue6;
    public UISprite BoxBackground;

    public GameObject PlayButton;
    public UISprite PlayButtonBackground;
    public UISprite PlayButtonMask;
    public UILabel PlayButtonLabel;

    public UISprite RTTBox;
    public UISprite RTTArrow;
    public UILabel RTTText;

    [System.NonSerialized]
    public bool TotalScoreBool = false;
    [System.NonSerialized]
    public float CurrentMoney = 0;
    [System.NonSerialized]
    public float UpdateMoney = 0;
    [System.NonSerialized]
    public bool HideRematchAnyways = false; // dajiang hack, we hide rematch after 5 consecutive matches

    public void interstitialWindowEndUI()
    {
        if (GameManager_script.Instance().StartingOutAsANetWorkGame)
        {
            GameManager_script.Instance().connectionStatus = Localization.Get("GameEndEmptyHolder");

            LoadingText.charOffset = 4;
        }
        else
        {
            GameManager_script.Instance().connectionStatus = Localization.Get("GameEndDisclaimer");

            LoadingText.charOffset = 1;
        }

        if (!GameManager_script.Instance().isEverythingFocusedOnFrdsSelector && GameManager_script.Instance().rematchYourWinCount + GameManager_script.Instance().rematchOppoWinCount >= 3.0f)
        {
            HideRematchAnyways = true;
        }
        else
        {
            HideRematchAnyways = false;
        }

        // BackgroundXY
        BoxBackground.height = (int)(RootPanel.height * 0.565f); // this value is sort of set
        BoxBackground.width = (int)(Mathf.Min(Mathf.Max(1024.0f * 0.85f, RootPanel.width * 0.70f), BoxBackground.height * 2.0f)); // cannot be smaller than 870.0f and cannot be larger than 2.0X height
        BoxBackground.transform.localPosition = new Vector3(0.0f, BackgroundXY.y * -0.10f, 0.0f);
        BoxBackground.color = new Color(0.0f, 0.0f, 0.0f, 1);
        BoxBackground.alpha = 0.925f;

        // background
        BackgroundXY.x = BoxBackground.width;
        BackgroundXY.y = BoxBackground.height;

        // MoneyImage
        MoneyImage.width = (int)(RootPanel.height * 0.20f);
        MoneyImage.height = (int)(RootPanel.height * 0.20f);
        MoneyImage.transform.localPosition = new Vector3(0, RootPanel.height * 0.5f - MoneyImage.height * 0.625f, 0);
        MoneyImage.spriteName = GetChipTypeBasedOnChips(GameManager_script.Instance().CurrentWager * 2.0f);

        // money text
        MoneyText.width = (int)(MoneyImage.height * 6);
        MoneyText.height = (int)(MoneyImage.height * 0.30f);
        MoneyText.transform.localPosition = new Vector3(0, MoneyImage.transform.localPosition.y - MoneyImage.height / 2 - MoneyText.height / 2, 0);

        // calculate calculate
        CalculateUpdateMoney(0.0f, GameManager_script.Instance().CurrentWager * 2.0f);

        // show or hide these fucking texts and stuffz
        if (GameManager_script.Instance().StartingOutAsANetWorkGame && GameManager_script.Instance().interstitialPageInfo.trueIfYouWin)
        {
            MoneyImage.gameObject.SetActive(true);
            MoneyText.gameObject.SetActive(true);
        }
        else
        {
            MoneyImage.gameObject.SetActive(false);
            MoneyText.gameObject.SetActive(false);
        }
		///--------------GU-----------更改了字体的宽，从0.075f改为了1f
        //Profilepopuptext1
        Profilepopuptext1.width = (int)(BackgroundXY.x * 0.5f);
        Profilepopuptext1.height = (int)(BackgroundXY.y * 0.1f);
        Profilepopuptext1.transform.localPosition = new Vector3(0.0f, BackgroundXY.y * 0.3625f + BoxBackground.transform.localPosition.y, 0);
		///-----------20170308---------
        //Profilepopuptextvalue1
        Profilepopuptextvalue1.width = (int)(BackgroundXY.x * 0.30f);
		Profilepopuptextvalue1.height = (int)(BackgroundXY.y * 0.1f);
        Profilepopuptextvalue1.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * 0.3625f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext1
        LeftProfilepopuptextvalue1.width = (int)(BackgroundXY.x * 0.30f);
		LeftProfilepopuptextvalue1.height = (int)(BackgroundXY.y * 0.1f);
        LeftProfilepopuptextvalue1.transform.localPosition = new Vector3(BackgroundXY.x * -0.46f, BackgroundXY.y * 0.3625f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext2 (special case we can extend the box coz the right side is very short and left side is very long)
        Profilepopuptext2.width = (int)(BackgroundXY.x * 0.525f);
		Profilepopuptext2.height = (int)(BackgroundXY.y * 0.1f);
        Profilepopuptext2.transform.localPosition = new Vector3(0.0f, BackgroundXY.y * 0.2175f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptextvalue2
        Profilepopuptextvalue2.width = (int)(BackgroundXY.x * 0.30f);
		Profilepopuptextvalue2.height = (int)(BackgroundXY.y * 0.1f);
        Profilepopuptextvalue2.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * 0.2175f + BoxBackground.transform.localPosition.y, 0);

        //LeftProfilepopuptext2
        LeftProfilepopuptextvalue2.width = (int)(BackgroundXY.x * 0.30f);
		LeftProfilepopuptextvalue2.height = (int)(BackgroundXY.y * 0.1f);
        LeftProfilepopuptextvalue2.transform.localPosition = new Vector3(BackgroundXY.x * -0.46f, BackgroundXY.y * 0.2175f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext3
        Profilepopuptext3.width = (int)(BackgroundXY.x * 0.425f);
		Profilepopuptext3.height = (int)(BackgroundXY.y * 0.1f);
        Profilepopuptext3.transform.localPosition = new Vector3(0.0f, BackgroundXY.y * 0.0725f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptextvalue3
        Profilepopuptextvalue3.width = (int)(BackgroundXY.x * 0.30f);
		Profilepopuptextvalue3.height = (int)(BackgroundXY.y * 0.1f);
        Profilepopuptextvalue3.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * 0.0725f + BoxBackground.transform.localPosition.y, 0);

        //LeftProfilepopuptextvalue3
        LeftProfilepopuptextvalue3.width = (int)(BackgroundXY.x * 0.30f);
		LeftProfilepopuptextvalue3.height = (int)(BackgroundXY.y * 0.1f);
        LeftProfilepopuptextvalue3.transform.localPosition = new Vector3(BackgroundXY.x * -0.46f, BackgroundXY.y * 0.0725f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext4
        Profilepopuptext4.width = (int)(BackgroundXY.x * 0.425f);
		Profilepopuptext4.height = (int)(BackgroundXY.y * 0.1f);
        Profilepopuptext4.transform.localPosition = new Vector3(0.0f, BackgroundXY.y * -0.0725f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptextvalue4
        Profilepopuptextvalue4.width = (int)(BackgroundXY.x * 0.30f);
		Profilepopuptextvalue4.height = (int)(BackgroundXY.y * 0.1f);
        Profilepopuptextvalue4.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * -0.0725f + BoxBackground.transform.localPosition.y, 0);

        //LeftProfilepopuptextvalue4
        LeftProfilepopuptextvalue4.width = (int)(BackgroundXY.x * 0.30f);
		LeftProfilepopuptextvalue4.height = (int)(BackgroundXY.y * 0.1f);
        LeftProfilepopuptextvalue4.transform.localPosition = new Vector3(BackgroundXY.x * -0.46f, BackgroundXY.y * -0.0725f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext5
        Profilepopuptext5.width = (int)(BackgroundXY.x * 0.425f);
		Profilepopuptext5.height = (int)(BackgroundXY.y * 0.1f);
        Profilepopuptext5.transform.localPosition = new Vector3(0.0f, BackgroundXY.y * -0.2175f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptextvalue5
        Profilepopuptextvalue5.width = (int)(BackgroundXY.x * 0.30f);
		Profilepopuptextvalue5.height = (int)(BackgroundXY.y * 0.1f);
        Profilepopuptextvalue5.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * -0.2175f + BoxBackground.transform.localPosition.y, 0);

        //LeftProfilepopuptextvalue5
        LeftProfilepopuptextvalue5.width = (int)(BackgroundXY.x * 0.30f);
		LeftProfilepopuptextvalue5.height = (int)(BackgroundXY.y * 0.1f);
        LeftProfilepopuptextvalue5.transform.localPosition = new Vector3(BackgroundXY.x * -0.46f, BackgroundXY.y * -0.2175f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptext6
        Profilepopuptext6.width = (int)(BackgroundXY.x * 0.425f);
		Profilepopuptext6.height = (int)(BackgroundXY.y * 0.1f);
        Profilepopuptext6.transform.localPosition = new Vector3(0.0f, BackgroundXY.y * -0.3625f + BoxBackground.transform.localPosition.y, 0);

        //Profilepopuptextvalue6
        Profilepopuptextvalue6.width = (int)(BackgroundXY.x * 0.30f);
		Profilepopuptextvalue6.height = (int)(BackgroundXY.y * 0.1f);
        Profilepopuptextvalue6.transform.localPosition = new Vector3(BackgroundXY.x * 0.46f, BackgroundXY.y * -0.3625f + BoxBackground.transform.localPosition.y, 0);

        //LeftProfilepopuptextvalue6
        LeftProfilepopuptextvalue6.width = (int)(BackgroundXY.x * 0.30f);
		LeftProfilepopuptextvalue6.height = (int)(BackgroundXY.y * 0.1f);
        LeftProfilepopuptextvalue6.transform.localPosition = new Vector3(BackgroundXY.x * -0.46f, BackgroundXY.y * -0.3625f + BoxBackground.transform.localPosition.y, 0);

        // Play按钮
        PlayButtonBackground.width = (int)(BackgroundXY.x * 0.26f);
        PlayButtonBackground.height = (int)(PlayButtonBackground.width / 4.05f);
        PlayButtonMask.width = (int)(PlayButtonBackground.width) + 2;
        PlayButtonMask.height = (int)(PlayButtonBackground.height) + 2;
        PlayButtonMask.GetComponent<BoxCollider>().size = new Vector3(PlayButtonMask.width, PlayButtonMask.height, 0);
        PlayButton.transform.localPosition = new Vector3(0.0f, Screen.height * -0.5f + PlayButtonBackground.height * 0.9f, 0.0f);

        // play label
        PlayButtonLabel.width = (int)(PlayButtonBackground.width * 0.9f);
        PlayButtonLabel.height = (int)(PlayButtonBackground.height * 0.9f);
        PlayButtonLabel.transform.localPosition = new Vector3(PlayButtonLabel.transform.localPosition.x, PlayButtonLabel.transform.localPosition.y - PlayButtonLabel.height * 0.075f, PlayButtonLabel.transform.localPosition.z);
        PlayButtonLabel.text = Localization.Get("Rematch");

        // show or hide rematch button
        if (GameManager_script.Instance().StartingOutAsANetWorkGame && !HideRematchAnyways)
        {
            ToggleRematchButton(true);
        }
        else
        {
            ToggleRematchButton(false);
        }

        // initialize tooltips
        RTTBox.height = (int)(PlayButtonBackground.height * 1.0f);
        RTTBox.width = (int)(RTTBox.height * 6.85f);
        RTTBox.transform.localPosition = PlayButton.transform.localPosition + new Vector3(RTTBox.width * 0.54f + PlayButtonBackground.width * 0.50f, 0.0f, 0.0f);
        RTTBox.gameObject.SetActive(false);

        RTTArrow.width = (int)(RTTBox.width * 0.083f);
        RTTArrow.height = (int)(RTTArrow.width * 0.550f);
        RTTArrow.transform.localRotation = Quaternion.Euler(0, 0, -90);
        RTTArrow.transform.localPosition = RTTBox.transform.localPosition + new Vector3(-0.51525f * RTTBox.width, 0.0f * RTTBox.height, 0.0f);
        RTTArrow.gameObject.SetActive(false);
        
        RTTText.width = (int)(RTTBox.width * 0.95f);
        RTTText.height = (int)(RTTBox.height * 0.90f);
        RTTText.transform.localPosition = RTTBox.transform.localPosition;
        RTTText.fontSize = (int)(Screen.height / 29);
        RTTText.gameObject.SetActive(false);

        // set all the things!
        if (GameManager_script.Instance().interstitialPageInfo != null)
        {
            Profilepopuptext1.text = Localization.Get("IinterstitialProfilepopuptext9");
            Profilepopuptextvalue1.text = GameManager_script.Instance().interstitialPageInfo.PlayerName_two;
            LeftProfilepopuptextvalue1.text = GameManager_script.Instance().interstitialPageInfo.PlayerName_one;

            Profilepopuptext2.text = Localization.Get("IinterstitialProfilepopuptext10");
            Profilepopuptextvalue2.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().interstitialPageInfo.single_TPAScore_two, "percentage");
            LeftProfilepopuptextvalue2.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().interstitialPageInfo.single_TPAScore_one, "percentage");

            Profilepopuptext3.text = Localization.Get("IinterstitialProfilepopuptext11");
            Profilepopuptextvalue3.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().interstitialPageInfo.balls_potted_two, "number");
            LeftProfilepopuptextvalue3.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().interstitialPageInfo.balls_potted_one, "number");

            Profilepopuptext4.text = Localization.Get("IinterstitialProfilepopuptext12");
            Profilepopuptextvalue4.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().interstitialPageInfo.balls_missed_two, "number");
            LeftProfilepopuptextvalue4.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().interstitialPageInfo.balls_missed_one, "number");

            Profilepopuptext5.text = Localization.Get("IinterstitialProfilepopuptext13");
            Profilepopuptextvalue5.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().interstitialPageInfo.out_of_position_two, "number");
            LeftProfilepopuptextvalue5.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().interstitialPageInfo.out_of_position_one, "number");

            Profilepopuptext6.text = Localization.Get("IinterstitialProfilepopuptext14");
            Profilepopuptextvalue6.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().interstitialPageInfo.scratches_two, "number");
            LeftProfilepopuptextvalue6.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().interstitialPageInfo.scratches_one, "number");
        }
    }

    public void Update()
    {
        if (TotalScoreBool)
        {
            CurrentMoney += UpdateMoney;

            MoneyText.text = Localization.Get("IinterstitialProfilepopuptext0") + GameManager_script.convertNumberIntoGoodStringFormat(CurrentMoney, "gamecoinz");

            if (CurrentMoney > GameManager_script.Instance().CurrentWager * 2.0f)
            {
                MoneyText.text = Localization.Get("IinterstitialProfilepopuptext0") + GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().CurrentWager * 2.0f, "gamecoinz");

                TotalScoreBool = false;
            }
        }
    }

    public void ToggleRematchButton(bool inShow)
    {
        PlayButton.SetActive(inShow);
        PlayButtonBackground.gameObject.SetActive(inShow);
        PlayButtonLabel.gameObject.SetActive(inShow);
        PlayButtonMask.gameObject.SetActive(inShow);

        RTTBox.gameObject.SetActive(inShow);
        RTTArrow.gameObject.SetActive(inShow);
        RTTText.gameObject.SetActive(inShow);
    }

    public void ToggleRematchToolTip(bool inShow, string inText)
    {
        if (!HideRematchAnyways)
        {
            if (inText == "RematchNo")
            {
                // no can sub wait and yes, and only sub in others, coz we want to tell people ya!
                if (RTTText.gameObject.activeSelf && (RTTText.text == Localization.Get("RematchYes") || RTTText.text == Localization.Get("RematchWait")))
                {
                    RTTBox.gameObject.SetActive(inShow);
                    RTTArrow.gameObject.SetActive(inShow);
                    RTTText.gameObject.SetActive(inShow);

                    RTTText.text = Localization.Get(inText);
                }
            }

            if (inText == "RematchYes")
            {
                // yes can appear any time but only thing cannot do is cannot sub no coinz
                if (!(RTTText.gameObject.activeSelf && RTTText.text == Localization.Get("RematchNoCoin")))
                {
                    RTTBox.gameObject.SetActive(inShow);
                    RTTArrow.gameObject.SetActive(inShow);
                    RTTText.gameObject.SetActive(inShow);

                    RTTText.text = Localization.Get(inText);
                }
            }

            if (inText == "RematchWait")
            {
                // wait cannot sub nothing so ya... only has to appear out of the blue
                if (!RTTText.gameObject.activeSelf)
                {
                    RTTBox.gameObject.SetActive(inShow);
                    RTTArrow.gameObject.SetActive(inShow);
                    RTTText.gameObject.SetActive(inShow);

                    RTTText.text = Localization.Get(inText);
                }
            }

            if (inText == "RematchNoCoin")
            {
                // no coinz can show up any time...
                if (true)
                {
                    RTTBox.gameObject.SetActive(inShow);
                    RTTArrow.gameObject.SetActive(inShow);
                    RTTText.gameObject.SetActive(inShow);

                    RTTText.text = Localization.Get(inText);
                }
            }
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
}
