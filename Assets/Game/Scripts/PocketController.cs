// this class needs a careful look over
// I haven't had time to take a look at this yet but it seems like a lot of it can be reworked or removed

using UnityEngine;
using System.Collections;

public class PocketController : MonoBehaviour
{
    private CueController cueController;

    [SerializeField]
    private PocketController[] neighbors;

    public AnimationSpline finalBallSpline;
    [SerializeField]
    private Transform[] finalNodes;
    [System.NonSerialized]
    public float splineTotalLength = 0.0f;
    [System.NonSerialized]
    public float splineLength = 0.0f;
    [System.NonSerialized]
    public float splineCurrentLength = 0.0f;
    
    public AnimationSpline initialBallSpline;
    [SerializeField]
    private Transform[] initialNodes;
    [System.NonSerialized]
    public float initialSplineLength = 0.0f;

    public int id = 1; // ids are 1 2 3 4 5 6, so be careful

    void Awake()
    {
        for (int j = 0; j < initialNodes.Length; j++)
        {
            initialNodes[j].GetComponent<Renderer>().enabled = false;
        }

        initialBallSpline = new AnimationSpline(WrapMode.Clamp);

        for (int i = 0; i < finalNodes.Length; i++)
        {
            finalNodes[i].GetComponent<Renderer>().enabled = false;
        }

        finalBallSpline = new AnimationSpline(WrapMode.Clamp);

        cueController = CueController.FindObjectOfType(typeof(CueController)) as CueController;

        GetComponent<Renderer>().enabled = false;
    }

    void Start()
    {
        if (initialNodes.Length > 0)
        {
            initialBallSpline.CreateSpline(initialNodes, true, false, false, 2.0f * cueController.ballRadius);

            initialSplineLength = initialBallSpline.splineLength;
        }

        if (finalNodes.Length > 0) // there are a whole bunch of them, should simplify
        {
            finalBallSpline.CreateSpline(finalNodes, true, false, false, 2.0f * cueController.ballRadius);

            splineTotalLength = finalBallSpline.splineLength;
            splineLength = finalBallSpline.splineLength;
        }
    }

    public static Vector3 FindeHolePositionById(int holeId)
    {
        return GameManager_script.RobotHolePositions[holeId];
    }

    public static PocketController FindeHoleById(int holeId)
    {
        foreach (PocketController item in FindObjectsOfType(typeof(PocketController)) as PocketController[])
        {
            if (item.id == holeId)
            {
                return item;
            }
        }

        return null;
    }

    // I have no idea what does this do and why is it here. But I know without it we will have a bad time with rolling balls down the pockets
    public bool haveNeighbors(PocketController pocketController)
    {
        if (!pocketController)
        {
            return false;
        }

        foreach (PocketController item in neighbors)
        {
            if (pocketController == item)
            {
                return true;
            }
        }

        return false;
    }

    // I have no idea what does this do and why is it here. But I know without it we will have a bad time with rolling balls down the pockets
    public void DecreaseSplineLength()
    {
        splineCurrentLength = splineLength;
        splineLength -= 2.0f * cueController.ballRadius;

        for (int i = 0; i < neighbors.Length; i++)
        {
            neighbors[i].splineCurrentLength = splineLength;
            neighbors[i].splineLength -= 2.0f * cueController.ballRadius;
        }
    }

    // I have no idea what does this do and why is it here. But I know without it we will have a bad time with rolling balls down the pockets
    public void IncreaseSplineLength()
    {
        splineLength += 2.0f * cueController.ballRadius;

        for (int i = 0; i < neighbors.Length; i++)
        {
            neighbors[i].splineLength += 2.0f * cueController.ballRadius;
        }
    }

    // this will be called on master as well as slave on a ball pocket, this is when ball triggers the box collider
    void OnTriggerEnter(Collider other)
    {
        BallController ballController = other.GetComponent<BallController>();

        if (ballController)
        {
            if (!ballController.ballIsPocketed)
            {
                // choose pocket sound
                float b_p_volume = Mathf.Clamp01(ballController.GetComponent<Rigidbody>().velocity.magnitude / cueController.ballMaxVelocity);
                int b_p_index = Random.Range((int)MusicClip.B_Pocket_0, (int)MusicClip.B_Pocket_4 + 1);

                // play pocket sound
                GameManager_script.Instance().PlaySound(b_p_index, false, b_p_volume);

                StartCoroutine(DelayPlayingRollUnderTableSound(ballController));

                ballController.finalAnimationPlacement = 0.0f;
                ballController.initialAnimationPlacement = 0.0f;
                DecreaseSplineLength();

                ballController.ballIsPocketed = true;
                ballController.OnSetHoleSpline(splineCurrentLength, initialSplineLength, id);

                OnBallPocketedHouseKeeping(ballController);
            }
        }
    }

    IEnumerator DelayPlayingRollUnderTableSound(BallController bc)
    {
        yield return new WaitForSeconds(0.5f); // just ball parking this...

        // choose roll under table sound
        float b_r_volume = Mathf.Clamp01(1.0f);
        int b_r_index = Random.Range((int)MusicClip.B_Roll_Under, (int)MusicClip.B_Roll_Under + 1);

        // play roll under table sound
        bc.rollUnderTableSound = GameManager_script.Instance().PlaySound(b_r_index, false, b_r_volume);
    }

    // calls after a ball has completely been played. For cueball and most of 9 ball, we reset it back up. For object balls, we screw it!
    // there should be more complicated logics that does this. So modify this later
    public void OnBallPocketedHouseKeeping(BallController ballController)
    {
        // cue ball should always be spotted
        if (ballController.isMain)
        {
            // set flags to indicate ball is pocketed
            cueController.pocketedTheCueBall = true;
            cueController.OnCueBallIsPocketed(true);
        }
        else if (ballController.id == 9)
        {
            // set flags to indicate ball is pocketed
            ballController.ballIsPocketed = true;
            cueController.pocketedNineBall = true;
            cueController.pocketedAnyObjectBall = true;

            // remove it from currentBallControllers
            cueController.currentBallControllers.Remove(ballController); // don't change this. Because nearest object ball depends on this. Talk with DJ before proceed.
            cueController.currentBallControllers.TrimExcess();
        }
        else
        {
            // set flags to indicate ball is pocketed
            ballController.ballIsPocketed = true;
            cueController.pocketedAnyObjectBall = true;

            // remove it from currentBallControllers
            cueController.currentBallControllers.Remove(ballController); // don't change this. Because nearest object ball depends on this. Talk with DJ before proceed.
            cueController.currentBallControllers.TrimExcess();
        }

        GameManager_script.IncrementBallPocketed(cueController.TrackingShotAsPlayerOne, cueController.TrackingShotAsPlayerTwo);

        cueController.pocketedBallControllers.Add(ballController);
    }
}
