using UnityEngine;
using System.Collections;

public class UILanguagePanel : MonoBehaviour
{
    public Vector2 BackgroundXY;
    public UIPanel WindowPanel; 
    public UILabel Title;
    public UISprite BoxBackground;
    public GameObject[] LanguageButtonPrefabs;

    public void LanguageUI()
    {
        BoxBackground.height = (int)(BackgroundXY.y * 0.90f);
        BoxBackground.width = (int)(BackgroundXY.x * 0.9385f);
        BoxBackground.transform.localPosition = new Vector3(BackgroundXY.x * -0.0025f, 0.0f, 0.0f);
        BoxBackground.GetComponent<BoxCollider>().size = new Vector3(BoxBackground.width, BoxBackground.height, 0);

        //标题
        Title.width = (int)(BoxBackground.width * 0.95f);
        Title.height = (int)(BoxBackground.height * 0.10f);
        Title.transform.localPosition = new Vector3(0, BoxBackground.height * 0.475f - Title.height * 0.55f, 0);
        Title.text = Localization.Get("WindowSettingSelectLanguage");

        for (int i = 0; i < LanguageButtonPrefabs.Length; i++)
        {
            LanguageButtonPrefabs[i].name = i.ToString("D20");
            
            LanguageButtonPrefabs[i].transform.localScale = new Vector3(1, 1, 1);
            LanguageButtonPrefabs[i].GetComponent<LanguageButton>().buttonId = i;
            LanguageButtonPrefabs[i].GetComponent<LanguageButton>().WindowPanel = WindowPanel;
            LanguageButtonPrefabs[i].GetComponent<LanguageButton>().Background.width = (int)(BoxBackground.width * 0.225f);
            LanguageButtonPrefabs[i].GetComponent<LanguageButton>().Background.height = (int)(LanguageButtonPrefabs[i].GetComponent<LanguageButton>().Background.width * 0.6660f);
			///-------GU-------
            if (i == 0)
            {
				LanguageButtonPrefabs[i].GetComponent<LanguageButton>().ButtonName.text = "English";
                LanguageButtonPrefabs[i].transform.localPosition = new Vector3(BoxBackground.width * -0.32f, BoxBackground.height * 0.20f, 0);
            }
            else if (i == 1)
            {
				LanguageButtonPrefabs[i].GetComponent<LanguageButton>().ButtonName.text = "Français";
                LanguageButtonPrefabs[i].transform.localPosition = new Vector3(BoxBackground.width * 0.00f, BoxBackground.height * 0.20f, 0);
            }
			///----20170310-----
            else if (i == 2)
            {
				LanguageButtonPrefabs[i].GetComponent<LanguageButton>().ButtonName.text = "Deutsch";
				LanguageButtonPrefabs[i].transform.localPosition = new Vector3(BoxBackground.width * 0.32f, BoxBackground.height * 0.20f, 0);
            }
			///------GU-添加代码，想要实现：增加语言选项的功能-----
			else if (i == 3)
			{
				LanguageButtonPrefabs[i].GetComponent<LanguageButton>().ButtonName.text = "Español";
				LanguageButtonPrefabs[i].transform.localPosition = new Vector3(BoxBackground.width * -0.32f, BoxBackground.height * -0.20f, 0);
			}
			else if (i == 4)
			{
				LanguageButtonPrefabs[i].GetComponent<LanguageButton>().ButtonName.text = "简体中文";
				LanguageButtonPrefabs[i].transform.localPosition = new Vector3(BoxBackground.width * -0.00f, BoxBackground.height * -0.20f, 0);
			}
			else if (i == 5)
			{
				LanguageButtonPrefabs[i].GetComponent<LanguageButton>().ButtonName.text = "日本語";
				LanguageButtonPrefabs[i].transform.localPosition = new Vector3(BoxBackground.width * 0.32f, BoxBackground.height * -0.20f, 0);
			}
			///-------20170309-------
            LanguageButtonPrefabs[i].GetComponent<LanguageButton>().UI();
        }
    }

    public void ChangeInsideDepth(int vDepth)
    {
        gameObject.GetComponent<UIPanel>().depth = (vDepth);
    }
}
