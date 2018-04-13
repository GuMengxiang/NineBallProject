// This function is actually the cue runner, very important class

using UnityEngine;
using System.Collections;

public class CueControllerUpdater : MonoBehaviour 
{
	[SerializeField]
	private CueController cueController; // already set in unity 

    public const int CUE_BALL_MOVABLE_ON_TABLE = 1;
    public const int CUE_BALL_FIXED_ON_TABLE = 2;
    public const int CUE_BALL_IN_HAND = 3;

    public static int current_control_status = CUE_BALL_MOVABLE_ON_TABLE;

    // don't use fixed updates here because this is more about control than physics.
    public void Update()
    {
        cueController.allIsSleeping = cueController.CheckAllBallSleepingStatusAnyTime(); // cueController.CheckSleepingStatusAnyTime();

        if (current_control_status == CUE_BALL_MOVABLE_ON_TABLE)
        {
            cueController.BallMovableOnTable();
        }
        else if (current_control_status == CUE_BALL_IN_HAND)
        {
            cueController.BallInHand();
        }
        else if (current_control_status == CUE_BALL_FIXED_ON_TABLE)
        {
            cueController.BallFixedOnTable();
        }

        if (cueController.allIsSleeping)
        {
            // this controls tiny movements in the cue relative to the cue ball. I will find another place for it later
            cueController.OnControlCue();

            // this controls all the bot bullshits
            cueController.RobotUpdate();
        }
    }
}
