using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Firebase.Auth;

public class LoginAndRegister : MonoBehaviour {

	public UILabel mTitle;//登录界面的title
	public UIInput mUserInput;//登录或者注册的输入框
	public UIInput mPassInput;//密码的输入框
	public UIButton Yesbutt;//确定按钮
	public UIButton Nobutton;//取消按钮
	public UILabel PopupsLabel;//弹窗里的文本
	public GameObject PopupWindow;//弹窗
	public UILabel LorR;//Landing or Registered //登陆或者注册
	public GameObject Precautions;//玩家注意事项,toggle

	FirebaseAuth auth;
	FirebaseUser user;
	public string mEmeil;
	public string newPassword;
	public GameObject mRegistered;//登录和注册的窗口
	public UISprite Background;//遮罩
	Savethedatabase thisdata;
	int mType=4;

	// Use this for initialization
	void Start () {
	
		mTitle.text = "Login";
		LorR.text = "Registered>>";
		InitializeFirebase ();

		thisdata = new Savethedatabase ();

	}

	// Update is called once per frame
	void Update () {
		
	}

//	#region  单例模式
//	private static LoginAndRegister instance;
//
//	private LoginAndRegister()
//	{
//
//	}
//
//	public static LoginAndRegister GetInstance()
//	{
//		if (instance==null) {
//			instance = new LoginAndRegister ();
//		}
//		return instance;
//	}
//	#endregion
	public void adaptscreen(UIPanel RootPanel)
	{
	this.gameObject.transform.localScale = new Vector3 (2f,2f,2f);

		mRegistered.GetComponent<UISprite> ().width = (int)(RootPanel.width)/2;
		mRegistered.GetComponent<UISprite> ().height = (int)(RootPanel.height)/2;
		PopupWindow.GetComponent<UISprite> ().width = (int)(RootPanel.width)/2;
		PopupWindow.GetComponent<UISprite> ().height = (int)(RootPanel.height)/2;

		// 下方的两个按钮 yes and no 
		Yesbutt.gameObject.GetComponent<UISprite> ().width = mRegistered.GetComponent<UISprite> ().width / 5;
		Yesbutt.gameObject.GetComponent<UISprite> ().height = Yesbutt.gameObject.GetComponent<UISprite> ().width / 3;
		Yesbutt.gameObject.transform.localPosition = new Vector3 (-Yesbutt.gameObject.GetComponent<UISprite> ().width,-mRegistered.GetComponent<UISprite>().height/3,0);
		Nobutton.gameObject.GetComponent<UISprite> ().width = mRegistered.GetComponent<UISprite> ().width / 5;
		Nobutton.gameObject.GetComponent<UISprite> ().height = Nobutton.gameObject.GetComponent<UISprite> ().width / 3;
		Nobutton.gameObject.transform.localPosition = new Vector3 (Nobutton.gameObject.GetComponent<UISprite> ().width,-mRegistered.GetComponent<UISprite>().height/3,0);

		//登陆和注册的lable
		mRegistered.transform.Find ("UserEmail/email").localScale = new Vector3 (1.0f, 1.0f, 0f);
		mRegistered.transform.Find ("UserPassWord/pass").localScale = new Vector3 (1.0f, 1.0f, 0f);
		mRegistered.transform.Find ("UserEmail/email/Sprite").localScale = new Vector3 (1.0f, 1.0f, 0f);
		mRegistered.transform.Find ("UserPassWord/pass/Sprite").localScale = new Vector3 (1.0f, 1.0f, 0f);

		mRegistered.transform.Find ("UserEmail/email").GetComponent<UILabel>().width = mRegistered.GetComponent<UISprite> ().width / 4;
		mRegistered.transform.Find ("UserEmail/email").GetComponent<UILabel> ().height = mRegistered.transform.Find ("UserEmail/email").GetComponent<UILabel> ().width / 3;
		mRegistered.transform.Find ("UserEmail/email").localPosition = new Vector3 (- mRegistered.GetComponent<UISprite> ().width / 3+30,mRegistered.GetComponent<UISprite>().height/6,0);
		mRegistered.transform.Find ("UserEmail/email").GetComponent<UILabel> ().fontSize = mRegistered.transform.Find ("UserEmail/email").GetComponent<UILabel> ().height-5;

		mRegistered.transform.Find ("UserPassWord/pass").GetComponent<UILabel> ().width = mRegistered.GetComponent<UISprite> ().width / 4;
		mRegistered.transform.Find ("UserPassWord/pass").GetComponent<UILabel> ().height = mRegistered.transform.Find ("UserPassWord/pass").GetComponent<UILabel> ().width / 3;
		mRegistered.transform.Find ("UserPassWord/pass").localPosition = new Vector3 (-mRegistered.GetComponent<UISprite> ().width / 3,-mRegistered.GetComponent<UISprite>().height/18,0);
		mRegistered.transform.Find ("UserPassWord/pass").GetComponent<UILabel> ().fontSize = mRegistered.transform.Find ("UserPassWord/pass").GetComponent<UILabel> ().height-5;
		//输入框
		mRegistered.transform.Find ("UserEmail/email/Sprite").GetComponent<UISprite>().width = mRegistered.GetComponent<UISprite> ().width / 2;
		mRegistered.transform.Find ("UserEmail/email/Sprite").GetComponent<UISprite> ().height = mRegistered.transform.Find ("UserEmail/email").GetComponent<UILabel> ().height;
		mRegistered.transform.Find ("UserEmail/email/Sprite").localPosition = new Vector3 (mRegistered.transform.Find ("UserEmail/email/Sprite").GetComponent<UISprite> ().height*5-30,0,0);

		mRegistered.transform.Find ("UserPassWord/pass/Sprite").GetComponent<UISprite> ().width = mRegistered.GetComponent<UISprite> ().width / 2;
		mRegistered.transform.Find ("UserPassWord/pass/Sprite").GetComponent<UISprite> ().height = mRegistered.transform.Find ("UserPassWord/pass").GetComponent<UILabel> ().height;
		mRegistered.transform.Find ("UserPassWord/pass/Sprite").localPosition = new Vector3 (mRegistered.transform.Find ("UserPassWord/pass/Sprite").GetComponent<UISprite> ().height*5,0,0);

		//标题
		mTitle.width = mRegistered.GetComponent<UISprite> ().width / 4;
		mTitle.height = mTitle.width / 3;
		mTitle.transform.localPosition = new Vector3 (0,mTitle.height *2 + 10,0);
		mTitle.fontSize = mTitle.height-5;

		//LorR
		LorR.width = mRegistered.GetComponent<UISprite> ().width / 4;
		LorR.height = LorR.width / 3;
		LorR.fontSize = LorR.height-5;
		LorR.transform.localPosition = new Vector3 (mRegistered.GetComponent<UISprite>().width/3,LorR.height*2 + 20,0);

		//toggle
		Precautions.transform.localPosition=new Vector3(mRegistered.GetComponent<UISprite>().width/14,-mRegistered.GetComponent<UISprite>().height/5,0);
		Precautions.GetComponent<UILabel> ().width =mRegistered.GetComponent<UISprite> ().width / 2;
		Precautions.GetComponent<UILabel> ().height = mRegistered.GetComponent<UISprite> ().width / 20;

		Precautions.transform.Find ("flaseCheck").localPosition = new Vector3 (-mRegistered.GetComponent<UISprite> ().width/3+8,0,0);
		Precautions.transform.Find ("flaseCheck").GetComponent<UISprite> ().width = Precautions.GetComponent<UILabel> ().width / 8;
		Precautions.transform.Find ("flaseCheck").GetComponent<UISprite> ().height = Precautions.transform.Find ("flaseCheck").GetComponent<UISprite> ().width / 2 - 2;

		Precautions.transform.Find ("flaseCheck/trueCheck").localPosition = new Vector3 (0,0,0);
		Precautions.transform.Find ("flaseCheck/trueCheck").GetComponent<UISprite> ().width = Precautions.transform.Find ("flaseCheck").GetComponent<UISprite> ().width;
		Precautions.transform.Find ("flaseCheck/trueCheck").GetComponent<UISprite> ().height = Precautions.transform.Find ("flaseCheck").GetComponent<UISprite> ().height;

		Precautions.GetComponent<UILabel> ().text = Localization.Get ("Log Note");
		//遮挡图 ok
		Background.transform.localPosition = new Vector3(0, 0, 0);
		Background.height = (int)(RootPanel.height);
		Background.width = (int)(RootPanel.width);

		//背景框
		Background.GetComponent<BoxCollider>().size = new Vector3(mRegistered.GetComponent<UISprite >().width * 2f, mRegistered.GetComponent<UISprite >().height * 2f, 0);

		//弹窗
		PopupWindow.GetComponent<UISprite>().width = mRegistered.GetComponent<UISprite>().width/2+30;
		PopupWindow.GetComponent<UISprite> ().height = PopupWindow.GetComponent<UISprite> ().height/2+30;

		PopupsLabel.GetComponent<UILabel> ().width = PopupWindow.GetComponent<UISprite> ().width - (PopupWindow.GetComponent<UISprite> ().width / 8);
		PopupsLabel.GetComponent<UILabel> ().height = PopupWindow.GetComponent<UISprite> ().height / 2;
		PopupsLabel.gameObject.transform.localPosition = new Vector3 (0,10,0);

		PopupWindow.transform.Find ("YesButton").gameObject.GetComponent<UISprite> ().width = PopupWindow.GetComponent<UISprite> ().width / 3;
		PopupWindow.transform.Find ("YesButton").gameObject.GetComponent<UISprite> ().height = PopupWindow.GetComponent<UISprite> ().height  / 6;
		PopupWindow.transform.Find("YesButton").localPosition = new Vector3 (0,-PopupWindow.GetComponent<UISprite>().height/3,0);

		PopupWindow.transform.Find ("close").gameObject.GetComponent<UISprite> ().width = PopupWindow.GetComponent<UISprite> ().width / 10;
		PopupWindow.transform.Find ("close").gameObject.GetComponent<UISprite> ().height = PopupWindow.transform.Find ("close").gameObject.GetComponent<UISprite> ().width;
		int a = PopupWindow.transform.Find ("close").gameObject.GetComponent<UISprite> ().width;
		PopupWindow.transform.Find ("close").localPosition = new Vector3 (a*5-(a /2)+4,PopupWindow.GetComponent<UISprite> ().height/2-(a/2)+2,0);

	}

	// Handle initialization of the necessary firebase modules:
	//处理必要的firebase模块的初始化：
public void InitializeFirebase() {
		Debug.Log("Setting up Firebase Auth");
		auth = FirebaseAuth.DefaultInstance;
		auth.StateChanged += AuthStateChanged;
		AuthStateChanged(this, null);
	}

	// Track state changes of the auth object.
	//跟踪auth对象的状态更改。
	void AuthStateChanged(object sender, System.EventArgs eventArgs) {
		if (auth.CurrentUser != user) {
			bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
			if (!signedIn && user != null) {
				Debug.Log("Signed out " + user.UserId);
			}
			user = auth.CurrentUser;
			if (signedIn) {
				Debug.Log("Signed in " + user.UserId);
			}
		}
	}
	void OnDestroy() {
		auth.StateChanged -= AuthStateChanged;
		auth = null;
	}
	/// <summary>
	/// Gets the user profile.获取用户个人资料
	/// </summary>
	public void GetUserProfile()
	{
		Firebase.Auth.FirebaseUser user = auth.CurrentUser;
		// user = auth.CurrentUser;
		if (user != null) {
			string name = user.DisplayName;
			string email = user.Email;
			Uri photo_url = user.PhotoUrl;
			// The user's Id, unique to the Firebase project.
			// Do NOT use this value to authenticate with your backend server, if you
			// have one; use User.TokenAsync() instead.
			string uid = user.UserId;
			thisdata.ReadDataOnce ();
			//GetComponent<Savethedatabase> ().ReadDataOnce (uid);
			Debug.Log ("9ball"+"Getuser"+"uid: "+uid);
		}
	}
	/// <summary>
	/// Sets the updata email.设置用户邮箱
	/// </summary>
	void SetUpdataEmail()
	{
		Firebase.Auth.FirebaseUser user = auth.CurrentUser;
		// user = auth.CurrentUser;
		mEmeil = mUserInput.label.text;
		if (user != null) {
			user.UpdateEmailAsync(mEmeil).ContinueWith(task => {
				if (task.IsCanceled) {
					Debug.LogError("UpdateEmailAsync was canceled.");
					return;
				}
				if (task.IsFaulted) {
					Debug.LogError("UpdateEmailAsync encountered an error: " + task.Exception);
					return;
				}

				Debug.Log("User email updated successfully.");
			});
		}
	}
	/// <summary>
	/// Sets the updata pass.设置用户密码
	/// </summary>
	void SetUpdataPass()
	{
		//Firebase.Auth.FirebaseUser user = auth.CurrentUser;
		user = auth.CurrentUser;
		newPassword = mPassInput.label.text;
		if (user != null) {
			user.UpdatePasswordAsync(newPassword).ContinueWith(task => {
				if (task.IsCanceled) {
					Debug.LogError("UpdatePasswordAsync was canceled.");
					return;
				}
				if (task.IsFaulted) {
					Debug.LogError("UpdatePasswordAsync encountered an error: " + task.Exception);
					return;
				}

				Debug.Log("Password updated successfully.");
			});
		}
	}
	/// <summary>
	/// Changes the lable.改变登陆窗口的mTitle
	/// </summary>
	public void ChangeLable()
	{
		switch (LorR.text) {
		case "Registered>>":
			mTitle.text = "Registered";
			LorR.text = "Login>>";
			Precautions.GetComponent<UILabel> ().text = Localization.Get ("registration attention");
			break;
		case "Login>>":
			mTitle.text = "Login";
			LorR.text="Registered>>";
			Precautions.GetComponent<UILabel> ().text = Localization.Get ("Log Note");
			break;
		default:
			break;
		}
	}
	/// <summary>
	/// Raises the click event. 点击确定按钮，弹出窗口进行再次确认
	/// </summary>
	public void OnClick()
	{
		FirebaseUser newUser;
		if (Precautions.transform.Find("flaseCheck").GetComponent<UIToggle>().value) {
			switch (mTitle.text) {
			case "Login":
				mEmeil = mUserInput.label.text;
				newPassword = mPassInput.label.text;
				auth.SignInWithEmailAndPasswordAsync(mEmeil,newPassword).ContinueWith(task1 => {
					if (task1.IsCanceled) {
						Debug.Log("9Ball"+"LandingSuccessfully"+"SignInWithEmailAndPasswordAsync was canceled.");

						return;
					}
					if (task1.IsFaulted) {
						Debug.Log("9Ball"+"SignInWithEmailAndPasswordAsync encountered an error: " + task1.Exception);
						mRegistered.SetActive(false);
						PopupWindow.SetActive(true);
						PopupsLabel.text=Localization.Get("Login failed");
						mType= (int)ConfirmType.CaNotLogin;
						return;
					}

					newUser = task1.Result;
					Debug.Log("9ballLand "+"uid: "+newUser.UserId);
					//GetComponent<Savethedatabase>().ReadDataOnce(newUser.UserId);
					Debug.Log("9Ball"+"User Landing in successfully: "+"newUser.Email: "+newUser.Email +"newUser.UserId: " +newUser.UserId);
					EIDP();
					PlayerPrefs.SetString("_USERID",newUser.UserId);//将用户的userID存入本地
					PlayerPrefs.SetString("_EMAIL",newUser.Email);//将用户的Email存入本地
					PlayerPrefs.SetString("_PASSAGAIN",newPassword );//将用户的邮箱密码存入本地
					GetUserProfile();
					mType= (int)ConfirmType.StartSign;
					Confirmlanding();

				});
				break;
			case "Registered":
				SetUpdataEmail ();
				SetUpdataPass ();
				if (mUserInput.label.text.Contains ("@")) {
					if (mPassInput.label.text.Length>6 && mPassInput.label.text.Length<18) {
						PopupWindow.SetActive (true);
						mRegistered.SetActive (false);
						PopupsLabel.text = Localization.Get ("registration success") + mEmeil + " ?";
						mType = (int)ConfirmType.ConfimationInfo;
					} else {
						PopupWindow.SetActive (true);
						mRegistered.SetActive (false);
						PopupsLabel.text = Localization.Get ("wrong password");
						mType = (int)ConfirmType.WrongPass;
					}
				} else {//if(mUserInput.label.text.Contains ("/")||mUserInput.label.text.Contains (" ")||mUserInput.label.text.Contains ("$")){
					PopupWindow.SetActive (true);
					mRegistered.SetActive (false);
					PopupsLabel.text = Localization.Get ("registration failed");
					mType = (int)ConfirmType.CaNotRegis;
				}
				break;
			default:
				PopupWindow.SetActive (true);
				mRegistered.SetActive (false);
				PopupsLabel.text = Localization.Get ("registration failed");
				mType = (int)ConfirmType.CaNotRegis;
				break;
			}
		}
		else {
			Precautions.transform.Find("flaseCheck").gameObject.GetComponent<TweenScale> ().enabled = true;
		}

		GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
	}
	/// <summary>
	/// Confirmlanding this instance.确认窗口处理
	/// </summary>

	public void Confirmlanding()
	{
//		PopupWindow.SetActive (true);
//		mRegistered.SetActive (false);
		FirebaseUser newUser;
		switch (mType) {
		case 0:
			auth.CreateUserWithEmailAndPasswordAsync (mEmeil,newPassword ).ContinueWith (task =>{
				if (task.IsCanceled) {
					Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled");
					return;
				}
				if (task.IsFaulted) {
					Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: "+task.Exception );
					PopupsLabel.text = Localization.Get ("registration failed");
					mType = (int)ConfirmType.CaNotRegis;
					return;
				}

				auth.SignInWithEmailAndPasswordAsync(mEmeil, newPassword).ContinueWith(task1 => {
					if (task.IsCanceled) {
						Debug.Log("9Ball"+"SignInWithEmailAndPasswordAsync was canceled.");
						return;
					}
					if (task.IsFaulted) {
						Debug.Log("9Ball"+"SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
						return;
					}
					newUser = task.Result;
					Debug.Log("9Ball"+"User signed in successfully: "+"newUser.Email: "+newUser.Email +"newUser.UserId: " +newUser.UserId);
					EIDP();
					PlayerPrefs.SetString("_USERID",newUser.UserId);//将用户的userID存入本地
					PlayerPrefs.SetString("_EMAIL",newUser.Email);//将用户的Email存入本地
					PlayerPrefs.SetString("_PASSAGAIN",newPassword );//将用户的邮箱密码存入本地
					this.gameObject.SetActive (false);
				});
			});
			break;
		case 1:
		case 2:
		case 3:
		case 5:
			PopupWindow.SetActive (false);
			mRegistered.SetActive (true);
			mPassInput.value = null;
			break;
		case 4:
			this.gameObject.SetActive (false);
			break;
		default:
			break;
		}
		GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
	}
/// <summary>
/// Dis this instance.将确认登陆的弹窗激死
/// </summary>
	public void dis()
	{
		PopupWindow.SetActive (false);
		mRegistered.SetActive (true);
		mPassInput.value = null;
		GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
	}
/// <summary>
/// Exits the game.清除窗口
/// </summary>
	public void ExitGame()
	{
		Destroy (this.gameObject);
		GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
	}
	/// <summary>
	/// Clicks the toggle tween.注意事项选项的toggle
	/// </summary>
	public void ClickToggleTween()
	{
		if (Precautions.transform.Find("flaseCheck").GetComponent<UIToggle>().value) {
			Precautions.transform.Find("flaseCheck").gameObject.GetComponent<TweenScale> ().enabled = false;
		}
	}

	public enum ConfirmType
	{
		/// <summary>
		/// The confimation info.确认账号的信息，你确定要注册这个账号吗……这样的
		/// </summary>
		ConfimationInfo=0,
		/// <summary>
		/// The wrong pass.密码错误
		/// </summary>
		WrongPass,
		/// <summary>
		/// The ca not regis.因为一些原因无法被注册
		/// </summary>
		CaNotRegis,
		/// <summary>
		/// The ca not login.因为一些原因无法被登录
		/// </summary>
		CaNotLogin,
		/// <summary>
		/// The start sign.开始登录
		/// </summary>
		StartSign,
		/// <summary>
		/// The start regis.开始注册
		/// </summary>
		StartRegis
	}
	/// <summary>
	/// Email and UserID and Password. 用户的邮箱，userID和密码
	/// </summary>
	public void EIDP()
	{
		if (PlayerPrefs.HasKey("_USERID")) {
			PlayerPrefs.DeleteKey ("_USERID");
		}
		if (PlayerPrefs.HasKey("_EMAIL")) {
			PlayerPrefs.DeleteKey ("_EMAIL");
		}
		if (PlayerPrefs.HasKey("_PASSAGAIN")) {
			PlayerPrefs.DeleteKey ("_PASSAGAIN");
		}
	}
}