using UnityEngine;
using System.Collections;

public class GameSpinShadow : MonoBehaviour
{
    public UIPanel RootPanel;
    public Vector2 BackgroundXY;
    public GameObject SpinShadow;
    public GameObject CueControl;
    public UISprite SpinShadowBackground;
    public UISprite SpinShadowMask;
    public GameObject SpinNaked;
    public UISprite Centerpoint;

    public void SpinShadowUI()
    {
        SpinShadowBackground.width = (int)(BackgroundXY.x);
        SpinShadowBackground.height = (int)(BackgroundXY.y);
        SpinShadowMask.width = (int)(BackgroundXY.x);
        SpinShadowMask.height = (int)(BackgroundXY.y);
        SpinShadow.GetComponent<BoxCollider>().size = new Vector3(BackgroundXY.x * 1.67f, BackgroundXY.y * 1.67f, 0);

        Centerpoint.width = (int)(BackgroundXY.x / 8.31f);
        Centerpoint.height = (int)(BackgroundXY.y / 8.31f);
        Centerpoint.transform.localPosition = Vector3.zero;
    }

    public void ChangeSpinShadowCenterpoint(float CenterpointMovementX, float CenterpointMovementY)
    {
        float Radius = SpinShadowBackground.height / 2 / 1.14f;

        Centerpoint.transform.localPosition = new Vector3(CenterpointMovementX * Radius, CenterpointMovementY * Radius, 0);
    }

    void OnClick()
    {
        // we can ONLY click this open when balls are all sleeping and you are in control
        if (CueControl != null && CueControl.GetComponent<CueController>() != null)
        {
            CueController cc = CueControl.GetComponent<CueController>();

            if (cc.allIsSleeping && (cc.SoloMasterInControl() || cc.SoloSecondPersonInControl() || cc.BotMasterInControl() || cc.NetworkMasterInControl()))
            {
                SpinNaked.SetActive(true);
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
