using UnityEngine;
using System.Collections;

public class SelectMouse : MonoBehaviour
{
	void Update ()
    {
        float Lerp = Mathf.PingPong(Time.time, 1) / 1;
        gameObject.GetComponent<Renderer>().material.color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), Lerp);
	}
}
