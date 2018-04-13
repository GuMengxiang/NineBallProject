using UnityEngine;
using System.Collections;

public class UIDailyBonusPanel : MonoBehaviour
{
    public Vector2 BackgroundXY;
    public UIPanel WindowPanel;
    public UIPanel DailyBonusPanel; // this is the main thing   
    public UILabel Title;
    public UILabel DoorLabel;
    public UILabel Body;
    public UISprite BoxBackground;
    public UISprite MaskBackground;
    public GameObject DoorButton1;
    public UISprite DoorButtonBackground1;
    public UISprite DoorButtonMask1;
    public GameObject DoorButton2;
    public UISprite DoorButtonBackground2;
    public UISprite DoorButtonMask2;
    public GameObject DoorButton3;
    public UISprite DoorButtonBackground3;
    public UISprite DoorButtonMask3;
    public float PositionTitle = 3.14f;
    public float heightBoxBackground = 4.22f;
    public float PositionBoxBackground = 6.46f;
    public float PositionSlider = -10.15f; // -25.6f original value
    public float heightDoorButton = 8.27f;
    public float PositionDoorButton = -2.85f;
    
    public void DailyBonusUI(DailyBonusPopupInfo dailyBonusPopupInfo)
    {
        // only do this whenever we generate this window
        if (GameManager_script.Instance().ClaimedDailyBonusEver == 0.0f)
        {
            GameManager_script.Instance().CurrentBonusAmount = GameManager_script.DailyBonusArray[Random.Range((int)(GameManager_script.DailyBonusArray.Length * 0.33f), (int)(GameManager_script.DailyBonusArray.Length * 0.66f))];
        }
        else
        {
            GameManager_script.Instance().CurrentBonusAmount = GameManager_script.DailyBonusArray[Random.Range(0, (int)(GameManager_script.DailyBonusArray.Length))];
        }

        // record things showing up
        Analytic.EventHappenPing("DailyBonus Show");

        //标题
        Title.width = (int)(BackgroundXY.x * 0.75f);
        Title.height = (int)(BackgroundXY.y * 0.15f);
        Title.transform.localPosition = new Vector3(0, BackgroundXY.y / 2 - Title.height / 2 - BackgroundXY.y * 0.175f, 0);

        Body.width = (int)(BackgroundXY.x * 0.90f);
        Body.height = (int)(BackgroundXY.y * 0.10f);
        Body.transform.localPosition = new Vector3(0, BackgroundXY.y / 2 - Title.height / 2 - BackgroundXY.y * 0.30f, 0);

        if (GameManager_script.Instance().ClaimedDailyBonusEver == 0.0f)
        {
            Title.text = Localization.Get("DailyBonusTitleOtherTime");
            Body.text = Localization.Get("DailyBonusSubTitleFirstTime");
        }
        else
        {
            Title.text = Localization.Get("DailyBonusTitleOtherTime");
            Body.text = Localization.Get("DailyBonusSubTitleOtherTime");
        }

        DoorLabel.width = (int)(BackgroundXY.x / 1.5f);
        DoorLabel.height = (int)(Title.width / 3.5f);
        DoorLabel.GetComponent<TweenPosition>().animationCurve = GameManager_script.Instance().NGUITweenPosition;
        DoorLabel.GetComponent<TweenScale>().animationCurve = GameManager_script.Instance().NGUITweenScale;
        DoorLabel.text = Localization.Get("DailyBonusWon") + GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().CurrentBonusAmount, "gamecoinz") + Localization.Get("DailyBonusWonEnd");

        BoxBackground.height = (int)(BackgroundXY.y * 0.90f);
        BoxBackground.width = (int)(BackgroundXY.x * 0.9385f);
        BoxBackground.transform.localPosition = new Vector3(BackgroundXY.x * -0.0025f, 0.0f, 0.0f);

        //MaskBackground
        MaskBackground.height = (int)(BackgroundXY.y * 0.90f);
        MaskBackground.width = (int)(BackgroundXY.x * 0.9385f);
        MaskBackground.transform.localPosition = new Vector3(BackgroundXY.x * -0.0025f, 0.0f, 0.0f);

        //Door按钮1
        DoorButtonBackground1.height = (int)(BackgroundXY.y * 0.75f);
        DoorButtonBackground1.width = (int)(DoorButtonBackground1.height * 1.5f);
        DoorButtonMask1.height = DoorButtonBackground1.height;
        DoorButtonMask1.width = DoorButtonBackground1.width ;
        DoorButton1.GetComponent<BoxCollider>().size = new Vector3(DoorButtonMask1.width * 0.275f, DoorButtonMask1.height * 0.550f, 0);
        DoorButton1.GetComponent<BoxCollider>().transform.localPosition = new Vector3(0.0f, -40.0f, 0.0f);
        DoorButton1.transform.localPosition = new Vector3(-BoxBackground.width * 0.3f, -BoxBackground.height / 2 + DoorButtonBackground1.height / 2 + DoorButtonBackground1.height * 0.02f, 0);

        //Door按钮2
        DoorButtonBackground2.height = (int)(BackgroundXY.y * 0.75f);
        DoorButtonBackground2.width = (int)(DoorButtonBackground2.height * 1.5f);
        DoorButtonMask2.height = DoorButtonBackground2.height;
        DoorButtonMask2.width = DoorButtonBackground2.width;
        DoorButton2.GetComponent<BoxCollider>().size = new Vector3(DoorButtonMask2.width * 0.275f, DoorButtonMask2.height * 0.550f, 0);
        DoorButton2.transform.localPosition = new Vector3(0, -BoxBackground.height / 2 + DoorButtonBackground1.height / 2 + DoorButtonBackground1.height * 0.02f, 0);

        //Door按钮3
        DoorButtonBackground3.height = (int)(BackgroundXY.y * 0.75f);
        DoorButtonBackground3.width = (int)(DoorButtonBackground3.height * 1.5f);
        DoorButtonMask3.height = DoorButtonBackground3.height;
        DoorButtonMask3.width = DoorButtonBackground3.width;
        DoorButton3.GetComponent<BoxCollider>().size = new Vector3(DoorButtonMask3.width * 0.275f, DoorButtonMask3.height * 0.550f, 0);
        DoorButton3.transform.localPosition = new Vector3(BoxBackground.width * 0.3f, -BoxBackground.height / 2 + DoorButtonBackground1.height / 2 + DoorButtonBackground1.height * 0.02f, 0);
    }

    public void ChangeInsideDepth(int vDepth)
    {
        gameObject.GetComponent<UIPanel>().depth = (vDepth);
    }

    public void CloseDailyBonusPanel()
    {
        Destroy(WindowPanel.gameObject);

        GameManager_script.Instance().PopupCurrentlyVisible = false;

        GameManager_script.Instance().ChangeMenuMoney();
    }
}
