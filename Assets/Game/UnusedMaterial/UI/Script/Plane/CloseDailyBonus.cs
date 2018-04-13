using UnityEngine;
using System.Collections;

public class CloseDailyBonus : MonoBehaviour
{
    public GameObject DailyBonus;

    void Start ()
    {
        StartCoroutine(CloseUI());
	}

    public IEnumerator CloseUI()
    {
        yield return new WaitForSeconds(4.0f);

        DailyBonus.GetComponent<UIDailyBonusPanel>().CloseDailyBonusPanel();
    }
}
