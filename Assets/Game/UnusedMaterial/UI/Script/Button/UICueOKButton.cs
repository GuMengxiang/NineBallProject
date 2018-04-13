using UnityEngine;
using System.Collections;

public class UICueOKButton : MonoBehaviour
{
    public void OnClick()
    {
        // clicking on the main thing does nothing, this should be transferred to button click
        if (gameObject.transform.parent.GetComponent<CueButton>().genericButtonState == GenericButtonState.equipped || gameObject.transform.parent.GetComponent<CueButton>().genericButtonState == GenericButtonState.locked)
        {
            // if it is already equipped, no change
        }
        else if (gameObject.transform.parent.GetComponent<CueButton>().genericButtonState == GenericButtonState.free || gameObject.transform.parent.GetComponent<CueButton>().genericButtonState == GenericButtonState.owned)
        {
            gameObject.transform.parent.GetComponent<CueButton>().genericButtonState = GenericButtonState.equipped;

            GameManager_script.Instance().SetCueStateAfterPurchase(gameObject.transform.parent.GetComponent<CueButton>().buttonId);

            SwapCurrentEquippedCue();

            EquipCurrentCue();
        }
        else if (gameObject.transform.parent.GetComponent<CueButton>().genericButtonState == GenericButtonState.payable)
        {
//            if (gameObject.transform.parent.GetComponent<CueButton>().buttonId > 19) // dajiang dajiang, hack the first 20 are coin based
//            {
//                // dajiang dajiang 在这里激活360购买的弹窗，buttonId就是要购买的球杆的编号
//            }
//            else
//            {
                PrepareTheConfirmationWindow();
//            }
        }
        else if (gameObject.transform.parent.GetComponent<CueButton>().genericButtonState == GenericButtonState.cantafford)
        {
            // do nothing
        }

        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
    }

    public void PrepareTheConfirmationWindow()
    {
        GameObject Group = (GameObject)Instantiate(gameObject.transform.parent.GetComponent<CueButton>().WarningPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);

        Group.transform.parent = gameObject.transform.parent.GetComponent<CueButton>().WindowPanel.transform;
        Group.transform.localPosition = Vector3.zero; // final destination
        Group.transform.localScale = new Vector3(1, 1, 1);
        Group.GetComponent<UIWarningPanel>().RootPanel = gameObject.transform.parent.GetComponent<CueButton>().WindowPanel;
        Group.GetComponent<UIWarningPanel>().ChangeInsideDepth(GameManager_script.UIConfirmLayerStart);
        Group.GetComponent<UIWarningPanel>().WarningUI(new WarningInfo("Warning3", "Yes", "No"));

        gameObject.transform.parent.GetComponent<CueButton>().ListeningForConfirmation = true; // bitches!!!
    }

    void EquipCurrentCue()
    {
        // change button appearance
        gameObject.transform.parent.GetComponent<CueButton>().Oklabel.text = Localization.Get("equipped");
        gameObject.transform.parent.GetComponent<CueButton>().MaskOkBackground.spriteName = "blueButton";

        // sub in the current cue object
        gameObject.transform.parent.GetComponent<CueButton>().WindowPanel.GetComponent<UIWindowPanel>().EquippedCue = gameObject.transform.parent.gameObject;
    }

    void SwapCurrentEquippedCue()
    {
        // CueButton about to be swapped out
        CueButton gb = gameObject.transform.parent.GetComponent<CueButton>().WindowPanel.GetComponent<UIWindowPanel>().EquippedCue.GetComponent<CueButton>();

        // unequip the previous cue object
        gb.Oklabel.text = Localization.Get("owned");
        gb.MaskOkBackground.spriteName = "greenbutton";

        // change state
        gb.genericButtonState = GenericButtonState.owned;
    }

    void Update()
    {
        if (gameObject.transform.parent.GetComponent<CueButton>().ListeningForConfirmation)
        {
            if (GameManager_script.ConfirmationState == ConfirmationType.confirmed)
            {
                // click confirm
                ButtonClickConfirmed();

                // stop listening
                gameObject.transform.parent.GetComponent<CueButton>().ListeningForConfirmation = false;

                // reset confirmation state
                GameManager_script.ConfirmationState = ConfirmationType.undecided;
            }
            else if (GameManager_script.ConfirmationState == ConfirmationType.denied)
            {
                // do nothing for real

                // stop listening
                gameObject.transform.parent.GetComponent<CueButton>().ListeningForConfirmation = false;

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
        if (gameObject.transform.parent.GetComponent<CueButton>().genericButtonState == GenericButtonState.equipped || gameObject.transform.parent.GetComponent<CueButton>().genericButtonState == GenericButtonState.locked)
        {
            // if it is already equipped, no change
        }
        else if (gameObject.transform.parent.GetComponent<CueButton>().genericButtonState == GenericButtonState.free || gameObject.transform.parent.GetComponent<CueButton>().genericButtonState == GenericButtonState.owned)
        {
            // do nothing coz we already took care of it earlier
        }
        else if (gameObject.transform.parent.GetComponent<CueButton>().genericButtonState == GenericButtonState.payable)
        {
            float oldMoney = GameManager_script.Instance().CoinCount;

            gameObject.transform.parent.GetComponent<CueButton>().genericButtonState = GenericButtonState.equipped;

            GameManager_script.Instance().UpdateCoinCount(-1.0f * GameManager_script.CueAttributes[gameObject.transform.parent.GetComponent<CueButton>().buttonId][(int)CueAttributesType.price]);

            GameManager_script.Instance().SetCueStateAfterPurchase(gameObject.transform.parent.GetComponent<CueButton>().buttonId);

            SwapCurrentEquippedCue();

            EquipCurrentCue();

            ChangeMoney(true, oldMoney);
        }
        else if (gameObject.transform.parent.GetComponent<CueButton>().genericButtonState == GenericButtonState.cantafford)
        {
            // take the user to buy page
        }
    }

    public void ChangeMoney(bool Bool, float oldMoney)
    {
        UIPanel Panel= gameObject.transform.parent.GetComponent<CueButton>().WindowPanel;
        
        if (Bool)
        {
            Panel.GetComponent<UIWindowPanel>().UpdateMoney(oldMoney);
        }
        else
        {
            Panel.GetComponent<UIWindowPanel>().ChangeMoney(GameManager_script.Instance().CoinCount);
        }
    }
}
