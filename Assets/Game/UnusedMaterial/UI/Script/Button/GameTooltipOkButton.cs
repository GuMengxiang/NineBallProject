﻿using UnityEngine;
using System.Collections;

public class GameTooltipOkButton : MonoBehaviour
{
    public GameObject UI;
    public bool fadeOutAnimation = false;

    public void OnClick()
    {
        GameManager_script.PushOutConfirmState = ConfirmationType.confirmed;

        // dajiang hack, this is so fucking ugly...
        UI = transform.parent.parent.parent.gameObject;

        // ok we can animate this stuff...
        if (UI && UI.GetComponent<UIPanel>())
        {
            UI.GetComponent<UIPanel>().alpha = 1.0f;

            fadeOutAnimation = true;
        }

        // play them soundz
        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
    }

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            GameManager_script.Instance().DownOnRealButtons = true;
        }
        else
        {
            GameManager_script.Instance().DownOnRealButtons = false;
        }
    }

    public void Update()
    {
        if (fadeOutAnimation && UI && UI.GetComponent<UIPanel>())
        {
            // now this is just PLAIN WEIRD
            // WTF is this!!!!! WTFFFFFF!!!! Why are we using -1.0f for alpha? Where does that even make sense?????
            UI.GetComponent<UIPanel>().alpha = Mathf.Lerp(UI.GetComponent<UIPanel>().alpha, -1.0f, Time.deltaTime * 10.0f);

            if (UI.GetComponent<UIPanel>().alpha < 0.01f)
            {
                fadeOutAnimation = false;

                UI.SetActive(false); // Destroy(UI);
            }
        }
    }
}
