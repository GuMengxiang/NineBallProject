using UnityEngine;
using System.Collections;

public class UICenter : MonoBehaviour
{
    public UIPanel RootPanel;
    public UIPanel Panel;
    public UIScrollView ScrollView;
    public UIGrid Grid;
    public GameObject BackGround;
    public UISprite Logo;
    public GameObject Money;
	public GameObject mLandingLabel; 

    [System.NonSerialized]
    public float setheight = 0.31f;
    [System.NonSerialized]
    public float scale = 1.33f;
    [System.NonSerialized]
    public float count = 3.12f;

    public UISprite[] Buttons;
    public UISprite[] ButtonsMask;
    public UILabel[] ButtonsLabel;
    public UILabel[] ButtonsPlay;

    [System.NonSerialized]
    public float distanceX = 0.00f;
    [System.NonSerialized]
    public float leftgap = 1.00f;
    [System.NonSerialized]
    public float buttongap = 1.25f;
    [System.NonSerialized]
    public float ScrollViewPosition = -0.02f;

    void Start()
    {
        Init_Horizontal_UI();

        GameManager_script.Instance().uiCenter = this;
    }

    void Init_Horizontal_UI()
    {
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

        Panel.SetRect(0, 0, RootPanel.width, RootPanel.height);

        Money.GetComponent<UISprite>().width = (int)(Screen.width * 0.175f);
        Money.GetComponent<UISprite>().height = (int)(Money.GetComponent<UISprite>().width * 0.25f);
        Money.transform.localPosition = new Vector3(Screen.width * 0.5f - Money.GetComponent<UISprite>().width * 0.5625f, Screen.height * 0.5f - Money.GetComponent<UISprite>().height * 0.75f, 0.0f);
		//Gu,
		mLandingLabel.transform.localPosition =new Vector3 (Money.transform.localPosition.x- Money.GetComponent<UISprite>().width * 1.0f,Money.transform.localPosition.y, 0.0f);
		mLandingLabel.GetComponent<UILabel> ().width = Money.GetComponent<UISprite > ().width;
		mLandingLabel.GetComponent<UILabel> ().height = Money.GetComponent<UISprite > ().height;
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].height = (int)(RootPanel.height * setheight);
            Buttons[i].width = (int)(Buttons[i].height * scale);
            ButtonsMask[i].height = Buttons[i].height;
            ButtonsMask[i].width = Buttons[i].width;

            if (GameManager_script.Instance().Savelanguage == "简体中文" || GameManager_script.Instance().Savelanguage == "繁體中文")
            {
                ButtonsLabel[i].width = (int)(Buttons[i].width * 0.800f);
                ButtonsLabel[i].height = (int)(Buttons[i].height * 0.350f);
                ButtonsLabel[i].transform.localPosition = new Vector3(0, ButtonsLabel[i].height * 0.2500f, 0);

                ButtonsPlay[i].width = (int)(Buttons[i].width * 0.800f);
                ButtonsPlay[i].height = (int)(Buttons[i].height * 0.250f);
                ButtonsPlay[i].transform.localPosition = new Vector3(0, ButtonsPlay[i].height * -0.8000f, 0);
            }
            else if (GameManager_script.Instance().Savelanguage == "日本語")
            {
                ButtonsLabel[i].width = (int)(Buttons[i].width * 0.660f);
                ButtonsLabel[i].height = (int)(Buttons[i].height * 0.400f);
                ButtonsLabel[i].transform.localPosition = new Vector3(0, ButtonsLabel[i].height * -0.4000f, 0);

                ButtonsPlay[i].width = (int)(Buttons[i].width * 0.725f);
                ButtonsPlay[i].height = (int)(Buttons[i].height * 0.190f);
                ButtonsPlay[i].transform.localPosition = new Vector3(0, ButtonsPlay[i].height * 0.7500f, 0);
            }
            else
            {
                ButtonsLabel[i].width = (int)(Buttons[i].width * 0.725f);
                ButtonsLabel[i].height = (int)(Buttons[i].height * 0.190f);
                ButtonsLabel[i].transform.localPosition = new Vector3(0, ButtonsLabel[i].height * -0.950f, 0);

                ButtonsPlay[i].width = (int)(Buttons[i].width * 0.660f);
                ButtonsPlay[i].height = (int)(Buttons[i].height * 0.400f);
                ButtonsPlay[i].transform.localPosition = new Vector3(0, ButtonsPlay[i].height * 0.200f, 0);
            }

            switch (i)
            {
                case 0:
                    {
                        ButtonsPlay[i].text = Localization.Get("MainMenuButtonPlayMulti");
                        ButtonsLabel[i].text = Localization.Get("MainMenuMulti");
                        break;
                    }
                case 1:
                    {
                        ButtonsPlay[i].text = Localization.Get("MainMenuButtonPlayFrds");
                        ButtonsLabel[i].text = Localization.Get("MainMenuFrds");
                        break;
                    }
                case 2:
                    {
                        ButtonsPlay[i].text = Localization.Get("MainMenuButtonPlayBots");
                        ButtonsLabel[i].text = Localization.Get("MainMenuBots");
                        break;
                    }
                case 3:
                    {
                        ButtonsPlay[i].text = Localization.Get("MainMenuButtonPlaySolo");
                        ButtonsLabel[i].text = Localization.Get("MainMenuSolo");
                        break;
                    }
            }
        }

        if (Buttons[0].width > RootPanel.width / count)
        {
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].width = (int)(RootPanel.width / count);
                Buttons[i].height = (int)(Buttons[i].width / scale);
                ButtonsMask[i].height = Buttons[i].height;
                ButtonsMask[i].width = Buttons[i].width;

                ButtonsLabel[i].width = (int)(Buttons[i].width * 0.725f);
                ButtonsLabel[i].height = (int)(Buttons[i].height * 0.190f);
                ButtonsLabel[i].transform.localPosition = new Vector3(0, ButtonsLabel[i].height * -0.950f, 0);

                ButtonsPlay[i].height = (int)(Buttons[i].height * 0.400f);
                ButtonsPlay[i].width = (int)(Buttons[i].width * 0.660f);
                ButtonsPlay[i].transform.localPosition = new Vector3(0, ButtonsPlay[i].height * 0.200f, 0);
            }

            int vcount = (int)count;
            float width = (RootPanel.width - count * Buttons[0].width); // 总剩余宽度 = 宽 -（按钮 * 数量（含小数））

            if (count - vcount > 0) // 有小数
            {
                distanceX = width / (leftgap + (vcount - 1) * buttongap + buttongap);//实际距离单位=总剩余/（1+空格数量（按钮数量-1）+（1\1.5））
            }
            else
            {
                distanceX = width / (leftgap + (vcount - 1) * buttongap + leftgap);
            }

            Grid.cellWidth = distanceX * buttongap + Buttons[0].width;
            Grid.Reposition();

            Grid.GetComponent<UICenterOnChild>().distanceX = distanceX;
            Grid.GetComponent<UICenterOnChild>().Buttonwidth = Buttons[0].width;
            Grid.GetComponent<UICenterOnChild>().count = count;
            Grid.GetComponent<UICenterOnChild>().length = Buttons.Length;

            ChildRecenter();

            Grid.transform.localPosition = new Vector3(Grid.transform.localPosition.x - ((RootPanel.width / 2) - distanceX - Buttons[0].width / 2), Grid.transform.localPosition.y, Grid.transform.localPosition.z);
        }
        else
        {
            int vcount = (int)count;
            float width = (RootPanel.width - count * Buttons[0].width);//总剩余宽度=宽-（按钮*数量（含小数））

            if (count - vcount > 0)//有小数
            {
                distanceX = width / (leftgap + (vcount - 1) * buttongap + buttongap);//实际距离单位=总剩余/（1+空格数量（按钮数量-1）+（1\1.5））
            }
            else
            {
                distanceX = width / (leftgap + (vcount - 1) * buttongap + leftgap);
            }

            Grid.cellWidth = distanceX * buttongap + Buttons[0].width;
            Grid.Reposition();

            Grid.GetComponent<UICenterOnChild>().distanceX = distanceX;
            Grid.GetComponent<UICenterOnChild>().Buttonwidth = Buttons[0].width;
            Grid.GetComponent<UICenterOnChild>().count = count;
            Grid.GetComponent<UICenterOnChild>().length = Buttons.Length;

            ChildRecenter();

            Grid.transform.localPosition = new Vector3(Grid.transform.localPosition.x - ((RootPanel.width / 2) - distanceX - Buttons[0].width / 2), Grid.transform.localPosition.y, Grid.transform.localPosition.z);
        }

        // time to bring out the violators what what!
        HideAllNewBanner();

        // kill everything first
        HideAllFtueTooltip();

        // show the tooltips
        if (GameManager_script.Instance().SeenSwipeAndPullEver == 1.0f)
        {
            if (GameManager_script.Instance().SeenNetworkGameFlagEver == 0.0f && GameManager_script.Instance().Total_Games_Played == 0.0f)
            {
                // display arrow here (there is no level or other situational gates, this is shown immediately).
                Buttons[0].GetComponent<LabelHolder>().InitializeFtueRelatedStuffz("FtueOnline");

                // remember that this has been shown
                GameManager_script.Instance().SeenNetworkGameFlagEver = 1.0f;
                PlayerPrefs.SetFloat("SeenNetworkGameFlagEver", GameManager_script.Instance().SeenNetworkGameFlagEver);
            }
        }

        // change money up
        ChangeMoney(GameManager_script.Instance().CoinCount);
    }

    void ChildRecenter()
    {
        Grid.GetComponent<UICenterOnChild>().Recenter();
    }

    public void HideAllFtueTooltip()
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].GetComponent<LabelHolder>().TuckAwayFtueRelatedStuffz();
        }
    }

    public void HideAllNewBanner()
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].GetComponent<LabelHolder>().HideNewLabel();
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
