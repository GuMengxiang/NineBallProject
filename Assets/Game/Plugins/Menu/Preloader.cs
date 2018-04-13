using UnityEngine;
using System.Collections;

public class Preloader : MonoBehaviour 
{
	[System.NonSerialized]
	public bool isDone = false;

	[System.NonSerialized]
	public MenuController controller;

	public void SetState (bool state)
	{
		isDone = !state;
		controller.loaderCamera.enabled = state; // there is actually a camera right here
	}

	public void UpdateLoader (float time)
	{
	}
}
