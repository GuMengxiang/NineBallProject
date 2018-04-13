using UnityEngine;
using System.Collections;

public class DailyBonusDoorButton : MonoBehaviour
{
    public GameObject[] OtherDoor;
    public GameObject DoorLabel;
    public GameObject DoorBackground;
    public bool DoorLabelBool;
    public GameObject ParticleCamera;

    public void OnClick()
    {
        for (int i = 0; i < OtherDoor.Length; i++)
        {
            OtherDoor[i].GetComponent<BoxCollider>().enabled = false;
        }

        DoorLabel.GetComponent<TweenPosition>().from = new Vector3(0, 0, 0);
        ParticleCamera.SetActive(true);
        DoorLabel.SetActive(true);
        DoorBackground.SetActive(true);
        DoorLabelBool = true;

        // add coins
        GameManager_script.Instance().UpdateCoinCount(GameManager_script.Instance().CurrentBonusAmount);

        // label as claimed ever
        GameManager_script.Instance().ClaimedDailyBonusEver = 1.0f;
        PlayerPrefs.SetFloat("ClaimedDailyBonusEver", GameManager_script.Instance().ClaimedDailyBonusEver);

        // button click
        Analytic.EventHappenPing("DailyBonus Click");

        float d_b_volume = 1.0f;
        int d_b_index = (int)MusicClip.Cash;

        GameManager_script.Instance().PlaySound(d_b_index, false, d_b_volume);
    }
}
