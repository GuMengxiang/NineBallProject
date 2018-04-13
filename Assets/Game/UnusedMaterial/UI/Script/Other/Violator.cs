using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Violator : MonoBehaviour
{
    public UISprite Parent;
    public GameObject ViolatorPrefab;
    public ViolatorType vViolatorType;
    private GameObject Group;
    public float localPositionX;
    public float localPositionY;
    public int ButtonSize = 30;

    public void ShowUIViolator(int VNewsNumber)
    {
        if (Group != null)
        {
            Destroy(Group);
        }

        Group = (GameObject)Instantiate(ViolatorPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);
        Group.transform.parent = gameObject.transform;
        Group.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // define violator position and size based on different types of buttons it is attached to
        if (vViolatorType == ViolatorType.MainMenuCenter)
        {
            Group.transform.localPosition = new Vector3(Parent.width * 0.92f / 2.0f, Parent.height * 0.90f / 2.0f, 0);
            Group.GetComponent<UIViolator>().Backgorundwidth = (int)(Parent.width / 7.0f);
        }
        else if (vViolatorType == ViolatorType.MainMenuSetting)
        {
            Group.transform.localPosition = new Vector3(Parent.width, Parent.height, 0); //Vector3.zero;
            Group.GetComponent<UIViolator>().Backgorundwidth = (int)(Parent.height / 2.0f);
        }
        else if (vViolatorType == ViolatorType.MainMenuProfileShop)
        {
            Group.transform.localPosition = new Vector3(0, Parent.height, 0);  //Vector3.zero;
            Group.GetComponent<UIViolator>().Backgorundwidth = (int)(Parent.height / 2.0f);
        }

        // define violator count
        Group.GetComponent<UIViolator>().NewsNumber = VNewsNumber;
    }

    public void HideUIViolator()
    {
        if (Group != null)
        {
            Destroy(Group);
        }
    }
}
