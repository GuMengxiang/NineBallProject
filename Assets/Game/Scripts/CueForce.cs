// I don't have time to look into this yet.
// If we have needs, we can try to improve upon this later.

using UnityEngine;
using System.Collections;

public class CueForce : MonoBehaviour 
{
	public SliderSprite slider;
	[SerializeField]
	private MeshRenderer mesr;
	public float cueForceValue = 0.0f;
	[SerializeField]
	private float startValue = 1.0f;
	[SerializeField]
	private CueController cueController;

	void Awake ()
	{
		if(MenuControllerGenerator.controller)
		{
            // transform.parent.gameObject.SetActive(false); // this disables the current cue force selector
            
			slider.MoveSlider += MoveForceSlider;
			slider.CheckSlider += MoveForceSlider;
		}
	}

	void Start ()
	{
		slider.Value = startValue;
		MoveForceSlider (slider);
        cueController.cueForceisActive = false;
	}

	void MoveForceSlider (Slider slider)
	{
		if(!cueController.allIsSleeping)
        {
            return;
        }

        if (cueController.NetworkSlaveInControl() || cueController.BotInControl() || cueController.NetworkBotInControl())
        {
            return;
        }

		cueController.cueForceisActive = true;
		cueForceValue = slider.Value;
		transform.localScale = new Vector3(slider.Value, 1.0f, 1.0f);
		mesr.sharedMaterial.SetTextureScale("_MainTex", new Vector2(1.0f, slider.Value));

		cueController.cueDisplacement = cueController.cueMaxDisplacement * cueForceValue;
	}

	public void Reset()
	{
		StartCoroutine(WaitAndResetValue());
	}

	IEnumerator WaitAndResetValue ()
	{
		yield return new WaitForEndOfFrame();
		print("Reset Value "+ cueForceValue);
		slider.Value = startValue;
		slider.Reset();
		transform.localScale = new Vector3(slider.Value, 1.0f, 1.0f);
		mesr.sharedMaterial.SetTextureScale("_MainTex", new Vector2(1.0f, slider.Value));
	}
}
