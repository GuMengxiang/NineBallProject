using UnityEngine;
using System.Collections;

public class UIWarningPanel : MonoBehaviour
{
    public Vector2 BackgroundXY;
    public UIPanel RootPanel;
    public UILabel Title;
    public UISprite BoxBackground;
    public UISprite Background;

    public GameObject OkButton;
    public UISprite OkButtonBackground;
    public UISprite OkButtonMask;
    public UILabel OkButtonLabel;

    public GameObject CanelButton;
    public UISprite CanelButtonBackground;
    public UISprite CanelButtonMask;
    public UILabel CanelButtonLabel;

    public float PositionTitle = 3.14f;

    public float heightOkButton = 8.27f;
    public float heightCanelButton = 8.27f;
    public float PositionOkButtonX = -2.85f;
    public float PositionCanelButtonX = -2.85f;
    public float PositionOkButtonY = -2.85f;
    public float PositionCanelButtonY = -2.85f;

    public void WarningUI(WarningInfo vWarningInfo)
    {
        //窗口大小
        BackgroundXY.x = BoxBackground.width;
        BackgroundXY.y = BoxBackground.height;

        //背景框 ok
        BoxBackground.transform.localPosition = new Vector3(0, 0, 0);
        BoxBackground.width = (int)(RootPanel.width * 0.333f);
        BoxBackground.height = (int)(BoxBackground.width / 1.6f);
        BoxBackground.GetComponent<BoxCollider>().size = new Vector3(BoxBackground.width, BoxBackground.height, 0);
        BoxBackground.alpha = 0.975f; // dajiang hack, special case not 0.925f

        //遮挡图 ok
        Background.transform.localPosition = new Vector3(0, 0, 0);
        Background.height = (int)(RootPanel.height * 1.15f);
        Background.width = (int)(RootPanel.width * 1.15f);
        Background.GetComponent<BoxCollider>().size = new Vector3(RootPanel.width * 1.15f, RootPanel.height * 1.15f, 0);

        //标题
        Title.transform.localPosition = new Vector3(0, BoxBackground.height * 0.15f, 0);
        Title.width = (int)(BoxBackground.width * 0.9f);
        Title.height = (int)(BoxBackground.height * 0.5f);
        
        //Ok按钮
        OkButtonBackground.width = (int)(BoxBackground.width * 0.375f);
        OkButtonBackground.height = (int)(OkButtonBackground.width * 0.333f);
        OkButtonMask.height = OkButtonBackground.height + 2;
        OkButtonMask.width = OkButtonBackground.width + 2;
        OkButtonMask.GetComponent<BoxCollider>().size = new Vector3(OkButtonMask.width, OkButtonMask.height, 0);
        OkButton.transform.localPosition = new Vector3(-BoxBackground.width / 2 + OkButtonBackground.width / 2 + BoxBackground.width * PositionOkButtonX, -BoxBackground.height * 0.3f, 0);

        // Ok label
        OkButtonLabel.width = (int)(OkButtonBackground.width * 0.9f);
        OkButtonLabel.height = (int)(OkButtonBackground.height * 0.9f);
        OkButtonLabel.transform.localPosition = new Vector3(OkButtonLabel.transform.localPosition.x, OkButtonLabel.transform.localPosition.y - OkButtonLabel.height * 0.075f, OkButtonLabel.transform.localPosition.z);

        //Canel按钮
        CanelButtonBackground.width = (int)(BoxBackground.width * 0.375f);
        CanelButtonBackground.height = (int)(CanelButtonBackground.width * 0.333f);
        CanelButtonMask.height = CanelButtonBackground.height + 2;
        CanelButtonMask.width = CanelButtonBackground.width + 2;
        CanelButtonMask.GetComponent<BoxCollider>().size = new Vector3(CanelButtonMask.width, CanelButtonMask.height, 0);
        CanelButton.transform.localPosition = new Vector3(BoxBackground.width / 2 - CanelButtonBackground.width / 2 - BoxBackground.width * PositionCanelButtonX, -BoxBackground.height * 0.3f, 0);

        // cancel label
        CanelButtonLabel.width = (int)(CanelButtonBackground.width * 0.9f);
        CanelButtonLabel.height = (int)(CanelButtonBackground.height * 0.9f);
        CanelButtonLabel.transform.localPosition = new Vector3(CanelButtonLabel.transform.localPosition.x, CanelButtonLabel.transform.localPosition.y - CanelButtonLabel.height * 0.075f, CanelButtonLabel.transform.localPosition.z);

        // make sure alpha is 0.0f
        gameObject.GetComponent<UIPanel>().alpha = 0.0f;

        Title.text = Localization.Get(vWarningInfo.Title);
        OkButtonLabel.text = Localization.Get(vWarningInfo.OKMessage);
        CanelButtonLabel.text = Localization.Get(vWarningInfo.CancelMessage);
    }

    public void ChangeInsideDepth(int vDepth)
    {
        gameObject.GetComponent<UIPanel>().depth = (vDepth);
    }

    public void Update()
    {
        if (gameObject && gameObject.GetComponent<UIPanel>())
        {
            gameObject.GetComponent<UIPanel>().alpha = Mathf.Lerp(gameObject.GetComponent<UIPanel>().alpha, 1.0f, Time.deltaTime * 10.0f);
        }
    }

}
