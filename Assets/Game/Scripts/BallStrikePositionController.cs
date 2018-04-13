// this class is for figuring out where do we want to hit on the cue ball
// we will replace this with a better UX

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallStrikePositionController : MonoBehaviour 
{
	[SerializeField]
	public CircularSlider circularSlider;
	[SerializeField]
    public CueController cueController;

    [System.NonSerialized]
    public float radius = 1.0f;
    [System.NonSerialized]
    private Vector3 strPosition = Vector3.zero;

    void Start()
	{
		circularSlider.CircularSliderPress += SlideBallPivot;
		strPosition = transform.position;
	}
	
	void SlideBallPivot (CircularSlider circularSlider)
	{
		if(cueController.NetworkSlaveInControl())
        {
            return;
        }

        GameManager_script.Instance().CanControlCue = false;

		transform.localPosition = new Vector3(-circularSlider.displacementZ, circularSlider.displacementX, 0.0f);
		float distance = Vector3.Distance(transform.position, strPosition);

		if(distance > radius)
		{
            transform.position -= (distance - radius) * (transform.position - strPosition).normalized;
		}
	}

	public void SetPosition (Vector3 position)
	{
		transform.position = position;
	}

	public void Reset ()
	{
		transform.position = strPosition;
	}
}
