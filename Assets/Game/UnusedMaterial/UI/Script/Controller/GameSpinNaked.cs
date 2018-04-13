using UnityEngine;
using System.Collections;

public class GameSpinNaked : MonoBehaviour
{
    public UIPanel RootPanel;
    
    public GameObject SpinNaked;
    public GameObject SpinShadow;
    public GameObject CueControl;
    public UISprite SpinNakedBackground;
    public UISprite Centerpoint;

    public float CenterpointMovementX;
    public float CenterpointMovementY;

    public float Radius;
    public Vector2 Center;
    public bool EverAffectedCoordinates = false;

    public CueController cc;
    public Camera CueCamera;

	// Use this for initialization
	void Start ()
    {
        Center.x = RootPanel.width * 0.5f;
        Center.y = RootPanel.height * 0.5f;
        Radius = RootPanel.height * 0.5f / 1.33f;

        if (CueControl && CueControl.GetComponent<CueController>())
        {
            cc = CueControl.GetComponent<CueController>();
        }

        SpinNakedUI();

        if (GameObject.Find("Camera2D/CueCamera"))
        {
            CueCamera = GameObject.Find("Camera2D/CueCamera").GetComponent<Camera>();
        }
	}
	
    void SpinNakedUI()
    {
        //SpinNaked按钮
        SpinNakedBackground.width = (int)(RootPanel.height);
        SpinNakedBackground.height = (int)(RootPanel.height);

        SpinNaked.GetComponent<BoxCollider>().size = new Vector3(RootPanel.height, RootPanel.height, 0);
        SpinNaked.transform.localPosition = Vector3.zero;

        Centerpoint.width = (int)(RootPanel.height/8.31f);
        Centerpoint.height = (int)(RootPanel.height/8.31f);
        Centerpoint.transform.localPosition = Vector3.zero;
        Centerpoint.GetComponent<GameSpinOnDrag>().UI();
        Centerpoint.GetComponent<GameSpinOnDrag>().cc = cc;
        Centerpoint.GetComponent<BoxCollider>().size = new Vector3(Centerpoint.width, Centerpoint.height, 0.0f);
	}

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            GameManager_script.Instance().DownOnRealButtons = true;

            CenterpointMovementX = Input.mousePosition.x - RootPanel.width * 0.5f;
            CenterpointMovementY = Input.mousePosition.y - RootPanel.height * 0.5f;

            if (new Vector2(CenterpointMovementX, CenterpointMovementY).magnitude / Radius > 1)
            {
                EverAffectedCoordinates = false;
            }
            else
            {
                EverAffectedCoordinates = true;

                OnDrag();
            }
        }
        else
        {
            GameManager_script.Instance().DownOnRealButtons = false;

            if (!EverAffectedCoordinates)
            {
                // exit this mode
                SpinNaked.gameObject.SetActive(false);

                // CloseCueCamera
                if (CueCamera)
                {
                  CueCamera.depth = 5;
                }
            }
            else
            {
                ConvertFromRectToCircle();

                Centerpoint.gameObject.transform.localPosition = new Vector3(CenterpointMovementX, CenterpointMovementY, 0);

                SetCueControllerValue();

                ChangeSpinShadow(CenterpointMovementX / Radius, CenterpointMovementY / Radius);
            }
        }
    }

    void OnDrag()
    {
        if (EverAffectedCoordinates)
        {
            CenterpointMovementX = Input.mousePosition.x - RootPanel.width * 0.5f;
            CenterpointMovementY = Input.mousePosition.y - RootPanel.height * 0.5f;

            ConvertFromRectToCircle();

            Centerpoint.gameObject.transform.localPosition = new Vector3(CenterpointMovementX, CenterpointMovementY, 0);

            SetCueControllerValue();

            ChangeSpinShadow(CenterpointMovementX / Radius, CenterpointMovementY / Radius);
        }
    }

    void ConvertFromRectToCircle()
    {
        float MultipleOfRadius = new Vector2(CenterpointMovementX, CenterpointMovementY).magnitude / Radius;

        if (MultipleOfRadius > 1.0f)
        {
            CenterpointMovementX = CenterpointMovementX / MultipleOfRadius;
            CenterpointMovementY = CenterpointMovementY / MultipleOfRadius;
        }
    }

    public void ChangeSpinShadow(float vCenterpointMovementX, float vCenterpointMovementY)
    {
        SpinShadow.GetComponent<GameSpinShadow>().ChangeSpinShadowCenterpoint(vCenterpointMovementX, vCenterpointMovementY);
    }

    void SetCueControllerValue()
    {
        if (cc)
        {
            cc.cueBallPivot = new Vector3(CenterpointMovementX / Radius, CenterpointMovementY / Radius, 0.0f);
        }
    }
}
