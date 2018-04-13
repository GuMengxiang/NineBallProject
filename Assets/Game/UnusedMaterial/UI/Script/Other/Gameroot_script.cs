using UnityEngine;
using System.Collections;

public class Gameroot_script : MonoBehaviour
{
    private UICamera_script uiCamera_script;

	// Use this for initialization
	void Start ()
    {
        uiCamera_script = gameObject.GetComponentInChildren<UICamera_script>();
    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    // disable UI camera and enable game camera when we switch from menu to game
    void prepareGameStart()
    {
        // actually just load a different level is the right way to go
    }
}
