using UnityEngine;
using System.Collections;

public class Label : MonoBehaviour
{
    public UISprite LabelImage;
    public UILabel Text;
    public string labeltext = "Lock";
    public string LabelImageName = "BlueLabel";

	void Start ()
    {
        UI();
	}
	
    void UI()
    {
        LabelImage.spriteName = LabelImageName;

        Text.text = labeltext;
        Text.transform.localEulerAngles = new Vector3(0, 0, 36.5f);
        Text.width = (int)(LabelImage.width * 0.57f); // 0.57f is measured
        Text.height = (int)(LabelImage.height * 0.32f); // 0.32f is also measured
        Text.transform.localPosition = new Vector3(LabelImage.width * -0.17f, LabelImage.height * 0.20f, 0.0f);
    }

    public void ChangeLabelImage(string name)
    {
        LabelImage.spriteName = name;
    }
}
