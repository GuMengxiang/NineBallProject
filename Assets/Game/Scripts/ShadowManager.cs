using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShadowManager : MonoBehaviour
{
    public List<GameObject> ShadowPrefabs;
    public GameObject ShadowGroup;
    public List<GameObject> Shadows;
    public CueController cueController;
    public Texture ShadowTexture;

    public float shadowBoxHeight; // changed in the scene

    void FixedUpdate()
    {
        if (cueController && Shadows.Count == cueController.ballsCount)
        {
            for (int i = 0; i < Shadows.Count; i++)
            {
                Shadows[i].transform.localPosition = cueController.allBallControllers[i].GetComponent<Rigidbody>().position;
                Shadows[i].transform.localPosition += new Vector3(0.0f, shadowBoxHeight, 0.0f);
                
                if (cueController.allBallControllers[i].ballIsPocketed)
                {
                    Shadows[i].SetActive(false);
                }
                else
                {
                    Shadows[i].SetActive(true);
                }
            }
        }
	}

    public void InitShadow(CueController inCueController)
    {
        cueController = inCueController;

        for (int i = 0; i < cueController.allBallControllers.Length; i++)
        {
            GameObject Group = (GameObject)Instantiate(ShadowPrefabs[i], (Vector3)ShadowGroup.transform.position, (Quaternion)ShadowPrefabs[i].transform.rotation);

            Group.transform.parent = ShadowGroup.transform;
            Group.transform.localPosition = cueController.allBallControllers[i].GetComponent<Rigidbody>().position;
            Group.transform.localPosition += new Vector3(0.0f, shadowBoxHeight, 0.0f);
            Group.transform.localScale = new Vector3(0.24f, 0.24f, 0.24f);
            Group.GetComponent<Renderer>().material.mainTexture = ShadowTexture;
            Group.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
            
            Shadows.Add(Group);
        }
    }

    public void UpdateShadow(CueController inCueController, int inColorNumber)
    {
        cueController = inCueController;

        for (int i = 0; i < cueController.allBallControllers.Length; i++)
        {
            Shadows[i].GetComponent<SelectBall>().ChangeSelect(false);
        }

        if (inColorNumber != 0)
        {
            Shadows[inColorNumber].GetComponent<SelectBall>().ChangeSelect(true);
        }
    }
}
