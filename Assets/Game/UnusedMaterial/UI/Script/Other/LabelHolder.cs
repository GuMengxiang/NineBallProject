using UnityEngine;
using System.Collections;

public class LabelHolder : MonoBehaviour
{
    public UISprite Parent;

    public GameObject LabelPrefab;
    private GameObject GroupLabel;

    public UISprite FtueBox;
    public UISprite FtueArrow;
    public UILabel FtueActualText;

    [System.NonSerialized]
    public bool FtueIsShown = false;
    [System.NonSerialized]
    public float FtueIsShownCounter = 0.0f;

    public void ShowNewLabel(string inText)
    {
        if (GroupLabel != null)
        {
            Destroy(GroupLabel);
        }

        GroupLabel = (GameObject)Instantiate(LabelPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);

        GroupLabel.transform.parent = gameObject.transform;

        GroupLabel.GetComponent<Label>().GetComponent<UISprite>().width = (int)(Parent.width * 0.55f);
        GroupLabel.GetComponent<Label>().GetComponent<UISprite>().height = (int)(Parent.width * 0.55f);

        GroupLabel.transform.localPosition = new Vector3(Parent.width * -0.1890f, Parent.height * 0.0865f, 0.0f); // final destination
        GroupLabel.transform.localScale = new Vector3(1, 1, 1);

        GroupLabel.GetComponent<Label>().labeltext = Localization.Get(inText);
    }

    public void HideNewLabel()
    {
        if (GroupLabel != null)
        {
            Destroy(GroupLabel);
        }
    }

    public void TuckAwayFtueRelatedStuffz()
    {
        FtueIsShown = false;

        if (FtueBox)
        {
            FtueBox.transform.localPosition = new Vector3(9999.0f, 9999.0f, 9999.0f);
            FtueBox.alpha = 0.0f;
        }

        if (FtueArrow)
        {
            FtueArrow.transform.localPosition = new Vector3(9999.0f, 9999.0f, 9999.0f);
            FtueArrow.alpha = 0.0f;
        }

        if (FtueActualText)
        {
            FtueActualText.transform.localPosition = new Vector3(9999.0f, 9999.0f, 9999.0f);
            FtueActualText.alpha = 0.0f;
        }
    }

    public void InitializeFtueRelatedStuffz(string inText)
    {
        FtueBox.width = (int)(Parent.width * 1.30f);
        FtueBox.height = (int)(FtueBox.width * 0.25f);
        FtueBox.transform.localPosition = new Vector3(0.15f * Parent.width, -0.80f * FtueBox.height - Parent.height * 0.5f, 0.0f);
        FtueBox.transform.localScale = Vector3.one;
        FtueBox.alpha = 1.0f;

        FtueArrow.width = (int)(FtueBox.width * 0.175f);
        FtueArrow.height = (int)(FtueArrow.width * 0.550f);
        FtueArrow.transform.localRotation = Quaternion.Euler(0, 0, 180);
        FtueArrow.transform.localPosition = FtueBox.transform.localPosition + new Vector3(-0.20f * FtueBox.width, 0.650f * FtueBox.height, 0.0f);
        FtueArrow.transform.localScale = Vector3.one;
        FtueArrow.alpha = 1.0f;

        FtueActualText.width = (int)(FtueBox.width * 0.95f);
        FtueActualText.height = (int)(FtueBox.height * 0.90f);
        FtueActualText.transform.localPosition = FtueBox.transform.localPosition;
        FtueActualText.transform.localScale = Vector3.one;
        FtueActualText.text = Localization.Get(inText);
        FtueActualText.alpha = 1.0f;

        FtueIsShown = true;
        FtueIsShownCounter = 0.0f;
    }

    void Update()
    {
        if (FtueIsShown)
        {
            FtueIsShownCounter += Time.deltaTime;

            if (FtueIsShownCounter > 4.5f)
            {
                float lerp = FtueIsShownCounter % 4.5f;

                if (lerp < 2.25f)
                {
                    float lerpAlpha = 2.25f - lerp;

                    FtueBox.alpha = lerpAlpha;
                    FtueArrow.alpha = lerpAlpha;
                    FtueActualText.alpha = lerpAlpha;
                }
                else
                {
                    float lerpAlpha = lerp - 2.25f;

                    FtueBox.alpha = lerpAlpha;
                    FtueArrow.alpha = lerpAlpha;
                    FtueActualText.alpha = lerpAlpha;
                }
            }
        }
    }
}
