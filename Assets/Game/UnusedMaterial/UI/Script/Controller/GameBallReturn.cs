using UnityEngine;
using System.Collections;

public class GameBallReturn : MonoBehaviour
{
    public GameObject[] BallReturns;

	void Start ()
    {
        BallReturnSetActive(InGameBallReturnDisplayInfo.BallReturn);
	}

    public void BallReturnSetActive(InGameBallReturnDisplayInfo vInGameBallReturnDisplayInfo)
    {
        for (int i = 0; i < BallReturns.Length; i++)
        {
            if (i == (int)(vInGameBallReturnDisplayInfo))
            {
                BallReturns[i].SetActive(true);
            }
            else
            {
                BallReturns[i].SetActive(false);
            }
        }
    }
}
