using UnityEngine;
using System.Collections;

public class LoadingText : MonoBehaviour
{
    public UIPanel RootPanel;
    public UILabel MainText;
    public UILabel mLabel;

    public static int charOffset = 4;

    [System.NonSerialized]
    public string Mainstring;
    [System.NonSerialized]
    string mText;
    [System.NonSerialized]
    int mOffset = 0;
    [System.NonSerialized]
    float mNextChar = 0f;
    [System.NonSerialized]
    bool mReset = true;
    [System.NonSerialized]
    int textOffsetDifference;
    [System.NonSerialized]
    public int charsPerSecond = 3;
    [System.NonSerialized]
    public bool needToShrink = false;

    void Start()
    {
        Mainstring = GameManager_script.Instance().connectionStatus;

        MainText.text = GameManager_script.Instance().connectionStatus;

        needToShrink = ((float)Screen.width / (float)Screen.height) < 1.35f && (GameManager_script.Instance().Savelanguage == "Français" || GameManager_script.Instance().Savelanguage == "Español");

        if (needToShrink)
        {
            MainText.fontSize = (int)(Screen.height / 30.5f);

            gameObject.transform.localPosition = new Vector3(MainText.width * -0.5f, RootPanel.height * -0.5f + MainText.height * 2.00f, 0.0f);
        }
        else
        {

            MainText.fontSize = (int)(Screen.height / 26.0f);

            gameObject.transform.localPosition = new Vector3(MainText.width * -0.5f, RootPanel.height * -0.5f + MainText.height * 1.80f, 0.0f);
        }

        mReset = true;
    }

    void Update()
    {
        if (mReset)
        {
            MainText.text = GameManager_script.Instance().connectionStatus;

            mReset = false;
            mLabel = MainText;
            mText = mLabel.processedText;
            mOffset = mText.Length - charOffset;

            if (needToShrink)
            {
                gameObject.transform.localPosition = new Vector3(MainText.width * -0.5f, RootPanel.height * -0.5f + MainText.height * 2.00f, 0.0f);
            }
            else
            {
                gameObject.transform.localPosition = new Vector3(MainText.width * -0.5f, RootPanel.height * -0.5f + MainText.height * 1.80f, 0.0f);
            }
        }

        if (MainText.text.Length > GameManager_script.Instance().connectionStatus.Length - charOffset)
        {
            if (MainText.text.Length > 0 && GameManager_script.Instance().connectionStatus.Substring(0, GameManager_script.Instance().connectionStatus.Length - charOffset) != MainText.text.Substring(0, GameManager_script.Instance().connectionStatus.Length - charOffset))
            {
                int textOffsetDifference = GameManager_script.Instance().connectionStatus.Length - MainText.text.Length;

                mOffset += textOffsetDifference;

                mReset = true;

                MainText.text = GameManager_script.Instance().connectionStatus;
            }
        }

        if (mOffset < mText.Length && mNextChar <= RealTime.time && mText.Length > 0)
        {
            // Periods and end-of-line characters should pause for a longer time.
            float delay = 1.0f / charsPerSecond;
            char c = mText[Mathf.Clamp(mOffset, 0, mText.Length - 1)];

            // Automatically skip all symbols
            NGUIText.ParseSymbol(mText, ref mOffset);

            mNextChar = RealTime.time + delay;
            mLabel.text = mText.Substring(0, ++mOffset);
        }
        else if (mOffset >= mText.Length && mNextChar > RealTime.time)
        {
            mReset = true;
        }
    }
}
