// This is in charge of the buttons (3D/2D and backhome)
// We will definitely rid all of these in the future

using UnityEngine;
using System.Collections;

public class GameMenu : MonoBehaviour 
{ 
	public bool is3D = true;

	void ChangeCamera(Button btn)
	{
		is3D = btn.state;
	}
}
