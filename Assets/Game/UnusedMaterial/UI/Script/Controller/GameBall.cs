using UnityEngine;
using System.Collections;

public class GameBall : MonoBehaviour
{
    public int ButtonId;
    public Vector2 BackgroundXY;
    public UISprite Ball;
    public UISprite BallBackground;
    public UISprite BallImage;

    public void UI()
    {
        Ball.height = (int)(BackgroundXY.x);
        Ball.width = Ball.height;
        BallImage.height = Ball.width;
        BallImage.width = Ball.height;
        BallBackground.height =(int)(Ball.width*1.1f);
        BallBackground.width = (int)(Ball.height*1.1f);

        ChangeBall(InGameBallDisplayInfo.Open);
    }

    public void ChangeBall(InGameBallDisplayInfo buttonInfo)
    {
        if (buttonInfo == InGameBallDisplayInfo.Close)
        {
            BallImage.gameObject.SetActive(false);
        }
        else
        {
            BallImage.gameObject.SetActive(true);
        }
    }
}
