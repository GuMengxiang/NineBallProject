using UnityEngine;
using System.Collections;

public class UIOKButton : MonoBehaviour
{
    void OnClick()
    {
        // try to disable the tutorial page and show the normal page
        GameManager_script.Instance().ChangeFrdsSelectorFtueAbsolutePath();

        // music
        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
    }
}
