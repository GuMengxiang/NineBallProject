using UnityEngine;
using System.Collections;

public class OpenAvatarShop : MonoBehaviour
{
    public GameObject Parent;
    public UIPanel RootPanel;
    public GameObject WindowPanel;
    public WindowState state;

    void OnClick()
    {
        GameObject Group = (GameObject)Instantiate(WindowPanel, (Vector3)transform.position, (Quaternion)transform.rotation);

        Group.transform.parent = Parent.transform;
        Group.transform.localPosition = Vector3.zero;
        Group.transform.localScale = new Vector3(1, 1, 1);
        Group.GetComponent<UIWindowPanel>().RootPanel = RootPanel;
        Group.GetComponent<UIWindowPanel>().windowState = state;
    }
}
