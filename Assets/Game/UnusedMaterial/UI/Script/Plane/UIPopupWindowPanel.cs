using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPopupWindowPanel : MonoBehaviour {

    public UIPanel RootPanel;
    public GameObject BCollider;
    public UISprite Background;
    public GameObject Close; 
    public GameObject WindowPanelPrefab;
    public GameObject UISelectorPrefab;
    public GameObject UIDailyDealPrefab;
    public GameObject UIDailyBonusPrefab;
    public GameObject UIPlayerProfilePrefab;
    public GameObject UIHelpPrefab;
    public GameObject UIFTUEPopupPrefab;
    public GameObject UILanguagePrefab;
  
    public PopupWindowState WindowState;
    public List<GameObject> InsideWindow;
    public DailyBonusPopupInfo dailyBonusPopupInfo;
    public int ProfileID;

    public int vDepth;
    public int InsideCount = 0;

    public string Titletext;
    public string[] Bodytext;

	// Use this for initialization
	void Start ()
    {
        UI();
	}
	
    public void UI()
    {
        //SelectorPanel.SetRect(0, 0, RootPanel.width, RootPanel.height);
        BCollider.GetComponent<UISprite>().height = (int)(RootPanel.height * 1.15f);
        BCollider.GetComponent<UISprite>().width = (int)(RootPanel.width * 1.15f);
        BCollider.GetComponent<BoxCollider>().size = new Vector3(RootPanel.width * 1.15f, RootPanel.height * 1.15f, 0);

        // 背景
        Background.height = (int)(RootPanel.height / 1.4f);
        Background.width = (int)(Background.height * 1.48f);
        Background.GetComponent<BoxCollider>().size = new Vector3(Background.width, Background.height, 0);

        if (Background.width > RootPanel.width)
        {
            Background.width = (int)RootPanel.width;
            Background.height = (int)(Background.width / 1.48f);
        }

        // 界面类型
        if (WindowState == PopupWindowState.Selector)
        {
            Selector();
        }
        else if (WindowState == PopupWindowState.DailyDeal)
        {
            DailyDeal();
        }
        else if (WindowState == PopupWindowState.DailyBonus)
        {
            DailyBonus();
        }
        else if (WindowState == PopupWindowState.PlayerProfile)
        {
            PlayerProfile();
        }
        else if (WindowState == PopupWindowState.Help)
        {
            Help();
        }
        else if (WindowState == PopupWindowState.FTUE_Popup)
        {
            FTUE_Popup();
        }
        else if (WindowState == PopupWindowState.StarterDeal)
        {
            StarterDeal();
        }
        else if (WindowState == PopupWindowState.Language)
        {
            Language();
        }

        Close.GetComponent<UIButton>().tweenTarget.GetComponent<UISprite>().width = Background.height / 11;
        Close.GetComponent<UIButton>().tweenTarget.GetComponent<UISprite>().height = Background.height / 11;
        Close.GetComponent<BoxCollider>().size = new Vector3(Background.height / 5.5f, Background.height / 5.5f, 0);
        Close.transform.localPosition = new Vector3(Background.width * 0.5f * 0.9685f , Background.height * 0.5f * 0.9405f, 0);

        // a bunch of special case for player profile
        if (WindowState == PopupWindowState.PlayerProfile)
        {
            Close.GetComponent<UIButton>().tweenTarget.GetComponent<UISprite>().width = Background.height / 11;
            Close.GetComponent<UIButton>().tweenTarget.GetComponent<UISprite>().height = Background.height / 11;
            Close.GetComponent<BoxCollider>().size = new Vector3(Background.height / 5.5f, Background.height / 5.5f, 0);
            Close.transform.localPosition = new Vector3(Background.width / 2 * 1.00f, Background.height / 2 * 0.99f, 0);

            Background.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    void Selector()
    {
        GameObject Group = (GameObject)Instantiate(UISelectorPrefab);
        Vector3 vec3 = Vector3.zero;
        InitInsideWindow(Group, vec3);
        Group.GetComponent<UISelectorPanel>().WindowPanel = gameObject.GetComponent<UIPanel>();
        Group.GetComponent<UISelectorPanel>().BackgroundXY = new Vector2(Background.width, Background.height);
        InsideCount++;
        Group.GetComponent<UISelectorPanel>().ChangeInsideDepth(vDepth + InsideCount);
        InsideWindow.Add(Group);
    }

    void DailyDeal()
    {
        GameObject Group = (GameObject)Instantiate(UIDailyDealPrefab);
        Vector3 vec3 = Vector3.zero;
        InitInsideWindow(Group, vec3);
        Group.GetComponent<UIDailyDealPanel>().WindowPanel = gameObject.GetComponent<UIPanel>();
        Group.GetComponent<UIDailyDealPanel>().BackgroundXY = new Vector2(Background.width, Background.height);
        Group.GetComponent<UIDailyDealPanel>().DailyDealUI();
        InsideCount++;
        Group.GetComponent<UIDailyDealPanel>().ChangeInsideDepth(vDepth + InsideCount);
        InsideWindow.Add(Group);
    }

    void DailyBonus()
    {
        GameObject Group = (GameObject)Instantiate(UIDailyBonusPrefab);
        Vector3 vec3 = Vector3.zero;
        InitInsideWindow(Group, vec3);
        Group.GetComponent<UIDailyBonusPanel>().WindowPanel = gameObject.GetComponent<UIPanel>();
        Group.GetComponent<UIDailyBonusPanel>().BackgroundXY = new Vector2(Background.width, Background.height);
        Group.GetComponent<UIDailyBonusPanel>().DailyBonusUI(dailyBonusPopupInfo);
        InsideCount++;
        Group.GetComponent<UIDailyBonusPanel>().ChangeInsideDepth(vDepth + InsideCount);
        InsideWindow.Add(Group);
    }

    void PlayerProfile()
    {
        //背景
        Background.height = (int)(RootPanel.height / 1.4f * 0.75f);
        Background.width = (int)(Background.height * 1.48f/ 0.75f);
        Background.GetComponent<BoxCollider>().size = new Vector3(Background.width, Background.height, 0);
        Background.spriteName = "PlayerProfileBackground";

        GameObject Group = (GameObject)Instantiate(UIPlayerProfilePrefab);
        Vector3 vec3 = Vector3.zero;
        InitInsideWindow(Group, vec3);
        Group.GetComponent<GamePlayerProfilePanel>().WindowPanel = gameObject.GetComponent<UIPanel>();
        Group.GetComponent<GamePlayerProfilePanel>().BackgroundXY = new Vector2(Background.width, Background.height);
        Group.GetComponent<GamePlayerProfilePanel>().PlayerProfileUI(ProfileID);
        
        InsideCount++;
        Group.GetComponent<GamePlayerProfilePanel>().ChangeInsideDepth(vDepth + InsideCount);
        InsideWindow.Add(Group);
    }

    void Help()
    {
        GameObject Group = (GameObject)Instantiate(UIHelpPrefab);
        Vector3 vec3 = Vector3.zero;
        InitInsideWindow(Group, vec3);
        Group.GetComponent<UIHelpPlane>().WindowPanel = gameObject.GetComponent<UIPanel>();
        Group.GetComponent<UIHelpPlane>().BackgroundXY = new Vector2(Background.width, Background.height);      
        Group.GetComponent<UIHelpPlane>().Titletext = Titletext;
        Group.GetComponent<UIHelpPlane>().Bodytext = Bodytext;
        Group.GetComponent<UIHelpPlane>().HelpUI();
        InsideCount++;
        Group.GetComponent<UIHelpPlane>().ChangeInsideDepth(vDepth + InsideCount);
        InsideWindow.Add(Group);
    }

    void FTUE_Popup()
    {
        Destroy(Close.gameObject);
        GameObject Group = (GameObject)Instantiate(UIFTUEPopupPrefab);
        Vector3 vec3 = Vector3.zero;
        InitInsideWindow(Group, vec3);
        Group.GetComponent<UIFTUE_Popup>().WindowPanel = gameObject.GetComponent<UIPanel>();
        Group.GetComponent<UIFTUE_Popup>().BackgroundXY = new Vector2(Background.width, Background.height);
        Group.GetComponent<UIFTUE_Popup>().FTUE_Popup();
        InsideCount++;
        Group.GetComponent<UIFTUE_Popup>().ChangeInsideDepth(vDepth + InsideCount);
        InsideWindow.Add(Group);
    }

    void StarterDeal()
    {
        GameObject Group = (GameObject)Instantiate(UIDailyDealPrefab);
        Vector3 vec3 = Vector3.zero;
        InitInsideWindow(Group, vec3);
        Group.GetComponent<UIDailyDealPanel>().WindowPanel = gameObject.GetComponent<UIPanel>();
        Group.GetComponent<UIDailyDealPanel>().BackgroundXY = new Vector2(Background.width, Background.height);
        Group.GetComponent<UIDailyDealPanel>().DailyDealUI();
        InsideCount++;
        Group.GetComponent<UIDailyDealPanel>().ChangeInsideDepth(vDepth + InsideCount);
        InsideWindow.Add(Group);
    }

    void Language()
    {
        GameObject Group = (GameObject)Instantiate(UILanguagePrefab);
        Vector3 vec3 = Vector3.zero;
        InitInsideWindow(Group, vec3);
        Group.GetComponent<UILanguagePanel>().WindowPanel = gameObject.GetComponent<UIPanel>();
        Group.GetComponent<UILanguagePanel>().BackgroundXY = new Vector2(Background.width, Background.height);
        Group.GetComponent<UILanguagePanel>().LanguageUI();

        InsideCount++;
        Group.GetComponent<UILanguagePanel>().ChangeInsideDepth(vDepth + InsideCount);
        InsideWindow.Add(Group);
    }

    GameObject InitInsideWindow(GameObject prefab, Vector3 localPosition)
    {
        prefab.transform.parent = Background.transform;
        prefab.transform.localPosition = localPosition;
        prefab.transform.localScale = Vector3.one;

        return prefab;
    }

    public void ChangeWindowDepth(int Depth)
    {
        vDepth = Depth; ;
        gameObject.GetComponent<UIPanel>().depth = (Depth);
    }
}
