using UnityEngine;
using System.Collections;

public class ShopMainButton : MonoBehaviour
{
    // ShopMain component
    public UIPanel WindowPanel;
    public int buttonId = 0;
    public UISprite Background;
    public UISprite MaskBackground;
    public UISprite MaskShopMainButton;
    public float xdistance;
    public float ydistance;
    public GameObject LabelPrefab;
    public UISprite ButtonImage;
    public UILabel  ButtonName;
    public GameObject ActualBannerLabel;

    // Use this for initialization
    void Start()
    {
        UI();
    }

    void UI()
    {
        Background.GetComponent<BoxCollider>().size = new Vector3(Background.width + xdistance, Background.height + ydistance, 0);

        MaskBackground.width = Background.width;
        MaskBackground.height = Background.height;

        // MaskShopMainButton
        MaskShopMainButton.width = Background.width;
        MaskShopMainButton.height = Background.height;

        // dajiang hack, picture's ratio is 2.16f measured, and problems start to appear below the 1.75f button ratio line
        float buttonBackgroundRatio = 1.0f * Background.width / Background.height;
        float buttonRatioSubtraction = buttonBackgroundRatio < 1.80f ? buttonBackgroundRatio : 1.80f;
        float buttonImageNaturalRatio = 2.16f - (1.80f - buttonRatioSubtraction);

        // ButtonImage
        ButtonImage.height = (int)(Background.height * 1.0f);
        ButtonImage.width = (int)(ButtonImage.height * buttonImageNaturalRatio);
        ButtonImage.transform.localPosition = Vector3.zero;

        //ButtonName
        ButtonName.height = (int)(Background.height * 0.13f);
        ButtonName.width = (int)(Background.width);
        ButtonName.transform.localPosition = new Vector3(0, -Background.height * 0.5f + ButtonName.height * 0.5f + Background.height * 0.025f, 0);

        // try to init lock label texts...
        if (buttonId == 0)
        {
            if (GameManager_script.Instance().newDailyDealPrStarterPackageSeen == 0 && GameManager_script.Instance().CurrentLevel > 1.0f) // is not seen, special case we don't show purchase banner till level 2
            {
                InitLockLabelText(true, "NEW");
            }
        }

        if (buttonId == 1)
        {
            if (GameManager_script.Instance().GetNewCueCount() > 0)
            {
                InitLockLabelText(true, "NEW");
            }
        }
        
        if (buttonId == 2)
        {
            if (GameManager_script.Instance().GetNewCoinCount() > 0)
            {
                InitLockLabelText(true, "NEW");
            }
        }
        
        if (buttonId == 3)
        {
            if (GameManager_script.Instance().GetNewAvatarCount() > 0)
            {
                InitLockLabelText(true, "NEW");
            }
        }
    }

    public void InitLockLabelText(bool inUse, string inLabel)
    {
        if (inUse)
        {
            Destroy(ActualBannerLabel); // destory old one

            ActualBannerLabel = (GameObject)Instantiate(LabelPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);

            ActualBannerLabel.transform.parent = gameObject.transform;

            ActualBannerLabel.GetComponent<Label>().GetComponent<UISprite>().width = (int)(Background.width * 0.33f);
            ActualBannerLabel.GetComponent<Label>().GetComponent<UISprite>().height = (int)(Background.width * 0.33f);

            ActualBannerLabel.transform.localPosition = new Vector3(Background.width * -0.5f + ActualBannerLabel.GetComponent<Label>().GetComponent<UISprite>().width * 0.5f, Background.height * 0.5f + ActualBannerLabel.GetComponent<Label>().GetComponent<UISprite>().height * -0.5f, 0.0f); // final destination
            ActualBannerLabel.transform.localScale = new Vector3(1, 1, 1);

            ActualBannerLabel.GetComponent<Label>().labeltext = Localization.Get(inLabel);
        }
        else
        {
            Destroy(ActualBannerLabel); // destory old one
        }
    }

    public void OnClick()
    {
        // what does this do?
       
    }
}
