// we need to add a few more functions in here for a complete set of interface

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerController : Photon.MonoBehaviour
{
    public static ServerController serverController;

    public ServerMessenger serverMessenger;

    [System.NonSerialized]
    public PhotonView myPhotonView = null;

    [System.NonSerialized]
    public bool playerInControl = false;

    [System.NonSerialized]
    public bool NetworkGameStarted = false;

    [System.NonSerialized]
    public float lastMessageTimeStamp = 0.0f;

    void Awake()
    {
        // view id
        myPhotonView = gameObject.AddComponent<PhotonView>();
        myPhotonView.viewID = 1; // this is the general rpc photonview id, 1 is the viewID.

        lastMessageTimeStamp = Time.realtimeSinceStartup;
    }

    // helper function
    public void SendRPCToNetworkViewOthers(string message, params object[] args)
    {
        // has to make sure we are NOT in network bot mode, coz we are not supposed to send anything in that mode
        if (myPhotonView && !GameManager_script.Instance().SmartBotInActionGame)
        {
            myPhotonView.RPC(message, PhotonTargets.Others, args); // PhotonTargets.All
        }

        lastMessageTimeStamp = Time.realtimeSinceStartup;
    }

    // (I) This is a very critical info but doesn't really give players a lot of nasty problems if it fails to get through
    [PunRPC]
    public void OnPersonalInfoReceived(string inName, int inCue, int inAvatar, int starColor, int starNumber, float TPAScore, int gamesPlayed, int gamesWon, float inCoins, int inStreak)
    {
        serverMessenger.OnPersonalInfoReceived(inName, inCue, inAvatar, starColor, starNumber, TPAScore, gamesPlayed, gamesWon, inCoins, inStreak);

        lastMessageTimeStamp = Time.realtimeSinceStartup;
    }

    // (I) This is a very critical info but doesn't really give players a lot of nasty problems if it fails to get through
    [PunRPC]
    public void OnFriendPersonalInfoReceived(string inName, int inCue, int inAvatar, int starColor, int starNumber, float TPAScore, int gamesPlayed, int gamesWon, float inCoins, int inStreak, float inWager, int inTableTexture)
    {
        serverMessenger.OnFriendPersonalInfoReceived(inName, inCue, inAvatar, starColor, starNumber, TPAScore, gamesPlayed, gamesWon, inCoins, inStreak, inWager, inTableTexture);

        lastMessageTimeStamp = Time.realtimeSinceStartup;
    }

    // (P) this is the (P) part of cue activity, updating local rotation and positions
    // this function controls the cue movement on the other side. this is also an important and frequently called function
    [PunRPC]
    public void OnUpdateCueControl(float cueNumber, bool isinHand, Vector3 physicalPosition, Quaternion localRotation, Vector3 localPosition, Vector3 rotationDisplacement)
    {
        serverMessenger.OnUpdateCueControl(cueNumber, isinHand, physicalPosition, localRotation, localPosition, rotationDisplacement);

        lastMessageTimeStamp = Time.realtimeSinceStartup;
    }

    // (P + I) this function currently only updates while all balls are moving as well as the one time immediate update when we shoot cue.
    // this function should have shot #, time lapsed, pocketed, position, velocity, angular.
    // this function updates ALL ball positions
    [PunRPC]
    public void OnUpdateAllBallStatus(int inShotCount, float inTimeLapsed, bool inMandatory, bool[] inPocketed, Vector3[] inPosition, Vector3[] inVelocity, Vector3[] inAngular, Vector3 ballShotVelocity, Vector3 hitBallVelocity, Vector3 ballShotAngularVelocity)
    {
        serverMessenger.OnUpdateAllBallStatus(inShotCount, inTimeLapsed, inMandatory, inPocketed, inPosition, inVelocity, inAngular, ballShotVelocity, hitBallVelocity, ballShotAngularVelocity);

        lastMessageTimeStamp = Time.realtimeSinceStartup;
    }

    // (I) this function changes the control, we need this as an anchor.
    // However, losing this signal is kinda critical, it would make the game kinda weird
	[PunRPC]
    public void OnStatusUpdateStillPositions(int inShotCount, bool inPlayerInControl, bool inPushoutable, bool inSkippable, int inStatus, bool[] inPocketed, Vector3[] inPositions, bool inFirstTime)
	{
        serverMessenger.OnStatusUpdateStillPositions(inShotCount, inPlayerInControl, inPushoutable, inSkippable, inStatus, inPocketed, inPositions, inFirstTime);

        lastMessageTimeStamp = Time.realtimeSinceStartup;
    }

    // (I) this function needs to be expanded to accomodate the needs of many different end game scenarios
    [PunRPC]
    public void OnGameFinished(bool inSelfWin)
    {
        serverMessenger.OnGameFinished(inSelfWin);

        lastMessageTimeStamp = Time.realtimeSinceStartup;
    }

    // (I) this function kind of transmits the foul and push and skip states between things
    // However, this is for display purpose only, don't use this anywhere else
    // If it fails to get through sometimes, its totally OK
    [PunRPC]
    public void OnHelpfulTipReceived(string inTip)
    {
        serverMessenger.OnHelpfulTipReceived(inTip);

        lastMessageTimeStamp = Time.realtimeSinceStartup;
    }

    // (I) this function, or this series of functions, tell users about ways to handle rematch requests and communications
    // this one handles slave and master initial functionality acknowledgement, to and fro sharing the same function
    [PunRPC]
    public void OnRematchCapabilityCheckReceived(string inKey)
    {
        serverMessenger.OnRematchCapabilityCheckReceived(inKey);

        lastMessageTimeStamp = Time.realtimeSinceStartup;
    }

    // (I) this function, or this series of functions, tell users about ways to handle rematch requests and communications
    // if you and your opponent are eligible for rematch and click the rematch button, you will send this to your opponent
    // whenever the condition of both clicking rematch button and receiving this key are met, you start searching for the room
    [PunRPC]
    public void OnRematchConfirmationReceived(bool inWantToRematch)
    {
        serverMessenger.OnRematchConfirmationReceived(inWantToRematch);

        lastMessageTimeStamp = Time.realtimeSinceStartup;
    }

    // (I) This is a very critical info but doesn't really give players a lot of nasty problems if it fails to get through
    [PunRPC]
    public void OnRematchPersonalInfoReceived(string inName, int inCue, int inAvatar, int starColor, int starNumber, float TPAScore, int gamesPlayed, int gamesWon, float inCoins, int inStreak)
    {
        serverMessenger.OnRematchPersonalInfoReceived(inName, inCue, inAvatar, starColor, starNumber, TPAScore, gamesPlayed, gamesWon, inCoins, inStreak);

        lastMessageTimeStamp = Time.realtimeSinceStartup;
    }

    // (I) This is chat, sort of critical lolz
    [PunRPC]
    public void OnChatMessageReceived(string inString)
    {
        serverMessenger.OnChatMessageReceived(inString);

        lastMessageTimeStamp = Time.realtimeSinceStartup;
    }
}
