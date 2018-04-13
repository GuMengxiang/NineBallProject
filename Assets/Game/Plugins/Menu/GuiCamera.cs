// This will most likely be modified by a NGUI camera because all visible components will be changed to NGUI later (circular slider stuff we can talk later)

using UnityEngine;
using System.Collections;

public class GuiCamera : MonoBehaviour 
{
	void Awake () 
	{
		if(MenuControllerGenerator.controller)
        {
            MenuControllerGenerator.controller.guiCamera = GetComponent<Camera>();
        }
	}
}
