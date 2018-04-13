using UnityEngine;
using System.Collections;

public class GameReplaceTableTexture : MonoBehaviour
{
    public Material TableMaterials;
    public Texture[] TableTextures;
    
    private static GameReplaceTableTexture pInterface;

    public static GameReplaceTableTexture SingleTon()
    {
        return pInterface;
    }

    void Awake()
    {
        pInterface = this;
    }

	void Start ()
    {
        SetTableTexture(GameManager_script.Instance().TableTextureIndex);
	}

    public void SetTableTexture(int index)
    {
        if (index < TableTextures.Length && index >= 0)
        {
            TableMaterials.mainTexture = TableTextures[index];
        }
        else
        {
            TableMaterials.mainTexture = TableTextures[0];
        }
    }
}
