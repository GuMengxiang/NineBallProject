using UnityEngine;
using System.Collections;

public class GameCueOnDrag : MonoBehaviour
{
    public UIPanel RootPanel;
    public GameObject CueControl;
    public UISprite Cue;
    public GameCueBackground GCB;

    public float StartTopY;
    public float StartBottomY;

    public float initialPosition = 0.0f;

    public void UI()
    {
        StartTopY = GCB.transform.localPosition.y + Cue.transform.localPosition.y + Cue.width / 2 + RootPanel.height * 0.5f;
        StartBottomY = GCB.transform.localPosition.y + Cue.transform.localPosition.y - Cue.width / 2 + RootPanel.height * 0.5f;
    }

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            GameManager_script.Instance().DownOnRealButtons = true;

            initialPosition = Input.mousePosition.y;
        }
        else
        {
            GameManager_script.Instance().DownOnRealButtons = false;

            Cue.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f + GCB.CueBackground.height * -0.05f, 0.0f);
        }
    }

    void OnDrag()
    {
        float cueMovement = Input.mousePosition.y - initialPosition;

        if (cueMovement < StartBottomY - StartTopY)
        {
            Cue.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f + GCB.CueBackground.height * -0.05f + StartBottomY - StartTopY, 0.0f);

            if (CueControl && CueControl.GetComponent<CueController>())
            {
                CueController cc = CueControl.GetComponent<CueController>();

                cc.cueDisplacement = cc.cueMaxDisplacement;
            }
        }
        else if (cueMovement < 0)
        {
            Cue.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f + GCB.CueBackground.height * -0.05f + cueMovement, 0.0f);

            if (CueControl && CueControl.GetComponent<CueController>())
            {
                CueController cc = CueControl.GetComponent<CueController>();

                cc.cueDisplacement = cc.cueMaxDisplacement * Mathf.Abs(cueMovement) / Mathf.Abs(StartBottomY - StartTopY);
            }
        }
        else
        {
            Cue.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f + GCB.CueBackground.height * -0.05f, 0.0f);

            if (CueControl && CueControl.GetComponent<CueController>())
            {
                CueController cc = CueControl.GetComponent<CueController>();

                cc.cueDisplacement = 0.0f;
            }
        }
    }
}
