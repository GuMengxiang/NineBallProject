using UnityEngine;
using System.Collections;

public class ProfileButton : MonoBehaviour
{
    // Profile component
    public UIPanel WindowPanel;
    public int buttonId = 0;
    public UISprite Background;
    public UISprite MaskBackground;
    public UISprite MaskProfileButton;
    public float xdistance;
    public float ydistance;
    public GameObject LabelPrefab;
    public UISprite ButtonImage;
    public UILabel ButtonName;

    void Start()
    {
        UI();
    }

    void UI()
    {
        Background.GetComponent<BoxCollider>().size = new Vector3(Background.width + xdistance, Background.height + ydistance, 0);
        MaskBackground.width = Background.width;
        MaskBackground.height = Background.height;

        //MaskProfileButton
        MaskProfileButton.width = Background.width;
        MaskProfileButton.height = Background.height;

        //ButtonImage
        ButtonImage.height = (int)(Background.height);
        ButtonImage.width = (int)(Background.height*2);
        ButtonImage.transform.localPosition = Vector3.zero;

        //ButtonName
        ButtonName.height = (int)(Background.height * 0.13f);
        ButtonName.width = (int)(Background.width);
        ButtonName.transform.localPosition = new Vector3(0, -Background.height * 0.5f + ButtonName.height * 0.5f + Background.height * 0.025f, 0);

        // try to init lock label texts...
        if (buttonId == 0)
        {
            if (GameManager_script.Instance().GetNewStatsCount() > 0)
            {
                InitLockLabelText(true, Localization.Get("NEW"));
            }

            ButtonName.text = Localization.Get("WindowTitleStats");
        }

        if (buttonId == 1)
        {
            if (true) // right now its true....
            {
                InitLockLabelText(true, Localization.Get("ComingSoon"));
            }

            ButtonName.text = Localization.Get("WindowTitleAchievement");
        }
    }

    public void InitLockLabelText(bool inUse, string inLabel)
    {
        if (inUse)
        {
            GameObject Group = (GameObject)Instantiate(LabelPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);

            Group.transform.parent = gameObject.transform;

            Group.GetComponent<Label>().GetComponent<UISprite>().width = (int)(Background.width * 0.33f);
            Group.GetComponent<Label>().GetComponent<UISprite>().height = (int)(Background.width * 0.33f);

            Group.transform.localPosition = new Vector3(Background.width * -0.5f + Group.GetComponent<Label>().GetComponent<UISprite>().width * 0.5f, Background.height * 0.5f + Group.GetComponent<Label>().GetComponent<UISprite>().height * -0.5f, 0.0f); // final destination
            Group.transform.localScale = new Vector3(1, 1, 1);

            Group.GetComponent<Label>().labeltext = Localization.Get(inLabel);
        }
    }

    public void OnClick()
    {
      //  GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
    }
}
