using UnityEngine;
using System.Collections;
using Firebase.Auth;
public class GamePureExitButton : MonoBehaviour
{
    public UIPanel WindowPanel;
    public GameObject Exit;
    public GameObject WarningPrefab;
	FirebaseAuth auth;
    [System.NonSerialized]
    public bool ListeningForConfirmation = false;

    void Start()
    {
        UI();
		auth = FirebaseAuth.DefaultInstance;
    }

    public void UI()
    {
        Exit.GetComponent<UIButton>().tweenTarget.GetComponent<UISprite>().width = (int)(WindowPanel.height * 0.095f);
        Exit.GetComponent<UIButton>().tweenTarget.GetComponent<UISprite>().height = (int)(WindowPanel.height * 0.095f);
        Exit.GetComponent<BoxCollider>().size = new Vector3(WindowPanel.height * 0.095f * 1.25f, WindowPanel.height * 0.095f * 1.25f, 0);
        Exit.transform.localPosition = new Vector3(WindowPanel.height * 0.095f * 0.625f - WindowPanel.width * 0.5f, WindowPanel.height * 0.5f - WindowPanel.height * 0.095f * 0.625f, 0.0f);

#if UNITY_ANDROID
        Exit.gameObject.SetActive(true);
#else
        Exit.gameObject.SetActive(false);
#endif
    }

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            GameManager_script.Instance().DownOnRealButtons = true;
        }
        else
        {
            GameManager_script.Instance().DownOnRealButtons = false;
        }
    }

    void OnClick()
    {
        WarningUI();
		GameObject.Find ("Gameroot").GetComponent<Savethedatabase>().OnClick();
		auth.SignOut();
		Debug.Log ("9ball "+" SignOut");
        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);

    }

    void WarningUI()
    {
        GameObject Group = (GameObject)Instantiate(WarningPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);

        // this tag is for in game little popups i think
        Group.transform.parent = WindowPanel.transform;
        Group.transform.localPosition = Vector3.zero; // final destination
        Group.transform.localScale = new Vector3(1, 1, 1);
        Group.GetComponent<UIWarningPanel>().RootPanel = WindowPanel;
        Group.GetComponent<UIWarningPanel>().ChangeInsideDepth(GameManager_script.UIConfirmLayerStart);
        Group.GetComponent<UIWarningPanel>().WarningUI(new WarningInfo("Warning4", "Yes", "No"));

        ListeningForConfirmation = true; // bitches!!!
    }

    void Update()
    {
        if (ListeningForConfirmation)
        {
            if (GameManager_script.ConfirmationState == ConfirmationType.confirmed)
            {
                // stop listening
                ListeningForConfirmation = false;

                // reset confirmation state
                GameManager_script.ConfirmationState = ConfirmationType.undecided;

                // kill the application
                Application.Quit();
            }
            else if (GameManager_script.ConfirmationState == ConfirmationType.denied)
            {
                // stop listening
                ListeningForConfirmation = false;

                // reset confirmation state
                GameManager_script.ConfirmationState = ConfirmationType.undecided;
            }
            else
            {
                // do nothing and keep on listening
            }
        }
    }
}
