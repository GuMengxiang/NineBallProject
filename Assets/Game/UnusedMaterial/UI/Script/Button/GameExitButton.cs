using UnityEngine;
using System.Collections;

public class GameExitButton : MonoBehaviour
{
    public UIPanel WindowPanel;
    public GameObject Exit;
    public GameObject CueControl;
    public Vector2 BackgroundXY;
    public GameObject WarningPrefab;

    [System.NonSerialized]
    public bool ListeningForConfirmation = false;

    public void UI()
    {
        //关闭按钮
        Exit.GetComponent<UIButton>().tweenTarget.GetComponent<UISprite>().width = (int)(BackgroundXY.x);
        Exit.GetComponent<UIButton>().tweenTarget.GetComponent<UISprite>().height = (int)(BackgroundXY.y);
        Exit.GetComponent<BoxCollider>().size = new Vector3(BackgroundXY.x * 1.25f, BackgroundXY.y * 1.25f, 0);
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

    void OnClick()
    {
        WarningUI();

        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
    }

    void WarningUI()
    {
        GameObject Group = (GameObject)Instantiate(WarningPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);

        // this tag is for in game little popups i think
        Group.transform.parent = WindowPanel.transform;
        Group.transform.localPosition = Vector3.zero; // final destination
        Group.transform.localScale = new Vector3(1, 1, 1);
        Group.GetComponent<UIWarningPanel>().RootPanel = WindowPanel;
        Group.GetComponent<UIWarningPanel>().ChangeInsideDepth(GameManager_script.UIConfirmLayerStart);
        Group.GetComponent<UIWarningPanel>().WarningUI(new WarningInfo(GameManager_script.Instance().FTUEInActionGame ? "Warning0" : "Warning1", "Yes", "No"));

        ListeningForConfirmation = true; // bitches!!!
    }

    void Update()
    {
        if (ListeningForConfirmation)
        {
            if (GameManager_script.ConfirmationState == ConfirmationType.confirmed)
            {
                // click and resign?
                if (CueControl && CueControl.GetComponent<CueController>())
                {
                    CueControl.GetComponent<CueController>().ResignFromGame();
                }

                // stop listening
                ListeningForConfirmation = false;

                // reset confirmation state
                GameManager_script.ConfirmationState = ConfirmationType.undecided;
            }
            else if (GameManager_script.ConfirmationState == ConfirmationType.denied)
            {
                // do nothing for real

                // stop listening
                ListeningForConfirmation = false;

                // reset confirmation state
                GameManager_script.ConfirmationState = ConfirmationType.undecided;
            }
            else
            {
                // do nothing and keep on listening
            }
        }
    }
}
