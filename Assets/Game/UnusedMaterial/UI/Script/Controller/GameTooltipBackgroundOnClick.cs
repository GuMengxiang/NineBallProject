using UnityEngine;
using System.Collections;

public class GameTooltipBackgroundOnClick : MonoBehaviour
{
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
}
