using UnityEngine;
using System.Collections;

public class UIProfilePanel : MonoBehaviour
{
    public UIPanel WindowPanel;
    public UIPanel InsidePanel;
    public Vector2 BackgroundXY;
    public GameObject UpInsideWindow;
    public GameObject Head;
    public UISprite Headimage;

    // yo yo usernamez
    public GameObject UserName;
    public UILabel UserNameLabel;
    public UIInput mInput;

    public UISprite MoneyImage;
    public UISprite PencilImage;
    public UILabel CoinCounts;
    public UILabel Rank;
    public UILabel Skill;
    public UILabel WinRatio;
    public UILabel TPA_Score;
    public UILabel RankValue;
    public UILabel SkillValue;
    public UILabel WinRatioValue;
    public UILabel TPA_ScoreValue;
    public GameObject ProfileSlider;
    public UISprite ProfileSliderBackgound;
    public UISprite ProfileSliderForeground;
    public UISprite ProfileSprite;
    public UISprite Star;
    public UILabel TextLabel;

    void Start()
    {
        UI();
    }

    public void UI()
    {
        // waaasaaaaap! madafakaz!
        GameManager_script.Instance().PopulateSelfGameProfileInfo();

        // dajiang hack, we are currently in profile mode no matter what else (so we input into profile name stuffz)
        GameManager_script.Instance().isEverythingFocusedOnFrdsSelector = false; 

        //上方内部窗口
        UpInsideWindow.GetComponent<UISprite>().width = (int)(BackgroundXY.x);
        UpInsideWindow.GetComponent<UISprite>().height = (int)(BackgroundXY.y);

        //头像
        Headimage.width = (int)(BackgroundXY.x * 0.15f);
        Headimage.height = (int)(BackgroundXY.x * 0.15f);

        Head.GetComponent<OpenWindowUI>().RootPanel = WindowPanel.gameObject.transform.parent.parent.GetComponent<UIPanel>();
        Head.GetComponent<OpenWindowUI>().Parent = WindowPanel.gameObject.transform.parent.gameObject;
        Head.GetComponent<BoxCollider>().size = new Vector3(Headimage.width, Headimage.height, 0);
        Head.transform.localPosition = new Vector3(-BackgroundXY.x / 2.0f + Headimage.width / 2.0f + Headimage.width * 0.2f, 0, 0.0f);

        // change image
        ChangeHeadImage(GameManager_script.Instance().AvatarEquipped);

        // prepare for next round of positioning
        float HeadImageSpace = Headimage.width + Headimage.width * 0.2f * 2.0f;

        //钱
        MoneyImage.width = (int)(BackgroundXY.y * 0.20f);
        MoneyImage.height = (int)(BackgroundXY.y * 0.20f);
        MoneyImage.transform.localPosition = new Vector3(BackgroundXY.x * -0.5f + MoneyImage.width * 0.5f + HeadImageSpace, BackgroundXY.y * 0.0f, 0.0f);

        CoinCounts.width = (int)(BackgroundXY.x * 0.20f);
        CoinCounts.height = (int)(BackgroundXY.y * 0.175f);
        CoinCounts.transform.localPosition = new Vector3(MoneyImage.transform.localPosition.x + MoneyImage.width * 0.5f + MoneyImage.width * 0.25f, BackgroundXY.y * 0.00f, 0.0f);
        CoinCounts.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().CoinCount, "gamecoinz");

        // 名字
        UserName.transform.localPosition = new Vector3(CoinCounts.transform.localPosition.x + CoinCounts.width * 0.5f, BackgroundXY.y * 0.275f, 0.0f);

        // more names
        UserNameLabel.width = (int)(BackgroundXY.x * 0.20f);
        UserNameLabel.height = (int)(BackgroundXY.y * 0.175f);
        UserNameLabel.transform.localPosition = new Vector3(-CoinCounts.width * 0.5f, 0.0f, 0.0f);

        // 铅笔
        PencilImage.width = (int)(BackgroundXY.y * 0.20f);
        PencilImage.height = (int)(BackgroundXY.y * 0.20f);
        PencilImage.transform.localPosition = new Vector3(BackgroundXY.x * -0.5f + PencilImage.width * 0.5f + HeadImageSpace, BackgroundXY.y * 0.275f, 0.0f);
        
        // input invis box
        mInput.GetComponent<BoxCollider>().size = new Vector3(UserNameLabel.width * 1.3f, UserNameLabel.height, 0.0f); // include the pencil
        mInput.GetComponent<BoxCollider>().center = new Vector3(UserNameLabel.width * -0.15f, 0, 0);
        mInput.value = GameManager_script.Instance().CharLength(GameManager_script.Instance().First_Name, GameManager_script.Instance().Max_Name_Length);

        //Star
        Star.height = Mathf.CeilToInt(BackgroundXY.y * 0.2f);
        Star.width = Mathf.CeilToInt(BackgroundXY.y * 0.2f);
        Star.transform.localPosition = new Vector3(BackgroundXY.x * -0.5f + Star.width * 0.5f + HeadImageSpace, BackgroundXY.y * -0.275f, 0.0f);

        Star.GetComponent<UIStar>().Text.width = GameManager_script.Instance().StarTextFineTuneWidth(Star.width, GameManager_script.Instance().CurrentLevel);
        Star.GetComponent<UIStar>().Text.height = GameManager_script.Instance().StarTextFineTuneHeight(Star.height, GameManager_script.Instance().CurrentLevel);
        Star.GetComponent<UIStar>().Text.transform.localPosition = GameManager_script.Instance().StarTextFineTunePosition(Star.GetComponent<UIStar>().Text.transform.localPosition, Star.GetComponent<UIStar>().Text.width, Star.GetComponent<UIStar>().Text.height, GameManager_script.Instance().CurrentLevel);
        Star.GetComponent<UIStar>().StartUI(new StarInfo((int)GameManager_script.Instance().CurrentLevel, GameManager_script.Instance().GetSelfStarType()));

        //经验条
        ProfileSprite.width = (int)(BackgroundXY.x * 0.20f);
        ProfileSprite.height = (int)(ProfileSprite.width / 8.0f); // measured and squashed
        ProfileSlider.transform.localPosition = new Vector3(Star.transform.localPosition.x + Star.width * 0.5f + Star.width * 0.25f, BackgroundXY.y * -0.250f, 0.0f);

        ProfileSliderBackgound.width = ProfileSprite.width;
        ProfileSliderBackgound.height = ProfileSprite.height;

        ProfileSliderForeground.width = ProfileSprite.width;
        ProfileSliderForeground.height = ProfileSprite.height;

        ChangeProfileSlider();

        // experience bar label
        TextLabel.width = ProfileSprite.width;
        TextLabel.height = (int)(ProfileSprite.height * 0.85f);
        TextLabel.transform.localPosition = new Vector3(ProfileSlider.transform.localPosition.x + TextLabel.width / 2, ProfileSlider.transform.localPosition.y - ProfileSprite.height * 0.5f - TextLabel.height * 0.75f, 0.0f);

        string currentExperience = "";
        string baseExperience = "";

        if ((int)GameManager_script.Instance().CurrentLevel < GameManager_script.LevelExperienceCounts.Length)
        {
            currentExperience = GameManager_script.convertNumberIntoGoodStringFormat((int)(GameManager_script.LevelExperienceCounts[(int)GameManager_script.Instance().CurrentLevel] * GameManager_script.Instance().CurrentLevelExperience), "number");
            baseExperience = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.LevelExperienceCounts[(int)GameManager_script.Instance().CurrentLevel], "number");
            TextLabel.text = currentExperience + " / " + baseExperience;
        }
        else
        {
            currentExperience = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.LevelExperienceCounts[GameManager_script.LevelExperienceCounts.Length - 1], "number");
            baseExperience = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.LevelExperienceCounts[GameManager_script.LevelExperienceCounts.Length - 1], "number");
            TextLabel.text = currentExperience + " / " + baseExperience;
        }

        // ranking (now its army rank)
        Rank.height = (int)(BackgroundXY.y * 0.12f);
        Rank.width = (int)(BackgroundXY.x * 0.35f);
        Rank.transform.localPosition = new Vector3(0.01f * BackgroundXY.x, BackgroundXY.y * -0.3405f, 0.0f);
        Rank.text = Localization.Get("WindowProfileRank");

        RankValue.height = (int)(BackgroundXY.y * 0.12f);
        RankValue.width = (int)(BackgroundXY.x * 0.25f);
        RankValue.transform.localPosition = new Vector3(BackgroundXY.x * 0.5f - BackgroundXY.x * 0.025f, BackgroundXY.y * -0.3405f, 0.0f);
        RankValue.text = Localization.Get(GameManager_script.MilitaryRankName[GameManager_script.Instance().selfGameProfileInfo.Star.text]);

        // skills (now its streak)
        Skill.height = (int)(BackgroundXY.y * 0.12f);
        Skill.width = (int)(BackgroundXY.x * 0.35f);
        Skill.transform.localPosition = new Vector3(0.01f * BackgroundXY.x, BackgroundXY.y * -0.1135f, 0.0f);
        Skill.text = Localization.Get("WindowProfileWinStreak");

        SkillValue.height = (int)(BackgroundXY.y * 0.12f);
        SkillValue.width = (int)(BackgroundXY.x * 0.25f);
        SkillValue.transform.localPosition = new Vector3(BackgroundXY.x * 0.5f - BackgroundXY.x * 0.025f, BackgroundXY.y * -0.1135f, 0.0f);
        SkillValue.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().Current_Win_Streak, "number");

        // TPA_Score (now its win percentage)
        TPA_Score.height = (int)(BackgroundXY.y * 0.12f);
        TPA_Score.width = (int)(BackgroundXY.x * 0.35f);
        TPA_Score.transform.localPosition = new Vector3(0.01f * BackgroundXY.x, BackgroundXY.y * 0.1135f, 0.0f);
        TPA_Score.text = Localization.Get("WindowProfileWinRatio");

        TPA_ScoreValue.height = (int)(BackgroundXY.y * 0.12f);
        TPA_ScoreValue.width = (int)(BackgroundXY.x * 0.25f);
        TPA_ScoreValue.transform.localPosition = new Vector3(BackgroundXY.x * 0.5f - BackgroundXY.x * 0.025f, BackgroundXY.y * 0.1135f, 0.0f);
        TPA_ScoreValue.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().GetWinRatio(), "percentage");

        // WinRatio (now its tpa score)
        WinRatio.height = (int)(BackgroundXY.y * 0.12f);
        WinRatio.width = (int)(BackgroundXY.x * 0.35f);
        WinRatio.transform.localPosition = new Vector3(0.01f * BackgroundXY.x, BackgroundXY.y * 0.3405f, 0.0f);
        WinRatio.text = Localization.Get("WindowProfileTPAScore");

        WinRatioValue.height = (int)(BackgroundXY.y * 0.12f);
        WinRatioValue.width = (int)(BackgroundXY.x * 0.25f);
        WinRatioValue.transform.localPosition = new Vector3(BackgroundXY.x * 0.5f - BackgroundXY.x * 0.025f, BackgroundXY.y * 0.3405f, 0.0f);
        WinRatioValue.text = GameManager_script.convertNumberIntoGoodStringFormat(GameManager_script.Instance().GetMaxTPAScore(), "percentage");
    }

    public void ChangeHeadImage(int index)
    {
        Headimage.spriteName = "" + index;
    }

    public void ChangeInsideDepth(int vDepth)
    {
        gameObject.GetComponent<UIPanel>().depth = (vDepth);
    }

    public void ChangeProfileSlider()
    {
        float count = Mathf.Clamp(GameManager_script.Instance().CurrentLevelExperience * 100.0f, 0, 101);

        ProfileSlider.GetComponent<UISlider>().value = (count / 100.0f);
    }
}
