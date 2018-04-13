using UnityEngine;
using System.Collections;

public class MenuControllerGenerator : MonoBehaviour 
{
    [SerializeField]
    private MenuController controllerPrefab;
    
    public static MenuController controller;
	
	void Awake()
	{
		this.gameObject.AddComponent<Rigidbody>();

        if (controller)
        {
            Destroy(controller.serverMasterController); // we can explicitly destroy a "don't destroy" item any time.
            Destroy(controller.gameObject);
            Destroy(controller);

            controller = null;
        }

        if(controller == null) // start everything fresh
		{
            bool checkLeyers = CheckLayers();

			SetPhysics();

            Input.multiTouchEnabled = false;

			controller = MenuController.Instantiate(controllerPrefab) as MenuController;
			controller.name = "Controller";
			DontDestroyOnLoad(controller.gameObject);

			if(checkLeyers)
			{
				if(controller.layerHelp)
				Destroy(controller.layerHelp);
#if !UNITY_EDITOR
				controller.isTouchScreen = Application.platform != RuntimePlatform.WindowsWebPlayer && Application.platform != RuntimePlatform.OSXWebPlayer
					&& Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.OSXPlayer && Application.platform != RuntimePlatform.LinuxPlayer;
				if(controller.isTouchScreen)
				Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
                controller.OnStart();
			}
			else
			{
				Debug.LogError("Please add the layers \n " + "Ball, Canvas, Wall, MainBall, Graund, GUI \n" + "as shown in the image ");
			}
		}
	}

	bool CheckLayers ()
	{
		return LayerMask.NameToLayer("Ball") > 0 && LayerMask.NameToLayer("Canvas") > 0 && LayerMask.NameToLayer("Wall") > 0
			&& LayerMask.NameToLayer("MainBall") > 0 && LayerMask.NameToLayer("Graund") > 0 && LayerMask.NameToLayer("GUI") > 0;
	}

	void SetPhysics ()
	{
        // dajiang physics 3
        // I need to adjust these values 
		Physics.gravity = 50.0f * Vector3.down; // w.e
		Physics.bounceThreshold = 1.0f; // velocity below this will NOT bounce to reduce jitter
		Physics.sleepVelocity = 0.1f; // sleeps
        Physics.sleepAngularVelocity = 0.2f; // sleeps
		GetComponent<Rigidbody>().maxAngularVelocity=335.7f;
//        Physics.maxAngularVelocity = 335.7f; // this is pretty high too (right now 203.67f is max velocity, this is 235.00f * 5/7 * 2).
//        Physics.minPenetrationForPenalty = 0.001f; // eating into each other
		Physics.defaultContactOffset=0.001f;
		Physics.solverIterationCount = 7; // this is good enough, even 7 is good enough

        Time.fixedDeltaTime = 0.00190f; // really it is 525 frames per second

		for (int i = 0; i < 32; i++)
		{
			string layerName_i = LayerMask.LayerToName(i);

            for (int j = 0; j < 32; j++)
			{
				string layerName_j = LayerMask.LayerToName(j);

                if(layerName_i == "Ball")
				{
					if(layerName_j == "Graund" || layerName_j == "MainBall" || layerName_j == "Wall" || layerName_j == "Ball")
                    {
                        Physics.IgnoreLayerCollision(i, j, false);
                    }
					else
                    {
                        Physics.IgnoreLayerCollision(i, j, true);
                    }
				}
				else if(layerName_i == "Canvas")
				{
					Physics.IgnoreLayerCollision(i,j, true);
				}
				else if(layerName_i == "Wall")
				{
					if(layerName_j == "MainBall" || layerName_j == "Ball")
                    {
                        Physics.IgnoreLayerCollision(i, j, false);
                    }
					else
                    {
                        Physics.IgnoreLayerCollision(i, j, true);

                    }
				}
				else if(layerName_i == "MainBall")
				{
                    if (layerName_j == "Graund" || layerName_j == "MainBall" || layerName_j == "Wall" || layerName_j == "Ball")
                    {
                        Physics.IgnoreLayerCollision(i, j, false);
                    }
					else
                    {
                        Physics.IgnoreLayerCollision(i, j, true);
                    }
				}

				if(layerName_i == "Graund")
				{
					if(layerName_j == "MainBall" || layerName_j == "Ball")
                    {
                        Physics.IgnoreLayerCollision(i, j, false);
                    }
					else
                    {
                        Physics.IgnoreLayerCollision(i, j, true);
                    }
				}
				else if(layerName_i == "GUI")
				{
					Physics.IgnoreLayerCollision(i,j, true);
				}
			}
		}
	}
}
