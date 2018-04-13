using UnityEngine;
using System.Collections;

public class GameSpinOnDrag : MonoBehaviour
{
    public UIPanel RootPanel;
    public UISprite Centerpoint;
    public Vector2 Center;
    public float Radius;
    public GameObject SpinShadow;
    public float CenterpointMovementX;
    public float CenterpointMovementY;

    public CueController cc;

    public void UI()
    {
        Center.x = RootPanel.width * 0.5f;
        Center.y = RootPanel.height * 0.5f;
        Radius = RootPanel.height * 0.5f / 1.33f;
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

            ConvertFromRectToCircle();

            Centerpoint.gameObject.transform.localPosition = new Vector3(CenterpointMovementX, CenterpointMovementY, 0);

            SetCueControllerValue();

            ChangeSpinShadow(CenterpointMovementX / Radius, CenterpointMovementY / Radius);
        }
    }

    void OnDrag()
    {
        CenterpointMovementX = Input.mousePosition.x - RootPanel.width * 0.5f;
        CenterpointMovementY = Input.mousePosition.y - RootPanel.height * 0.5f;

        ConvertFromRectToCircle();

        Centerpoint.gameObject.transform.localPosition = new Vector3(CenterpointMovementX, CenterpointMovementY, 0);

        SetCueControllerValue();

        ChangeSpinShadow(CenterpointMovementX / Radius, CenterpointMovementY / Radius);
    }

    void SetCueControllerValue()
    {
        if (cc)
        {
            cc.cueBallPivot = new Vector3(CenterpointMovementX / Radius, CenterpointMovementY / Radius, 0.0f);
        }
    }

    void ConvertFromRectToCircle()
    {
        float MultipleOfRadius = new Vector2(CenterpointMovementX, CenterpointMovementY).magnitude / Radius;
        
        if (MultipleOfRadius > 1.0f)
        {
            CenterpointMovementX = CenterpointMovementX / MultipleOfRadius;
            CenterpointMovementY = CenterpointMovementY / MultipleOfRadius;
        }
    }

    public void ChangeSpinShadow(float vCenterpointMovementX,float vCenterpointMovementY)
    {
        SpinShadow.GetComponent<GameSpinShadow>().ChangeSpinShadowCenterpoint(vCenterpointMovementX, vCenterpointMovementY);
    }
}
