using UnityEngine;
using System.Collections;

public class UISelectorPanel : MonoBehaviour
{
    public Vector2 BackgroundXY;
    public UIPanel WindowPanel;
    public UIPanel SelectorPanel; // this is the main thing   
    public UISprite BoxBackground;
    public UISlider Slider;
    public UISprite SliderBackground;
    public UISprite SliderForeground;
    public UISprite SliderHole;
    public UISprite SliderThumb;
    public GameObject PlayButton;
    public UISprite PlayButtonBackground;
    public UISprite PlayButtonMask;
    public UILabel PlayButtonLabel;
    public UILabel Moneytext1;
    public UILabel Moneytext2;
    public UILabel stakes;
    public UILabel buy_in;

    public UISprite PencilImage;
    public UISprite FtueBoxBackground;
    public UILabel FtueTitle;
    public UILabel FtueBody1;
    public GameObject TitleInput;
    public UIInput mInput;
    public UILabel Title;
    public UILabel TitleMulti;

    public GameObject OKButton;
    public UISprite OKButtonBackground;
    public UISprite OKButtonMask;
    public UILabel OKButtonLabel;

    [System.NonSerialized]
    public float heightPlayButton = 8.27f;
    [System.NonSerialized]
    public float heightOKButton = 9.27f;
    [System.NonSerialized]
    public float localSelectorValue = 0.0f;
    
	// Use this for initialization
	void Start()
    {
        UI();
	}
	
	// Update is called once per frame
    public void UI()
    {
        if (GameManager_script.Instance().isFrdsSelector)
        {
            // dajiang hack, we are currently in selector mode no matter what else (so we input into selector stuffz)
            GameManager_script.Instance().isEverythingFocusedOnFrdsSelector = true;

            // disable normal selector related shiite
            TitleMulti.gameObject.SetActive(false);

            // frds related stuffz
            TitleInput.transform.localPosition = new Vector3(0, BackgroundXY.y * 0.36f, 0);

            // 标题
            Title.width = (int)(BackgroundXY.x * 0.625f);
            Title.height = (int)(Title.width * 0.125f);
            Title.transform.localPosition = Vector3.zero;

            // more frds related stuffz
            mInput.GetComponent<BoxCollider>().size = new Vector3(Title.width, Title.height, 0.0f);
            mInput.GetComponent<BoxCollider>().center = Vector3.zero;

            PencilImage.width = (int)(BackgroundXY.y * 0.075f);
            PencilImage.height = (int)(BackgroundXY.y * 0.075f);
            PencilImage.transform.localRotation = Quaternion.Euler(0, 0, 0);
            PencilImage.transform.localPosition = new Vector3(BackgroundXY.x * -0.345f, BackgroundXY.y * 0.365f, 0.0f);

            BoxBackground.transform.localPosition = new Vector3(0, BackgroundXY.y * 0.150f, 0);
            BoxBackground.height = (int)(BackgroundXY.y / 4.22f);
            BoxBackground.width = (int)(BackgroundXY.x / 1.1f);

            // 背景框内容
            Moneytext1.transform.localPosition = new Vector3(BackgroundXY.x / -(2.55f * 2), BackgroundXY.y / (2.8f * 2), 0);
            Moneytext1.width = (int)(BackgroundXY.x / 2.9f);
            Moneytext1.height = (int)(Moneytext1.width / 4.0f);

            Moneytext2.transform.localPosition = new Vector3(BackgroundXY.x / (2.3f * 2), BackgroundXY.y / (2.8f * 2), 0);
            Moneytext2.width = (int)(BackgroundXY.x / 2.9f);
            Moneytext2.height = (int)(Moneytext2.width / 4.0f);

            stakes.transform.localPosition = new Vector3(BackgroundXY.x / -(2.55f * 2), BackgroundXY.y / (6f * 2), 0);
            stakes.width = (int)(BackgroundXY.x / 3.7f);
            stakes.height = (int)(stakes.width / 5.5f);
            stakes.text = Localization.Get("PopupSelectorBuyin");

            buy_in.transform.localPosition = new Vector3(BackgroundXY.x / (2.3f * 2), BackgroundXY.y / (6f * 2), 0);
            buy_in.width = (int)(BackgroundXY.x / 3.7f);
            buy_in.height = (int)(buy_in.width / 5.5f);
            buy_in.text = Localization.Get("PopupSelectorPayout");

            // 滑竿      
            SliderBackground.width = (int)(BackgroundXY.x / 1.1f);
            SliderBackground.height = (int)(SliderBackground.width / 16.66f);
            SliderForeground.height = (int)(SliderBackground.height * 0.80f);
            SliderForeground.width = (int)(SliderBackground.width * 0.96f);

            SliderHole.height = (int)(SliderBackground.height * 1f);
            SliderHole.width = (int)(SliderBackground.width * 1f);

            SliderThumb.height = (int)(BackgroundXY.y / 7.31f * 1.3f);
            SliderThumb.width = (int)(SliderThumb.height);// * 2.33);
            SliderThumb.GetComponent<BoxCollider>().size = new Vector3(BackgroundXY.x / 5.11f, BackgroundXY.y / 8.82f, 0);
            Slider.transform.localPosition = new Vector3(0, BackgroundXY.y / -10.15f, 0);

            FtueTitle.width = (int)(BackgroundXY.x * 0.75f);
            FtueTitle.height = (int)(BackgroundXY.x * 0.07f);
            FtueTitle.transform.localPosition = new Vector3(BackgroundXY.x * -0.0025f, BackgroundXY.y * 0.38f, 0.0f);
            FtueTitle.text = Localization.Get("FrdsGameFtueTitle");

            // Body label
            FtueBody1.width = (int)(BackgroundXY.x * 0.90f);
            FtueBody1.height = (int)(BackgroundXY.y * 0.58f);
            FtueBody1.transform.localPosition = new Vector3(BackgroundXY.x * -0.450f, BackgroundXY.y * 0.03f, 0.0f);
            FtueBody1.text = Localization.Get("FrdsGameFtueLine");

            // ftue box background
            FtueBoxBackground.height = (int)(BackgroundXY.y * 0.74f);
            FtueBoxBackground.width = (int)(BackgroundXY.x * 0.9385f);
            FtueBoxBackground.transform.localPosition = new Vector3(BackgroundXY.x * -0.0025f, BackgroundXY.y * 0.08f, 0.0f);

            // OK 按钮
            OKButtonBackground.height = (int)(BackgroundXY.y / heightOKButton);
            OKButtonBackground.width = (int)(BackgroundXY.y / heightOKButton * 4.05f);
            OKButtonMask.height = (int)(BackgroundXY.y / heightOKButton) + 2;
            OKButtonMask.width = (int)(BackgroundXY.y / heightOKButton * 4.05f) + 2;
            OKButtonMask.GetComponent<BoxCollider>().size = new Vector3(OKButtonMask.width, OKButtonMask.height, 0);
            OKButton.transform.localPosition = new Vector3(0, BackgroundXY.y * -0.370f, 0);

            // OK label
            OKButtonLabel.width = (int)(BackgroundXY.y / heightOKButton * 4.05f * 0.9f);
            OKButtonLabel.height = (int)(BackgroundXY.y / heightOKButton * 0.9f);
            OKButtonLabel.transform.localPosition = new Vector3(OKButtonLabel.transform.localPosition.x, OKButtonLabel.transform.localPosition.y - OKButtonLabel.height * 0.075f, OKButtonLabel.transform.localPosition.z);
            OKButtonLabel.text = Localization.Get("Ok");

            if (GameManager_script.Instance().ftueSeenFrdsSelector == 0.0f)
            {
                ShowOrHideFtue(true);

                GameManager_script.Instance().ftueSeenFrdsSelector = 1.0f;
                PlayerPrefs.SetFloat("ftueSeenFrdsSelector", GameManager_script.Instance().ftueSeenFrdsSelector);
            }
            else
            {
                ShowOrHideFtue(false);
            }
        }
        else
        {
            // dajiang hack, we are currently in selector mode but not frds selector
            GameManager_script.Instance().isEverythingFocusedOnFrdsSelector = false;

            // other shiites are disabled
            TitleInput.gameObject.SetActive(false);
            mInput.gameObject.SetActive(false);
            PencilImage.gameObject.SetActive(false);
            FtueBoxBackground.gameObject.SetActive(false);
            FtueTitle.gameObject.SetActive(false);
            FtueBody1.gameObject.SetActive(false);
            OKButton.SetActive(false);
            OKButtonBackground.gameObject.SetActive(false);
            OKButtonMask.gameObject.SetActive(false);
            OKButtonLabel.gameObject.SetActive(false);

            // 标题
            TitleMulti.width = (int)(BackgroundXY.x * 0.75f);
            TitleMulti.height = (int)(TitleMulti.width * 0.115f);
            TitleMulti.transform.localPosition = new Vector3(0, BackgroundXY.y * 0.36f, 0);

            BoxBackground.transform.localPosition = new Vector3(0, BackgroundXY.y * 0.150f, 0);
            BoxBackground.height = (int)(BackgroundXY.y / 4.22f);
            BoxBackground.width = (int)(BackgroundXY.x / 1.1f);

            // ftue box background
            FtueBoxBackground.height = (int)(BackgroundXY.y * 0.90f);
            FtueBoxBackground.width = (int)(BackgroundXY.x * 0.9385f);
            FtueBoxBackground.transform.localPosition = new Vector3(BackgroundXY.x * -0.0025f, 0.0f, 0.0f);
            FtueBoxBackground.gameObject.SetActive(false);

            // 背景框内容
            Moneytext1.transform.localPosition = new Vector3(BackgroundXY.x / -(2.55f * 2), BackgroundXY.y / (2.8f * 2), 0);
            Moneytext1.width = (int)(BackgroundXY.x / 2.9f);
            Moneytext1.height = (int)(Moneytext1.width / 4.0f);

            Moneytext2.transform.localPosition = new Vector3(BackgroundXY.x / (2.3f * 2), BackgroundXY.y / (2.8f * 2), 0);
            Moneytext2.width = (int)(BackgroundXY.x / 2.9f);
            Moneytext2.height = (int)(Moneytext2.width / 4.0f);

            stakes.transform.localPosition = new Vector3(BackgroundXY.x / -(2.55f * 2), BackgroundXY.y / (6f * 2), 0);
            stakes.width = (int)(BackgroundXY.x / 3.7f);
            stakes.height = (int)(stakes.width / 5.5f);
            stakes.text = Localization.Get("PopupSelectorBuyin");

            buy_in.transform.localPosition = new Vector3(BackgroundXY.x / (2.3f * 2), BackgroundXY.y / (6f * 2), 0);
            buy_in.width = (int)(BackgroundXY.x / 3.7f);
            buy_in.height = (int)(buy_in.width / 5.5f);
            buy_in.text = Localization.Get("PopupSelectorPayout");

            // 滑竿      
            SliderBackground.width = (int)(BackgroundXY.x / 1.1f);
            SliderBackground.height = (int)(SliderBackground.width / 16.66f);
            SliderForeground.height = (int)(SliderBackground.height * 0.80f);
            SliderForeground.width = (int)(SliderBackground.width * 0.96f);

            SliderHole.height = (int)(SliderBackground.height * 1f);
            SliderHole.width = (int)(SliderBackground.width * 1f);

            SliderThumb.height = (int)(BackgroundXY.y / 7.31f * 1.3f);
            SliderThumb.width = (int)(SliderThumb.height);// * 2.33);
            SliderThumb.GetComponent<BoxCollider>().size = new Vector3(BackgroundXY.x / 5.11f, BackgroundXY.y / 8.82f, 0);
            Slider.transform.localPosition = new Vector3(0, BackgroundXY.y / -10.15f, 0);
        }
        
        // Play按钮
        PlayButtonBackground.height = (int)(BackgroundXY.y / heightPlayButton);
        PlayButtonBackground.width = (int)(BackgroundXY.y / heightPlayButton * 4.05f);
        PlayButtonMask.height = (int)(BackgroundXY.y / heightPlayButton)+2;
        PlayButtonMask.width = (int)(BackgroundXY.y / heightPlayButton * 4.05f)+2;
        PlayButtonMask.GetComponent<BoxCollider>().size = new Vector3(PlayButtonMask.width, PlayButtonMask.height, 0);
        PlayButton.transform.localPosition = new Vector3(0, BackgroundXY.y * -0.315f, 0);

        // play label
        PlayButtonLabel.width = (int)(BackgroundXY.y / heightPlayButton * 4.05f* 0.9f);
        PlayButtonLabel.height = (int)(BackgroundXY.y / heightPlayButton * 0.9f);
        PlayButtonLabel.transform.localPosition = new Vector3(PlayButtonLabel.transform.localPosition.x, PlayButtonLabel.transform.localPosition.y - PlayButtonLabel.height * 0.075f, PlayButtonLabel.transform.localPosition.z);
        PlayButtonLabel.text = Localization.Get("PopupSelectorPlayButton");

        // dajiang hack, we need better logics
        Slider.GetComponent<UISlider>().value = GameManager_script.Instance().DetermineGoodSliderPosition();

        // give value
        localSelectorValue = 0.0f;
    }

    void Update()
    {
        // this gives us 10 possible values from 0 to 9
        GameManager_script.Instance().CurrentWagerLevel = (int)(Slider.GetComponent<UISlider>().value * 9.0f + 0.5f); // 0.5f means we offset a bit

        // update current wager level
        if (GameManager_script.Instance().CurrentWagerLevel < GameManager_script.WagerLevels.Length)
        {
            GameManager_script.Instance().CurrentWager = GameManager_script.WagerLevels[(int)GameManager_script.Instance().CurrentWagerLevel];
        }

        // do stuff if different
        if (localSelectorValue != GameManager_script.Instance().CurrentWager)
        {
            localSelectorValue = GameManager_script.Instance().CurrentWager;

            if (GameManager_script.Instance().isFrdsSelector)
            {
                ChangeText((int)GameManager_script.Instance().CurrentWagerLevel);
            }
            else
            {
                ChangeTextMulti((int)GameManager_script.Instance().CurrentWagerLevel);
            }
        }
    }

    public void ShowOrHideFtue(bool inShow)
    {
        TitleInput.SetActive(!inShow);
        Title.gameObject.SetActive(!inShow);
        mInput.gameObject.SetActive(!inShow);
        PencilImage.gameObject.SetActive(!inShow);
        BoxBackground.gameObject.SetActive(!inShow);
        Moneytext1.gameObject.SetActive(!inShow);
        Moneytext2.gameObject.SetActive(!inShow);
        stakes.gameObject.SetActive(!inShow);
        buy_in.gameObject.SetActive(!inShow);
        SliderBackground.gameObject.SetActive(!inShow);
        SliderHole.gameObject.SetActive(!inShow);
        SliderThumb.gameObject.SetActive(!inShow);
        Slider.gameObject.SetActive(!inShow);
        PlayButton.SetActive(!inShow);
        PlayButtonBackground.gameObject.SetActive(!inShow);
        PlayButtonMask.gameObject.SetActive(!inShow);

        FtueBoxBackground.gameObject.SetActive(inShow);
        FtueTitle.gameObject.SetActive(inShow);
        FtueBody1.gameObject.SetActive(inShow);
        OKButton.SetActive(inShow);
        OKButtonBackground.gameObject.SetActive(inShow);
        OKButtonMask.gameObject.SetActive(inShow);
        OKButtonLabel.gameObject.SetActive(inShow);
    }

    public void ChangeText(int inLevel)
    {
        // change name on top for both (if name is one of the pre-existing names, use the translated names)
        if (GameManager_script.Instance().SelectorFrdKey == "" || GameManager_script.Instance().SelectorFrdKey == "Enter Passcode Here" || GameManager_script.Instance().SelectorFrdKey == "这里输入验证码" || GameManager_script.Instance().SelectorFrdKey == "Introduce el Código Aquí" || GameManager_script.Instance().SelectorFrdKey == "Gib hier deinen PIN-Code ein" || GameManager_script.Instance().SelectorFrdKey == "Saisissez votre mot de passe" || GameManager_script.Instance().SelectorFrdKey == "パスコードを入力してください")
        {
            Title.text = GameManager_script.Instance().CharLength(Localization.Get("SelectorFriendKey"), GameManager_script.Instance().Max_Key_Length);
        }
        else
        {
            Title.text = GameManager_script.Instance().CharLength(GameManager_script.Instance().SelectorFrdKey, GameManager_script.Instance().Max_Key_Length);
        }

        // more whateverz
        mInput.value = Title.text;
        GameManager_script.Instance().SelectorFrdKey = Title.text;

        // color shit
        if (inLevel == 0)
        {
            Title.gradientTop = new Color(0.0157f, 0.5137f, 0.6980f, 1);
            Title.gradientBottom = new Color(0, 0.1804f, 0.4039f, 1);
        }
        else if (inLevel == 1)
        {
            Title.gradientTop = new Color(0.2353f, 1, 0.4f, 1);
            Title.gradientBottom = new Color(0.0667f, 0.1412f, 0.0196f, 1);
        }
        else if (inLevel == 2)
        {
            Title.gradientTop = new Color(0.8235f, 0.4314f, 0.4f, 1);
            Title.gradientBottom = new Color(1, 0, 0, 1);
        }
        else if (inLevel == 3)
        {
            Title.gradientTop = new Color(0.675f, 0.576f, 0.384f, 1);
            Title.gradientBottom = new Color(0.427f, 0.302f, 0.161f, 1);
        }
        else if (inLevel == 4)
        {
            Title.gradientTop = new Color(0, 0.361f, 0.655f, 1);
            Title.gradientBottom = new Color(0, 0.137f, 0.329f, 1);
        }
        else if (inLevel == 5)
        {
            Title.gradientTop = new Color(0.882f, 0.910f, 0.929f, 1);
            Title.gradientBottom = new Color(0.345f, 0.455f, 0.537f, 1);
        }
        else if (inLevel == 6)
        {
            Title.gradientTop = new Color(0.875f, 0.757f, 0.592f, 1);
            Title.gradientBottom = new Color(0.675f, 0.525f, 0.357f, 1);
        }
        else if (inLevel == 7)
        {
            Title.gradientTop = new Color(0.6392f, 0.6392f, 0.6392f, 1);
            Title.gradientBottom = new Color(0, 0, 0, 1);
        }
        else if (inLevel == 8)
        {
            Title.gradientTop = new Color(0.369f, 0, 0.459f, 1);
            Title.gradientBottom = new Color(0.165f, 0, 0.243f, 1);
        }
        else if (inLevel == 9)
        {
            Title.gradientTop = new Color(0.996f, 0.965f, 0.8f, 1);
            Title.gradientBottom = new Color(0.925f, 0.722f, 0.012f, 1);
        }

        // change money texts
        Moneytext1.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().CurrentWager, "gamecoinz");
        Moneytext2.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().CurrentWager * 2.0f, "gamecoinz");

        // change text color
        if (GameManager_script.Instance().CurrentWager > GameManager_script.Instance().CoinCount)
        {
            ChangeTextColor(Color.red);
        }
        else
        {
            ChangeTextColor(Color.white);
        }
    }

    public void ChangeTextMulti(int inLevel) // dajiang hack, OMGGGGGGGGGGGG this is a fucked up dupe
    {
        // change name on top
        TitleMulti.text = Localization.Get("PopupSelector" + inLevel);

        if (inLevel == 0)
        {
            TitleMulti.gradientTop = new Color(0.0157f, 0.5137f, 0.6980f, 1);
            TitleMulti.gradientBottom = new Color(0, 0.1804f, 0.4039f, 1);
        }
        else if (inLevel == 1)
        {
            TitleMulti.gradientTop = new Color(0.2353f, 1, 0.4f, 1);
            TitleMulti.gradientBottom = new Color(0.0667f, 0.1412f, 0.0196f, 1);
        }
        else if (inLevel == 2)
        {
            TitleMulti.gradientTop = new Color(0.8235f, 0.4314f, 0.4f, 1);
            TitleMulti.gradientBottom = new Color(1, 0, 0, 1);
        }
        else if (inLevel == 3)
        {
            TitleMulti.gradientTop = new Color(0.675f, 0.576f, 0.384f, 1);
            TitleMulti.gradientBottom = new Color(0.427f, 0.302f, 0.161f, 1);
        }
        else if (inLevel == 4)
        {
            TitleMulti.gradientTop = new Color(0, 0.361f, 0.655f, 1);
            TitleMulti.gradientBottom = new Color(0, 0.137f, 0.329f, 1);
        }
        else if (inLevel == 5)
        {
            TitleMulti.gradientTop = new Color(0.882f, 0.910f, 0.929f, 1);
            TitleMulti.gradientBottom = new Color(0.345f, 0.455f, 0.537f, 1);
        }
        else if (inLevel == 6)
        {
            TitleMulti.gradientTop = new Color(0.875f, 0.757f, 0.592f, 1);
            TitleMulti.gradientBottom = new Color(0.675f, 0.525f, 0.357f, 1);
        }
        else if (inLevel == 7)
        {
            TitleMulti.gradientTop = new Color(0.6392f, 0.6392f, 0.6392f, 1);
            TitleMulti.gradientBottom = new Color(0, 0, 0, 1);
        }
        else if (inLevel == 8)
        {
            TitleMulti.gradientTop = new Color(0.369f, 0, 0.459f, 1);
            TitleMulti.gradientBottom = new Color(0.165f, 0, 0.243f, 1);
        }
        else if (inLevel == 9)
        {
            TitleMulti.gradientTop = new Color(0.996f, 0.965f, 0.8f, 1);
            TitleMulti.gradientBottom = new Color(0.925f, 0.722f, 0.012f, 1);
        }

        // change money texts
        Moneytext1.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().CurrentWager, "gamecoinz");
        Moneytext2.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().CurrentWager * 2.0f, "gamecoinz");

        // change text color
        if (GameManager_script.Instance().CurrentWager > GameManager_script.Instance().CoinCount)
        {
            ChangeTextColor(Color.red);
        }
        else
        {
            ChangeTextColor(Color.white);
        }
    }

    public void ChangeTextColor(Color color)
    {
        Moneytext1.color = color;
        Moneytext2.color = color;
        stakes.color = color;
        buy_in.color = color;
    }

    public void ChangeInsideDepth(int vDepth)
    {
        gameObject.GetComponent<UIPanel>().depth = (vDepth);
    }
}
