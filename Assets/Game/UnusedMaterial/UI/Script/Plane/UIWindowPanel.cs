using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWindowPanel : MonoBehaviour
{
    public static float setwidth = 0.01f;
    public static float setBoxBackgroundheight = 0.865f;

    public WindowState windowState;
    public UIPanel RootPanel;
    public GameObject BCollider;
    public UIPanel WindowPanel;
    public UISprite Background;
    public GameObject Close;
    public GameObject Money;
    public UILabel Title;
    public UISprite BoxBackground;
    public GameObject WindowPanelPrefab;
    public GameObject PopupWindowPanelPrefab;
    public GameObject UIHorizontalScollPrefab;
    public GameObject UIVerticalScollPrefab;
    public GameObject UIProfilePrefab;
    public GameObject InsideWindowPosition;
    public List<GameObject> InsideWindow;
    public int vDepth;
    public int InsideCount = 0;

    public GameObject EquippedAvatar;
    public GameObject EquippedCue;
    public SettingInfo vSettingInfo;

    private static UIWindowPanel pInterface;
    public static UIWindowPanel SingleTon()
    {
        return pInterface;
    }

    void Awake()
    {
        pInterface = this;
    }

	// Use this for initialization
	void Start ()
    {
        UI();
	}

    public void UI()
    {
        BCollider.GetComponent<UISprite>().height = (int)RootPanel.height + 5;
        BCollider.GetComponent<UISprite>().width = (int)RootPanel.width + 5;
        BCollider.GetComponent<BoxCollider>().size = new Vector3(RootPanel.width, RootPanel.height, 0);

        //背景框
        Background.height = (int)(RootPanel.height);
        Background.width = (int)(RootPanel.width);

        //内框
        float xx = ((1.0f - setBoxBackgroundheight) / 2.0f) * Background.height;
        float yy = Background.width * setwidth;
        float zz = xx - yy;
        BoxBackground.transform.localPosition = new Vector3(0, 0.0f - zz, 0);
        BoxBackground.height = (int)(Background.height * setBoxBackgroundheight);
        BoxBackground.width = (int)(Background.width * (1 - (setwidth * 2)));

        // 关闭按钮, dajiang hack, probably not even shown and useless
        float ww = (1.0f - setBoxBackgroundheight) * Background.height - yy;
        Close.GetComponent<UIButton>().tweenTarget.GetComponent<UISprite>().width = (int)(ww * 0.85f);
        Close.GetComponent<UIButton>().tweenTarget.GetComponent<UISprite>().height = (int)(ww * 0.85f);
        Close.GetComponent<BoxCollider>().size = new Vector3(ww, ww, 0);
        Close.transform.localPosition = new Vector3(Background.width * -0.5f + ww * 0.5f, Background.height * 0.5f - ww * 0.5f, 0.0f);

        // money
        Money.GetComponent<UISprite>().width = (int)(Screen.width * 0.175f);
        Money.GetComponent<UISprite>().height = (int)(Money.GetComponent<UISprite>().width * 0.25f);
        Money.transform.localPosition = new Vector3(Screen.width * 0.5f - Money.GetComponent<UISprite>().width * 0.5625f, Screen.height * 0.5f - ww * 0.5f, 0.0f);

        // update money
        ChangeMoney(GameManager_script.Instance().CoinCount);

        // 标题
        Title.transform.localPosition = new Vector3(0, Background.height * 0.5f - ww * 0.5f, 0);
        Title.width = (int)(Background.width * 0.5f);
        Title.height = (int)(ww * 0.85f);

        // 界面类型
        if (windowState == WindowState.AvatarShop)
        {
            AvatarShop();
            Title.text = Localization.Get("WindowTitleAvatarShop");
        }
        else if (windowState == WindowState.Profile)
        {
            Profile();
            Title.text = Localization.Get("WindowTitleProfile");
        }
        else if (windowState == WindowState.Setting)
        {
            Setting();
            Title.text = Localization.Get("WindowTitleSetting");
        }
        else if (windowState == WindowState.ShopMain)
        {
            ShopMain();
            Title.text = Localization.Get("WindowTitleShopMain");
        }
        else if (windowState == WindowState.Coin)
        {
            Coin();
            Title.text = Localization.Get("WindowTitleCoin");
        }
        else if (windowState == WindowState.Cue)
        {
            Cue();
            Title.text = Localization.Get("WindowTitleCue");
        }
        else if (windowState == WindowState.Stats)
        {
            Stats();
            Title.text = Localization.Get("WindowTitleStats");
        }
    }

    void AvatarShop()
    {
        GameObject Group=(GameObject)Instantiate(UIHorizontalScollPrefab);
        Vector3 vec3 = Vector3.zero;
        InitInsideWindow(Group, vec3);
        Group.GetComponent<UIHorizontalScollPanel>().WindowPanel = gameObject.GetComponent<UIPanel>();
        Group.GetComponent<UIHorizontalScollPanel>().BackgroundXY = new Vector2(BoxBackground.width, BoxBackground.height);
       
        Group.GetComponent<UIHorizontalScollPanel>().Init_Avatar_UI();
        InsideCount++;
        Group.GetComponent<UIHorizontalScollPanel>().ChangeInsideDepth(vDepth + InsideCount);
        InsideWindow.Add(Group);
    }

    void Profile()
    {
        //下方内部窗口
        GameObject Group = (GameObject)Instantiate(UIHorizontalScollPrefab);
        Vector3 vec3 = new Vector3(0, -BoxBackground.height / 2 + (BoxBackground.height * GameManager_script.Instance().UserInfo.setDownInsideWindow / 2), 0);
        Group.GetComponent<UIHorizontalScollPanel>().WindowPanel = gameObject.GetComponent<UIPanel>();
        Group.GetComponent<UIHorizontalScollPanel>().BackgroundXY = new Vector2(BoxBackground.width, BoxBackground.height * GameManager_script.Instance().UserInfo.setDownInsideWindow);

        GameManager_script.Instance().ProfileInfo.ButtonScale = ButtonScaleHorizontalPreCalc((float)BoxBackground.width, 
            (float)BoxBackground.height * GameManager_script.Instance().UserInfo.setDownInsideWindow, 
            (float)GameManager_script.Instance().ProfileInfo.ButtonCount, 
            (float)GameManager_script.Instance().ProfileInfo.ButtonLine, 
            (float)GameManager_script.Instance().ProfileInfo.XDistance, 
            (float)GameManager_script.Instance().ProfileInfo.YDistance, 
            (float)GameManager_script.Instance().ProfileInfo.Xgap, 
            (float)GameManager_script.Instance().ProfileInfo.Ygap);
        Group.GetComponent<UIHorizontalScollPanel>().Init_Profile_UI();
        InsideCount++;
        Group.GetComponent<UIHorizontalScollPanel>().ChangeInsideDepth(vDepth + InsideCount);
        InitInsideWindow(Group, vec3);
        
        InsideWindow.Add(Group);
        
        //上方内部窗口
        GameObject Group1 = (GameObject)Instantiate(UIProfilePrefab);
        Vector3 vec3_1 = new Vector3(0, BoxBackground.height / 2 - (BoxBackground.height * GameManager_script.Instance().UserInfo.setUpInsideWindow) / 2, 0);
        InitInsideWindow(Group1, vec3_1);
        Group1.GetComponent<UIProfilePanel>().WindowPanel = gameObject.GetComponent<UIPanel>();
        Group1.GetComponent<UIProfilePanel>().BackgroundXY = new Vector2(BoxBackground.width - (BoxBackground.width * GameManager_script.Instance().ProfileInfo.Xgap * 2), (BoxBackground.height * GameManager_script.Instance().UserInfo.setUpInsideWindow - BoxBackground.height * GameManager_script.Instance().ProfileInfo.Ygap));
        Group1.GetComponent<UIProfilePanel>().ChangeInsideDepth(vDepth + InsideCount);
        InsideWindow.Add(Group1);
    }

    void Setting()
    {
        GameObject Group = (GameObject)Instantiate(UIVerticalScollPrefab);
        Vector3 vec3 = Vector3.zero;
        InitInsideWindow(Group, vec3);
        Group.GetComponent<UIVerticalScollPanel>().WindowPanel = gameObject.GetComponent<UIPanel>();
        Group.GetComponent<UIVerticalScollPanel>().BackgroundXY = new Vector2(BoxBackground.width, BoxBackground.height);
        Group.GetComponent<UIVerticalScollPanel>().Init_Setting_UI();
        InsideCount++;
        Group.GetComponent<UIVerticalScollPanel>().ChangeInsideDepth(vDepth + InsideCount);
        InsideWindow.Add(Group);
    }

    void ShopMain()
    {
        GameObject Group = (GameObject)Instantiate(UIHorizontalScollPrefab);
        Vector3 vec3 = Vector3.zero;
        InitInsideWindow(Group, vec3);
        Group.GetComponent<UIHorizontalScollPanel>().WindowPanel = gameObject.GetComponent<UIPanel>();
        Group.GetComponent<UIHorizontalScollPanel>().BackgroundXY = new Vector2(BoxBackground.width, BoxBackground.height);
        GameManager_script.Instance().ShopMainInfo.ButtonScale = ButtonScaleHorizontalPreCalc((float)BoxBackground.width,
            (float)BoxBackground.height,
            (float)GameManager_script.Instance().ShopMainInfo.ButtonCount,
            (float)GameManager_script.Instance().ShopMainInfo.ButtonLine,
            (float)GameManager_script.Instance().ShopMainInfo.XDistance,
            (float)GameManager_script.Instance().ShopMainInfo.YDistance,
            (float)GameManager_script.Instance().ShopMainInfo.Xgap,
            (float)GameManager_script.Instance().ShopMainInfo.Ygap);

        Group.GetComponent<UIHorizontalScollPanel>().Init_ShopMain_UI();
        InsideCount++;
        Group.GetComponent<UIHorizontalScollPanel>().ChangeInsideDepth(vDepth + InsideCount);
        InsideWindow.Add(Group);
    }

    void Coin()
    {
        Analytic.EventHappenPing("ChipShop Show"); // special case, we only care about chip shop o ya....

        GameObject Group = (GameObject)Instantiate(UIHorizontalScollPrefab);
        Vector3 vec3 = Vector3.zero;
        InitInsideWindow(Group, vec3);
        Group.GetComponent<UIHorizontalScollPanel>().WindowPanel = gameObject.GetComponent<UIPanel>();
        Group.GetComponent<UIHorizontalScollPanel>().BackgroundXY = new Vector2(BoxBackground.width, BoxBackground.height);
        GameManager_script.Instance().CoinInfo.ButtonScale = ButtonScaleHorizontalPreCalc((float)BoxBackground.width,
             (float)BoxBackground.height,
             (float)GameManager_script.Instance().CoinInfo.ButtonCount,
             (float)GameManager_script.Instance().CoinInfo.ButtonLine,
             (float)GameManager_script.Instance().CoinInfo.XDistance,
             (float)GameManager_script.Instance().CoinInfo.YDistance,
             (float)GameManager_script.Instance().CoinInfo.Xgap,
             (float)GameManager_script.Instance().CoinInfo.Ygap);
        Group.GetComponent<UIHorizontalScollPanel>().Init_Coin_UI();
        InsideCount++;
        Group.GetComponent<UIHorizontalScollPanel>().ChangeInsideDepth(vDepth + InsideCount);
        InsideWindow.Add(Group);
    }

    void Cue()
    {
        GameObject Group = (GameObject)Instantiate(UIVerticalScollPrefab);
        Vector3 vec3 = Vector3.zero;
        InitInsideWindow(Group, vec3);
        Group.GetComponent<UIVerticalScollPanel>().WindowPanel = gameObject.GetComponent<UIPanel>();
        Group.GetComponent<UIVerticalScollPanel>().BackgroundXY = new Vector2(BoxBackground.width, BoxBackground.height);

        Group.GetComponent<UIVerticalScollPanel>().Init_Cue_UI();
        InsideCount++;
        Group.GetComponent<UIVerticalScollPanel>().ChangeInsideDepth(vDepth + InsideCount);
        InsideWindow.Add(Group);
    }

    void Stats()
    {
        GameObject Group = (GameObject)Instantiate(UIVerticalScollPrefab);
        Vector3 vec3 = Vector3.zero;
        InitInsideWindow(Group, vec3);
        Group.GetComponent<UIVerticalScollPanel>().WindowPanel = gameObject.GetComponent<UIPanel>();
        Group.GetComponent<UIVerticalScollPanel>().BackgroundXY = new Vector2(BoxBackground.width, BoxBackground.height);

        Group.GetComponent<UIVerticalScollPanel>().Init_Stats_UI();
        InsideCount++;
        Group.GetComponent<UIVerticalScollPanel>().ChangeInsideDepth(vDepth + InsideCount);
        InsideWindow.Add(Group);
    }

    GameObject InitInsideWindow(GameObject prefab, Vector3 localPosition)
    {

        prefab.transform.parent = BoxBackground.transform;
        prefab.transform.localPosition = localPosition;
        prefab.transform.localScale = Vector3.one;
       
        return prefab;
    }

    float ButtonScaleHorizontalPreCalc(float width, float height, float count, float line, float XDistance, float YDistance, float Xgap, float Ygap)
    {
        float scale=0;
        float columnCount = Mathf.Ceil(count / line);
        float setheight = (height - ((line - 1) * YDistance * height + 2 * Ygap * height)) / line;
        float setwidth = (width - ((columnCount - 1) * XDistance * width + 2.0f * Xgap * width)) / columnCount;
        scale = setwidth / setheight;
        return scale;
    }

    float ButtonScaleVerticalPreCalc(float width, float height, float count, float line, float XDistance, float YDistance, float Xgap, float Ygap)
    {
        float scale = 0;
        float columnCount = Mathf.Ceil(count / line);
        float setheight = (height - ((columnCount - 1) * YDistance * height + 2 * Ygap * height)) / columnCount;
        float setwidth = (width - ((line - 1) * XDistance * width + 2.0f * Xgap * width)) / line;

        scale = setwidth / setheight;

        return scale;
    }

    public void ChangeWindowDepth(int Depth)
    {
        vDepth = Depth; ;
        WindowPanel.depth = (vDepth);
    }

    public void ChangeMoney(float inMoney)
    {
        Money.GetComponent<UIMoney>().ChangeMoney(inMoney);
    }

    public void UpdateMoney(float inStart)
    {
        GameManager_script.Instance().UpdateWindowMoney(inStart);
    }
}
