using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public GameObject Ngui;
	
	void Update()
    {
        if (Ngui.activeSelf)
        {
            if (Application.loadedLevelName == "Game")
            {
                Ngui.SetActive(false);
            }

        }
	}
}
