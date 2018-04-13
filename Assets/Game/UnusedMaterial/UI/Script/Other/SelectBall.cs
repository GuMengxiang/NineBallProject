using UnityEngine;
using System.Collections;

public class SelectBall : MonoBehaviour
{

    public Material PanelMaterial;
    public bool ChangeBool=false;
    private Vector3 color;
    public GameObject select;
    public GameObject Group;

	void Update ()
    {
        if (ChangeBool && Group != null)
        { 
            float Lerp = Mathf.PingPong(Time.time, 1) / 1;
            Group.GetComponent<Renderer>().material.color = Color.Lerp(new Color(color.x, color.y, color.z, 0), new Color(color.x, color.y, color.z, 1), Lerp);
        }
	}

    public void ChangeSelect(bool Bool)
    {
        Destroy(Group);

        Group = null;

        if (Bool)
        {
            if (Group == null)
            {
                Group = (GameObject)Instantiate(select, (Vector3)gameObject.transform.position, (Quaternion)gameObject.transform.rotation);
                Group.transform.parent = gameObject.transform;
                Group.transform.localPosition = Vector3.zero;
                Group.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }

            color = new Vector3(1, 1, 1);
        }
        
        ChangeBool = Bool;
    }
}
