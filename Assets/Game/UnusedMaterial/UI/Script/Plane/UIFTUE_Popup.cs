using UnityEngine;
using System.Collections;

public class UIFTUE_Popup : MonoBehaviour
{
    public Vector2 BackgroundXY;
    public UIPanel WindowPanel;
    public UIPanel DailyBonusPanel; // this is the main thing   
    public UILabel Title;
    public UISprite BoxBackground;
    public UILabel BodyLabel;

    public GameObject PlayButton;
    public UISprite PlayButtonBackground;
    public UISprite PlayButtonMask;
    public UILabel PlayButtonLabel;

    public float PositionTitle = 3.14f;
    public float heightBoxBackground = 4.22f;
    public float PositionBoxBackground = 6.46f;
    public float PositionSlider = -10.15f; // -25.6f original value
    public float heightPlayButton = 8.27f;
    public float PositionPlayButton = -2.85f;

    public string Titletext;
    public string Bodytext;

    public void FTUE_Popup() // this is not being used in V1
    {
        BoxBackground.height = (int)(BackgroundXY.y * 0.90f);
        BoxBackground.width = (int)(BackgroundXY.x * 0.9385f);
        BoxBackground.transform.localPosition = new Vector3(BackgroundXY.x * -0.0025f, 0.0f, 0.0f);
        BoxBackground.GetComponent<BoxCollider>().size = new Vector3(BoxBackground.width, BoxBackground.height, 0);

        // 标题
        Title.width = (int)(BackgroundXY.x * 0.65f);
        Title.height = (int)(BackgroundXY.y * 0.12f);
        Title.transform.localPosition = new Vector3(0, BackgroundXY.y / 2 - Title.height / 2 - BackgroundXY.y * 0.10f, 0);
        Title.text = Localization.Get("FtueTitle");

        BodyLabel.width = (int)(BackgroundXY.x * 0.9f);
        BodyLabel.height = (int)(BackgroundXY.y * 0.06f);
        BodyLabel.transform.localPosition = new Vector3(0, BackgroundXY.y / 2 - Title.height / 2 - BackgroundXY.y * 0.19f, 0);
        BodyLabel.text = Localization.Get("FtueSubtitle");

        // Play按钮
        PlayButtonBackground.height = (int)(BackgroundXY.y / heightPlayButton);
        PlayButtonBackground.width = (int)(BackgroundXY.y / heightPlayButton * 4.05f);
        PlayButtonMask.height = (int)(BackgroundXY.y / heightPlayButton) + 2;
        PlayButtonMask.width = (int)(BackgroundXY.y / heightPlayButton * 4.05f) + 2;
        PlayButtonMask.GetComponent<BoxCollider>().size = new Vector3(PlayButtonMask.width, PlayButtonMask.height, 0);
        PlayButton.transform.localPosition = new Vector3(0, BackgroundXY.y / PositionPlayButton + BackgroundXY.y * 0.045f, 0);

        // play label
        PlayButtonLabel.width = (int)(BackgroundXY.y / heightPlayButton * 4.05f * 0.9f);
        PlayButtonLabel.height = (int)(BackgroundXY.y / heightPlayButton * 0.9f);
        PlayButtonLabel.transform.localPosition = new Vector3(PlayButtonLabel.transform.localPosition.x, PlayButtonLabel.transform.localPosition.y - PlayButtonLabel.height * 0.045f, PlayButtonLabel.transform.localPosition.z);
        PlayButtonLabel.text = Localization.Get("FtueEnter");
    }

    public void ChangeInsideDepth(int vDepth)
    {
        gameObject.GetComponent<UIPanel>().depth = (vDepth);
    }
}
