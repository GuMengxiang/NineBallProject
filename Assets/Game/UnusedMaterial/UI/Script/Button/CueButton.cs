using UnityEngine;
using System.Collections;

public class CueButton : MonoBehaviour
{
    // generic component
    public int buttonId = 0;
    public GenericButtonState genericButtonState;

    // prefab
    public GameObject WarningPrefab;
    public GameObject LabelPrefab;

    // Cue component
    public UIPanel WindowPanel;
    public UISprite Background;
    public UISprite MaskBackground;
    public UISprite MaskCueButton;
    public UILabel Titlelabel;
    public UILabel TitleMoneylabel;
    public UISprite OkBackground;
    public UISprite MaskOkBackground;
    public UISprite CueBackground;
    public UISprite LockBackground;
    public UISprite Lock;
    public UISprite Image1;
    public UISprite Image2;
    public UISprite Image3;
    public UILabel label1;
    public UILabel label2;
    public UILabel label3;

    public UILabel Oklabel;
    public float xdistance;
    public float ydistance;

    [System.NonSerialized]
    public static float CueGapFromLeft = 0.0f;

    [System.NonSerialized]
    public bool ListeningForConfirmation = false;

    void Start()
    {
        UI();
    }

    void UI()
    {
        Background.GetComponent<BoxCollider>().size = new Vector3(Background.width + xdistance, Background.height + ydistance, 0);

        CueGapFromLeft = Background.height * 0.25f; // slightly smaller than 0.30f, which is button gap to the right

        // overall background
        MaskBackground.width = Background.width;
        MaskBackground.height = Background.height;

        //MaskCueButton
        MaskCueButton.width = Background.width;
        MaskCueButton.height = Background.height;

        //LockBackground
        LockBackground.width = Background.width;
        LockBackground.height = Background.height;

        //Lock
        Lock.height = (int)(Background.height * 0.75f);
        Lock.width = (int)(Lock.height / 1.32f);

        //CueBackground
	
        CueBackground.width = (int)(Background.width * 0.7f);
        CueBackground.height = (int)(Background.width * 0.7f / 8.5f); // 17 is a measurement
        CueBackground.transform.localPosition = new Vector3(CueGapFromLeft - Background.width * 0.5f + CueBackground.width * 0.5f, Background.height * 0.13f , 0);

        if (buttonId < GameManager_script.CueNames.Length)
        {
            CueBackground.spriteName = buttonId.ToString();
        }

        // title should occupy about 1/3 of the real estates on top and located slightly to the center left
        Titlelabel.height = (int)(Background.height * 0.25f);
        Titlelabel.width = (int)(Background.width * 0.15f);
        Titlelabel.transform.localPosition = new Vector3(CueGapFromLeft + Background.width * -0.5f, -Background.height * 0.22f, 0);
        Titlelabel.text = Localization.Get(GameManager_script.CueNames[buttonId]);

        // everything related to the OK button
        OkBackground.height = (int)(Background.height * 0.4f);
        OkBackground.width = (int)(OkBackground.height * 3.0f);

        MaskOkBackground.height = (int)(OkBackground.height * 1.00f);
        MaskOkBackground.width = (int)(OkBackground.width * 1.00f);

        Oklabel.width = (int)(OkBackground.width * 0.900f);
        Oklabel.height = (int)(OkBackground.height * 0.750f);

        OkBackground.GetComponent<BoxCollider>().size = new Vector3(OkBackground.width * 1.00f, OkBackground.height * 1.00f, 0);
        OkBackground.transform.localPosition = new Vector3(Background.width * 0.5f - OkBackground.width * 0.5f - Background.height * 0.30f, 0, 0); // same as height gap

        Image1.width = (int)(Background.height / 3.0f);
        Image1.height = (int)(Background.height / 3.0f);
        Image1.transform.localPosition = new Vector3(CueGapFromLeft + Background.width * -0.5f + Image1.width * 0.5f + Image1.width * 0.5f + Titlelabel.width, -Background.height * 0.2f, 0);

        label1.height = (int)(Background.height * 0.225f);
        label1.width = (int)(Background.width * 0.15f);
        label1.transform.localPosition = new Vector3(CueGapFromLeft + Image1.transform.localPosition.x + Image1.width * 0.25f, -Background.height * 0.22f, 0);

        ChangeImage1(GameManager_script.getStarLevelOnSpeed(GameManager_script.CueAttributes[buttonId][(int)CueAttributesType.force]));
        label1.text = GameManager_script.getTextOnSpeed(GameManager_script.CueAttributes[buttonId][(int)CueAttributesType.force]);

        Image2.width = (int)(Background.height / 3.0f);
        Image2.height = (int)(Background.height / 3.0f);
        Image2.transform.localPosition = new Vector3(CueGapFromLeft + label1.transform.localPosition.x + label1.width * 0.5f + Image2.width * 0.5f + label1.width * 0.5f, -Background.height * 0.2f, 0);

        label2.height = (int)(Background.height * 0.225f);
        label2.width = (int)(Background.width * 0.15f);
        label2.transform.localPosition = new Vector3(CueGapFromLeft + Image2.transform.localPosition.x + Image2.width * 0.25f, -Background.height * 0.22f, 0);

        ChangeImage2(GameManager_script.getStarLevelOnExtension(GameManager_script.CueAttributes[buttonId][(int)CueAttributesType.extend]));
        label2.text = GameManager_script.getTextOnExtension(GameManager_script.CueAttributes[buttonId][(int)CueAttributesType.extend]);

        Image3.width = (int)(Background.height / 3.0f);
        Image3.height = (int)(Background.height / 3.0f);
        Image3.transform.localPosition = new Vector3(CueGapFromLeft + label2.transform.localPosition.x + label2.width * 0.5f + Image3.width * 0.5f + label2.width * 0.5f, -Background.height * 0.2f, 0);

        label3.height = (int)(Background.height * 0.225f);
        label3.width = (int)(Background.width * 0.15f);
        label3.transform.localPosition = new Vector3(CueGapFromLeft + Image3.transform.localPosition.x + Image3.width * 0.25f, -Background.height * 0.22f, 0);

        ChangeImage3(GameManager_script.getStarLevelOnSpin(GameManager_script.CueAttributes[buttonId][(int)CueAttributesType.spin]));
        label3.text = GameManager_script.getTextOnSpin(GameManager_script.CueAttributes[buttonId][(int)CueAttributesType.spin]);

        InitOKLabelText();

        InitLockLabelText();
    }

    public  void InitOKLabelText()
    {
        if (genericButtonState == GenericButtonState.equipped)
        {
            Oklabel.text = Localization.Get("equipped");
            MaskOkBackground.spriteName = "blueButton";
        }
        else if (genericButtonState == GenericButtonState.owned)
        {
            Oklabel.text = Localization.Get("owned");
            MaskOkBackground.spriteName = "greenbutton";
        }
        else if (genericButtonState == GenericButtonState.locked)
        {
            Oklabel.text = Localization.Get("level") + " " + GameManager_script.CueAttributes[buttonId][(int)CueAttributesType.level];
            MaskOkBackground.spriteName = "renbutton"; // typo in asset

            ChangeLock(true);
        }
        else if (genericButtonState == GenericButtonState.free)
        {
            Oklabel.text = Localization.Get("free");
            MaskOkBackground.spriteName = "greenbutton";
        }
        else if (genericButtonState == GenericButtonState.payable)
        {
            if (buttonId > 19) // dajiang dajiang, hack the first 20 are coin based
            {
                Oklabel.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.CueAttributes[buttonId][(int)CueAttributesType.price], "largenumber") + " " + Localization.Get("USD");
                MaskOkBackground.spriteName = "greenbutton";
            }
            else
            {
                Oklabel.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.CueAttributes[buttonId][(int)CueAttributesType.price], "gamecoinz");
                MaskOkBackground.spriteName = "greenbutton";
            }
        }
        else if (genericButtonState == GenericButtonState.cantafford)
        {
            if (buttonId > 19) // dajiang dajiang, hack the first 20 are coin based
            {
                Oklabel.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.CueAttributes[buttonId][(int)CueAttributesType.price], "largenumber") + " " + Localization.Get("USD");
                MaskOkBackground.spriteName = "greenbutton";
            }
            else
            {
                Oklabel.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.CueAttributes[buttonId][(int)CueAttributesType.price], "gamecoinz");
                MaskOkBackground.spriteName = "greenbutton";
            }
        }
    }

    void InitLockLabelText()
    {
        if (GameManager_script.Instance().CueNew[buttonId])
        {
            GameObject Group = (GameObject)Instantiate(LabelPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);

            Group.transform.parent = gameObject.transform;

            Group.GetComponent<Label>().GetComponent<UISprite>().width = (int)(Background.height * 1.38f); // 1.38f is measured
            Group.GetComponent<Label>().GetComponent<UISprite>().height = (int)(Background.height * 1.38f);

            Group.transform.localPosition = new Vector3(Background.width * -0.5f + Group.GetComponent<Label>().GetComponent<UISprite>().width * 0.5f, Background.height * -0.19f, 0.0f); // 0.19f = (1.38f - 1.0f) / 2;
            Group.transform.localScale = new Vector3(1, 1, 1);

            if (GameManager_script.Instance().CueNew[buttonId])
            {
                Group.GetComponent<Label>().labeltext = Localization.Get("NEW");
                Group.GetComponent<Label>().LabelImageName = "BlueLabel";
            }
        }

        if (genericButtonState == GenericButtonState.locked)
        {
            ChangeLock(true);
        }
    }

    public void ChangeImage1(int index)
    {
        Image1.spriteName = GameManager_script.CueRankName[index];
    }

    public void ChangeImage2(int index)
    {
        Image2.spriteName = GameManager_script.CueRankName[index];
    }

    public void ChangeImage3(int index)
    {
        Image3.spriteName = GameManager_script.CueRankName[index];
    }

    public void ChangeCueImage(int index)
    {
        MaskBackground.spriteName = index.ToString();
    }

    public void ChangeLock(bool inDisplay)
    {
        LockBackground.gameObject.SetActive(inDisplay);
        Lock.gameObject.SetActive(inDisplay);

        if (inDisplay)
        {
            OkBackground.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
