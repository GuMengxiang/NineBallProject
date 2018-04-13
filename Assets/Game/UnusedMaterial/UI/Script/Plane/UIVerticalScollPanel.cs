using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIVerticalScollPanel : MonoBehaviour
{
    public Vector2 BackgroundXY;
    public UIPanel InsidePanel;
    public UIPanel WindowPanel;
    public UIScrollView ScrollView;
    public UIGrid Grid;
    public int Line = 3;
    public int count = 0;
    public float Xgap = 0.02f;
    public float XDistance = 0.02f;
    public float Ygap = 0.02f;
    public float YDistance = 0.02f;
    public float scale = 1.3333f;
    public List<GameObject> Buttons;
    public float setheight;
    public float setwidth;

    public GameObject SettingButtonPrefab;
    public GameObject StatsButtonPrefab;
    public GameObject CueButtonPrefab;
    
    private int countnum = 0;

    public void Init_Setting_UI()
    {
        InsidePanel.SetRect(0, 0, BackgroundXY.x, BackgroundXY.y);
        count = (int)GameManager_script.Instance().SettingInfo.ButtonCount;
        Grid.GetComponent<UIGrid>().maxPerLine = (int)GameManager_script.Instance().SettingInfo.ButtonLine;
        Line = (int)GameManager_script.Instance().SettingInfo.ButtonLine;
        scale = GameManager_script.Instance().SettingInfo.ButtonScale;
        Xgap = GameManager_script.Instance().SettingInfo.Xgap;
        XDistance = GameManager_script.Instance().SettingInfo.XDistance;
        Ygap = GameManager_script.Instance().SettingInfo.Ygap;
        YDistance = GameManager_script.Instance().SettingInfo.YDistance;

        // update the only 2 values we need to update
        GameManager_script.Instance().vSettingInfo.SoundType = GameManager_script.Instance().SoundEnabled == "e" ? ClickType.Disable : ClickType.Enable;
        GameManager_script.Instance().vSettingInfo.VibrateonturnType = GameManager_script.Instance().VibeEnabled == "e" ? ClickType.Disable : ClickType.Enable;

        // give it into a local var for simplicity purposes
        SettingInfo vSettingInfo = GameManager_script.Instance().vSettingInfo;

        for (int i = 0; i < GameManager_script.Instance().SettingInfo.ButtonCount; i++)
        {
            GameObject Group = (GameObject)Instantiate(SettingButtonPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);
            Group.name = countnum.ToString("D20");
            Group.transform.parent = Grid.transform;
            Group.transform.localPosition = Vector3.zero;
            Group.transform.localScale = new Vector3(1, 1, 1);
            Group.GetComponent<SettingButton>().xdistance = XDistance * BackgroundXY.x;
            Group.GetComponent<SettingButton>().ydistance = YDistance * BackgroundXY.y;
            Group.GetComponent<SettingButton>().buttonId = i;
            Group.GetComponent<SettingButton>().WindowPanel = WindowPanel;

            if (i == 0)
            {
                Group.GetComponent<SettingButton>().LookBackground.GetComponent<OpenPopupUI>().RootPanel = WindowPanel.gameObject.transform.parent.parent.GetComponent<UIPanel>();
                Group.GetComponent<SettingButton>().LookBackground.GetComponent<OpenPopupUI>().Parent = WindowPanel.gameObject.transform.parent.gameObject;
                Group.GetComponent<SettingButton>().LookBackground.GetComponent<OpenPopupUI>().state = PopupWindowState.Language;

                Group.GetComponent<SettingButton>().MaskLookBackground.GetComponent<UISprite>().color = Color.black;
                Group.GetComponent<SettingButton>().Settinglabel.text = Localization.Get(vSettingInfo.LanguageText);
                Group.GetComponent<SettingButton>().Label.text = Localization.Get(vSettingInfo.LanguageButtonText);
                Group.GetComponent<SettingButton>().Clicktype = vSettingInfo.ScoreType;
                Group.GetComponent<SettingButton>().LookLabel.text = Localization.Get(vSettingInfo.LanguageButtonText);
            }
            else if (i == 1)
            {
                Group.GetComponent<SettingButton>().LookBackground.GetComponent<OpenPopupUI>().RootPanel = WindowPanel.gameObject.transform.parent.parent.GetComponent<UIPanel>();
                Group.GetComponent<SettingButton>().LookBackground.GetComponent<OpenPopupUI>().Parent = WindowPanel.gameObject.transform.parent.gameObject;
                Group.GetComponent<SettingButton>().LookBackground.GetComponent<OpenPopupUI>().state = PopupWindowState.Help;
                Group.GetComponent<SettingButton>().LookBackground.GetComponent<OpenPopupUI>().Titletext = Localization.Get("PopupHelpText");
                Group.GetComponent<SettingButton>().LookBackground.GetComponent<OpenPopupUI>().Bodytext = new string[] { "PopupHelpText1", "PopupHelpText2", "PopupHelpText3", "PopupHelpText4", "PopupHelpText5", "PopupHelpText6", "PopupHelpText7" };

                Group.GetComponent<SettingButton>().Settinglabel.text = Localization.Get(vSettingInfo.HelpText);
                Group.GetComponent<SettingButton>().Label.text = Localization.Get(vSettingInfo.HelpButtonText);
                Group.GetComponent<SettingButton>().Clicktype = vSettingInfo.HelpType;
                Group.GetComponent<SettingButton>().LookLabel.text = Localization.Get(vSettingInfo.HelpButtonText);
            }
            else if (i == 2)
            {
                Group.GetComponent<SettingButton>().LookBackground.GetComponent<OpenPopupUI>().RootPanel = WindowPanel.gameObject.transform.parent.parent.GetComponent<UIPanel>();
                Group.GetComponent<SettingButton>().LookBackground.GetComponent<OpenPopupUI>().Parent = WindowPanel.gameObject.transform.parent.gameObject;
                Group.GetComponent<SettingButton>().LookBackground.GetComponent<OpenPopupUI>().state = PopupWindowState.Help;
                Group.GetComponent<SettingButton>().LookBackground.GetComponent<OpenPopupUI>().Titletext = Localization.Get("PopupGeneralRulesText");
                Group.GetComponent<SettingButton>().LookBackground.GetComponent<OpenPopupUI>().Bodytext = new string[] { "PopupGeneralRulesText1", "PopupGeneralRulesText2", "PopupGeneralRulesText3", "PopupGeneralRulesText4", "PopupGeneralRulesText5", "PopupGeneralRulesText6", "PopupGeneralRulesText7", "PopupGeneralRulesText8" };

                Group.GetComponent<SettingButton>().Settinglabel.text = Localization.Get(vSettingInfo.Main_helpText);
                Group.GetComponent<SettingButton>().Label.text = Localization.Get(vSettingInfo.Main_helpButtonText);
                Group.GetComponent<SettingButton>().Clicktype = vSettingInfo.Main_helpType;
                Group.GetComponent<SettingButton>().LookLabel.text = Localization.Get(vSettingInfo.Main_helpButtonText);
            }
            else if (i == 3)
            {
                Group.GetComponent<SettingButton>().Settinglabel.text = Localization.Get(vSettingInfo.SoundText);
                Group.GetComponent<SettingButton>().Label.text = Localization.Get(vSettingInfo.SoundButtonText);
                Group.GetComponent<SettingButton>().Clicktype = vSettingInfo.SoundType;
            }
            else if (i == 4)
            {
                Group.GetComponent<SettingButton>().Settinglabel.text = Localization.Get(vSettingInfo.VibrateonturnText);
                Group.GetComponent<SettingButton>().Label.text = Localization.Get(vSettingInfo.VibrateonturnButtonText);
                Group.GetComponent<SettingButton>().Clicktype = vSettingInfo.VibrateonturnType;
            }
            else if (i == 5)
            {
                Group.GetComponent<SettingButton>().Settinglabel.text = Localization.Get(vSettingInfo.ProducerText);
                Group.GetComponent<SettingButton>().Label.text = Localization.Get(vSettingInfo.ProducerButtonText);
                Group.GetComponent<SettingButton>().Clicktype = vSettingInfo.ProducerType;
            }
            else if (i == 6)
            {
                Group.GetComponent<SettingButton>().Settinglabel.text = Localization.Get(vSettingInfo.Programmer1Text);
                Group.GetComponent<SettingButton>().Label.text = Localization.Get(vSettingInfo.Programmer1ButtonText);
                Group.GetComponent<SettingButton>().Clicktype = vSettingInfo.Programmer1Type;
            }
            else if (i == 7)
            {
                Group.GetComponent<SettingButton>().Settinglabel.text = Localization.Get(vSettingInfo.Programmer2Text);
                Group.GetComponent<SettingButton>().Label.text = Localization.Get(vSettingInfo.Programmer2ButtonText);
                Group.GetComponent<SettingButton>().Clicktype = vSettingInfo.Programmer2Type;
            }
            else if (i == 8)
            {
                Group.GetComponent<SettingButton>().Settinglabel.text = Localization.Get(vSettingInfo.Artist1Text);
                Group.GetComponent<SettingButton>().Label.text = Localization.Get(vSettingInfo.ArtistButton1Text);
                Group.GetComponent<SettingButton>().Clicktype = vSettingInfo.Artist1Type;
            }
            else if (i == 9)
            {
                Group.GetComponent<SettingButton>().Settinglabel.text = Localization.Get(vSettingInfo.Artist2Text);
                Group.GetComponent<SettingButton>().Label.text = Localization.Get(vSettingInfo.Artist2ButtonText);
                Group.GetComponent<SettingButton>().Clicktype = vSettingInfo.Artist2Type;
            }
            else if (i == 10)
            {
                Group.GetComponent<SettingButton>().Settinglabel.text = Localization.Get(vSettingInfo.Artist3Text);
                Group.GetComponent<SettingButton>().Label.text = Localization.Get(vSettingInfo.Artist3ButtonText);
                Group.GetComponent<SettingButton>().Clicktype = vSettingInfo.Artist3Type;
            }
            else if (i == 11)
            {
                Group.GetComponent<SettingButton>().Settinglabel.text = Localization.Get(vSettingInfo.DesignerText);
                Group.GetComponent<SettingButton>().Label.text = Localization.Get(vSettingInfo.DesignerButtonText);
                Group.GetComponent<SettingButton>().Clicktype = vSettingInfo.DesignerType;
            }
            else if (i == 12)
            {
                Group.GetComponent<SettingButton>().Settinglabel.text = Localization.Get(vSettingInfo.VersionText);
                Group.GetComponent<SettingButton>().Label.text = Localization.Get(vSettingInfo.VersionButtonText);
                Group.GetComponent<SettingButton>().Clicktype = vSettingInfo.VersionType;
            }
            
            countnum++;
            Grid.GetComponent<UIGrid>().repositionNow = true;
            Buttons.Add(Group);
        }

        InitInside();
    }

    public void Init_Stats_UI()
    {
        InsidePanel.SetRect(0, 0, BackgroundXY.x, BackgroundXY.y);
        count = (int)GameManager_script.Instance().StatsInfo.ButtonCount;
        Grid.GetComponent<UIGrid>().maxPerLine = (int)GameManager_script.Instance().StatsInfo.ButtonLine;
        Line = (int)GameManager_script.Instance().StatsInfo.ButtonLine;
        scale = GameManager_script.Instance().StatsInfo.ButtonScale;
        Xgap = GameManager_script.Instance().StatsInfo.Xgap;
        XDistance = GameManager_script.Instance().StatsInfo.XDistance;
        Ygap = GameManager_script.Instance().StatsInfo.Ygap;
        YDistance = GameManager_script.Instance().StatsInfo.YDistance;

        for (int i = 0; i < GameManager_script.Instance().StatsInfo.ButtonCount; i++)
        {
            GameObject Group = (GameObject)Instantiate(StatsButtonPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);
            Group.name = countnum.ToString("D20");
            Group.transform.parent = Grid.transform;
            Group.transform.localPosition = Vector3.zero;
            Group.transform.localScale = new Vector3(1, 1, 1);
            Group.GetComponent<StatsButton>().xdistance = XDistance * BackgroundXY.x;
            Group.GetComponent<StatsButton>().ydistance = YDistance * BackgroundXY.y;
            Group.GetComponent<StatsButton>().buttonId = i;
            Group.GetComponent<StatsButton>().WindowPanel = WindowPanel;
        

            countnum++;

            Grid.GetComponent<UIGrid>().repositionNow = true;

            Buttons.Add(Group);
        }

        InitInside();
    }

    public void Init_Cue_UI()
    {
        InsidePanel.SetRect(0, 0, BackgroundXY.x, BackgroundXY.y);
        count = (int)GameManager_script.Instance().CueInfo.ButtonCount;
        Grid.GetComponent<UIGrid>().maxPerLine = (int)GameManager_script.Instance().CueInfo.ButtonLine;
        Line = (int)GameManager_script.Instance().CueInfo.ButtonLine;
        scale = GameManager_script.Instance().CueInfo.ButtonScale;
        Xgap = GameManager_script.Instance().CueInfo.Xgap;
        XDistance = GameManager_script.Instance().CueInfo.XDistance;
        Ygap = GameManager_script.Instance().CueInfo.Ygap;
        YDistance = GameManager_script.Instance().CueInfo.YDistance;

        for (int i = 0; i < GameManager_script.Instance().CueInfo.ButtonCount; i++)
        {
            GameObject Group = (GameObject)Instantiate(CueButtonPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);
            Group.name = countnum.ToString("D20");
            Group.transform.parent = Grid.transform;
            Group.transform.localPosition = Vector3.zero;
            Group.transform.localScale = new Vector3(1, 1, 1);
            Group.GetComponent<CueButton>().xdistance = XDistance * BackgroundXY.x;
            Group.GetComponent<CueButton>().ydistance = YDistance * BackgroundXY.y;
            Group.GetComponent<CueButton>().buttonId = i;
            Group.GetComponent<CueButton>().WindowPanel = WindowPanel;

            Group.GetComponent<CueButton>().genericButtonState = GameManager_script.Instance().GetCueState(i);

            if (Group.GetComponent<CueButton>().genericButtonState == GenericButtonState.equipped)
            {
                WindowPanel.GetComponent<UIWindowPanel>().EquippedCue = Group;
            }

            Grid.GetComponent<UIGrid>().repositionNow = true;

			Buttons.Add(Group);

            countnum++;
        }

        InitInside();
    }

    void InitInside()
    {
        setwidth = (BackgroundXY.x - ((Line - 1) * XDistance * BackgroundXY.x + 2 * Xgap * BackgroundXY.x)) / Line;
        setheight = setwidth / scale;

        Grid.cellHeight = YDistance * BackgroundXY.y + setheight;
        Grid.cellWidth = XDistance * BackgroundXY.x + setwidth;

        for (int i = 0; i < Buttons.Count; i++)
        {
            Buttons[i].GetComponent<UISprite>().width = (int)(setwidth);
            Buttons[i].GetComponent<UISprite>().height = (int)(setheight);
        }

        Grid.Reposition();
        Grid.GetComponent<UICenterOnChild>().distanceY = Ygap * BackgroundXY.y;
        Grid.GetComponent<UICenterOnChild>().Buttonheight = setheight;
        Grid.GetComponent<UICenterOnChild>().count = count;
        Grid.GetComponent<UICenterOnChild>().length = Buttons.Count;
        float rowCount = Mathf.CeilToInt((float)count / (float)Line);
        float VHeight = (rowCount * setheight + (rowCount - 1) * YDistance * BackgroundXY.y) / 2;
        Grid.GetComponent<UICenterOnChild>().Recenter(true);
        Grid.transform.localPosition = new Vector3(Grid.transform.localPosition.x, Grid.transform.localPosition.y - 1.0f * (VHeight - BackgroundXY.y / 2 + Ygap * BackgroundXY.y), Grid.transform.localPosition.z);
    }

    public void ChangeInsideDepth(int vDepth)
    {
        InsidePanel.depth = (vDepth);
    }
}
