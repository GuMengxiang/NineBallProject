using UnityEngine;
using System.Collections;

public class Close : MonoBehaviour
{
    public GameObject UI; // we can animate and destroy against this one dude, that is good stuff.

    public bool slideOutAnimation = false;
    public bool fadeOutAnimation = false;

    void OnClick()
    {
        if (UI.GetComponent<UIWindowPanel>())
        {
            slideOutAnimation = true;

            GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
        }

        if (UI.GetComponent<UIPopupWindowPanel>())
        {
            fadeOutAnimation = true;

            GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
        }

        GameManager_script.Instance().ChangeMenuMoney();
    }

    void Update()
    {
        if (slideOutAnimation && UI)
        {
            Vector3 targetPosition = new Vector3(0.0f, 0.0f - 2.0f * Screen.height, 0.0f);

            UI.transform.localPosition = Vector3.Lerp(UI.transform.localPosition, targetPosition, Time.deltaTime * 10.0f);

            if ((UI.transform.localPosition - targetPosition).magnitude < Screen.height)
            {
                GameManager_script.Instance().RemoveMenuFromStack(UI);

                slideOutAnimation = false;

                Destroy(UI);
            }
        }

        if (fadeOutAnimation && UI && UI.GetComponent<UIPanel>())
        {
            UI.GetComponent<UIPanel>().alpha = Mathf.Lerp(UI.GetComponent<UIPanel>().alpha, -1.0f, Time.deltaTime * 10.0f);

            if (UI.GetComponent<UIPanel>().alpha < 0.01f)
            {
                GameManager_script.Instance().AllowPopupToHideOnPage();

                fadeOutAnimation = false;

                Destroy(UI);
            }
        }
    }

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            GameManager_script.Instance().DownOnRealButtons = true;
        }
        else
        {
            GameManager_script.Instance().DownOnRealButtons = false;
        }
    }
}
