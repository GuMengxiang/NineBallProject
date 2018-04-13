// I will replace all elements with NGUI at some point, but that is not right now
// I think this is for cue shooting

using UnityEngine;
using System.Collections;

[AddComponentMenu("Menu/Slider Sprite")]
[RequireComponent(typeof(ButtonSprite))]
public class SliderSprite : Slider 
{
	private Vector3 strPosition;
	[SerializeField]
	private bool hideSliderObjectThenUp = false;
	[SerializeField]
	private bool resetValue = false;
	[SerializeField]
	private float valueToReset = 0.0f;

	new void Awake ()
	{
		base.Awake();

        strPosition = slideObject.localPosition;

		if(hideSliderObjectThenUp)
        {
            slideObject.GetComponent<Renderer>().enabled = canMove;
        }
	}

	new void Update ()
	{
		base.Update();

        if(hideSliderObjectThenUp)
        {
            slideObject.GetComponent<Renderer>().enabled = canMove;
        }

		if(menu.GetButtonUp())
		{
			if(resetValue)
			{
                slideObject.position = strPosition;
				Value = valueToReset;
			}
		}
	}
}
