using UnityEngine;
using System.Collections;

public class GameCenter : MonoBehaviour
{
    public UIPanel RootPanel;
    public GameObject Exit;
    public Vector2 ExitBackgroundXY;
    public GameObject Cue;
    public Vector2 CueBackgroundXY;
    public GameObject Head1;
    public GameObject Head2;
    public UILabel PlayerName1;
    public UILabel PlayerName2;
    public UILabel CenterLabelRed;
    public UILabel CenterLabelBlue;
    public Vector2 PlayerBackgroundXY;
    public Vector2 HeadBackgroundXY;
    public UIPanel BallScrollView;
    public Vector2 BallBackgroundXY;
    public UISprite[] Balls;
    public UIGrid Grid;
    public GameObject SpinShadow;
    public Vector2 SpinShadowBackgroundXY;
    public GameObject BallReturn;
    public UISprite Star1;
    public UISprite Star2;
    public UISprite HeadBoxBackground;
    public UILabel GameOverLabelLose;
    public UILabel GameOverLabelWin;
    public UILabel FTUEStarLabel;
    public GameOverType gameOverType;
    public GameObject Firework;

    public UISprite FtueBox;
    public UISprite FtueArrow;
    public UILabel FtueActualText;

    public UISprite LCBox;
    public UISprite LCArrow;
    public UILabel LCText;

    public UISprite RCBox;
    public UISprite RCArrow;
    public UILabel RCText;

    [System.NonSerialized]
    public float ToolTipWidthFactor = 0.0f;

    public UISprite ToolTipBoxBackground;
    public UILabel ToolTipTitle;

    public GameObject OkButton;
    public UISprite OkButtonBackground;
    public UISprite OkButtonMask;
    public UILabel OkButtonLabel;

    public GameObject CanelButton;
    public UISprite CanelButtonBackground;
    public UISprite CanelButtonMask;
    public UILabel CanelButtonLabel;

    [System.NonSerialized]
    public string ToolTipMainText = "";
    [System.NonSerialized]
    public string ToolTipCurrentType = ""; // push, skip, help
    [System.NonSerialized]
    public bool ToolTipCurrentActive = false;

    [System.NonSerialized]
    public bool FtueIsShown = false;
    [System.NonSerialized]
    public float FtueIsShownCounter = 0.0f;

    void Start()
    {
        UI();
    }

    void Update()
    {
        if (FtueIsShown)
        {
            FtueIsShownCounter += Time.deltaTime;

            if (FtueIsShownCounter > 4.5f)
            {
                float lerp = FtueIsShownCounter % 4.5f;

                if (lerp < 2.25f)
                {
                    float lerpAlpha = 2.25f - lerp;

                    FtueBox.alpha = lerpAlpha;
                    FtueArrow.alpha = lerpAlpha;
                    FtueActualText.alpha = lerpAlpha;
                }
                else
                {
                    float lerpAlpha = lerp - 2.25f;

                    FtueBox.alpha = lerpAlpha;
                    FtueArrow.alpha = lerpAlpha;
                    FtueActualText.alpha = lerpAlpha;
                }
            }
        }
    }

    void UI()
    {
        float top_padding = GameManager_script.Instance().Top_Padding;
        float top = GameManager_script.Instance().Top_Gap;
        float mid_padding = GameManager_script.Instance().Mid_Gap;
        float bot_padding = GameManager_script.Instance().Bot_Gap;
        float side = GameManager_script.Instance().Side_Gap;
        
        //关闭按钮
        ExitBackgroundXY.x = top * 0.675f;
        ExitBackgroundXY.y = top * 0.675f;
        Exit.GetComponent<GameExitButton>().BackgroundXY = new Vector2(ExitBackgroundXY.x, ExitBackgroundXY.y);
        Exit.transform.localPosition = new Vector3(top * 0.075f + RootPanel.width * -0.5f + ExitBackgroundXY.x * 0.5f, top * -0.075f + RootPanel.height / 2.0f - ExitBackgroundXY.y / 2 - top_padding, 0.0f);
        Exit.SetActive(GameManager_script.Instance().FTUEInActionGame ? false : true);
        Exit.GetComponent<GameExitButton>().UI();

        //Spin
        SpinShadowBackgroundXY.x = top * 0.675f;
        SpinShadowBackgroundXY.y = top * 0.675f;
        SpinShadow.GetComponent<GameSpinShadow>().BackgroundXY = new Vector2(SpinShadowBackgroundXY.x, SpinShadowBackgroundXY.y);
        SpinShadow.GetComponent<GameSpinShadow>().SpinShadowUI();
        SpinShadow.transform.localPosition = new Vector3(top * -0.075f + RootPanel.width * 0.5f - SpinShadowBackgroundXY.x * 0.5f, top * -0.075f + RootPanel.height / 2.0f - ExitBackgroundXY.y / 2 - top_padding, 0.0f);

        // 球杆
        float cueBGRatio = Cue.GetComponent<GameCueBackground>().CueBackground.height / Cue.GetComponent<GameCueBackground>().CueBackground.width;
        CueBackgroundXY.y = (Screen.height - top_padding - top - mid_padding - bot_padding) * 0.75f; ;
        CueBackgroundXY.x = CueBackgroundXY.y / cueBGRatio;
        Cue.GetComponent<GameCueBackground>().BackgroundXY = new Vector2(CueBackgroundXY.x, CueBackgroundXY.y);
        Cue.transform.localPosition = new Vector3(RootPanel.width * -0.5f + CueBackgroundXY.x * 0.5f, 0.0f - (top_padding + top + mid_padding - bot_padding) * 0.5f, 0.0f);
        Cue.GetComponent<GameCueBackground>().UI();

        //头像
        HeadBackgroundXY.x = top * 0.775f;
        HeadBackgroundXY.y = top * 0.775f;

        Head1.GetComponent<GamePlayerHead>().BackgroundXY = new Vector2(HeadBackgroundXY.x, HeadBackgroundXY.y);
        Head1.GetComponent<GamePlayerHead>().UI();
        Head1.transform.localPosition = new Vector3(RootPanel.width * -0.5f + HeadBackgroundXY.x * 0.5f + HeadBackgroundXY.x + side, RootPanel.height * 0.5f - top_padding - top * 0.5f, 0.0f);
        ChangeHead1Image(GameManager_script.Instance().AvatarEquipped);
        ChangeHead1Name();

        Head2.GetComponent<GamePlayerHead>().BackgroundXY = new Vector2(HeadBackgroundXY.x, HeadBackgroundXY.y);
        Head2.GetComponent<GamePlayerHead>().UI();
        Head2.transform.localPosition = new Vector3(RootPanel.width * 0.5f - HeadBackgroundXY.x * 0.5f - HeadBackgroundXY.x - side, RootPanel.height * 0.5f - top_padding - top * 0.5f, 0.0f);
        ChangeHead2Image();
        ChangeHead2Name();

        //九球
        BallScrollView.SetRect(0, 0, RootPanel.width * 2, RootPanel.height * 2.0f); // this is arbitrarily large anyways

        // gotta be in between them chicklets
        float widthBasedBallWidth = Mathf.Abs(Head2.transform.localPosition.x - Head1.transform.localPosition.x - HeadBackgroundXY.x) * 0.11f * 0.65f;
        float heightBasedBallWidth = HeadBackgroundXY.y * 0.50f; // slightly taller than 0.5f i guess...

        float Ballwidth = widthBasedBallWidth > heightBasedBallWidth ? heightBasedBallWidth : widthBasedBallWidth;
        float BallDistance = Ballwidth * 0.35f / 0.65f;

        for (int i = 0; i < Balls.Length; i++)
        {
            BallBackgroundXY.x = Ballwidth;
            BallBackgroundXY.y = Ballwidth;

            Balls[i].GetComponent<GameBall>().BackgroundXY = new Vector2(BallBackgroundXY.x, BallBackgroundXY.y);
            Balls[i].GetComponent<GameBall>().UI();

            Grid.cellWidth = BallDistance + BallBackgroundXY.x;
            Grid.Reposition();

            Balls[i].GetComponent<GameBall>().ChangeBall(InGameBallDisplayInfo.Open);
        }

        float BallDisplayTotalWidth = BallDistance * 8.0f + Ballwidth * 9.0f;

        // this unique co-dependent thing, we need to redo chicklet's position based on that of the balls in the middle
        // they need to be equally spaced out comparing to the rest of the balls
        Head1.transform.localPosition = new Vector3(-BallDisplayTotalWidth * 0.5f - HeadBackgroundXY.x * 0.5f - BallDistance * 1.5f, RootPanel.height * 0.5f - top_padding - top * 0.5f, 0.0f);
        Head2.transform.localPosition = new Vector3(BallDisplayTotalWidth * 0.5f + HeadBackgroundXY.x * 0.5f + BallDistance * 1.5f, RootPanel.height * 0.5f - top_padding - top * 0.5f, 0.0f);

        // box that covers the balls and names and shiites (only do it after we firmed up on the head transformations.
        HeadBoxBackground.height = (int)(top * 0.85f); // beats the 0.8f on the header
        HeadBoxBackground.width = (int)(Head2.transform.localPosition.x - Head1.transform.localPosition.x - HeadBackgroundXY.x - BallDistance * 1.0f);
        HeadBoxBackground.transform.localPosition = new Vector3(0.0f, Head1.transform.localPosition.y + HeadBoxBackground.height * 0.025f, 0.0f); // a very subtle offset

        float DisplayBoundboxRatio = HeadBoxBackground.width / HeadBoxBackground.height;

        if (DisplayBoundboxRatio <= 3.95f) // 3.88 was measured
        {
            HeadBoxBackground.spriteName = "SmallHeadBoxBackground";
        }
        else if (DisplayBoundboxRatio <= 4.45f)
        {
            HeadBoxBackground.spriteName = "HeadBoxBackground4.0";
        }
        else if (DisplayBoundboxRatio <= 5.35f)
        {
            HeadBoxBackground.spriteName = "HeadBoxBackground4.9";
        }
        else if (DisplayBoundboxRatio <= 6.25f)
        {
            HeadBoxBackground.spriteName = "HeadBoxBackground5.8";
        }
        else
        {
            HeadBoxBackground.spriteName = "HeadBoxBackground6.7";
        }

        //玩家名字
        PlayerName1.width = (int)(BallDisplayTotalWidth * 0.30f); // special case using RootPanel.height
        PlayerName1.height = (int)(0.4f * HeadBackgroundXY.y);
        PlayerName1.transform.localPosition = new Vector3(-BallDisplayTotalWidth * 0.5f + PlayerName1.width * 0.5f, Head1.transform.localPosition.y + HeadBoxBackground.height * 0.5f * 0.5f - HeadBoxBackground.height * 0.015f, 0.0f); ;

        PlayerName2.width = (int)(BallDisplayTotalWidth * 0.30f); // special case using RootPanel.height
        PlayerName2.height = (int)(0.4f * HeadBackgroundXY.y);
        PlayerName2.transform.localPosition = new Vector3(BallDisplayTotalWidth * 0.5f - PlayerName1.width * 0.5f, Head1.transform.localPosition.y + HeadBoxBackground.height * 0.5f * 0.5f - HeadBoxBackground.height * 0.015f, 0.0f); ;

        // CenterLabel 1
        CenterLabelRed.width = (int)(BallDisplayTotalWidth * 0.33f);
        CenterLabelRed.height = (int)(0.5f * HeadBackgroundXY.y);
        CenterLabelRed.transform.localPosition = new Vector3(0.0f, PlayerName1.transform.localPosition.y, 0.0f);

        // center label 2
        CenterLabelBlue.width = (int)(BallDisplayTotalWidth * 0.33f);
        CenterLabelBlue.height = (int)(0.5f * HeadBackgroundXY.y);
        CenterLabelBlue.transform.localPosition = new Vector3(0.0f, PlayerName2.transform.localPosition.y, 0.0f);

        // vs or coin count
        if (GameManager_script.Instance().StartingOutAsANetWorkGame)
        {
            if (GameManager_script.Instance().rematchCurrentMatchIsRematch && GameManager_script.Instance().rematchSmartBotSeries)
            {
                ChangeCenterLabel("Red", GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().rematchYourWinCount, "number") + Localization.Get("SpacedMaoHao") + GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().rematchOppoWinCount, "number"));
            }
            else
            {
                ChangeCenterLabel("Red", GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().CurrentWager * 2.0f, "gamecoinz"));
            }
        }
        else
        {
            ChangeCenterLabel("Blue", Localization.Get("Versus"));
        }

        // ball display position
        BallScrollView.transform.localPosition = new Vector3(BallDisplayTotalWidth * -0.5f + Ballwidth * 0.5f, Head1.transform.localPosition.y - HeadBoxBackground.height * 0.5f * 0.5f * 0.8f, 0.0f);

        //BallReturn
        BallReturn.transform.localScale = new Vector3(1, 1, 1);

        //Star
        Star1.height = Mathf.CeilToInt(HeadBackgroundXY.x * 0.60f);
        Star1.width = Mathf.CeilToInt(HeadBackgroundXY.x * 0.60f);
        Star1.transform.localPosition = new Vector3(Head1.transform.localPosition.x - HeadBackgroundXY.x * 0.85f, Head1.transform.localPosition.y + HeadBackgroundXY.y * 0.1f, 0);
        Star1.GetComponent<UIStar>().Text.width = GameManager_script.Instance().StarTextFineTuneWidth(Star1.width, GameManager_script.Instance().CurrentLevel);
        Star1.GetComponent<UIStar>().Text.height = GameManager_script.Instance().StarTextFineTuneHeight(Star1.height, GameManager_script.Instance().CurrentLevel);
        Star1.GetComponent<UIStar>().Text.transform.localPosition = GameManager_script.Instance().StarTextFineTunePosition(Star1.GetComponent<UIStar>().Text.transform.localPosition, Star1.GetComponent<UIStar>().Text.width, Star1.GetComponent<UIStar>().Text.height, GameManager_script.Instance().CurrentLevel);
        Star1.GetComponent<UIStar>().StartUI(new StarInfo(GameManager_script.Instance().selfGameProfileInfo.Star.text, GameManager_script.Instance().selfGameProfileInfo.Star.starType));

        Star2.height = Mathf.CeilToInt(HeadBackgroundXY.x * 0.60f);
        Star2.width = Mathf.CeilToInt(HeadBackgroundXY.x * 0.60f);
        Star2.transform.localPosition = new Vector3(Head2.transform.localPosition.x + HeadBackgroundXY.x * 0.85f, Head2.transform.localPosition.y + HeadBackgroundXY.y * 0.1f, 0);
        Star2.GetComponent<UIStar>().Text.width = GameManager_script.Instance().StarTextFineTuneWidth(Star2.width, GameManager_script.Instance().CurrentLevel);
        Star2.GetComponent<UIStar>().Text.height = GameManager_script.Instance().StarTextFineTuneHeight(Star2.height, GameManager_script.Instance().CurrentLevel);
        Star2.GetComponent<UIStar>().Text.transform.localPosition = GameManager_script.Instance().StarTextFineTunePosition(Star2.GetComponent<UIStar>().Text.transform.localPosition, Star2.GetComponent<UIStar>().Text.width, Star2.GetComponent<UIStar>().Text.height, GameManager_script.Instance().CurrentLevel);

        if (GameManager_script.Instance().otherGameProfileInfo != null)
        {
            Star2.GetComponent<UIStar>().StartUI(new StarInfo(GameManager_script.Instance().otherGameProfileInfo.Star.text, GameManager_script.Instance().otherGameProfileInfo.Star.starType));
        }
        else
        {
            Star2.GetComponent<UIStar>().StartUI(new StarInfo(1, StarType.bronze));
        }

        // GameOverLabelWin
        GameOverLabelWin.width = (int)(RootPanel.width * 0.35f);
        GameOverLabelWin.height = (int)(GameOverLabelWin.width * 0.35f);
        GameOverLabelWin.transform.localPosition = Vector3.zero;
        GameOverLabelWin.GetComponent<TweenScale>().animationCurve = GameManager_script.Instance().NGUITweenScale;

        // GameOverLabelLose
        GameOverLabelLose.width = (int)(RootPanel.width * 0.35f);
        GameOverLabelLose.height = (int)(GameOverLabelLose.width * 0.35f);
        GameOverLabelLose.transform.localPosition = Vector3.zero;
        GameOverLabelLose.GetComponent<TweenScale>().animationCurve = GameManager_script.Instance().NGUITweenScale;
        
        // ya change it
        ChangeGameOverLabel(GameOverType.None);

        // FTUE star bubble (not related to the actual ftue stuffz, well sort of)
        FTUEStarLabel.width = (int)(RootPanel.width * 0.45f);
        FTUEStarLabel.height = (int)(FTUEStarLabel.width * 0.45f);
        FTUEStarLabel.transform.localPosition = Vector3.zero;
        FTUEStarLabel.GetComponent<TweenScale>().animationCurve = GameManager_script.Instance().NGUITweenScale;

        // remember that this has been shown
        if (GameManager_script.Instance().FTUEInActionGame)
        {
            GameManager_script.Instance().SeenSwipeAndPullEver = 1.0f;
            PlayerPrefs.SetFloat("SeenSwipeAndPullEver", GameManager_script.Instance().SeenSwipeAndPullEver);
        }

        // tuck away all these sprites regardless
        TuckAwayFtueAssets();

        // prepare locale specific stuff
        if (GameManager_script.Instance().Savelanguage == "简体中文" || GameManager_script.Instance().Savelanguage == "繁體中文" || GameManager_script.Instance().Savelanguage == "日本語")
        {
            ToolTipWidthFactor = 1.050f;
        }
        else
        {
            ToolTipWidthFactor = 0.475f;
        }

        // init all the ui components
        CleanUpAllToolTipRelatedUI();

        // chats
        InitLeftChatBubble();
        InitRiteChatBubble();
    }

    public void CleanUpAllToolTipRelatedUI()
    {
        // const
        ToolTipCurrentActive = false;
        ToolTipCurrentType = "";
        ToolTipMainText = "";

        // ui
        ToolTipBoxBackground.gameObject.SetActive(false);
        ToolTipTitle.gameObject.SetActive(false);
        CanelButton.SetActive(false);
        OkButton.SetActive(false);
    }

    public void ToolTipShowPushOutOption()
    {
        // const
        ToolTipMainText = Localization.Get("ToolTip1");
        ToolTipCurrentType = "push";
        ToolTipCurrentActive = true;

        // ui
        ToolTipShowButtonOption();
    }

    public void ToolTipShowSkipOption()
    {
        // const
        ToolTipMainText = Localization.Get("ToolTip2");
        ToolTipCurrentType = "skip";
        ToolTipCurrentActive = true;

        // ui
        ToolTipShowButtonOption();
    }

    public void ToolTipShowButtonOption()
    {
        // ui
        float idealFontSize = Screen.height / 22;
        float idealHeight = idealFontSize * 1.50f;
        float idealWidth = idealFontSize * ToolTipMainText.Length * ToolTipWidthFactor;

        ToolTipBoxBackground.width = (int)(idealWidth + idealHeight * 5.0f);
        ToolTipBoxBackground.height = (int)idealHeight;
        ToolTipBoxBackground.transform.localPosition = new Vector3(0.0f, Screen.height * -0.5f + idealHeight * 0.5f, 0.0f);
        ToolTipBoxBackground.transform.localScale = Vector3.one;
        ToolTipBoxBackground.alpha = 0.925f;

        ToolTipTitle.width = (int)(idealWidth * 0.99f);
        ToolTipTitle.height = (int)(idealHeight * 0.60f);
        ToolTipTitle.transform.localPosition = new Vector3(idealHeight * -2.5f, Screen.height * -0.5f + idealHeight * 0.5f, 0.0f);
        ToolTipTitle.transform.localScale = Vector3.one;
        ToolTipTitle.fontSize = (int)idealFontSize;
        ToolTipTitle.text = ToolTipMainText;

        CanelButtonBackground.height = (int)(idealHeight * 0.75f);
        CanelButtonBackground.width = (int)(CanelButtonBackground.height * 3.0f);
        CanelButtonMask.height = CanelButtonBackground.height + 2;
        CanelButtonMask.width = CanelButtonBackground.width + 2;
        CanelButtonMask.GetComponent<BoxCollider>().size = new Vector3(CanelButtonMask.width, CanelButtonMask.height, 0);
        CanelButton.transform.localPosition = new Vector3(ToolTipBoxBackground.width * 0.5f - idealHeight * 1.35f, Screen.height * -0.5f + idealHeight * 0.5f, 0.0f);
        CanelButton.transform.localScale = Vector3.one;

        CanelButtonLabel.width = (int)(CanelButtonBackground.width * 0.9f);
        CanelButtonLabel.height = (int)(CanelButtonBackground.height * 0.9f);
        CanelButtonLabel.transform.localPosition = new Vector3(CanelButtonLabel.transform.localPosition.x, CanelButtonLabel.transform.localPosition.y - CanelButtonLabel.height * 0.05f, CanelButtonLabel.transform.localPosition.z);
        CanelButtonLabel.text = Localization.Get("No");
        CanelButtonLabel.fontSize = Screen.height / 30;

        OkButtonBackground.height = (int)(idealHeight * 0.75f);
        OkButtonBackground.width = (int)(CanelButtonBackground.height * 3.0f);
        OkButtonMask.height = OkButtonBackground.height + 2;
        OkButtonMask.width = OkButtonBackground.width + 2;
        OkButtonMask.GetComponent<BoxCollider>().size = new Vector3(OkButtonMask.width, OkButtonMask.height, 0);
        OkButton.transform.localPosition = new Vector3(ToolTipBoxBackground.width * 0.5f - idealHeight * 3.85f, Screen.height * -0.5f + idealHeight * 0.5f, 0.0f);
        OkButton.transform.localScale = Vector3.one;

        OkButtonLabel.width = (int)(OkButtonBackground.width * 0.9f);
        OkButtonLabel.height = (int)(OkButtonBackground.height * 0.9f);
        OkButtonLabel.transform.localPosition = new Vector3(OkButtonLabel.transform.localPosition.x, OkButtonLabel.transform.localPosition.y - OkButtonLabel.height * 0.05f, OkButtonLabel.transform.localPosition.z);
        OkButtonLabel.text = Localization.Get("Yes");
        OkButtonLabel.fontSize = Screen.height / 30;

        ToolTipBoxBackground.gameObject.SetActive(true);
        ToolTipTitle.gameObject.SetActive(true);
        CanelButton.SetActive(true);
        OkButton.SetActive(true);
    }

    public void ToolTipShowHelpOption(string inString)
    {
        // const
        ToolTipMainText = inString;
        ToolTipCurrentType = "help";
        ToolTipCurrentActive = true;

        // ui
        float idealFontSize = Screen.height / 22;
        float idealHeight = idealFontSize * 1.50f;
        float idealWidth = idealFontSize * inString.Length * ToolTipWidthFactor;

        ToolTipBoxBackground.width = (int)idealWidth;
        ToolTipBoxBackground.height = (int)idealHeight;
        ToolTipBoxBackground.transform.localPosition = new Vector3(0.0f, Screen.height * -0.5f + idealHeight * 0.5f, 0.0f);
        ToolTipBoxBackground.transform.localScale = Vector3.one;
        ToolTipBoxBackground.alpha = 0.925f;

        ToolTipTitle.width = (int)(idealWidth * 0.99f);
        ToolTipTitle.height = (int)(idealHeight * 0.60f);
        ToolTipTitle.transform.localPosition = new Vector3(0.0f, Screen.height * -0.5f + idealHeight * 0.5f, 0.0f);
        ToolTipTitle.transform.localScale = Vector3.one;
        ToolTipTitle.fontSize = (int)idealFontSize;
        ToolTipTitle.text = inString;

        ToolTipBoxBackground.gameObject.SetActive(true);
        ToolTipTitle.gameObject.SetActive(true);
    }

    public void TuckAwayFtueAssets()
    {
        FtueIsShown = false;

        if (FtueBox)
        {
            FtueBox.transform.localPosition = new Vector3(9999.0f, 9999.0f, 9999.0f);
            FtueBox.alpha = 0.0f;
        }

        if (FtueArrow)
        {
            FtueArrow.transform.localPosition = new Vector3(9999.0f, 9999.0f, 9999.0f);
            FtueArrow.alpha = 0.0f;
        }

        if (FtueActualText)
        {
            FtueActualText.transform.localPosition = new Vector3(9999.0f, 9999.0f, 9999.0f);
            FtueActualText.alpha = 0.0f;
        }
    }

    public void InitLeftChatBubble()
    {
        // box
        LCBox.width = (int)(HeadBoxBackground.width * 0.50f);
        LCBox.height = (int)(LCBox.width * 0.20f);
        LCBox.transform.localPosition = Head1.transform.localPosition + new Vector3(0.20f * LCBox.width, -0.95f * LCBox.height - 0.5f * Head1.GetComponent<GamePlayerHead>().BackgroundXY.y, 0.0f); // dajiang hack, using HeadBoxBackground as a reference
        LCBox.alpha = 0.0f;

        // arrow
        LCArrow.width = (int)(LCBox.width * 0.175f);
        LCArrow.height = (int)(LCArrow.width * 0.550f);
        LCArrow.transform.localRotation = Quaternion.Euler(0, 0, 180);
        LCArrow.transform.localPosition = LCBox.transform.localPosition + new Vector3(-0.20f * LCBox.width, 0.695f * LCBox.height, 0.0f);
        LCArrow.alpha = 0.0f;

        // text
        LCText.width = (int)(LCBox.width * 0.95f);
        LCText.height = (int)(LCBox.height * 0.90f);
        LCText.text = "";
        LCText.transform.localPosition = LCBox.transform.localPosition;
        LCText.alpha = 0.0f;
    }

    public void InitRiteChatBubble()
    {
        // box
        RCBox.width = (int)(HeadBoxBackground.width * 0.50f);
        RCBox.height = (int)(RCBox.width * 0.20f);
        RCBox.transform.localPosition = Head2.transform.localPosition + new Vector3(-0.20f * RCBox.width, -0.95f * RCBox.height - 0.5f * Head2.GetComponent<GamePlayerHead>().BackgroundXY.y, 0.0f); // dajiang hack, using HeadBoxBackground as a reference
        RCBox.alpha = 0.0f;

        // arrow
        RCArrow.width = (int)(RCBox.width * 0.175f);
        RCArrow.height = (int)(RCArrow.width * 0.550f);
        RCArrow.transform.localRotation = Quaternion.Euler(0, 0, 180);
        RCArrow.transform.localPosition = RCBox.transform.localPosition + new Vector3(0.20f * RCBox.width, 0.695f * RCBox.height, 0.0f);
        RCArrow.alpha = 0.0f;

        // text
        RCText.width = (int)(RCBox.width * 0.95f);
        RCText.height = (int)(RCBox.height * 0.90f);
        RCText.text = "";
        RCText.transform.localPosition = RCBox.transform.localPosition;
        RCText.alpha = 0.0f;
    }

    public void ShowLeftChatBubble(string inString)
    {
        LCText.text = Localization.Get(inString);

        LCBox.alpha = 1.0f;
        LCArrow.alpha = 1.0f;
        LCText.alpha = 1.0f;

        StopCoroutine(HideLeftChatBubble());
        StartCoroutine(HideLeftChatBubble());
    }

    public void ShowRiteChatBubble(string inString)
    {
        RCText.text = Localization.Get(inString);

        RCBox.alpha = 1.0f;
        RCArrow.alpha = 1.0f;
        RCText.alpha = 1.0f;

        StopCoroutine(HideRiteChatBubble());
        StartCoroutine(HideRiteChatBubble());
    }

    public IEnumerator HideLeftChatBubble()
    {
        yield return new WaitForSeconds(5.00f);

        LCBox.alpha = 0.0f;
        LCArrow.alpha = 0.0f;
        LCText.alpha = 0.0f;
    }

    public IEnumerator HideRiteChatBubble()
    {
        yield return new WaitForSeconds(5.00f);

        RCBox.alpha = 0.0f;
        RCArrow.alpha = 0.0f;
        RCText.alpha = 0.0f;
    }

    public void ShowFtueTextBubbleSwipe()
    {
        FtueBox.width = (int)(HeadBoxBackground.width * 0.75f);
        FtueBox.height = (int)(FtueBox.width * 0.20f);
        FtueBox.transform.localPosition = new Vector3(Screen.width * 0.5f - FtueBox.width * 0.5f - GameManager_script.Instance().Side_Gap, Screen.height * 0.5f - FtueBox.height * 0.475f - GameManager_script.Instance().Top_Gap - GameManager_script.Instance().Top_Padding - GameManager_script.Instance().Mid_Gap, 0.0f); // dajiang hack, using HeadBoxBackground as a reference
        FtueBox.alpha = 1.0f;

        FtueArrow.width = (int)(FtueBox.width * 0.175f);
        FtueArrow.height = (int)(FtueArrow.width * 0.550f);
        FtueArrow.transform.localPosition = FtueBox.transform.localPosition + new Vector3(0.20f * FtueBox.width, -0.700f * FtueBox.height, 0.0f);
        FtueArrow.alpha = 1.0f;

        FtueActualText.width = (int)(FtueBox.width * 0.95f);
        FtueActualText.height = (int)(FtueBox.height * 0.90f);
        FtueActualText.text = Localization.Get("FtueSwipe");
        FtueActualText.transform.localPosition = FtueBox.transform.localPosition;
        FtueActualText.alpha = 1.0f;

        FtueIsShown = true;
        FtueIsShownCounter = 0.0f;
    }

    public void ShowFtueTextBubbleShoot()
    {
        // define locationz
        FtueBox.width = (int)(HeadBoxBackground.width * 0.75f);
        FtueBox.height = (int)(FtueBox.width * 0.20f);
        FtueBox.transform.localPosition = new Vector3(FtueBox.width * 0.5f + FtueBox.width * 0.070f + CueBackgroundXY.x - Screen.width * 0.5f, CueBackgroundXY.y * 0.25f, 0.0f); // dajiang hack, using HeadBoxBackground as a reference
        FtueBox.alpha = 1.0f;

        FtueArrow.width = (int)(FtueBox.width * 0.125f);
        FtueArrow.height = (int)(FtueArrow.width * 0.550f);
        FtueArrow.transform.localRotation = Quaternion.Euler(0, 0, -90);
        FtueArrow.transform.localPosition = FtueBox.transform.localPosition + new Vector3(-0.52675f * FtueBox.width, 0.0f * FtueBox.height, 0.0f);
        FtueArrow.alpha = 1.0f;

        FtueActualText.width = (int)(FtueBox.width * 0.95f);
        FtueActualText.height = (int)(FtueBox.height * 0.90f);
        FtueActualText.text = Localization.Get("FtueShoot");
        FtueActualText.transform.localPosition = FtueBox.transform.localPosition;
        FtueActualText.alpha = 1.0f;
        
        FtueIsShown = true;
        FtueIsShownCounter = 0.0f;
    }

    public void ChangeHead1Image(int index)
    {
        Head1.GetComponent<GamePlayerHead>().Head.spriteName = "" + index;
        Head1.GetComponent<GamePlayerHead>().LightBackground.spriteName = "" + index;
    }

    public void ChangeHead2Image()
    {
        if (GameManager_script.Instance().otherGameProfileInfo != null)
        {
            Head2.GetComponent<GamePlayerHead>().Head.spriteName = "" + GameManager_script.Instance().otherGameProfileInfo.HeadImage;
            Head2.GetComponent<GamePlayerHead>().LightBackground.spriteName = "" + GameManager_script.Instance().otherGameProfileInfo.HeadImage;
        }
        else
        {
            Head2.GetComponent<GamePlayerHead>().Head.spriteName = "0";
            Head2.GetComponent<GamePlayerHead>().LightBackground.spriteName = "0";
        }
    }

    public void ChangeHead1Name()
    {
        if (GameManager_script.Instance().TrulySelfInActionGame || GameManager_script.Instance().FTUEInActionGame)
        {
            PlayerName1.text = Localization.Get("SoloPlayerOneName");
        }
        else
        {
            PlayerName1.text = Localization.Get(GameManager_script.Instance().CharLength(GameManager_script.Instance().First_Name, GameManager_script.Instance().Max_Name_Length));
        }
    }

    public void ChangeHead2Name()
    {
        if (GameManager_script.Instance().otherGameProfileInfo != null)
        {
            if (GameManager_script.Instance().TrulySelfInActionGame || GameManager_script.Instance().FTUEInActionGame)
            {
                PlayerName2.text = Localization.Get("SoloPlayerTwoName");
            }
            else
            {
                PlayerName2.text = Localization.Get(GameManager_script.Instance().CharLength(GameManager_script.Instance().otherGameProfileInfo.PlayerName, GameManager_script.Instance().Max_Name_Length));
            }
        }
        else
        {
            PlayerName2.text = Localization.Get("YourOpponent");
        }
    }

    public void changeStarImage1(StarType VstarType)
    {
        Star1.GetComponent<UIStar>().changeImage(VstarType);
    }

    public void changeStarText1(string text)
    {
        Star1.GetComponent<UIStar>().changeText(text);
    }

    public void changeStarImage2(StarType VstarType)
    {
        Star2.GetComponent<UIStar>().changeImage(VstarType);
    }

    public void changeStarText2(string text)
    {
        Star2.GetComponent<UIStar>().changeText(text);
    }

    public void ChangeCenterLabel(string Color, string text)
    {
        if (Color == "Blue")
        {
            CenterLabelBlue.gameObject.SetActive(true);
            CenterLabelRed.gameObject.SetActive(false);

            CenterLabelBlue.text = text;
        }
        else if (Color == "Red")
        {
            CenterLabelBlue.gameObject.SetActive(false);
            CenterLabelRed.gameObject.SetActive(true);

            CenterLabelRed.text = text;
        }
    }

    public void ChangeGameOverLabel(GameOverType vGameOverType)
    {
        if (vGameOverType == GameOverType.Win)
        {
            GameOverLabelWin.text = Localization.Get("GameOverLabelWin");

            FTUEStarLabel.gameObject.SetActive(false);
            GameOverLabelWin.gameObject.SetActive(true);
            GameOverLabelLose.gameObject.SetActive(false);
            Firework.SetActive(true);

            StartCoroutine(PlayEndofGameSound(true));
        }
        else if (vGameOverType == GameOverType.Lose)
        {
            GameOverLabelLose.text = Localization.Get("GameOverLabelLose");

            FTUEStarLabel.gameObject.SetActive(false);
            GameOverLabelWin.gameObject.SetActive(false);
            GameOverLabelLose.gameObject.SetActive(true);
            Firework.SetActive(false);

            StartCoroutine(PlayEndofGameSound(false));
        }
        else if (vGameOverType == GameOverType.SoloOneWin)
        {
            GameOverLabelWin.text = Localization.Get("GameOverLabeSoloOnelWin");

            FTUEStarLabel.gameObject.SetActive(false);
            GameOverLabelWin.gameObject.SetActive(true);
            GameOverLabelLose.gameObject.SetActive(false);
            Firework.SetActive(true);

            StartCoroutine(PlayEndofGameSound(true));
        }
        else if (vGameOverType == GameOverType.SoloTwoWin)
        {
            GameOverLabelWin.text = Localization.Get("GameOverLabeSoloTwolWin");

            FTUEStarLabel.gameObject.SetActive(false);
            GameOverLabelWin.gameObject.SetActive(true);
            GameOverLabelLose.gameObject.SetActive(false);
            Firework.SetActive(true);

            StartCoroutine(PlayEndofGameSound(true));
        }
        else if (vGameOverType == GameOverType.Tutorial)
        {
            // actually give players some coinz here
            GameManager_script.Instance().CurrentBonusAmount = GameManager_script.DailyBonusArray[Random.Range((int)(GameManager_script.DailyBonusArray.Length * 0.33f), (int)(GameManager_script.DailyBonusArray.Length * 0.66f))];
            GameManager_script.Instance().UpdateCoinCount(GameManager_script.Instance().CurrentBonusAmount);

            FTUEStarLabel.text = Localization.Get("GameOverLabelTutorialWin") + GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().CurrentBonusAmount, "gamecoinz") + Localization.Get("GameOverLabelTutorialWinMark");

            FTUEStarLabel.gameObject.SetActive(true);
            GameOverLabelWin.gameObject.SetActive(false);
            GameOverLabelLose.gameObject.SetActive(false);
            Firework.SetActive(true);

            StartCoroutine(PlayEndofGameSound(true));
        }
        else // if (vGameOverType == GameOverType.None)
        {
            GameOverLabelLose.text = Localization.Get("GameOverLabelLose");

            FTUEStarLabel.gameObject.SetActive(false);
            GameOverLabelWin.gameObject.SetActive(false);
            GameOverLabelLose.gameObject.SetActive(false);
        }

        gameOverType = vGameOverType;
    }

    public IEnumerator PlayEndofGameSound(bool inWin)
    {
        // choose sound
        float e_g_volume = 1.0f;
        int e_g_index = inWin ? (int)MusicClip.Win_Over : (int)MusicClip.Lose_Over;

        // play sound
        GameManager_script.Instance().PlaySound(e_g_index, false, e_g_volume);

        yield return new WaitForSeconds(0.00f);
    }
}
