using UnityEngine;
using System.Collections;
using System;

/**
 * Implementation of the Handler used for AdToServe
 */

//AdFinishedEventArgs类由以下参数组成。
public class AdFinishedEventArgs_OPen : EventArgs
{
	//如果用户点击下载按钮去存储，则为true
	public bool WasCallToActionClicked;
	
	//如果至少有80％的视频被观看，则为true
	public bool IsCompletedView;

	//持续的Vungle广告观看
	public double TimeWatched;
	
	// Vungle广告的总持续时间
	public double TotalDuration;

}
//		GameManager_script.Instance().CoinCount  
public class AddVungleAdvertising : MonoBehaviour {
	//当Vungle广告准备好显示时触发
	public static event Action <bool> adPlayableEvent;
	//当Vungle广告开始时发生
	public static event Action onAdStartedEvent;
	//Vungle 广告完成并提供有关该事件的全部信息时触发
	public static event Action <AdFinishedEventArgs_OPen> OnAdFinishedEvent;
	//当SDK发送日志事件时触发
	public static event Action <string> onLogEvent;
	// Use this for initialization

	public bool IsWatchthead=false;
	public UILabel isAdvert;
	private float TextChange = 0.0f;


//	WHandlerImpl handlerImpl;
	void Awake()
	{
		//Vungle.init ("59263dd625126db838000fd3", "Test_iOS", "Test_Windows");
		isAdvert.text  = "Get Free Coins";
	}
	void Start () {

		RegistrationEvent();

	}
	
	// Update is called once per frame
	void Update () {
		if (isAdvert.text != "Get Free Coins")
		{
//			GameManager_script.Instance().UpdateCoinCount(100.0f);
			TextChange -= Time.deltaTime;
			if (TextChange <= 0) {
				TextChange = 3.0f;
				isAdvert.text = "Get Free Coins";
			}

		}
	}
	public void OnClick()
	{
		//		GameManager_script.Instance().CurrentLevel += 5.0f;
//				GameManager_script.Instance().UpdateCoinCount(100000.0f);

		//Voice
		GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);

		if (Vungle.isAdvertAvailable () /*|| mValue != 1*/) {//当Vungle广告准备完成或者随机数为不1
			Vungle.playAd ();
			isAdvert.text = "Watch the ad";
		} else {
			isAdvert.text = "not ready yet";
		}
	}
	/// <summary>
	/// 添加onPause和onResume功能的代码，使得当应用程序被背景恢复播放时已暂停的广告
	/// </summary>
	/// <param name="pauseStatus">If set to <c>true</c> pause status.</param>
	void OnApplicationPause(bool pauseStatus) {
		if (pauseStatus) {
			Vungle.onPause();
		}
		else {
			Vungle.onResume();
		}
	}
	void RegistrationEvent()
	{
		//Event is triggered when the ad's playable state has been changed
		//It can be used to enable certain functionality only accessible when ad plays are available
		Vungle.adPlayableEvent += (adPlayable) => {
			Debug.Log ("Ad's playable state has been changed! Now: " + adPlayable);
		};
		Vungle.onAdStartedEvent += () => {

		};
		Vungle.onAdFinishedEvent += (args)=> {
			if (args.IsCompletedView) {
				// old money here
				float oldMoney = GameManager_script.Instance().CoinCount;

				// update coin count
				GameManager_script.Instance().UpdateCoinCount(100.0f);

				// broadcast them coinz
				GameManager_script.Instance().UpdateWindowMoney(oldMoney);
			}
		};
		
		//Fired log event from sdk
		Vungle.onLogEvent += (log) => {
			Debug.Log ("Log: " + log);
		};
	}
}
