using UnityEngine;
using System.Collections;

public class FTYE_Bubble : MonoBehaviour {

    public Vector2 BackgroundXY;
    public UIPanel RootPanel;
    public UILabel Title;
    public UISprite BoxBackground;
    public UISprite Background;
    private string BubbleText;
    private Vector3 BackgroundPosition;
    private Vector3 NeedlePosition;
    private Vector2 BackgroundSize;
    private Vector2 NeedleSize;

    

    public float PositionTitle = 3.14f;

    public float heightOkButton = 8.27f;
    public float heightCanelButton = 8.27f;
    public float PositionOkButtonX = -2.85f;
    public float PositionCanelButtonX = -2.85f;
    public float PositionOkButtonY = -2.85f;
    public float PositionCanelButtonY = -2.85f;

     void FTYE_BubbleUI()
    {
        //窗口大小
      //  BackgroundXY.x = BoxBackground.width;
      //  BackgroundXY.y = BoxBackground.height;

        //背景框 ok
        BoxBackground.transform.localPosition = BackgroundPosition;
        BoxBackground.width = (int)(BackgroundSize.x);
        BoxBackground.height = (int)(BackgroundSize.y);
        BoxBackground.GetComponent<BoxCollider>().size = new Vector3(BoxBackground.width, BoxBackground.height, 0);
        BoxBackground.alpha = 0.925f;

        //指针 ok
        Background.transform.localPosition = NeedlePosition;
        Background.height = (int)(NeedleSize.x);
        Background.width = (int)(NeedleSize.y);
        //Background.GetComponent<BoxCollider>().size = new Vector3(Background.width, Background.height, 0);

        //标题
        Title.transform.localPosition = new Vector3(0, BoxBackground.height * 0.15f, 0);
        Title.width = (int)(BoxBackground.width * 0.9f);
        Title.height = (int)(BoxBackground.height * 0.5f);
        Title.text = BubbleText;
     
        // make sure alpha is 0.0f
        gameObject.GetComponent<UIPanel>().alpha = 0.0f;

        
  
    }

    public void ChangeInsideDepth(int vDepth)
    {
        gameObject.GetComponent<UIPanel>().depth = (vDepth);
    }

    public void Update()
    {
        if (gameObject && gameObject.GetComponent<UIPanel>())
        {
            gameObject.GetComponent<UIPanel>().alpha = Mathf.Lerp(gameObject.GetComponent<UIPanel>().alpha, 1.0f, Time.deltaTime * 10.0f);
        }
    }

    public void InitUI(string vBubbleText, Vector3 vBackgroundPosition, Vector3 vNeedlePosition, Vector2 vBackgroundSize, Vector2 vNeedleSize)
    {
        BubbleText = vBubbleText;
        BackgroundPosition = vBackgroundPosition;
        NeedlePosition = vNeedlePosition;
        BackgroundSize = vBackgroundSize;
        NeedleSize = vNeedleSize;
        FTYE_BubbleUI();
    }
    public void DestroyMyself()
    {
        Destroy(gameObject);
    }
}
