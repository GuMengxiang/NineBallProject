
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class Demo_IAB : MonoBehaviour {
	public GUISkin skin;
	void Start(){
		AndroidPurchases.StartInAppBilling(); // Now calls Load on OnIabSetupFinished callback
	}
	
	void OnDestroy(){
		AndroidPurchases.StopInAppBilling();
	}

	void OnGUI(){
		if (skin != null)
        {
            GUI.skin = skin;
        }
		
		BeginPage(Screen.width/1.3f, Screen.height/1.1f );
		GUILayout.Label("In App Purchasing available: " + AndroidPurchases.iabsetup);	
		if(GUILayout.Button("Restore")){
			AndroidPurchases.RestoreTransactions();
		}

		GUILayout.Label("Items for sale:");	

		foreach(string purchasableitem in AndroidPurchases.PurchasableItems){
			GUI.enabled = !(AndroidPurchases.HasPurchased(purchasableitem));
			if(GUILayout.Button(purchasableitem)){
				AndroidPurchases.PurchaseItem(purchasableitem);
			}
			GUI.enabled =true;
		}		
		GUILayout.Label("Items you own:");	

		GUILayout.BeginHorizontal ();
		GUILayout.Label("Gold Coins:" + AndroidPurchases.Instance.GoldCoins);	
		if (GUILayout.Button ("Spend 1 Gold")) {
			AndroidPurchases.Instance.GoldCoins -= 1;  
			AndroidPurchases.Save();//Save the fact that the user has spent some of his gold coins
		}

		GUILayout.EndHorizontal ();
		foreach(PurchaseInfo pinfo in AndroidPurchases.Instance.PurchaseInfos){
			GUILayout.BeginHorizontal();
			GUILayout.Label(pinfo.orderId);	
			GUILayout.Label(pinfo.productId);	
			GUILayout.EndHorizontal();
		}

		GUILayout.Label("Log Info:");	
		GUILayout.Label(AndroidPurchases.LogInfo());	
		
		EndPage();
	}
	
	
	
	void PurchaseItem(string productID)
	{
#if UNITY_EDITOR
			string purchasestate = "";
      		Debug.Log("Unity Editor - PurchaseItem");
			switch(productID)
			{
				case "android.test.canceled":
					purchasestate = "CANCELED";
					break;
				case "android.test.refunded":
					purchasestate = "REFUNDED";
					break;
				case "android.test.item_unavailable":
					purchasestate = "UNAVAILABLE";
					break;
			
				default:
					purchasestate = "PURCHASED";
					break;
			}
			
			UnityGameObjectReceiverIAB unitygameobjectreceiver = GameObject.Find("UnityGameObjectReceiver").GetComponent<UnityGameObjectReceiverIAB>();
			PurchaseInfo purchaseInfo = new PurchaseInfo();
			purchaseInfo.orderId = "123" + productID ;
			purchaseInfo.productId = productID;
			purchaseInfo.purchasePayload = "";
			purchaseInfo.purchaseState = purchasestate;
			purchaseInfo.purchaseTime=12321312312312;
			unitygameobjectreceiver.OnPurchaseResponse(JsonMapper.ToJson(purchaseInfo));
#elif UNITY_ANDROID
      		Debug.Log("ANDROID - PurchaseItem");
			AndroidJavaClass jc = new AndroidJavaClass("com.platoevolved.inappbilling.UnityAndroidInterface");
			jc.CallStatic("PurchaseItem",new string[] {productID});	
#endif
    }
	
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		    Application.Quit();		
	}
	
	
	
	void BeginPage(float width, float height)
    {
        GUILayout.BeginArea(new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height));
    }

    void EndPage()
    {
        GUILayout.EndArea();
    }

}
