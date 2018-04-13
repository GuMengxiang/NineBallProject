using UnityEngine;
using System.Collections;

public class AvatarButton : MonoBehaviour
{
    // generic component
    public int buttonId = 0;
    public GenericButtonState genericButtonState;

    // prefab
    public GameObject WarningPrefab;
    public GameObject LabelPrefab;

    // avatar component
    public UIPanel WindowPanel;
    public UISprite Sprite_Enter;
    public UILabel ButtonEnterLabel;
    public UISprite Background;
    public UISprite MaskBackground;
    public UISprite MaskAvatarButton;
    public UISprite LockBackground;
    public UISprite Lock;

    public float heightButton_Enter=8.27f;
    public float PositionButton_Enter = -2.85f;
    public float heightSprite_Enter = 1f;
    public float PositionSprite_Enter = 2.85f;
    public float xdistance;
    public float ydistance;

    [System.NonSerialized]
    public bool ListeningForConfirmation = false;

	void Start ()
    {
        UI();
	}
	
    void UI()
    {
        Background.GetComponent<BoxCollider>().size = new Vector3(Background.width + xdistance, Background.height + ydistance, 0);
        MaskBackground.width = Background.width;
        MaskBackground.height = Background.height;

        //MaskAvatarButton
        MaskAvatarButton.width = Background.width;
        MaskAvatarButton.height = Background.height;

        //LockBackground
        LockBackground.width = Background.width;
        LockBackground.height = Background.height;
        
        //Lock
        Lock.height = (int)(Background.height * 0.30f);
        Lock.width = (int)(Lock.height / 1.32f);
        Lock.transform.localPosition = new Vector3(Background.width * -0.35f, Background.height * 0.32f, 0);
        
        //图片框
        Sprite_Enter.transform.localPosition = new Vector3(0, Background.height / PositionSprite_Enter, 0);
        Sprite_Enter.height = (int)(Background.height / heightSprite_Enter*1.1f);
        Sprite_Enter.width = (int)(Sprite_Enter.height);
        
        //按钮文本
        ButtonEnterLabel.width = (int)(Background.width);
        ButtonEnterLabel.height = (int)(Background.height * 0.13f);
        ButtonEnterLabel.transform.localPosition = new Vector3(0, -Background.height * 0.5f + ButtonEnterLabel.height * 0.5f + Background.height * 0.025f, 0);

        if (genericButtonState == GenericButtonState.equipped)
        {
            ButtonEnterLabel.text = Localization.Get("equipped");
            MaskBackground.spriteName = "BlueAvatarButton";
        }
        else if (genericButtonState == GenericButtonState.owned)
        {
            ButtonEnterLabel.text = Localization.Get("owned");
        }
        else if (genericButtonState == GenericButtonState.locked)
        {
            ButtonEnterLabel.text = Localization.Get("level") + " " + GameManager_script.AvatarAttributes[buttonId][(int)AvatarAttributesType.level];
        }
        else if (genericButtonState == GenericButtonState.free)
        {
            ButtonEnterLabel.text = Localization.Get("free");
        }
        else if (genericButtonState == GenericButtonState.payable)
        {
            ButtonEnterLabel.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.AvatarAttributes[buttonId][(int)AvatarAttributesType.price], "gamecoinz");
        }
        else if (genericButtonState == GenericButtonState.cantafford)
        {
            ButtonEnterLabel.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.AvatarAttributes[buttonId][(int)AvatarAttributesType.price], "gamecoinz"); // this one should be in red
        }

        InitLockAndBanner();
    }

    public void OnClick()
    {
        if (genericButtonState == GenericButtonState.equipped || genericButtonState == GenericButtonState.locked)
        {
            // if it is already equipped, no change
        }
        else if (genericButtonState == GenericButtonState.free || genericButtonState == GenericButtonState.owned)
        {
            // take care of biniz rite her
            genericButtonState = GenericButtonState.equipped;

            GameManager_script.Instance().SetAvatarStateAfterPurchase(buttonId);

            SwapCurrentEquippedAvatar();

            EquipCurrentAvatar();
        }
        else if (genericButtonState == GenericButtonState.payable)
        {
            PrepareTheConfirmationWindow();
        }
        else if (genericButtonState == GenericButtonState.cantafford)
        {
            // take the user to buy page
        }

        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
    }

    public void PrepareTheConfirmationWindow()
    {
        GameObject Group = (GameObject)Instantiate(WarningPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);

        Group.transform.parent = WindowPanel.transform;
        Group.transform.localPosition = Vector3.zero; // final destination
        Group.transform.localScale = new Vector3(1, 1, 1);
        Group.GetComponent<UIWarningPanel>().RootPanel = WindowPanel;
        Group.GetComponent<UIWarningPanel>().ChangeInsideDepth(GameManager_script.UIConfirmLayerStart);
        Group.GetComponent<UIWarningPanel>().WarningUI(new WarningInfo("Warning2", "Yes", "No"));

        ListeningForConfirmation = true; // bitches!!!
    }

    void Update()
    {
        if (ListeningForConfirmation)
        {
            if (GameManager_script.ConfirmationState == ConfirmationType.confirmed)
            {
                // click confirm
                ButtonClickConfirmed();

                // stop listening
                ListeningForConfirmation = false;

                // reset confirmation state
                GameManager_script.ConfirmationState = ConfirmationType.undecided;
            }
            else if (GameManager_script.ConfirmationState == ConfirmationType.denied)
            {
                // do nothing for real

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

    void ButtonClickConfirmed()
    {
        if (genericButtonState == GenericButtonState.equipped || genericButtonState == GenericButtonState.locked)
        {
            // if it is already equipped, no change
        }
        else if (genericButtonState == GenericButtonState.free || genericButtonState == GenericButtonState.owned)
        {
            // do nothing coz we already took care of it earlier
        }
        else if (genericButtonState == GenericButtonState.payable)
        {
            float oldMoney = GameManager_script.Instance().CoinCount;

            genericButtonState = GenericButtonState.equipped;

            GameManager_script.Instance().UpdateCoinCount(-1.0f * GameManager_script.AvatarAttributes[buttonId][(int)AvatarAttributesType.price]);

            GameManager_script.Instance().SetAvatarStateAfterPurchase(buttonId);

            SwapCurrentEquippedAvatar();

            EquipCurrentAvatar();

            ChangeMoney(true, oldMoney);
        }
        else if (genericButtonState == GenericButtonState.cantafford)
        {
            // take the user to buy page
        }
    }

    void EquipCurrentAvatar()
    {
        // change button appearance
        ButtonEnterLabel.text = Localization.Get("equipped");
        MaskBackground.spriteName = "BlueAvatarButton";

        // sub in the current avatar object
        WindowPanel.GetComponent<UIWindowPanel>().EquippedAvatar = gameObject;

        // refresh player profile picture
        // buttonId
    }

    void SwapCurrentEquippedAvatar()
    {
        // avatar button about to be swapped out
        AvatarButton gb = WindowPanel.GetComponent<UIWindowPanel>().EquippedAvatar.GetComponent<AvatarButton>();

        // unequip the previous avatar object
        gb.ButtonEnterLabel.text = Localization.Get("owned");
        gb.MaskBackground.spriteName = "AvatarButton";
        // change state
        gb.genericButtonState = GenericButtonState.owned;
    }

    void InitLockAndBanner()
    {
        if (GameManager_script.Instance().AvatarNew[buttonId])
        {
            GameObject Group = (GameObject)Instantiate(LabelPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);

            Group.transform.parent = gameObject.transform;
            Group.GetComponent<Label>().GetComponent<UISprite>().width = (int)(Background.width * 0.5f);
            Group.GetComponent<Label>().GetComponent<UISprite>().height = (int)(Background.width * 0.5f);
            Group.transform.localPosition = new Vector3(Background.width * -0.5f + Group.GetComponent<Label>().GetComponent<UISprite>().width * 0.5f, Background.height * 0.5f + Group.GetComponent<Label>().GetComponent<UISprite>().height * -0.5f, 0.0f); // final destination
            Group.transform.localScale = new Vector3(1, 1, 1);

            if (GameManager_script.Instance().AvatarNew[buttonId])
            {
                Group.GetComponent<Label>().labeltext = Localization.Get("NEW");
            }
        }

        if (genericButtonState == GenericButtonState.locked)
        {
            ChangeLock(true);
        }
    }

    public void ChangeLock(bool inDisplay)
    {
        LockBackground.gameObject.SetActive(inDisplay);
        Lock.gameObject.SetActive(inDisplay);

        if (inDisplay)
        {
            gameObject.GetComponent<UIButton>().enabled = false;
        }
    }

    public void ChangeMoney(bool Bool, float oldMoney)
    {
        if (Bool)
        {
            WindowPanel.GetComponent<UIWindowPanel>().UpdateMoney(oldMoney);
        }
        else
        {
            WindowPanel.GetComponent<UIWindowPanel>().ChangeMoney(GameManager_script.Instance().CoinCount);
        }
    }
}
