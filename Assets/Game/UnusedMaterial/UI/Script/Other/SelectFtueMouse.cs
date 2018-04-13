using UnityEngine;
using System.Collections;

public class SelectFtueMouse : MonoBehaviour
{
    public AnimationCurve Curvez;

    // dajiang hack, all the numbers in here are so hacky... I mean it really sucks
    void Start()
    {
        // solid color
        gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);

        // bigger
        gameObject.transform.localScale = new Vector3(1.1f, 0.7f, 1.1f);
    }

    // dajiang hack, all the numbers in here are so hacky... I mean it really sucks
    void Update()
    {
        if (GameManager_script.Instance().SwipeFtueStage == 0)
        {
            float smallSwipeLerp = Mathf.Clamp(Mathf.PingPong(Time.time, 1.125f), 0.35f, 0.90f);

            float xValue = Curvez.Evaluate(smallSwipeLerp) * 3.0f;
            float zValue = smallSwipeLerp * 3.0f;

            gameObject.transform.position = new Vector3(xValue, 0.0f, zValue) + GameManager_script.Instance().ftueMouseCursorPositionHack;
        }
    }
}
