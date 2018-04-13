using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHorizontalScollPanel : MonoBehaviour
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
    public List<GameObject> Buttons; // we can handle this with different types of buttons just fine...
    public float setheight;
    public float setwidth;

    //商店数据//
    public GameObject AvatarButtonPrefab;
    public GameObject ProfileButtonPrefab;
    public GameObject ShopMainButtonPrefab;
    public GameObject CoinButtonPrefab;
   
    private int countnum = 0;

    public void Init_Avatar_UI()
    {
        InsidePanel.SetRect(0, 0, BackgroundXY.x, BackgroundXY.y);
        count = (int)GameManager_script.Instance().AvatarInfo.ButtonCount;
        Grid.GetComponent<UIGrid>().maxPerLine = (int)GameManager_script.Instance().AvatarInfo.ButtonLine;
        Line = (int)GameManager_script.Instance().AvatarInfo.ButtonLine;
        scale = GameManager_script.Instance().AvatarInfo.ButtonScale;
        Xgap = GameManager_script.Instance().AvatarInfo.Xgap;
        XDistance = GameManager_script.Instance().AvatarInfo.XDistance;
        Ygap = GameManager_script.Instance().AvatarInfo.Ygap;
        YDistance = GameManager_script.Instance().AvatarInfo.YDistance;

        for (int i = 0; i < GameManager_script.Instance().AvatarInfo.ButtonCount; i++)
        {
            GameObject Group = (GameObject)Instantiate(AvatarButtonPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);
            Group.name = countnum.ToString("D20");
            Group.transform.parent = Grid.transform;
            Group.transform.localPosition = Vector3.zero;
            Group.transform.localScale = new Vector3(1, 1, 1);
            Group.GetComponent<AvatarButton>().buttonId = i;
            Group.GetComponent<AvatarButton>().xdistance = XDistance * BackgroundXY.x;
            Group.GetComponent<AvatarButton>().ydistance = YDistance * BackgroundXY.y;
            Group.GetComponent<AvatarButton>().Sprite_Enter.spriteName = "" + i;
            Group.GetComponent<AvatarButton>().WindowPanel = WindowPanel;

            Group.GetComponent<AvatarButton>().genericButtonState = GameManager_script.Instance().GetAvatarState(i);

            if (Group.GetComponent<AvatarButton>().genericButtonState == GenericButtonState.equipped)
            {
                WindowPanel.GetComponent<UIWindowPanel>().EquippedAvatar = Group;
            }

            Grid.GetComponent<UIGrid>().repositionNow = true;

            Buttons.Add(Group);

            countnum++;
        }

        InitInside();
    }

    public void Init_Profile_UI()
    {
        InsidePanel.SetRect(0, 0, BackgroundXY.x, BackgroundXY.y);
        count = (int)GameManager_script.Instance().ProfileInfo.ButtonCount;
        Grid.GetComponent<UIGrid>().maxPerLine = (int)GameManager_script.Instance().ProfileInfo.ButtonLine;
        Line = (int)GameManager_script.Instance().ProfileInfo.ButtonLine;
        scale = GameManager_script.Instance().ProfileInfo.ButtonScale;
        Xgap = GameManager_script.Instance().ProfileInfo.Xgap;
        XDistance = GameManager_script.Instance().ProfileInfo.XDistance;
        Ygap = GameManager_script.Instance().ProfileInfo.Ygap;
        YDistance = GameManager_script.Instance().ProfileInfo.YDistance;

        for (int i = 0; i < GameManager_script.Instance().ProfileInfo.ButtonCount; i++)
        {
            GameObject Group = (GameObject)Instantiate(ProfileButtonPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);
            Group.name = countnum.ToString("D20");
            Group.transform.parent = Grid.transform;
            Group.transform.localPosition = Vector3.zero;
            Group.transform.localScale = new Vector3(1, 1, 1);
            Group.GetComponent<ProfileButton>().xdistance = XDistance * BackgroundXY.x;
            Group.GetComponent<ProfileButton>().ydistance = YDistance * BackgroundXY.y;
            Group.GetComponent<ProfileButton>().buttonId = i;
            Group.GetComponent<ProfileButton>().WindowPanel = WindowPanel;

            if (i == 0)
            {
                Group.AddComponent<OpenWindowUI>();
                Group.GetComponent<OpenWindowUI>().RootPanel = WindowPanel.gameObject.transform.parent.parent.GetComponent<UIPanel>();
                Group.GetComponent<OpenWindowUI>().Parent = WindowPanel.gameObject.transform.parent.gameObject;
                Group.GetComponent<OpenWindowUI>().state = WindowState.Stats;
                Group.GetComponent<OpenWindowUI>().WindowPanel=WindowPanel.GetComponent<UIWindowPanel>().WindowPanelPrefab;
                Group.GetComponent<ProfileButton>().ButtonName.text = Localization.Get("ProfileButtonName1");
                Group.GetComponent<ProfileButton>().ButtonImage.spriteName = "ProfileButtonName1";
            }
            else if (i == 1)
            {
                Group.GetComponent<ProfileButton>().ButtonName.text = Localization.Get("ProfileButtonName2");
                Group.GetComponent<ProfileButton>().ButtonImage.spriteName = "ProfileButtonName2";
            }

            Grid.GetComponent<UIGrid>().repositionNow = true;

            Buttons.Add(Group);

            countnum++;
        }

        InitInside();
    }

    public void Init_ShopMain_UI()
    {
        InsidePanel.SetRect(0, 0, BackgroundXY.x, BackgroundXY.y);
        count = (int)GameManager_script.Instance().ShopMainInfo.ButtonCount;
        Grid.GetComponent<UIGrid>().maxPerLine = (int)GameManager_script.Instance().ShopMainInfo.ButtonLine;
        Line = (int)GameManager_script.Instance().ShopMainInfo.ButtonLine;
        scale = GameManager_script.Instance().ShopMainInfo.ButtonScale;
        Xgap = GameManager_script.Instance().ShopMainInfo.Xgap;
        XDistance = GameManager_script.Instance().ShopMainInfo.XDistance;
        Ygap = GameManager_script.Instance().ShopMainInfo.Ygap;
        YDistance = GameManager_script.Instance().ShopMainInfo.YDistance;

        for (int i = 0; i < GameManager_script.Instance().ShopMainInfo.ButtonCount; i++)
        {
            GameObject Group = (GameObject)Instantiate(ShopMainButtonPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);
            Group.name = countnum.ToString("D20");
            Group.transform.parent = Grid.transform;
            Group.transform.localPosition = Vector3.zero;
            Group.transform.localScale = new Vector3(1, 1, 1);
            Group.GetComponent<ShopMainButton>().xdistance = XDistance * BackgroundXY.x;
            Group.GetComponent<ShopMainButton>().ydistance = YDistance * BackgroundXY.y;
            Group.GetComponent<ShopMainButton>().buttonId = i;
            Group.GetComponent<ShopMainButton>().WindowPanel = WindowPanel;

            if (i == 0)
            {
                Group.AddComponent<OpenPopupUI>();
                Group.GetComponent<OpenPopupUI>().RootPanel = WindowPanel.gameObject.transform.parent.parent.GetComponent<UIPanel>();
                Group.GetComponent<OpenPopupUI>().Parent = WindowPanel.gameObject.transform.parent.gameObject;
                Group.GetComponent<OpenPopupUI>().WindowPanel = WindowPanel.GetComponent<UIWindowPanel>().PopupWindowPanelPrefab;

                if (GameManager_script.Instance().MadePurchaseEver == 0.0f)
                {
                    Group.GetComponent<OpenPopupUI>().state = PopupWindowState.StarterDeal;
                    Group.GetComponent<ShopMainButton>().ButtonName.text = Localization.Get("PopupStarterDeal");
                    Group.GetComponent<ShopMainButton>().ButtonImage.spriteName = "DailyDeal";
                }
                else
                {
                    Group.GetComponent<OpenPopupUI>().state = PopupWindowState.DailyDeal;
                    Group.GetComponent<ShopMainButton>().ButtonName.text = Localization.Get("PopupDailyDeal");
                    Group.GetComponent<ShopMainButton>().ButtonImage.spriteName = "DailyDeal";
                }
            }

            if (i == 1)
            {
                Group.AddComponent<OpenWindowUI>();
                Group.GetComponent<OpenWindowUI>().RootPanel = WindowPanel.gameObject.transform.parent.parent.GetComponent<UIPanel>();
                Group.GetComponent<OpenWindowUI>().Parent = WindowPanel.gameObject.transform.parent.gameObject;
                Group.GetComponent<OpenWindowUI>().WindowPanel = WindowPanel.GetComponent<UIWindowPanel>().WindowPanelPrefab;

                Group.GetComponent<OpenWindowUI>().state = WindowState.Cue;
                Group.GetComponent<ShopMainButton>().ButtonName.text = Localization.Get("WindowTitleCue");
                Group.GetComponent<ShopMainButton>().ButtonImage.spriteName = "Cue";
            }

            if (i == 2)
            {
                Group.AddComponent<OpenWindowUI>();
                Group.GetComponent<OpenWindowUI>().RootPanel = WindowPanel.gameObject.transform.parent.parent.GetComponent<UIPanel>();
                Group.GetComponent<OpenWindowUI>().Parent = WindowPanel.gameObject.transform.parent.gameObject;
                Group.GetComponent<OpenWindowUI>().WindowPanel = WindowPanel.GetComponent<UIWindowPanel>().WindowPanelPrefab;

                Group.GetComponent<OpenWindowUI>().state = WindowState.Coin;
                Group.GetComponent<ShopMainButton>().ButtonName.text = Localization.Get("WindowTitleCoin");
                Group.GetComponent<ShopMainButton>().ButtonImage.spriteName = "Coin";
            }
           
            if (i == 3)
            {
                Group.AddComponent<OpenWindowUI>();
                Group.GetComponent<OpenWindowUI>().RootPanel = WindowPanel.gameObject.transform.parent.parent.GetComponent<UIPanel>();
                Group.GetComponent<OpenWindowUI>().Parent = WindowPanel.gameObject.transform.parent.gameObject;
                Group.GetComponent<OpenWindowUI>().WindowPanel = WindowPanel.GetComponent<UIWindowPanel>().WindowPanelPrefab;

                Group.GetComponent<OpenWindowUI>().state = WindowState.AvatarShop;
                Group.GetComponent<ShopMainButton>().ButtonName.text = Localization.Get("WindowTitleAvatarShop");
                Group.GetComponent<ShopMainButton>().ButtonImage.spriteName = "AvatarShop";
            }
           
            Grid.GetComponent<UIGrid>().repositionNow = true;

            Buttons.Add(Group);

            countnum++;
        }

        InitInside();
    }

   public void Init_Coin_UI()
   {
       InsidePanel.SetRect(0, 0, BackgroundXY.x, BackgroundXY.y);
       count = (int)GameManager_script.Instance().CoinInfo.ButtonCount;
       Grid.GetComponent<UIGrid>().maxPerLine = (int)GameManager_script.Instance().CoinInfo.ButtonLine;
       Line = (int)GameManager_script.Instance().CoinInfo.ButtonLine;
       scale = GameManager_script.Instance().CoinInfo.ButtonScale;
       Xgap = GameManager_script.Instance().CoinInfo.Xgap;
       XDistance = GameManager_script.Instance().CoinInfo.XDistance;
       Ygap = GameManager_script.Instance().CoinInfo.Ygap;
       YDistance = GameManager_script.Instance().CoinInfo.YDistance;

       for (int i = 0; i < GameManager_script.Instance().CoinInfo.ButtonCount; i++)
       {
           GameObject Group = (GameObject)Instantiate(CoinButtonPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);
           Group.name = countnum.ToString("D20");
           //Group.tag = "text";
           Group.transform.parent = Grid.transform;
           Group.transform.localPosition = Vector3.zero;
           Group.transform.localScale = new Vector3(1, 1, 1);
           Group.GetComponent<CoinButton>().xdistance = XDistance * BackgroundXY.x;
           Group.GetComponent<CoinButton>().ydistance = YDistance * BackgroundXY.y;
           Group.GetComponent<CoinButton>().buttonId = i;
           Group.GetComponent<CoinButton>().WindowPanel = WindowPanel;
           if (i == 0)
           {
               Group.GetComponent<CoinButton>().ChangeCoinButton(new CoinInfo
                   (
                       "CoinCounts", // labels from 1 to 8 (not 0 to 7)
                       "Chip8",
                       GameManager_script.CoinCounts[i],
                       GameManager_script.MoneyCounts[i]
                   )
                );
           }
           else if (i == 1)
           {
               Group.GetComponent<CoinButton>().ChangeCoinButton(new CoinInfo
                   (
                       "CoinCounts", // labels from 1 to 8 (not 0 to 7)
                       "Chip5",
                       GameManager_script.CoinCounts[i],
                       GameManager_script.MoneyCounts[i]
                   )
                );
           }
           else if (i == 2)
           {
               Group.GetComponent<CoinButton>().ChangeCoinButton(new CoinInfo
                   (
                       "CoinCounts", // labels from 1 to 8 (not 0 to 7)
                       "Chip7",
                       GameManager_script.CoinCounts[i],
                       GameManager_script.MoneyCounts[i]
                   )
                );
           }
           else if (i == 3)
           {
               Group.GetComponent<CoinButton>().ChangeCoinButton(new CoinInfo
                   (
                       "CoinCounts", // labels from 1 to 8 (not 0 to 7)
                       "Chip4",
                       GameManager_script.CoinCounts[i],
                       GameManager_script.MoneyCounts[i]
                   )
                );
           }
           else if (i == 4)
           {
               Group.GetComponent<CoinButton>().ChangeCoinButton(new CoinInfo
                   (
                       "CoinCounts", // labels from 1 to 8 (not 0 to 7)
                       "Chip6",
                       GameManager_script.CoinCounts[i],
                       GameManager_script.MoneyCounts[i]
                   )
                );
           }
           else if (i == 5)
           {
               Group.GetComponent<CoinButton>().ChangeCoinButton(new CoinInfo
                   (
                       "CoinCounts", // labels from 1 to 8 (not 0 to 7)
                       "Chip3",
                       GameManager_script.CoinCounts[i],
                       GameManager_script.MoneyCounts[i]
                   )
                );
           }
           Grid.GetComponent<UIGrid>().repositionNow = true;

           Buttons.Add(Group);

           countnum++;
       }

       InitInside();
   }

    void InitInside()
    {
        setheight = (BackgroundXY.y - ((Line - 1) * YDistance * BackgroundXY.y + 2 * Ygap * BackgroundXY.y)) / Line;
        setwidth = setheight * scale;
        Grid.cellHeight = YDistance * BackgroundXY.y + setheight;

        Grid.cellWidth = XDistance * BackgroundXY.x + setwidth;
        for (int i = 0; i < Buttons.Count; i++)
        {
            Buttons[i].GetComponent<UISprite>().width = (int)(setwidth);
            Buttons[i].GetComponent<UISprite>().height = (int)(setheight);
        }

        Grid.Reposition();
        Grid.GetComponent<UICenterOnChild>().distanceX = Xgap * BackgroundXY.x;
        Grid.GetComponent<UICenterOnChild>().Buttonwidth = setwidth;
        Grid.GetComponent<UICenterOnChild>().count = count;
        Grid.GetComponent<UICenterOnChild>().length = Buttons.Count;

        float columCount = Mathf.CeilToInt((float)count / (float)Line);
        float Vwidth = (columCount * setwidth + (columCount - 1) * XDistance * BackgroundXY.x) / 2;

        Grid.GetComponent<UICenterOnChild>().Recenter(true);
        Grid.transform.localPosition = new Vector3(Grid.transform.localPosition.x + Vwidth - BackgroundXY.x / 2 + Xgap * BackgroundXY.x, Grid.transform.localPosition.y, Grid.transform.localPosition.z);
    }

    public void ChangeInsideDepth(int vDepth)
    {
        InsidePanel.depth = (vDepth);
    }
}
