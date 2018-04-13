using UnityEngine;
using System.Collections;

public class UIBottomRight : MonoBehaviour
{
    public UIPanel RootPanel;
    public UISprite Profile;
    public UISprite ProfileBackgound;
    public UISprite ProfileMask;
    public UISprite Shop;
    public UISprite Setting;
    public UISprite ShopMask;
    public UISprite ProfileImage;
    public UISprite ShopImage;
    public GameObject ProfileParent;
    public UISprite Star;
    public UISprite ProfileSlider;
    
    public UILabel ProfileLabel;
    public UILabel ShopLabel;
    public int distance = 5;
    public float setheight = 0.09f;
    public float setwidth = 5;
    public float widthhalf = 0.5f;

	public UISprite Advertising;//“观看广告获得金币”的按钮
	public UILabel AdLabel;//"免费获得金币"
	public UISprite adMoneySprite;//"免费获得金币"的钱币图标
  
    void Start()
    {
        UI();

        ChangeHeadImage();
        ChangeProfileName();
        ChangeProfileSlider();

        GameManager_script.Instance().uiBottomRight = this;
    }

    void UI()
    {
        Profile.height = (int)(RootPanel.height * setheight);
        Profile.width =(int) (Profile.height * 4.3f); // 4.3f is the button ratio

        if ((Profile.width * 2) >= RootPanel.width * widthhalf)//如果按钮加起来超过屏幕一半
        {
            Profile.width = (int)(RootPanel.width * widthhalf) / 2;
            Profile.height = (int)(Profile.width / 4.3f);
        }
        
        float Distance = Profile.height / distance;

        Profile.transform.localPosition = new Vector3(Profile.transform.localPosition.x - Distance, Profile.transform.localPosition.y + Distance, Profile.transform.localPosition.z);

        ProfileBackgound.width = Profile.width;
        ProfileBackgound.height = Profile.height;
        ChangeProfileViolatorCount(GameManager_script.GetProfileIconViolatorCount());

        Setting.width = Profile.height;
        Setting.height = Profile.height;
        Setting.transform.localPosition = new Vector3(Setting.transform.localPosition.x + Distance, Setting.transform.localPosition.y + Distance, Setting.transform.localPosition.z);
        Setting.GetComponent<BoxCollider>().size = new Vector3(Setting.width + Setting.transform.localPosition.x + Distance, Setting.width + Setting.transform.localPosition.x + Distance, 0);
        ChangeSettingViolatorCount(GameManager_script.GetSettingIconViolatorCount());

        Shop.height = Profile.height;
        Shop.width = Profile.width;
        Shop.transform.localPosition = new Vector3(Profile.transform.localPosition.x - Profile.width - Distance * 2, Shop.transform.localPosition.y + Distance, Shop.transform.localPosition.z);
        ChangeShopViolatorCount(GameManager_script.GetShopIconViolatorCount());

        ProfileImage.height = (int)(Profile.width / 4.3f / 1.25f);
        ProfileImage.width = ProfileImage.height;
        ProfileImage.transform.localPosition = new Vector3(-Profile.width + ProfileImage.width + Profile.width / 29.0f, Profile.height / 9.5f, ProfileImage.transform.localPosition.z);

        ProfileSlider.height = Profile.height;
        ProfileSlider.width = Mathf.CeilToInt( Profile.width / 1.97f / 1.135f);
        ProfileSlider.transform.localPosition = new Vector3(-Profile.width / 1.97f, 0, ProfileImage.transform.localPosition.z);

        ProfileLabel.height = Mathf.CeilToInt(Profile.height * 0.5f);
        ProfileLabel.width = Mathf.CeilToInt(Profile.width * 0.5f);// Mathf.CeilToInt(Profile.height * 4.3f / 2);
        ProfileLabel.transform.localPosition = new Vector3(-Profile.width * 0.285f, Profile.height / 2 - ProfileLabel.height / 2, 0);

        Star.height = Mathf.CeilToInt(Profile.height * 0.75f);
        Star.width = Mathf.CeilToInt(Profile.height * 0.75f);// Mathf.CeilToInt(Profile.height * 4.3f / 2);
        Star.transform.localPosition = new Vector3(-Profile.width / 1.87f - Star.width/2, Profile.height / 2, 0);
        Star.GetComponent<UIStar>().Text.width = GameManager_script.Instance().StarTextFineTuneWidth(Star.width, GameManager_script.Instance().CurrentLevel);
        Star.GetComponent<UIStar>().Text.height = GameManager_script.Instance().StarTextFineTuneHeight(Star.height, GameManager_script.Instance().CurrentLevel);
        Star.GetComponent<UIStar>().Text.transform.localPosition = GameManager_script.Instance().StarTextFineTunePosition(Star.GetComponent<UIStar>().Text.transform.localPosition, Star.GetComponent<UIStar>().Text.width, Star.GetComponent<UIStar>().Text.height, GameManager_script.Instance().CurrentLevel);
        Star.GetComponent<UIStar>().StartUI(new StarInfo((int)GameManager_script.Instance().CurrentLevel, GameManager_script.Instance().GetSelfStarType()));

        ShopImage.height = (int)(Shop.width / 4.3f / 1.25f);
        ShopImage.width = ShopImage.height;
        ShopImage.transform.localPosition = new Vector3(-Shop.width + ShopImage.width + Shop.width / 29.0f, Shop.height / 9.5f, ShopImage.transform.localPosition.z);

        ShopLabel.height = (int)(Shop.height * 0.6f);
        ShopLabel.width = (int)(Shop.width * 0.68f);
        ShopLabel.transform.localPosition = new Vector3(-Shop.width * 0.375f, Shop.height * 0.45f, 0.0f);
        ShopLabel.text = Localization.Get("WindowTitleShopMain");
        ChangeProfileSlider();

        ProfileMask.height = Profile.height;
        ProfileMask.width = Profile.width;
        ProfileMask.GetComponent<BoxCollider>().size = new Vector3(ProfileMask.width + Distance + Setting.transform.localPosition.x, ProfileMask.height + Distance + Setting.transform.localPosition.x, 0);

        ShopMask.height = Shop.height;
        ShopMask.width = Shop.width;
        ShopMask.GetComponent<BoxCollider>().size = new Vector3(ShopMask.width + Distance + Setting.transform.localPosition.x, ShopMask.height + Distance + Setting.transform.localPosition.x, 0);
   
		///免费获得金币
		Advertising.height = Profile.height;
		Advertising.width = Profile.width / 2;
		Advertising.transform.localPosition = new Vector3 (Profile.transform.localPosition.x - Profile.width * 2.5f, Setting.transform.localPosition.y + Distance * 3.6f, Advertising.transform.localPosition.z);

		adMoneySprite.transform.localPosition = new Vector3 (Advertising.transform.localPosition.x/15,0,0);

		AdLabel.text = "Get Free Coins";

	}

    public void ChangeProfileViolatorCount(int inCounter)
    {
        if (inCounter == 0)
        {
            Profile.GetComponent<Violator>().HideUIViolator();
        }
        else
        {
            Profile.GetComponent<Violator>().ShowUIViolator(inCounter);
        }
    }

    public void ChangeShopViolatorCount(int inCounter)
    {
        if (inCounter == 0)
        {
            Shop.GetComponent<Violator>().HideUIViolator();
        }
        else
        {
            Shop.GetComponent<Violator>().ShowUIViolator(inCounter);
        }
    }

    public void ChangeSettingViolatorCount(int inCounter)
    {
        if (inCounter == 0)
        {
            Setting.GetComponent<Violator>().HideUIViolator();
        }
        else
        {
            Setting.GetComponent<Violator>().ShowUIViolator(inCounter);
        }
    }

    public void ChangeHeadImage()
    {
        ProfileImage.spriteName = "" + GameManager_script.Instance().AvatarEquipped;
    }

    public void ChangeProfileSlider()
    {
        float count = Mathf.Clamp(GameManager_script.Instance().CurrentLevelExperience * 100.0f, 0, 101);

        ProfileParent.GetComponent<UISlider>().value = (count / 100.0f);
    }

    public void ChangeProfileName()
    {
        ProfileLabel.text = GameManager_script.Instance().CharLength(GameManager_script.Instance().First_Name, GameManager_script.Instance().Max_Name_Length);
    }
}
