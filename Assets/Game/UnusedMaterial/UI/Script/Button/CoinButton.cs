using UnityEngine;
using System.Collections;
using Prime31;

public class CoinButton : MonoBehaviour
{
    public UIPanel WindowPanel;
    public int buttonId = 0;
    public UISprite Background;
    public UISprite MaskBackground;
    public UISprite MaskButtonBackground;
    public UISprite Image;
    public UILabel Title;
    public UILabel CoinCount;
    public UILabel PayMoney;
    public float xdistance;
    public float ydistance;
    public GameObject LabelPrefab;
    public CoinInfo vCoinInfo;

    void Start()
    {
        CoinButtonUI();
    }

    public void CoinButtonUI()
    {
        Background.GetComponent<BoxCollider>().size = new Vector3(Background.width + xdistance, Background.height + ydistance, 0);

        // mask
        MaskBackground.width = Background.width;
        MaskBackground.height = Background.height;

        // MaskButtonBackground
        MaskButtonBackground.width = Background.width;
        MaskButtonBackground.height = Background.height;

        // 图片框
        float heightWidthOffset = 0.0f;

        if (Background.height > 0.65f * Background.width)
        {
            heightWidthOffset = (0.65f * Background.width) / Background.height;
        }
        else
        {
            heightWidthOffset = 1.0f;
        }

        // image
        Image.height = (int)(Background.height * GameManager_script.ImageRatio[buttonId] * heightWidthOffset);
        Image.width = (int)(Image.height * 1.023f);
        Image.transform.localPosition = new Vector3(-Background.width * 0.225f, Background.height * 0.015f, 0);

        // 金币数量
        CoinCount.width = (int)(Background.width * 0.375f);
        CoinCount.height = (int)(Background.height * 0.125f);
        CoinCount.transform.localPosition = new Vector3(Background.width * 0.235f, 0, 0);

        // 金币名称
        Title.width = (int)(Background.width * 0.450f);
        Title.height = (int)(Background.height * 0.125f);
        Title.transform.localPosition = new Vector3(Background.width * 0.235f, CoinCount.height * 1.05f, 0);

        // 金币价格
        PayMoney.width = (int)(Background.width);
        PayMoney.height = (int)(Background.height * 0.13f);
        PayMoney.transform.localPosition = new Vector3(0, -Background.height * 0.5f + PayMoney.height * 0.5f + Background.height * 0.025f, 0);

        InitLabel();
    }

    void InitLabel()
    {
        if (buttonId == 0 || buttonId == 1 || buttonId == 2)
        {
            GameObject Group = (GameObject)Instantiate(LabelPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);

            Group.transform.parent = gameObject.transform;

            Group.GetComponent<Label>().GetComponent<UISprite>().width = (int)(Background.width * 0.40f);
            Group.GetComponent<Label>().GetComponent<UISprite>().height = (int)(Background.width * 0.40f);

            switch (buttonId)
            {
                case 0: Group.GetComponent<Label>().labeltext = Localization.Get("3XChips"); break;
                case 1: Group.GetComponent<Label>().labeltext = Localization.Get("PopDeal"); break;
                case 2: Group.GetComponent<Label>().labeltext = Localization.Get("2XChips"); break;
            }

            Group.transform.localPosition = new Vector3(Background.width * -0.5f + Group.GetComponent<Label>().GetComponent<UISprite>().width * 0.5f, Background.height * 0.5f + Group.GetComponent<Label>().GetComponent<UISprite>().height * -0.5f, 0.0f); // final destination
            Group.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void ChangeCoinButton(CoinInfo vCoinInfo)
    {
        Image.spriteName = vCoinInfo.ImageName;
        Title.text = Localization.Get(vCoinInfo.Title);
        PayMoney.text = GameManager_script.convertNumberIntoGoodStringFormat(vCoinInfo.PayMoney, "largenumber") + " " + Localization.Get("USD"); // dajiang hack, this is kinda hacky
        CoinCount.text = GameManager_script.convertNumberIntoGoodStringFormat(vCoinInfo.CoinCount, "gamecoinz");
    }

    public void OnClick()
    {
		//GameManager_script.Instance().UpdateCoinCount(-10.0f);
#if UNITY_ANDROID
        AndroidPurchases.PurchaseItem(GameManager_script.AndroidPurchasableItems[buttonId]);

        Analytic.EventHappenPing(GameManager_script.MoneyCounts[buttonId] + " Click");
#endif

#if UNITY_IPHONE
        StoreKitBinding.purchaseProduct(GameManager_script.IPhonePurchasableItems[buttonId], 1);

        Analytic.EventHappenPing(GameManager_script.MoneyCounts[buttonId] + " Click");
#endif

        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
    }
}
