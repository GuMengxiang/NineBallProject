
using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using LitJson;
using System;
using System.Linq;

public class AndroidPurchases: ISerializable { //This class, along with UnityGameObjectReceiver, handles all In App Purchasing for Android
	//These top two lines are the very least you will need to edit to get this plugin demo to work for you!
    #region Fields
    private static string publickey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAhEzG31M/l1MLYk8ky7F5TceykHBGn7RIrh+aV3Z/lpLVaS1896SxEA8NCRezWWsK2xVb1yNNPDS4iZEZ5YJTsEKV3rONA/WReGv0PHoswn1zDVGi9R1f39DOm3+BFF6EFHKp4lnVWNghwWAqR0qK2Uxji75hPBvlafQSj4Ta+2uwQsBUAIkc0BQM/NFRZV2j++DQi6zWGB6aPEuilFfzHfIOlBrEdGzkyrkn9bI7I4haD1Fubif+QWjch6djbaFusf9aZjHUUYigALOe4D6nIcyug1FVVQt5aYU7ht+i8dhMaUsZtDWtY7JhNy6wN/pgR3jXHFg+teDK3tRPS3fSywIDAQAB"; //Find this at the Google Play Android Developer Console -> Edit Profile
    private static string[] purchasableitems = { "chippackage6", "chippackage3", "chippackage5", "chippackage2", "chippackage4", "chippackage1", "dailydeal1", "dailydeal2" }; //Items available for sale, array of your ProductID's
	private static string purchasefilename = "AndroidPurchases.PUR";
    private static AndroidPurchases instance;
	private List<PurchaseInfo> _purchaseinfos; // List of products that have been purchased, after app is reinstalled, run RestoreTransactions to repopulate via UnityGameObjectReceiver->OnPurchaseResponse
	public static bool started = false;
	private static UnityGameObjectReceiverIAB unitygameobjectreceiver;
	private static string classname = "com.platoevolved.inappbillingunity.UnityAndroidInterface";
	public static bool iabsetup = false; // If this is true, in app purchasing is initialised, doesn't necessarily mean there is an internet connection

	// Example of virtual currency, note these will be lost if the user reinstalls the game
	private int _goldcoins;
	public int GoldCoins{get {return _goldcoins;} set {_goldcoins = value;}}

    // really the only thing useful...
    public static string PackageName = "";

	#endregion	
	#region Properties
	public static AndroidPurchases Instance
	{
        get
        {
            if (instance == null)
            {
                	instance = new AndroidPurchases();
            }
            return instance;
        }
	}
	
	public static string[] PurchasableItems{
		get{return purchasableitems;}
	}
	
	public List<PurchaseInfo> PurchaseInfos
	{
		get{ return _purchaseinfos;}
		set{_purchaseinfos = value;}
	}
	
	#endregion
	#region Functions
    public AndroidPurchases(){
		_purchaseinfos = new List<PurchaseInfo>();
		if(GameObject.Find("UnityGameObjectReceiver"))
			unitygameobjectreceiver = GameObject.Find("UnityGameObjectReceiver").GetComponent<UnityGameObjectReceiverIAB>();		
		started = false;
	}

	public static void UpdatePurchase(PurchaseInfo purchaseInfo)
    {
        if (purchaseInfo == null)
        {
            return;
        }

        if(purchaseInfo.purchaseState == "PURCHASED")
        {
            AndroidPurchases.PackageName = purchaseInfo.productId;

            Debug.Log("OrderID:" + purchaseInfo.orderId + " productId:" + purchaseInfo.productId + " purchaseState:" + purchaseInfo.purchaseState);

            Consume(purchaseInfo.productId);

            return;
        }
    }
	
	public static bool HasPurchased(){
		return (Instance.PurchaseInfos.Count != 0);
	}
	
	public static bool HasPurchased(string productid){
		foreach(PurchaseInfo pinfo in Instance.PurchaseInfos){
			if(pinfo.productId == productid) 
				return true;
		}
		return false;
	}
	
    public static void Load(){ //Load Purchases from file, if no file exists, restore transactions from app store
        string filename = Application.persistentDataPath + @"/"  + purchasefilename;
		Debug.Log("Loading Purchases from: " + filename);
        instance = DeSerializObject<AndroidPurchases>(filename);
		if(instance == null)
			RestoreTransactions();
		foreach(PurchaseInfo purchaseinfo in Instance.PurchaseInfos){
			Debug.Log("Purchased: " + purchaseinfo.productId);
		}
    }

    // whatever, this is bullshit anyways
    public static void Save()
    {
		Debug.Log("Saving Purchases.");
        string filename = Application.persistentDataPath + @"/" + purchasefilename;
        Debug.Log(filename);
        SerializeObject(filename, Instance);
    }

    public static void SerializeObject<T>(string filename, T data){
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        TextWriter textWriter = new StreamWriter(filename);
        serializer.Serialize(textWriter, data);
        textWriter.Close();
    }
    public static T DeSerializObject<T>(string filename){
        try
        {
            using (Stream stream = new FileStream(filename, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(T));
                T container = (T)serializer.Deserialize(stream);
                stream.Close();
                return container;
            }
        }
        catch
        {
            return default(T);
        }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
    {
    }
	
	public static void StartInAppBilling()
    {
		if(!started)
        {
#if UNITY_EDITOR 
			Load();//Just added so that demo works similar in editor as deployed, when deployed Load called on successful iabsetup
#endif

			started = true;
      		Debug.Log("StartInAppBilling");

			try
            {
#if UNITY_ANDROID
                AndroidJavaClass jc = new AndroidJavaClass(classname);
				jc.CallStatic("StartInAppBilling",publickey);
#endif
            }
            catch (Exception ex)
            {
				Debug.Log("Note, you must be deployed to an Android device! " + ex.Message);
			}
		}
	}

	public static void StopInAppBilling(){
		if(started){
			started = false;
      		Debug.Log("StopInAppBilling");
			try
            {
#if UNITY_ANDROID
                AndroidJavaClass jc = new AndroidJavaClass(classname);
				jc.CallStatic("StopInAppBilling");
#endif
            } catch (Exception ex) {
				Debug.Log(ex.Message);
			}
		}
	}
	
	public static void PurchaseItem(string productID)
	{
		if(!iabsetup) return;

#if UNITY_EDITOR 
			string purchasestate = ""; // Note that this block of code is here just so that something seems to happen in the Editor
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
			
			PurchaseInfo purchaseInfo = new PurchaseInfo();
			purchaseInfo.orderId = "123" + productID ;
			purchaseInfo.productId = productID;
			purchaseInfo.purchasePayload = "";
			purchaseInfo.purchaseState = purchasestate;
			purchaseInfo.purchaseTime=12321312312312;
			unitygameobjectreceiver.OnPurchaseResponse(JsonMapper.ToJson(purchaseInfo)); //You don't call this normally, only done here to emulate a callback
#elif UNITY_ANDROID
      		Debug.Log("ANDROID - PurchaseItem");
			AndroidJavaClass jc = new AndroidJavaClass(classname);
			jc.CallStatic("PurchaseItem",new string[] {productID});	
#endif
    }

	public static void RestoreTransactions()
	{
		if(!iabsetup) return;
#if UNITY_EDITOR
      		Debug.Log("Unity Editor - RestoreTransactions"); // Note that this block of code is here just so that something seems to happen in the Editor
			PurchaseInfo purchaseInfo = new PurchaseInfo();
			purchaseInfo.orderId = "123sword001";
			purchaseInfo.productId = "sword001";
			purchaseInfo.purchaseState = "PURCHASED";
			UpdatePurchase(purchaseInfo);
#elif UNITY_ANDROID
      		Debug.Log("ANDROID - RestoreTransactions");
			AndroidJavaClass jc = new AndroidJavaClass(classname);
			jc.CallStatic("RestoreTransactions");	
#endif
    }

    //IAB v3 way of handling unmanaged items, should be called on Purchase callback (UpdatePurchase)
    public static void Consume(string productID)
    {
#if UNITY_EDITOR // this is whatever
		UnityGameObjectReceiverIAB	 unitygameobjectreceiver = GameObject.Find("UnityGameObjectReceiver").GetComponent<UnityGameObjectReceiverIAB>();
		PurchaseInfo purchaseInfo = new PurchaseInfo();
		purchaseInfo.orderId = "123" + productID ;
		purchaseInfo.productId = productID;
		purchaseInfo.purchasePayload = "";
		purchaseInfo.purchaseState = "PURCHASED";
		purchaseInfo.purchaseTime=12321312312312;
		unitygameobjectreceiver.OnConsumeFinished(JsonMapper.ToJson(purchaseInfo));
#elif UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass(classname);
		jc.CallStatic("Consume", productID);
#endif
    }

    //Essentially applies a consume so should be called in OnConsumeFinished
	public static void ProvisionPurchase(PurchaseInfo purchaseInfo)
    {
        if (purchaseInfo == null)
        {
            return;
        }

		Debug.Log("Provision-OrderID:" + purchaseInfo.orderId + " productId:" + purchaseInfo.productId + " purchaseState:" + purchaseInfo.purchaseState);

        if (purchaseInfo.purchaseState == "PURCHASED")
        {
            // do some crazy stuff, i deleted these. if i want i can go look for it in a older version

            // whatever

            Save();
        }
	}
	
	public static string LogInfo()
	{
		return unitygameobjectreceiver.logInfo;
	}
	
	#endregion
}
