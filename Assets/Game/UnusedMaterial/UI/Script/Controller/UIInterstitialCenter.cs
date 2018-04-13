using UnityEngine;
using System.Collections;

public class UIInterstitialCenter : MonoBehaviour
{
    public UIPanel RootPanel;
    public GameObject BackGround;
    public UISprite Logo;
    public GameObject Close;
    public UIinterstitialWindow vUIinterstitialWindow;
    public UIinterstitialWindowEnd vUIinterstitialWindowEnd;
    public UISprite BoxBackground;
    public UILabel UpLabel;
    public UISprite UpStar;
    public GameObject Money;

    public float Exitsetheight = 0.1f;
    public bool ChangeBool = false;

    public GameObject TypewriterMusic = null;

    void Start()
    {
        UI();
	}

    void UI()
    {
        //BackgroundXY
        BoxBackground.width = (int)(RootPanel.width);
        BoxBackground.height = (int)(RootPanel.height);

        float Logowith1 = RootPanel.width * 0.50f;
        float Logowith2 = RootPanel.height * 0.45f * 3.342f; // 3.342f is the actual ratio of the logo png

        if (Logowith1 < Logowith2)
        {
            Logo.width = (int)Logowith1;
        }
        else
        {
            Logo.width = (int)Logowith2;
        }

        Logo.height = (int)(Logo.width / 3.342f);
        Logo.transform.localPosition = new Vector3(Logo.transform.localPosition.x, RootPanel.height * 0.5f - Logo.height * 0.5f - RootPanel.height * 0.0625f, Logo.transform.localPosition.z);

        if (RootPanel.width / RootPanel.height > 2)
        {
            BackGround.GetComponent<UISprite>().width = (int)(RootPanel.width + 1.0f);
            BackGround.GetComponent<UISprite>().height = (int)(RootPanel.height + 1.0f);
        }
        else
        {
            BackGround.GetComponent<UISprite>().width = (int)(RootPanel.height * 2 + 1.0f); // this X2 is true because we always want to maintain the ratio of 2 with width and height
            BackGround.GetComponent<UISprite>().height = (int)(RootPanel.height + 1.0f);

            float spillOverTotalLength = RootPanel.height * 2 - RootPanel.width; // this is expressed in terms of actual screen and NOT real picture's dimension
            float spillOverOnLeft = (0.63f - 0.50f) * spillOverTotalLength; // the number 0.63 was emperically determined and only works with this specific background

            BackGround.transform.localPosition = new Vector3(BackGround.transform.localPosition.x - spillOverOnLeft, BackGround.transform.localPosition.y, BackGround.transform.localPosition.z);
        }

        // 关闭按钮, dajiang hack, probably not even shown and useless
        Close.GetComponent<UIButton>().tweenTarget.GetComponent<UISprite>().width = (int)(RootPanel.height * Exitsetheight);
        Close.GetComponent<UIButton>().tweenTarget.GetComponent<UISprite>().height = (int)(RootPanel.height * Exitsetheight);
        Close.GetComponent<BoxCollider>().size = new Vector3(RootPanel.height * Exitsetheight, RootPanel.height * Exitsetheight, 0);
        Close.transform.localPosition = new Vector3(-RootPanel.width / 2.0f + RootPanel.height * Exitsetheight / 2.0f, RootPanel.height / 2.0f - RootPanel.height * Exitsetheight / 2.0f, 0.0f);

        // money
        Money.GetComponent<UISprite>().width = (int)(Screen.width * 0.175f);
        Money.GetComponent<UISprite>().height = (int)(Money.GetComponent<UISprite>().width * 0.25f);
        Money.transform.localPosition = new Vector3(Screen.width * 0.5f - Money.GetComponent<UISprite>().width * 0.5625f, Screen.height * 0.5f - Money.GetComponent<UISprite>().height * 0.75f, 0.0f);

        // interstitialWindow
        if (vUIinterstitialWindow)
        {
            vUIinterstitialWindow.BackgroundXY.x = RootPanel.width;
            vUIinterstitialWindow.BackgroundXY.y = RootPanel.height;
            vUIinterstitialWindow.interstitialWindowUI();
        }

        // interstitialWindowEnd
        if (vUIinterstitialWindowEnd)
        {
            vUIinterstitialWindowEnd.BackgroundXY.x = RootPanel.width;
            vUIinterstitialWindowEnd.BackgroundXY.y = RootPanel.height;
            vUIinterstitialWindowEnd.interstitialWindowEndUI();
        }

        //UpStar
        UpStar.width = (int)(RootPanel.width / 2 * 0.3f);
        UpStar.height = (int)(RootPanel.width / 2 * 0.3f);
        UpStar.transform.localPosition = new Vector3(0, RootPanel.height * 0.3f, 0);
        UpStar.GetComponent<UIStar>().Text.width = GameManager_script.Instance().StarTextFineTuneWidth(UpStar.width, GameManager_script.Instance().CurrentLevel);
        UpStar.GetComponent<UIStar>().Text.height = GameManager_script.Instance().StarTextFineTuneHeight(UpStar.height, GameManager_script.Instance().CurrentLevel);
        UpStar.GetComponent<UIStar>().Text.transform.localPosition = GameManager_script.Instance().StarTextFineTunePosition(UpStar.GetComponent<UIStar>().Text.transform.localPosition, UpStar.GetComponent<UIStar>().Text.width, UpStar.GetComponent<UIStar>().Text.height, GameManager_script.Instance().CurrentLevel);
        UpStar.GetComponent<TweenScale>().animationCurve = GameManager_script.Instance().NGUITweenScale;
        
        //UpLabel       
        UpLabel.height = (int)(UpStar.height / 4);
        UpLabel.width = (int)(UpLabel.height * 6);
        UpLabel.transform.localPosition = new Vector3(0, UpStar.transform.localPosition.y - UpStar.height / 2 - UpLabel.height / 2, 0);
        UpLabel.GetComponent<TweenScale>().animationCurve = GameManager_script.Instance().NGUITweenScale;
        UpLabel.height = (int)(UpStar.height / 4);
        UpLabel.width = (int)(UpLabel.height * 6);
        UpLabel.transform.localPosition = new Vector3(0, UpStar.transform.localPosition.y - UpStar.height / 2 - UpLabel.height / 2, 0);
        
        // network begin + end, bot end, solo end
        bool shouldShowInterstitial = false;
        shouldShowInterstitial = shouldShowInterstitial || GameManager_script.Instance().StartingOutAsANetWorkGame;
        shouldShowInterstitial = shouldShowInterstitial || (GameManager_script.Instance().StupidBotInActionGame && GameManager_script.Instance().interstitialPageInfo != null && !GameManager_script.Instance().interstitialPageInfo.trueIfIncomingPage);
        shouldShowInterstitial = shouldShowInterstitial || (GameManager_script.Instance().TrulySelfInActionGame && GameManager_script.Instance().interstitialPageInfo != null && !GameManager_script.Instance().interstitialPageInfo.trueIfIncomingPage);

        if (shouldShowInterstitial)
        {
            if (GameManager_script.Instance().interstitialPageInfo.trueIfIncomingPage)
            {
                if (vUIinterstitialWindow)
                {
                    vUIinterstitialWindow.gameObject.SetActive(true);

                    vUIinterstitialWindow.TotalScoreBool = true;

                    UpdateMoney(GameManager_script.Instance().CoinCount, GameManager_script.Instance().CoinCount - GameManager_script.Instance().CurrentWager);

                    Logo.gameObject.SetActive(false);

                    ChangeExitButton(false);
                }

                if (vUIinterstitialWindowEnd)
                {
                    vUIinterstitialWindowEnd.gameObject.SetActive(false);
                }
            }
            else
            {
                if (vUIinterstitialWindow)
                {
                    vUIinterstitialWindow.gameObject.SetActive(false);
                }

                if (vUIinterstitialWindowEnd)
                {
                    float musicStartTime = 0.0f;

                    vUIinterstitialWindowEnd.gameObject.SetActive(true);

                    ChangeExitButton(true);

                    if (GameManager_script.Instance().interstitialPageInfo.performLevelUp)
                    {
                        StartCoroutine(OpenLevelUpUI());

                        musicStartTime += GameManager_script.Instance().LevelUpScreenTime;
                    }

                    if (GameManager_script.Instance().StartingOutAsANetWorkGame)
                    {
                        StartCoroutine(DelayHideRematchButtonOnNetworkGame(GameManager_script.Instance().EndOfGameWaitAndChangeTime - GameManager_script.Instance().EndOfGameExitRematchHideTime));

                        StartCoroutine(DelayShowRematchTooltipOnNetworkGame(musicStartTime));
                    }

                    StartCoroutine(DelayShowCoinCountIncrement(musicStartTime, GameManager_script.Instance().interstitialPageInfo.trueIfYouWin));

                    StartCoroutine(DelayShowLogoUI(musicStartTime, GameManager_script.Instance().interstitialPageInfo.trueIfYouWin));

                    StartCoroutine(PlayCelebMusic(musicStartTime, GameManager_script.Instance().interstitialPageInfo.trueIfYouWin));
                }
            }
        }
        else
        {
            if (vUIinterstitialWindow)
            {
                vUIinterstitialWindow.gameObject.SetActive(false);
            }

            if (vUIinterstitialWindowEnd)
            {
                vUIinterstitialWindowEnd.gameObject.SetActive(false);
            }

            ChangeMoney(GameManager_script.Instance().CoinCount);

            Logo.gameObject.SetActive(true);

            ChangeExitButton(false);
        }
    }

    public IEnumerator PlayCelebMusic(float inDelay, bool inWin)
    {
        // yield
        yield return new WaitForSeconds(inDelay);

        // choose sound
        float c_m_volume = 1.0f;
        int c_m_index = inWin ? (int)MusicClip.Win_Music : (int)MusicClip.Lose_Music;

        // play sound
        GameManager_script.Instance().PlaySound(c_m_index, false, c_m_volume);
    }

    public void ChangeExitButton(bool inActive)
    {
        if (Close && Close.gameObject)
        {
            if (inActive)
            {
                Close.gameObject.SetActive(true);
            }
            else
            {
                Close.gameObject.SetActive(false);
            }
        }
    }

    public IEnumerator DelayShowCoinCountIncrement(float delayTimer, bool inWin)
    {
        if (inWin)
        {
            ChangeMoney(GameManager_script.Instance().CoinCount - GameManager_script.Instance().CurrentWager * 2.0f);
        }
        else
        {
            ChangeMoney(GameManager_script.Instance().CoinCount);
        }

        yield return new WaitForSeconds(delayTimer);

        if (inWin)
        {
            if (vUIinterstitialWindowEnd)
            {
                vUIinterstitialWindowEnd.TotalScoreBool = true;
            }

            UpdateMoney(GameManager_script.Instance().CoinCount - GameManager_script.Instance().CurrentWager * 2.0f, GameManager_script.Instance().CoinCount);
        }
        else
        {
            if (vUIinterstitialWindowEnd)
            {
                vUIinterstitialWindowEnd.TotalScoreBool = false;
            }

            UpdateMoney(GameManager_script.Instance().CoinCount, GameManager_script.Instance().CoinCount);
        }
    }

    public IEnumerator DelayShowLogoUI(float delayTimer, bool inWin)
    {
        Logo.gameObject.SetActive(false);

        yield return new WaitForSeconds(delayTimer);

        if (GameManager_script.Instance().StartingOutAsANetWorkGame && inWin)
        {
            Logo.gameObject.SetActive(false);
        }
        else
        {
            Logo.gameObject.SetActive(true);
        }
    }

    public IEnumerator OpenLevelUpUI()
    {
        // kill the page clean
        if (vUIinterstitialWindow)
        {
            vUIinterstitialWindow.gameObject.SetActive(false);
        }

        if (vUIinterstitialWindowEnd)
        {
            vUIinterstitialWindowEnd.gameObject.SetActive(false);
        }

        ChangeExitButton(false);

        yield return new WaitForSeconds(GameManager_script.Instance().LevelUpScreenTime * 0.2f);

        // 1 second later, show the stars
        UpStar.gameObject.SetActive(true);
        UpLabel.gameObject.SetActive(true);

        UpStar.GetComponent<UIStar>().StartUI(new StarInfo(GameManager_script.Instance().interstitialPageInfo.Star_one.text - 1, GameManager_script.Instance().interstitialPageInfo.Star_one.starType));
        UpLabel.text = "";

        yield return new WaitForSeconds(GameManager_script.Instance().LevelUpScreenTime * 0.3f);

        // 1 second later, do some particle and change state ahaa

        UpStar.GetComponent<UIStar>().StartUI(GameManager_script.Instance().interstitialPageInfo.Star_one); // dajiang hack, since we cannot know if our TPA score improved without adding a bunch of code, we will only do level up when players LEVEL UP the number
        UpLabel.text = Localization.Get("InterstitialNumberLevelUp");

        // play a good sound
        GameManager_script.Instance().PlaySound((int)MusicClip.LevelUp, false, 1.0f);

        yield return new WaitForSeconds(GameManager_script.Instance().LevelUpScreenTime * 0.5f);

        // whatever other false shiite
        UpStar.gameObject.SetActive(false);
        UpLabel.gameObject.SetActive(false);

        // whatever other false shiite
        if (vUIinterstitialWindow)
        {
            vUIinterstitialWindow.gameObject.SetActive(false);
        }

        if (vUIinterstitialWindowEnd)
        {
            vUIinterstitialWindowEnd.gameObject.SetActive(true);
        }

        // show exit button
        ChangeExitButton(true);
    }

    IEnumerator DelayShowRematchTooltipOnNetworkGame(float inTimer)
    {
        yield return new WaitForSeconds(inTimer);

        if (vUIinterstitialWindowEnd)
        {
            if (GameManager_script.Instance().rematchOppoWantToRematch)
            {
                // we can immediately show this guy, unsolicited
                vUIinterstitialWindowEnd.ToggleRematchToolTip(true, "RematchYes");
            }
            else
            {
                // we cannot show anything else...
                vUIinterstitialWindowEnd.ToggleRematchToolTip(false, "");
            }
        }
    }

    IEnumerator DelayHideRematchButtonOnNetworkGame(float inTimer)
    {
        yield return new WaitForSeconds(inTimer);

        // tell opponent we are done, we don't rematch anymore
        ServerController.serverController.SendRPCToNetworkViewOthers("OnRematchConfirmationReceived", false);

        // hide the rematch button not exit button...
        if (vUIinterstitialWindowEnd)
        {
            vUIinterstitialWindowEnd.ToggleRematchButton(false);
        }
    }

    public void ChangeMoney(float inMoney)
    {
        Money.GetComponent<UIMoney>().ChangeMoney(inMoney);
    }

    public void UpdateMoney(float inStart, float inEnd)
    {
        Money.GetComponent<UIMoney>().AnimateUpdateMoney(inStart, inEnd);
    }
}
