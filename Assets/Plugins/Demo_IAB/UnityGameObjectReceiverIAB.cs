using UnityEngine;
using System.Collections;
using LitJson;
using System;

public class UnityGameObjectReceiverIAB : MonoBehaviour { //You must have a GameObject in your scene called UnityGameObjectReceiver that this is attached to (like the Demo)
	private string _log;

	public string logInfo
    {
        get { return _log; }
    }

	public void OnBillingSupported(string message){
		BillingLog("OnBillingSupported: " + message); 
	}

	public void OnPurchaseStateChange(string message){
		BillingLog("OnPurchaseStateChange: " + message); 
	}

	public void OnRequestPurchaseResponse(string message){
		BillingLog("OnRequestPurchaseResponse: " + message); 
	}

	public void OnRestoreTransactionsResponse(string message){
		BillingLog("OnRestoreTransactionsResponse: " + message); 
	}

	public void OnPurchaseResponse(string message){ // The only important method, receives all the purchase information back from the Android plugin (you don't call this yourself)
		BillingLog("OnPurchaseResponse: " + message); 
		PurchaseInfo purchaseInfo = JsonMapper.ToObject<PurchaseInfo>(message);
		AndroidPurchases.UpdatePurchase(purchaseInfo);
	}

	public void OnPurchaseError(string message){
		BillingLog("OnPurchaseError: " + message); 
	}

	public void OnError(string message){
		BillingLog("OnError: " + message); 
	}

	//Android Google IAB v3
	public void OnConsumeFinished(string message)
    {
		BillingLog("OnConsumeFinished: " + message); 
		PurchaseInfo purchaseInfo = JsonMapper.ToObject<PurchaseInfo>(message);
		AndroidPurchases.ProvisionPurchase(purchaseInfo);
	}

    public void OnIabSetupFinished(string message) // Receieved when iab is ready to go
    {
		BillingLog("OnIabSetupFinished: " + message); 

		AndroidPurchases.iabsetup = true;

		AndroidPurchases.Load();
	}

	public void OnIabSetupError(string message)
    {
		BillingLog("OnIabSetupError: " + message); 

		AndroidPurchases.iabsetup = false;
		AndroidPurchases.started = false;

        AndroidPurchases.StartInAppBilling();
    }

	void BillingLog(string logInfo){
		_log = logInfo + Environment.NewLine + _log;
	}
	
}
