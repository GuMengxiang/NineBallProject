using UnityEngine;
using System.Collections;

public class UIViolator : MonoBehaviour
{

    public UISprite Background;
    public UILabel Text;
    public int NewsNumber;
    public int Backgorundwidth;

    void Start()
    {
        UI();
    }

    void UI()
    {
        Background.width = Backgorundwidth;
        Background.height = Backgorundwidth;
        Text.width = (int)(Background.width / 1.333f);
        Text.height = (int)(Background.height / 1.333f);
        Text.text = "" + NewsNumber;
        Text.transform.localPosition = new Vector3(Text.transform.localPosition.x, Text.transform.localPosition.y - Text.height * 0.075f, Text.transform.localPosition.z);
    }
}
