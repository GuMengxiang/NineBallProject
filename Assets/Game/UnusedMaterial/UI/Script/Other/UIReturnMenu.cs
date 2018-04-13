using UnityEngine;
using System.Collections;

public class UIReturnMenu : MonoBehaviour
{
    public GameObject[] Parent;

    void OnClick()
    {
        // clean up selector profile stuff
        GameManager_script.Instance().selfGameProfileInfo = null;
        GameManager_script.Instance().otherGameProfileInfo = null;

        // clean up rematch consts
        GameManager_script.Instance().CleanUpRematchConsts();

        // send signal to friend saying we don't want to rematch
        if (ServerController.serverController)
        {
            ServerController.serverController.SendRPCToNetworkViewOthers("OnRematchConfirmationReceived", false);
        }

        // it is time to hide the room for all other players forever
        GameManager_script.KillRoomAndDisconnect();

        // for photon view bug?
        GameManager_script.DestroyServerController();

        // donno what this is
        for (int i = 0; i < Parent.Length; i++)
        {
            if (Parent[i])
            {
                Parent[i].SetActive(false);
            }
        }

        // kill sound
        GameManager_script.Instance().StartCoroutine(GameManager_script.Instance().KillLookingForPlayerMusicAndPlayFoundOppoDing(false));

        // play sound
        GameManager_script.Instance().PlaySound((int)MusicClip.Button_Click, false, 1.0f);

        // clear popup
        GameManager_script.Instance().PopupCurrentlyVisible = false;

        // no network gamez
        GameManager_script.Instance().NetworkGameSceneCurrentLoad = false;

        // load main menu
        Application.LoadLevel("NGUI_MENU");
    }
}
