using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallController : MonoBehaviour 
{
    // identification of the ball
	public bool isMain = false;
	public int id = 0;
    public int pocketid = -1;
    
    // this directs the ball animation in pockets
    public float finalAnimationPlacement = 0.0f;
    public float initialAnimationPlacement = 0.0f;

    // cue controller (1 and only)
	[System.NonSerialized]
	public CueController cueController;

    // states of the ball
	[System.NonSerialized]
	public bool ballIsSelected = false;
	[System.NonSerialized]
	public bool ballIsPocketed = false;
	[System.NonSerialized]
	public bool inMove = false;
	
    // velocity expressed as a % of maximum allowed velocity. i am not sure if we should have a max velocity
	private float velocityNormalized = 0.0f;

    // this controls the hole collision works
    [System.NonSerialized]
    public PocketController pocketController;
    // this controls the animation after a ball is potted
    [System.NonSerialized]
    public AnimationSpline finalHoleSpline;
    // this is the first portion of the spline run
    [System.NonSerialized]
    public AnimationSpline initialHoleSpline;    
    // this helps with the ball animation after it is potted
	[System.NonSerialized]
	public float finalHoleSplineLength = 0.0f;
    // this is the initial spline length
	[System.NonSerialized]
    public float initialHoleSplineLength = 0.0f;
    // this too is a helper of that
    [System.NonSerialized]
    private Vector3 ballVeolociyInHole;

    // I will determine their uses later. I am not sure what are they used for
    public Vector3 reserveVelocity = Vector3.zero;
    public Vector3 reserveAngularVelocity = Vector3.zero;

    // roll under table sound handle
    public GameObject rollUnderTableSound = null;

    [System.NonSerialized]
    public float suddenStop = 0.33f; // 0.33f is fine
    [System.NonSerialized]

    public float xClamp = 0.0f;
    [System.NonSerialized]
    public float zClamp = 0.0f;

	void Awake ()
	{
        // this SERIOUSLY hurts the physics performance by a matter of 5 folds with ACTUALLY worse physics performances, so thanks but no thanks
		//rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // I don't know what this does, ex and in feels the same to me...
        GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
	}
    
    // tells you if a ball has stopped
	public bool IsSleeping()
	{
		//GetComponent<Rigidbody> ().sleepThreshold;
		return GetComponent<Rigidbody>().velocity.magnitude <= Physics.sleepThreshold&& GetComponent<Rigidbody>().angularVelocity.magnitude <=Physics.sleepThreshold;
		//Debug.Log("获得球的速度："+GetComponent<Rigidbody>().velocity.magnitude + "獲得球的角速度：" + GetComponent<Rigidbody>().angularVelocity.magnitude);
		//return GetComponent<Rigidbody>().velocity.magnitude <= 1.0f && GetComponent<Rigidbody>().angularVelocity.magnitude <= 1.0f;//Gu

	}

    // this has something to do with preparing a ball for a pocket + animation
	public void OnSetHoleSpline (float finalLength, float initialLength, int pocketId)
	{
        // house keeping
        PocketController activePocketController = PocketController.FindeHoleById(pocketId);

        finalHoleSpline = activePocketController.finalBallSpline;
        finalHoleSplineLength = finalLength;

        initialHoleSpline = activePocketController.initialBallSpline;
        initialHoleSplineLength = initialLength;

        pocketid = pocketId;
        pocketController = activePocketController;
	}

    // this is called once when the cue ball is shot
	public void OnShootCueBall()
	{
        // velocity and angular velocity
		if(!GetComponent<Rigidbody>().isKinematic)
		{
			GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(cueController.ballShotVelocity, cueController.ballMaxVelocity);
			GetComponent<Rigidbody>().angularVelocity = cueController.ballShotAngularVelocity;
		}

		reserveVelocity = GetComponent<Rigidbody>().velocity;
		reserveAngularVelocity = cueController.ballShotAngularVelocity;

        // reset the spin controller
        BallStrikePositionController BallStrikePositionController = BallStrikePositionController.FindObjectOfType(typeof(BallStrikePositionController)) as BallStrikePositionController;

        if(BallStrikePositionController)
        {
            BallStrikePositionController.Reset();
        }

        // reset cue's relative position in terms of rotation and spin
		cueController.cueBallPivotLocalPosition = Vector3.zero;
		cueController.cueRotationLocalPosition = Vector3.zero;
	}

    // this one resets the cueball's position after it was potted or something
    public void SpotBallOnTableAndHouseKeeping(Vector3 position, Vector3 velocity, Vector3 angular)
    {
        // determine if the position is valid, if not keep on trying to get the closest position possible
        GetComponent<Rigidbody>().position = cueController.givemeagoodposition(position);

        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Collider>().enabled = true;

        GetComponent<Rigidbody>().velocity = velocity;
        GetComponent<Rigidbody>().angularVelocity = angular;

        if (pocketid != -1)
        {
            PocketController pc = PocketController.FindeHoleById(pocketid);

            pc.IncreaseSplineLength();

            pocketid = -1;
        }

        GameManager_script.DecrementBallPocketed(cueController.TrackingShotAsPlayerOne, cueController.TrackingShotAsPlayerTwo);

        cueController.pocketedBallControllers.Remove(this);
        cueController.pocketedBallControllers.TrimExcess();
    }

    // this is called when a ball is potted
    // it's main purpose is to clean up some ball properties and make sure it stays put at the bottom of the rails.
	public void BallPocketedHouseKeeping (float infinalHoleSplineLength)
	{
        GetComponent<Rigidbody>().position = finalHoleSpline.Evaluate(infinalHoleSplineLength);
        GetComponent<Rigidbody>().position += cueController.ThreeDOffset;

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Collider>().enabled = false;

        for (int i = 0; i < cueController.allBallControllers.Length; i++) // pocketedBallControllers
        {
            if (cueController.allBallControllers[i].ballIsPocketed)
            {
                Vector3 currentPosition = cueController.allBallControllers[i].GetComponent<Rigidbody>().position;

                cueController.allBallControllers[i].GetComponent<Rigidbody>().position = new Vector3(GetComponent<Rigidbody>().position.x, currentPosition.y, currentPosition.z);
            }
        }

        if (rollUnderTableSound)
        {
            Destroy(rollUnderTableSound); // stop the sound
        }
    }

    // in here, we only set the reserve velocity at the very end and NEVER use it in the middle
    void FixedUpdate()
    {
        if (GameManager_script.Instance().SmartBotFreezeInPlaceFlag)
        {
            // save k state
            bool kinematicState = GetComponent<Rigidbody>().isKinematic;

            // give it a new state
            GetComponent<Rigidbody>().isKinematic = false;

            // freeze all ball in their positions
            GetComponent<Rigidbody>().position = GetComponent<Rigidbody>().position;

            // freeze all ball's velocities
            GetComponent<Rigidbody>().velocity = Vector3.zero;

            // freeze all ball's angular velocities
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            // put the state back
            GetComponent<Rigidbody>().isKinematic = kinematicState;
        }
        else
        {
            // dajiang physics 1
            // we need angular velocity and real velocity, since we are working with perfect surfaces, we can go ahead and update it ourselves.
            // we need frictions of the surface (which reacts differently to different angular and direct speeds)
            if (cueController && cueController.ballsIsCreated)
            {
                if (ballIsPocketed)
                {
                    if (!GetComponent<Rigidbody>().isKinematic && initialAnimationPlacement < initialHoleSplineLength)
                    {
                        // 20.0f is the constant speed where balls drop
                        initialHoleSpline.AnimationSlider(transform, 17.5f, ref initialAnimationPlacement, out ballVeolociyInHole, 1, false);

                        // special casing for 3D ball rolling, we just want to apply this to the final section of the roll
                        if (initialAnimationPlacement / initialHoleSplineLength > 0.5f)
                        {
                            GetComponent<Rigidbody>().position += cueController.ThreeDOffset;
                        }

                        GetComponent<Rigidbody>().velocity = ballVeolociyInHole/*球在洞里的速度*/;
                        GetComponent<Rigidbody>().angularVelocity = new Vector3(ballVeolociyInHole.z * 2.0f, ballVeolociyInHole.y * 2.0f, -ballVeolociyInHole.x * 2.0f);
                    }
                    else if (!GetComponent<Rigidbody>().isKinematic && finalAnimationPlacement < finalHoleSplineLength)
                    {
                        // 20.0f is the constant speed where balls drop
                        finalHoleSpline.AnimationSlider(transform, 17.5f, ref finalAnimationPlacement, out ballVeolociyInHole, 1, false);

                        // special casing for 3D ball rolling
                        GetComponent<Rigidbody>().position += cueController.ThreeDOffset;

                        GetComponent<Rigidbody>().velocity = ballVeolociyInHole;
                        GetComponent<Rigidbody>().angularVelocity = new Vector3(ballVeolociyInHole.z * 2.0f, ballVeolociyInHole.y * 2.0f, -ballVeolociyInHole.x * 2.0f);
                    }
                    else
                    {
                        if (GetComponent<Collider>().enabled)
                        {
                            BallPocketedHouseKeeping(finalHoleSplineLength);
                        }
                    }
                }
                else
                {
                    if (inMove && !GetComponent<Rigidbody>().isKinematic) // ball is moving
                    {
                        // do velocity and angular velocity sync check
                        Vector3 noyVelocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0, GetComponent<Rigidbody>().velocity.z);
                        Vector3 noyAngularVelocity = new Vector3(-0.5f * GetComponent<Rigidbody>().angularVelocity.z, 0, 0.5f * GetComponent<Rigidbody>().angularVelocity.x);

                        // different scores and hard gates
                        float VelocityMultiple = 0.0f; // from -1.0f to 1.0f, from completely opposite to completely synced
                        float VelocityMagnitude = 0.0f; // from 0.0f to 1.5f, from completely stationary to completely top speed

                        // 1 is when we are either completely in sync or completely opposite
                        // 0 is when one speed is 0 and the other speed is not
                        if (noyVelocity.magnitude != 0.0f || noyAngularVelocity.magnitude != 0.0f)
                        {
							//会返回区间在0~1之间的value
                            if (noyVelocity.magnitude > noyAngularVelocity.magnitude)
                            {
                                VelocityMultiple = Mathf.Clamp01(noyAngularVelocity.magnitude / noyVelocity.magnitude);
                            }
                            else
                            {
                                VelocityMultiple = Mathf.Clamp01(noyVelocity.magnitude / noyAngularVelocity.magnitude);
                            }
                        }

						// 2 speeds are countering each other, so we subtract  2个速度相互抵触，所以我们减去
                        if (Vector3.Dot(noyVelocity.normalized, noyAngularVelocity.normalized) < 0.0f)
                        {
                            VelocityMultiple = VelocityMultiple - 1.0f;
                        }

                        // clamp it
                        VelocityMultiple = Mathf.Clamp(VelocityMultiple, -1.0f, 1.0f);

                        // 1.5f when we spin at 71%. 1.3f when we completely spin to 1 side. 1.0f is when we shoot straight
                        VelocityMagnitude = Mathf.Clamp((noyVelocity.magnitude + noyAngularVelocity.magnitude) / cueController.ballMaxVelocity, 0.0f, 1.5f);

                        // we need to reconcile somethingz
						if ((noyVelocity - noyAngularVelocity).magnitude >0.1f/* 0.1f*/)
						{
							
							float reconciliationE = 0.0500f; //0.0500f// between 500 and 600

                            // reconcile more when speed is higher, till 1.2ish, then slightly decline coz the ball is plating but skidding anymore
                            reconciliationE = reconciliationE * cueController.ReconVelocityMagnitudeCurve.Evaluate(VelocityMagnitude);

							// reconcile more when multiple is closer to -1.0f  多数更接近-1.0f时，调和更多
                            reconciliationE = reconciliationE * cueController.ReconVelocityMultipleCurve.Evaluate(VelocityMagnitude);

							// preparations  准备
                            Vector3 velocityDifference = noyVelocity - noyAngularVelocity;

							// actual reconciliation 实际和解
                            noyVelocity -= velocityDifference.normalized * reconciliationE ;
							//20170712
							noyAngularVelocity += velocityDifference.normalized * 2.5f /*2.5f*/* reconciliationE; // dajiang hack, should be 2.5f, 100 trans turns into 71.5 trans and 143 ang. This means ang is 143/28.5 == 5.0 of trans, thus 2.5f since we divided it by 2.0 before
                        }

                        // we need to decay somethingz
                        if (true)
                        {
                            float decayE = 0.000100f;//0.000100f;

                            // decay more when speed is closer to 0.0f
                            decayE = decayE * cueController.DecayVelocityMagnitudeCurve.Evaluate(VelocityMagnitude);

                            // decay more when multiple is closer to -1.0f
                            decayE = decayE * cueController.DecayVelocityMultipleCurve.Evaluate(VelocityMagnitude);

                            // actual decay   //Gu,控制球的撞击后滚动的速度，（那个困扰了几个月的BUG）
                            GetComponent<Rigidbody>().velocity = this.transform.GetComponent<Rigidbody>().velocity * 0.9987f;//new Vector3(noyVelocity.x * (1.0f - decayE), GetComponent<Rigidbody>().velocity.y * (1.0f - decayE), noyVelocity.z * (1.0f - decayE));
                            GetComponent<Rigidbody>().angularVelocity = new Vector3(2.0f * noyAngularVelocity.z * (1.0f - decayE), GetComponent<Rigidbody>().angularVelocity.y * (1.0f - decayE), -2.0f * noyAngularVelocity.x * (1.0f - decayE));
                        }
                    }
                }

                // we will always do this thing no matter what? ya we do.
                if (GetComponent<Rigidbody>().velocity.magnitude < suddenStop && GetComponent<Rigidbody>().angularVelocity.magnitude < suddenStop * 2.0f) // this number needs to be greater than sleep threshold
                {
                    if (!GetComponent<Rigidbody>().isKinematic)
                    {
                        // dajiang hack, all these numbers depend on the table NEVER gets moved. So we really shouldn't be moving the table under any circumstances.
                        xClamp = GetComponent<Rigidbody>().position.z > 6.09f || GetComponent<Rigidbody>().position.z < -11.30f ? GetComponent<Rigidbody>().position.x : Mathf.Clamp(GetComponent<Rigidbody>().position.x, -18.71f, 19.05f);
                        zClamp = GetComponent<Rigidbody>().position.x > 18.60f || GetComponent<Rigidbody>().position.x < -18.26f || (GetComponent<Rigidbody>().position.x > -0.81f && GetComponent<Rigidbody>().position.x < 1.15f) ? GetComponent<Rigidbody>().position.z : Mathf.Clamp(GetComponent<Rigidbody>().position.z, -11.73f, 6.54f);

                        GetComponent<Rigidbody>().position = new Vector3(xClamp, GetComponent<Rigidbody>().position.y, zClamp);
                        GetComponent<Rigidbody>().velocity = Vector3.zero;
                        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    }
                }

                // put it in for next time a collision or something weird happens
                reserveVelocity = GetComponent<Rigidbody>().velocity;
                reserveAngularVelocity = GetComponent<Rigidbody>().angularVelocity;
            }
        }
    }

    // this function is doing awefully little, only works some sounds and work with the main ball
    // in here, we use reserve velocity all we want but NEVER set it
	void OnCollisionEnter(Collision collision)
	{
        // dajiang physics 2
        // we need angular velocity and real velocity for both items before.
        // we need frictions of the 2 contact surfaces (this is already known through the layerName).

        // reserveVelocity stuff is the backup, which I will NOT change in this function but only change in fixedupdate

        // we need to make sure the exit velocities of the 2 balls are perpendicular to each other
        // you can and do have an initial angular velocity, and in some cases it immediately matches with your real velocity

        // we can set a flag so we know if the real rigidbody.velocity is modified upon impact or not at each frame
        // if it has not been impacted, we will swap in the new values
        // if it has been impacted, we will apply the new value on top of it

        // velocity of the rigidbody right now is already calculated by unity for after the collision
        // if we want to update it any further (or in case of wall, not updated), we can do our calculation here.

        // angular is immediately reconciled for wall collisions and should be reconciles quickly for normal hits except for skidding (which can last some time).

		string layerName = LayerMask.LayerToName(collision.collider.gameObject.layer);

        // An important pre-condition
        // If we get a stationary ball to ball or ball to wall collision due to initial game setting, we need to ignore those collisions because they do crazy things
        if ((layerName == "Wall" || layerName == "Ball" || layerName == "MainBall") && collision.relativeVelocity.magnitude > 0.01f)
        {
            if (layerName == "Ball" || layerName == "MainBall") // ballllzzz
            {
                BallController otherBall = collision.collider.gameObject.GetComponent<BallController>();

                if (otherBall && otherBall.id < id)
                {
                    // sound portion for ball on ball
                    float b_b_volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / cueController.ballMaxVelocity);
                    int b_b_index = 0;

                    if (b_b_volume > 0.66f)
                    {
                        b_b_index = Random.Range((int)MusicClip.B_B_Hard_0, (int)MusicClip.B_B_Hard_2 + 1);
                    }
                    else if (b_b_volume > 0.33f)
                    {
                        b_b_index = Random.Range((int)MusicClip.B_B_Mid_0, (int)MusicClip.B_B_Mid_1 + 1);
                    }
                    else
                    {
                        b_b_index = Random.Range((int)MusicClip.B_B_Weak_0, (int)MusicClip.B_B_Weak_1 + 1);
                    }

                    GameManager_script.Instance().PlaySound(b_b_index, false, b_b_volume);
                }

                // real physics stuff starts here
                if (inMove && cueController && collision.collider.GetComponent<Rigidbody>())
                {
                    Vector3 exitVelocity = Vector3.zero;
                    float energyRetained = 0.0f;

                    // object ball with a spline attached (risky but necessary for a successful game)
                    // has to be the first contact 
                    if (otherBall.isMain && cueController.CurrentBallID == id && cueController.TargetPocketDirection != Vector3.zero)
                    {
                        // assign collision normal
                        exitVelocity = cueController.TargetPocketDirection;

                        // zero out stuff so there is no next use
                        cueController.TargetPocketDirection = Vector3.zero;
                        cueController.CurrentBallID = -1;

                        // calculate energy retained
                        energyRetained = Vector3.Dot((cueController.currentHitBallController.reserveVelocity + cueController.cueBallController.reserveVelocity).normalized, exitVelocity.normalized);

                        //  the following are all copied from ball controller, make them into a function
                        if (energyRetained < 0.0f)
                        {
                            energyRetained += 1;
                        }

                        if (!cueController.currentHitBallController.GetComponent<Rigidbody>().isKinematic)
                        {
                            cueController.currentHitBallController.GetComponent<Rigidbody>().velocity = exitVelocity.normalized * energyRetained * (cueController.cueBallController.reserveVelocity.magnitude + cueController.currentHitBallController.reserveVelocity.magnitude);
                        }
                    }
                    // cue ball with its spline, this will always check out
                    else if (isMain && cueController.CurrentBallID == otherBall.id && cueController.CueBallBounceDirection != Vector3.zero)
                    {
                        // assign collision normal
                        exitVelocity = cueController.CueBallBounceDirection;

                        // zero out the spline
                        cueController.CueBallBounceDirection = Vector3.zero;

                        // calculate energy retained
                        energyRetained = Vector3.Dot((cueController.currentHitBallController.reserveVelocity + cueController.cueBallController.reserveVelocity).normalized, exitVelocity.normalized);

                        //  the following are all copied from ball controller, make them into a function
                        if (energyRetained < 0.0f)
                        {
                            energyRetained += 1;
                        }

                        if (!cueController.cueBallController.GetComponent<Rigidbody>().isKinematic)
                        {
                            cueController.cueBallController.GetComponent<Rigidbody>().velocity = exitVelocity.normalized * energyRetained * (cueController.cueBallController.reserveVelocity.magnitude + cueController.currentHitBallController.reserveVelocity.magnitude);
                        }
                    }
                    // all other cases of ball to ball collision
                    else
                    {
                        // we use unity built in engine for this purpose
                        exitVelocity = GetComponent<Rigidbody>().velocity;

                        // zero out stuff so there is no next use
                        cueController.TargetPocketDirection = Vector3.zero;
                        cueController.CurrentBallID = -1;
                        cueController.CueBallBounceDirection = Vector3.zero;

                        // calculate energy retained
                        energyRetained = exitVelocity.magnitude / (reserveVelocity.magnitude + otherBall.reserveVelocity.magnitude);

                        //  guard against negative numbers
                        if (energyRetained < 0.0f)
                        {
                            energyRetained += 1;
                        }

                        if (!GetComponent<Rigidbody>().isKinematic)
                        {
                            GetComponent<Rigidbody>().velocity = exitVelocity.normalized * energyRetained * (reserveVelocity.magnitude + otherBall.reserveVelocity.magnitude);
                        }
                    }
                }

                // some game rules, you can only first hit the current first ball
                if (cueController.firstBallBallCollisionSinceShot && otherBall != null)
                {
                    if (isMain || otherBall.isMain)
                    {
                        int tempId = 0;

                        if (isMain)
                        {
                            tempId = otherBall.id;
                        }
                        else
                        {
                            tempId = id;
                        }

                        if (cueController.currentBallControllers.Count > 1)
                        {
                            if (tempId != cueController.currentBallControllers[1].id) // first ball remaining
                            {
                                cueController.hittingTheRightFirstBall = false;
                            }
                            else
                            {
                                cueController.hittingTheRightFirstBall = true;
                            }
                        }

                        cueController.firstBallBallCollisionSinceShot = false;
                    }
                }

                cueController.contactedAtLeastOneRealBall = true;
            }

            if (layerName == "Wall" && !GetComponent<Rigidbody>().isKinematic && !ballIsPocketed) // walllllz, should include pocket walls
            {
                // choose sound
                float b_w_volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / cueController.ballMaxVelocity);
                int b_w_index = Random.Range((int)MusicClip.B_W_0, (int)MusicClip.B_W_1 + 1);

                // play sound
                GameManager_script.Instance().PlaySound(b_w_index, false, b_w_volume);

                // take half a ball back to avoid the risk of running into the other side of the wall before our wall, vectoroperator has things that does this as well
                Ray ray = new Ray(GetComponent<Rigidbody>().position - cueController.ballRadius * reserveVelocity.normalized * 1.25f, reserveVelocity.normalized);
                RaycastHit hit;
                
                // use a common angular velocity bullshit
                Vector3 reserveXZVelocity = VectorOperator.getProjectXZ(reserveVelocity, false);
                Vector3 reserveXZAngularVelocity = VectorOperator.getProjectXZ(reserveAngularVelocity, false);

                if (Physics.SphereCast(ray, cueController.ballRadius, out hit, 20.0f * cueController.ballRadius, cueController.wallMask))
                {
                    // make sure we only have a bounce but no pocket direction, meaning it is not a ball hit
                    if (isMain && cueController.CueBallBounceDirection != Vector3.zero && cueController.TargetPocketDirection == Vector3.zero)
                    {
                        // set the straight velocity up
                        GetComponent<Rigidbody>().velocity = Vector3.Magnitude(reserveVelocity) * Vector3.Normalize(cueController.CueBallBounceDirection);

                        // reset everything just to be sure
                        cueController.TargetPocketDirection = Vector3.zero;
                        cueController.CurrentBallID = -1;
                        cueController.CueBallBounceDirection = Vector3.zero;
                    }
                    else
                    {
                        // calculate linear velocity
                        GetComponent<Rigidbody>().velocity = reserveXZVelocity - 2.0f * Vector3.Project(reserveXZVelocity, VectorOperator.CleanYAxis(-hit.normal));
                        GetComponent<Rigidbody>().velocity = VectorOperator.getProjectXZ(GetComponent<Rigidbody>().velocity, true);
                    }

                    if (true)
                    {
                        // calculate angular velocity
                        Vector3 XZHitNormal = VectorOperator.CleanYAxis(-hit.normal);

                        // this declaration is fucking retarded
                        float XAngular = reserveXZAngularVelocity.x;
                        float ZAngular = reserveXZAngularVelocity.z;

                        // z is NOT flat, so x needs to be re-calculated. Since reserve velocity x is always negative of the reserve angular z, so we add.
                        if (Mathf.Abs(XZHitNormal.x) > 0.01f)
                        {
                            ZAngular += Mathf.Abs(XZHitNormal.x) * reserveVelocity.x * 2.0f * 2.0f; // 2.0f for twice the angular, 2.0f for negation
                            ZAngular = Mathf.Clamp(ZAngular, Mathf.Abs(reserveXZAngularVelocity.z) * -1.0f, Mathf.Abs(reserveXZAngularVelocity.z) * 1.0f); // dajiang hack, i omitted the other component, make sure ZAngular doesn't exceed the total magnitude it previously had
                        }

                        // x is NOT flat, so z needs to be re-calculated. Since reserve velocity z is always in sync with reserve angluar x, so we subtract.
                        if (Mathf.Abs(XZHitNormal.z) > 0.01f)
                        {
                            XAngular -= Mathf.Abs(XZHitNormal.z) * reserveVelocity.z * 2.0f * 2.0f; // 2.0f for twice the angular, 2.0f for negation
                            XAngular = Mathf.Clamp(ZAngular, Mathf.Abs(reserveXZAngularVelocity.x) * -1.0f, Mathf.Abs(reserveXZAngularVelocity.x) * 1.0f); // dajiang hack, i omitted the other component, make sure XAngular doesn't exceed the total magnitude it previously had
                        }

                        GetComponent<Rigidbody>().angularVelocity = new Vector3(XAngular, 0.0f, ZAngular);
                    }

                    // add Y component of angular velocity to linear velocity and subtract itself from the angular component, dajiang hack? Donno what this is no more
                    Vector3 YAngularVelocityOnly = new Vector3(0.0f, reserveAngularVelocity.y, 0.0f);

                    // actually add and subtract angular velocities
                    GetComponent<Rigidbody>().velocity += 0.36f * Vector3.Magnitude(YAngularVelocityOnly * 0.5f) * Vector3.Cross(VectorOperator.CleanYAxis(-hit.normal), YAngularVelocityOnly.normalized);
                    GetComponent<Rigidbody>().angularVelocity -= new Vector3(0.0f, 0.36f * reserveAngularVelocity.y, 0.0f); // this line checks out

                    // calculate restitution cooef, 1 is straight, 0 is slice
                    float impactAngle = Mathf.Clamp01(Vector3.Dot(reserveXZVelocity.normalized, VectorOperator.CleanYAxis(-hit.normal).normalized));

                    // restitution, choose wisely
                    GetComponent<Rigidbody>().velocity = (0.96f - impactAngle * 0.24f) * GetComponent<Rigidbody>().velocity;
                    GetComponent<Rigidbody>().angularVelocity = (0.96f - impactAngle * 0.24f) * GetComponent<Rigidbody>().angularVelocity;
                }

                // only start counting the wall hits after a good ball hit, otherwise all wall hits are pointless
                if (cueController.contactedAtLeastOneRealBall)
                {
                    cueController.wallHitCount += 1;
                }
            }
        }
    }

    // we do need this, for a later time maybe
    void OnCollisionExit(Collision collision)
    {
        // do nothing
    }
}
