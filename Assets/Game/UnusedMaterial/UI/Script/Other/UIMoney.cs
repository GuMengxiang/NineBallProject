using UnityEngine;
using System.Collections;

public class UIMoney : MonoBehaviour
{
    public UISprite Background;
    public UISprite MaskBackground;
    public UISprite MaskRimBackground;
    public UISprite MoneyImage;
    public UILabel MoneyLabel;

    [System.NonSerialized]
    public bool TotalScoreBool = false;
    [System.NonSerialized]
    public float StartMoney = 0;
    [System.NonSerialized]
    public float CurrentMoney = 0;
    [System.NonSerialized]
    public float EndMoney = 0;
    [System.NonSerialized]
    public float UpdateMoney = 0;

    void Start()
    {
        UI();
    }

    void Update()
    {
        if (TotalScoreBool)
        {
            CurrentMoney += UpdateMoney;

            MoneyLabel.text = "" + GameManager_script.convertNumberIntoGoodStringFormat(CurrentMoney, "gamecoinz");

            if ((CurrentMoney - EndMoney) * (EndMoney - StartMoney) > 0.0f)
            {
                ChangeMoney(EndMoney);

                TotalScoreBool = false;
            }
        }
    }

    void UI()
    {
        MaskBackground.width = Background.width;
        MaskBackground.height = Background.height;

        MaskRimBackground.width = Background.width;
        MaskRimBackground.height = Background.height;

        MoneyImage.width = (int)(Background.height * 0.7f);
        MoneyImage.height = MoneyImage.width;
        MoneyImage.transform.localPosition = new Vector3(-MaskBackground.width * 0.45f + MoneyImage.width * 0.47f, 0, 0);

        MoneyLabel.height = (int)(Background.height * 0.6f);
        MoneyLabel.width = (int)(Background.width * 0.7f);
        MoneyLabel.transform.localPosition = new Vector3(Background.width * 0.45f, MoneyLabel.transform.localPosition.y - MoneyLabel.height * 0.075f, 0.0f);
    }

    public void ChangeMoney(float inMoney)
    {
        MoneyLabel.text = GameManager_script.convertNumberIntoGoodStringFormat(inMoney, "gamecoinz");

    }

    public void AnimateUpdateMoney(float inStart, float inEnd)
    {
        StartMoney = inStart;

        CurrentMoney = inStart;

        EndMoney = inEnd;

        CalculateUpdateMoney(StartMoney, EndMoney);

        TotalScoreBool = true;
    }

    public void AnimateUpdateMoneyBroadcast(float inStart)
    {
        StartMoney = inStart;

        CurrentMoney = inStart;

        EndMoney = GameManager_script.Instance().CoinCount;

        CalculateUpdateMoney(StartMoney, EndMoney);

        TotalScoreBool = true;
    }

    public void CalculateUpdateMoney(float inStart, float inEnd)
    {
        float diff = inEnd - inStart;

        if (Mathf.Abs(diff) > 1000000)
        {
            UpdateMoney = diff / (60 * 5.00f); // 6 seconds
        }
        else if (Mathf.Abs(diff) > 100000)
        {
            UpdateMoney = diff / (60 * 4.00f); // 5 seconds
        }
        else if (Mathf.Abs(diff) > 10000)
        {
            UpdateMoney = diff / (60 * 3.00f); // 4 seconds
        }
        else if (Mathf.Abs(diff) > 1000)
        {
            UpdateMoney = diff / (60 * 2.25f); // 3 seconds
        }
        else if (Mathf.Abs(diff) > 100)
        {
            UpdateMoney = diff / (60 * 1.50f); // 2 seconds
        }
        else
        {
            UpdateMoney = diff / (60 * 1.00f); // 1 second
        }
    }
}
