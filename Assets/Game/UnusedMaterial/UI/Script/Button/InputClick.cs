using UnityEngine;
using System.Collections;

public class InputClick : MonoBehaviour
{
    public UIInput mInput;
    public UILabel InputLabel;
    public UISprite Background;

    // Use this for initialization
    void Start()
    {
        mInput = GetComponent<UIInput>();
        mInput.label.maxLineCount = 1;
    }

    public void OnSubmit()
    {
        if (mInput.label.text != null)
        {
            // It's a good idea to strip out all symbols as we don't want user input to alter colors, add new lines, etc
            string text = NGUIText.StripSymbols(mInput.value);

            if (!string.IsNullOrEmpty(text))
            {
                mInput.label.text = "";
                mInput.value = text;
                mInput.isSelected = false;

                if (GameManager_script.Instance().isEverythingFocusedOnFrdsSelector)
                {
                    mInput.value = GameManager_script.Instance().CharLength(text, GameManager_script.Instance().Max_Key_Length);

                    GameManager_script.Instance().SelectorFrdKey = mInput.value;
                    mInput.defaultText = mInput.value;
                }
                else
                {
                    mInput.value = GameManager_script.Instance().CharLength(text, GameManager_script.Instance().Max_Name_Length);

                    GameManager_script.Instance().First_Name = mInput.value;
                    mInput.defaultText = mInput.value;

                    PlayerPrefs.SetString("First_Name", mInput.value);
                }
            }
        }
    }

    public void OnClick()
    {
        mInput.value = mInput.value;
    }
}
