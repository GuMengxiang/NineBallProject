// This is probably the most frequently used file

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CueController : MonoBehaviour
{
    // constants
    [System.NonSerialized]
	public float ballMaxVelocity = 235.0f; // This is set in the scene cueController  这是在场景cueController中设置的
    [System.NonSerialized]
    private float ballsDistance = 0.002f; // This is the gap between balls on start, we can make it random later. (+- 0.001f translates to +- 0.05mm)
    [System.NonSerialized]
    private float ballLineLength = 3.0f; // This is the max length of the line we are drawing
    [System.NonSerialized]
    public float cueMaxDisplacement = 9.0f; // This is the maximum cue draw back
    [System.NonSerialized]
    public Vector3 ballReturnOffset = new Vector3(-4.0f, 0.0f, 0.0f);

    // duration and other analytic specific stuff
    [System.NonSerialized]
    public float gameDuration = 0.0f;

    // anglez
    [System.NonSerialized]
    public float PerfectAngle = 1.000000f;
    [System.NonSerialized]
    public float LowerThreshold = 0.999391f;
    [System.NonSerialized]
    public float EightySevenFiveDegree = 0.999048f;
    [System.NonSerialized]
    public float EightyFiveDegree = 0.996194f;
    [System.NonSerialized]
    public float EightyDegree = 0.985f;
    [System.NonSerialized]
    public float SeventyFiveDegree = 0.966f;
    [System.NonSerialized]
    public float SeventyDegree = 0.940f;
    [System.NonSerialized]
    public float SixtyDegree = 0.866f;
    [System.NonSerialized]
    public float FiftyDegree = 0.766f;
    [System.NonSerialized]
    public float FourtyFiveDegree = 0.707f;
    [System.NonSerialized]
    public float FourtyDegree = 0.643f;
    [System.NonSerialized]
    public float ThirtyDegree = 0.500f;
    [System.NonSerialized]
    public float TwentyDegree = 0.342f;
    [System.NonSerialized]
    public float TenDegree = 0.174f;
    [System.NonSerialized]
    public float NinetyNinePercent = 0.99f;

    // more constants?
    [System.NonSerialized]
    public float ballRadius = 0.35f;
    [System.NonSerialized]
    public float ballRadiusMultiple = 1.5f;
    [System.NonSerialized]
    public float newBallRadius = 0.35f;
    
    // key locations used in the game
    [SerializeField]
    private Transform mainBallPoint;
    [SerializeField]
    private Transform firstBallPoint;
    [SerializeField]
    public Transform centerPoint;

    // These are all related to how much line do we give when we try to draw the lines when people swipe around and the sensitivity of the touch
    // A more generous curve means we give players more lines on slice shots, thus rendering cue line more powerful
    public AnimationCurve ballStrikeEnergyRetention; // dajiang hack, not doing the 0.717/0.717 thing coz its too weak, add some to it so players like it better
    public AnimationCurve ballAngularVelocityCurve; // dajiang hack, 1.0f should be 0.5f but what the hell... add some to it so players like it better
    public AnimationCurve touchSensitivityCurve;
    public AnimationCurve sliderForceCurve;
    public AnimationCurve extensionFactor;

    // UI elements
    [SerializeField]
	private Menu menu; // swiper
	[SerializeField]
	private Camera guiCamera; // swiper cam
	[SerializeField]
	private Camera camera2D; // normal cam
    [SerializeField]
    private Camera Cuecamera; // very important haha
	[SerializeField]
	private Camera camera3D;
    [SerializeField]
    private GameObject forceSlider;
    [SerializeField]
    private GameObject SpinShadow;
    [SerializeField]
    private Transform StartCube; // starting box area for the cue ball
    [SerializeField]
    private Transform MoveCube; // probably the entire table for the cue ball to move
    [SerializeField]
    private Transform currentCollision;
    [SerializeField]
    private Transform collisionSphere;
    [SerializeField]
    private Transform collisionSphereRed;
    [SerializeField]
    private LineRenderer firstCollisionLine;
    [SerializeField]
    private LineRenderer secondCollisionLine;
    [SerializeField]
    private LineRenderer ballCollisionLine;
    [SerializeField]
    private LineRenderer firstCollisionLine1;
    [SerializeField]
    private LineRenderer secondCollisionLine1;
    [SerializeField]
    private LineRenderer ballCollisionLine1;
    [SerializeField]
    private LineRenderer firstCollisionLine2;
    [SerializeField]
    private LineRenderer secondCollisionLine2;
    [SerializeField]
    private LineRenderer ballCollisionLine2;
    [System.NonSerialized]
    private float collisionLineWidth = 0.30f;
    [System.NonSerialized]
    private bool collisionTotallyDisabled = false;

    // curves related to physical decays
    public AnimationCurve DecayVelocityMagnitudeCurve;
    public AnimationCurve DecayVelocityMultipleCurve;
    public AnimationCurve ReconVelocityMagnitudeCurve;
    public AnimationCurve ReconVelocityMultipleCurve;

    // camera related vars
    [System.NonSerialized]
    public Camera3DController camera3DController; // not needed no more
    [System.NonSerialized]
    private Camera currentCamera;
    [System.NonSerialized]
    private float camScreenSize;
    [System.NonSerialized]
    public bool is3D = true;
    [System.NonSerialized]
    public Vector3 ThreeDOffset = Vector3.zero;

    // 3D UI display effects (don't touch if not necessary)
    [SerializeField]
    private Transform ballsParent;

    // ball displays and positions
	[SerializeField]
	private Texture2D[] ballTextures; // 10 textures for all different balls
	[SerializeField]
    private Vector2[] deltaPositions; // 7 relative positions for all balls except for cue, 1 and 9
    
    // ball controllers
    [System.NonSerialized]
    public List<BallController> currentBallControllers; // controls all balls on the table (not sure if cue ball is included)
    [System.NonSerialized]
    public List<BallController> pocketedBallControllers; // controls all balls on the table (not sure if cue ball is included)
    [System.NonSerialized]
    public BallController[] allBallControllers; // controls all balls ever created
    [System.NonSerialized]
    public BallController cueBallController; // controls cue ball
    [System.NonSerialized]
    public BallController currentHitBallController = null;
    [SerializeField]
    private BallController ballControllerPrefab; // help load all other balls

    // smart bot fake freeze controllers
    [System.NonSerialized]
    public Vector3[] SmartBotPausePosition;
    [System.NonSerialized]
    public Vector3[] SmartBotPauseVelocity;
    [System.NonSerialized]
    public Vector3[] SmartBotPauseAngularVelocity;
    [System.NonSerialized]
    public bool[] SmartBotPauseKinematic;
    [System.NonSerialized]
    public bool[] SmartBotPausePocket;

    // sparseness value for smart bots ya?
    [System.NonSerialized]
    public float SparsenessThreshold = 4.0f;

    // smart bot curvez
    public AnimationCurve SmartBotTolerateLevelCurve;

    // cue pivots and some positions and rotations
    public Vector3 cueBallPivot = Vector3.zero;
    public Transform cuePivot;
	public Transform cueRotation;
    private Vector3 tempCuePosition = Vector3.zero;
    private Vector2 tempHitPoint = Vector2.zero;

    // bools
    [System.NonSerialized]
    public float cueForceValue = 1.0f;
    [System.NonSerialized]
    public bool cueForceisActive = false;
    [System.NonSerialized]
	public float cueDisplacement = 0.0f;
	[System.NonSerialized]
	public bool allIsSleeping = true;
	[System.NonSerialized]
	public bool inMove = false;
	[System.NonSerialized]
	public bool canMoveBall = true;
	[System.NonSerialized]
    public bool cueBallIsPocketed = false;

    // physics and logics vars
	[System.NonSerialized]
	public Vector3 ballShotVelocity = Vector3.zero; // lets all hope this doesn't get changed after the cue ball is shot
	[System.NonSerialized]
	public Vector3 ballShotAngularVelocity = Vector3.zero;
	[System.NonSerialized]
	public Vector3 ballVelocityOrient = Vector3.forward;
	[System.NonSerialized]
	public Vector3 OutBallFirstVelocityOrient;
	[System.NonSerialized]
	public Vector3 OutBallSecondVelocityOrient;
	[System.NonSerialized]
	public Vector2 rotationDisplacement = Vector2.zero;
	[System.NonSerialized]
	public Vector3 hitBallVelocity = Vector3.zero;
	[System.NonSerialized]
	public Collider hitCollider = null;

    // physics collisions boundaries
	private LayerMask mainBallMask;
	[System.NonSerialized]
	public LayerMask ballMask;
	[System.NonSerialized]
	public LayerMask canvasMask;
	[System.NonSerialized]
	public LayerMask wallMask;
	[System.NonSerialized]
	public LayerMask wallAndBallMask;
    [System.NonSerialized]
    public LayerMask canvasAndBallMask;

	private bool hitCanvas = false;
	
    // variables related to the rules (don't pocket cue ball, etc etc)
    [System.NonSerialized]
    public bool pocketedTheCueBall = false;
    [System.NonSerialized]
    public bool pocketedAnyObjectBall = false;
    [System.NonSerialized]
    public bool pocketedNineBall = false;
    [System.NonSerialized]
    public bool hittingTheRightFirstBall = false;
    [System.NonSerialized]
    public bool firstBallBallCollisionSinceShot = true; // this thing is tricky, don't take it for its face value
    [System.NonSerialized]
    public bool pushoutAllowed = false;
    [System.NonSerialized]
    public bool pushoutCalled = false;
    [System.NonSerialized]
    public bool skipAllowed = false;
    [System.NonSerialized]
    public bool skipCalled = false;
    [System.NonSerialized]
    public bool contactedAtLeastOneRealBall = false; // wallHitCount's buddy
    [System.NonSerialized]
    public int shotCount = 0;
    [System.NonSerialized]
    public int wallHitCount = 0;

    // final ball positions, stillBallPositions == null stuff is not very tight after the very first update, beware.
    [System.NonSerialized]
    public Vector3[] stillBallPositions = null;
    [System.NonSerialized]
    public bool[] stillBallPocketed = null;

    // varaiables related to how the cue's helper ball moves
    private Vector3 cueRotationStrLocalPosition = Vector3.zero;
    private Quaternion cuePivotLocalRotation = Quaternion.identity;
    [System.NonSerialized]
	public Vector3 cueRotationLocalPosition = Vector3.zero;
	[System.NonSerialized]
	public Vector3 cueBallPivotLocalPosition = Vector3.zero;
	[System.NonSerialized]
	public Vector3 ballMovePosition = Vector3.zero;
    [System.NonSerialized]
    public Vector3 ballCurrentPosition = Vector3.zero;

    [System.NonSerialized]
    public Vector3 noyPickUpOffSet = Vector3.zero;

    // this shouldn't really be here la, solo control flag
    public bool SoloModeMasterInControl = false;

    // tracker flags
    public bool TrackingShotAsPlayerOne = false; // this is kool
    public bool TrackingShotAsPlayerTwo = false; // this is kool

    // bots cue rotation stuff
    [System.NonSerialized]
    public float SecondsOnBall = 2.0f; // we stay 2 seconds on the ball
    [System.NonSerialized]
    public float SecondsOnOthers = 2.0f; // we spend 2 seconds for all the angles that are NOT ball
    [System.NonSerialized]
    public float TotalRotateAngle = 0.0f;
    [System.NonSerialized]
    public float PrecisionMinimum = 1.99925f;
    [System.NonSerialized]
    public float TolerateLevel = 0.0f; // automatically impossible to hit

    // bot cue draw and spin stuff
    [System.NonSerialized]
    public float CurrentDrawSpeed = 0.00f;
    [System.NonSerialized]
    public bool FinalApproach = false;
    [System.NonSerialized]
    public Vector2 DrawTarget = Vector2.zero;

    // bot bools
    public bool ShootHasFinished = false;
    public bool BotShotConfirmed = false;
    public bool BotShootStrengthConfirmed = false;
    public bool AimScoreIncreasing = false;

    // bot miscs
    public PossibleShootPlacement BestBotShootPosition;
    public List<Vector3> BotCueBallPositions;

     // duo purpose with physics and bot
    public Vector3 TargetPocketDirection;
    public Vector3 CueBallBounceDirection;
    public Vector3 CueShootDirection;
    public float CurrentBallID;

    // swipe consts
    [System.NonSerialized]
    float swipeBase = 0.0f;
    [System.NonSerialized]
    float swipeLength = 0.0f;
    [System.NonSerialized]
    float swipeCount = 0.0f; // calculated later

    // ball creation process
    [System.NonSerialized]
    public bool ballsIsCreated = false; // whatever
    [System.NonSerialized]
    public int ballsCount = 0; // will be texture length which is 10 for now

    // time stamp constants
    public const float AIM_TIME_INTERVAL = 0.100f;
    public const float SHOOT_TIME_INTERVAL = 0.333f;
    public const float SHOOT_UPDTAE_FREQUENCY = 9.0f; // every 9th shot update is mandatory (3 seconds apart)

    // time stamps for data transfer and audio control, etc
    public float aimTimeStamp = 0.0f; // this is responsible for making sure we send an update every 0.1 second during the aiming phase
    public float shootTimeStamp = 0.0f; // this is responsible for timing ball movements and shots
    public float shootMandatoryStamp = 0.0f; // this is responsible for making sure some shots are mandatory (knock back clock if we have to)
    public float lastSoundTimeStamp = 0.0f; // this is only useful for ball to ball collision sound
    public float shootTotalTimeLapsed = 0.0f; // this is used for network transmission and lags

    // ball velocity at the instant of shooting
    public Vector3 instaBallShotVelocity = Vector3.zero;
    public Vector3 instaHitBallVelocity = Vector3.zero;
    public Vector3 instaBallShotAngularVelocity = Vector3.zero;

    // timer
    public bool incrementTimer = false;
    public float incrementalTimer = 0.0f;
    public float totalStartTimer = 0.0f;
    public float totalEndTimer = 0.0f;

    // current cue in display
    [System.NonSerialized]
    public float CueCurrentlyInUse = 0.0f;

    // need to average out updates
    public List<float> playerSwipeStorage;

    // ftue related stuffz
    [System.NonSerialized]
    public int ftueSwipeCount = 0;
    [System.NonSerialized]
    public bool ftueShotComplete = false;

    // Listen for pushout and skip options
    public bool ListeningForPSConfirmation = false;

    // in game ui handle
    public UIPanel inGameMainUIPanel; // this is the center panel
    public GameObject toolTipPrefab;
    public GameObject gamecenter;
    public GameCenter GC;
    public List<CueInfo> CueList;
    public Material CueMaterial;
    public GameObject CueModel;
    public GameObject PlayerHead1;
    public GameObject PlayerHead2;
    public GameObject CueUI;
    public GameObject Mouse;
    public GameObject FTUE_Mouse;

    // random network cue control gate
    [System.NonSerialized]
    public bool AllowCueStatusToUpdate = false;

    // stops the special bug where players receives a load MM from self finish and another one from opponent quitting
    [System.NonSerialized]
    public bool LoadMainMenuAlreadyCalled = false;

    void Awake()
    {
		if(MenuControllerGenerator.controller) // without this, nothing is there
        {
            if (ServerController.serverController)
            {
                enabled = false; // enable it later?
            }

            // dajiang physics 6
            // figure out possible table size
            // we can go maximum 88% of total width as table width
            // we can go maximum 83% of total height as table height
            // current table is NOT accurate. If ball diameter is 1 unit == 57mm, a 9 foot table is roughly 48.13 units long and half as wide (without the coushions) 
            // currently, table is 34.4X16.5 without coushions and 37X19 with coushions, about 1.8 times.

            // the viewport's w and h are 1:1 so it completely depends on where we want the table be with the screen w and h ratio
            // right now we have 2 curves going through catering to the current (short) table and UI elements, we need to change this once the real tables come in
            // the actual whereabouts of the table on screen is completely up to us, who put dots into the curves
            // and of course, no work is being done on the 3D camera yet

            // screen width and height ratio clamped between the curves available
            // right now, table outline ratio is 1.83f
            float ratio = 1.0f * Screen.width / Screen.height;

            if (ratio > GameManager_script.Table_Self_Ratio) // 1.83 is table measured
            {
                // use height as primary factor
                camera2D.orthographicSize = 11.80f * (1.0f / GameManager_script.Table_Screen_Ratio); // 80% and 20% in terms of height
                Cuecamera.orthographicSize = 11.80f * (1.0f / GameManager_script.Table_Screen_Ratio); // 80% and 20% in terms of height
            }
            else
            {
                // use width as primary factor
                camera2D.orthographicSize = (11.80f * Screen.height) / (Screen.width * GameManager_script.Table_Screen_Ratio / GameManager_script.Table_Self_Ratio); // 0.85 is 85%
                Cuecamera.orthographicSize = (11.80f * Screen.height) / (Screen.width * GameManager_script.Table_Screen_Ratio / GameManager_script.Table_Self_Ratio); // 0.85 is 85%
            }

            float totalGapTopBot = 2.0f * (camera2D.orthographicSize - 11.80f); // this is gap expressed in terms of camera length
            float botGapRequired = GameManager_script.Instance().Bot_Gap / (GameManager_script.Instance().Bot_Gap + GameManager_script.Instance().Top_Gap + GameManager_script.Instance().Mid_Gap + GameManager_script.Instance().Top_Padding) * totalGapTopBot;
            float movementToBottom = totalGapTopBot * 0.5f - botGapRequired;

            camera2D.transform.position = new Vector3(0.20f, camera2D.transform.position.y, -2.58f + movementToBottom);

            // get camera view port sreen size
            Vector3 p1 = camera2D.ViewportToWorldPoint(new Vector3(0, 0, camera2D.nearClipPlane));
            Vector3 p3 = camera2D.ViewportToWorldPoint(new Vector3(1, 1, camera2D.nearClipPlane));

            camScreenSize = (p3 - p1).magnitude;

            if (GameManager_script.Instance().StupidBotInActionGame || GameManager_script.Instance().TrulySelfInActionGame)
            {
                SoloModeMasterInControl = Random.Range(0, 2) == 1 ? true : false;
            }

            if (GameManager_script.Instance().FTUEInActionGame)
            {
                SoloModeMasterInControl = true;
            }

            // init balls
            currentBallControllers = new List<BallController>(0);
            pocketedBallControllers = new List<BallController>(0);

            if (GameManager_script.Instance().FTUEInActionGame)
            {
                FtueCreateAndSortBalls();
            }
            else
            {
                NormalCreateAndSortBalls();
            }

            ballsIsCreated = true;

            foreach (BallController item in currentBallControllers)
            {
                item.cueController = this;
            }

            // init masks?
            mainBallMask = 1 << LayerMask.NameToLayer("MainBall");
            ballMask = 1 << LayerMask.NameToLayer("Ball");
            canvasMask = 1 << LayerMask.NameToLayer("Canvas");
            wallMask = 1 << LayerMask.NameToLayer("Wall");
            wallAndBallMask = wallMask | ballMask;
            canvasAndBallMask = canvasMask | ballMask;

            // disable cams for now, wait for the ready to play signal?
            camera3D.enabled = false;
            camera2D.enabled = false;

            // init swipe constants
#if UNITY_IPHONE
            swipeBase = 4.0f;
            swipeLength = 8.0f;
#elif UNITY_ANDROID
            swipeBase = 3.5f;
            swipeLength = 7.0f;
#else
            swipeBase = 2.0f;
            swipeLength = 4.0f;
#endif
            swipeCount = (swipeBase + swipeBase + swipeLength - 1) * swipeLength * 0.5f;

            // init bot cue position list
            BotCueBallPositions = new List<Vector3>(0);

            // init swipe queue
            playerSwipeStorage = new List<float>(0);

            // init force slider
            ConditionalEnableForceSlider();

            // init curvezzz
            PutSmartBotCurvesInLists();

            // set tolerate level just for the hack of it
            TolerateLevel = Random.Range(LowerThreshold, PerfectAngle);

            // init ftue dajiang hack
            if (GameManager_script.Instance().FTUEInActionGame)
            {
                forceSlider.SetActive(false);
            }

            // pipe in game UI portion
            if (gamecenter != null)
            {
                GC = gamecenter.GetComponent<GameCenter>();
            }
        }
    }

    void Start()
    {
        if (MenuControllerGenerator.controller)
        {
            // start counting duration
            gameDuration = Time.realtimeSinceStartup;

            // definitely starting the game, so sending some signal
            SendGameStartSignal();

            // cue init
            cueRotationStrLocalPosition = cueRotation.localPosition;
            OnEnableLineAndSphere();

            // status init
            if (GameManager_script.Instance().FTUEInActionGame)
            {
                CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_FIXED_ON_TABLE;
            }
            else
            {
                CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_MOVABLE_ON_TABLE;
            }

            // bot init
            if (BotInControl())
            {
                DetermineListOfGoodPositions();
            }

            // smart robot intention inits
            if (NetworkBotInControl())
            {
                // do nothing, dajiang hack, i can add a list of good positions too?
            }

            // clear all bot related stuff no matter what
            BotBeginShotHouseKeeping();

            // init camera
            is3D = false;
            ThreeDOffset = is3D ? ballReturnOffset : Vector3.zero; // we don't need to update ball positions because none were pocketed (or created for that matter).
            currentCamera = is3D ? camera3D : camera2D;
            currentCamera.enabled = true;

            // init timer
            starttimer();

            // cont'd...
            CueCurrentlyInUse = GameManager_script.Instance().CueEquipped;
            ChangeCueUIImage(CueCurrentlyInUse);
            UpdateDisplayCue();

            // init helpful tooltip
            if (GameManager_script.Instance().SmartBotInActionGame || GameManager_script.Instance().StupidBotInActionGame || GameManager_script.Instance().TrulySelfInActionGame || GameManager_script.Instance().FTUEInActionGame)
            {
                ShowHelpfulTooltipPopup("", "YourBreak", true, false);

                if (BotInControl() || NetworkSlaveInControl() || NetworkBotInControl()) // set for rematch breaks
                {
                    GameManager_script.Instance().rematchYouAreThePrevBreaker = false;
                }
                else
                {
                    GameManager_script.Instance().rematchYouAreThePrevBreaker = true;
                }
            }
        }
	}

    // we can keep if fixed for now, minor physics
    void FixedUpdate()
	{
        aimTimeStamp += Time.fixedDeltaTime;
        shootTimeStamp += Time.fixedDeltaTime;
        shootTotalTimeLapsed += Time.fixedDeltaTime;

        // timer problems...
        if (incrementTimer)
        {
            incrementalTimer += Time.fixedDeltaTime;
            totalEndTimer = Time.realtimeSinceStartup;

            if (!GameManager_script.Instance().FTUEInActionGame)
            {
                animateIncrementalTimer();
            }

            if (NetworkMasterInControl() || NetworkBotInControl() || SoloMasterInControl() || SoloSecondPersonInControl() || BotMasterInControl() || BotInControl())
            {
                // if the game is paused for more than x seconds, we use the real deal to sub in the incremental timer
                if (totalEndTimer - totalStartTimer - incrementalTimer > GameManager_script.Instance().SkippableTimer)
                {
                    incrementalTimer += totalEndTimer - totalStartTimer;
                }

                if (incrementalTimer > GameManager_script.Instance().ClockCap && GameManager_script.Instance().clockMusic == null && !GameManager_script.Instance().FTUEInActionGame)
                {
                    float c_c_volume = 1.0f;
                    int c_c_index = (int)MusicClip.Clock;

                    GameManager_script.Instance().clockMusic = GameManager_script.Instance().PlaySound(c_c_index, true, c_c_volume);

                    // vibrate only when self is in control just to make sure player knows
                    if (NetworkMasterInControl() || BotMasterInControl() || SoloMasterInControl() || SoloSecondPersonInControl())
                    {
                        GameManager_script.Instance().Vibrate();
                    }
                }

                // if incremental timer (after the addition) is greater than 10.0f seconds, turn is DONE
                if (incrementalTimer > GameManager_script.Instance().TimerCap && !GameManager_script.Instance().FTUEInActionGame)
                {
                    // clean up all possible UI buttons
                    GC.CleanUpAllToolTipRelatedUI();

                    // stops updating till its started again
                    incrementTimer = false;

                    // goes to end of shot
                    StopCoroutine("WaitTillAllBallsStopsMoving");
                    StartCoroutine("WaitTillAllBallsStopsMoving");
                }
            }
        }

        // give network update commands
        if (NetworkMasterInControl())
        {
            if (allIsSleeping)
            {
                bool isInHand = true;

                if (CueControllerUpdater.current_control_status == CueControllerUpdater.CUE_BALL_IN_HAND)
                {
                    isInHand = true;
                }
                else
                {
                    isInHand = false;
                }

                if (aimTimeStamp > AIM_TIME_INTERVAL)
                {
                    aimTimeStamp = 0.0f;

                    cueRotationLocalPosition = cueRotation.localPosition;
                    cuePivotLocalRotation = cuePivot.localRotation;

                    ServerController.serverController.SendRPCToNetworkViewOthers("OnUpdateCueControl", CueCurrentlyInUse, isInHand, cueBallController.transform.position, cuePivot.localRotation, cueRotation.localPosition, new Vector3(rotationDisplacement.x, rotationDisplacement.y, 0.0f));
                }
            }
            else
            {
                if (shootTimeStamp > SHOOT_TIME_INTERVAL)
                {
                    shootTimeStamp = 0.0f;

                    bool mand = false;

                    shootMandatoryStamp++;

                    if (shootMandatoryStamp >= SHOOT_UPDTAE_FREQUENCY)
                    {
                        shootMandatoryStamp = 0.0f;

                        mand = true;
                    }

                    if (NetworkMasterInControl())
                    {
                        MasterBallStatusUpdate(mand); // random hack
                    }
                }
            }
        }

        if (GameManager_script.Instance().SmartBotFreezeInPlaceFlag && NetworkBotInControl())
        {
            // increment timer and wait for this to blow over
            GameManager_script.Instance().SmartBotFreezeInPlaceIncreTimer += Time.fixedDeltaTime;

            // are we over yet?
            if (GameManager_script.Instance().SmartBotFreezeInPlaceIncreTimer > GameManager_script.Instance().SmartBotFreezeInPlaceDuration)
            {
                // set flags back
                GameManager_script.Instance().SmartBotFreezeInPlaceIncreTimer = 0.0f;
                GameManager_script.Instance().SmartBotFreezeInPlaceDuration = 0.0f;
                GameManager_script.Instance().SmartBotFreezeInPlaceFlag = false;

                // boolean
                bool BeforeAfterSameState = true;

                // check if its kool
                for (int i = 0; i < allBallControllers.Length; i++)
                {
                    BeforeAfterSameState = BeforeAfterSameState && (allBallControllers[i].ballIsPocketed == SmartBotPausePocket[i]);
                }

                // put everything back if we are all in the same state
                if (BeforeAfterSameState)
                {
                    for (int i = 0; i < allBallControllers.Length; i++)
                    {
                        allBallControllers[i].GetComponent<Rigidbody>().isKinematic = false;

                        allBallControllers[i].GetComponent<Rigidbody>().position = SmartBotPausePosition[i];
                        allBallControllers[i].GetComponent<Rigidbody>().velocity = SmartBotPauseVelocity[i];
                        allBallControllers[i].GetComponent<Rigidbody>().angularVelocity = SmartBotPauseAngularVelocity[i];
                        allBallControllers[i].GetComponent<Rigidbody>().isKinematic = SmartBotPauseKinematic[i];
                        allBallControllers[i].ballIsPocketed = SmartBotPausePocket[i];
                    }
                }
            }
        }
        else if (GameManager_script.Instance().SmartBotFreezeInThePastKnockBack && NetworkBotInControl())
        {
            // set flag back
            GameManager_script.Instance().SmartBotFreezeInThePastIncreTimer = 0.0f;
            GameManager_script.Instance().SmartBotFreezeInThePastDuration = 0.0f;
            GameManager_script.Instance().SmartBotFreezeInThePastCountDown = false;
            GameManager_script.Instance().SmartBotFreezeInThePastKnockBack = false;

            // this means balls are still rolling, so we can do this
            if (!allIsSleeping)
            {
                // boolean
                bool BeforeAfterSameState = true;

                // check if its kool
                for (int i = 0; i < allBallControllers.Length; i++)
                {
                    BeforeAfterSameState = BeforeAfterSameState && (allBallControllers[i].ballIsPocketed == SmartBotPausePocket[i]);
                }

                // put everything back if we are all in the same state
                if (BeforeAfterSameState)
                {
                    for (int i = 0; i < allBallControllers.Length; i++)
                    {
                        if (!SmartBotPausePocket[i])
                        {
                            allBallControllers[i].GetComponent<Rigidbody>().isKinematic = false;

                            allBallControllers[i].GetComponent<Rigidbody>().position = SmartBotPausePosition[i];
                            allBallControllers[i].GetComponent<Rigidbody>().velocity = SmartBotPauseVelocity[i];
                            allBallControllers[i].GetComponent<Rigidbody>().angularVelocity = SmartBotPauseAngularVelocity[i];
                            allBallControllers[i].GetComponent<Rigidbody>().isKinematic = SmartBotPauseKinematic[i];
                            allBallControllers[i].ballIsPocketed = SmartBotPausePocket[i];
                        }
                    }
                }
            }
        }
        else
        {
            // only do it when network bot is in control
            if (NetworkBotInControl())
            {
                // if we are not in the process of counting down
                if (!GameManager_script.Instance().SmartBotFreezeInThePastCountDown)
                {
                    // run a update function that will start this stuff
                    if (!allIsSleeping && Random.Range(0.0f, 0.0f) < 0.0f) // dajiang hack, we don't activate this for now
                    {
                        // set flags
                        GameManager_script.Instance().SmartBotFreezeInPlaceIncreTimer = 0.0f;
                        GameManager_script.Instance().SmartBotFreezeInPlaceDuration = Random.Range(0.15f, 0.75f); // seconds
                        GameManager_script.Instance().SmartBotFreezeInPlaceFlag = true;

                        // record everything
                        for (int i = 0; i < allBallControllers.Length; i++)
                        {
                            SmartBotPausePosition[i] = allBallControllers[i].GetComponent<Rigidbody>().position;
                            SmartBotPauseVelocity[i] = allBallControllers[i].GetComponent<Rigidbody>().velocity;
                            SmartBotPauseAngularVelocity[i] = allBallControllers[i].GetComponent<Rigidbody>().angularVelocity;
                            SmartBotPauseKinematic[i] = allBallControllers[i].GetComponent<Rigidbody>().isKinematic;
                            SmartBotPausePocket[i] = allBallControllers[i].ballIsPocketed;
                        }
                    }
                    else if (!allIsSleeping && Random.Range(0.0f, 45000.0f) < 1.0f) // every 75 seconds
                    {
                        // set flagz
                        GameManager_script.Instance().SmartBotFreezeInThePastIncreTimer = 0.0f;
                        GameManager_script.Instance().SmartBotFreezeInThePastDuration = Random.Range(0.50f, 1.50f); // seconds
                        GameManager_script.Instance().SmartBotFreezeInThePastCountDown = true;
                        GameManager_script.Instance().SmartBotFreezeInThePastKnockBack = false;

                        // record everything
                        for (int i = 0; i < allBallControllers.Length; i++)
                        {
                            SmartBotPausePosition[i] = allBallControllers[i].GetComponent<Rigidbody>().position;
                            SmartBotPauseVelocity[i] = allBallControllers[i].GetComponent<Rigidbody>().velocity;
                            SmartBotPauseAngularVelocity[i] = allBallControllers[i].GetComponent<Rigidbody>().angularVelocity;
                            SmartBotPauseKinematic[i] = allBallControllers[i].GetComponent<Rigidbody>().isKinematic;
                            SmartBotPausePocket[i] = allBallControllers[i].ballIsPocketed;
                        }
                    }
                }
                else
                {
                    // we wait for 3 seconds before knocking EVERYTHING back
                    GameManager_script.Instance().SmartBotFreezeInThePastIncreTimer += Time.fixedDeltaTime;

                    // are we over yet?
                    if (GameManager_script.Instance().SmartBotFreezeInThePastIncreTimer > GameManager_script.Instance().SmartBotFreezeInThePastDuration)
                    {
                        // set flagz
                        GameManager_script.Instance().SmartBotFreezeInThePastIncreTimer = 0.0f;
                        GameManager_script.Instance().SmartBotFreezeInThePastDuration = 0.0f;
                        GameManager_script.Instance().SmartBotFreezeInThePastCountDown = false;
                        GameManager_script.Instance().SmartBotFreezeInThePastKnockBack = true;
                    }
                }
            }

            // dajiang physics 5, waiting for the dumb ass first collision to happen
            // THIS ONLY WORKS FOR update == between 100 and 200, anything else should be toned towards it (0.01f)
            // WE SHOULD MAKE IT SO THIS ONLY WORKS FOR SPEED HIGHER THAN X AND ANGLE LESS THAN Y
            if (currentHitBallController != null && (CueBallBounceDirection != Vector3.zero || TargetPocketDirection != Vector3.zero))
            {
                float velocityDifference = (cueBallController.reserveVelocity - currentHitBallController.reserveVelocity).magnitude;
                float angle = Vector3.Dot((currentHitBallController.reserveVelocity + cueBallController.reserveVelocity).normalized, CueBallBounceDirection.normalized);
                float distance = (cueBallController.GetComponent<Rigidbody>().position - currentHitBallController.GetComponent<Rigidbody>().position).magnitude;
                float threshold = 1.2f + velocityDifference * angle * 0.0047f; // from 1.2f to 2.5f

                // the 2 balls are pretty close and at least one of them is moving and making sure the angle is acute enough
                if ((velocityDifference > 100.0f && angle > 0.75f) && distance < threshold && (cueBallController.reserveVelocity.magnitude != 0.0f || currentHitBallController.reserveVelocity.magnitude != 0.0f))
                {
                    Vector3 exitVelocity = Vector3.zero;
                    float energyRetained = 0.0f;

                    // set velocity for other ball first (remember, we never set reserve velocity in this guy here because this is a collision related function)
                    if (TargetPocketDirection != Vector3.zero)
                    {
                        // assign collision normal
                        exitVelocity = TargetPocketDirection;

                        // zero out stuff so there is no next use
                        TargetPocketDirection = Vector3.zero;
                        CurrentBallID = -1;

                        // calculate energy retained
                        energyRetained = Vector3.Dot((currentHitBallController.reserveVelocity + cueBallController.reserveVelocity).normalized, exitVelocity.normalized);

                        //  the following are all copied from ball controller, make them into a function
                        if (energyRetained < 0.0f)
                        {
                            energyRetained += 1;
                        }

                        if (!currentHitBallController.GetComponent<Rigidbody>().isKinematic)
                        {
                            currentHitBallController.GetComponent<Rigidbody>().velocity = exitVelocity.normalized * energyRetained * (cueBallController.reserveVelocity.magnitude + currentHitBallController.reserveVelocity.magnitude);
                        }
                    }

                    // cue ball with its spline, this will always check out
                    if (CueBallBounceDirection != Vector3.zero)
                    {
                        // assign collision normal
                        exitVelocity = CueBallBounceDirection;

                        // zero out the spline
                        CueBallBounceDirection = Vector3.zero;

                        // calculate energy retained
                        energyRetained = Vector3.Dot((currentHitBallController.reserveVelocity + cueBallController.reserveVelocity).normalized, exitVelocity.normalized);

                        //  the following are all copied from ball controller, make them into a function
                        if (energyRetained < 0.0f)
                        {
                            energyRetained += 1;
                        }

                        if (!cueBallController.GetComponent<Rigidbody>().isKinematic)
                        {
                            cueBallController.GetComponent<Rigidbody>().velocity = exitVelocity.normalized * energyRetained * (cueBallController.reserveVelocity.magnitude + currentHitBallController.reserveVelocity.magnitude);
                        }
                    }

                    // some game rules, you can only first hit the current first ball
                    if (firstBallBallCollisionSinceShot)
                    {
                        if (currentBallControllers.Count > 1)
                        {
                            if (currentHitBallController.id != currentBallControllers[1].id) // first ball remaining
                            {
                                hittingTheRightFirstBall = false;
                            }
                            else
                            {
                                hittingTheRightFirstBall = true;
                            }
                        }

                        firstBallBallCollisionSinceShot = false;
                    }

                    contactedAtLeastOneRealBall = true;

                    // some sound shiites, just 1 sound ball to ball
                    float b_b_volume = Mathf.Clamp01(velocityDifference / ballMaxVelocity);
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
            }
        }

        if (ListeningForPSConfirmation)
        {
            if (GameManager_script.PushOutConfirmState == ConfirmationType.confirmed)
            {
                // set switch for one thing
                if (GC.ToolTipCurrentActive)
                {
                    if (GC.ToolTipCurrentType == "push")
                    {
                        PushOutAccepted();

                        GC.CleanUpAllToolTipRelatedUI();
                    }
                    else if (GC.ToolTipCurrentType == "skip")
                    {
                        ShootOptionAccepted();

                        GC.CleanUpAllToolTipRelatedUI();
                    }
                }

                // stop listening
                ListeningForPSConfirmation = false;

                // reset confirmation state
                GameManager_script.PushOutConfirmState = ConfirmationType.undecided;
            }
            else if (GameManager_script.PushOutConfirmState == ConfirmationType.denied)
            {
                if (GC.ToolTipCurrentActive)
                {
                    if (GC.ToolTipCurrentType == "push")
                    {
                        PushOutNotAccepted();

                        GC.CleanUpAllToolTipRelatedUI();
                    }
                    else if (GC.ToolTipCurrentType == "skip")
                    {
                        ShootOptionNotAccepted();

                        GC.CleanUpAllToolTipRelatedUI();
                    }
                }

                // stop listening
                ListeningForPSConfirmation = false;

                // reset confirmation state
                GameManager_script.PushOutConfirmState = ConfirmationType.undecided;
            }
            else
            {
                // do nothing and keep on listening
            }
        }

        // set shadow spin position?
        if (SpinShadow && (NetworkSlaveInControl() || NetworkBotInControl() || BotInControl()))
        {
            SpinShadow.GetComponent<GameSpinShadow>().ChangeSpinShadowCenterpoint(cueBallPivot.x, cueBallPivot.y);
        }

        // make sure hand curser stays?
        if (Mouse && Mouse.activeSelf)
        {
            Mouse.transform.localPosition = cueBallController.GetComponent<Rigidbody>().position + new Vector3(0.25f, 1.0f, -0.25f);
        }
    }

    public void animateIncrementalTimer()
    {
        if (NetworkMasterInControl() || SoloMasterInControl() || BotMasterInControl())
        {
            ChangeHead1BackgroundValue(Mathf.Clamp01(Mathf.Abs(1.0f - incrementalTimer / GameManager_script.Instance().TimerCap)));
        }
        else if (NetworkSlaveInControl() || NetworkBotInControl() || SoloSecondPersonInControl() || BotInControl())
        {
            ChangeHead2BackgroundValue(Mathf.Clamp01(Mathf.Abs(1.0f - incrementalTimer / GameManager_script.Instance().TimerCap)));
        }
    }

    public void MasterBallStatusUpdate(bool inMandatory)
    {
        // isPocketed, position, velocity, angular
        bool[] isPocketed = new bool[allBallControllers.Length];
        Vector3[] position = new Vector3[allBallControllers.Length];
        Vector3[] velocity = new Vector3[allBallControllers.Length];
        Vector3[] angular = new Vector3[allBallControllers.Length];

        for (int i = 0; i < allBallControllers.Length; i++)
        {
            isPocketed[i] = (allBallControllers[i].ballIsPocketed);
            position[i] = (allBallControllers[i].GetComponent<Rigidbody>().position);
            velocity[i] = (allBallControllers[i].GetComponent<Rigidbody>().velocity);
            angular[i] = (allBallControllers[i].GetComponent<Rigidbody>().angularVelocity);
        }

        if (NetworkMasterInControl())
        {
            ServerController.serverController.SendRPCToNetworkViewOthers("OnUpdateAllBallStatus", shotCount, shootTotalTimeLapsed, inMandatory, isPocketed, position, velocity, angular, instaBallShotVelocity, instaHitBallVelocity, instaBallShotAngularVelocity);
        }
    }

    // trying to move the cue ball around by hand or just draw the spheres
    public void BallMovableOnTable()
    {
        // this block picks up the cue ball if we call it every frame
        if (NetworkMasterInControl() || BotMasterInControl() || SoloMasterInControl() || SoloSecondPersonInControl())
        {
            if (currentCamera && menu && menu.GetButtonDown())
            {
                Ray ray = currentCamera.ScreenPointToRay(menu.GetScreenPoint());
                RaycastHit hit;

                if(Physics.SphereCast(ray, ballRadius, out hit, 1000.0f, canvasAndBallMask))
                {
                    bool noyCanPickUp = false;

                    Vector3 noyClick = new Vector3(hit.point.x, 0, hit.point.z);
                    Vector3 noyCueBall = new Vector3(cueBallController.GetComponent<Rigidbody>().position.x, 0, cueBallController.GetComponent<Rigidbody>().position.z);

                    // if the click is below the cue ball
                    if (noyCueBall.z - noyClick.z > 0.0f && noyCueBall.z - noyClick.z < 3.0f * ballRadius)
                    {
                        // if x is within some bounds
                        if (Mathf.Abs(noyCueBall.x - noyClick.x) < 2.0f * ballRadius)
                        {
                            noyCanPickUp = true;
                        }
                    }

                    // if the click is above the cue ball
                    if (noyCueBall.z - noyClick.z < 0.0f && noyCueBall.z - noyClick.z > -2.0f * ballRadius)
                    {
                        if ((noyClick - noyCueBall).magnitude < 2.0f * ballRadius)
                        {
                            noyCanPickUp = true;
                        }
                    }

                    // do it
                    if (noyCanPickUp)
                    {
                        noyPickUpOffSet = noyCueBall - noyClick;

                        OnPickUpCueBall(cueBallController.transform.position);
                    }
                }
            }
        }

        BallFixedOnTable();
    }

    public void BallFixedOnTable()
    {
        // this block controls the movement and draws the cue ball pivot and rotation everything
        if (allIsSleeping && !cueBallIsPocketed)
        {
            DrawLinesAndSphere();
        }
    }

    // really move the cue ball around
	public void BallInHand()
	{
        // find out what the hell does this do
        if (cueBallIsPocketed)
		{
			OnCueBallIsPocketed(false);
		}

        // this part moves the ball around
		if(cueBallController.ballIsSelected)
		{
			if((NetworkMasterInControl() || SoloMasterInControl() || SoloSecondPersonInControl() || BotMasterInControl()) && menu.MouseIsMove)
            {
                OnMoveBall();
            }

			if(NetworkSlaveInControl())
			{
				cueBallController.transform.position = Vector3.Lerp(cueBallController.transform.position, ballMovePosition, 10.0f * Time.deltaTime);
            }
		}

        // this part drops the ball down, but we ignore what the slave thinks here
		if(cueBallController.ballIsSelected && !menu.GetButton() && !NetworkSlaveInControl() && !NetworkBotInControl()) // used to be menu.GetButtonUp(), but that is not very reliably is it?
		{
			OnDropOffCueBall();
        }
	}

	public void OnPickUpCueBall(Vector3 ballPosition) // by hand
	{
        if (allIsSleeping && !GameManager_script.Instance().DownOnRealButtons)
        {
            OnHideLineAndSphere();

            GameManager_script.Instance().CanControlCue = false;

            CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_IN_HAND;

            ballMovePosition = ballPosition;
            ballCurrentPosition = mainBallPoint.position;

            cueBallController.ballIsSelected = true;
            cueBallController.GetComponent<Collider>().enabled = false;
            cueBallController.GetComponent<Rigidbody>().useGravity = false;
            cueBallController.GetComponent<Rigidbody>().isKinematic = true;
            canMoveBall = true;
        }
    }

    void OnMoveBall() // cue ball is being moved
    {
        Transform StartOrMoveCube = shotCount == 0 ? StartCube : MoveCube;
        Ray ray = currentCamera.ScreenPointToRay(menu.GetScreenPoint());
        RaycastHit hit;

        if (Physics.SphereCast(ray, 1.0f * ballRadius, out hit, 1000.0f, canvasMask))
        {
            Vector3 doctoredHitPoint = new Vector3(hit.point.x, mainBallPoint.position.y, hit.point.z) + noyPickUpOffSet;

            VectorOperator.MoveBallInQuad(StartOrMoveCube, ballRadius, doctoredHitPoint, ref ballCurrentPosition);

            cueBallController.transform.position = ballCurrentPosition + 2.5f * ballRadius * Vector3.up;
        }
    }

    public void OnDropOffCueBallBySlave()
    {
        GameManager_script.Instance().CanControlCue = true;

        cueBallController.ballIsSelected = false;

        canMoveBall = false;

        CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_MOVABLE_ON_TABLE;

        cueBallController.GetComponent<Collider>().enabled = true;
        cueBallController.GetComponent<Rigidbody>().useGravity = true;
        cueBallController.GetComponent<Rigidbody>().isKinematic = false;

        if (!cueBallIsPocketed && allIsSleeping)
        {
            OnEnableLineAndSphere();
        }
    }

	public void OnDropOffCueBall() // by hand
	{
        GameManager_script.Instance().CanControlCue = true;
		Ray ray = new Ray(cueBallController.transform.position + 5.0f * ballRadius * Vector3.up, -Vector3.up);
		RaycastHit hit;
		
		if(Physics.SphereCast(ray, 1.2f * ballRadius , out hit, 1000.0f, canvasAndBallMask))
		{
			hitCanvas = hit.collider.gameObject.layer == LayerMask.NameToLayer("Canvas");
		}
		else
		{
			hitCanvas = false;
		}

		cueBallController.ballIsSelected = false;
		
        canMoveBall = false;

        CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_MOVABLE_ON_TABLE;

		if (hitCanvas)
		{
            cueBallController.transform.position = new Vector3(cueBallController.transform.position.x, mainBallPoint.position.y, cueBallController.transform.position.z);
        }
		else
		{
            cueBallController.SpotBallOnTableAndHouseKeeping(cueBallController.transform.position, Vector3.zero, Vector3.zero);
        }

		cueBallController.GetComponent<Collider>().enabled = true;
		cueBallController.GetComponent<Rigidbody>().useGravity = true;
		cueBallController.GetComponent<Rigidbody>().isKinematic = false;

        if (!cueBallIsPocketed && allIsSleeping)
        {
            OnEnableLineAndSphere();
        }
	}

    public bool CheckAllBallSleepingStatusAnyTime()
    {
        bool _allIsSleeping = true;

        if (allBallControllers != null)
        {
            foreach (BallController ballC in allBallControllers)
            {
                _allIsSleeping = _allIsSleeping && ballC.IsSleeping();
            }
        }

        return _allIsSleeping && !GameManager_script.Instance().SmartBotFreezeInPlaceFlag; // dajiang hack, this is pretty bad...
    }

    public bool CheckSleepingStatusAnyTime()
    {
        bool _allIsSleeping = true;

        foreach (BallController ballC in currentBallControllers)
        {
            if (ballC != cueBallController)
            {
                _allIsSleeping = _allIsSleeping && ballC.IsSleeping();
            }
            else
            {
                _allIsSleeping = _allIsSleeping && (cueBallController.IsSleeping() || cueBallController.ballIsPocketed);
            }
        }

        return _allIsSleeping && !GameManager_script.Instance().SmartBotFreezeInPlaceFlag; // dajiang hack, this is pretty bad...
    }

    // initial update for slave before game starts
    public void updateSlaveStillPositions(int inShotCount)
    {
        // this is the updating shot count portion, making sure it is aligned
        shotCount = inShotCount;

        // examine if this is not a cheat
        // if it looks anywhere close to a cheat, break off the contact and figure out who should act 
        if (stillBallPositions != null && stillBallPocketed != null)
        {
            for (int i = 0; i < stillBallPositions.Length; i++)
            {
                if (!stillBallPocketed[i] && !allBallControllers[i].ballIsPocketed)
                {
                    // good ball on both sides, update position
                    allBallControllers[i].GetComponent<Rigidbody>().position = stillBallPositions[i];
                }
                else if (stillBallPocketed[i] && !allBallControllers[i].ballIsPocketed)
                {
                    // ball should be pocketed but is not, purge the ball

                    BallController bc = allBallControllers[i];
                    PocketController pc = PocketController.FindeHoleById(Random.Range(1, 7)); // 1, 2, 3, 4, 5, 6

                    pc.DecreaseSplineLength();
                    bc.ballIsPocketed = true;
                    bc.OnSetHoleSpline(pc.splineCurrentLength, pc.initialSplineLength, pc.id);
                    pc.OnBallPocketedHouseKeeping(bc);
                }
                else if (!stillBallPocketed[i] && allBallControllers[i].ballIsPocketed)
                {
                    bool balladded = false;

                    // ball is wrongfully pocketed, revive the ball
                    allBallControllers[i].SpotBallOnTableAndHouseKeeping(stillBallPositions[i], Vector3.zero, Vector3.zero);
                    allBallControllers[i].ballIsPocketed = false;

                    // add back to currentballcontroller
                    for (int j = 0; j < currentBallControllers.Count; j++)
                    {
                        if (currentBallControllers[j].id >= allBallControllers[i].id && allBallControllers[i].id != 0)
                        {
                            currentBallControllers.Insert(j, allBallControllers[i]);

                            balladded = true;

                            break;
                        }
                    }

                    // add at the very end
                    if (!balladded && allBallControllers[i].id != 0)
                    {
                        currentBallControllers.Add(allBallControllers[i]);
                    }
                }
                else
                {
                    // ball is duly pocketed, don't touch it anymore
                }
            }
        }

        stillBallPocketed = null;
        stillBallPositions = null;
    }

    // updates from network
    public void UpdateAllBallStatusFromNetwork(int inShotCount, float inTimeLapsed, bool inMandatory, bool[] inPocketed, Vector3[] inPosition, Vector3[] inVelocity, Vector3[] inAngular, Vector3 inBallShotVelocity, Vector3 inHitBallVelocity, Vector3 inBallShotAngularVelocity)
    {
        // this is the updating shot cue portion
        if (inShotCount == shotCount + 1)
        {
            ballShotVelocity = inBallShotVelocity;
            hitBallVelocity = inHitBallVelocity;
            ballShotAngularVelocity = inBallShotAngularVelocity;

            OnShootCue();
        }

        // this determines if we want to update the stuff...
        if (inShotCount == shotCount)
        {
            if (inTimeLapsed > shootTotalTimeLapsed || inMandatory)
            {
                // make sure both counts agree with each other for future references
                shootTotalTimeLapsed = inTimeLapsed;

                // update for all ball positions start here
                for (int i = 0; i < inPocketed.Length; i++)
                {
                    if (!inPocketed[i] && !allBallControllers[i].ballIsPocketed)
                    {
                        // ball is up on the table on both sides, update its properties

                        allBallControllers[i].ballIsPocketed = inPocketed[i];
                        allBallControllers[i].GetComponent<Rigidbody>().position = inPosition[i];
                        allBallControllers[i].GetComponent<Rigidbody>().velocity = inVelocity[i];
                        allBallControllers[i].GetComponent<Rigidbody>().angularVelocity = inAngular[i];
                    }
                    else if (inPocketed[i] && !allBallControllers[i].ballIsPocketed)
                    {
                        // ball should be pocketed but is not, purge the ball

                        BallController bc = allBallControllers[i];
                        PocketController pc = PocketController.FindeHoleById(Random.Range(1, 7)); // 1, 2, 3, 4, 5, 6

                        pc.DecreaseSplineLength();
                        bc.ballIsPocketed = true;
                        bc.OnSetHoleSpline(pc.splineCurrentLength, pc.initialSplineLength, pc.id);
                        pc.OnBallPocketedHouseKeeping(bc);
                    }
                    else if (!inPocketed[i] && allBallControllers[i].ballIsPocketed)
                    {
                        // ball is wrongfully pocketed, revive the ball

                        bool balladded = false;

                        allBallControllers[i].SpotBallOnTableAndHouseKeeping(inPosition[i], inVelocity[i], inAngular[i]);
                        allBallControllers[i].ballIsPocketed = false;

                        // add back to currentballcontroller
                        for (int j = 0; j < currentBallControllers.Count; j++)
                        {
                            if (currentBallControllers[j].id >= allBallControllers[i].id && allBallControllers[i].id != 0)
                            {
                                currentBallControllers.Insert(j, allBallControllers[i]);

                                balladded = true;

                                break;
                            }
                        }

                        // add at the very end if it is the last one, this looks shitty
                        if (!balladded && allBallControllers[i].id != 0)
                        {
                            currentBallControllers.Add(allBallControllers[i]);
                        }
                    }
                    else
                    {
                        // ball is duly pocketed, don't touch it anymore
                    }
                }
            }
        }
    }

    void FtueCreateAndSortBalls()
    {
        ballsCount = 2; // dajiang hack, just 2 balls

        for (int t = 0; t < deltaPositions.Length; t++) 
        {
            Vector2 tmp = deltaPositions[t];
            int r = Random.Range(t, deltaPositions.Length);
            deltaPositions[t] = deltaPositions[r];
            deltaPositions[r] = tmp;
        }

        ballRadius = 0.500f * ballControllerPrefab.transform.lossyScale.x;
        newBallRadius = ballRadius + ballsDistance;

        allBallControllers = new BallController[ballsCount];

        SmartBotPausePosition = new Vector3[ballsCount];
        SmartBotPauseVelocity = new Vector3[ballsCount];
        SmartBotPauseAngularVelocity = new Vector3[ballsCount];
        SmartBotPauseKinematic = new bool[ballsCount];
        SmartBotPausePocket = new bool[ballsCount];

        Vector3[] initialPositions = new Vector3[ballsCount];
        bool[] initialPockets = new bool[ballsCount];

        for (int i = 0; i < ballsCount; i++)
        {
            // declare ball
            BallController bc = null;
            Vector3 position = Vector3.zero;

            if (i == 0)
            {
                position = mainBallPoint.position + new Vector3(0.0f, 0.0f, StartCube.localScale.z * -0.30f);
            }
            else
            {
                position = firstBallPoint.position + new Vector3(7.25f, 0.0f, 5.25f);
            }

            // the rest
            bc = BallController.Instantiate(ballControllerPrefab) as BallController;
            bc.transform.position = position;
            bc.transform.parent = ballsParent;
            bc.isMain = i == 0;

            if (i == 0)
            {
                cueBallController = bc;
                cueBallController.gameObject.layer = LayerMask.NameToLayer("MainBall");
            }
            else
            {
                bc.gameObject.layer = LayerMask.NameToLayer("Ball");
            }

            bc.id = i;
            bc.cueController = this;
            bc.GetComponent<Renderer>().material.mainTexture = ballTextures[i];

            currentBallControllers.Add(bc);
            allBallControllers[i] = bc;

            initialPositions[i] = bc.transform.position;
            initialPockets[i] = false;
        }

        // this is just a one time thing
        stillBallPocketed = initialPockets;
        stillBallPositions = initialPositions;

        // aim at first ball
        AimAtFirstBall();

        // initializing shadows
        InitializeAllColorShadow();

        // give shadows meanings of life
        if (currentBallControllers.Count > 1)
        {
            UpdateAllColorShadow(currentBallControllers[1].id);
        }

        // play the rack sound
        StartCoroutine(PlayRackSoundLolz());

        // officially kick off ftue sequence
        StartCoroutine(MasterFtueCoRoutine());
    }

    public IEnumerator MasterFtueCoRoutine()
    {
        // part 1, an initial pause and wait till the player reacted to the screen
        yield return new WaitForSeconds(1.00f);

        GameManager_script.Instance().SwipeFtueStage = 0;

        yield return new WaitForEndOfFrame();

        SetFTUEMousePosition();
        ActivateFTUEMouse();
        GC.ShowFtueTextBubbleSwipe();

        // part 1a, wait for players to complete the swipe
        while (ftueSwipeCount < 80)
        {
            yield return null;
        }

        DeactivateFTUEMouse();
        GC.TuckAwayFtueAssets();

        // part 2, another pause before letting players hit the slider
        yield return new WaitForSeconds(0.50f);

        GameManager_script.Instance().SwipeFtueStage = 1;

        yield return new WaitForEndOfFrame();

        forceSlider.SetActive(true);
        GC.ShowFtueTextBubbleShoot();

        // part 2a, wait for players to complete the hit
        while (!ftueShotComplete)
        {
            yield return null;
        }

        forceSlider.SetActive(false);

        // part 3, prepare to dis-engage
        yield return new WaitForSeconds(0.50f);

        // part 3a, get out
        StartCoroutine(OnLoadMainMenu(true, true, 0, false, false, true));
    }

	void NormalCreateAndSortBalls()
	{
        // #1, slight shifts in position of individual balls (+- 0.001f translates to +- 0.05mm)
        Vector3 globalOffset = VectorOperator.generateRandomNumber(0.04f) * VectorOperator.generateRandomXZVector();
        
        for (int t = 0; t < deltaPositions.Length; t++)
        {
            Vector2 tmp = deltaPositions[t];
            int r = Random.Range(t, deltaPositions.Length);
            deltaPositions[t] = deltaPositions[r];
            deltaPositions[r] = tmp;
        }

        ballRadius = 0.500f * ballControllerPrefab.transform.lossyScale.x;
		newBallRadius = ballRadius + ballsDistance;
        ballsCount = ballTextures.Length;

		allBallControllers = new BallController[ballsCount];

        SmartBotPausePosition = new Vector3[ballsCount];
        SmartBotPauseVelocity = new Vector3[ballsCount];
        SmartBotPauseAngularVelocity = new Vector3[ballsCount];
        SmartBotPauseKinematic = new bool[ballsCount];
        SmartBotPausePocket = new bool[ballsCount];

        Vector3[] initialPositions = new Vector3[ballsCount];
        bool[] initialPockets = new bool[ballsCount];

		for (int i = 0; i < ballsCount; i++)
		{
            float deltaX = 0.0f;
            float deltaZ = 0.0f;
            
            if (i == 0 || i == 1)
            {
                deltaX = 0.0f;
                deltaZ = 0.0f;
            }
            else if (i == 9)
            {
                deltaX = 2.0f;
                deltaZ = 0.0f;
            }
            else
            {
                deltaX = deltaPositions[i - 2].x;
                deltaZ = deltaPositions[i - 2].y;
            }
            
            // declare ball
            BallController bc = null;

            // declare ideal position
            Vector3 cueBallPosition = GetGoodCueBallInitialPlacement(mainBallPoint.position);
            Vector3 objectBallPosition = firstBallPoint.position + new Vector3(deltaX * Mathf.Sqrt(Mathf.Pow(2.0f * newBallRadius, 2.0f) - Mathf.Pow(newBallRadius, 2.0f)), 0.0f, deltaZ * newBallRadius);
            Vector3 position = i == 0 ? cueBallPosition : objectBallPosition;

            // #2, overall diamond position and orientation slightly different (+- 0.04f translates to +- 2mm)
            Vector3 localOffset = VectorOperator.generateRandomNumber(0.001f) * VectorOperator.generateRandomXZVector();
			
            // randomize position for object balls
            if (i != 0)
            {
                position += (globalOffset + localOffset);
            }

            // the rest
            bc = BallController.Instantiate(ballControllerPrefab) as BallController;
			bc.transform.position = position;
			bc.transform.parent = ballsParent;
			bc.isMain = i == 0;

			if(i == 0)
			{
				cueBallController = bc;
				cueBallController.gameObject.layer = LayerMask.NameToLayer("MainBall");
            }
			else
			{
				bc.gameObject.layer = LayerMask.NameToLayer("Ball");
			}

			bc.id = i;
			bc.cueController = this;
			bc.GetComponent<Renderer>().material.mainTexture = ballTextures[i];

            currentBallControllers.Add(bc);
			allBallControllers[i] = bc;

            initialPositions[i] = bc.transform.position;
            initialPockets[i] = false;
		}

        // this is just a one time thing
        stillBallPocketed = initialPockets;
        stillBallPositions = initialPositions;

        // aim at first ball
        AimAtFirstBall();

        // initializing shadows
        InitializeAllColorShadow();

        // give shadows meanings of life
        if (currentBallControllers.Count > 1)
        {
            UpdateAllColorShadow(currentBallControllers[1].id);
        }

        // do gay ass spoof when smart bot is master for the first shot
        if (NetworkBotInControl())
        {
            StartCoroutine(ReRackStartingPosition());

            StartCoroutine(OverallPositionAfterReRack());
        }

        // play the rack sound
        StartCoroutine(PlayRackSoundLolz());
    }

    public IEnumerator OverallPositionAfterReRack()
    {
        yield return new WaitForSeconds(0.100f);

        // determine overall position for bot's very first shot
        if (NetworkBotInControl())
        {
            IntentionOverAllFunction();
        }
    }

    public IEnumerator ReRackStartingPosition()
    {
        yield return new WaitForSeconds(0.075f);

        // cue ball position changed
        cueBallController.transform.position = GetGoodCueBallInitialPlacement(mainBallPoint.position);

        // aim at first ball
        AimAtFirstBall();
    }

    public Vector3 GetGoodCueBallInitialPlacement(Vector3 inInitPosition)
    {
        float zOffset = GameManager_script.DetermineLotteryResult(0.50f) ? Random.Range(StartCube.localScale.z * -0.32f, StartCube.localScale.z * -0.12f) : Random.Range(StartCube.localScale.z * 0.12f, StartCube.localScale.z * 0.32f);

        return inInitPosition + new Vector3(0.0f, 0.0f, zOffset);
    }

    public IEnumerator PlayRackSoundLolz()
    {
        yield return new WaitForSeconds(0.150f);

        // choose sound
        float b_r_volume = 1.0f;
        int b_r_index = Random.Range((int)MusicClip.B_Rack_0, (int)MusicClip.B_Rack_1 + 1);

        // play sound
        GameManager_script.Instance().PlaySound(b_r_index, false, b_r_volume);
    }

	public void OnCueBallIsPocketed (bool isPocketed)
	{
        cueBallIsPocketed = isPocketed;
        cueBallController.ballIsPocketed = cueBallIsPocketed;
	}

	void DrawLinesAndSphere()
	{
		if(!cueBallController.ballIsSelected && allIsSleeping)
        {
            OnDrawLinesAndSphere();
        }
    }

    // a big and important function
    void ControleCueThenDraw()
    {
        if (GameManager_script.Instance().CanControlCue)
        {
            bool adding = false;

            // as long as the mouse button is down
            if(menu.GetButton())
            {
                Ray ray = currentCamera.ScreenPointToRay(menu.GetScreenPoint());
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit, 1000.0f, canvasMask))
                {
                    // preping a possible rotation, just in case
                    Vector2 hitPoint = new Vector2(hit.point.x, hit.point.z);
                    Vector2 ballPoint = new Vector2(cuePivot.position.x, cuePivot.position.z);
                    float angle = Vector2.Angle(tempHitPoint - ballPoint, hitPoint - ballPoint);
                    float mag = (tempHitPoint - hitPoint).magnitude;

                    if (menu && !menu.GetButtonDown() && !menu.GetButtonUp() && allIsSleeping && !GameManager_script.Instance().DownOnRealButtons)
                    {
                        // difference small enough, we can trust it
                        if (angle > 0.01f && mag > 0.01f) // 1.2 degrees per swipe is good and we need a good mag as well
                        {
                            // we need slower swipes...
                            float tempCueRotationSpeed = angle * touchSensitivityCurve.Evaluate(Mathf.Clamp01((hitPoint - ballPoint).magnitude / camScreenSize));

                            Vector2 oldPerp = VectorOperator.getLeftPerpendicular(tempHitPoint - ballPoint);
                            Vector2 newFollow = tempHitPoint - hitPoint; 

                            float singleSwipe = Vector2.Dot(oldPerp, newFollow) > 0.0f ? tempCueRotationSpeed : -tempCueRotationSpeed;

                            // add this swipe to the system ya!
                            playerSwipeStorage.Add(singleSwipe);

                            // we are adding to it
                            adding = true;
                        }
                    }

                    // ok? we are changing this line's location?
                    tempHitPoint = hitPoint;
                }
            }
            else
            {
                cueForceValue = sliderForceCurve.Evaluate(Mathf.Clamp01(cueDisplacement / cueMaxDisplacement)) * Mathf.Clamp01(cueDisplacement / cueMaxDisplacement);
                cueForceisActive = false;

                if(cueForceValue > 0.01f)
                {
                    MasterShootCue();
                }
                else
                {
                    cueForceValue = 1.0f;
                    cueRotation.localPosition = cueRotationStrLocalPosition;
                    cueDisplacement = 0.0f;
                }
            }

            RotateCueBasedOnBufferValues(adding);
        }
        else
        {
            cueDisplacement = 0.0f;
        }
    }

    public void RotateCueBasedOnBufferValues(bool inAdding)
    {
        if (playerSwipeStorage.Count > 0)
        {
            // this is total swipes
            float totalSwipes = 0.0f;
            float totalDivide = 0.0f;
            float ftueSwipeSpeed = 0.0f;

            if (playerSwipeStorage.Count > swipeLength || !inAdding)
            {
                // we are at max buffer and can remove one, or we just pure removing
                playerSwipeStorage.RemoveAt(0);
                playerSwipeStorage.TrimExcess();
            }
            
            // we are trying to rotate here
            for (int i = 0; i < playerSwipeStorage.Count; i++)
            {
                totalSwipes += ((i + swipeBase) * playerSwipeStorage[i]);
                totalDivide += (i + swipeBase);
            }

            // find out speed
            if (inAdding)
            {
                ftueSwipeSpeed = totalDivide == 0.0f ? 0.0f : totalSwipes / totalDivide;
            }
            else
            {
                ftueSwipeSpeed = totalSwipes / swipeCount;
            }

            // rotate the cue
            cuePivot.Rotate(Vector3.up, ftueSwipeSpeed);

            // try to count some shiite for ftue, useless otherwise
            if (GameManager_script.Instance().FTUEInActionGame && GameManager_script.Instance().SwipeFtueStage == 0 && Mathf.Abs(ftueSwipeSpeed) > 0.0f)
            {
                ftueSwipeCount += 1;
            }
        }
    }

    public void SetCueControlFromNetwork(float cueNumber, bool isinHand, Vector3 physicalPosition, Quaternion localRotation, Vector3 localPosition, Vector2 displacement)
    {
        // update cue whenever we are a slave
        if (cueNumber != CueCurrentlyInUse)
        {
            CueCurrentlyInUse = cueNumber;
            ChangeCueUIImage(CueCurrentlyInUse);
            UpdateDisplayCue();
        }

        // portion where it updates state of cue ball (some inaccuracies with the init position of cue ball and so forth, but it is ok for now I guess)
        if (isinHand)
        {
            OnPickUpCueBall(physicalPosition);

            ballMovePosition = physicalPosition;
            //cueBallController.rigidbody.position = physicalPosition;

            if (!cueBallController.GetComponent<Rigidbody>().isKinematic)
            {
                cueBallController.GetComponent<Rigidbody>().velocity = Vector3.zero; // we should not set these 2 lines for kinematic
                cueBallController.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // we should not set these 2 lines for kinematic
            }
        }
        else
        {
            //ballMovePosition = physicalPosition;
            cueBallController.GetComponent<Rigidbody>().position = physicalPosition;

            OnDropOffCueBallBySlave();
        }

        // portion where it updates cue ball itself
		cuePivotLocalRotation = localRotation;
		cueRotationLocalPosition = localPosition;
        cueBallPivotLocalPosition = new Vector3(displacement.x, displacement.y, cueBallPivot.z);

        // donno how well would this work but o well... Set to true coz we are catching up i guess...
        AllowCueStatusToUpdate = true;
    }

	void UpdateCueControl(Quaternion localRotation, Vector3 localPosition)
	{
		cuePivot.localRotation = Quaternion.Lerp(cuePivot.localRotation, localRotation, 10.0f * Time.deltaTime); // cue rotation around the table
		cueRotation.localPosition = Vector3.Lerp(cueRotation.localPosition, localPosition, 10.0f * Time.deltaTime); // cue displacement by drawing
        cueBallPivot = Vector3.Lerp(cueBallPivot, cueBallPivotLocalPosition, 10.0f * Time.deltaTime); // cue spin local
    }

    public void ListeningForButtonStates()
    {
        if (menu.GetButtonUp())
        {
            GameManager_script.Instance().DownOnRealButtons = false; // this down on real button thing should probably be left to menu stuff ONLY
        }
    }

    public void SimpleDrawLineAndSphere()
    {
        // set it to null before we start anything else
        currentHitBallController = null;

        if (allIsSleeping)
        {
            transform.position = cueBallController.transform.position;
        }

        Vector2 realRotation = new Vector2(rotationDisplacement.x, rotationDisplacement.y) * GameManager_script.CueAttributes[(int)CueCurrentlyInUse][(int)CueAttributesType.spin];
        float retainedLinearEnergy = ballStrikeEnergyRetention.Evaluate(Mathf.Clamp01(realRotation.magnitude)); // from 100% to 66%
        float retainedAngularEnergy = ballAngularVelocityCurve.Evaluate(Mathf.Clamp01(realRotation.magnitude)); // from 0% to 100% to 60%
        Vector3 pivotVector = Mathf.Clamp(realRotation.y, -1.0f, 1.0f) * cuePivot.right - Mathf.Clamp(realRotation.x, -1.0f, 1.0f) * cuePivot.up;

        ballVelocityOrient = VectorOperator.getProjectXZ(cuePivot.forward, true).normalized; // actual direction
        cueForceValue = sliderForceCurve.Evaluate(Mathf.Clamp01(cueDisplacement / cueMaxDisplacement)) * Mathf.Clamp01(cueDisplacement / cueMaxDisplacement); // some number from 0 to 1
        ballShotVelocity = ballMaxVelocity * cueForceValue * retainedLinearEnergy * ballVelocityOrient;
//        ballShotAngularVelocity = Physics.maxAngularVelocity * cueForceValue * retainedAngularEnergy * pivotVector.normalized;
		//古 
		ballShotAngularVelocity = GetComponent<Rigidbody>().maxAngularVelocity * cueForceValue * retainedAngularEnergy * pivotVector.normalized;

        Ray wallRay = new Ray(cueBallController.GetComponent<Rigidbody>().position, ballVelocityOrient);
        Ray ballRay = new Ray(cueBallController.GetComponent<Rigidbody>().position + 0.25f * ballRadius * ballVelocityOrient, ballVelocityOrient);

        RaycastHit wallHit;
        RaycastHit ballHit;
        RaycastHit currentHit;

        float distanceToWall = 1000.0f;
        float distanceToBall = 1000.0f;

        bool proceedToDrawLineAndSphere = false;

        if (Physics.SphereCast(wallRay, ballRadius, out wallHit, 1000.0f, wallMask))
        {
            distanceToWall = wallHit.distance;
            proceedToDrawLineAndSphere = true;
        }

        if (Physics.SphereCast(ballRay, ballRadius, out ballHit, 1000.0f, ballMask))
        {
            distanceToBall = ballHit.distance;
            proceedToDrawLineAndSphere = true;
        }

        if (proceedToDrawLineAndSphere)
        {
            if (distanceToWall > distanceToBall)
            {
                // we are balling
                currentHitBallController = ballHit.collider.GetComponent<BallController>();
                currentHit = ballHit;
            }
            else
            {
                // we are walling
                currentHitBallController = null;
                currentHit = wallHit;
            }

            if (currentHitBallController && currentBallControllers.Count > 1 && currentHitBallController != currentBallControllers[1])
            {
                if (!collisionTotallyDisabled)
                {
                    collisionSphereRed.GetComponent<Renderer>().enabled = true;
                    collisionSphere.GetComponent<Renderer>().enabled = false;

                    currentCollision = collisionSphereRed;

                    secondCollisionLine.enabled = false;
                    secondCollisionLine1.enabled = false;
                    secondCollisionLine2.enabled = false;

                    ballCollisionLine.enabled = false;
                    ballCollisionLine1.enabled = false;
                    ballCollisionLine2.enabled = false;
                }
            }
            else
            {
                if (!collisionTotallyDisabled)
                {
                    collisionSphereRed.GetComponent<Renderer>().enabled = false;
                    collisionSphere.GetComponent<Renderer>().enabled = true;

                    currentCollision = collisionSphere;

                    secondCollisionLine.enabled = true;
                    secondCollisionLine1.enabled = true;
                    secondCollisionLine2.enabled = true;

                    ballCollisionLine.enabled = true;
                    ballCollisionLine1.enabled = true;
                    ballCollisionLine2.enabled = true;
                }
            }

            currentCollision.position = currentHit.point + ballRadius * currentHit.normal;
            Vector3 outVelocity = VectorOperator.getProjectXZ(ballShotVelocity, true);
            Vector3 cueballVelocityOrient = VectorOperator.getCollisionSphereDirections(ballRadius, ballVelocityOrient, currentCollision.position, wallAndBallMask, 20.0f * ballRadius, currentHit).normalized;
            float angle1 = Mathf.Abs(Vector3.Dot(cueballVelocityOrient, cuePivot.forward.normalized));

            if (ballCollisionLine.enabled) // if it is already enabled...
            {
                ballCollisionLine.enabled = currentHitBallController && allIsSleeping;
                ballCollisionLine1.enabled = currentHitBallController && allIsSleeping;
                ballCollisionLine2.enabled = currentHitBallController && allIsSleeping;
            }

            // we add an offset to make it look better in different camera mode
            Vector3 cameraOffSet1 = is3D ? new Vector3(0.0f, 0.0f, 0.0f) : new Vector3(0.0f, ballRadius, 0.0f);
            float cameraOffSet2 = is3D ? ballRadius : 0.0f;

            CueShootDirection = (currentCollision.position - cueBallController.transform.position).normalized;

            Vector3 fcl_first = cueBallController.transform.position + cameraOffSet1;
            Vector3 fcl_second = fcl_first + CueShootDirection * collisionLineWidth * 0.5f; // 0.5f is the dimensional ratio of the picture itself
            Vector3 fcl_fourth = currentCollision.position + cameraOffSet1;
            Vector3 fcl_third = fcl_fourth - CueShootDirection * collisionLineWidth * 0.5f;

            if ((fcl_fourth - fcl_first).magnitude > collisionLineWidth)
            {
                firstCollisionLine1.SetVertexCount(2);
                firstCollisionLine1.SetPosition(0, fcl_second);
                firstCollisionLine1.SetPosition(1, fcl_first);
            }
            else
            {
                firstCollisionLine1.SetVertexCount(0);
            }

            if ((fcl_fourth - fcl_first).magnitude > collisionLineWidth)
            {
                firstCollisionLine.SetVertexCount(2);
                firstCollisionLine.SetPosition(0, fcl_second);
                firstCollisionLine.SetPosition(1, fcl_third);
            }
            else
            {
                firstCollisionLine.SetVertexCount(0);
            }

            if ((fcl_fourth - fcl_first).magnitude > collisionLineWidth)
            {
                firstCollisionLine2.SetVertexCount(2);
                firstCollisionLine2.SetPosition(1, fcl_fourth);
                firstCollisionLine2.SetPosition(0, fcl_third);
            }
            else
            {
                firstCollisionLine2.SetVertexCount(0);
            }

            if (currentHitBallController)
            {
                // how angles are determined for ball collision
                Vector3 hbvOrient = (currentHitBallController.GetComponent<Rigidbody>().position - currentCollision.position).normalized;
                float angle2 = Mathf.Abs(Vector3.Dot(hbvOrient, cuePivot.forward.normalized));

                TargetPocketDirection = hbvOrient.normalized;
                CurrentBallID = currentHitBallController.id;

                Vector3 bcl_first = currentHitBallController.GetComponent<Rigidbody>().position + cameraOffSet1;
                Vector3 bcl_second = bcl_first + hbvOrient * collisionLineWidth * 0.5f; // 0.5f is the dimensional ratio of the picture itself
                Vector3 bcl_fourth = currentHitBallController.GetComponent<Rigidbody>().position + (angle2 > TenDegree ? angle2 * ballLineLength * extensionFactor.Evaluate(Mathf.Clamp01(angle2)) : 0.0f + cameraOffSet2) * hbvOrient + cameraOffSet1;
                Vector3 bcl_third = bcl_fourth - hbvOrient * collisionLineWidth * 0.5f;

                if ((bcl_fourth - bcl_first).magnitude > collisionLineWidth)
                {
                    ballCollisionLine1.SetVertexCount(2);
                    ballCollisionLine1.SetPosition(0, bcl_second);
                    ballCollisionLine1.SetPosition(1, bcl_first);
                }
                else
                {
                    ballCollisionLine1.SetVertexCount(0);
                }

                if ((bcl_fourth - bcl_first).magnitude > collisionLineWidth)
                {
                    ballCollisionLine.SetVertexCount(2);
                    ballCollisionLine.SetPosition(0, bcl_second);
                    ballCollisionLine.SetPosition(1, bcl_third);
                }
                else
                {
                    ballCollisionLine.SetVertexCount(0);
                }

                if ((bcl_fourth - bcl_first).magnitude > collisionLineWidth)
                {
                    ballCollisionLine2.SetVertexCount(2);
                    ballCollisionLine2.SetPosition(1, bcl_fourth);
                    ballCollisionLine2.SetPosition(0, bcl_third);
                }
                else
                {
                    ballCollisionLine2.SetVertexCount(0);
                }

                // dajiang physics 4, this is how the angle is determined for subsequent cue ball movements
                CueBallBounceDirection = cueballVelocityOrient.normalized;

                Vector3 scl_first = currentCollision.position + cameraOffSet1;
                Vector3 scl_second = scl_first + CueBallBounceDirection * collisionLineWidth * 0.5f; // 0.5f is the dimensional ratio of the picture itself
                Vector3 scl_fourth = currentCollision.position + (angle1 > TenDegree ? angle1 * ballLineLength * extensionFactor.Evaluate(Mathf.Clamp01(angle1)) : 0.0f + cameraOffSet2) * cueballVelocityOrient.normalized + cameraOffSet1;
                Vector3 scl_third = scl_fourth - CueBallBounceDirection * collisionLineWidth * 0.5f;

                if ((scl_fourth - scl_first).magnitude > collisionLineWidth)
                {
                    secondCollisionLine1.SetVertexCount(2);
                    secondCollisionLine1.SetPosition(0, scl_second);
                    secondCollisionLine1.SetPosition(1, scl_first);
                }
                else
                {
                    secondCollisionLine1.SetVertexCount(0);
                }

                if ((scl_fourth - scl_first).magnitude > collisionLineWidth)
                {
                    secondCollisionLine.SetVertexCount(2);
                    secondCollisionLine.SetPosition(0, scl_second);
                    secondCollisionLine.SetPosition(1, scl_third);
                }
                else
                {
                    secondCollisionLine.SetVertexCount(0);
                }

                if ((scl_fourth - scl_first).magnitude > collisionLineWidth)
                {
                    secondCollisionLine2.SetVertexCount(2);
                    secondCollisionLine2.SetPosition(1, scl_fourth);
                    secondCollisionLine2.SetPosition(0, scl_third);
                }
                else
                {
                    secondCollisionLine2.SetVertexCount(0);
                }
            }
            else
            {
                // dajiang physics 4, this is how the angle is determined for subsequent cue ball movements
                CueBallBounceDirection = cueballVelocityOrient.normalized;

                Vector3 scl_first = currentCollision.position + cameraOffSet1;
                Vector3 scl_second = scl_first + CueBallBounceDirection * collisionLineWidth * 0.5f; // 0.5f is the dimensional ratio of the picture itself
                Vector3 scl_fourth = currentCollision.position + (ballLineLength + cameraOffSet2) * cueballVelocityOrient + cameraOffSet1;
                Vector3 scl_third = scl_fourth - CueBallBounceDirection * collisionLineWidth * 0.5f;

                if ((scl_fourth - scl_first).magnitude > collisionLineWidth)
                {
                    secondCollisionLine1.SetVertexCount(2);
                    secondCollisionLine1.SetPosition(0, scl_second);
                    secondCollisionLine1.SetPosition(1, scl_first);
                }
                else
                {
                    secondCollisionLine1.SetVertexCount(0);
                }

                if ((scl_fourth - scl_first).magnitude > collisionLineWidth)
                {
                    secondCollisionLine.SetVertexCount(2);
                    secondCollisionLine.SetPosition(0, scl_second);
                    secondCollisionLine.SetPosition(1, scl_third);
                }
                else
                {
                    secondCollisionLine.SetVertexCount(0);
                }

                if ((scl_fourth - scl_first).magnitude > collisionLineWidth)
                {
                    secondCollisionLine2.SetVertexCount(2);
                    secondCollisionLine2.SetPosition(1, scl_fourth);
                    secondCollisionLine2.SetPosition(0, scl_third);
                }
                else
                {
                    secondCollisionLine2.SetVertexCount(0);
                }

                ballCollisionLine1.SetVertexCount(0);
                ballCollisionLine.SetVertexCount(0);
                ballCollisionLine2.SetVertexCount(0);

                TargetPocketDirection = Vector3.zero; // always try to zero it out whenever possible
                CurrentBallID = -1;
            }
        }
    }

    // this is where logics are in onDrawLinesAndSphere and is stopping cue from being shot
	public void OnDrawLinesAndSphere()
	{
        // this function actually controls the cue rotation as well as pivots
        if (BotMasterInControl() || SoloMasterInControl() || NetworkMasterInControl() || SoloSecondPersonInControl())
        {
            ControleCueThenDraw();
        }

        // update cue 
        if (NetworkSlaveInControl() && AllowCueStatusToUpdate)
        {
            UpdateCueControl(cuePivotLocalRotation, cueRotationLocalPosition); // lerps
        }

        // smart bot update is NOT here
        if (NetworkBotInControl())
        {
            // do NOTHING
        }

        // no pre-reqs 
        SimpleDrawLineAndSphere();
    }

    // both bots update here
    public void RobotUpdate()
    {
        // stupid bot happens, stupid bot is really kinda dumb
        if (BotInControl())
        {
            StupidRobotUpdate();
        }

        // smart bot
        if (NetworkBotInControl() && !GameManager_script.Instance().SmartBotFreezeDueToInternet && !GameManager_script.Instance().SmartBotFreezeDueToFinish)
        {
            SmartRobotUpdate();
        }
    }

	public void OnHideLineAndSphere()
	{
        collisionTotallyDisabled = true;

        collisionSphereRed.GetComponent<Renderer>().enabled = false;
        collisionSphere.GetComponent<Renderer>().enabled = false;

        currentCollision = collisionSphere;

        firstCollisionLine.enabled = false;
        firstCollisionLine1.enabled = false;
        firstCollisionLine2.enabled = false;

        secondCollisionLine.enabled = false;
        secondCollisionLine1.enabled = false;
        secondCollisionLine2.enabled = false;

        ballCollisionLine.enabled = false;
        ballCollisionLine1.enabled = false;
        ballCollisionLine2.enabled = false;
    }

	public void OnEnableLineAndSphere()
	{
        collisionTotallyDisabled = false;

        GameManager_script.Instance().CanControlCue = true;

		cueForceValue = 1.0f;

        collisionSphereRed.GetComponent<Renderer>().enabled = true;
        collisionSphere.GetComponent<Renderer>().enabled = true;

        currentCollision = collisionSphere;

        firstCollisionLine.enabled = true;
        firstCollisionLine1.enabled = true;
        firstCollisionLine2.enabled = true;

        secondCollisionLine.enabled = true;
        secondCollisionLine1.enabled = true;
        secondCollisionLine2.enabled = true;

        ballCollisionLine.enabled = true;
        ballCollisionLine1.enabled = true;
        ballCollisionLine2.enabled = true;
    }

    public void OnControlCue()
	{
        // update cue whenever we are NOT a slave
        if (BotMasterInControl() || SoloMasterInControl() || NetworkMasterInControl() || SoloSecondPersonInControl())
        {
            if (CueCurrentlyInUse != GameManager_script.Instance().selfGameProfileInfo.cueEquipped)
            {
                CueCurrentlyInUse = GameManager_script.Instance().selfGameProfileInfo.cueEquipped;
                ChangeCueUIImage(CueCurrentlyInUse);
                UpdateDisplayCue();
            }
        }

        if (BotInControl() || NetworkBotInControl())
        {
            if (GameManager_script.Instance().otherGameProfileInfo != null && CueCurrentlyInUse != GameManager_script.Instance().otherGameProfileInfo.cueEquipped)
            {
                CueCurrentlyInUse = GameManager_script.Instance().otherGameProfileInfo.cueEquipped;
                ChangeCueUIImage(CueCurrentlyInUse);
                UpdateDisplayCue();
            }
        }

        // actually move the freaking cue around
        if (NetworkBotInControl() || BotMasterInControl() || SoloMasterInControl() || NetworkMasterInControl() || SoloSecondPersonInControl() || BotInControl())
        {
            float x = ballRadius * cueBallPivot.x;
            float y = ballRadius * cueBallPivot.y;
            float z = -Mathf.Sqrt(Mathf.Clamp(Mathf.Pow(ballRadius, 2.0f) - (Mathf.Pow(x, 2.0f) + Mathf.Pow(y, 2.0f)), 0.0f, Mathf.Pow(ballRadius, 2.0f)));

            rotationDisplacement = (1.0f / (ballRadius)) * (new Vector2(x, y));
            tempCuePosition = new Vector3(x, y, z);

            // since cueRotation.localPosition is being updated every frame, we should not have any logics depending on it anymore
            cueRotation.localPosition = tempCuePosition - cueDisplacement * Vector3.forward;
        }
    }

    public void UpdateDisplayCue()
    {
        // update cue appearance right here!
        ChangeCue((int)CueCurrentlyInUse);

        // update cue attributes right here
        if ((int)CueCurrentlyInUse >= 0 && (int)CueCurrentlyInUse < GameManager_script.CueAttributes.Length)
        {
            // extension line
            ballLineLength = GameManager_script.CueAttributes[(int)CueCurrentlyInUse][(int)CueAttributesType.extend];

            // force and speed, I will change the max speed for now, but in the future, we will change the length of the cue slider as well?
            ballMaxVelocity = GameManager_script.CueAttributes[(int)CueCurrentlyInUse][(int)CueAttributesType.force];

            // max angular lolz
   //        Physics.maxAngularVelocity = ballMaxVelocity * 1.4286f; // 5 / 7 * 2
			GetComponent<Rigidbody>().maxAngularVelocity = ballMaxVelocity * 1.4286f; // 5 / 7 * 2
        }
    }

    public Vector3 givemeagoodposition(Vector3 inPosition)
    {
        int trycount = 0;
        Vector3 position = inPosition;
        bool direction = inPosition.x < centerPoint.position.x ? true : false;

        while (trycount < 50)
        {
            trycount++;

            Ray ray = new Ray(position + 3.0f * ballRadius * Vector3.up, -Vector3.up);
            bool hittingCanvas = false;
            RaycastHit hit;

            if (Physics.SphereCast(ray, 1.0f * ballRadius, out hit, 1000.0f, canvasAndBallMask))
            {
                hittingCanvas = hit.collider.gameObject.layer == LayerMask.NameToLayer("Canvas");

                if (hittingCanvas)
                {
                    return new Vector3(position.x, mainBallPoint.position.y, position.z);
                }
                else
                {
                    // always move z axis towards the center point, 2.0f * ballRadius at a time
                    position = changePosition(position, direction);
                }
            }
        }

        return new Vector3(position.x, mainBallPoint.position.y, position.z);
    }

    Vector3 changePosition(Vector3 position, bool direction)
    {
        if (direction)
        {
            position += new Vector3(0.5f * ballRadius, 0.0f, 0.0f);
        }
        else
        {
            position -= new Vector3(0.5f * ballRadius, 0.0f, 0.0f);
        }

        return position;
    }

	void MasterShootCue()
    {
        instaBallShotVelocity = ballShotVelocity;
        instaHitBallVelocity = hitBallVelocity;
        instaBallShotAngularVelocity = ballShotAngularVelocity;

        OnShootCue();

        if (NetworkMasterInControl())
        {
            MasterBallStatusUpdate(true); // immediate send over
        }
    }

    public void starttimer()
    {
        incrementTimer = true;
        incrementalTimer = 0.0f;

        totalStartTimer = Time.realtimeSinceStartup;
        totalEndTimer = Time.realtimeSinceStartup;
        
        // swap the profile background images
        if (NetworkSlaveInControl() || NetworkBotInControl() || BotInControl() || SoloSecondPersonInControl())
        {
            PlayerHead1.GetComponent<GamePlayerHead>().ChangHeadBackground(PlayerHeadInfo.Default);
            PlayerHead2.GetComponent<GamePlayerHead>().ChangHeadBackground(PlayerHeadInfo.Light);
        }
        else if (NetworkMasterInControl() || BotMasterInControl() || SoloMasterInControl())
        {
            PlayerHead1.GetComponent<GamePlayerHead>().ChangHeadBackground(PlayerHeadInfo.Light);
            PlayerHead2.GetComponent<GamePlayerHead>().ChangHeadBackground(PlayerHeadInfo.Default);
        }
    }

    public void stoptimer()
    {
        incrementTimer = false;
        incrementalTimer = 0.0f;
        totalStartTimer = Time.realtimeSinceStartup;
        totalEndTimer = Time.realtimeSinceStartup;
    }

    // this is the primary cue shooting, without sending the data over to the network others
    public void OnShootCue()
    {
        if(allIsSleeping)
        {
            // stop timer, it used to be outside the all is sleeping and bug free
            stoptimer();

            // stop clock music
            GameManager_script.Instance().StopClockMusic();

            // choose sound
            float b_c_volume = Mathf.Clamp01(cueDisplacement / cueMaxDisplacement);
            int b_c_index = 0;

            if (b_c_volume > 0.66f)
            {
                b_c_index = Random.Range((int)MusicClip.B_C_Hard_0, (int)MusicClip.B_C_Hard_0 + 1);
            }
            else if (b_c_volume > 0.33f)
            {
                b_c_index = Random.Range((int)MusicClip.B_C_Mid_0, (int)MusicClip.B_C_Mid_0 + 1);
            }
            else
            {
                b_c_index = Random.Range((int)MusicClip.B_C_Weak_0, (int)MusicClip.B_C_Weak_0 + 1);
            }

            // play sound
            GameManager_script.Instance().PlaySound(b_c_index, false, b_c_volume);

            // universal and place that increments a shot count for everyone, even network slaves
            shotCount++;

            // reset shoot timer
            shootTimeStamp = 0.0f;
            shootMandatoryStamp = 0.0f;

            // reset shoot time stamp for network lags
            shootTotalTimeLapsed = 0.0f;

            // reset rule related things, because these resetting doesn't appear to work immediately after queue switching
            firstBallBallCollisionSinceShot = true;

            pocketedTheCueBall = false;
            pocketedAnyObjectBall = false;
            pocketedNineBall = false;

            hittingTheRightFirstBall = false;

            allIsSleeping = false;
            inMove = true;

            // reset cue pivot
            cueBallPivot = Vector3.zero;

            OnHideLineAndSphere();
            OnCueBallIsPocketed(false);
            GC.CleanUpAllToolTipRelatedUI();

            cueRotation.localPosition = tempCuePosition + cueRotationStrLocalPosition;
            cueDisplacement = 0.0f;

            foreach (BallController item in currentBallControllers)
            {
                item.inMove = !allIsSleeping;
            }

            cueBallController.OnShootCueBall();

            // if the controller comes from this client
            if (BotMasterInControl() || SoloMasterInControl() || NetworkMasterInControl() || NetworkBotInControl() || BotInControl() || SoloSecondPersonInControl())
            {
                StopCoroutine("WaitTillAllBallsStopsMoving");
                StartCoroutine("WaitTillAllBallsStopsMoving");
            }
            else if (NetworkSlaveInControl())
            {
                StopCoroutine("SlaveWaitTillAllBallsStopsMoving");
                StartCoroutine("SlaveWaitTillAllBallsStopsMoving");
            }

            // this is the criteria for tracking a particular shot as player 1
            if (NetworkMasterInControl() || BotMasterInControl() || SoloMasterInControl())
            {
                TrackingShotAsPlayerOne = true;
                TrackingShotAsPlayerTwo = false;
            }
            else if (NetworkSlaveInControl() || NetworkBotInControl() || BotInControl() || SoloSecondPersonInControl())
            {
                TrackingShotAsPlayerOne = false;
                TrackingShotAsPlayerTwo = true;
            }
            else
            {
                TrackingShotAsPlayerOne = false;
                TrackingShotAsPlayerTwo = false;
            }

            // gotta do an animation and NOT a straight disappearance. Hide force slider for now 
            forceSlider.SetActive(false);

            // tuck away ftue o ya...
            GC.TuckAwayFtueAssets();

            // make sure all shadow stuff return to normal
            UpdateAllColorShadow();

            // make sure we don't use hand cursor no more
            DeactivateHandCursor();

            // clear smart bot related stuff
            if (NetworkBotInControl())
            {
                ClearAllSmartBotIntentionStuff();
            }
        }
	}

    public void InitializeAllColorShadow()
    {
        ShadowManager sm = gameObject.GetComponent<ShadowManager>();

        sm.InitShadow(this); // give black to all shadows
    }

    public void UpdateAllColorShadow(int inColorNumber = 0)
    {
        ShadowManager sm = gameObject.GetComponent<ShadowManager>();

        sm.UpdateShadow(this, inColorNumber);
    }

    // just do some minor house keeping here since the majority is done when the network msg is sent over since "positions == null" is not very reliable
	IEnumerator SlaveWaitTillAllBallsStopsMoving()
    {
        while (!allIsSleeping)
        {
            yield return null;
        }

        // sure why not
        CleanOutShotRelatedVars();

        // should we do this as well?
        cueBallIsPocketed = false;

        // reset cue ball position at table
        OnReSetCueBallPosition();

        // aim at first ball?
        AimAtFirstBall();

        // disable cue status update till re-opened by a network update
        AllowCueStatusToUpdate = false;

        // re-enable rendering for lines and spheres
        OnEnableLineAndSphere();
    }

    // this waits for all balls to settle down, make sure this is only called once!
	IEnumerator WaitTillAllBallsStopsMoving()
	{
        while (!allIsSleeping)
        {
            yield return null;
        }

        // dajiang hack (for faking smart bot game freezes)
        if (NetworkBotInControl() && (GameManager_script.Instance().SmartBotFreezeDueToInternet || GameManager_script.Instance().SmartBotFreezeDueToFinish))
        {
            yield break;
        }

        // dajiang hack (for ftue shooting)
        if (GameManager_script.Instance().FTUEInActionGame)
        {
            ftueShotComplete = true;
        }

        bool isFoul = false;

        if (incrementalTimer > GameManager_script.Instance().TimerCap) // out of time
        {
            // transfer control with ball in hand
            CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_MOVABLE_ON_TABLE;

            isFoul = true;
            pushoutAllowed = false;
            skipAllowed = false;

            ShowHelpfulTooltipPopup("", "OutOfTime", true, false);

            if (NetworkMasterInControl())
            {
                ServerController.serverController.SendRPCToNetworkViewOthers("OnHelpfulTipReceived", "OutOfTime");
            }

            DeactivateHandCursor(); // de-activate cursor coz its not yours anymore

            FlipsControlSwitch(isFoul);
        }
        // if we skip a shot and physically call this place
        else if (skipCalled)
        {
            CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_FIXED_ON_TABLE;

            pushoutAllowed = false;
            skipAllowed = false;

            FlipsControlSwitch(isFoul);
        }
        // first shot if legal or not
        else if (shotCount == 1)
        {
            if (pocketedTheCueBall) // cue ball pocketed
            {
                // transfer control with ball in hand
                CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_MOVABLE_ON_TABLE;

                isFoul = true;
                pushoutAllowed = false;

                ShowHelpfulTooltipPopup("", "PocketCueBall", true, false);

                if (NetworkMasterInControl())
                {
                    ServerController.serverController.SendRPCToNetworkViewOthers("OnHelpfulTipReceived", "PocketCueBall");
                }

                FlipsControlSwitch(isFoul);
            }
            else if (wallHitCount <= 4 && !pocketedAnyObjectBall) // less than 4 balls hitting the rail + no ball is pocketed
            {
                // transfer control with ball in hand
                CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_MOVABLE_ON_TABLE;

                isFoul = true;
                pushoutAllowed = false;

                ShowHelpfulTooltipPopup("", "FourBallRail", true, false);

                if (NetworkMasterInControl())
                {
                    ServerController.serverController.SendRPCToNetworkViewOthers("OnHelpfulTipReceived", "FourBallRail");
                }

                FlipsControlSwitch(isFoul);
            }
            else if (!hittingTheRightFirstBall)
            {
                // transfer control with ball in hand
                CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_MOVABLE_ON_TABLE;

                isFoul = true;
                pushoutAllowed = false;

                ShowHelpfulTooltipPopup("", "RightFirstBall", true, false);

                if (NetworkMasterInControl())
                {
                    ServerController.serverController.SendRPCToNetworkViewOthers("OnHelpfulTipReceived", "RightFirstBall");
                }

                FlipsControlSwitch(isFoul);
            }
            else if (!pocketedAnyObjectBall) // no ball is pocketed
            {
                // transfer control with ball fixed
                CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_FIXED_ON_TABLE;

                pushoutAllowed = true;

                FlipsControlSwitch(isFoul);

                GameManager_script.IncrementShotsMissed(TrackingShotAsPlayerOne, TrackingShotAsPlayerTwo);
            }
            else
            {
                // continue hitting
                CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_FIXED_ON_TABLE;

                CheckForSnookerSelf();

                pushoutAllowed = true;
            }

            skipAllowed = false;
        }
        // second shot if legal or not (case of push out)
        else if (shotCount == 2 && pushoutCalled) // and calls pushout
        {
            if (pocketedTheCueBall) // cue ball pocketed
            {
                // transfer control with ball in hand
                CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_MOVABLE_ON_TABLE;

                isFoul = true;
                skipAllowed = false;

                ShowHelpfulTooltipPopup("", "PocketCueBall", true, false);

                if (NetworkMasterInControl())
                {
                    ServerController.serverController.SendRPCToNetworkViewOthers("OnHelpfulTipReceived", "PocketCueBall");
                }

                FlipsControlSwitch(isFoul);
            }
            else
            {
                // always transfer to opponent
                CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_FIXED_ON_TABLE;

                skipAllowed = true;

                FlipsControlSwitch(isFoul);
            }

            pushoutAllowed = false;
        }
        // all other shots
        else
        {
            if (pocketedTheCueBall) // cue ball pocketed
            {
                // transfer control with ball in hand
                CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_MOVABLE_ON_TABLE;

                isFoul = true;

                ShowHelpfulTooltipPopup("", "PocketCueBall", true, false);

                if (NetworkMasterInControl())
                {
                    ServerController.serverController.SendRPCToNetworkViewOthers("OnHelpfulTipReceived", "PocketCueBall");
                }

                FlipsControlSwitch(isFoul);
            }
            else if (!hittingTheRightFirstBall) // didn't hit the right first ball
            {
                // transfer control with ball in hand
                CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_MOVABLE_ON_TABLE;

                isFoul = true;

                ShowHelpfulTooltipPopup("", "RightFirstBall", true, false);

                if (NetworkMasterInControl())
                {
                    ServerController.serverController.SendRPCToNetworkViewOthers("OnHelpfulTipReceived", "RightFirstBall");
                }

                FlipsControlSwitch(isFoul);
            }
            else if (!pocketedAnyObjectBall && wallHitCount == 0) // no rail after contact and no pockets
            {
                // transfer control with ball in hand
                CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_MOVABLE_ON_TABLE;

                isFoul = true;

                ShowHelpfulTooltipPopup("", "BallRailContact", true, false);

                if (NetworkMasterInControl())
                {
                    ServerController.serverController.SendRPCToNetworkViewOthers("OnHelpfulTipReceived", "BallRailContact");
                }

                FlipsControlSwitch(isFoul);
            }
            else if (!pocketedAnyObjectBall) // no ball is pocketed
            {
                // transfer control with ball fixed
                CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_FIXED_ON_TABLE;

                FlipsControlSwitch(isFoul);

                GameManager_script.IncrementShotsMissed(TrackingShotAsPlayerOne, TrackingShotAsPlayerTwo);
            }
            else
            {
                // continue hitting
                CueControllerUpdater.current_control_status = CueControllerUpdater.CUE_BALL_FIXED_ON_TABLE;

                CheckForSnookerSelf();
            }

            pushoutAllowed = false;
            skipAllowed = false;
        }

        // spot cue ball if necessary
        if (pocketedTheCueBall)
        {
            cueBallController.SpotBallOnTableAndHouseKeeping(centerPoint.position, Vector3.zero, Vector3.zero);
            cueBallController.ballIsPocketed = false;
        }

        // increment smart bot stats and shiite
        if (GameManager_script.Instance().SmartBotInActionGame && BestBotShootPosition != null)
        {
            GameManager_script.Instance().Smart_Bot_Single_Long_Shot_Taken += BestBotShootPosition.smartBotLongShot;
            GameManager_script.Instance().Smart_Bot_Single_Long_Shot_Made += BestBotShootPosition.smartBotIntendToPocket;
        }

        // spot it or whatever
        if (pocketedNineBall)
        {
            if (isFoul || (shotCount == 2 && pushoutCalled))
            {
                // get 9 ball always the last ball in all ball controller, dangerous but ok...
                BallController bc = allBallControllers[allBallControllers.Length - 1];

                // spot 9 ball is necessary (add back to the currentBallController)
                bc.SpotBallOnTableAndHouseKeeping(firstBallPoint.position, Vector3.zero, Vector3.zero);
                bc.ballIsPocketed = false;

                // add 9 ball back to current ball controller, cue ball don't need this coz its never gone
                currentBallControllers.Add(bc);
            }
            else
            {
                // visual update will be taken care of by coin awake. Supposedly, currenWager should only be non-zero for network-ever games
                if (NetworkMasterInControl())
                {
                    GameManager_script.Instance().UpdateCoinCount(2.0f * GameManager_script.Instance().CurrentWager);
                }

                // game is practically done, if we are doing smart bot stuff, need to tell it we should stop updating
                if (GameManager_script.Instance().SmartBotInActionGame)
                {
                    GameManager_script.Instance().SmartBotFreezeDueToFinish = true;
                }
                else
                {
                    GameManager_script.Instance().SmartBotFreezeDueToFinish = false;
                }

                // loads slave main menu
                if (GameManager_script.Instance().StartingOutAsANetWorkGame)
                {
                    ServerController.serverController.SendRPCToNetworkViewOthers("OnGameFinished", false);
                }

                // loads my own main menu
                if (NetworkMasterInControl())
                {
                    StartCoroutine(OnLoadMainMenu(true, true));
                }
                else if (NetworkBotInControl())
                {
                    StartCoroutine(OnLoadMainMenu(false, true));
                }
                else if (SoloMasterInControl() && !GameManager_script.Instance().StartingOutAsANetWorkGame)
                {
                    StartCoroutine(OnLoadMainMenu(true, true, 1));
                }
                else if (SoloSecondPersonInControl() && !GameManager_script.Instance().StartingOutAsANetWorkGame)
                {
                    StartCoroutine(OnLoadMainMenu(true, true, 2));
                }
                else if (BotInControl())
                {
                    StartCoroutine(OnLoadMainMenu(false, true));
                }
                else if (BotMasterInControl())
                {
                    StartCoroutine(OnLoadMainMenu(true, true));
                }
                else
                {
                    StartCoroutine(OnLoadMainMenu(false, true)); // this is a fail save
                }
            }
        }

        // foul but not out of time fouls
        if (isFoul && incrementalTimer <= GameManager_script.Instance().TimerCap)
        {
            GameManager_script.IncrementScratch(TrackingShotAsPlayerOne, TrackingShotAsPlayerTwo);
        }

        // end of shot action
        EndOfShotAction();

        // house keeping
        EndOfShotHouseKeeping();
	}

    public void CheckForSnookerSelf()
    {
        if (currentBallControllers.Count > 1)
        {
            // since there is no foul and we can keep hitting, we can safely assume currentBallControllers[1] is our new target
            // one exception is when player call pushout (something not known at this point), we need to tweak the logic
            // dajiang hack WHENEVER we pushout, we should delete all snooker count of that game, trust me

            Vector3 straightAhead = (currentBallControllers[1].GetComponent<Rigidbody>().position - cueBallController.GetComponent<Rigidbody>().position).normalized;
            Vector3 counterClockwiseAhead = (currentBallControllers[1].GetComponent<Rigidbody>().position - cueBallController.GetComponent<Rigidbody>().position + VectorOperator.counterClockWisePerp(straightAhead) * ballRadius * 2.0f).normalized;
            Vector3 clockwiseAhead = (currentBallControllers[1].GetComponent<Rigidbody>().position - cueBallController.GetComponent<Rigidbody>().position + VectorOperator.clockWisePerp(straightAhead) * ballRadius * 2.0f).normalized;

            bool straightHit = CueBallWillHitTargetBall(cueBallController, cueBallController.GetComponent<Rigidbody>().position, straightAhead, currentBallControllers[1]);
            bool counterClockWiseHit = CueBallWillHitTargetBall(cueBallController, cueBallController.GetComponent<Rigidbody>().position, counterClockwiseAhead, currentBallControllers[1]);
            bool clockWiseHit = CueBallWillHitTargetBall(cueBallController, cueBallController.GetComponent<Rigidbody>().position, clockwiseAhead, currentBallControllers[1]);

            if (!straightHit && !counterClockWiseHit && !clockWiseHit)
            {
                GameManager_script.IncrementSnookerSelf(TrackingShotAsPlayerOne, TrackingShotAsPlayerTwo);
            }
        }
    }

    public void EndOfShotAction()
    {
        // this is only called if you are a master in the shot that just happened.
        // at this moment, you could be a slave or a master (this is the previous shot's shot owner)
        if (GameManager_script.Instance().StartingOutAsANetWorkGame)
        {
            // create stillBallPocketed, etc
            stillBallPocketed = new bool[allBallControllers.Length];
            stillBallPositions = new Vector3[allBallControllers.Length];

            // update final ball position and pocket info
            for (int i = 0; i < allBallControllers.Length; i++)
            {
                stillBallPocketed[i] = allBallControllers[i].ballIsPocketed;
                stillBallPositions[i] = allBallControllers[i].GetComponent<Rigidbody>().position;
            }

            // time to give opponent a honest update
            ServerController.serverController.SendRPCToNetworkViewOthers("OnStatusUpdateStillPositions", shotCount, !ServerController.serverController.playerInControl, pushoutAllowed, pushoutCalled, CueControllerUpdater.current_control_status, stillBallPocketed, stillBallPositions, false);

            // destroy them immediately after init or use
            stillBallPocketed = null;
            stillBallPositions = null;
        }

        // clear all bot related vars no matter what
        BotBeginShotHouseKeeping();

        // dumb bot
        if (BotInControl())
        {
            DetermineListOfGoodPositions();
        }

        // smart robot 
        if (NetworkBotInControl())
        {
            IntentionOverAllFunction();
        }
    }

    public void EndOfShotHouseKeeping()
    {
        // reset the disconnected timer and start over
        GameManager_script.Instance().timeSinceDisconnected = 0.0f;

        // house keeping
        CleanOutShotRelatedVars();

        // should we do this as well?
        cueBallIsPocketed = false;

        // set tolerate level just for the hack of it
        TolerateLevel = Random.Range(LowerThreshold, PerfectAngle);

        // reset playerSwipeStorage for new swipes
        if (playerSwipeStorage.Count > 0)
        {
            playerSwipeStorage.Clear();
            playerSwipeStorage.TrimExcess();
        }

        // settle all the balls in hole stuff
        OnSettlePocketedAndDisplayBallsPositions();

        // re-enable rendering for lines and spheres
        OnEnableLineAndSphere();

        // give pushout call a chance
        OnShowPushOutOption();

        // give pushout call a chance
        OnShowSkipOption();

        // reset cue ball position at table
        OnReSetCueBallPosition();

        // auto aim at first good ball's direction
        AimAtFirstBall();

        // make sure it is back to its original position
        OnResetForceSlider();

        // refresh this either way?
        ConditionalEnableForceSlider();

        // disable clock music
        GameManager_script.Instance().StopClockMusic();

        // start timer
        starttimer();

        // give shadows meanings of life
        if (currentBallControllers.Count > 1)
        {
            UpdateAllColorShadow(currentBallControllers[1].id);
        }

        // reset change spin shadow back to 0,0
        if (SpinShadow)
        {
            SpinShadow.GetComponent<GameSpinShadow>().ChangeSpinShadowCenterpoint(0.0f, 0.0f);

            if (SpinShadow.GetComponent<GameSpinShadow>().SpinNaked)
            {
                SpinShadow.GetComponent<GameSpinShadow>().SpinNaked.GetComponent<GameSpinNaked>().Centerpoint.gameObject.transform.localPosition = Vector3.zero;
            }
        }

        // reset actual cue spin if i have not already done so
        cueBallPivot = Vector3.zero;
    }

    public void CleanOutShotRelatedVars()
    {
        hittingTheRightFirstBall = false;

        pocketedTheCueBall = false;
        pocketedAnyObjectBall = false;
        pocketedNineBall = false;

        pushoutCalled = false;
        skipCalled = false;

        contactedAtLeastOneRealBall = false;
        wallHitCount = 0;
    }

    public void ConditionalEnableForceSlider()
    {
        if (forceSlider)
        {
            if (NetworkSlaveInControl() || NetworkBotInControl() || BotInControl())
            {
                forceSlider.SetActive(false);
            }
            else
            {
                forceSlider.SetActive(true);
            }
        }
    }

    // reset cue ball position at table
    // this didn't completely fix the issue, well kinda. Whenever this is triggered, cue ball auto aim direction will be messed up for that one shot only.
    public void OnReSetCueBallPosition()
    {
        if (cueBallController.GetComponent<Rigidbody>().position.y - mainBallPoint.position.y > 1.0f) // dajiang hack, 1.0f is arbitrary
        {
            OnDropOffCueBall();

            cueBallController.ballIsPocketed = false;

            cuePivot.localRotation = Quaternion.identity; // this dude messes up the auto aim for that one shot
        }
    }

    // clean up force slider
    public void OnResetForceSlider()
    {
        cueDisplacement = 0.0f;
    }

    // dajiang hack, i might wanna just the last 2 positions _end_1 and _end_2 stuff, or whatever
    public void OnSettlePocketedAndDisplayBallsPositions()
    {
        // always use pocket 1 as a proxy, doesn't hurt
        AnimationSpline ans = PocketController.FindeHoleById(2).finalBallSpline;
        float spline = PocketController.FindeHoleById(2).splineTotalLength;

        if (pocketedBallControllers != null)
        {
            // update positions of all available balls in the pocket ONCE AND FOR ALL
            foreach (BallController pbc in pocketedBallControllers)
            {
                pbc.GetComponent<Rigidbody>().position = ans.Evaluate(spline);

                pbc.GetComponent<Rigidbody>().position += ThreeDOffset;

                spline -= 2.0f * ballRadius;
            }
        }

        // we also want to settle all the display ball positions
        for (int i = 1; i < allBallControllers.Length; i++) // screw 0, thats cue ball
        {
            if (GC)
            {
                if (allBallControllers[i].ballIsPocketed)
                {
                    GC.Balls[i - 1].GetComponent<GameBall>().ChangeBall(InGameBallDisplayInfo.Close);
                }
                else
                {
                    GC.Balls[i - 1].GetComponent<GameBall>().ChangeBall(InGameBallDisplayInfo.Open);
                }
            }
        }
    }

    // show buttons that give the push out option. the UI part is totally throw away
    public void OnShowPushOutOption()
    {
        if (pushoutAllowed && !GameManager_script.Instance().FTUEInActionGame && !BotInControl() && !NetworkSlaveInControl() && !NetworkBotInControl() && CueControllerUpdater.current_control_status == CueControllerUpdater.CUE_BALL_FIXED_ON_TABLE)
        {
            GC.ToolTipShowPushOutOption();

            ListeningForPSConfirmation = true;
        }
    }

    public void OnShowSkipOption()
    {
        if (skipAllowed && !GameManager_script.Instance().FTUEInActionGame && !BotInControl() && !NetworkSlaveInControl() && !NetworkBotInControl() && CueControllerUpdater.current_control_status == CueControllerUpdater.CUE_BALL_FIXED_ON_TABLE)
        {
            GC.ToolTipShowSkipOption();

            ListeningForPSConfirmation = true;
        }
    }

    void ShootOptionAccepted()
    {
        // set it on my side, switch control immediately
        skipCalled = true;

        // show some stuff
        if (BotInControl() || NetworkBotInControl()) // masters don't need it, network slave is separate
        {
            ShowHelpfulTooltipPopup("", "YourSkip", true, false);
        }

        // tell opponent?
        if (NetworkMasterInControl())
        {
            ServerController.serverController.SendRPCToNetworkViewOthers("OnHelpfulTipReceived", "YourSkip");
        }

        // immediately call the end of shot function...
        StopCoroutine("WaitTillAllBallsStopsMoving");
        StartCoroutine("WaitTillAllBallsStopsMoving");
    }

    void ShootOptionNotAccepted()
    {
        // set it on my side, just proceed as usual
        skipCalled = false;

        // tell opponent?
    }

    void PushOutAccepted()
    {
        // set it on my side
        pushoutCalled = true;

        // show some stuff
        if (BotInControl() || NetworkBotInControl()) // masters don't need it, network slave is separate
        {
            ShowHelpfulTooltipPopup("", "YourPush", true, false);
        }

        // tell opponent?
        if (NetworkMasterInControl())
        {
            ServerController.serverController.SendRPCToNetworkViewOthers("OnHelpfulTipReceived", "YourPush");
        }
    }

    void PushOutNotAccepted()
    {
        // set it on my side
        pushoutCalled = false;

        // tell opponent?
    }

    public void FlipsControlSwitch(bool inFoul)
    {
        float transfer_volume = Mathf.Clamp01(1.0f);

        if (GameManager_script.Instance().StupidBotInActionGame)
        {
            if (BotMasterInControl())
            {
                // transferring control
                SoloModeMasterInControl = false;
            }
            else
            {
                // transferring control to person from bot
                SoloModeMasterInControl = true;

                if (inFoul)
                {
                    ActivateHandCursor();
                }
            }

            GameManager_script.Instance().PlaySound((int)MusicClip.Good_Transfer, false, transfer_volume); // bot always cheers
        }

        if (GameManager_script.Instance().TrulySelfInActionGame)
        {
            if (SoloMasterInControl())
            {
                SoloModeMasterInControl = false;

                if (inFoul)
                {
                    ActivateHandCursor();
                }
            }
            else
            {
                SoloModeMasterInControl = true;

                if (inFoul)
                {
                  ActivateHandCursor();
                }
            }

            GameManager_script.Instance().PlaySound((int)MusicClip.Good_Transfer, false, transfer_volume); // solo always cheers
        }

        if (GameManager_script.Instance().CurrentlyInANetWorkGame || GameManager_script.Instance().SmartBotInActionGame)
        {
            if (NetworkMasterInControl())
            {
                ServerController.serverController.playerInControl = false;

                GameManager_script.Instance().PlaySound((int)MusicClip.Bad_Transfer, false, transfer_volume); // losing control always sad
            }
            else
            {
                ServerController.serverController.playerInControl = true;

                if (inFoul) // this is most likely NOT going to happen coz this whole clause will not be called, ever
                {
                    ActivateHandCursor();
                }

                GameManager_script.Instance().PlaySound((int)MusicClip.Good_Transfer, false, transfer_volume); // getting control always cheers
            }
        }
    }

    public IEnumerator OnLoadMainMenu(bool inWin, bool inShowWinLabelAnimation, int inSoloWinnerCount = 0, bool inWinByDisconnect = false, bool inLoseByDisconnect = false, bool inTutorialWin = false)
    {
        if (!LoadMainMenuAlreadyCalled)
        {
            LoadMainMenuAlreadyCalled = true;

            yield return new WaitForSeconds(0.05f); // dajiang hack, this wait allows the "end of game" signal to get through

            // level up trigger
            float levelOfLevelUp = 0.0f;

            // stats stuff here! Write into main stats and add some experience shiites
            if (GameManager_script.Instance().StartingOutAsANetWorkGame)
            {
                // stats and experiences
                GameManager_script.Instance().addStatsToLastTwentyGames(inWin);
                GameManager_script.Instance().addStatsToAllTimeTally(inWin);
                GameManager_script.IncreaseExperience(inWin);

                levelOfLevelUp = GameManager_script.CalculateLevel();

                // see if we got any seriously good cues and avatars
                if (levelOfLevelUp > 0.0f)
                {
                    GameManager_script.Instance().IncreaseCueNewCount(levelOfLevelUp);
                    GameManager_script.Instance().IncreaseAvatarNewCount(levelOfLevelUp);
                }

                // set game winning info for a possible rematch series?
                if (inWin)
                {
                    GameManager_script.Instance().rematchYouAreThePrevWinner = true;
                    GameManager_script.Instance().rematchYourWinCount += 1;
                }
                else
                {
                    GameManager_script.Instance().rematchYouAreThePrevWinner = false;
                    GameManager_script.Instance().rematchOppoWinCount += 1;
                }
            }

            // give exit screen a timer for its win interstitial (has nothing to do with the label win stuff down below)
            if (GameManager_script.Instance().FTUEInActionGame)
            {
                GameManager_script.Instance().EndOfGameWaitAndChangeTime = GameManager_script.Instance().EndOfFtueScreenTime;
            }
            else if (levelOfLevelUp > 0.0f)
            {
                GameManager_script.Instance().EndOfGameWaitAndChangeTime = GameManager_script.Instance().LevelUpScreenTime + GameManager_script.Instance().EndOfGameScreenTime;
            }
            else
            {
                GameManager_script.Instance().EndOfGameWaitAndChangeTime = GameManager_script.Instance().EndOfGameScreenTime;
            }

            // see if bot wants to rematch intent
            if (GameManager_script.Instance().SmartBotInActionGame)
            {
                GameManager_script.Instance().GenerateSmartBotIntentToRematch(shotCount, !inWin, GameManager_script.Instance().rematchOppoWinCount, GameManager_script.Instance().rematchYourWinCount);

                GameManager_script.Instance().rematchSmartBotClickPhase = 1; // we are ready to 

                GameManager_script.Instance().StartCoroutine(GameManager_script.Instance().SmartBotRematchFakeButtonClick(SmartBotRematchFakeClickTime(), GameManager_script.Instance().rematchSmartBotClickPhase));
            }

            // get them animation and other bullshit going
            float showWinLabelTime = 0.0f;

            GameOverType vGameOverType;

            if (inShowWinLabelAnimation)
            {
                showWinLabelTime = 2.0f;

                if (inTutorialWin)
                {
                    showWinLabelTime = 4.0f;

                    vGameOverType = GameOverType.Tutorial;
                }
                else if (inSoloWinnerCount == 1)
                {
                    vGameOverType = GameOverType.SoloOneWin;
                }
                else if (inSoloWinnerCount == 2)
                {
                    vGameOverType = GameOverType.SoloTwoWin;
                }
                else if (inLoseByDisconnect)
                {
                    vGameOverType = GameOverType.Lose;
                }
                else if (inWinByDisconnect)
                {
                    vGameOverType = GameOverType.Win;
                }
                else if (inWin)
                {
                    vGameOverType = GameOverType.Win;
                }
                else
                {
                    vGameOverType = GameOverType.Lose;
                }
            }
            else
            {
                showWinLabelTime = 0.0f;

                vGameOverType = GameOverType.None;
            }

            inGameMainUIPanel.GetComponent<GameCenter>().ChangeGameOverLabel(vGameOverType);

            // populate interstitial stuff
            if (GameManager_script.Instance().StartingOutAsANetWorkGame || GameManager_script.Instance().StupidBotInActionGame || GameManager_script.Instance().TrulySelfInActionGame)
            {
                GameManager_script.Instance().PopulateInterstitialEndScreen
                (
                    false,
                    inWin,
                    inWinByDisconnect,
                    inLoseByDisconnect,
                    levelOfLevelUp > 0.0f,
                    GameManager_script.Instance().P_One_Balls_Potted,
                    GameManager_script.Instance().P_One_Miss_Shots,
                    GameManager_script.Instance().P_One_Snookered_Self,
                    GameManager_script.Instance().P_One_Scratch,
                    GameManager_script.Instance().GetTPAScore(GameManager_script.Instance().P_One_Balls_Potted, GameManager_script.Instance().P_One_Miss_Shots, GameManager_script.Instance().P_One_Snookered_Self, GameManager_script.Instance().P_One_Scratch),
                    GameManager_script.Instance().P_Two_Balls_Potted,
                    GameManager_script.Instance().P_Two_Miss_Shots,
                    GameManager_script.Instance().P_Two_Snookered_Self,
                    GameManager_script.Instance().P_Two_Scratch,
                    GameManager_script.Instance().GetTPAScore(GameManager_script.Instance().P_Two_Balls_Potted, GameManager_script.Instance().P_Two_Miss_Shots, GameManager_script.Instance().P_Two_Snookered_Self, GameManager_script.Instance().P_Two_Scratch),
                    vGameOverType,
                    GC.PlayerName1.text,
                    GC.PlayerName2.text,
                    GameManager_script.Instance().otherGameProfileInfo != null ? GameManager_script.Instance().otherGameProfileInfo.HeadImage : 0
                );
            }

            // populate possible rematch tpa anywayz...
            GameManager_script.Instance().rematchPrevTPAScore = GameManager_script.Instance().GetTPAScore(GameManager_script.Instance().P_Two_Balls_Potted, GameManager_script.Instance().P_Two_Miss_Shots, GameManager_script.Instance().P_Two_Snookered_Self, GameManager_script.Instance().P_Two_Scratch);

            yield return new WaitForSeconds(showWinLabelTime); // this wait is not dangerous

            // stop music here
            GameManager_script.Instance().StopClockMusic();

            // send parse signal
            SendGameEndSignal(inWin, inShowWinLabelAnimation, inSoloWinnerCount, inWinByDisconnect, inLoseByDisconnect, inTutorialWin);

            // clear popup
            GameManager_script.Instance().PopupCurrentlyVisible = false;

            // no no network game
            GameManager_script.Instance().NetworkGameSceneCurrentLoad = false;

            // load main menu
            //Application.LoadLevel("NGUI_MENU");
			SceneManager.LoadScene("NGUI_MENU");
        }
        else
        {
            yield return new WaitForSeconds(0.00f);
        }
    }

    public void ResignFromGame()
    {
        // all throw away code...
        if (MenuControllerGenerator.controller)
        {
            // resign first?
            if (GameManager_script.Instance().StartingOutAsANetWorkGame)
            {
                ServerController.serverController.SendRPCToNetworkViewOthers("OnGameFinished", true);

                ServerController.serverController.SendRPCToNetworkViewOthers("OnHelpfulTipReceived", "OppoLeftGame");
            }

            // loads my own main menu after?
            StartCoroutine(OnLoadMainMenu(false, false));
        }
    }

	void SetCamera(Button btn)
	{
		if(MenuControllerGenerator.controller)
        {
            is3D = btn.state;
            currentCamera = is3D ? camera3D : camera2D;
            camera3D.enabled = false;
            camera2D.enabled = false;
            currentCamera.enabled = true;

            ThreeDOffset = is3D ? ballReturnOffset : Vector3.zero;
            OnSettlePocketedAndDisplayBallsPositions();
        }
	}

    // the following functions return the state and ownership of the current cue & game
    public bool BotMasterInControl()
    {
        return ServerController.serverController == null && SoloModeMasterInControl && GameManager_script.Instance().StupidBotInActionGame;
    }

    public bool SoloMasterInControl() // dajiang hack, ftue piggy backed on here
    {
        return ServerController.serverController == null && SoloModeMasterInControl && (GameManager_script.Instance().TrulySelfInActionGame || GameManager_script.Instance().FTUEInActionGame);
    }

    public bool NetworkMasterInControl()
    {
        return ServerController.serverController && ServerController.serverController.playerInControl && (GameManager_script.Instance().CurrentlyInANetWorkGame || GameManager_script.Instance().SmartBotInActionGame);
    }

    public bool BotInControl()
    {
        return ServerController.serverController == null && !SoloModeMasterInControl && GameManager_script.Instance().StupidBotInActionGame;
    }

    public bool SoloSecondPersonInControl() // dajiang hack, ftue piggy backed on here
    {
        return ServerController.serverController == null && !SoloModeMasterInControl && (GameManager_script.Instance().TrulySelfInActionGame || GameManager_script.Instance().FTUEInActionGame);
    }

    public bool NetworkSlaveInControl()
    {
        return ServerController.serverController && !ServerController.serverController.playerInControl && GameManager_script.Instance().CurrentlyInANetWorkGame;
    }

    public bool NetworkBotInControl()
    {
        return ServerController.serverController && !ServerController.serverController.playerInControl && GameManager_script.Instance().SmartBotInActionGame;
    }

    public void BotBeginShotHouseKeeping()
    {
        // counter reset, we need more but maybe not as much as all the duo purposes constants
        ShootHasFinished = false;
        BotShotConfirmed = false;
        BotShootStrengthConfirmed = false;
        AimScoreIncreasing = false;

        // all positional data are nullified
        BestBotShootPosition = null;
        TotalRotateAngle = 0.0f;

        BotCueBallPositions.Clear();
        BotCueBallPositions.TrimExcess();
    }

    public bool RayWillNotHitOtherBallsStrict(Vector3 startingPosition, Vector3 direction, BallController target)
    {
        Ray ray = new Ray(startingPosition, direction);
        RaycastHit hit = new RaycastHit();

        bool collisionDetected = Physics.SphereCast(ray, ballRadius * 0.5f, out hit, 1000.0f, ballMask); // we are using a thinner ray

        if (collisionDetected)
        {
            BallController bc = hit.collider.GetComponent<BallController>();

            if (bc && bc != target)
            {
                // we actually got to some other balls, this is no good
                return false;
            }
        }

        return true;
    }

    public bool CueBallWillHitTargetBall(BallController ignore, Vector3 startingPosition, Vector3 direction, BallController target)
    {
        Ray ray = new Ray(startingPosition, direction);
        RaycastHit hit = new RaycastHit();

        ignore.GetComponent<Collider>().enabled = false;
        bool collisionDetected = Physics.SphereCast(ray, ballRadius * 1.0f, out hit, 1000.0f, wallAndBallMask); // this needs to be ball and wall coz we want to make sure
        ignore.GetComponent<Collider>().enabled = true;

        if (collisionDetected)
        {
            BallController bc = hit.collider.GetComponent<BallController>();

            if (bc && bc == target)
            {
                return true;
            }
        }

        return false;
    }

    public bool CueBallWillGetIntoPocket(BallController ignore, Vector3 startingPosition, Vector3 direction, Vector3 target)
    {
        // we are looking for a good angle as well as a relatively close position
        Ray ray = new Ray(startingPosition, direction);
        RaycastHit hit = new RaycastHit();
        bool collisionClear = true;

        ignore.GetComponent<Collider>().enabled = false;
        bool collisionDetected = Physics.SphereCast(ray, ballRadius, out hit, 1000.0f, ballMask); // this is just ball, so no wall sliding mess up.
        ignore.GetComponent<Collider>().enabled = true;

        if (collisionDetected && hit.distance < (target - startingPosition).magnitude * NinetyNinePercent)
        {
            collisionClear = false;
        }

        float rayDirectionCo = Vector3.Dot(direction.normalized, (target - startingPosition).normalized);
        float distanceInBetween = (startingPosition - target).magnitude;

        if ((collisionClear) && ((rayDirectionCo > EightyFiveDegree && distanceInBetween < ballRadius * 10.0f) ||       // 85
                                 (rayDirectionCo > SeventyFiveDegree && distanceInBetween < ballRadius * 8.0f) ||       // 75
                                 (rayDirectionCo > SixtyDegree && distanceInBetween < ballRadius * 6.0f) ||             // 60
                                 (rayDirectionCo > FourtyFiveDegree && distanceInBetween < ballRadius * 4.0f) ||        // 45
                                 (rayDirectionCo > TenDegree && distanceInBetween < ballRadius * 2.0f)))                // 10
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TargetBallWillClearPocket(BallController ignore, Vector3 startingPosition, Vector3 direction, Vector3 target, int targetIndex, bool firstaryShot)
    {
        // figure out tolerate level
        if (NetworkBotInControl())
        {
            // determine the actual offset
            float TolerateThreshold = 0.0f;

            if ((startingPosition - target).magnitude > GameManager_script.SmartBotMinDistanceForThisLongShot && !GameManager_script.SmartBotIntendToMakeThisLongShot)
            {
                // if we try to miss the thing intentionally, we will need to try and fuck it up
                TolerateThreshold = Random.Range(1.50f, 1.65f);
            }
            else if (!firstaryShot)
            {
                // if we are doing chain shots on even short shots, then we cannot be sure everything will work...
                TolerateThreshold = Random.Range(0.00f, 1.65f);
            }
            else
            {
                // give a small random factor but make sure the ball will go in (will always go in)
                TolerateThreshold = Random.Range(0.00f, 0.60f);
            }

            // set tolerate level
            TolerateLevel = Mathf.Sin(Mathf.Atan((startingPosition - target).magnitude / (ballRadius * TolerateThreshold)));
        }

        // do direction first
        bool raySimilarDirection = Vector3.Dot(direction.normalized, (target - startingPosition).normalized) > TolerateLevel;

        if (raySimilarDirection)
        {
            // we also need a very thin line pass through these points so we are very accurate? Or just 99% thing should do....
            Ray ray = new Ray(startingPosition, direction);
            RaycastHit hit = new RaycastHit();

            // ray projection
            ignore.GetComponent<Collider>().enabled = false;
            bool collisionDetected = Physics.SphereCast(ray, ballRadius, out hit, 1000.0f, ballMask); // this is just ball, so no wall sliding mess up.
            ignore.GetComponent<Collider>().enabled = true;

            // actual path clear logics
            if (collisionDetected)
            {
                bool rayStoppedShort = hit.distance < (target - startingPosition).magnitude * NinetyNinePercent;

                if (!rayStoppedShort) // ray didn't stop short
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        // if the starting position is really close to the target, which is always a pocket, we count it as a win.
        float distanceInBetween = (startingPosition - target).magnitude;

        if (distanceInBetween < ballRadius * ballRadiusMultiple)
        {
            return true;
        }

        // all else fails, confirm the path is blocked
        return false;
    }

    public bool CueBallWillHitBankShot()
    {
        Vector3 bounceDirection = Vector3.zero;
        Vector3 revisedDirection = Vector3.zero;
        Vector3 bouncePosition = Vector3.zero;

        // cue ball new direction and % energy retained
        if (collisionSphere.GetComponent<Renderer>().enabled && secondCollisionLine) // make sure we are not bouncing off of red circles
        {
            // this is direction
            bounceDirection = CueBallBounceDirection.normalized;

            // this is new position
            bouncePosition = currentCollision.position;

            // collision
            if (CueBallWillHitTargetBall(cueBallController, bouncePosition, bounceDirection, currentBallControllers[1]))
            {
                return true;
            }
        }

        return false;
    }

    public float ObjectBallWillBankIntoPocket()
    {
        return 1.0f;

        // dajiang hack, these code for banking into a pocket is OK. however, this block of code fails to address 3 problems.
        // dajiang hack, 1, a lot of times, normal cue swipe movements will not necessarily reach the point where we can have a good reverse bank.
        // dajiang hack, 2, this function block fails to address the problem of ball kicking back and hits the cue ball problem, which is sometimes painfully obvious to human eyes.
        // dajiang hack, 3, the exact order the 3 secondary shots should be ranked in is still a problem and this type of shot directly conflict with the other 2 types of shots.

		if (ballCollisionLine && currentHitBallController)
        {
            // slave ball direction and location, this has to be the currentballcontroller[1] for second chain
            Vector3 originalDirection = TargetPocketDirection;
            Vector3 originalPosition = currentHitBallController.GetComponent<Rigidbody>().position;

            // slave ray
            Ray ray = new Ray(originalPosition, originalDirection);
            RaycastHit hit = new RaycastHit();

            // shoot ray
            currentHitBallController.GetComponent<Collider>().enabled = false;
            bool collisionDetected = Physics.SphereCast(ray, ballRadius, out hit, 1000.0f, wallMask);
            currentHitBallController.GetComponent<Collider>().enabled = true;

            if (collisionDetected)
            {
                // we have final direction now
                Vector3 finalDirection = originalDirection - 2.0f * Vector3.Project(originalDirection, VectorOperator.CleanYAxis(-hit.normal));
                finalDirection = VectorOperator.getProjectXZ(finalDirection, true).normalized;

                // check final direction against all 6 pockets
                for (int i = 1; i < 7; i++)
                {
                    Vector3 ballConnectHoleDirection = (PocketController.FindeHolePositionById(i) - hit.point).normalized;

                    if (Vector3.Dot(finalDirection, ballConnectHoleDirection) > EightyDegree)
                    {
                        // we also need a very thin line pass through these points so we are very accurate? Or just 99% thing should do....
                        Ray bankRay = new Ray(hit.point, finalDirection);
                        RaycastHit bankHit = new RaycastHit();

                        // ray projection
                        currentHitBallController.GetComponent<Collider>().enabled = false;
                        bool bankCollisionDetected = Physics.SphereCast(bankRay, ballRadius * 0.5f, out bankHit, 1000.0f, ballMask); // this is just ball, so no wall sliding mess up.
                        currentHitBallController.GetComponent<Collider>().enabled = true;

                        // actual path clear logics
                        if (bankCollisionDetected)
                        {
                            bool rayStoppedShort = bankHit.distance < (hit.point - PocketController.FindeHolePositionById(i)).magnitude * NinetyNinePercent;

                            if (!rayStoppedShort) // ray didn't stop short
                            {
                                // return a greater than 1 value
                                return 1.0f + Mathf.Clamp01(Vector3.Dot(finalDirection, ballConnectHoleDirection));
                            }
                        }
                        else
                        {
                            // return a greater than 1 value
                            return 1.0f + Mathf.Clamp01(Vector3.Dot(finalDirection, ballConnectHoleDirection));
                        }
                    }
                }
            }
        }

        return 1.0f;
    }

    public float CueBallWillHitChainShot()
    {
        if (ballCollisionLine && currentHitBallController)
        {
            // slave ball direction and location, this has to be the currentballcontroller[1] for second chain
            Vector3 originalDirection = TargetPocketDirection;
            Vector3 originalPosition = currentHitBallController.GetComponent<Rigidbody>().position;

            // slave ray
            Ray ray = new Ray(originalPosition, originalDirection);
		    RaycastHit hit = new RaycastHit();

            currentHitBallController.GetComponent<Collider>().enabled = false;
            bool collisionDetected = Physics.SphereCast(ray, ballRadius, out hit, 1000.0f, ballMask);
            currentHitBallController.GetComponent<Collider>().enabled = true;

            if (collisionDetected)
		    {
                BallController bc = hit.collider.GetComponent<BallController>();

                // we are hitting another ball with our ball
                if (bc)
                {
                    // hit point and direction on next ball
                    Vector3 secondaryDirection = (bc.GetComponent<Rigidbody>().position - hit.point).normalized;
                    Vector3 secondaryPosition = bc.GetComponent<Rigidbody>().position;
                    
                    // out of all 6 pockets, anything that gets through?
                    for (int i = 1; i < 7; i++)
                    {
                        Vector3 hp = PocketController.FindeHolePositionById(i);
                        bool isPathToPocketClear = TargetBallWillClearPocket(bc, secondaryPosition, secondaryDirection, hp, i, false);
                        float totalTravelLength = (secondaryPosition - originalPosition).magnitude + (hp - secondaryPosition).magnitude;
                        float anglePrecision = 1.0f;

                        if ((hp - secondaryPosition).magnitude < ballRadius * ballRadiusMultiple)
                        {
                            anglePrecision = 2.0f;
                        }
                        else
                        {
                            anglePrecision = 1.0f + Mathf.Clamp01(Vector3.Dot((hp - secondaryPosition).normalized, secondaryDirection.normalized));
                        }

                        // make sure we can clean pocket (with the enlarged threshold), and the total distance travelled is small, and precision angle is ok
                        if (isPathToPocketClear && totalTravelLength < 0.50f * GameManager_script.DiagDistance && anglePrecision > 1.0f)
                        {
                            return anglePrecision;
                        }
                    }
                }
            }
        }

        return 1.0f;
    }

    public float CueBallWillCarromShot()
    {
        return 1.0f;
    }

    // this is used inside OnDrawLineAndSphere ONLY. It will be referencing A LOT OF stuff from OnDrawLineAndSphere so beware
    // 1 Direct pocket object ball
    // 2 Chain shot pocket object ball
    // 3 Hitting object ball via bank shot or straight shot
    // 4 Not hitting object ball but not obviously fouling
    // 5 Hitting with red circles
    public void RobotAnalysis()
    {
        if (currentBallControllers.Count > 1)
        {
            Vector3 currentShotShadowSpin = Vector3.zero;
            RobotShotLevelType currentShotObjectLevel = RobotShotLevelType.FoulHit;

            float currentShotObjectScore = 0.0f;
            float currentShotStrength = 0.0f;

            float isSmartBotLongShot = 0.0f;
            float isSmartBotIntendToPocket = 0.0f;

            float strengthMultiple = 0.0f;

            // determine level
            if (CueBallWillHitTargetBall(cueBallController, cueBallController.GetComponent<Rigidbody>().position, CueShootDirection, currentBallControllers[1])) // first ball remaining
            {
                for (int i = 1; i < 7; i++) // 6 different pockets
                {
                    Vector3 hp = PocketController.FindeHolePositionById(i);

                    // pocktable and clear path
                    bool isPocketable = VectorOperator.isPocketableAngle(cueBallController.GetComponent<Rigidbody>().position, currentBallControllers[1].GetComponent<Rigidbody>().position, hp);
                    bool isPathToPocketClear = TargetBallWillClearPocket(currentBallControllers[1], currentBallControllers[1].GetComponent<Rigidbody>().position, TargetPocketDirection, hp, i, true);
                    bool isCloseEnoughToPocket = (currentBallControllers[1].GetComponent<Rigidbody>().position - hp).magnitude < ballRadius * ballRadiusMultiple;

                    // consts for shadow spin
                    Vector3 aimAngle = (currentBallControllers[1].GetComponent<Rigidbody>().position - cueBallController.GetComponent<Rigidbody>().position).normalized;
                    Vector3 currentAngle = (currentBallControllers[1].GetComponent<Rigidbody>().position - currentCollision.position).normalized;
                    float dotBetweenMidAndCurrent = Vector3.Dot(aimAngle, currentAngle);

                    if (shotCount == 0)
                    {
                        currentShotShadowSpin = new Vector3(Random.Range(-0.35f, 0.35f), Random.Range(-0.35f, 0.35f), 0.0f);
                    }
                    else
                    {
                        if (dotBetweenMidAndCurrent > EightyFiveDegree)
                        {
                            // a heavy kick backward
                            currentShotShadowSpin = new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.90f, -0.75f), 0.0f);
                        }
                        else if (dotBetweenMidAndCurrent > EightyDegree)
                        {
                            if (GameManager_script.DetermineLotteryResult(0.66f))
                            {
                                // still kick back really heavy
                                currentShotShadowSpin = new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.90f, -0.75f), 0.0f);
                            }
                            else
                            {
                                // or we can do a heavy forward spin as well
                                currentShotShadowSpin = new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(0.75f, 0.90f), 0.0f);
                            }
                        }
                        else
                        {
                            if (GameManager_script.DetermineLotteryResult(0.20f))
                            {
                                // some soft vertical bot spin
                                currentShotShadowSpin = new Vector3(Random.Range(-0.20f, 0.20f), Random.Range(-0.85f, -0.50f), 0.0f);
                            }
                            else if (GameManager_script.DetermineLotteryResult(0.25f))
                            {
                                // some soft vertical top spin
                                currentShotShadowSpin = new Vector3(Random.Range(-0.20f, 0.20f), Random.Range(0.50f, 0.85f), 0.0f);
                            }
                            else if (GameManager_script.DetermineLotteryResult(0.33f))
                            {
                                // some soft left english spin
                                currentShotShadowSpin = new Vector3(Random.Range(-0.85f, -0.50f), Random.Range(-0.25f, 0.25f), 0.0f);
                            }
                            else if (GameManager_script.DetermineLotteryResult(0.50f))
                            {
                                // some soft right english spin
                                currentShotShadowSpin = new Vector3(Random.Range(0.50f, 0.85f), Random.Range(-0.25f, 0.25f), 0.0f);
                            }
                            else
                            {
                                // some random spin all around
                                currentShotShadowSpin = new Vector3(Random.Range(-0.85f, 0.85f), Random.Range(-0.85f, 0.85f), 0.0f);
                            }
                        }
                    }

                    // every 1.0 ball speed roughly translates into 2.0 distance travelled, but in order to add some randomness to it, we will do a random call from 2.0 to 3.0
                    float angleBetween = Mathf.Abs(Vector3.Dot((hp - currentBallControllers[1].GetComponent<Rigidbody>().position).normalized, (currentBallControllers[1].GetComponent<Rigidbody>().position - cueBallController.GetComponent<Rigidbody>().position).normalized));
                    float totalDistanceTravelled = (hp - currentBallControllers[1].GetComponent<Rigidbody>().position).magnitude / angleBetween + (currentBallControllers[1].GetComponent<Rigidbody>().position - cueBallController.GetComponent<Rigidbody>().position).magnitude;

                    if (dotBetweenMidAndCurrent > EightyFiveDegree)
                    {
                        // 2X to 3X the original force for a true back spin
                        currentShotStrength = Mathf.Clamp(totalDistanceTravelled * Random.Range(2.90f, 3.80f) / ballMaxVelocity * cueMaxDisplacement, cueMaxDisplacement * 0.05f, cueMaxDisplacement * 1.00f);
                    }
                    else if (dotBetweenMidAndCurrent > EightyDegree)
                    {
                        // 2X to 3X the original force for a true back spin
                        currentShotStrength = Mathf.Clamp(totalDistanceTravelled * Random.Range(2.10f, 3.20f) / ballMaxVelocity * cueMaxDisplacement, cueMaxDisplacement * 0.05f, cueMaxDisplacement * 1.00f);
                    }
                    else
                    {
                        // the only special place we need to test travel multiple
                        strengthMultiple = totalDistanceTravelled * 1.60f / ballMaxVelocity;

                        // a smaller original force is required
                        currentShotStrength = Mathf.Clamp(strengthMultiple * cueMaxDisplacement, cueMaxDisplacement * 0.05f, cueMaxDisplacement * 1.00f);
                    }

                    // if we can pocket stuff, we can put this to good use
                    float precisionAngle = 1.0f;
                    float SecondaryPrecisionAngle = 1.0f;

                    if (collisionSphereRed.GetComponent<Renderer>().enabled)
                    {
                        currentShotObjectLevel = RobotShotLevelType.FoulHit;
                    }
                    else if (CueballDivePocketExam(currentBallControllers[1], currentCollision.position, CueBallBounceDirection.normalized))
                    {
                        currentShotObjectLevel = RobotShotLevelType.FoulHit;
                    }
                    else if (strengthMultiple > 1.333f)
                    {
                        currentShotObjectLevel = RobotShotLevelType.GoodHit; // cannot recover this coz we are not strong enough to slice it in
                    }
                    else if (isPathToPocketClear)
                    {
                        if (isPocketable || isCloseEnoughToPocket)
                        {
                            if (i == 1 || i == 4) // the 2 middle pockets
                            {
                                Vector3 ZVerticalVector = new Vector3(0.0f, 0.0f, 1.0f);

                                if (Mathf.Abs(Vector3.Dot(currentAngle.normalized, ZVerticalVector.normalized)) < FourtyDegree)
                                {
                                    currentShotObjectLevel = RobotShotLevelType.GoodHit; // a hit but no pocket
                                }
                                else
                                {
                                    if ((hp - currentBallControllers[1].GetComponent<Rigidbody>().position).magnitude < ballRadius * ballRadiusMultiple)
                                    {
                                        precisionAngle = 2.0f;
                                    }
                                    else
                                    {
                                        precisionAngle = 1.0f + Mathf.Clamp01(Vector3.Dot((hp - currentBallControllers[1].GetComponent<Rigidbody>().position).normalized, (currentBallControllers[1].GetComponent<Rigidbody>().position - currentCollision.position).normalized));
                                    }

                                    currentShotObjectLevel = RobotShotLevelType.StraightPocket; // very good, we can attempt a sure hit
                                }
                            }
                            else
                            {
                                if ((hp - currentBallControllers[1].GetComponent<Rigidbody>().position).magnitude < ballRadius * ballRadiusMultiple)
                                {
                                    precisionAngle = 2.0f;
                                }
                                else
                                {
                                    precisionAngle = 1.0f + Mathf.Clamp01(Vector3.Dot((hp - currentBallControllers[1].GetComponent<Rigidbody>().position).normalized, (currentBallControllers[1].GetComponent<Rigidbody>().position - currentCollision.position).normalized));
                                }

                                currentShotObjectLevel = RobotShotLevelType.StraightPocket; // very good, we can attempt a sure hit
                            }
                        }
                        else
                        {
                            currentShotObjectLevel = RobotShotLevelType.GoodHit; // probably the least savable kind, we cannot hit the ball properly for it to go into the pocket
                        }

                        // determine a bunch of things for smart bot
                        isSmartBotLongShot = NetworkBotInControl() && (currentShotObjectLevel == RobotShotLevelType.StraightPocket || currentShotObjectLevel == RobotShotLevelType.SecondaryPocket) && (currentBallControllers[1].GetComponent<Rigidbody>().position - hp).magnitude > GameManager_script.SmartBotMinDistanceForThisLongShot ? 1.0f : 0.0f;
                        isSmartBotIntendToPocket = isSmartBotLongShot == 1.0f && GameManager_script.SmartBotIntendToMakeThisLongShot ? 1.0f : 0.0f;
                    }
                    else
                    {
                        float BankShotAvailableNess = ObjectBallWillBankIntoPocket();
                        float SecondChainableNess = CueBallWillHitChainShot();
                        float CarromPossible = CueBallWillCarromShot();

                        // testing robustness when we stop the auto break
                        if (shotCount == 0) // change this to when balls are all balled up
                        {
                            float angle = Vector3.Dot((currentCollision.position - cueBallController.GetComponent<Rigidbody>().position).normalized, (currentBallControllers[1].GetComponent<Rigidbody>().position - currentCollision.position).normalized);

                            if (angle > SeventyDegree && angle < EightySevenFiveDegree)
                            {
                                precisionAngle = Random.Range(1.0f, 2.0f);
                                currentShotObjectLevel = RobotShotLevelType.StraightPocket; // just shoot, a totally valid shot but we don't worry about cue ball exit angle
                            }
                            else
                            {
                                precisionAngle = Random.Range(1.0f, 2.0f);
                                currentShotObjectLevel = RobotShotLevelType.GoodHit; // just shoot, a totally valid shot but we don't worry about cue ball exit angle
                            }
                        }
                        else if (DetermineSparsenessOfballs() < SparsenessThreshold)
                        {
                            float angle = Vector3.Dot((currentCollision.position - cueBallController.GetComponent<Rigidbody>().position).normalized, (currentBallControllers[1].GetComponent<Rigidbody>().position - currentCollision.position).normalized);

                            if (angle > SeventyDegree && angle < EightySevenFiveDegree)
                            {
                                precisionAngle = Random.Range(1.0f, 2.0f);
                                currentShotObjectLevel = RobotShotLevelType.StraightPocket; // just shoot, a totally valid shot but we don't worry about cue ball exit angle
                            }
                            else
                            {
                                precisionAngle = Random.Range(1.0f, 2.0f);
                                currentShotObjectLevel = RobotShotLevelType.GoodHit; // just shoot, a totally valid shot but we don't worry about cue ball exit angle
                            }
                        }
                        else if (BankShotAvailableNess > 1.0f) // if there is a reverse bank available
                        {
                            SecondaryPrecisionAngle = BankShotAvailableNess;

                            currentShotObjectLevel = RobotShotLevelType.SecondaryPocket; // if there is a possible chain available
                        }
                        else if (SecondChainableNess > 1.0f)
                        {
                            SecondaryPrecisionAngle = SecondChainableNess;

                            currentShotObjectLevel = RobotShotLevelType.SecondaryPocket; // if there is a possible chain available
                        }
                        else if (CarromPossible > 1.0f)
                        {
                            SecondaryPrecisionAngle = CarromPossible;

                            currentShotObjectLevel = RobotShotLevelType.SecondaryPocket; // if there is a possible carrom available
                        }
                        else
                        {
                            currentShotObjectLevel = RobotShotLevelType.GoodHit; // can still kinda hit it
                        }
                    }

                    // determine score
                    currentShotObjectScore = GenerateCueScore(currentShotObjectLevel, precisionAngle, SecondaryPrecisionAngle);

                    // compare and write to existing high score
                    CheckAndSwapObjectScore(currentShotObjectLevel, currentShotObjectScore, precisionAngle, SecondaryPrecisionAngle, currentShotStrength, cueBallController.GetComponent<Rigidbody>().position, cuePivot.localRotation, cuePivot.forward, currentShotShadowSpin, isSmartBotLongShot, isSmartBotIntendToPocket);
                }
            }
            else // we cannot even directly hit the target ball
            {
                // since we don't really want to mess with the thing when we cannot even hit the cue ball, just do a simple random draw would be enough
                currentShotShadowSpin = new Vector3(Random.Range(-0.10f, 0.10f), Random.Range(-0.10f, 0.10f), 0.0f);

                // every 1.0 ball speed roughly translates into 2.0 distance travelled
                float totalDistanceTravelled = (currentCollision.position - currentBallControllers[1].GetComponent<Rigidbody>().position).magnitude + (currentCollision.position - cueBallController.GetComponent<Rigidbody>().position).magnitude + (PocketController.FindeHolePositionById(Random.Range(1, 7)) - currentBallControllers[1].GetComponent<Rigidbody>().position).magnitude;
                currentShotStrength = Mathf.Clamp(totalDistanceTravelled * 2.30f / ballMaxVelocity * cueMaxDisplacement, cueMaxDisplacement * 0.05f, cueMaxDisplacement * 1.00f);

                if (collisionSphereRed.GetComponent<Renderer>().enabled)
                {
                    currentShotObjectLevel = RobotShotLevelType.FoulHit;
                }
                else if (CueballDivePocketExam(currentBallControllers[1], currentCollision.position, CueBallBounceDirection.normalized))
                {
                    currentShotObjectLevel = RobotShotLevelType.FoulHit;
                }
                else if (CueBallWillHitBankShot())
                {
                    currentShotObjectLevel = RobotShotLevelType.GoodHit;
                }
                else
                {
                    currentShotObjectLevel = RobotShotLevelType.RandomHit;
                }

                // determine score
                currentShotObjectScore = GenerateCueScore(currentShotObjectLevel, 0.0f, 0.0f);

                // compare and write to existing high score
                CheckAndSwapObjectScore(currentShotObjectLevel, currentShotObjectScore, 1.0f, 1.0f, currentShotStrength, cueBallController.GetComponent<Rigidbody>().position, cuePivot.localRotation, cuePivot.forward, currentShotShadowSpin, isSmartBotLongShot, isSmartBotIntendToPocket);
            }
        }
    }

    // this function determines if cue ball will dive into pockets
    public bool CueballDivePocketExam(BallController inIgnore, Vector3 inStartPosition, Vector3 inDiection)
    {
        for (int i = 1; i < 7; i++) // 6 different pockets
        {
            Vector3 hp = PocketController.FindeHolePositionById(i);
            bool CueBallDiveIntoPocket = CueBallWillGetIntoPocket(inIgnore, inStartPosition, inDiection, hp);

            if (CueBallDiveIntoPocket)
            {
                return true;
            }
        }

        return false;
    }

    // this function can only generate the score of the configuration we are currently and physically in
    public float GenerateCueScore(RobotShotLevelType inLevel, float inPrecisionAngle, float inChainPrecisionAngle) // everything is 1 to 2
    {
        float totalScore = 0.0f;
        float cueBallAngle = Mathf.Clamp01(Vector3.Dot(CueShootDirection.normalized, CueBallBounceDirection.normalized)); // 0 to 1, 0 is straight ahead, 1 is scratching ball

        if (inLevel == RobotShotLevelType.StraightPocket || inLevel == RobotShotLevelType.SecondaryPocket)
        {
            float precisionFactor = inPrecisionAngle * inChainPrecisionAngle; // product is from 1.0f to 2.0f
            float knockDownFactor = 1.0f; // starting at 100%

            // this essentially puts all angles greater than 66 degrees and over on a completely different level
            if (cueBallAngle > SixtyDegree) // 60 degrees and over
            {
                knockDownFactor *= 0.90f;
            }

            // this basically eliminates all non precise ones, and the penalty is big
            if ((inPrecisionAngle > 1.0f && inPrecisionAngle < PrecisionMinimum) || (inChainPrecisionAngle > 1.0f && inChainPrecisionAngle < PrecisionMinimum)) // they are usually higher than 1.999f
            {
                knockDownFactor *= 0.90f;
            }

            totalScore = knockDownFactor * precisionFactor;
        }
        else if (inLevel == RobotShotLevelType.GoodHit)
        {
            float angleFactor = cueBallAngle < TenDegree ? 1.5f : 1.0f;
            float randomFactor = Random.Range(0.33f, 1.00f);

            if (collisionSphere.GetComponent<Renderer>().enabled)
            {
                float distanceFactor = 1.0f - Mathf.Clamp01(Mathf.Abs((currentCollision.position - currentBallControllers[1].GetComponent<Rigidbody>().position).magnitude / GameManager_script.DiagDistance));

                totalScore = randomFactor * angleFactor * distanceFactor;
            }
            else
            {
                float distanceFactor = 0.0f;

                totalScore = randomFactor * angleFactor * distanceFactor;
            }
        }
        else if (inLevel == RobotShotLevelType.FoulHit || inLevel == RobotShotLevelType.RandomHit)
        {
            float randomFactor = Random.Range(0.0f, 1.0f);

            totalScore = randomFactor;
        }
        else
        {
            totalScore = 0.0f;
        }

        return totalScore;
    }

    public void CheckAndSwapObjectScore(RobotShotLevelType inLevel, float inScore, float inPrecision, float inChainPrecision, float inStrength, Vector3 actualPosition, Quaternion localRotation, Vector3 forwardRotation, Vector3 localPosition, float inIsLongShot, float inIsGoingToGoIn)
    {
        if (BestBotShootPosition == null)
        {
            BestBotShootPosition = new PossibleShootPlacement();
        }

        // compare and write to existing high score (we 100% will admit it if its of a higher level but only selectively admit it if its a lower level)
        bool LevelIsUp = (int)BestBotShootPosition.ObjectLevel > (int)inLevel;
        bool LevelIsEqual = (int)BestBotShootPosition.ObjectLevel == (int)inLevel;
        bool ScoreIsDown = (int)BestBotShootPosition.ObjectLevel == (int)inLevel && BestBotShootPosition.ObjectScore > inScore;
        bool ScoreIsUp = (int)BestBotShootPosition.ObjectLevel == (int)inLevel && BestBotShootPosition.ObjectScore < inScore;

        if (GameManager_script.Instance().SmartBotInActionGame && (LevelIsUp || (LevelIsEqual && ScoreIsDown)) || GameManager_script.Instance().StupidBotInActionGame && (LevelIsUp || (LevelIsEqual && ScoreIsUp)))
        {
            BestBotShootPosition.ObjectLevel = inLevel;

            BestBotShootPosition.ObjectScore = inScore;

            BestBotShootPosition.cueBallPosition = actualPosition;
            BestBotShootPosition.cuePivotLocalRotation = localRotation;
            BestBotShootPosition.cuePivotForwardRotation = forwardRotation;
            BestBotShootPosition.cueRotationLocalPosition = localPosition;
            BestBotShootPosition.smartBotLongShot = inIsLongShot;
            BestBotShootPosition.smartBotIntendToPocket = inIsGoingToGoIn;

            // a truly good strength number (minor the breaking part)
            float strengthBeforeRevision = Mathf.Clamp01(inStrength / cueMaxDisplacement) * cueMaxDisplacement / sliderForceCurve.Evaluate(Mathf.Clamp01(inStrength / cueMaxDisplacement));

            if (shotCount == 0)
            {
                BestBotShootPosition.cueBallStrength = Random.Range(0.95f, 1.00f) * cueMaxDisplacement;
            }
            else
            {
                if (NetworkBotInControl())
                {
                    float lower = 0.0f;
                    float higher = 0.0f;
                    float strengthIndex = Random.Range(0.0f, 1.0f);

                    if (strengthIndex < GameManager_script.SmartBotAngryPowerShotPercentage)
                    {
                        lower = Mathf.Max(strengthBeforeRevision, cueMaxDisplacement * 0.85f);
                        higher = cueMaxDisplacement * 1.0f;
                    }
                    else if (strengthIndex < GameManager_script.SmartBotNormalPowerShotPercentage)
                    {
                        lower = strengthBeforeRevision * 1.0f;
                        higher = Mathf.Min(strengthBeforeRevision * 1.50f, cueMaxDisplacement * 1.0f);
                    }
                    else
                    {
                        lower = strengthBeforeRevision * 1.0f;
                        higher = strengthBeforeRevision * 1.0f;
                    }

                    BestBotShootPosition.cueBallStrength = Random.Range(lower, higher);
                }
                else
                {
                    BestBotShootPosition.cueBallStrength = strengthBeforeRevision;
                }
            }

            AimScoreIncreasing = true;
        }
    }

    public bool AllOptionsExhausted()
    {
        if (TotalRotateAngle > 360) // 360 degree o
        {
            return true;
        }

        return false;
    }

    public void StupidRobotUpdate()
    {
        if (!ShootHasFinished)
        {
            if (!BotShotConfirmed)
            {
                // analysis, this analysis is very generic and i can use it everywhere
                RobotAnalysis();

                // first clause is giving up, second clause is found good shiites (even though the scale is just 1 (direct pockets), but only a high enough score has real meanings)
                if (AllOptionsExhausted() || shotCount == 0)
                {
                    if (StupidRobotMoreSpotsAvailable())
                    {
                        StupidRobotRespotCueBallAndStartOver();
                    }
                    else
                    {
                        BotShotConfirmed = true;

                        // make sure cue ball is in the right place
                        cueBallController.GetComponent<Rigidbody>().position = BestBotShootPosition.cueBallPosition;
                        cuePivot.localRotation = BestBotShootPosition.cuePivotLocalRotation;
                        cueRotation.localPosition = BestBotShootPosition.cueRotationLocalPosition;

                        cueDisplacement = 0.0f;
                        cueBallPivot = Vector3.zero;

                        // think about what to do before the shooting
                        EstimateStrengthAndPivotSpinAndCallOptions();

                        // clear up draw constants
                        DrawTarget = new Vector2(Mathf.Clamp(BestBotShootPosition.cueBallStrength * 0.60f, 0.0f, cueMaxDisplacement), Mathf.Clamp(BestBotShootPosition.cueBallStrength * 1.66f, 0.0f, cueMaxDisplacement));
                        CurrentDrawSpeed = 0.0f;
                        FinalApproach = false;
                    }
                }
                else
                {
                    if (currentBallControllers != null && currentBallControllers.Count > 1)
                    {
                        // first 2.0f is for symmetry, second 2.0f is for the collision sphere's own radius
                        float totalBallAngle = 2.0f * 2.0f * Mathf.Rad2Deg * Mathf.Atan(ballRadius / (currentBallControllers[1].GetComponent<Rigidbody>().position - cueBallController.GetComponent<Rigidbody>().position).magnitude);

                        Vector3 aimAngle = (currentBallControllers[1].GetComponent<Rigidbody>().position - cueBallController.GetComponent<Rigidbody>().position).normalized;
                        Vector3 currentAngle = (currentCollision.position - cueBallController.GetComponent<Rigidbody>().position).normalized;

                        float currentBallAngle = Vector3.Angle(aimAngle, currentAngle);

                        // sometimes off ball swipes are too large, it will completely skip the ball through a gap
                        if (currentBallAngle < totalBallAngle * 0.5f)
                        {
                            StupidRobotRotateCue(totalBallAngle / SecondsOnBall); // swipe is X degrees per second
                        }
                        else if (currentBallAngle < totalBallAngle)
                        {
                            StupidRobotRotateCue(totalBallAngle / SecondsOnBall + Mathf.Abs((totalBallAngle - currentBallAngle) / (totalBallAngle * 0.5f) - 1.0f) * ((360.0f - totalBallAngle) / SecondsOnOthers - totalBallAngle / SecondsOnBall));
                        }
                        else
                        {
                            StupidRobotRotateCue((360.0f - totalBallAngle) / SecondsOnOthers);
                        }
                    }
                }

                AimScoreIncreasing = false;
            }
            else
            {
                // this is where we draw cue and try to figure out when to shoot
                if (!BotShootStrengthConfirmed)
                {
                    if (cueDisplacement <= DrawTarget.x)
                    {
                        CurrentDrawSpeed = cueMaxDisplacement * Time.deltaTime;
                    }

                    if (cueDisplacement >= DrawTarget.y)
                    {
                        CurrentDrawSpeed = -cueMaxDisplacement * Time.deltaTime;
                        DrawTarget = new Vector2(Mathf.Clamp(BestBotShootPosition.cueBallStrength * 0.60f, 0.0f, cueMaxDisplacement), Mathf.Clamp(BestBotShootPosition.cueBallStrength * 1.00f, 0.0f, cueMaxDisplacement));
                        FinalApproach = true;
                    }

                    cueDisplacement += CurrentDrawSpeed;

                    // make sure its final approach and we are on the pull back phase
                    if (FinalApproach && CurrentDrawSpeed > 0 && cueDisplacement > DrawTarget.y * NinetyNinePercent) // 99%
                    {
                        BotShootStrengthConfirmed = true;
                    }
                }
                else
                {
                    ShootHasFinished = true;

                    StartCoroutine(StupidBotShootCue());
                }

                cueBallPivot = Vector3.Lerp(cueBallPivot, BestBotShootPosition.cueRotationLocalPosition, Time.deltaTime * 5.0f);
            }
        }
    }

    public void EstimateStrengthAndPivotSpinAndCallOptions()
    {
        if (shotCount == 0)
        {
            BestBotShootPosition.cueBallStrength = Random.Range(0.95f, 1.0f) * cueMaxDisplacement;
        }
        else if (shotCount == 1 && pushoutAllowed)
        {
            if (BestBotShootPosition.ObjectLevel != RobotShotLevelType.StraightPocket && BestBotShootPosition.ObjectLevel != RobotShotLevelType.SecondaryPocket)
            {
                PushOutAccepted();
            }
            else
            {
                PushOutNotAccepted();
            }

            BestBotShootPosition.cueBallStrength = Random.Range(0.99f, 1.00f) * BestBotShootPosition.cueBallStrength;
        }
        else if (shotCount == 2 && skipAllowed)
        {
            if (BestBotShootPosition.ObjectLevel != RobotShotLevelType.StraightPocket && BestBotShootPosition.ObjectLevel != RobotShotLevelType.SecondaryPocket)
            {
                ShootOptionAccepted();
            }
            else
            {
                ShootOptionNotAccepted();
            }

            BestBotShootPosition.cueBallStrength = Random.Range(0.00f, 0.01f) * BestBotShootPosition.cueBallStrength;
        }
    }

    IEnumerator StupidBotShootCue()
    {
        yield return new WaitForSeconds(1.5f); // this wait is not dangerous, it is a hack mimicking the behavior of a person (put all in a different class is better)

        OnShootCue();
    }

    public void AimAtFirstBall()
    {
        if (currentBallControllers.Count > 1 && currentBallControllers[1] != null)
        {
            float totalBallAngle = 2.00f * Mathf.Rad2Deg * Mathf.Abs(Mathf.Atan(ballRadius / (currentBallControllers[1].GetComponent<Rigidbody>().position - cueBallController.GetComponent<Rigidbody>().position).magnitude));
            float totalBallAnglePercentage = Random.Range(-0.01f, 0.01f);

            if (!BotInControl() || shotCount == 0)
            {
                // add a tiny randomization to cure some bugs?
                cuePivot.forward = (currentBallControllers[1].GetComponent<Rigidbody>().position - cueBallController.GetComponent<Rigidbody>().position).normalized;
                cuePivot.Rotate(Vector3.down, totalBallAnglePercentage * totalBallAngle);
            }
            else
            {
                // we need to go and start swiping from the left side of the ball
                cuePivot.forward = (currentBallControllers[1].GetComponent<Rigidbody>().position - cueBallController.GetComponent<Rigidbody>().position).normalized;
                cuePivot.Rotate(Vector3.down, totalBallAngle);
            }

            firstCollisionLine.SetVertexCount(0);
            firstCollisionLine1.SetVertexCount(0);
            firstCollisionLine2.SetVertexCount(0);

            secondCollisionLine.SetVertexCount(0);
            secondCollisionLine1.SetVertexCount(0);
            secondCollisionLine2.SetVertexCount(0);

            ballCollisionLine.SetVertexCount(0);
            ballCollisionLine1.SetVertexCount(0);
            ballCollisionLine2.SetVertexCount(0);

            CueBallBounceDirection = Vector3.zero;
		    TargetPocketDirection = Vector3.zero;

            SimpleDrawLineAndSphere();
        }
    }

    public bool StupidRobotMoreSpotsAvailable()
    {
        if (BotCueBallPositions.Count > 0)
        {
            return true;
        }

        return false;
    }

    public void StupidRobotRespotCueBallAndStartOver()
    {
        if (BotCueBallPositions.Count > 0)
        {
            cueBallController.GetComponent<Rigidbody>().position = BotCueBallPositions[0];
            cueBallController.GetComponent<Rigidbody>().velocity = Vector3.zero;
            cueBallController.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            cueBallController.GetComponent<Collider>().enabled = true;
            cueBallController.GetComponent<Rigidbody>().useGravity = true;
            cueBallController.GetComponent<Rigidbody>().isKinematic = false;

            BotCueBallPositions.RemoveAt(0);
            BotCueBallPositions.TrimExcess();

            TotalRotateAngle = 0.0f;
        }
    }

    public void StupidRobotRotateCue(float inStep)
    {
        float rotation = inStep * Time.deltaTime;

        cuePivot.Rotate(Vector3.up, rotation);

        TotalRotateAngle += rotation;
    }

    public void ChangeCue(int index)
    {
		CueModel.GetComponent<MeshFilter>().sharedMesh = CueList[index].ModelMesh.sharedMesh;
        CueMaterial.mainTexture = CueList[index].ModelTexture;
    }

    // dajiang hack, we are literally using special cases for everything here
    public void ShowHelpfulTooltipPopup(string inAlreadyHasName, string inText, bool inAutoClose, bool inKillEverythingElse)
    {
        if (!GameManager_script.Instance().FTUEInActionGame)
        {
            string inName = "";

            if (inAlreadyHasName != "")
            {
                inName = inAlreadyHasName;
            }
            else if (inText == "OutOfTime" || inText == "PocketCueBall" || inText == "FourBallRail" || inText == "RightFirstBall" || inText == "Disconnected")
            {
                if (BotInControl() || NetworkSlaveInControl() || NetworkBotInControl())
                {
                    inName = Localization.Get("YourOpponent");
                }
                else if (SoloMasterInControl())
                {
                    inName = Localization.Get("SoloPlayerOneName");
                }
                else if (SoloSecondPersonInControl())
                {
                    inName = Localization.Get("SoloPlayerTwoName");
                }
                else
                {
                    inName = Localization.Get("YourSelf");
                }
            }
            else if (inText == "YourBreak" || inText == "YourPush" || inText == "YourSkip")
            {
                if (BotInControl() || NetworkSlaveInControl() || NetworkBotInControl())
                {
                    inName = Localization.Get("YourOpponentIs");
                }
                else if (SoloMasterInControl())
                {
                    inName = Localization.Get("SoloPlayerOneNameIs");
                }
                else if (SoloSecondPersonInControl())
                {
                    inName = Localization.Get("SoloPlayerTwoNameIs");
                }
                else
                {
                    inName = Localization.Get("YourSelfAre");
                }
            }
            else if (inText == "BallRailContact" || inText == "Connecting" || inText == "OppoLeftGame")
            {
                inName = "";
            }

            if (inKillEverythingElse || !GC.ToolTipCurrentActive || (GC.ToolTipCurrentType != "push" && GC.ToolTipCurrentType != "skip"))
            {
                GC.CleanUpAllToolTipRelatedUI();

                GC.ToolTipShowHelpOption(inName + Localization.Get(inText));

                if (inAutoClose)
                {
                    StartCoroutine(DelayHideHelpfulTooltipPopup());
                }
            }
        }
    }

    public IEnumerator DelayHideHelpfulTooltipPopup()
    {
        string currentString = GC.ToolTipMainText;

        yield return new WaitForSeconds(4.5f); // this is harmless

        if (GC.ToolTipCurrentActive && GC.ToolTipCurrentType == "help" && GC.ToolTipMainText == currentString)
        {
            GC.CleanUpAllToolTipRelatedUI();
        }
    }

    public void ChangeCueUIImage(float id)
    {
       CueUI.GetComponent<GameCueBackground>().Cue.spriteName = id.ToString();
       CueUI.GetComponent<GameCueBackground>().CueMask.spriteName = id.ToString();
    }

    public void ActivateHandCursor()
    {
        Mouse.SetActive(true);
    }

    public void DeactivateHandCursor()
    {
        Mouse.SetActive(false);
    }

    public void ActivateFTUEMouse()
    {
        ftueSwipeCount = 0; // start ftue mouse count here

        FTUE_Mouse.SetActive(true);
    }

    public void DeactivateFTUEMouse()
    {
        FTUE_Mouse.SetActive(false);
    }

    public void SetFTUEMousePosition()
    {
        if (GameManager_script.Instance().SwipeFtueStage == 0)
        {
            GameManager_script.Instance().ftueMouseCursorPositionHack = new Vector3(GameManager_script.RobotHolePositions[2][0] + 1.0f, 5.0f, 0.0f); // always on the right edge of the table
        }
    }

    public void ChangeHead1BackgroundValue(float Val)
    {
        PlayerHead1.GetComponent<GamePlayerHead>().ChangeHeadBackgroundValue(Val);
    }

    public void ChangeHead2BackgroundValue(float Val)
    {
        PlayerHead2.GetComponent<GamePlayerHead>().ChangeHeadBackgroundValue(Val);
    }

    public float SmartBotRematchFakeClickTime()
    {
        float maxTime = GameManager_script.Instance().EndOfGameWaitAndChangeTime - GameManager_script.Instance().EndOfGameSmartBotDecideTime;
        float twoThirdTime = maxTime * 0.65f;
        float oneThirdTime = maxTime * 0.35f;
        float zeroThirdTime = maxTime * 0.05f;
        float time = 0.0f;

        if (GameManager_script.DetermineLotteryResult(0.50f)) // roughly 0.50f chance
        {
            time = Random.Range(zeroThirdTime, oneThirdTime);
        }
        else // roughly 0.50f chance
        {
            time = Random.Range(oneThirdTime, twoThirdTime);
        }

        return time;
    }

    public void SendGameStartSignal()
    {
        if (GameManager_script.Instance().FTUEInActionGame)
        {
            Analytic.GameStartPing("ftue", 0.0f, GameManager_script.Instance().CoinCount, GameManager_script.Instance().GetMaxTPAScore(), GameManager_script.Instance().Total_Games_Played, GameManager_script.Instance().Total_Games_Won);
        }
        else if (GameManager_script.Instance().TrulySelfInActionGame)
        {
            Analytic.GameStartPing("solo", 0.0f, GameManager_script.Instance().CoinCount, GameManager_script.Instance().GetMaxTPAScore(), GameManager_script.Instance().Total_Games_Played, GameManager_script.Instance().Total_Games_Won);
        }
        else if (GameManager_script.Instance().StupidBotInActionGame)
        {
            Analytic.GameStartPing("stupid", 0.0f, GameManager_script.Instance().CoinCount, GameManager_script.Instance().GetMaxTPAScore(), GameManager_script.Instance().Total_Games_Played, GameManager_script.Instance().Total_Games_Won);
        }
        else if (GameManager_script.Instance().SmartBotInActionGame)
        {
            if (GameManager_script.Instance().rematchCurrentMatchIsRematch)
            {
                Analytic.GameStartPing("smartRematch", GameManager_script.Instance().CurrentWager, GameManager_script.Instance().CoinCount, GameManager_script.Instance().GetMaxTPAScore(), GameManager_script.Instance().Total_Games_Played, GameManager_script.Instance().Total_Games_Won);
            }
            else
            {
                Analytic.GameStartPing("smart", GameManager_script.Instance().CurrentWager, GameManager_script.Instance().CoinCount, GameManager_script.Instance().GetMaxTPAScore(), GameManager_script.Instance().Total_Games_Played, GameManager_script.Instance().Total_Games_Won);
            }
        }
        else
        {
            if (GameManager_script.Instance().isEverythingFocusedOnFrdsSelector)
            {
                if (GameManager_script.Instance().rematchCurrentMatchIsRematch)
                {
                    Analytic.GameStartPing("friendRematch", GameManager_script.Instance().CurrentWager, GameManager_script.Instance().CoinCount, GameManager_script.Instance().GetMaxTPAScore(), GameManager_script.Instance().Total_Games_Played, GameManager_script.Instance().Total_Games_Won);
                }
                else
                {
                    Analytic.GameStartPing("friend", GameManager_script.Instance().CurrentWager, GameManager_script.Instance().CoinCount, GameManager_script.Instance().GetMaxTPAScore(), GameManager_script.Instance().Total_Games_Played, GameManager_script.Instance().Total_Games_Won);
                }
            }
            else
            {
                if (GameManager_script.Instance().rematchCurrentMatchIsRematch)
                {
                    Analytic.GameStartPing("strangerRematch", GameManager_script.Instance().CurrentWager, GameManager_script.Instance().CoinCount, GameManager_script.Instance().GetMaxTPAScore(), GameManager_script.Instance().Total_Games_Played, GameManager_script.Instance().Total_Games_Won);
                }
                else
                {
                    Analytic.GameStartPing("stranger", GameManager_script.Instance().CurrentWager, GameManager_script.Instance().CoinCount, GameManager_script.Instance().GetMaxTPAScore(), GameManager_script.Instance().Total_Games_Played, GameManager_script.Instance().Total_Games_Won);
                }
            }
        }
    }

    public void SendGameEndSignal(bool inWin, bool inShowWinLabelAnimation, int inSoloWinnerCount, bool inWinByDisconnect, bool inLoseByDisconnect, bool inTutorialWin)
    {
        // game type
        string gameType = "";

        if (GameManager_script.Instance().FTUEInActionGame)
        {
            gameType = "ftue";
        }
        else if (GameManager_script.Instance().TrulySelfInActionGame)
        {
            gameType = "solo";
        }
        else if (GameManager_script.Instance().StupidBotInActionGame)
        {
            gameType = "stupid";
        }
        else if (GameManager_script.Instance().SmartBotInActionGame)
        {
            if (GameManager_script.Instance().rematchCurrentMatchIsRematch)
            {
                gameType = "smartRematch";
            }
            else
            {
                gameType = "smart";
            }
        }
        else
        {
            if (GameManager_script.Instance().isEverythingFocusedOnFrdsSelector)
            {
                if (GameManager_script.Instance().rematchCurrentMatchIsRematch)
                {
                    gameType = "friendRematch";
                }
                else
                {
                    gameType = "friend";
                }
            }
            else
            {
                if (GameManager_script.Instance().rematchCurrentMatchIsRematch)
                {
                    gameType = "strangerRematch";
                }
                else
                {
                    gameType = "stranger";
                }
            }
        }
        
        // over type
        string finishType = "";
        
        if (!inShowWinLabelAnimation)
        {
            finishType = "selfQuit";
        }
        else
        {
            if (inWinByDisconnect)
            {
                finishType = "winByDisconnect";
            }
            else if (inLoseByDisconnect)
            {
                finishType = "loseByDisconnect";
            }
            else
            {
                finishType = "normalEnd";
            }
        }

        Analytic.GameFinishPing(gameType, inWin, Time.realtimeSinceStartup - gameDuration, finishType);
    }
    
    // smart bot logic starts here

    // Cue ball Pocket intentions
    [System.NonSerialized]
    public List<Vector3> SBIAimDirections = new List<Vector3>(0);

    // Pocket score
    [System.NonSerialized]
    public List<float> SBIScores = new List<float>(0);

    // Cue ball position intentions
    [System.NonSerialized]
    public List<Vector3> SBIPositions = new List<Vector3>(0);

    // initial gesture component
    [System.NonSerialized]
    public List<SmartBotGesture> InitialGestureSequence = new List<SmartBotGesture>(0);

    // final gesture component
    [System.NonSerialized]
    public List<SmartBotGesture> FinalGestureSequence = new List<SmartBotGesture>(0);

    // this function determines how sparse the balls are (the smaller the number, the less sparse)
    // emperical evidence, anything less than 4.0 is very much acceptable as a break shot
    public float DetermineSparsenessOfballs()
    {
        float sparseness = 0.0f;

        if (currentBallControllers.Count > 1)
        {
            for (int i = 1; i < currentBallControllers.Count; i++)
            {
                BallController obj = currentBallControllers[i];
                BallController current = currentBallControllers[1];

                sparseness += (obj.GetComponent<Rigidbody>().position - current.GetComponent<Rigidbody>().position).magnitude;
            }
        }
        else
        {
            sparseness = (GameManager_script.RobotHolePositions[5] - GameManager_script.RobotHolePositions[2]).magnitude * allBallControllers.Length;
        }

        if (currentBallControllers.Count < (int)(allBallControllers.Length - 3.0f)) // if 2 few balls, this is moot
        {
            sparseness = SparsenessThreshold;
        }
        else
        {
            sparseness /= currentBallControllers.Count;
        }

        return sparseness;
    }

    // this function kills all random stuffs
    public void ClearAllSmartBotIntentionStuff()
    {
        SBIAimDirections.Clear();
        SBIAimDirections.TrimExcess();

        SBIScores.Clear();
        SBIScores.TrimExcess();

        SBIPositions.Clear();
        SBIPositions.TrimExcess();

        BotCueBallPositions.Clear();
        BotCueBallPositions.TrimExcess();

        InitialGestureSequence.Clear();
        InitialGestureSequence.TrimExcess();

        FinalGestureSequence.Clear();
        FinalGestureSequence.TrimExcess();

        SequenceIndex = 0;
    }

    // this function is the master intention function, called once every shot
    public void IntentionOverAllFunction()
    {
        // determine the mood
        if (NetworkBotInControl())
        {
            DetermineMoodOfTheShot();
        }

        // add original position always (except when its cue shot, then fuck OG position)
        if (!(shotCount == 0))
        {
            BotCueBallPositions.Add(cueBallController.GetComponent<Rigidbody>().position);
        }

        // determine possible shot locations, ranking backward so when intention function iterate through them, the best is left for the last
        if (CueControllerUpdater.current_control_status == CueControllerUpdater.CUE_BALL_MOVABLE_ON_TABLE)
        {
            DetermineListOfGoodPositions();
        }

        // determine possible intentions
        for (int i = 0; i < BotCueBallPositions.Count; i++)
        {
            DetermineIntentionPositionAndPocket(BotCueBallPositions[i], BotCueBallPositions.Count - i);
        }

        // determine initial sequence
        if (true)
        {
            DetermineInitialSequenceOfGestures();
        }

        // prepare for movements
        if (true)
        {
            InitSequenceBoolGate = true;
            FinalSequenceBoolGate = false;
        }
    }

    // this function determines the mood of the shot (we hope to being the overall win rate towards 50%, bots and players)
    public void DetermineMoodOfTheShot()
    {
        // we will pick a random position between 0.25f to 0.50f of the total diagonal distance (we can use some eval curves here in the future, dajiang hack)
        GameManager_script.SmartBotMinDistanceForThisLongShot = Random.Range(0.25f, 0.50f) * GameManager_script.DiagDistance;

        // intermediate vars
        float LoseCount = GameManager_script.Instance().SmartBotGameLoseList.Count;
        float LoseNumber = GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().SmartBotGameLoseList);
        float TakenNumber = GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().SmartBotLongShotTakenList);
        float MadeNumber = GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().SmartBotLongShotMadeList);

        // we want to figure out if we intend to hit this shot
        float EmpiricalFactor = 0.475f; // dajiang hack
        float SmartBotLoseToPlayerPercentage = LoseCount == 0.0f ? 0.50f : LoseNumber / LoseCount;
        float SmartBotLongShotPerformance = TakenNumber == 0.0f ? 0.50f : MadeNumber / TakenNumber;
        float FinalProbability = SmartBotLongShotPerformance + SmartBotLoseToPlayerPercentage - EmpiricalFactor; // dajiang hack, use shot percentage and losing percentage as a baseline. 0.45f is sort of empirical

        // clamp it
        FinalProbability = Mathf.Clamp01(FinalProbability);

        // add a random factor based on our intent to win or lose (we can use some eval curves here in the future, dajiang hack)
        FinalProbability += GameManager_script.SmartBotIntendToWinGame ? Random.Range(0.325f, 0.525f) : Random.Range(-0.375f, -0.175f);

        // clamp it again
        FinalProbability = Mathf.Clamp01(FinalProbability);

        // final shot intention
        GameManager_script.SmartBotIntendToMakeThisLongShot = GameManager_script.DetermineLotteryResult(FinalProbability) ? true : false;

        // angle vs. distance factors
        GameManager_script.SmartBotDiagDistanceFactor = Random.Range(0.675f, 0.775f);
    }

    // this function determines if we should move positions (from far to near, on straight lines that rarely deviates from the line depending on clusterness)
    public void DetermineListOfGoodPositions()
    {
        if (CueControllerUpdater.current_control_status == CueControllerUpdater.CUE_BALL_MOVABLE_ON_TABLE)
        {
            if (currentBallControllers.Count > 1)
            {
                // 1 good extension from the closest pocket, best positions
                int numberOfExtAttempts = 15;
                float distanceFromPocket = 999.0f;
                Vector3 shortestDistancePosition = Vector3.zero;

                for (int i = 1; i < 7; i++) // 1 and 4 are middle pockets, somewhat restricted
                {
                    Vector3 hp = PocketController.FindeHolePositionById(i);
                    Vector3 objectBallPosition = currentBallControllers[1].GetComponent<Rigidbody>().position;
                    Vector3 objectToPocketDirection = (hp - objectBallPosition).normalized;

                    float distanceFromPocketTemp = (hp - objectBallPosition).magnitude;
                    bool objectClearsPocket = TargetBallWillClearPocket(currentBallControllers[1], objectBallPosition, objectToPocketDirection, hp, i, true);
                    bool objectSeesMidPocket = true;

                    if (i == 1 || i == 4)
                    {
                        Vector3 ZVerticalVector = new Vector3(0.0f, 0.0f, 1.0f);

                        if (Mathf.Abs(Vector3.Dot(objectToPocketDirection.normalized, ZVerticalVector.normalized)) < FourtyDegree)
                        {
                            objectSeesMidPocket = false;
                        }
                    }

                    if (objectClearsPocket && objectSeesMidPocket)
                    {
                        if (distanceFromPocketTemp < distanceFromPocket)
                        {
                            distanceFromPocket = distanceFromPocketTemp;
                            shortestDistancePosition = hp;
                        }
                    }
                }

                if (shortestDistancePosition != Vector3.zero)
                {
                    bool FoundAnExtensionPosition = false;
                    int NumberOfExtensionTriesAttempted = 0;

                    while (!FoundAnExtensionPosition && NumberOfExtensionTriesAttempted < numberOfExtAttempts)
                    {
                        Vector3 extensionDirectionAndLength = (currentBallControllers[1].GetComponent<Rigidbody>().position - shortestDistancePosition) * Random.Range(0.15f, 0.75f);
                        Vector3 extensionSideWay = Random.Range(-0.15f, 0.15f) * VectorOperator.getPerpendicularXZ(extensionDirectionAndLength);
                        Vector3 extensionSeparation = (currentBallControllers[1].GetComponent<Rigidbody>().position - shortestDistancePosition).normalized * ballRadius * 2.0f;
                        Vector3 extensionFromPocket = extensionDirectionAndLength + extensionSeparation + extensionSideWay + currentBallControllers[1].GetComponent<Rigidbody>().position;
                        Vector3 randomizedPositionRevised = DetermineTrueWhenPositionOnTableAndViable(extensionFromPocket);
                        Vector3 positionToObjectDirection = (currentBallControllers[1].GetComponent<Rigidbody>().position - randomizedPositionRevised).normalized;

                        bool positionToObjectPossible = CueBallWillHitTargetBall(cueBallController, randomizedPositionRevised, positionToObjectDirection, currentBallControllers[1]);

                        if (randomizedPositionRevised != Vector3.zero && positionToObjectPossible)
                        {
                            FoundAnExtensionPosition = true;

                            BotCueBallPositions.Add(randomizedPositionRevised);
                        }

                        NumberOfExtensionTriesAttempted += 1;
                    }
                }
                
                // 1 random position right beside the ball
                int numberOfNearbyAttempts = 15;
                int NumberOfTriesAttemptedCloseToBall = 0;
                bool FoundARandomPositionCloseToBall = false;

                while (!FoundARandomPositionCloseToBall && NumberOfTriesAttemptedCloseToBall < numberOfNearbyAttempts)
                {
                    float randomX = Random.Range(0, 2) == 1 ? Random.Range(-10.0f, -3.0f) * ballRadius : Random.Range(3.0f, 10.0f) * ballRadius;
                    float randomZ = Random.Range(0, 2) == 1 ? Random.Range(-10.0f, -3.0f) * ballRadius : Random.Range(3.0f, 10.0f) * ballRadius;

                    Vector3 randomizedPositionCloseToBall = currentBallControllers[1].GetComponent<Rigidbody>().position + new Vector3(randomX, 0.0f, randomZ);
                    Vector3 randomizedPositionCloseToBallRevised = DetermineTrueWhenPositionOnTableAndViable(randomizedPositionCloseToBall);
                    Vector3 randomizedpositionToObjectDirection = (currentBallControllers[1].GetComponent<Rigidbody>().position - randomizedPositionCloseToBallRevised).normalized;

                    bool randomizedPositionToObjectPossible = CueBallWillHitTargetBall(cueBallController, randomizedPositionCloseToBallRevised, randomizedpositionToObjectDirection, currentBallControllers[1]);

                    if (randomizedPositionCloseToBallRevised != Vector3.zero && randomizedPositionToObjectPossible)
                    {
                        FoundARandomPositionCloseToBall = true;

                        BotCueBallPositions.Add(randomizedPositionCloseToBallRevised);
                    }

                    NumberOfTriesAttemptedCloseToBall += 1;
                }
            }
        }
    }

    // helper function, helps determine if a position is good
    public Vector3 DetermineTrueWhenPositionOnTableAndViable(Vector3 inRandPosition)
    {
        Vector3 returnVec = Vector3.zero;
        Transform StartOrMoveCube = shotCount == 0 ? StartCube : MoveCube;

        bool isHittingTargetBall = CueBallWillHitTargetBall(cueBallController, inRandPosition, (currentBallControllers[1].GetComponent<Rigidbody>().position - inRandPosition).normalized, currentBallControllers[1]);
        bool isFitPositionInBounds = VectorOperator.FitPositionInCubeBounds(StartOrMoveCube, inRandPosition, ballRadius);

        if (isFitPositionInBounds && isHittingTargetBall)
        {
            Ray ray = new Ray(inRandPosition + 30.0f * ballRadius * Vector3.up, -Vector3.up);
            bool hittingCanvas = false;
            RaycastHit hit;

            if (Physics.SphereCast(ray, 1.0f * ballRadius, out hit, 1000.0f, canvasAndBallMask))
            {
                hittingCanvas = hit.collider.gameObject.layer == LayerMask.NameToLayer("Canvas");
            }

            if (hittingCanvas)
            {
                returnVec = new Vector3(inRandPosition.x, mainBallPoint.position.y, inRandPosition.z);
            }
        }

        return returnVec;
    }

    // this function determines intention and ranks (we are basically using the old ranking stuff but doing it more fine-tuned)
    // this also differs from the "stupid bot" coz this is mostly intentions and guessing before actually move the cue ball direction over
    // we are keeping ranked candidates
    public void DetermineIntentionPositionAndPocket(Vector3 inCueBallPosition, int inMultiple)
    {
        if (currentBallControllers.Count > 1)
        {
            // we go through the pockets and find all possible pockets that we can shoot over to
            for (int i = 1; i < 7; i++)
            {
                // get HP
                Vector3 hp = PocketController.FindeHolePositionById(i);

                Vector3 ObjectPocketDirection = (hp - currentBallControllers[1].GetComponent<Rigidbody>().position).normalized;
                Vector3 CueBallPocketDirection = (hp - inCueBallPosition).normalized;
                Vector3 CueBallObjectDirection = (currentBallControllers[1].GetComponent<Rigidbody>().position - inCueBallPosition).normalized;
                Vector3 CueBallFinalAimDirection = ((currentBallControllers[1].GetComponent<Rigidbody>().position - ObjectPocketDirection * ballRadius * 2.0f) - inCueBallPosition).normalized;

                float CueObjectPocketAngle = Vector3.Dot(CueBallObjectDirection, ObjectPocketDirection);
                float ObjectPocketDistance = (hp - currentBallControllers[1].GetComponent<Rigidbody>().position).magnitude;
                float DistanceBonusFactor = Mathf.Clamp((GameManager_script.DiagDistance * GameManager_script.SmartBotDiagDistanceFactor - ObjectPocketDistance) / GameManager_script.DiagDistance, 0.05f, 0.95f); // symmetry, all about symmetry
                float ScoreThresholdCandidate = currentBallControllers.Count > 2 ? TwentyDegree : FourtyDegree; // if it is 9 ball and cue ball ONLY, be a bit more strict
                float ScoreThreshold = inMultiple > 1 ? ScoreThresholdCandidate : 0.0f;
                float FinalScore = -1.0f;

                bool FinalPositionHitPossible = CueBallWillHitTargetBall(cueBallController, inCueBallPosition, CueBallFinalAimDirection, currentBallControllers[1]);
                bool ObjectBallClearPocketPossible = TargetBallWillClearPocket(currentBallControllers[1], currentBallControllers[1].GetComponent<Rigidbody>().position, ObjectPocketDirection, hp, i, true);

                if (i == 1 || i == 4)
                {
                    Vector3 ZVerticalVector = new Vector3(0.0f, 0.0f, 1.0f);
                    
                    if (Mathf.Abs(Vector3.Dot(ObjectPocketDirection.normalized, ZVerticalVector.normalized)) > FourtyDegree)
                    {
                        if (CueObjectPocketAngle > ScoreThreshold) // a very wide angle
                        {
                            FinalScore = CueObjectPocketAngle * DistanceBonusFactor;
                        }
                        else if (ObjectPocketDistance < ballRadius * ballRadiusMultiple) // a very small distance
                        {
                            FinalScore = 1.0f;
                        }
                    }
                }
                else
                {
                    if (CueObjectPocketAngle > ScoreThreshold) // a very wide angle for normal and small angle for moveable bots
                    {
                        FinalScore = CueObjectPocketAngle * DistanceBonusFactor;
                    }
                    else if (ObjectPocketDistance < ballRadius * ballRadiusMultiple) // a very small distance
                    {
                        FinalScore = 1.0f;
                    }
                }

                if (inMultiple == 1)
                {
                    // if final position is at all possible, final score threshold is good
                    if (FinalPositionHitPossible && FinalScore >= ScoreThreshold)
                    {
                        InsertIntentionSequence(CueBallFinalAimDirection, FinalScore, inCueBallPosition);
                    }
                }
                else
                {
                    // if we are moveable positions, we will be very strict and make sure the target pocket is also clear
                    if (FinalPositionHitPossible && FinalScore >= ScoreThreshold && ObjectBallClearPocketPossible)
                    {
                        InsertIntentionSequence(CueBallFinalAimDirection, FinalScore, inCueBallPosition);
                    }
                }
            }
            
            // we have to come up with something coz we are on our last position (or the only position).
            if (inMultiple == 1)
            {
                // Y position 
                float bothY = GameManager_script.RobotHolePositions[1][1];
                float verticalTop = GameManager_script.RobotHolePositions[4][2];
                float verticalBot = GameManager_script.RobotHolePositions[1][2];
                float horizontalLeft = GameManager_script.RobotHolePositions[5][0];
                float horizontalRite = GameManager_script.RobotHolePositions[2][0];

                // calculate the 2 banking positions vertical
                float verticalTopCueBallRatio = Mathf.Abs((inCueBallPosition.z - verticalTop) / (currentBallControllers[1].GetComponent<Rigidbody>().position.z - verticalTop));
                float verticalBotCueBallRatio = Mathf.Abs((inCueBallPosition.z - verticalBot) / (currentBallControllers[1].GetComponent<Rigidbody>().position.z - verticalBot));
                float verticalTopX = inCueBallPosition.x + (currentBallControllers[1].GetComponent<Rigidbody>().position.x - inCueBallPosition.x) * (verticalTopCueBallRatio / (1.0f + verticalTopCueBallRatio));
                float verticalBotX = inCueBallPosition.x + (currentBallControllers[1].GetComponent<Rigidbody>().position.x - inCueBallPosition.x) * (verticalBotCueBallRatio / (1.0f + verticalBotCueBallRatio));
                float verticalTopScore = -1.0f * Mathf.Abs(currentBallControllers[1].GetComponent<Rigidbody>().position.z - verticalTop);
                float verticalBotScore = -1.0f * Mathf.Abs(currentBallControllers[1].GetComponent<Rigidbody>().position.z - verticalBot);

                // top
                Vector3 verticalTopBankLocation = new Vector3(verticalTopX, bothY, verticalTop);
                Vector3 verticalTopCueBankDirection = (verticalTopBankLocation - inCueBallPosition).normalized;
                Vector3 verticalTopBankBallDirection = (currentBallControllers[1].GetComponent<Rigidbody>().position - verticalTopBankLocation).normalized;

                if (RayWillNotHitOtherBallsStrict(inCueBallPosition, verticalTopCueBankDirection, currentBallControllers[1]) && RayWillNotHitOtherBallsStrict(verticalTopBankLocation, verticalTopBankBallDirection, currentBallControllers[1]))
                {
                    InsertIntentionSequence((verticalTopBankLocation - inCueBallPosition).normalized, verticalTopScore, inCueBallPosition);
                }
                else
                {
                    InsertIntentionSequence((verticalTopBankLocation - inCueBallPosition).normalized, Random.Range(-2.0f, -1.0f) * GameManager_script.DiagDistance, inCueBallPosition);
                }

                // bot
                Vector3 verticalBotBankLocation = new Vector3(verticalBotX, bothY, verticalBot);
                Vector3 verticalBotCueBankDirection = (verticalBotBankLocation - inCueBallPosition).normalized;
                Vector3 verticalBotBankBallDirection = (currentBallControllers[1].GetComponent<Rigidbody>().position - verticalBotBankLocation).normalized;

                if (RayWillNotHitOtherBallsStrict(inCueBallPosition, verticalBotCueBankDirection, currentBallControllers[1]) && RayWillNotHitOtherBallsStrict(verticalBotBankLocation, verticalBotBankBallDirection, currentBallControllers[1]))
                {
                    InsertIntentionSequence((verticalBotBankLocation - inCueBallPosition).normalized, verticalBotScore, inCueBallPosition);
                }
                else
                {
                    InsertIntentionSequence((verticalBotBankLocation - inCueBallPosition).normalized, Random.Range(-2.0f, -1.0f) * GameManager_script.DiagDistance, inCueBallPosition);
                }

                // calculate the 2 banking positions horizontal
                float horizontalLeftCueBallRatio = Mathf.Abs((inCueBallPosition.x - horizontalLeft) / (currentBallControllers[1].GetComponent<Rigidbody>().position.x - horizontalLeft));
                float horizontalRiteCueBallRatio = Mathf.Abs((inCueBallPosition.x - horizontalRite) / (currentBallControllers[1].GetComponent<Rigidbody>().position.x - horizontalRite));
                float horizontalLeftZ = inCueBallPosition.z + (currentBallControllers[1].GetComponent<Rigidbody>().position.z - inCueBallPosition.z) * (horizontalLeftCueBallRatio / (1.0f + horizontalLeftCueBallRatio));
                float horizontalRiteZ = inCueBallPosition.z + (currentBallControllers[1].GetComponent<Rigidbody>().position.z - inCueBallPosition.z) * (horizontalRiteCueBallRatio / (1.0f + horizontalRiteCueBallRatio));
                float horizontalLeftScore = -1.0f * Mathf.Abs(currentBallControllers[1].GetComponent<Rigidbody>().position.x - horizontalLeft);
                float horizontalRiteScore = -1.0f * Mathf.Abs(currentBallControllers[1].GetComponent<Rigidbody>().position.x - horizontalRite);

                // left
                Vector3 horizontalLeftBankLocation = new Vector3(horizontalLeft, bothY, horizontalLeftZ);
                Vector3 horizontalLeftCueBankDirection = (horizontalLeftBankLocation - inCueBallPosition).normalized;
                Vector3 horizontalLeftBankBallDirection = (currentBallControllers[1].GetComponent<Rigidbody>().position - horizontalLeftBankLocation).normalized;

                if (RayWillNotHitOtherBallsStrict(inCueBallPosition, horizontalLeftCueBankDirection, currentBallControllers[1]) && RayWillNotHitOtherBallsStrict(horizontalLeftBankLocation, horizontalLeftBankBallDirection, currentBallControllers[1]))
                {
                    InsertIntentionSequence((horizontalLeftBankLocation - inCueBallPosition).normalized, horizontalLeftScore, inCueBallPosition);
                }
                else
                {
                    InsertIntentionSequence((horizontalLeftBankLocation - inCueBallPosition).normalized, Random.Range(-2.0f, -1.0f) * GameManager_script.DiagDistance, inCueBallPosition);
                }

                // rite
                Vector3 horizontalRiteBankLocation = new Vector3(horizontalRite, bothY, horizontalRiteZ);
                Vector3 horizontalRiteCueBankDirection = (horizontalRiteBankLocation - inCueBallPosition).normalized;
                Vector3 horizontalRiteBankBallDirection = (currentBallControllers[1].GetComponent<Rigidbody>().position - horizontalRiteBankLocation).normalized;

                if (RayWillNotHitOtherBallsStrict(inCueBallPosition, horizontalRiteCueBankDirection, currentBallControllers[1]) && RayWillNotHitOtherBallsStrict(horizontalRiteBankLocation, horizontalRiteBankBallDirection, currentBallControllers[1]))
                {
                    InsertIntentionSequence((horizontalRiteBankLocation - inCueBallPosition).normalized, horizontalRiteScore, inCueBallPosition);
                }
                else
                {
                    InsertIntentionSequence((horizontalRiteBankLocation - inCueBallPosition).normalized, Random.Range(-2.0f, -1.0f) * GameManager_script.DiagDistance, inCueBallPosition);
                }
            }
        }
    }

    // we should take EVERY gesture, not just the ones that appears more promising than the last one
    public void InsertIntentionSequence(Vector3 inAimDirection, float inFinalScore, Vector3 inCueBallPosition)
    {
        bool inserted = false;

        if (SBIPositions.Count == 0)
        {
            SBIAimDirections.Add(inAimDirection);
            SBIPositions.Add(inCueBallPosition);
            SBIScores.Add(inFinalScore);

            inserted = true;
        }
        else
        {
            for (int j = 0; j < SBIPositions.Count; j++)
            {
                if (inFinalScore > SBIScores[j])
                {
                    SBIAimDirections.Insert(j, inAimDirection);
                    SBIPositions.Insert(j, inCueBallPosition);
                    SBIScores.Insert(j, inFinalScore);

                    inserted = true;

                    break;
                }
            }

            if (!inserted)
            {
                SBIAimDirections.Add(inAimDirection);
                SBIPositions.Add(inCueBallPosition);
                SBIScores.Add(inFinalScore);
            }
        }
    }

    // current robotic bool gate
    [System.NonSerialized]
    public bool InitSequenceBoolGate = false;

    // current robotic bool gate for final
    [System.NonSerialized]
    public bool FinalSequenceBoolGate = false;

    // current gesture is the only one we care about
    [System.NonSerialized]
    public bool KeyGestureBoolGate = false;

    // current robotic index
    [System.NonSerialized]
    public int SequenceIndex = 0;

    // current robotic time for a single gesture
    [System.NonSerialized]
    public float SequenceTimeLimit = -1;

    // current robotic curve for current gesture
    [System.NonSerialized]
    public float SequenceCursor = -1;

    // the curve used by the current gesture
    [System.NonSerialized]
    public AnimationCurve SequenceCurrentCurve = null;

    // the curve used by the current gesture
    [System.NonSerialized]
    public AnimationCurve SequenceCurrentCurveX = null;

    // the curve used by the current gesture
    [System.NonSerialized]
    public AnimationCurve SequenceCurrentCurveY = null;

    // swipes
    [System.NonSerialized]
    public List<AnimationCurve> SwipeShortTimeSmallAngleFastCurves = new List<AnimationCurve>(0);
    [System.NonSerialized]
    public List<AnimationCurve> SwipeShortTimeSmallAngleCurves = new List<AnimationCurve>(0);
    [System.NonSerialized]
    public List<AnimationCurve> SwipeLongTimeSmallAngleCurves = new List<AnimationCurve>(0);
    [System.NonSerialized]
    public List<AnimationCurve> SwipeLongTimeBigAngleCurves = new List<AnimationCurve>(0);

    // moves
    [System.NonSerialized]
    public List<AnimationCurve> MoveLongCurves = new List<AnimationCurve>(0);
    [System.NonSerialized]
    public List<AnimationCurve> MoveMediumCurves = new List<AnimationCurve>(0);
    [System.NonSerialized]
    public List<AnimationCurve> MoveShortCurves = new List<AnimationCurve>(0);

    // pumps
    [System.NonSerialized]
    public List<AnimationCurve> PumpLongCurves = new List<AnimationCurve>(0);
    [System.NonSerialized]
    public List<AnimationCurve> PumpShortCurves = new List<AnimationCurve>(0);

    // shadow spins
    [System.NonSerialized]
    public List<AnimationCurve> SpinLongCurves = new List<AnimationCurve>(0);
    [System.NonSerialized]
    public List<AnimationCurve> SpinMediumCurves = new List<AnimationCurve>(0);
    [System.NonSerialized]
    public List<AnimationCurve> SpinShortCurves = new List<AnimationCurve>(0);

    // put all the curves in the right places
    public void PutSmartBotCurvesInLists()
    {
        // super fast lolz
        SwipeShortTimeSmallAngleFastCurves.Add(GameManager_script.Instance().Spin0);
        SwipeShortTimeSmallAngleFastCurves.Add(GameManager_script.Instance().Spin1);
        SwipeShortTimeSmallAngleFastCurves.Add(GameManager_script.Instance().Spin2);
        SwipeShortTimeSmallAngleFastCurves.Add(GameManager_script.Instance().Spin3);
        SwipeShortTimeSmallAngleFastCurves.Add(GameManager_script.Instance().Spin4);
        SwipeShortTimeSmallAngleFastCurves.Add(GameManager_script.Instance().Smooth0);
        SwipeShortTimeSmallAngleFastCurves.Add(GameManager_script.Instance().Smooth1);
        SwipeShortTimeSmallAngleFastCurves.Add(GameManager_script.Instance().Smooth2);
        SwipeShortTimeSmallAngleFastCurves.Add(GameManager_script.Instance().Smooth3);
        SwipeShortTimeSmallAngleFastCurves.Add(GameManager_script.Instance().Smooth4);

        // short time, small angle
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Smooth0);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Smooth1);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Smooth2);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Smooth3);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Smooth4);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Random5);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Random6);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Random7);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Random9);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Random11);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Random16);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Random17);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Random18);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Spin10);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Spin11);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Spin12);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Spin13);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().Spin14);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().CuePump5);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().CuePump6);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().CuePump7);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().CuePump8);
        SwipeShortTimeSmallAngleCurves.Add(GameManager_script.Instance().CuePump9);

        // long time, small angle
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Smooth2);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Smooth3);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Smooth4);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random4);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random5);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random6);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random7);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random8);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random9);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random10);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random11);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random12);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random13);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random14);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random15);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random16);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random17);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random18);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random19);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random20);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random21);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random22);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random23);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Random24);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Spin10);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Spin11);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Spin12);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Spin13);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().Spin14);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().CuePump5);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().CuePump6);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().CuePump7);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().CuePump8);
        SwipeLongTimeSmallAngleCurves.Add(GameManager_script.Instance().CuePump9);

        // long long time, big angles
        SwipeLongTimeBigAngleCurves.Add(GameManager_script.Instance().Random5);
        SwipeLongTimeBigAngleCurves.Add(GameManager_script.Instance().Random6);
        SwipeLongTimeBigAngleCurves.Add(GameManager_script.Instance().Random7);
        SwipeLongTimeBigAngleCurves.Add(GameManager_script.Instance().Random14);
        SwipeLongTimeBigAngleCurves.Add(GameManager_script.Instance().Random16);
        SwipeLongTimeBigAngleCurves.Add(GameManager_script.Instance().Random17);
        SwipeLongTimeBigAngleCurves.Add(GameManager_script.Instance().Random18);
        SwipeLongTimeBigAngleCurves.Add(GameManager_script.Instance().Random19);
        SwipeLongTimeBigAngleCurves.Add(GameManager_script.Instance().Spin5);
        SwipeLongTimeBigAngleCurves.Add(GameManager_script.Instance().Spin7);
        SwipeLongTimeBigAngleCurves.Add(GameManager_script.Instance().Spin9);
        SwipeLongTimeBigAngleCurves.Add(GameManager_script.Instance().Spin12);

        // move ballz long version, we will only accept the long curves
        MoveLongCurves.Add(GameManager_script.Instance().Spin10);
        MoveLongCurves.Add(GameManager_script.Instance().Spin11);
        MoveLongCurves.Add(GameManager_script.Instance().Spin12);
        MoveLongCurves.Add(GameManager_script.Instance().Spin13);
        MoveLongCurves.Add(GameManager_script.Instance().Spin14);

        // move ballz mid version, we will only accept the mid curves
        MoveMediumCurves.Add(GameManager_script.Instance().Spin5); 
        MoveMediumCurves.Add(GameManager_script.Instance().Spin6); 
        MoveMediumCurves.Add(GameManager_script.Instance().Spin7); 
        MoveMediumCurves.Add(GameManager_script.Instance().Spin8); 
        MoveMediumCurves.Add(GameManager_script.Instance().Spin9); 

        // move ballz short version, we accept all smooth stuff (maybe even some randomness)
        MoveShortCurves.Add(GameManager_script.Instance().Spin0); 
        MoveShortCurves.Add(GameManager_script.Instance().Spin1); 
        MoveShortCurves.Add(GameManager_script.Instance().Spin2); 
        MoveShortCurves.Add(GameManager_script.Instance().Spin3); 
        MoveShortCurves.Add(GameManager_script.Instance().Spin4); 
        MoveShortCurves.Add(GameManager_script.Instance().Smooth0); 
        MoveShortCurves.Add(GameManager_script.Instance().Smooth1); 
        MoveShortCurves.Add(GameManager_script.Instance().Smooth2); 
        MoveShortCurves.Add(GameManager_script.Instance().Smooth3); 
        MoveShortCurves.Add(GameManager_script.Instance().Smooth4);

        // spin ballz long version, we will only accept the long curves
        SpinLongCurves.Add(GameManager_script.Instance().Spin10); 
        SpinLongCurves.Add(GameManager_script.Instance().Spin11); 
        SpinLongCurves.Add(GameManager_script.Instance().Spin12); 
        SpinLongCurves.Add(GameManager_script.Instance().Spin13); 
        SpinLongCurves.Add(GameManager_script.Instance().Spin14);

        // spin ballz mid version, we will only accept the mid curves
        SpinMediumCurves.Add(GameManager_script.Instance().Spin5); 
        SpinMediumCurves.Add(GameManager_script.Instance().Spin6); 
        SpinMediumCurves.Add(GameManager_script.Instance().Spin7); 
        SpinMediumCurves.Add(GameManager_script.Instance().Spin8); 
        SpinMediumCurves.Add(GameManager_script.Instance().Spin9);
        
        // spin ballz short version, we accept all smooth stuff (maybe even some randomness)
        SpinShortCurves.Add(GameManager_script.Instance().Spin0); 
        SpinShortCurves.Add(GameManager_script.Instance().Spin1); 
        SpinShortCurves.Add(GameManager_script.Instance().Spin2); 
        SpinShortCurves.Add(GameManager_script.Instance().Spin3); 
        SpinShortCurves.Add(GameManager_script.Instance().Spin4);
        SpinShortCurves.Add(GameManager_script.Instance().Smooth0); 
        SpinShortCurves.Add(GameManager_script.Instance().Smooth1); 
        SpinShortCurves.Add(GameManager_script.Instance().Smooth2); 
        SpinShortCurves.Add(GameManager_script.Instance().Smooth3); 
        SpinShortCurves.Add(GameManager_script.Instance().Smooth4);

        // pumps long version, we accept all long pumps (maybe some randomness even)
        PumpLongCurves.Add(GameManager_script.Instance().CuePump0); 
        PumpLongCurves.Add(GameManager_script.Instance().CuePump1); 
        PumpLongCurves.Add(GameManager_script.Instance().CuePump2); 
        PumpLongCurves.Add(GameManager_script.Instance().CuePump3); 
        PumpLongCurves.Add(GameManager_script.Instance().CuePump4);
        PumpLongCurves.Add(GameManager_script.Instance().CuePump5); 
        PumpLongCurves.Add(GameManager_script.Instance().CuePump6); 
        PumpLongCurves.Add(GameManager_script.Instance().CuePump7); 
        PumpLongCurves.Add(GameManager_script.Instance().CuePump8); 
        PumpLongCurves.Add(GameManager_script.Instance().CuePump9);

        // pumps short version, we accept all short and smooth
        PumpShortCurves.Add(GameManager_script.Instance().CuePump0); 
        PumpShortCurves.Add(GameManager_script.Instance().CuePump1); 
        PumpShortCurves.Add(GameManager_script.Instance().CuePump2); 
        PumpShortCurves.Add(GameManager_script.Instance().CuePump3); 
        PumpShortCurves.Add(GameManager_script.Instance().CuePump4);
        PumpShortCurves.Add(GameManager_script.Instance().Smooth0); 
        PumpShortCurves.Add(GameManager_script.Instance().Smooth1); 
        PumpShortCurves.Add(GameManager_script.Instance().Smooth2); 
        PumpShortCurves.Add(GameManager_script.Instance().Smooth3); 
        PumpShortCurves.Add(GameManager_script.Instance().Smooth4);
    }

    // this function handles most of the update functions
    public void SmartRobotUpdate()
    {
        if (InitSequenceBoolGate && KeyGestureBoolGate)
        {
            RobotAnalysis();
        }

        if (InitSequenceBoolGate)
        {
            PerformSequence(InitialGestureSequence);
        }

        if (FinalSequenceBoolGate)
        {
            PerformSequence(FinalGestureSequence);
        }
    }

    // sequencer
    public void PerformSequence(List<SmartBotGesture> inSequence)
    {
        if (SequenceIndex < inSequence.Count)
        {
            SmartBotGesture SBG = inSequence[SequenceIndex];

            // start of the gesture (should we start it at all)
            if (SequenceCurrentCurve == null && SequenceCurrentCurveX == null && SequenceCurrentCurveY == null && SequenceTimeLimit == -1 && SequenceCursor == -1)
            {
                // time to put in initial state before beginning this gesture
                SBG.InitPosition = cueBallController.GetComponent<Rigidbody>().position;
                SBG.InitDirection = cuePivot.forward;
                SBG.InitSpin = cueBallPivot;
                SBG.InitStrength = cueDisplacement;

                if (SBG.LocalGestureType == GestureType.MoveCueBallToDestination)
                {
                    // 0.0f to very close to 1.0f completely diag
                    float distance = Mathf.Clamp01((SBG.InitPosition - SBG.DestPosition).magnitude / GameManager_script.DiagDistance);

                    // pretty freaking far now
                    if (distance > 0.20f)
                    {
                        if (GameManager_script.DetermineLotteryResult(0.30f) && !GameManager_script.SmartBotFastDrawShotMood)
                        {
                            SequenceCurrentCurveX = MoveLongCurves[Random.Range(0, MoveLongCurves.Count)];
                            SequenceCurrentCurveY = MoveLongCurves[Random.Range(0, MoveLongCurves.Count)];
                            SequenceTimeLimit = Random.Range(5.0f, 6.0f);
                        }
                        else if (GameManager_script.DetermineLotteryResult(0.50f))
                        {
                            SequenceCurrentCurveX = MoveMediumCurves[Random.Range(0, MoveMediumCurves.Count)];
                            SequenceCurrentCurveY = MoveMediumCurves[Random.Range(0, MoveMediumCurves.Count)];
                            SequenceTimeLimit = Random.Range(4.0f, 5.0f);
                        }
                        else
                        {
                            SequenceCurrentCurveX = MoveShortCurves[Random.Range(0, MoveShortCurves.Count)];
                            SequenceCurrentCurveY = MoveShortCurves[Random.Range(0, MoveShortCurves.Count)];
                            SequenceTimeLimit = Random.Range(3.0f, 4.0f);
                        }
                    }
                    else
                    {
                        if (GameManager_script.DetermineLotteryResult(0.20f) && !GameManager_script.SmartBotFastDrawShotMood)
                        {
                            SequenceCurrentCurveX = MoveMediumCurves[Random.Range(0, MoveMediumCurves.Count)];
                            SequenceCurrentCurveY = MoveMediumCurves[Random.Range(0, MoveMediumCurves.Count)];
                            SequenceTimeLimit = Random.Range(3.5f, 4.5f);
                        }
                        else
                        {
                            SequenceCurrentCurveX = MoveShortCurves[Random.Range(0, MoveShortCurves.Count)];
                            SequenceCurrentCurveY = MoveShortCurves[Random.Range(0, MoveShortCurves.Count)];
                            SequenceTimeLimit = Random.Range(2.5f, 3.5f);
                        }
                    }
                }

                if (SBG.LocalGestureType == GestureType.RotateTowardsDestination)
                {
                    // -1.0f completely opposite to 1.0f completely straight
                    float angle = Vector3.Dot(SBG.InitDirection, SBG.DestDirection);

                    // algo for determining which is which
                    if (angle < SeventyFiveDegree)
                    {
                        // smooth big anglez
                        SequenceCurrentCurve = SwipeLongTimeBigAngleCurves[Random.Range(0, SwipeLongTimeBigAngleCurves.Count)];
                        SequenceTimeLimit = Random.Range(4.0f, 5.0f);
                    }
                    else
                    {
                        // small angle whateverz
                        if (GameManager_script.DetermineLotteryResult(0.30f) && !GameManager_script.SmartBotFastDrawShotMood) // occassionally long time coz angle is small
                        {
                            SequenceCurrentCurve = SwipeLongTimeSmallAngleCurves[Random.Range(0, SwipeLongTimeSmallAngleCurves.Count)];
                            SequenceTimeLimit = Random.Range(6.0f, 7.0f);
                        }
                        else // mostly short time coz angle is small
                        {
                            SequenceCurrentCurve = SwipeShortTimeSmallAngleCurves[Random.Range(0, SwipeShortTimeSmallAngleCurves.Count)];
                            SequenceTimeLimit = Random.Range(3.0f, 4.0f);
                        }
                    }
                }

                if (SBG.LocalGestureType == GestureType.RotateTowardsDestinationShort)
                {
                    // use super fast
                    SequenceCurrentCurve = SwipeShortTimeSmallAngleFastCurves[Random.Range(0, SwipeShortTimeSmallAngleFastCurves.Count)];
                    SequenceTimeLimit = Random.Range(1.0f, 2.0f);
                }

                if (SBG.LocalGestureType == GestureType.GiveLocalBallSpin)
                {
                    if (GameManager_script.DetermineLotteryResult(0.10f) && !GameManager_script.SmartBotFastDrawShotMood)
                    {
                        // 2 steps variant
                        SequenceCurrentCurveX = SpinLongCurves[Random.Range(0, SpinLongCurves.Count)];
                        SequenceCurrentCurveY = SpinLongCurves[Random.Range(0, SpinLongCurves.Count)];
                        SequenceTimeLimit = Random.Range(2.5f, 3.5f);
                    }
                    else if (GameManager_script.DetermineLotteryResult(0.10f) && !GameManager_script.SmartBotFastDrawShotMood)
                    {
                        // 1 step variant
                        SequenceCurrentCurveX = SpinMediumCurves[Random.Range(0, SpinMediumCurves.Count)];
                        SequenceCurrentCurveY = SpinMediumCurves[Random.Range(0, SpinMediumCurves.Count)];
                        SequenceTimeLimit = Random.Range(1.5f, 2.5f);
                    }
                    else
                    {
                        // short variant
                        SequenceCurrentCurveX = SpinShortCurves[Random.Range(0, SpinShortCurves.Count)];
                        SequenceCurrentCurveY = SpinShortCurves[Random.Range(0, SpinShortCurves.Count)];
                        SequenceTimeLimit = Random.Range(0.5f, 1.5f);
                    }
                }

                if (SBG.LocalGestureType == GestureType.CuePump)
                {
                    if (GameManager_script.DetermineLotteryResult(0.15f) && !GameManager_script.SmartBotFastDrawShotMood)
                    {
                        // take long time to pump with a few traverses
                        SequenceCurrentCurve = PumpLongCurves[Random.Range(0, PumpLongCurves.Count)];
                        SequenceTimeLimit = Random.Range(2.5f, 5.0f);
                    }
                    else
                    {
                        // generic and generally very quick
                        SequenceCurrentCurve = PumpShortCurves[Random.Range(0, PumpShortCurves.Count)];
                        SequenceTimeLimit = Random.Range(0.25f, 1.0f);
                    }
                }

                if (SBG.LocalGestureType == GestureType.DoNothing)
                {
                    if (GameManager_script.DetermineLotteryResult(0.15f))
                    {
                        // take some timez
                        SequenceCurrentCurve = GameManager_script.Instance().PlaceHolder0;
                        SequenceTimeLimit = Random.Range(0.75f, 1.00f);
                    }
                    else
                    {
                        // generic and generally very quick
                        SequenceCurrentCurve = GameManager_script.Instance().PlaceHolder0;
                        SequenceTimeLimit = Random.Range(0.50f, 0.75f);
                    }
                }

                if (SBG.LocalGestureType == GestureType.DoNothingCompletely)
                {
                    // take some timez
                    SequenceCurrentCurve = GameManager_script.Instance().PlaceHolder0;
                    SequenceTimeLimit = Random.Range(60.0f, 120.0f);
                }

                if (SBG.LocalGestureType == GestureType.DoNothingShort)
                {
                    // take some timez
                    SequenceCurrentCurve = GameManager_script.Instance().PlaceHolder0;
                    SequenceTimeLimit = Random.Range(0.25f, 0.50f);
                }

                if (SBG.LocalGestureType == GestureType.HitTheBall)
                {
                    // generic quick fire, just like another do nothing
                    SequenceCurrentCurve = GameManager_script.Instance().PlaceHolder0;
                    SequenceTimeLimit = Random.Range(0.00f, 0.25f);
                }

                // starting at 0 as always
                SequenceCursor = 0.0f;

                // if we already got something good, we can stop right here
                if (shotCount != 0 && InitSequenceBoolGate && (SBG.LocalGestureType == GestureType.RotateTowardsDestination || SBG.LocalGestureType == GestureType.MoveCueBallToDestination))
                {
                    bool stopWhenNecessary = false;

                    // if we found ANY straight pockets, we stop
                    if (BestBotShootPosition != null && BestBotShootPosition.ObjectLevel <= RobotShotLevelType.StraightPocket && SBG.IntendedLevel == RobotShotLevelType.StraightPocket)
                    {
                        stopWhenNecessary = true;
                    }

                    // if we found ANY very good chain shot, we stop
                    if (BestBotShootPosition != null && BestBotShootPosition.ObjectLevel <= RobotShotLevelType.SecondaryPocket && SBG.IntendedLevel == RobotShotLevelType.SecondaryPocket)
                    {
                        stopWhenNecessary = true;
                    }

                    // if we are only looking for a good hit in the intended level, ANY (previous) good hit will do
                    if (BestBotShootPosition != null && BestBotShootPosition.ObjectLevel <= RobotShotLevelType.GoodHit && SBG.IntendedLevel == RobotShotLevelType.GoodHit)
                    {
                        stopWhenNecessary = true;
                    }

                    if (stopWhenNecessary)
                    {
                        DetermineStartOfFinalSequence();

                        SequenceCurrentCurve = null;
                        SequenceCurrentCurveX = null;
                        SequenceCurrentCurveY = null;

                        SequenceTimeLimit = -1;
                        SequenceCursor = -1;

                        return; // dajiang hack, this is kinda hacky
                    }
                }
            }

            // increment the cursor (there are a couple of cases for this dude)
            if (SBG.LocalGestureType == GestureType.RotateTowardsDestination && collisionSphereRed.GetComponent<Renderer>().enabled)
            {
                SequenceCursor += (2.0f * Time.deltaTime); // normal update every frame
            }
            else
            {
                SequenceCursor += Time.deltaTime; // normal update every frame
            }

            if (SBG.LocalGestureType == GestureType.MoveCueBallToDestination)
            {
                if (CueControllerUpdater.current_control_status == CueControllerUpdater.CUE_BALL_MOVABLE_ON_TABLE)
                {
                    OnPickUpCueBall(cueBallController.transform.position);
                }

                if (CueControllerUpdater.current_control_status == CueControllerUpdater.CUE_BALL_IN_HAND)
                {
                    // figure out x and x' and y'
                    float xAxis = Mathf.Clamp01(SequenceCursor / SequenceTimeLimit); // x limit is always 0 to 1
                    float xPrime = SequenceCurrentCurveX.Evaluate(xAxis);
                    float zPrime = SequenceCurrentCurveY.Evaluate(xAxis);

                    float xPosition = SBG.InitPosition.x + xPrime * (SBG.DestPosition.x - SBG.InitPosition.x);
                    float zPosition = SBG.InitPosition.z + zPrime * (SBG.DestPosition.z - SBG.InitPosition.z);

                    float yCoord = GameManager_script.RobotHolePositions[1][1];

                    Vector3 firstPassPosition = new Vector3(xPosition, yCoord, zPosition);
                    Vector3 secondPassPosition = Vector3.zero;
                    Transform StartOrMoveCube = shotCount == 0 ? StartCube : MoveCube;

                    VectorOperator.MoveBallInQuad(StartOrMoveCube, ballRadius, firstPassPosition, ref secondPassPosition);

                    cueBallController.transform.position = new Vector3(secondPassPosition.x, yCoord, secondPassPosition.z) + 2.5f * ballRadius * Vector3.up;
                }

                KeyGestureBoolGate = false;
            }

            if (SBG.LocalGestureType == GestureType.RotateTowardsDestination || SBG.LocalGestureType == GestureType.RotateTowardsDestinationShort)
            {
                // figure out x and y
                float xAxis = Mathf.Clamp01(SequenceCursor / SequenceTimeLimit); // x limit is always 0 to 1
                float yAxis = SequenceCurrentCurve.Evaluate(xAxis); // y limit is well, 0 to 1, so whatevern this is, it is ...

                // figure out cue displacement
                cuePivot.forward = (SBG.InitDirection + yAxis * (SBG.DestDirection - SBG.InitDirection)).normalized;

                // see if we want to update
                if (SBG.IntendedLevel == RobotShotLevelType.NeverStop)
                {
                    KeyGestureBoolGate = false;
                }
                else
                {
                    KeyGestureBoolGate = true;
                }
            }

            if (SBG.LocalGestureType == GestureType.GiveLocalBallSpin)
            {
                // figure out x and x' and y'
                float xAxis = Mathf.Clamp01(SequenceCursor / SequenceTimeLimit); // x limit is always 0 to 1
                float xPrime = SequenceCurrentCurveX.Evaluate(xAxis);
                float yPrime = SequenceCurrentCurveY.Evaluate(xAxis);

                float xPosition = Mathf.Clamp(SBG.InitSpin.x + xPrime * (SBG.DestSpin.x - SBG.InitSpin.x), -1.0f, 1.0f);
                float yPosition = Mathf.Clamp(SBG.InitSpin.y + yPrime * (SBG.DestSpin.y - SBG.InitSpin.y), -1.0f, 1.0f);

                cueBallPivot = new Vector3(xPosition, yPosition, 0.0f);

                KeyGestureBoolGate = false;
            }

            if (SBG.LocalGestureType == GestureType.CuePump)
            {
                // figure out x and y
                float xAxis = Mathf.Clamp01(SequenceCursor / SequenceTimeLimit); // x limit is always 0 to 1
                float yAxis = SequenceCurrentCurve.Evaluate(xAxis); // y limit is well, 0 to 1, so whatevern this is, it is ...

                // figure out cue displacement
                cueDisplacement = Mathf.Clamp(SBG.InitStrength + yAxis * (SBG.DestStrength - SBG.InitStrength), 0.0f, cueMaxDisplacement);

                KeyGestureBoolGate = false;
            }

            if (SBG.LocalGestureType == GestureType.DoNothing || SBG.LocalGestureType == GestureType.DoNothingShort)
            {
                KeyGestureBoolGate = false;
            }

            if (SBG.LocalGestureType == GestureType.HitTheBall)
            {
                // time to just hit the ball and it will reset at the end of this frame
                if (SequenceCursor > SequenceTimeLimit)
                {
                    OnShootCue();
                }

                KeyGestureBoolGate = false;
            }

            // work with cursor
            if (SequenceCursor > SequenceTimeLimit)
            {
                // special case for move cue ball
                if (SBG.LocalGestureType == GestureType.MoveCueBallToDestination)
                {
                    OnDropOffCueBall();
                }

                // we have completed this gesture and it is time to move on to the next
                SequenceCurrentCurve = null;
                SequenceCurrentCurveX = null;
                SequenceCurrentCurveY = null;

                SequenceTimeLimit = -1;
                SequenceCursor = -1;

                SequenceIndex += 1;
            }
        }

        // stop initial sequence and start final sequence
        if (InitSequenceBoolGate && SequenceIndex >= InitialGestureSequence.Count)
        {
            DetermineStartOfFinalSequence();
        }

        // stop final sequence
        if (FinalSequenceBoolGate && SequenceIndex >= FinalGestureSequence.Count)
        {
            FinalSequenceBoolGate = false;
        }
    }

    // time to finish up the init sequence and do final
    public void DetermineStartOfFinalSequence()
    {
        // we are done with the initial sequence, time to move onto the next phase
        DetermineFinalSequenceOfGestures();

        InitSequenceBoolGate = false;
        FinalSequenceBoolGate = true;

        SequenceIndex = 0;
    }

    // work with the current position and direction, SBIPositions and SBIAimDirections and come up with a sequence of gestures
    public void DetermineInitialSequenceOfGestures()
    {
        Vector3 initialPosition = cueBallController.GetComponent<Rigidbody>().position;
        Vector3 initialDirection = cuePivot.forward;

        if (shotCount == 0)
        {
            // 15% chance of moving it
            if (GameManager_script.DetermineLotteryResult(0.15f))
            {
                // just do nothing
                AddDoNothingToInitialSequence();

                // just do nothing again
                AddDoNothingToInitialSequence();

                // consts
                float randomX = Random.Range(StartCube.localScale.x * -0.75f, 0.0f);
                float randomZ = Random.Range(StartCube.localScale.z * -0.40f, StartCube.localScale.z * 0.40f);
                Vector3 randomVector = new Vector3(Random.Range(-0.20f, 0.20f) * ballRadius, 0.0f, Random.Range(-0.20f, 0.20f) * ballRadius);

                // random final position?
                Vector3 randomPosition = mainBallPoint.position + new Vector3(randomX, 0.0f, randomZ);
                Vector3 randomDestination = (firstBallPoint.position + randomVector - randomPosition).normalized;

                // position moved, time to move it
                SmartBotGesture SBG_move = new SmartBotGesture();
                SBG_move.LocalGestureType = GestureType.MoveCueBallToDestination;
                SBG_move.DestPosition = randomPosition;
                SBG_move.IntendedLevel = RobotShotLevelType.StraightPocket;
                InitialGestureSequence.Add(SBG_move);

                // just do nothing
                AddDoNothingToInitialSequence();

                // swipe over and aim at the ball
                SmartBotGesture SBG_swipe = new SmartBotGesture();
                SBG_swipe.LocalGestureType = GestureType.RotateTowardsDestination;
                SBG_swipe.DestDirection = randomDestination;
                SBG_swipe.IntendedLevel = RobotShotLevelType.StraightPocket;
                InitialGestureSequence.Add(SBG_swipe);
            }
        }
        else
        {
            for (int i = 0; i < SBIPositions.Count; i++)
            {
                if (i == 0)
                {
                    if (GameManager_script.DetermineLotteryResult(0.008f))
                    {
                        // just do nothing for real
                        AddDoNothingCompletelyToInitialSequence();
                    }
                    else
                    {
                        // just do nothing
                        AddDoNothingToInitialSequence();

                        // just do nothing again
                        AddDoNothingToInitialSequence();
                    }

                    // 3.5% chance of doing a random big swipe and back, needs to be completed in 2 steps
                    if (GameManager_script.DetermineLotteryResult(0.035f))
                    {
                        // fake rotate
                        SmartBotGesture SBG1 = new SmartBotGesture();
                        SBG1.LocalGestureType = GestureType.RotateTowardsDestinationShort;
                        SBG1.DestDirection = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
                        SBG1.IntendedLevel = RobotShotLevelType.NeverStop;
                        InitialGestureSequence.Add(SBG1);

                        // do nothing for a change?
                        if (GameManager_script.DetermineLotteryResult(0.50f))
                        {
                            AddDoNothingToInitialSequence();
                        }

                        // fake rotate back
                        SmartBotGesture SBG2 = new SmartBotGesture();
                        SBG2.LocalGestureType = GestureType.RotateTowardsDestinationShort;
                        SBG2.DestDirection = cuePivot.forward;
                        SBG2.IntendedLevel = RobotShotLevelType.NeverStop;
                        InitialGestureSequence.Add(SBG2);

                        // do nothing for a change?
                        if (GameManager_script.DetermineLotteryResult(0.10f))
                        {
                            AddDoNothingToInitialSequence();
                        }
                    }
                }
                else
                {
                    if (GameManager_script.DetermineLotteryResult(0.500f))
                    {
                        // just do nothing
                        AddDoNothingToInitialSequence();
                    }

                    if (GameManager_script.DetermineLotteryResult(0.500f))
                    {
                        // just do nothing, again
                        AddDoNothingToInitialSequence();
                    }
                }

                // 2.5% chance of doing some weird pump
                if (GameManager_script.DetermineLotteryResult(0.025f))
                {
                    // fake pump, first and last strength same
                    SmartBotGesture SBG = new SmartBotGesture();
                    SBG.LocalGestureType = GestureType.CuePump;
                    SBG.DestStrength = Random.Range(0.5f * cueMaxDisplacement, 1.0f * cueMaxDisplacement);
                    InitialGestureSequence.Add(SBG);

                    // fake pump, second part
                    SmartBotGesture SBG1 = new SmartBotGesture();
                    SBG1.LocalGestureType = GestureType.CuePump;
                    SBG1.DestStrength = 0.0f; // should be back to 0.0f
                    InitialGestureSequence.Add(SBG1);

                    // do nothing for a change?
                    if (GameManager_script.DetermineLotteryResult(0.10f))
                    {
                        AddDoNothingToInitialSequence();
                    }
                }
                
                // 2.5% chance of doing some weird shadow spin
                if (GameManager_script.DetermineLotteryResult(0.025f))
                {
                    // a 100% longer pause
                    AddDoNothingToInitialSequence();

                    // fake spin, first is first, last is some random
                    SmartBotGesture SBG1 = new SmartBotGesture();
                    SBG1.LocalGestureType = GestureType.GiveLocalBallSpin;
                    SBG1.DestSpin = new Vector3(Random.Range(-0.90f, 0.90f), Random.Range(-0.90f, 0.90f), 0.0f);
                    InitialGestureSequence.Add(SBG1);

                    // a 100% longer pause
                    AddDoNothingToInitialSequence();
                }

                // if next dude's position is different, move over
                if ((i == 0 && !VectorOperator.DetermineTrueIfSameVectorThree(initialPosition, SBIPositions[i])) || (i != 0 && !VectorOperator.DetermineTrueIfSameVectorThree(SBIPositions[i - 1], SBIPositions[i])))
                {
                    // a long pause and a short pause
                    AddDoNothingToInitialSequence();
                    AddDoNothingShortToInitialSequence();

                    // position moved, time to move it
                    SmartBotGesture SBG = new SmartBotGesture();
                    SBG.LocalGestureType = GestureType.MoveCueBallToDestination;
                    SBG.DestPosition = SBIPositions[i];
                    SBG.IntendedLevel = SBIScores[i] > 0.0f ? RobotShotLevelType.StraightPocket : RobotShotLevelType.GoodHit;
                    InitialGestureSequence.Add(SBG);

                    // do nothing short
                    AddDoNothingShortToInitialSequence();

                    // put a pause between move and rotate
                    if (GameManager_script.DetermineLotteryResult(0.1f))
                    {
                        // just do nothing, the very first of first has to be a pause
                        AddDoNothingToInitialSequence();
                    }
                }

                // if next dude's direction is different, swipe over
                if (true)
                {
                    // rotate them bastards
                    SmartBotGesture SBG = new SmartBotGesture();
                    SBG.LocalGestureType = GestureType.RotateTowardsDestination;
                    SBG.DestDirection = SBIAimDirections[i];
                    SBG.IntendedLevel = SBIScores[i] > 0.0f ? RobotShotLevelType.StraightPocket : RobotShotLevelType.GoodHit;
                    InitialGestureSequence.Add(SBG);

                    // do nothing for a change?
                    if (GameManager_script.DetermineLotteryResult(0.10f))
                    {
                        AddDoNothingToInitialSequence();
                    }
                }
            }
        }
    }

    public void DetermineFinalSequenceOfGestures()
    {
        Vector3 initialPosition = cueBallController.GetComponent<Rigidbody>().position;
        Vector3 initialDirection = cuePivot.forward;

        bool pushoutAccepted = false;

        if (BestBotShootPosition != null && shotCount == 2 && skipAllowed && (BestBotShootPosition.ObjectLevel != RobotShotLevelType.StraightPocket && BestBotShootPosition.ObjectLevel != RobotShotLevelType.SecondaryPocket))
        {
            // skip
            ShootOptionAccepted();

            return;
        }

        if (BestBotShootPosition != null && shotCount == 1 && pushoutAllowed && (BestBotShootPosition.ObjectLevel != RobotShotLevelType.StraightPocket && BestBotShootPosition.ObjectLevel != RobotShotLevelType.SecondaryPocket))
        {
            // pushout
            PushOutAccepted();

            pushoutAccepted = true;
        }

        if (BestBotShootPosition == null && shotCount == 0) // best shot position is most likely not even available, so we just use first aim and good strength (special case for the first break ONLY)
        {
            // construct BestBotShootPosition, we need position, direction, spin and strength
            BestBotShootPosition = new PossibleShootPlacement();

            BestBotShootPosition.cueBallPosition = cueBallController.GetComponent<Rigidbody>().position; // just current position
            BestBotShootPosition.cuePivotForwardRotation = cuePivot.forward; // just current swipe
            BestBotShootPosition.cueRotationLocalPosition = GameManager_script.DetermineLotteryResult(0.50f) ? Vector3.zero : new Vector3(Random.Range(-0.10f, 0.10f), Random.Range(-0.50f, 0.25f), 0.0f); // break shot standard swipe
            BestBotShootPosition.cueBallStrength = Random.Range(0.95f, 1.0f) * cueMaxDisplacement;
        }

        if (BestBotShootPosition != null)
        {
            // MOVE CUE BALL
            if (!VectorOperator.DetermineTrueIfSameVectorThree(initialPosition, BestBotShootPosition.cueBallPosition))
            {
                // a very short pause and a long pause
                AddDoNothingToFinalSequence();
                AddDoNothingShortToFinalSequence();

                SmartBotGesture SBG = new SmartBotGesture();
                SBG.LocalGestureType = GestureType.MoveCueBallToDestination;
                SBG.DestPosition = BestBotShootPosition.cueBallPosition;
                FinalGestureSequence.Add(SBG);

                // after move a very short pause
                AddDoNothingShortToFinalSequence();

                // do nothing for a change?
                if (GameManager_script.DetermineLotteryResult(0.10f))
                {
                    AddDoNothingToFinalSequence();
                }
            }

            // SWIPE ROTATION
            if (pushoutAccepted && GameManager_script.DetermineLotteryResult(0.75f))
            {
                // do nothing for a change?
                AddDoNothingToFinalSequence();

                // random rotate
                SmartBotGesture SBG = new SmartBotGesture();
                SBG.LocalGestureType = GestureType.RotateTowardsDestination;
                SBG.DestDirection = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
                FinalGestureSequence.Add(SBG);

                // do nothing for a change?
                if (GameManager_script.DetermineLotteryResult(0.10f))
                {
                    AddDoNothingToFinalSequence();
                }
            }
            else
            {
                SmartBotGesture SBG = new SmartBotGesture();

                if (Vector3.Dot(initialDirection.normalized, BestBotShootPosition.cuePivotForwardRotation.normalized) > SeventyFiveDegree)
                {
                    SBG.LocalGestureType = GestureType.RotateTowardsDestinationShort;
                    SBG.DestDirection = BestBotShootPosition.cuePivotForwardRotation;
                    FinalGestureSequence.Add(SBG);
                }
                else
                {
                    SBG.LocalGestureType = GestureType.RotateTowardsDestination;
                    SBG.DestDirection = BestBotShootPosition.cuePivotForwardRotation;
                    FinalGestureSequence.Add(SBG);
                }
                
                // do nothing for a change?
                if (GameManager_script.DetermineLotteryResult(0.10f))
                {
                    AddDoNothingToFinalSequence();
                }
            }

            // SHADOW SPIN
            bool FinalSpinPermitted = false;

            if (currentBallControllers.Count > 1)
            {
                Vector3 aimAngle = (currentBallControllers[1].GetComponent<Rigidbody>().position - cueBallController.GetComponent<Rigidbody>().position).normalized;
                Vector3 currentAngle = (currentBallControllers[1].GetComponent<Rigidbody>().position - currentCollision.position).normalized;

                float dotBetweenMidAndCurrent = Vector3.Dot(aimAngle, currentAngle);

                if (shotCount == 0)
                {
                    if (GameManager_script.DetermineLotteryResult(0.30f))
                    {
                        FinalSpinPermitted = true;
                    }
                    else
                    {
                        FinalSpinPermitted = false;
                    }
                }
                else
                {
                    if (dotBetweenMidAndCurrent > EightyFiveDegree && GameManager_script.DetermineLotteryResult(0.99f))
                    {
                        FinalSpinPermitted = true;
                    }
                    else if (dotBetweenMidAndCurrent > EightyDegree && GameManager_script.DetermineLotteryResult(0.66f))
                    {
                        FinalSpinPermitted = true;
                    }
                    else if (GameManager_script.DetermineLotteryResult(0.33f))
                    {
                        FinalSpinPermitted = true;
                    }
                    else
                    {
                        FinalSpinPermitted = false;
                    }
                }
            }

            if (FinalSpinPermitted)
            {
                // a 100% longer pause
                AddDoNothingToFinalSequence();

                SmartBotGesture SBG = new SmartBotGesture();
                SBG.LocalGestureType = GestureType.GiveLocalBallSpin;
                SBG.DestSpin = BestBotShootPosition.cueRotationLocalPosition;
                FinalGestureSequence.Add(SBG);

                // a 100% longer pause
                AddDoNothingToFinalSequence();
            }

            // DRAW CUE
            if (true)
            {
                SmartBotGesture SBG = new SmartBotGesture();
                SBG.LocalGestureType = GestureType.CuePump;
                SBG.DestStrength = BestBotShootPosition.cueBallStrength;
                FinalGestureSequence.Add(SBG);

                // do nothing for a change?
                if (GameManager_script.DetermineLotteryResult(0.10f))
                {
                    AddDoNothingToFinalSequence();
                }
            }

            // SHOOT
            if (true)
            {
                SmartBotGesture SBG = new SmartBotGesture();
                SBG.LocalGestureType = GestureType.HitTheBall;
                FinalGestureSequence.Add(SBG);
            }
        }
    }

    public void AddDoNothingCompletelyToInitialSequence()
    {
        SmartBotGesture SBG = new SmartBotGesture();

        SBG.LocalGestureType = GestureType.DoNothingCompletely;

        InitialGestureSequence.Add(SBG);
    }

    public void AddDoNothingToInitialSequence()
    {
        SmartBotGesture SBG = new SmartBotGesture();

        SBG.LocalGestureType = GestureType.DoNothing;

        InitialGestureSequence.Add(SBG);
    }

    public void AddDoNothingShortToInitialSequence()
    {
        SmartBotGesture SBG = new SmartBotGesture();

        SBG.LocalGestureType = GestureType.DoNothingShort;

        InitialGestureSequence.Add(SBG);
    }

    public void AddDoNothingToFinalSequence()
    {
        SmartBotGesture SBG = new SmartBotGesture();

        SBG.LocalGestureType = GestureType.DoNothing;

        FinalGestureSequence.Add(SBG);
    }

    public void AddDoNothingShortToFinalSequence()
    {
        SmartBotGesture SBG = new SmartBotGesture();

        SBG.LocalGestureType = GestureType.DoNothingShort;

        FinalGestureSequence.Add(SBG);
    }

    // smart bot logic ends here
}
