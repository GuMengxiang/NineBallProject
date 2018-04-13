using UnityEngine;
using System.Collections;

public class GameSpinNakedCollider : MonoBehaviour
{
    public UIPanel RootPanel;
    public UISprite SpinNakedCollider;
    public GameObject SpinNaked;
    public Camera CueCamera;
	// Use this for initialization
	void Start ()
    {
        UI();
   
        if (GameObject.Find("Camera2D/CueCamera"))
        {
            CueCamera = GameObject.Find("Camera2D/CueCamera").GetComponent<Camera>();
        }

	}
	
	// Update is called once per frame
	void UI ()
    {
        SpinNakedCollider.width = (int)(RootPanel.width*1.01f);
        SpinNakedCollider.height = (int)(RootPanel.height * 1.01f);
        SpinNakedCollider.GetComponent<BoxCollider>().size = new Vector3(RootPanel.width, RootPanel.height, 0);
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

            SpinNaked.gameObject.SetActive(false);

            //CloseCueCamera
            if (CueCamera)
            {
                CueCamera.depth = 5;
            }
        }
    }
}
