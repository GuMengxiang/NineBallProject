using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using Firebase.Auth;
public class Savethedatabase : MonoBehaviour {
	DatabaseReference reference;

	// Use this for initialization
	void Start () {
		// Set up the Editor before calling into the realtime database.
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://ball-pool-867282.firebaseio.com/");

		// Get the root reference location of the database.
		 reference = FirebaseDatabase.DefaultInstance.RootReference;
		//GetComponent<LoginAndRegister> ().newUser;
	}
	
	// Update is called once per frame
	void Update () {

	}
	#region 保存用户的数据库
	public void OnClick()
	{

		Debug.Log ("9Ball"+"OnClick");

		Debug.Log("9Ball"+" Uid:"+PlayerPrefs.GetString("_USERID")+" Name:"+"Mien "+" Email:"+PlayerPrefs.GetString("_EMAIL"));
		if (PlayerPrefs.HasKey("_USERID")) {
			writeNewUser (PlayerPrefs.GetString("_USERID"),PlayerPrefs.GetString("First_Name"),PlayerPrefs.GetString("_EMAIL"));
		}

	}

	public void writeNewUser(string userId, string name, string email) {
		Debug.Log ("9Ball"+"SuccessIn");
		Debug.Log ("9Ball"+" Uid:"+userId+" Name:"+name+" Email:"+email);
		User user = new User(email);
		string json = JsonUtility.ToJson(user);
		Debug.Log ("9Ball"+" Json:"+json+" Uid:"+userId+" Name:"+name+" Email:"+email);
		//if (userId==PlayerPrefs.GetString("_USERID")) {
			reference.Child(userId).SetRawJsonValueAsync(json);
		//}
	
		//	reference.Child (userId).Child ("userName").SetValueAsync (name);//在JSON下添加了一个叫“userName”的分支，并把name的值传进去

		//Debug.Log ("9Ball"+"数据库 Json:"+json+"数据库 Uid:"+userId+"数据库 Name:"+name+"数据库 Email:"+email);
		FirebaseDatabase.DefaultInstance.GoOnline();
	}

	public class LeaderboardEntry {
		public string uid;
		public int score = 0;

		public LeaderboardEntry() {
		}

		public LeaderboardEntry(string uid, int score) {
			this.uid = uid;
			this.score = score;
		}

		public Dictionary<string, object> ToDictionary() {
			Dictionary<string, object> result = new Dictionary<string, object>();
			result["uid"] = uid;
			result["score"] = score;

			return result;
		}
	}
	private void WriteNewScore(string userId, int score) {
		// Create new entry at /user-scores/$userid/$scoreid and at
		// /leaderboard/$scoreid simultaneously
		string key = reference.Child("scores").Push().Key;
		LeaderboardEntry entry = new LeaderboardEntry(userId, score);
		Dictionary<string, object> entryValues = entry.ToDictionary();

		Dictionary<string, object> childUpdates = new Dictionary<string, object>();
		childUpdates["/scores/" + key] = entryValues;
		childUpdates["/user-scores/" + userId + "/" + key] = entryValues;

		reference.UpdateChildrenAsync(childUpdates);
	}
	#endregion

	#region 检索玩家在数据库的信息并加载进游戏里
	/// <summary>
	/// Reads the data once.一次性加载当前节点上的所有数据
	/// </summary>
	public void ReadDataOnce()
	{
		Debug.Log ("9ballReadData"+" uid: "+PlayerPrefs.GetString("_USERID"));

	
		FirebaseDatabase.DefaultInstance
			.GetReference (PlayerPrefs.GetString("_USERID"))
			.GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				//handle the error
					Debug.Log("9ballError");
			}
			else if (task.IsCompleted) {
				DataSnapshot snapshot=task.Result;	
					Debug.Log("9ball Snapshot.key: "+snapshot.Key);
					string b=snapshot.Child("thisPlayerCoin").Value.ToString ();
					Debug.Log("9ball Snapshot.Value: "+b);
					//Debug.Log("9ball Snapshot.GetRawJsonValue(): "+snapshot.GetRawJsonValue().ToString());
					string a=snapshot.GetRawJsonValue();

					Debug.Log("9ballUserJson: "+ a);
					LoadData(a);
			}
		});
	}
	/// <summary>
	/// Loads the data.加载数据
	/// </summary>
	/// <param name="mjson">Mjson.</param>
	public void LoadData(string mjson)
	{
		User mUser = new User ();
		mUser = JsonUtility.FromJson<User>(mjson) as User; 
		JsonUtility.FromJsonOverwrite (mjson, mUser);
		Debug.Log ("9ballmUser"+" coin: "+mUser.thisPlayerCoin+" Name: "+mUser.thisPlayerName);
		float result;
		float mleve;
		float mexp;
		if (float.TryParse (mUser.thisPlayerCoin, out result)) 
		{
			float oldMoney =GameManager_script.Instance().CoinCount;
			float a = result - oldMoney;
			GameManager_script.Instance().UpdateCoinCount(a);
			Debug.Log ("9ballmUser"+" coin: "+result.ToString());
			PlayerPrefs.SetFloat ("Coin",result);
		}
		if (float.TryParse (mUser.thisPlayerLevel, out mleve))
		{
			GameManager_script.Instance ().CurrentLevel = mleve;
			PlayerPrefs.SetFloat ("CurrentLevel", mleve);
		}
		if (float.TryParse (mUser.thisPlayerExp, out mexp)) 
		{
			GameManager_script.Instance().NineBallExperience = mexp;
			PlayerPrefs.SetFloat ("NineBallExperience", mexp);
		}
		GameObject.Find("Gameroot/UICamera/Mainpanel/MainGroup/BottomRight/Profile/MaskProfile").GetComponent<OpenWindowUI>().OnClick();

		GameManager_script.Instance ().First_Name = mUser.thisPlayerName;
		PlayerPrefs.SetString("First_Name",  mUser.thisPlayerName);
		Debug.Log ("9ball"+" name: "+mUser.thisPlayerName);
		GameManager_script.Instance ().AvatarEquipped =int.Parse( mUser.thisPlayerAvatar);
		PlayerPrefs.SetInt ("AvatarEquipped", int.Parse (mUser.thisPlayerAvatar));
		GameManager_script.Instance ().CueEquipped = int.Parse (mUser.thisPlayerCue);
		PlayerPrefs.SetInt ("CueEquipped", int.Parse (mUser.thisPlayerCue));

		GameObject.Find ("Gameroot/UICamera/WindowPanel(Clone)/Center/BoxBackground/ProfilePanel(Clone)/UpProfileInsideWindow/UserName/InputLabel").GetComponent<UILabel> ().text = mUser.thisPlayerName;
		GameObject.Find ("Gameroot/UICamera/WindowPanel(Clone)/Center/BoxBackground/ProfilePanel(Clone)/UpProfileInsideWindow/Head/Background").GetComponent<UISprite> ().spriteName = mUser.thisPlayerAvatar;
		GameObject.Find ("Gameroot/UICamera/WindowPanel(Clone)/Center/Money/Label").GetComponent<UILabel> ().text = "$" + mUser.thisPlayerCoin;
		GameObject.Find ("Gameroot/UICamera/WindowPanel(Clone)/Center/BoxBackground/ProfilePanel(Clone)/UpProfileInsideWindow/Coincouts").GetComponent<UILabel> ().text = "$" + mUser.thisPlayerCoin;
	}
	#endregion
}

public class User {
	public string thisPlayerName;//玩家姓名
	public string thisPlayerEmail;//邮箱
	public string thisPlayerCue;//球杆
	public string thisPlayerCoin;//金币
	public string thisPlayerAvatar;//头像
	public string thisPlayerLevel;//等级
	public string thisPlayerExp;//经验值

//	public string pthisPlayerCue 
//	{
//		get{return thisPlayerCue;}
//		set{thisPlayerCue = PlayerPrefs.GetInt ("CueEquipped").ToString();}
//	}
	public User() {
	}

	public User(string email) {
		this.thisPlayerName = PlayerPrefs.GetString("First_Name");
		this.thisPlayerEmail = email;
		thisPlayerCue = PlayerPrefs.GetInt ("CueEquipped").ToString();
		thisPlayerCoin = PlayerPrefs.GetFloat ("Coin").ToString();
		thisPlayerAvatar = PlayerPrefs.GetInt ("AvatarEquipped").ToString ();
		thisPlayerLevel = PlayerPrefs.GetFloat ("CurrentLevel").ToString ();
		thisPlayerExp = PlayerPrefs.GetFloat ("NineBallExperience").ToString ();
	}
}

#region 需要上推到数据库的信息
///// <summary>
///// Dates the player prefs.用户信息本地化
///// </summary>
//public void DatePlayerPrefs()
//{
//  //玩家名字
//	PlayerPrefs.GetString("First_Name");
//	// 获得球杆装备
//	PlayerPrefs.GetInt("CueEquipped");
//	// 头像装备
//	PlayerPrefs.GetInt("AvatarEquipped");
//	// 获得等级
//	PlayerPrefs.GetFloat("CurrentLevel");
//	// 加载经验
//	PlayerPrefs.GetFloat("NineBallExperience");
//	//加载金币
//	PlayerPrefs.GetFloat("Coin");
//
//}
#endregion