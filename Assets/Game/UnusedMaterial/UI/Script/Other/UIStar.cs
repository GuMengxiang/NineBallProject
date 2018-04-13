using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIStar : MonoBehaviour
{
    public StarType starType;
    public UISprite Background;
    public UILabel Text;

    public StarInfo info;
	
    public void StartUI(StarInfo vStar)
    {
        info = vStar;

        changeImage(vStar.starType);

        changeText("" + vStar.text);

        changeTextPositionSlightly();
    }

    public void changeImage(StarType VstarType)
    {
        starType = VstarType;

        Background.spriteName = "" + starType;
    }

    public void changeTextPositionSlightly()
    {
        float xOffSet = 0.0f;

        // star based tweaks
        if (starType == StarType.bronze)
        {
            xOffSet += 0.0000f;
        }
        else if (starType == StarType.silver)
        {
            xOffSet += 0.0150f * Text.width;
        }
        else if (starType == StarType.gold)
        {
            xOffSet += 0.0110f * Text.width;
        }
        else if (starType == StarType.red)
        {
            xOffSet += 0.0050f;
        }

        // level based tweaks
        if (info.text > 99 && info.text <= 999) // dajiang hack, right now we only have level 100, if we do more levels, we need to modify this as well
        {
            xOffSet += -0.0505f * Text.width;
        }
        else if (info.text > 19 && info.text <= 99 && info.text % 10 == 1) // these are 21, 31, 41, 51, 61, 71, 81, 91 and we need to move it to the right ever so slightly
        {
            xOffSet += +0.0145f * Text.width;
        }
        else if (info.text > 9 && info.text <= 19) // these are 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 and we need to move it to the left quite a bit
        {
            xOffSet += -0.0185f * Text.width;
        }
        else if (info.text > 1 && info.text <= 9) // these are 2, 3, 4, 5, 6, 7, 8, 9 and we need to move it to the left quite a bit
        {
            xOffSet += -0.0125f * Text.width;
        }
        else if (info.text == 1)
        {
            xOffSet += -0.0195f * Text.width;
        }

        // change position
        Text.transform.localPosition += new Vector3(xOffSet, 0.0f, 0.0f);
    }

    public void changeText(string text)
    {
        Text.text = text;

        if (starType == StarType.bronze)
        {
            Text.color = new Color(1.0f, 1.0f, 1.0f, 1);
            Text.gradientTop = new Color(0.0f, 0.0f, 0.0f, 1);
            Text.gradientBottom = new Color(0.0f, 0.0f, 0.0f, 1);
        }
        else if (starType == StarType.silver)
        {
            Text.color = new Color(0.00f, 0.00f, 0.00f, 1);
            Text.gradientTop = new Color(0.0f, 0.0f, 0.0f, 1);
            Text.gradientBottom = new Color(0.0f, 0.0f, 0.0f, 1);
            Text.effectStyle = UILabel.Effect.None;
        }
        else if (starType == StarType.gold)
        {
            Text.color = new Color(0.00f, 0.00f, 0.00f, 1);
            Text.gradientTop = new Color(0.0f, 0.0f, 0.0f, 1);
            Text.gradientBottom = new Color(0.0f, 0.0f, 0.0f, 1);
            Text.effectStyle = UILabel.Effect.None;
        }
        else if (starType == StarType.red)
        {
            Text.color = new Color(1.0f, 1.0f, 1.0f, 1);
            Text.gradientTop = new Color(0.0f, 0.0f, 0.0f, 1);
            Text.gradientBottom = new Color(0.0f, 0.0f, 0.0f, 1);
        }
    }
}
