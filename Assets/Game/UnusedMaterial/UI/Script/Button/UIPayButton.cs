using UnityEngine;
using System.Collections;

public class UIPayButton : MonoBehaviour
{
    void OnClick()
    {
        int saleID = (int)(GameManager_script.Instance().DailyDealPopupInfoSave[0] + 5.0f); // dajiang hack, this only works when we have 6 normal packages

#if UNITY_ANDROID
        AndroidPurchases.PurchaseItem(GameManager_script.AndroidPurchasableItems[saleID]);

        Analytic.EventHappenPing(GameManager_script.Instance().DailyDealPopupInfoSave[4] + " Click");
#endif

#if UNITY_IPHONE
        StoreKitBinding.purchaseProduct(GameManager_script.IPhonePurchasableItems[saleID], 1);

        Analytic.EventHappenPing(GameManager_script.Instance().DailyDealPopupInfoSave[4] + " Click");
#endif

        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
    }
}
