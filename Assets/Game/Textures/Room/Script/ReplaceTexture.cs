using UnityEngine;
using System.Collections;

public class ReplaceTexture : MonoBehaviour
{
    public Material[] TableClothMaterials;
    public Material[] TableCornerMaterials;
    public Material[] TableWallMaterials;
    public Material[] TableBallArmMaterials;
    public Texture[] TableClothTextures;
    public Texture[] TableCornerTextures;
    public Texture[] TableWallTextures;
    public Texture[] TableBallArmTextures;

    private float changeInterval = 0.33f;
    private static ReplaceTexture pInterface;

    public static ReplaceTexture SingleTon()
    {
        return pInterface;
    }

    void Awake()
    {
        pInterface = this;
    }

    //设置桌布（所有绿色）//
    public void SetTableCloth(int index)
    {
        if (index < TableClothTextures.Length)
        {
        TableClothMaterials[0].mainTexture = TableClothTextures[index];
        }
        else 
        { 
            TableClothMaterials[0].mainTexture = TableClothTextures[0]; 
        }
    }

    //设置桌角//
    public void SetTableCorner(int index)
    {
        if (index < TableCornerTextures.Length)
        {
            TableCornerMaterials[0].mainTexture = TableCornerTextures[index];
        }
        else 
        { 
            TableCornerMaterials[0].mainTexture = TableCornerTextures[0]; 
        }
    }

    //设置桌边，墙//
    public void SetTableWall(int index)
    {
        if (index < TableWallTextures.Length)
        {
            TableWallMaterials[0].mainTexture = TableWallTextures[index];
        }
        else 
        { 
            TableWallMaterials[0].mainTexture = TableWallTextures[0]; 
        }
    }

    //设置球杆//
    public void SetTableBallArm(int index)
    {
        int count = TableBallArmTextures.Length / 5;
        print("count " + count);
        /* TableBallArmMaterials[0].mainTexture = TableBallArmTextures[5  * index + 0];
        TableBallArmMaterials[1].mainTexture = TableBallArmTextures[5  * index + 1];
        TableBallArmMaterials[2].mainTexture = TableBallArmTextures[5  * index + 2];
        TableBallArmMaterials[3].mainTexture = TableBallArmTextures[5  * index + 3];
        TableBallArmMaterials[4].mainTexture = TableBallArmTextures[5  * index + 4];*/

        if (index < TableBallArmTextures.Length)
        {
            TableBallArmMaterials[0].mainTexture = TableBallArmTextures[index];
        }
        else
        {
            TableBallArmMaterials[0].mainTexture = TableBallArmTextures[0];
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 50), "button"))
        {
            int  index  = (int)(Time.time / changeInterval);
            index = index % TableClothTextures.Length;
            TableClothMaterials[0].mainTexture= TableClothTextures[index];
            TableCornerMaterials[0].mainTexture = TableClothTextures[index];
            TableWallMaterials[0].mainTexture = TableClothTextures[index];
            index = Random.Range(0,4);
            print("index " + index);
            ReplaceTexture.SingleTon().SetTableBallArm(index);
        }
    }
}
