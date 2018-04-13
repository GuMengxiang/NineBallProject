using UnityEngine;
using System.Collections;

public class GameCueBackground : MonoBehaviour
{
    public UIPanel RootPanel;
    public UISprite CueBackground;
    public UISprite Cue;
    public UISprite CueMask;
    public UIPanel ScrollView;
    public Vector2 BackgroundXY;

    void Start()
    {
        UI();
    }

    public void UI()
    {
        //背景框
        float cueBGRatio = CueBackground.height / CueBackground.width;
        CueBackground.width = (int)(BackgroundXY.x);
        CueBackground.height = (int)(CueBackground.width * cueBGRatio);
        CueBackground.GetComponent<BoxCollider>().size = new Vector3(CueBackground.width, CueBackground.height, 0); // making it a little wider
        CueBackground.GetComponent<GameCueOnDrag>().UI();
       
        // slider
        float cueRatio = Cue.width / Cue.height;
        Cue.width = (int)(CueBackground.height); // width and height is 90 degrees, so this is right.
        Cue.height = (int)(Cue.width / cueRatio * 1.25f);
        Cue.transform.localPosition = new Vector3(0.0f, 0.0f + CueBackground.height * -0.05f, 0.0f);

        // mask
        CueMask.width = (int)(CueBackground.height);
        CueMask.height = (int)(CueMask.width / cueRatio * 1.25f);

        // the 90% is imperically measured...
        ScrollView.SetRect(0, 0, CueBackground.width * 1.0f, CueBackground.height * 0.9f); // scroll rect should be about 90% in height, so the cue doesn't poke out on either side
        ScrollView.transform.localPosition = new Vector3(0.0f, CueBackground.height * -0.013f, 0.0f); // measured...
    }

    public void ChangeCueImage(float id)
    {
        Cue.spriteName = id.ToString();
        CueMask.spriteName = id.ToString();
    }
}
