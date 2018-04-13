using System;
using UnityEngine;
using System.Collections;

public class Background_script : MonoBehaviour
{
    public static int eighteenHours = 18 * 3600;
    public static int twentyfourHours = 24 * 3600;

    [System.NonSerialized]
    public float interval = 1.0f; // calculate this every second
    [System.NonSerialized]
    public float dailyCounter = 1.0f;

    // this is going to update as long as the main menu page is alive
    // rtl and other popups should all be written in here
    void Update()
    {
        dailyCounter += Time.deltaTime;

        if (dailyCounter > interval)
        {
            // if last time and this time are on different sides of the gap
            float TotalSecondsSinceBeginning = (float)(System.DateTime.Now.Subtract(new System.DateTime(2014, 1, 1)).TotalSeconds);

            // ------------ftue popup starts here------------
            // thie is 1 of the only 2 popups that are auto popped, so we need to make sure there are NO other popups currently open and we are really in the main main page
            if (GameManager_script.Instance().InMenuAndAtMainPage && GameManager_script.Instance().SeenSwipeAndPullEver == 0.0f && GameManager_script.Instance().PopupCurrentlyVisible == false)
            {
                // make sure we open up the FTUE tab here once
                gameObject.GetComponent<OpenPopupUI>().OpenUI(PopupWindowState.FTUE_Popup);
            }

            // ------------daily bonus popup starts here------------
            int LastTimeDailyBonusOpenedWholeDays = (int)(GameManager_script.Instance().LastTimeDailyBonusOpened - eighteenHours) / twentyfourHours;
            int ThisTimeDailyBonusOpenedWholeDays = (int)(TotalSecondsSinceBeginning - eighteenHours) / twentyfourHours;

            // we are indeed on different sides of the day
            if (LastTimeDailyBonusOpenedWholeDays < ThisTimeDailyBonusOpenedWholeDays)
            {
                // thie is 1 of the only 2 popups that are auto popped, so we need to make sure there are NO other popups currently open and we are really in the main main page
                if (GameManager_script.Instance().InMenuAndAtMainPage && GameManager_script.Instance().SeenSwipeAndPullEver == 1.0f && GameManager_script.Instance().PopupCurrentlyVisible == false)
                {
                    // open up the daily deal page here
                    gameObject.GetComponent<OpenPopupUI>().OpenUI(PopupWindowState.DailyBonus);

                    // house keeping for daily bonus so it doesn't double show up
                    GameManager_script.Instance().LastTimeDailyBonusOpened = TotalSecondsSinceBeginning;
                    PlayerPrefs.SetFloat("LastTimeDailyBonusOpened", TotalSecondsSinceBeginning);
                }
            }

            // ------------daily deal refreshing starts here, including starter packs------------
            int LastTimeDailyDealCalculatedWholeDays = (int)GameManager_script.Instance().LastDayDailyDealCalculated;
            int ThisTimeDailyDealCalculatedWholeDays = (int)(TotalSecondsSinceBeginning / twentyfourHours);

            if (LastTimeDailyDealCalculatedWholeDays < ThisTimeDailyDealCalculatedWholeDays)
            {
                // try to calculate if we need the daily deal popup here
                // actually just calculating it here is enough, but we will do the same when we start a game or unpause a game anyways, just to make the refresh instant
                GameManager_script.Instance().GenerateDailyDealPackage();

                // house keeping
                GameManager_script.Instance().LastDayDailyDealCalculated = ThisTimeDailyDealCalculatedWholeDays;
                PlayerPrefs.SetFloat("LastDayDailyDealCalculated", GameManager_script.Instance().LastDayDailyDealCalculated);

                // house keeping more
                GameManager_script.Instance().newDailyDealPrStarterPackageSeen = 0; // not seen
                PlayerPrefs.SetInt("newDailyDealPrStarterPackageSeen", 0);

                GameManager_script.Instance().RefreshMainMenuPage();
            }

            dailyCounter = 0.0f;
        }
    }
}
