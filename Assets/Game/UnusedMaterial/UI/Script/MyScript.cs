using UnityEngine;
using System.Collections;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class MyScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log ("进入firebase");
		// Set this before calling into the realtime databass;
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://ball-pool-867282.firebaseio.com/");
		#if UNITY_EDITOR 
		ServiceAccount ();
		#endif 
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	void ServiceAccount()
	{
		//Set these values before before calling into the realtime database.
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://ball-pool-867282.firebaseio.com/");
		FirebaseApp.DefaultInstance.SetEditorP12FileName ("9 Ball Pool-b7f7fb68e015.p12");
		FirebaseApp.DefaultInstance.SetEditorServiceAccountEmail ("unitytest@ball-pool-867282.iam.gserviceaccount.com");
		FirebaseApp.DefaultInstance.SetEditorP12Password ("notasecret"); 
		Debug.Log ("进入firebase_unity");
	}


}
