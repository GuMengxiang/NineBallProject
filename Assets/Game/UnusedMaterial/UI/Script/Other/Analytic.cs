using UnityEngine;
using System;
using System.Collections;
using Parse;

public class Analytic : MonoBehaviour
{
    private static readonly DateTime UnixEpoch = new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static bool DebugDontSendParseSignals = true; // dajiang dajiang

    // send function
    public static void SendParseAction(ParseObject inAction)
    {
        if (inAction != null && !DebugDontSendParseSignals)
        {
            inAction.SaveAsync();
        }
    }

    // common bullshits
    public static void GetCommonFields(ref ParseObject inAction)
    {
        inAction["a_gi"] = (string)(GameManager_script.Instance().Ginger_ID); // player's id
        inAction["a_vn"] = (string)(GameManager_script.versionNumber); // version number, whatever version number agreed with settings page
        inAction["a_pt"] = (string)(GameManager_script.platformNumber); // platform, Android
        inAction["a_mk"] = (string)(GameManager_script.marketNumber); // market, China
        inAction["a_lc"] = (string)(GameManager_script.Instance().ReturnLanguageStringInEnglish()); // language, ES, etc
        inAction["a_hr"] = (int)(System.DateTime.Now.Subtract(new System.DateTime(2014, 1, 1)).TotalHours % 24); // time stamp, 1 ~ 24, local hour count
        inAction["a_gh"] = (int)(DateTime.UtcNow - UnixEpoch).TotalHours; // total time stamp
        inAction["a_ge"] = (int)((int)System.DateTime.Now.Subtract(new System.DateTime(2014, 1, 1)).TotalDays - GameManager_script.Instance().Install_Day_Since_Beginning); // days since install
    }

    // action 1, periodic ping the server and tell them we are online (performed in game manager script's update) (done)
    public static void OnlinePeriodicPing(float inChipCount, float inAccuScore, float inLevel, float inPlayed, float inWins)
    {
        // declare
        ParseObject action = new ParseObject("Online");

        // common stuff
        GetCommonFields(ref action);

        // specific stuff
        action["b_lagg"] = (int)(PhotonNetwork.GetPing()); // dajiang hack, if its exactly 300, it's probably no good
        action["b_chip"] = (int)(inChipCount);
        action["b_accu"] = (int)(inAccuScore);
        action["b_lvls"] = (int)(inLevel);
        action["b_plyd"] = (int)(inPlayed);
        action["b_wind"] = (int)(inWins);

        // send
        SendParseAction(action);
    }

    // action 2, performed when any event happens (each has its own name)
    public static void EventHappenPing(string inButtonName)
    {
        // declare
        ParseObject action = new ParseObject("EvtHpn");

        // common stuff
        GetCommonFields(ref action);

        // specific stuff
        action["b_name"] = (string)(inButtonName);

        // send
        SendParseAction(action);
    }

    // action 3, network and frds and smart bot game start ping (a lot of fields) (done)
    public static void GameStartPing(string inType, float inChips, float inTotalChips, float inTPA, float inGamesPlayed, float inGamesWon)
    {
        // declare
        ParseObject action = new ParseObject("GameSt");

        // common stuff
        GetCommonFields(ref action);

        // specific stuff
        action["b_type"] = (string)(inType);
        action["b_chip"] = (int)(inChips);
        action["b_totl"] = (int)(inTotalChips);
        action["b_tpas"] = (int)(inTPA);
        action["b_plyd"] = (int)(inGamesPlayed);
        action["b_wond"] = (int)(inGamesWon);

        // send
        SendParseAction(action);
    }

    // action 4, money game finished ping (done)
    public static void GameFinishPing(string inType, bool inWin, float inSeconds, string inFinishType)
    {
        // declare
        ParseObject action = new ParseObject("GameDn");

        // common stuff
        GetCommonFields(ref action);

        // specific stuff
        action["b_resn"] = (string)(inFinishType); // network or normal or quit or whatever
        action["b_type"] = (string)(inType);
        action["b_dura"] = (int)(inSeconds);
        action["b_wins"] = (bool)(inWin);

        // send
        SendParseAction(action);
    }

    // action 5, buy chips chip grant ping (done)
    public static void BuyItemPing(float inPrice)
    {
        // declare
        ParseObject action = new ParseObject("BuyItm");

        // common stuff
        GetCommonFields(ref action);

        // specific stuff
        action["b_price"] = inPrice;

        // send
        SendParseAction(action);
    }
}
