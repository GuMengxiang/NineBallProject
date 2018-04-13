using UnityEngine;
using System.Collections;

public class OpenPopupUI : MonoBehaviour
{
    public GameObject Parent;
    public UIPanel RootPanel;
    public GameObject WindowPanel;
    public PopupWindowState state;
    private GameObject Group;
    public int ProfileID;
    public DailyBonusPopupInfo dailyBonusPopupInfo;
    public string Titletext;
    public string[] Bodytext;
    public bool isFrdsSelector;

    void OnClick()
    {
        OpenUI(state);

        GameManager_script.Instance().isFrdsSelector = isFrdsSelector;

        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
    }

    void Update()
    {
        if (Group && Group.GetComponent<UIPanel>())
        {
            Group.GetComponent<UIPanel>().alpha = Mathf.Lerp(Group.GetComponent<UIPanel>().alpha, 1.0f, Time.deltaTime * 10.0f);
        }
    }

    public void OpenUI(PopupWindowState state)
    {
        if (!GameManager_script.Instance().PopupCurrentlyVisible)
        {
            Group = (GameObject)Instantiate(WindowPanel, (Vector3)transform.position, (Quaternion)transform.rotation);

            Group.transform.parent = Parent.transform;
            Group.transform.localPosition = Vector3.zero;
            Group.transform.localScale = Vector3.one;
            Group.GetComponent<UIPopupWindowPanel>().RootPanel = RootPanel;
            Group.GetComponent<UIPopupWindowPanel>().WindowState = state;
            Group.GetComponent<UIPopupWindowPanel>().WindowPanelPrefab = WindowPanel;

            if (PopupWindowState.PlayerProfile == state)
            {
                Group.GetComponent<UIPopupWindowPanel>().ProfileID = ProfileID;
            }
            else if (PopupWindowState.Help == state)
            {
                Group.GetComponent<UIPopupWindowPanel>().Titletext = Titletext;
                Group.GetComponent<UIPopupWindowPanel>().Bodytext = Bodytext;
            }
            else if (PopupWindowState.DailyBonus == state)
            {
                Group.GetComponent<UIPopupWindowPanel>().dailyBonusPopupInfo = dailyBonusPopupInfo;
            }
            else if (PopupWindowState.DailyDeal == state)
            {
                GameManager_script.Instance().newDailyDealPrStarterPackageSeen = 1; // 1 is seen
                PlayerPrefs.SetInt("newDailyDealPrStarterPackageSeen", 1);
            }
            else if (PopupWindowState.StarterDeal == state)
            {
                GameManager_script.Instance().newDailyDealPrStarterPackageSeen = 1; // 1 is seen
                PlayerPrefs.SetInt("newDailyDealPrStarterPackageSeen", 1);
            }

            // add shiites to the popup stack, that means we already have popups
            GameManager_script.Instance().AllowPopupToShowOnPage(Group);

            // animation preparation
            Group.GetComponent<UIPanel>().alpha = 0.0f;
        }
    }
}
