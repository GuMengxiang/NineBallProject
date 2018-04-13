using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

//滑动结构数据//
[System.Serializable]
public class AvatarStructInfo
{
    public float ButtonCount = 28;
    public float ButtonLine = 3;
    public float ButtonScale = 1.333f;
    public float ButtonWidth = 100;
    public float ButtonHeight = 50;
    public float Xgap = 0.02f;
    public float XDistance = 0.02f;
    public float Ygap = 0.02f;
    public float YDistance = 0.02f;
}

//滑动结构数据//
[System.Serializable]
public class ProfileStructInfo
{
    public float ButtonCount = 2;
    public float ButtonLine = 1;
    public float ButtonScale = 1.45f;
    public float ButtonWidth = 100;
    public float ButtonHeight = 50;
    public float Xgap = 0.02f;
    public float XDistance = 0.02f;
    public float Ygap = 0.02f;
    public float YDistance = 0.02f;
}

//用户信息结构数据//
[System.Serializable]
public class UserStructInfo
{
    public float setDownInsideWindow = 0.60f;
    public float setUpInsideWindow = 0.40f;
}

//设置结构数据//
[System.Serializable]
public class SettingStructInfo
{
    public float ButtonCount = 10;
    public float ButtonLine = 2;
    public float ButtonScale = 8;
    public float ButtonWidth = 100;
    public float ButtonHeight = 50;
    public float Xgap = 0.02f;
    public float XDistance = 0.02f;
    public float Ygap = 0.02f;
    public float YDistance = 0.02f;
}

//商店主按钮数据//
[System.Serializable]
public class ShopMainStructInfo
{
    public float ButtonCount = 4;
    public float ButtonLine = 2;
    public float ButtonScale = 1.333f;
    public float ButtonWidth = 100;
    public float ButtonHeight = 50;
    public float Xgap = 0.02f;
    public float XDistance = 0.02f;
    public float Ygap = 0.02f;
    public float YDistance = 0.02f;
}
//金币按钮数据//
[System.Serializable]
public class CoinStructInfo
{
    public float ButtonCount = 6;
    public float ButtonLine = 2;
    public float ButtonScale = 1.333f;
    public float ButtonWidth = 100;
    public float ButtonHeight = 50;
    public float Xgap = 0.02f;
    public float XDistance = 0.02f;
    public float Ygap = 0.02f;
    public float YDistance = 0.02f;
}
//球杆按钮数据//
[System.Serializable]
public class CueStructInfo
{
    public float ButtonCount = 6;
    public float ButtonLine = 2;
    public float ButtonScale = 1.333f;
    public float ButtonWidth = 100;
    public float ButtonHeight = 50;
    public float Xgap = 0.02f;
    public float XDistance = 0.02f;
    public float Ygap = 0.02f;
    public float YDistance = 0.02f;
}
//数据按钮数据//
[System.Serializable]
public class StatsStructInfo
{
    public float ButtonCount = 100;
    public float ButtonLine = 1;
    public float ButtonScale = 8f;
    public float ButtonWidth = 100;
    public float ButtonHeight = 50;
    public float Xgap = 0.02f;
    public float XDistance = 0.02f;
    public float Ygap = 0.02f;
    public float YDistance = 0.02f;
}
/// <summary>
/// 结构
/// </summary>
public enum WindowState : int
{
    Profile = 0,
    Setting,
    ShopMain,
    AvatarShop,
    Coin,
    Cue,
    Stats,
	Landing
}

public enum PopupWindowState : int
{
    Selector = 0,
    DailyDeal,
    DailyBonus,
    PlayerProfile,
    Help,
    FTUE_Popup,
    Language,
    StarterDeal
}

public enum MusicClip : int
{
    B_B_Hard_0 = 0,
    B_B_Hard_1,
    B_B_Hard_2,
    B_B_Mid_0,
    B_B_Mid_1,
    B_B_Weak_0,
    B_B_Weak_1,
    B_C_Hard_0,
    B_C_Mid_0,
    B_C_Weak_0,
    B_Pocket_0,
    B_Pocket_1,
    B_Pocket_2,
    B_Pocket_3,
    B_Pocket_4,
    B_Rack_0,
    B_Rack_1,
    B_Roll_Under,
    B_W_0,
    B_W_1,
    Bad_Transfer,
    Button_Click,
    Cash,
    Clock,
    Finding,
    Found_Match,
    Good_Transfer,
    LevelUp,
    Lose_Music,
    Lose_Over,
    Win_Music,
    Win_Over
}

public enum ViolatorType : int
{
    MainMenuCenter = 0,
    MainMenuSetting,
    MainMenuProfileShop
}

public enum StarType : int
{
    bronze = 0,
    silver,
    gold,
    red,
    bbronze,
    bsilver,
    bgold,
    bred
}
public struct StarInfo
{
    public int text;
    public StarType starType;

    public StarInfo(int vtext, StarType vstarType)
    {
        text = vtext;
        starType = vstarType;
    }
}

public enum MenuType : int
{
    start = 0,
    gameover
}

public enum ClickType : int
{
    Enable = 0,
    Disable,
    Label,
    Look
}

public class SettingInfo
{
    public string SoundText;
    public string SoundButtonText;
    public ClickType SoundType;
    public string VibrateonturnText;
    public string VibrateonturnButtonText;
    public ClickType VibrateonturnType;
    public string ProducerText;
    public string ProducerButtonText;
    public ClickType ProducerType;
    public string Programmer1Text;
    public string Programmer1ButtonText;
    public ClickType Programmer1Type;
    public string Programmer2Text;
    public string Programmer2ButtonText;
    public ClickType Programmer2Type;
    public string Artist1Text;
    public string ArtistButton1Text;
    public ClickType Artist1Type;
    public string Artist2Text;
    public string Artist2ButtonText;
    public ClickType Artist2Type;
    public string Artist3Text;
    public string Artist3ButtonText;
    public ClickType Artist3Type;
    public string DesignerText;
    public string DesignerButtonText;
    public ClickType DesignerType;
    public string VersionText;
    public string VersionButtonText;
    public ClickType VersionType;
    public string LanguageText;
    public string LanguageButtonText;
    public ClickType LanguageType;
    public string HelpText;
    public string HelpButtonText;
    public ClickType HelpType;
    public string ScoreText;
    public string ScoreButtonText;
    public ClickType ScoreType;
    public string Main_helpText;
    public string Main_helpButtonText;
    public ClickType Main_helpType;

    public SettingInfo
    (
      string vLanguageText,
      string vLanguageButtonText,
      ClickType vLanguageType,
      string vSoundText,
      string vSoundButtonText,
      ClickType vSoundType,
      string vVibrateonturnText,
      string vVibrateonturnButtonText,
      ClickType vVibrateonturnType,
      string vHelpText,
      string vHelpButtonText,
      ClickType vHelpType,
      string vMain_helpText,
      string vMain_helpButtonText,
      ClickType vMain_helpType,
      string vScoreText,
      string vScoreButtonText,
      ClickType vScoreType,
      string vProducerText,
      string vProducerButtonText,
      ClickType vProducerType,
      string vProgrammer1Text,
      string vProgrammer1ButtonText,
      ClickType vProgrammer1Type,
      string vProgrammer2Text,
      string vProgrammer2ButtonText,
      ClickType vProgrammer2Type,
      string vArtist1Text,
      string vArtistButton1Text,
      ClickType vArtist1Type,
      string vArtist2Text,
      string vArtist2ButtonText,
      ClickType vArtist2Type,
      string vArtist3Text,
      string vArtist3ButtonText,
      ClickType vArtist3Type,
      string vDesignerText,
      string vDesignerButtonText,
      ClickType vDesignerType,
      string vVersionText,
      string vVersionButtonText,
      ClickType vVersionType
    )
    {
        SoundText = vSoundText;
        SoundButtonText = vSoundButtonText;
        SoundType = vSoundType;
        VibrateonturnText = vVibrateonturnText;
        VibrateonturnButtonText = vVibrateonturnButtonText;
        VibrateonturnType = vVibrateonturnType;
        HelpText = vHelpText;
        HelpButtonText = vHelpButtonText;
        HelpType = vHelpType;
        ScoreText = vScoreText;
        ScoreButtonText = vScoreButtonText;
        ScoreType = vScoreType;
        LanguageText = vLanguageText;
        LanguageButtonText = vLanguageButtonText;
        LanguageType = vLanguageType;
        Main_helpText = vMain_helpText;
        Main_helpButtonText = vMain_helpButtonText;
        Main_helpType = vMain_helpType;
        ProducerText = vProducerText;
        ProducerButtonText = vProducerButtonText;
        ProducerType = vProducerType;
        Programmer1Text = vProgrammer1Text;
        Programmer1ButtonText = vProgrammer1ButtonText;
        Programmer1Type = vProgrammer1Type;
        Programmer2Text = vProgrammer2Text;
        Programmer2ButtonText = vProgrammer2ButtonText;
        Programmer2Type = vProgrammer2Type;
        Artist1Text = vArtist1Text;
        ArtistButton1Text = vArtistButton1Text;
        Artist1Type = vArtist1Type;
        Artist2Text = vArtist2Text;
        Artist2ButtonText = vArtist2ButtonText;
        Artist2Type = vArtist2Type;
        Artist3Text = vArtist3Text;
        Artist3ButtonText = vArtist3ButtonText;
        Artist3Type = vArtist3Type;
        DesignerText = vDesignerText;
        DesignerButtonText = vDesignerButtonText;
        DesignerType = vDesignerType;
        VersionText = vVersionText;
        VersionButtonText = vVersionButtonText;
        VersionType = vVersionType;
        
    }
}
public enum StateImages : int
{ 
    none=0,
    bronze,
    silver,
    gold,
    red
}

public enum PlayerHeadInfo : int
{
    Default = 0,
    Light
}
public enum InGameBallDisplayInfo : int
{
    Close = 0,
    Open
}
public enum InGameBallReturnDisplayInfo : int
{
    BallReturn = 0,
    BallReturnMedium,
    BallReturnLarge
}

//提示框类型//
public enum EToolTipType : byte
{
    eButton = 0,
    eLabel
}

public enum ConfirmationType : int
{
    undecided = 0,
    confirmed,
    denied
};

// different types of button states for avatar buttons
public enum GenericButtonState : int
{
    locked = 0,
    owned,
    free,
    payable,
    cantafford,
    equipped
};

public struct ToolTipInfo
{
    public string Title;
    public string CancelMessage;
    public string OKMessage;
    public EToolTipType ToolTipType;

    public ToolTipInfo(string vTitle, string vOKMessage, string vCancelMessage, EToolTipType vToolTipType)
    {
        Title = vTitle;
        OKMessage = vOKMessage;
        CancelMessage = vCancelMessage;
        ToolTipType = vToolTipType;
    }
}
public struct WarningInfo
{
    public string Title;
    public string CancelMessage;
    public string OKMessage;

    public WarningInfo (string vTitle, string vOKMessage, string vCancelMessage)
    {
        Title = vTitle;
        OKMessage = vOKMessage;
        CancelMessage = vCancelMessage;
    }
}

public enum AvatarAttributesType : int
{
    level = 0,
    price
};

public enum CueAttributesType : int
{
    level = 0,
    price,
    extend,
    force,
    spin,
    map
};

public class UIinterstitialWindowInfo
{
    // 如果是游戏开始的加载页面，就是true。如果是游戏结束的页面，就是false。
    public bool trueIfIncomingPage;

    // 这些是给开始页面的。
    public float Total_TPAScore;
    public float Total_GamesPlayed;
    public float Total_GamesWon;
    public float remainingCoins;
    public float streak;

    // 这些是给结束页面的。
    public bool trueIfYouWin; // 如果你赢得了游戏，就是true。
    public bool trueIfYouWinByDisconnect; // 如果你赢得了游戏，就是true。
    public bool trueIfYouLoseByDisconnect; // 如果你赢得了游戏，就是true。
    public bool performLevelUp; // 如果需要做一个升级页面。
    public float currentWager;

    // This is for player 1, begin and end
    public int HeadImage_one;
    public string PlayerName_one;
    public StarInfo Star_one;

    // This is for player 1, end
    public float balls_potted_one;
    public float balls_missed_one;
    public float out_of_position_one;
    public float scratches_one;
    public float single_TPAScore_one;

    // This is for player 2, end
    public int HeadImage_two;
    public string PlayerName_two;

    // This is for player 2, end
    public float balls_potted_two;
    public float balls_missed_two;
    public float out_of_position_two;
    public float scratches_two;
    public float single_TPAScore_two;

    // game over text...
    public GameOverType game_over_type;
}

public class ProfilePopupInfo
{
    public int HeadImage;
    public string PlayerName;
    public StarInfo Star;
    public float cueEquipped;
    public float streak;
    public float TPAScore;
    public float gamesPlayed;
    public float gamesWon;
    public float CoinsBalance;
}

public class DailyBonusPopupInfo
{
   // public DailyBonusPopupInfo()
   // { }
}

public class CustomDealPopupInfo
{
   // public CustomDealPopupInfo()
   // { }
}

public class InterstitialInfo
{
    // public InterstitialInfo()
    // { }
}

public struct CoinInfo
{
    public string Title;
    public string ImageName;
    public float CoinCount;
    public float PayMoney;

    public CoinInfo(string vTitle, string vImageName, float vCoinCount, float vPayMoney)
    {
        Title = vTitle;
        ImageName = vImageName;
        CoinCount = vCoinCount;
        PayMoney = vPayMoney;
    }
}

[System.Serializable]
public class CueInfo
{
    public MeshFilter ModelMesh;
    public Texture ModelTexture;    
    public CueInfo(MeshFilter vModelMesh, Texture vModelTexture)
    {
        ModelMesh = vModelMesh;
        ModelTexture = vModelTexture;
    }
}

public enum GameOverType : byte
{
    None=0,
    Win,
    Lose,
    SoloOneWin,
    SoloTwoWin,
    Tutorial
}

public class SingleGamePlayInfo
{
    public bool GameWon = false;
    public float ChipsWon = 0.0f;

    public float BallsPotted; // this is the ONLY success condition (numerator)
    public float SnookeredSelf; // fail 1
    public float MissShots; // fail 2
    public float Scratch; // fail 3
}

public class PossibleShootPlacement
{
    // level
    public RobotShotLevelType ObjectLevel = RobotShotLevelType.FoulHit;

    // score 
    public float ObjectScore = 0.0f;

    // cue natural stats
    public Vector3 cueBallPosition; // cue ball position
    public Quaternion cuePivotLocalRotation; // cue ball big spin rotation
    public Vector3 cuePivotForwardRotation; // cue ball rotation forward
    public Vector3 cueRotationLocalPosition; // should really be a Vector2
    public float cueBallStrength = 0.0f; // draw strength

    // smart bot related lolz
    public float smartBotLongShot = 0.0f;
    public float smartBotIntendToPocket = 0.0f;
}

public enum GestureType : int
{
    DoNothing = 0,
    DoNothingCompletely,
    DoNothingShort,
    MoveCueBallToDestination,
    RotateTowardsDestination,
    RotateTowardsDestinationShort,
    GiveLocalBallSpin,
    CuePump,
    HitTheBall
};

public enum GestureSpeed : int
{
    ShortDuration = 0,
    LongDuration
}

public class SmartBotGesture
{
    public GestureType LocalGestureType = GestureType.DoNothing;

    public Vector3 DestPosition = Vector3.zero;
    public Vector3 DestDirection = Vector3.zero;
    public Vector3 DestSpin = Vector3.zero;
    public float DestStrength = 0.0f;

    public Vector3 InitPosition = Vector3.zero;
    public Vector3 InitDirection = Vector3.zero;
    public Vector3 InitSpin = Vector3.zero;
    public float InitStrength = 0.0f;

    public RobotShotLevelType IntendedLevel = RobotShotLevelType.FoulHit;
};

public enum RobotShotLevelType : int
{
    StraightPocket = 1,
    SecondaryPocket,
    GoodHit,
    RandomHit,
    FoulHit,
    NeverStop
};

public enum ChipType : int
{
    Chip1 = 0,
    Chip2,
    Chip3,
    Chip4,
    Chip5,
    Chip6,
    Chip7,
    Chip8
}
