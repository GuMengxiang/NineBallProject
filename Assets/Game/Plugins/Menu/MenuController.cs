using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour 
{
	public delegate void LoadLevelHandler(MenuController menuController);
	public event LoadLevelHandler OnLoadLevel;
    public Preloader preloader;

    [SerializeField]
    private GameObject masterServerGUIPrefab;
    public GameObject serverMasterController;

	public Camera loaderCamera;
	public Camera guiCamera;
    public GameObject layerHelp;

	[System.NonSerialized]
	public string levelName = "";
	[System.NonSerialized]
	public int levelNumber = -1;
	[System.NonSerialized]
	public bool LoaderIsDoneUnload = false;

	public bool isTouchScreen = false;

	void Awake()
	{
		preloader.controller = this;
	}

	public void OnStart()
	{
		serverMasterController = GameObject.Instantiate(masterServerGUIPrefab) as GameObject;
		serverMasterController.transform.parent = transform;
	}

	void OnEnable ()
	{

	}

    public void LoadLevel (string levelName)
	{
		this.levelName = levelName;
		this.levelNumber = -1;
		StartCoroutine( Load () );
	}

	public void LoadLevel (int levelNumber)
	{
		this.levelName = "";
		this.levelNumber = levelNumber;
		StartCoroutine( Load () );
	}

	IEnumerator Load ()
	{
		preloader.SetState(true);

		if(OnLoadLevel != null)
        {
            OnLoadLevel(this);
        }

		if(loaderCamera)
        {
            loaderCamera.enabled = true;
        }
		
		if(guiCamera)
        {
            guiCamera.enabled = false;
        }
		
		LoaderIsDoneUnload = false;
        preloader.UpdateLoader(0.0f);

		Application.LoadLevel("Loader"); // this is loading the loader

		yield return StartCoroutine(UpdateLoader());
		yield return new WaitForEndOfFrame();
		
		preloader.UpdateLoader(1.0f);
			
		yield return null;
	}

	IEnumerator UpdateLoader ()	
	{
		if(levelName != "")
		{
			while(levelName != Application.loadedLevelName)
			{
				if(LoaderIsDoneUnload)
				{
					preloader.UpdateLoader( 0.8f );
				}
				yield return null;
			}
		}
		else
		{
			while(levelNumber != Application.loadedLevel)
			{
				if(LoaderIsDoneUnload)
				{
					preloader.UpdateLoader( 0.8f );
				}
					yield return null;
			}
		}

		if(Application.loadedLevel != 0)
		{
			if(guiCamera)
            {
                guiCamera.enabled = true;
            }
			
			if(loaderCamera)
            {
                loaderCamera.enabled = false;
            }
			
			preloader.SetState(false);
		}
	}
}
