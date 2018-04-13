using UnityEngine;
using System.Collections;

public class SelectionLanguage : MonoBehaviour
{
    [System.NonSerialized]
    public Languge LangugeType;

    //语种下载状态//
    public enum Languge : byte
    {
        English = 0,
        Spanish,
        German,
        French,
        Chinese,
        Taiwan,
        Japanese
    }

    public void Start()
    {
        if (GameManager_script.Instance().Savelanguage != "")
        {
            // we have a language, good
            SetLanguageFromStrings(GameManager_script.Instance().Savelanguage);
        }
        else
        {
            // try to detect actual language of the machine
            string systemLang = Application.systemLanguage.ToString();
            string finalSystemLang = ConvertSystemLanguageIntoGameString(systemLang);

            SetLanguageFromStrings(finalSystemLang);
        }

        // this is kool, keep this
        SetStringsFromLanguage(LangugeType);
    }

    public string ConvertSystemLanguageIntoGameString(string inlang)
    {
        switch (inlang)
        {
            case "English": return "English";
            case "Spanish": return "Español";
            case "German": return "Deutsch";
            case "French": return "Français";
            case "Chinese": return "简体中文";
            case "Taiwan": return "繁體中文";
            case "Japanese": return "日本語";
            default: return "English";
        }
    }

    public void SetStringsFromLanguage(Languge inLang)
    {
        switch (inLang)
        {
            case Languge.English: Localization.language = "English"; break;
            case Languge.Spanish: Localization.language = "Español"; break;
            case Languge.German: Localization.language = "Deutsch"; break;
            case Languge.French: Localization.language = "Français"; break;
            case Languge.Chinese: Localization.language = "简体中文"; break;
            case Languge.Taiwan: Localization.language = "繁體中文"; break;
            case Languge.Japanese: Localization.language = "日本語"; break;
            default: Localization.language = "English"; break;
        }

        GameManager_script.Instance().Savelanguage = Localization.language;
        PlayerPrefs.SetString("Savelanguage", GameManager_script.Instance().Savelanguage);
    }

    public void SetLanguageFromStrings(string inLang)
    {
        switch (inLang)
        {
            case "English": LangugeType = Languge.English; break;
            case "Español": LangugeType = Languge.Spanish; break;
            case "Deutsch": LangugeType = Languge.German; break;
            case "Français": LangugeType = Languge.French; break;
            case "简体中文": LangugeType = Languge.Chinese; break;
            case "繁體中文": LangugeType = Languge.Taiwan; break;
            case "日本語": LangugeType = Languge.Japanese; break;
            default: LangugeType = Languge.English; break;
        }

        GameManager_script.Instance().Savelanguage = inLang;
        PlayerPrefs.SetString("Savelanguage", GameManager_script.Instance().Savelanguage);
    }
}
