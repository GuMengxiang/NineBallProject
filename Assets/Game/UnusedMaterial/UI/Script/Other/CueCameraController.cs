using UnityEngine;
using System.Collections;

public class CueCameraController : MonoBehaviour {

    public bool CameraBool;
    public Camera CueCamera;

    void Start()
    {
        if (GameObject.Find("Camera2D/CueCamera"))
        {
            CueCamera = GameObject.Find("Camera2D/CueCamera").GetComponent<Camera>();
        }
    }

    void OnClick()
    {
        if (CueCamera)
        {
            if (CameraBool)
            {
                CueCamera.depth = 5;
            }
            else
            {
                CueCamera.depth = 3;
            }
        }
    }
	
}
