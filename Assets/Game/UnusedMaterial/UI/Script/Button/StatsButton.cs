using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsButton : MonoBehaviour
{
    public UIPanel WindowPanel;
    public int buttonId = 0;
    public UISprite Background;
    public UISprite MaskBackground;
    public UISprite BackgroundButton;
    public float xdistance;
    public float ydistance;
    public float ButtonLineDistance;
    public GameObject StatsLabelPrefab;
    public GameObject StatsImagePrefab;
    public GameObject StatsLinePrefab;

    [System.NonSerialized]
    public float OffSetGap = 0.0f;

    void Start()
    {
        StatsButtonUI();
    }

    public void StatsButtonUI()
    {
        Background.GetComponent<BoxCollider>().size = new Vector3(Background.width + xdistance, Background.height + ydistance, 0);

        MaskBackground.width = Background.width;
        MaskBackground.height = Background.height;

        BackgroundButton.width = Background.width;
        BackgroundButton.height = Background.height;

        OffSetGap = Background.width * 0.05f;

        GameObject label1 = (GameObject)Instantiate(StatsLabelPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);
        label1.transform.parent = gameObject.transform;
        label1.transform.localScale = new Vector3(1, 1, 1);
        label1.GetComponent<UILabel>().width = (int)(Background.width * 0.290f);
        label1.GetComponent<UILabel>().height = (int)(Background.height * 0.375f);
        label1.GetComponent<UILabel>().transform.localPosition = new Vector3(OffSetGap + Background.width * (-0.5f - 0.0135f)  + label1.GetComponent<UILabel>().width * 0.5f, 0.0f, 0.0f);
        label1.GetComponent<UILabel>().text = GetTextBasedOnLocation(0, buttonId);

        GameObject label2 = (GameObject)Instantiate(StatsLabelPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);
        label2.transform.parent = gameObject.transform;
        label2.transform.localScale = new Vector3(1, 1, 1);
        label2.GetComponent<UILabel>().text = GetTextBasedOnLocation(1, buttonId);

        if (buttonId == 1)
        {
            label2.GetComponent<UILabel>().width = (int)(Background.width * 0.300f);
            label2.GetComponent<UILabel>().height = (int)(Background.height * 0.425f);
            label2.GetComponent<UILabel>().transform.localPosition = new Vector3(OffSetGap + Background.width * 0.04f + Background.width * -0.16666f + label1.GetComponent<UILabel>().width * 0.5f, 0.0f, 0.0f);

            float tpascore_recent = GameManager_script.Instance().GetTPAScore
                        (
                            GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().BallsPottedList),
                            GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().MissShotsList),
                            GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().SnookeredSelfList),
                            GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().ScratchList)
                        );

            AddStar(1, GetStarBasedOnValue(tpascore_recent));
        }
        else
        {
            label2.GetComponent<UILabel>().width = (int)(Background.width * 0.290f);
            label2.GetComponent<UILabel>().height = (int)(Background.height * 0.375f);
            label2.GetComponent<UILabel>().transform.localPosition = new Vector3(OffSetGap + Background.width * (-0.16666f - 0.0135f)  + label1.GetComponent<UILabel>().width * 0.5f, 0.0f, 0.0f);
        }

        GameObject label3 = (GameObject)Instantiate(StatsLabelPrefab, (Vector3)transform.position, (Quaternion)transform.rotation);
        label3.transform.parent = gameObject.transform;
        label3.transform.localScale = new Vector3(1, 1, 1);
        label3.GetComponent<UILabel>().text = GetTextBasedOnLocation(2, buttonId);

        if (buttonId == 1)
        {
            label3.GetComponent<UILabel>().width = (int)(Background.width * 0.300f);
            label3.GetComponent<UILabel>().height = (int)(Background.height * 0.425f);
            label3.GetComponent<UILabel>().transform.localPosition = new Vector3(OffSetGap + Background.width * 0.04f + Background.width * 0.16666f + label1.GetComponent<UILabel>().width * 0.5f, 0.0f, 0.0f);
            
            float tpascore_alltime = GameManager_script.Instance().GetTPAScore
                        (
                            GameManager_script.Instance().Total_Balls_Potted,
                            GameManager_script.Instance().Total_Miss_Shots,
                            GameManager_script.Instance().Total_Snookered_Self,
                            GameManager_script.Instance().Total_Scratch
                        );

            AddStar(2, GetStarBasedOnValue(tpascore_alltime));
        
        }
        else
        {
            label3.GetComponent<UILabel>().width = (int)(Background.width * 0.290f);
            label3.GetComponent<UILabel>().height = (int)(Background.height * 0.375f);
            label3.GetComponent<UILabel>().transform.localPosition = new Vector3(OffSetGap + Background.width * (0.16666f - 0.0135f) + label1.GetComponent<UILabel>().width * 0.5f, 0.0f, 0.0f);
        }

        AddLine(1);
        AddLine(2);
    }

    public string GetTextBasedOnLocation(int inColumn, int inRow)
    {
        if (inRow == 0 || inRow == 6 || inColumn == 0)
        {
            return Localization.Get(GameManager_script.statsButtonTextArray[inRow][inColumn]);
        }
        else
        {
            if (inRow == 1) // tpa score
            {
                if (inColumn == 1) // 20 games
                {
                    return
                        GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().GetTPAScore
                        (
                            GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().BallsPottedList),
                            GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().MissShotsList),
                            GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().SnookeredSelfList),
                            GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().ScratchList)
                        ), "percentage");
                }
                else if (inColumn == 2)
                {
                    return
                        GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().GetTPAScore
                        (
                            GameManager_script.Instance().Total_Balls_Potted,
                            GameManager_script.Instance().Total_Miss_Shots,
                            GameManager_script.Instance().Total_Snookered_Self,
                            GameManager_script.Instance().Total_Scratch
                        ), "percentage");
                }
            }

            if (inRow == 2) // balls potted
            {
                if (inColumn == 1) // 20 games
                {
                    return GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().BallsPottedList), "number");
                }
                else if (inColumn == 2)
                {
                    return GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().Total_Balls_Potted, "number");
                }
            }

            if (inRow == 3) // BallsMissed
            {
                if (inColumn == 1) // 20 games
                {
                    return GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().MissShotsList), "number");
                }
                else if (inColumn == 2)
                {
                    return GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().Total_Miss_Shots, "number");
                }
            }

            if (inRow == 4) // out of position
            {
                if (inColumn == 1) // 20 games
                {
                    return GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().SnookeredSelfList), "number");
                }
                else if (inColumn == 2)
                {
                    return GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().Total_Snookered_Self, "number");
                }
            }

            if (inRow == 5) // scratch
            {
                if (inColumn == 1) // 20 games
                {
                    return GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().ScratchList), "number");
                }
                else if (inColumn == 2)
                {
                    return GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().Total_Scratch, "number");
                }
            }

            if (inRow == 7) // GamesWonLoss 100 / 200, we might need another stat line for the actual percentage
            {
                if (inColumn == 1) // 20 games
                {
                    string winRatioTwenty = GameManager_script.convertNumberIntoGoodStringFormat((100.0f * (GameManager_script.Instance().GameWonList.Count == 0.0f ? 0.0f : GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().GameWonList) / GameManager_script.Instance().GameWonList.Count)), "percentage");
                    string winNumberTwenty = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().GameWonList), "number") + " / " + GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().GameWonList.Count, "number");

                    return winNumberTwenty + " (" + winRatioTwenty + ")";
                }
                else if (inColumn == 2)
                {
                    string winRatio = GameManager_script.convertNumberIntoGoodStringFormat((100.0f * (GameManager_script.Instance().Total_Games_Played == 0.0f ? 0.0f : GameManager_script.Instance().Total_Games_Won / GameManager_script.Instance().Total_Games_Played)), "percentage");
                    string winNumber = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().Total_Games_Won, "number") + " / " + GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().Total_Games_Played, "number");

                    return winNumber + " (" + winRatio + ")";
                }
            }

            if (inRow == 8) // ChipsWonLoss
            {
                if (inColumn == 1) // 20 games
                {
                    return GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().ChipsWonList), "gamecoinz") + " / " + GameManager_script.convertNumberIntoGoodStringFormat(-1.0f * GameManager_script.Instance().SignedNumberCounter(GameManager_script.Instance().ChipsWonList, false), "gamecoinz");
                }
                else if (inColumn == 2)
                {
                    return GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().Total_Chips_Won, "gamecoinz") + " / " + GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().Total_Chips_Lost, "gamecoinz");
                }
            }

            if (inRow == 9) // WinStreak
            {
                if (inColumn == 1) // 20 games
                {
                    return GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().Current_Win_Streak, "number");
                }
                else if (inColumn == 2)
                {
                    return GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().Current_Win_Streak, "number");
                }
            }

            return "";
        }
    }

    public string GetStarBasedOnValue(float inValue) // needs to be the same as "GetSelfStarType" and "GetBotStarColor"
    {
        inValue = inValue * 0.01f;

        if (inValue <= GameManager_script.SkillsLevel[0])
        {
            return "bronze";
        }
        else if (inValue <= GameManager_script.SkillsLevel[1])
        {
            return "silver";
        }
        else if (inValue <= GameManager_script.SkillsLevel[2])
        {
            return "gold";
        }
        else
        {
            return "red";
        }
    }

    public void AddStar(int inColumn, string inImage)
    {
        GameObject Group1 = (GameObject)Instantiate(StatsImagePrefab, (Vector3)transform.position, (Quaternion)transform.rotation);

        Group1.transform.parent = gameObject.transform;
        Group1.transform.localScale = new Vector3(1, 1, 1);

        Group1.GetComponent<UISprite>().width = (int)(Background.width * 0.05f);
        Group1.GetComponent<UISprite>().height = (int)(Background.width * 0.05f);
        Group1.GetComponent<UISprite>().spriteName = inImage;

        Group1.transform.localPosition = new Vector3(OffSetGap + Background.width * -0.03f + Background.width * -0.5f + Background.width * (inColumn * 0.33333f) + Group1.GetComponent<UISprite>().width * 0.5f, Background.height * 0.05f, 0.0f);
    }

    public void AddLine(int inColumn)
    {
        GameObject Group1 = (GameObject)Instantiate(StatsLinePrefab, (Vector3)transform.position, (Quaternion)transform.rotation);

        Group1.transform.parent = gameObject.transform;
        Group1.transform.localScale = new Vector3(1, 1, 1);

        Group1.GetComponent<UISprite>().width = (int)(ydistance * 1.25f); // pure hack, making it look pretty...
        Group1.GetComponent<UISprite>().height = (int)(Background.height);

        Group1.transform.localPosition = new Vector3(Background.width * -0.5f + Background.width * (inColumn * 0.33333f), 0.0f, 0.0f);
    }
}
