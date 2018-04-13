// Figure out if this really improves performance
// Most likely it doesn't. Then we can spend time and kill this class.

using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour 
{
	private AsyncOperation unloadUnusedAssets;

	IEnumerator Start ()
	{
		if(MenuControllerGenerator.controller)
		{
			unloadUnusedAssets = Resources.UnloadUnusedAssets();
			StartCoroutine("UpdateLoader");
			yield return unloadUnusedAssets;

			StopCoroutine("UpdateLoader");
			MenuControllerGenerator.controller.LoaderIsDoneUnload = true;
			yield return new WaitForEndOfFrame();

			if(MenuControllerGenerator.controller.levelName != "")
            {
                Debug.Log("loading level " + MenuControllerGenerator.controller.levelName);
                Application.LoadLevel(MenuControllerGenerator.controller.levelName);
            }
			else
            {
                Debug.Log("loading level " + MenuControllerGenerator.controller.levelNumber);
                Application.LoadLevel(MenuControllerGenerator.controller.levelNumber);
            }
		}
		else
		{
			yield return null;
		}
	}
	
	IEnumerator UpdateLoader ()	
	{
		if(MenuControllerGenerator.controller.levelName != "")
		{
			while(true)
			{
				print("unloadUnusedAssets.progress : " + unloadUnusedAssets.progress);
                MenuControllerGenerator.controller.preloader.UpdateLoader(0.2f * unloadUnusedAssets.progress);
				yield return null;
			}
		}
		else
		{
			while(true)
			{
                MenuControllerGenerator.controller.preloader.UpdateLoader(0.2f * unloadUnusedAssets.progress);
				yield return null;
			}
		}
	}
}
