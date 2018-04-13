using UnityEngine;
using System.Collections;

public class UIDailyDealPanel : MonoBehaviour
{
    public Vector2 BackgroundXY;
    public UIPanel WindowPanel;
    public UIPanel DailyDealPanel; // this is the main thing   
    public UILabel Title;
    public UILabel Body;
    public UISprite BoxBackground;
    public UISprite CueBackground;

    public GameObject PayButton;
    public UISprite PayButtonBackground;
    public UISprite PayButtonMask;
    public UILabel PayButtonLabel;

    public UILabel LabelText1;
    public UILabel LabelText2;

    public UISprite MoneyImage;
    public UISprite MoneyImageRite;

    [System.NonSerialized]
    public float[] currentDeal = null;
    [System.NonSerialized]
    public string percent = "";
    [System.NonSerialized]
    public string titleString = "";
    [System.NonSerialized]
    public string subTitleString = "";
    [System.NonSerialized]
    public string subTitleStringEnd = "";
    [System.NonSerialized]
    public string final = "";
    [System.NonSerialized]
    public bool hasCue = false;

    public void DailyDealUI()
    {
        currentDeal = GameManager_script.Instance().DailyDealPopupInfoSave;

        if (currentDeal[0] == 1.0f)
        {
            if (currentDeal[1] != -1.0f)
            {
                // starter deal, has cue
                titleString = "PopupStarterDealTitle";
                subTitleString = "PopupStarterDealSubTitle1";
                subTitleStringEnd = "";

                hasCue = true;
            }
            else
            {
                // starter deal, no cue
                titleString = "PopupStarterDealTitle";
                subTitleString = "PopupStarterDealSubTitle2";
                subTitleStringEnd = "";
            }

            final = Localization.Get(subTitleString);

            Analytic.EventHappenPing("StarterDeal Show");
        }
        else
        {
            if (currentDeal[1] != -1.0f)
            {
                // daily deal, has cue
                titleString = "PopupDailyDealTitle";
                subTitleString = "PopupDailyDealSubTitle11";
                subTitleStringEnd = "PopupDailyDealSubTitle12";

                hasCue = true;
            }
            else
            {
                // daily deal, no cue
                titleString = "PopupDailyDealTitle";
                subTitleString = "PopupDailyDealSubTitle21";
                subTitleStringEnd = "PopupDailyDealSubTitle22";
            }

            percent = GameManager_script.convertNumberIntoGoodStringFormat(currentDeal[3] / currentDeal[2], "tinypercentage");

            final = Localization.Get(subTitleString) + percent + Localization.Get(subTitleStringEnd);

            Analytic.EventHappenPing("DailyDeal Show");
        }

        // title
        Title.width = (int)(BackgroundXY.x * 0.65f);
        Title.height = (int)(Title.width * 0.10f);
        Title.transform.localPosition = new Vector3(0, BackgroundXY.y * 0.5f - Title.height - BackgroundXY.y * 0.05f, 0);
        Title.text = Localization.Get(titleString);

        // sub title
        Body.width = (int)(BackgroundXY.x * 0.9f);
        Body.height = (int)(BackgroundXY.y * 0.065f);
        Body.transform.localPosition = new Vector3(0, BackgroundXY.y * 0.5f - Title.height * 0.5f - BackgroundXY.y * 0.21f, 0);
        Body.text = final;

        MoneyImage.width = (int)(BackgroundXY.x * 0.200f);
        MoneyImage.height = (int)(BackgroundXY.x * 0.200f);
        MoneyImage.transform.localPosition = new Vector3(BackgroundXY.x * -0.2220f, BackgroundXY.y * 0.0125f, 0.0f);
        MoneyImage.spriteName = GetChipTypeBasedOnChips(0.0f);

        MoneyImageRite.width = (int)(BackgroundXY.x * 0.200f);
        MoneyImageRite.height = (int)(BackgroundXY.x * 0.200f);
        MoneyImageRite.transform.localPosition = new Vector3(BackgroundXY.x * 0.2075f, BackgroundXY.y * 0.0125f, 0.0f);
        MoneyImageRite.spriteName = GetChipTypeBasedOnChips(0.0f);

        LabelText1.width = (int)(BackgroundXY.x * 0.33f);
        LabelText1.height = (int)(LabelText1.width * 0.130f);
        LabelText1.transform.localPosition = new Vector3(BackgroundXY.x * -0.2220f, BackgroundXY.y * -0.1625f, 0);
        LabelText1.text = GameManager_script.convertNumberIntoGoodStringFormat(currentDeal[2], "gamecoinz");

        LabelText2.width = (int)(BackgroundXY.x * 0.33f);
        LabelText2.height = (int)(LabelText2.width * 0.130f);
        LabelText2.transform.localPosition = new Vector3(BackgroundXY.x * 0.2075f, BackgroundXY.y * -0.1625f, 0);
        LabelText2.text = GameManager_script.convertNumberIntoGoodStringFormat(currentDeal[3], "gamecoinz");

        if (currentDeal[1] != -1.0f)
        {
            CueBackground.width = (int)(BackgroundXY.x * 0.9f);
            CueBackground.height = (int)(CueBackground.width / 8.5f);
            CueBackground.transform.localPosition = new Vector3(0, BackgroundXY.y * -0.25f, 0);
            CueBackground.spriteName = currentDeal[1].ToString();

            //Play按钮
            PayButtonBackground.height = (int)(BackgroundXY.y / 8.27f);
            PayButtonBackground.width = (int)(BackgroundXY.y / 8.27f * 4.05f);
            PayButtonMask.height = (int)(BackgroundXY.y / 8.27f) + 2;
            PayButtonMask.width = (int)(BackgroundXY.y / 8.27f * 4.05f) + 2;
            PayButtonMask.GetComponent<BoxCollider>().size = new Vector3(PayButtonMask.width, PayButtonMask.height, 0);
            PayButton.transform.localPosition = new Vector3(0, BackgroundXY.y * -0.5f + PayButtonBackground.height * 0.5f + BackgroundXY.y * 0.085f, 0);

            // play label
            PayButtonLabel.width = (int)(PayButtonMask.width * 0.950f);
            PayButtonLabel.height = (int)(PayButtonMask.height * 0.700f);
            PayButtonLabel.transform.localPosition = new Vector3(PayButtonLabel.transform.localPosition.x, PayButtonLabel.transform.localPosition.y - PayButtonLabel.height * 0.015f, PayButtonLabel.transform.localPosition.z);
            PayButtonLabel.text = Localization.Get("PopupDealOnlyTextBefore") + currentDeal[4] + " " + Localization.Get("USD") + Localization.Get("PopupDealOnlyTextAfter");
        }
        else
        {
            CueBackground.width = (int)(BackgroundXY.x * 0.9f);
            CueBackground.height = (int)(CueBackground.width / 8.5f);
            CueBackground.transform.localPosition = new Vector3(0, BackgroundXY.y * -0.25f, 0);
          //  CueBackground.gameObject.SetActive(false);
			CueBackground.gameObject.SetActive(true );
            CueBackground.spriteName = "5";

            //Play按钮
            PayButtonBackground.height = (int)(BackgroundXY.y / 8.27f);
            PayButtonBackground.width = (int)(BackgroundXY.y / 8.27f * 4.05f);
            PayButtonMask.height = (int)(BackgroundXY.y / 8.27f) + 2;
            PayButtonMask.width = (int)(BackgroundXY.y / 8.27f * 4.05f) + 2;
            PayButtonMask.GetComponent<BoxCollider>().size = new Vector3(PayButtonMask.width, PayButtonMask.height, 0);
            PayButton.transform.localPosition = new Vector3(0, BackgroundXY.y * -0.5f + PayButtonBackground.height * 0.5f + BackgroundXY.y * 0.110f, 0);

            // play label
            PayButtonLabel.width = (int)(PayButtonMask.width * 0.950f);
            PayButtonLabel.height = (int)(PayButtonMask.height * 0.700f);
            PayButtonLabel.transform.localPosition = new Vector3(PayButtonLabel.transform.localPosition.x, PayButtonLabel.transform.localPosition.y - PayButtonLabel.height * 0.015f, PayButtonLabel.transform.localPosition.z);
            PayButtonLabel.text = Localization.Get("PopupDealOnlyTextBefore") + currentDeal[4] + " " + Localization.Get("USD") + " " + Localization.Get("PopupDealOnlyTextAfter");
        }

        BoxBackground.height = (int)(BackgroundXY.y * 0.90f);
        BoxBackground.width = (int)(BackgroundXY.x * 0.9385f);
        BoxBackground.transform.localPosition = new Vector3(BackgroundXY.x * -0.0025f, 0.0f, 0.0f);
    }

    public void ChangeInsideDepth(int vDepth)
    {
        gameObject.GetComponent<UIPanel>().depth = (vDepth);
    }

    public string GetChipTypeBasedOnChips(float inAmount) // dajiang hack, only use 2, 3, 4 coz everything else look ugly
    {
        return ChipType.Chip2.ToString();
    }
}
