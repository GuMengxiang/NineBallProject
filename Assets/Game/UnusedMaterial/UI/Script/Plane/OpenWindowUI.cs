using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OpenWindowUI : MonoBehaviour
{
    public GameObject Parent;
    public UIPanel RootPanel;
    public GameObject WindowPanel;
    public WindowState state;
    private GameObject Group;

   public void OnClick()
    {
        // this "Group" thing, despite being a local variable, is the SetActive handle I need
        Group = (GameObject)Instantiate(WindowPanel, (Vector3)transform.position, (Quaternion)transform.rotation);

        // declaration
        Group.transform.parent = Parent.transform;
        Group.transform.localPosition = Vector3.zero; // final destination
        Group.transform.localScale = new Vector3(1, 1, 1);
        Group.GetComponent<UIWindowPanel>().RootPanel = RootPanel;
        Group.GetComponent<UIWindowPanel>().windowState = state;
        Group.GetComponent<UIWindowPanel>().Title.GetComponent<UILabel>().text = state.ToString();
        Group.GetComponent<UIWindowPanel>().WindowPanelPrefab = WindowPanel;

        // stop rendering the background page and add current page to page stack
        GameManager_script.Instance().AddMenuToStack(Group);

        // start animation sequence here
        Group.transform.localPosition = new Vector3(0.0f, 0.0f - Screen.height, 0.0f);

        // playing music?
        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
    }

    void Update()
    {
        if (Group)
        {
            Group.transform.localPosition = Vector3.Lerp(Group.transform.localPosition, Vector3.zero, Time.deltaTime * 10.0f);
        }
    }
}
