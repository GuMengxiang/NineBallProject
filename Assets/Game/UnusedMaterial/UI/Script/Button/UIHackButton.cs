using UnityEngine;
using System.Collections;

public class UIHackButton : MonoBehaviour
{
    public UIPanel RootPanel;
    public GameObject HackButton;
    public UISprite HackButtonBackground;
    public UISprite HackButtonMask;
    public UILabel HackButtonLabel;

    [System.NonSerialized]
    public float heightHackButton = 10.0f;

    void Start ()
    {
        if (HackButton == null)
        {
            HackButton = gameObject.transform.parent.gameObject;
        }

        //Hack按钮
        HackButtonBackground.height = (int)(RootPanel.height / heightHackButton);
        HackButtonBackground.width = (int)(RootPanel.height / heightHackButton * 4.05f);

        HackButtonMask.height = (int)(RootPanel.height / heightHackButton) + 2;
        HackButtonMask.width = (int)(RootPanel.height / heightHackButton * 4.05f) + 2;
        HackButtonMask.GetComponent<BoxCollider>().size = new Vector3(HackButtonMask.width, HackButtonMask.height, 0);
        
        // Hack label
        HackButtonLabel.width = (int)(RootPanel.height / heightHackButton * 4.05f * 0.9f);
        HackButtonLabel.height = (int)(RootPanel.height / heightHackButton * 0.9f);
        HackButtonLabel.transform.localPosition = new Vector3(HackButtonLabel.transform.localPosition.x, HackButtonLabel.transform.localPosition.y - HackButtonLabel.height * 0.035f, HackButtonLabel.transform.localPosition.z);

        HackButton.transform.localPosition = new Vector3(-RootPanel.width / 2 + HackButtonBackground.width / 2, RootPanel.height / 2 - HackButtonBackground.height / 2, 0.0f);
        HackButton.SetActive(false); // GameManager_script.Instance().HackButtonShow
	}
	
    void OnClick()
    {
        // dajiang hack lolz
    }

    void Update()
    {
        // dajiang hack lolz
        HackButtonLabel.text = "";
    }
}
