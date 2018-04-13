// I will replace all elements with NGUI at some point, but that is not right now
// I think this is for cue shooting

using UnityEngine;
using System.Collections;

[AddComponentMenu("Menu/Slider3D")]
[RequireComponent(typeof(Button3D))]
public class Slider3D : Slider
{
    public TextMesh text;

	new void Awake ()
	{
		base.Awake();
		if(text)
		text.text = Value.ToString(format);
	}
	new void Update ()
	{
	   base.Update();
	   if(canMove && text)
	   text.text = Value.ToString(format);
	}
}
