using UnityEngine;
using System.Collections;

public class ChangeButton : MonoBehaviour
{
    public GameObject Other;
    public ClickType Clicktype;
	
    public void OnClick()
    {
        gameObject.SetActive(false);

        Other.SetActive(true);

        if (transform.parent && transform.parent.GetComponent<SettingButton>())
        {
            // dajiang hack, every time we change the order of things, we need to change this.
            int id = transform.parent.GetComponent<SettingButton>().buttonId;

            switch (id)
            {
                case 3:
                    if (Clicktype == ClickType.Disable)
                    {
                        GameManager_script.Instance().SoundEnabled = "d";
                        PlayerPrefs.SetString("SoundEnabled", GameManager_script.Instance().SoundEnabled);
                    }
                    if (Clicktype == ClickType.Enable)
                    {
                        GameManager_script.Instance().SoundEnabled = "e";
                        PlayerPrefs.SetString("SoundEnabled", GameManager_script.Instance().SoundEnabled);

                        // play a sound just to make sure
                        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);
                    }
                    break;
                case 4:
                    if (Clicktype == ClickType.Disable)
                    {
                        GameManager_script.Instance().VibeEnabled = "d";
                        PlayerPrefs.SetString("VibeEnabled", GameManager_script.Instance().VibeEnabled);
                    }
                    if (Clicktype == ClickType.Enable)
                    {
                        GameManager_script.Instance().VibeEnabled = "e";
                        PlayerPrefs.SetString("VibeEnabled", GameManager_script.Instance().VibeEnabled);

                        // vibrate just to make sure
                        GameManager_script.Instance().Vibrate();
                    }
                    break;
            }
        }

        print("ClickType " + Clicktype); // this means the click type was just clicked
    }
}
