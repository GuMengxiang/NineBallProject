using UnityEngine;
using System.Collections;

public class GameRoot : MonoBehaviour {
	public GameObject LandingWindow;
	UIPanel rootpanel;
	float money=0.0f;
	// Use this for initialization
	void Start () {
        GameManager_script.Instance().gameRoot = this;
		rootpanel=this.gameObject.GetComponent<UIPanel>();
		money= PlayerPrefs.GetFloat("Coin");
	}
		
	public void OnClick()
	{

		GameObject mLandwin = (GameObject)Instantiate (LandingWindow);
		mLandwin.transform.parent = this.transform.Find ("UICamera");
		mLandwin.transform.localPosition = Vector3.zero; // final destination
		mLandwin.GetComponent<LoginAndRegister>().adaptscreen(rootpanel);
		//mLandwin.transform.localScale = new Vector3(1, 1, 1);

	}
}
