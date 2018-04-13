using UnityEngine;
using System.Collections;

public class UIMenuController : MonoBehaviour
{
    public GameObject Mainplanel;
    public GameObject Gameoverplanel;
    public MenuType Menutype;

	void Start()
    {
        ChangeMenu(GameManager_script.Instance().EndOfGameWaitAndChangeTime);

        GameManager_script.Instance().EndOfGameWaitAndChangeTime = 0.0f; // stop any further waits till set again.
    }

    public void ChangeMenu(float time)
    {
        if (time == 0.0f)
        {
            Mainplanel.SetActive(true);
            Gameoverplanel.SetActive(false);

            // clean up selector profile stuff
            GameManager_script.Instance().selfGameProfileInfo = null;
            GameManager_script.Instance().otherGameProfileInfo = null;

            // send signal to friend saying we don't want to rematch
            if (ServerController.serverController)
            {
                ServerController.serverController.SendRPCToNetworkViewOthers("OnRematchConfirmationReceived", false);
            }

            // clean up rematch consts
            GameManager_script.Instance().CleanUpRematchConsts();

            // it is time to hide the room for all other players forever
            GameManager_script.KillRoomAndDisconnect();

            // for photon view bug?
            GameManager_script.DestroyServerController();

            // when the main panel is up, we can start doing all the rtl related stuffz (including the daily bonus)
            GameManager_script.Instance().InMenuAndAtMainPage = true;
        }
        else
        {
            Mainplanel.SetActive(false);
            Gameoverplanel.SetActive(true);

            // we gotta make sure we stop all the rtl updates right here
            GameManager_script.Instance().InMenuAndAtMainPage = false;
        }

        StartCoroutine(WaitAndChange(time));
    }

    IEnumerator WaitAndChange(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Mainplanel.SetActive(true);
        Gameoverplanel.SetActive(false);

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

        // when the main panel is up, we can start doing all the rtl related stuffz (including the daily bonus)
        GameManager_script.Instance().InMenuAndAtMainPage = true;
    }
}
