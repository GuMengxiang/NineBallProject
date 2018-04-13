using UnityEngine;
using System.Collections;

public class GamePlayerHead : MonoBehaviour
{
    public UIPanel RootPanel;
    public UISprite HeadMask;
    public UISprite Head;
    public UISprite DefaultBackgound;
    public UISprite LightBackground;
    public float setwidth = 0.05f;
    public Vector2 BackgroundXY;
    public int PlayerId;

    public void UI()
    {
        Head.height = (int)(BackgroundXY.x);
        Head.width = Head.height;

        DefaultBackgound.height = (int)(Head.height * 1.075f);
        DefaultBackgound.width = (int)(Head.width * 1.075f);

        LightBackground.height = (int)(Head.height);
        LightBackground.width = (int)(Head.width );

        HeadMask.height = (int)(Head.height * 1.10f);
        HeadMask.width = (int)(Head.width * 1.10f); // bigger collider

        if (GameManager_script.Instance().TrulySelfInActionGame || GameManager_script.Instance().FTUEInActionGame)
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<BoxCollider>().size = new Vector3(HeadMask.width, HeadMask.height, 0.0f);
        }

        ChangHeadBackground(PlayerHeadInfo.Light);
        ChangeHeadBackgroundValue(100);
    }

    void ChangeHeadImage(int index)
    {
        Head.spriteName = "" + index;
        LightBackground.spriteName = "" + index;
    }

    public void ChangeHeadBackgroundValue(float Val)
    {
        float count = Mathf.Clamp(Val, 0, 101);

        Head.fillAmount = (count);
    }

    public void ChangHeadBackground(PlayerHeadInfo playerHeadInfo)
    {
        if (playerHeadInfo == PlayerHeadInfo.Default)
        {
            Head.fillAmount = 1;//gameObject.SetActive(false);
        }
        else
        {
           // Head.gameObject.SetActive(true);
            Head.fillAmount = 1;
        }
    }

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            GameManager_script.Instance().DownOnRealButtons = true;

            gameObject.GetComponent<OpenPopupUI>().ProfileID = PlayerId;
        }
        else
        {
            GameManager_script.Instance().DownOnRealButtons = false;
        }
    }
}
