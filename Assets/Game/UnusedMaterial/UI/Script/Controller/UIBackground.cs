using UnityEngine;
using System.Collections;

public class UIBackground : MonoBehaviour
{
    public UIPanel RootPanel;
    public GameObject BackGround;
    public UISprite Logo;

    void Start() 
    {
        UI();
	}

    void UI()
    {
        float Logowith1 = RootPanel.width * 0.50f;
        float Logowith2 = RootPanel.height * 0.45f * 3.342f; // 3.342f is the actual ratio of the logo png

        if (Logowith1 < Logowith2)
        {
            Logo.width = (int)Logowith1;
        }
        else
        {
            Logo.width = (int)Logowith2;
        }

        Logo.height = (int)(Logo.width / 3.342f);
        Logo.transform.localPosition = new Vector3(Logo.transform.localPosition.x, RootPanel.height * 0.5f - Logo.height * 0.5f - RootPanel.height * 0.0625f, Logo.transform.localPosition.z);

        if (RootPanel.width / RootPanel.height > 2)
        {
            BackGround.GetComponent<UISprite>().width = (int)(RootPanel.width + 1.0f);
            BackGround.GetComponent<UISprite>().height = (int)(RootPanel.height + 1.0f);
        }
        else
        {
            BackGround.GetComponent<UISprite>().width = (int)(RootPanel.height * 2 + 1.0f); // this X2 is true because we always want to maintain the ratio of 2 with width and height
            BackGround.GetComponent<UISprite>().height = (int)(RootPanel.height + 1.0f);

            float spillOverTotalLength = RootPanel.height * 2 - RootPanel.width; // this is expressed in terms of actual screen and NOT real picture's dimension
            float spillOverOnLeft = (0.63f - 0.50f) * spillOverTotalLength; // the number 0.63 was emperically determined and only works with this specific background

            BackGround.transform.localPosition = new Vector3(BackGround.transform.localPosition.x - spillOverOnLeft, BackGround.transform.localPosition.y, BackGround.transform.localPosition.z);
        }
    }
}
