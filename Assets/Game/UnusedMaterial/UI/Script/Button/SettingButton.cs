using UnityEngine;
using System.Collections;

public class SettingButton : MonoBehaviour
{
    public UIPanel WindowPanel;
    public int buttonId = 0;
    public UISprite Background;
    public UISprite MaskBackground;
    public UILabel Settinglabel;
    public UISprite OkBackground;
    public UISprite MaskOkBackground;
    public UISprite CancelBackground;
    public UISprite MaskCancelBackground;
    public UISprite LookBackground;
    public UISprite MaskLookBackground;
    public UILabel LookLabel;
    public UILabel Label;
    public float xdistance;
    public float ydistance;
    public ClickType Clicktype;

    void Start()
    {
        UI();
    }

    void UI()
    {
        Background.GetComponent<BoxCollider>().size = new Vector3(Background.width + xdistance, Background.height + ydistance, 0);

        MaskBackground.width = Background.width;
        MaskBackground.height = Background.height;

        // left side label
        Settinglabel.width = (int)(Background.width * 0.35f);
        Settinglabel.height = (int)(Background.height * 0.35f);
        Settinglabel.transform.localPosition = new Vector3(Background.width * -0.5f + Background.height * 0.25f, 0, 0); // try to give it the same border as height difference

        // all about OK button
        OkBackground.height = (int)(Background.height * 0.5f);
        OkBackground.width = (int)(OkBackground.height * 2.614f);

        MaskOkBackground.height = (int)(OkBackground.height * 1.00f);
        MaskOkBackground.width = (int)(OkBackground.width * 1.00f);

        OkBackground.GetComponent<BoxCollider>().size = new Vector3(OkBackground.width * 1.00f, OkBackground.height * 1.00f, 0);
        OkBackground.transform.localPosition = new Vector3(Background.width * 0.5f - OkBackground.width * 0.5f - Background.height * 0.25f, 0, 0);

        // all about cancel button
        CancelBackground.height = (int)(Background.height * 0.5f);
        CancelBackground.width = (int)(CancelBackground.height * 2.614f);

        MaskCancelBackground.height = (int)(CancelBackground.height * 1.00f);
        MaskCancelBackground.width = (int)(CancelBackground.width * 1.00f);

        CancelBackground.GetComponent<BoxCollider>().size = new Vector3(CancelBackground.width * 1.00f, CancelBackground.height * 1.00f, 0);
        CancelBackground.transform.localPosition = new Vector3(Background.width * 0.5f - CancelBackground.width * 0.5f - Background.height * 0.25f, 0, 0);

        // all about Look button
        LookBackground.height = (int)(Background.height * 0.5f);
        LookBackground.width = (int)(LookBackground.height * 3f);

        MaskLookBackground.height = (int)(LookBackground.height * 1.00f);
        MaskLookBackground.width = (int)(LookBackground.width * 1.00f);

        LookBackground.GetComponent<BoxCollider>().size = new Vector3(LookBackground.width * 1.00f, LookBackground.height * 1.00f, 0);
        LookBackground.transform.localPosition = new Vector3(Background.width * 0.5f - LookBackground.width * 0.5f - Background.height * 0.25f, 0, 0);
		///-----GU---更改高度 0.65～1
        LookLabel.height = (int)(LookBackground.height * 1f);
        LookLabel.width = (int)(LookBackground.width * 0.95f);
		///-----20170308----

        // if there is no button but just a label
        Label.width = (int)(Background.width * 0.35f);
        Label.height = (int)(Background.height * 0.35f);
        Label.transform.localPosition = new Vector3(Background.width * 0.5f - Background.height * 0.25f, 0, 0);

        if (Clicktype == ClickType.Enable)
        {
            OkBackground.gameObject.SetActive(false);
            CancelBackground.gameObject.SetActive(true);
            Label.gameObject.SetActive(false);
            LookBackground.gameObject.SetActive(false);
        }
        else if (Clicktype == ClickType.Disable)
        {
            OkBackground.gameObject.SetActive(true);
            CancelBackground.gameObject.SetActive(false);
            Label.gameObject.SetActive(false);
            LookBackground.gameObject.SetActive(false);
        }
        else if (Clicktype == ClickType.Label)
        {
            OkBackground.gameObject.SetActive(false);
            CancelBackground.gameObject.SetActive(false);
            Label.gameObject.SetActive(true);
            LookBackground.gameObject.SetActive(false);
        }
        else if (Clicktype == ClickType.Look)
        {           
            OkBackground.gameObject.SetActive(false);
            CancelBackground.gameObject.SetActive(false);
            Label.gameObject.SetActive(false);
            LookBackground.gameObject.SetActive(true);
        }
    }
}
