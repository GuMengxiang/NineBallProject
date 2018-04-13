using UnityEngine;
using System.Collections;

public class UIHelpPlane : MonoBehaviour
{
    public Vector2 BackgroundXY;
    public UIPanel WindowPanel;
    public UIPanel DailyBonusPanel; // this is the main thing   
    public UILabel Title;
    public UISprite BoxBackground;
    
    public UILabel BodyLabel;

    public float PositionTitle = 3.14f;
    public float heightBoxBackground = 4.22f;
    public float PositionBoxBackground = 6.46f;
    public float PositionSlider = -10.15f; // -25.6f original value
    public float heightPlayButton = 8.27f;
    public float PositionPlayButton = -2.85f;

    public string Titletext;
    public string[] Bodytext;

    public void HelpUI()
    {
        BoxBackground.height = (int)(BackgroundXY.y * 0.90f);
        BoxBackground.width = (int)(BackgroundXY.x * 0.9385f);
        BoxBackground.transform.localPosition = new Vector3(BackgroundXY.x * -0.0025f, 0.0f, 0.0f);
        BoxBackground.GetComponent<BoxCollider>().size = new Vector3(BoxBackground.width, BoxBackground.height, 0);

        //标题
        Title.width = (int)(BoxBackground.width * 0.80f);
        Title.height = (int)(BoxBackground.height * 0.10f);
        Title.transform.localPosition = new Vector3(0, BoxBackground.height * 0.5f - Title.height * 0.7f, 0);
        Title.text = Titletext;

        // Body label
        BodyLabel.width = (int)(BoxBackground.width * 0.95f);
        BodyLabel.height = (int)(BoxBackground.height * 0.85f);
        BodyLabel.transform.localPosition = new Vector3(0, BoxBackground.height * 0.5f - BodyLabel.height * 0.55f - Title.height, 0);
        BodyLabel.fontSize = (int)(Screen.height / 32);
        
        for (int i = 0; i < Bodytext.Length; i++)
        {
            BoxBackground.GetComponent<UITextList>().Add(Localization.Get(Bodytext[i]));
        }
    }

    public void ChangeInsideDepth(int vDepth)
    {
        gameObject.GetComponent<UIPanel>().depth = (vDepth);
    }
}
