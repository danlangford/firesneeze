using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LicenseIcon : MonoBehaviour
{
    [Tooltip("reference to the background sprite in our hierarchy")]
    public SpriteRenderer Background;
    [Tooltip("sound played when this icon is touched")]
    public AudioClip ClickSound;
    [Tooltip("the world-space height of this icon")]
    public float Height = 2f;
    [Tooltip("reference to the image in our hierarchy")]
    public GuiImage Image;
    [Tooltip("reference to the title in our hierarchy")]
    public GuiLabel TitleLabel;

    public static LicenseIcon Create(LicenseTableEntry entry)
    {
        string path = "Blueprints/Gui/License_Icon";
        GameObject original = Resources.Load<GameObject>(path);
        if (original != null)
        {
            GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(original);
            if (obj3 != null)
            {
                obj3.name = original.name;
                LicenseIcon component = obj3.GetComponent<LicenseIcon>();
                if (component != null)
                {
                    component.TitleLabel.Text = entry.Name;
                }
                return component;
            }
        }
        return null;
    }

    public void LoadImage(string id)
    {
        string path = "Blueprints/Icons/Licenses/" + id;
        SpriteRenderer renderer = Game.Cache.Get<SpriteRenderer>(path);
        if (renderer != null)
        {
            this.Image.Image = renderer.sprite;
        }
    }

    public void SetBackground()
    {
        GuiWindow current = GuiWindow.Current;
        if (current is GuiWindowStore)
        {
            GuiWindowStore store = current as GuiWindowStore;
            if (store != null)
            {
                GuiWindowStore.StorePanelType licensePanelType = LicenseManager.GetLicensePanelType(this.ID);
                switch (licensePanelType)
                {
                    case GuiWindowStore.StorePanelType.Gold:
                        if (this.ID == Constants.IAP_LICENSE_GOLD_SUBSCRIPTION_TIER1)
                        {
                            if (Game.Network.CurrentUser.GoldSubDaysRemaining > 0)
                            {
                                this.Background.color = store.ColorOwned;
                            }
                            else if (!LicenseManager.GetIsAvailable(this.ID, true))
                            {
                                this.Background.color = store.ColorUnavailable;
                            }
                            else
                            {
                                this.Background.color = store.ColorAvailable;
                            }
                        }
                        else if (!LicenseManager.GetIsAvailable(this.ID, true))
                        {
                            this.Background.color = store.ColorUnavailable;
                        }
                        else
                        {
                            this.Background.color = store.ColorAvailable;
                        }
                        return;

                    case GuiWindowStore.StorePanelType.Specials:
                        if (LicenseManager.GetIsLicensed(this.ID))
                        {
                            this.Background.color = store.ColorOwned;
                        }
                        else if (!LicenseManager.GetIsAvailable(this.ID, true))
                        {
                            this.Background.color = store.ColorUnavailable;
                        }
                        else
                        {
                            this.Background.color = store.ColorAvailable;
                        }
                        if (this.ID.ToLower().Equals(Constants.IAP_LICENSE_DUMMY))
                        {
                            this.Background.color = store.ColorAvailable;
                        }
                        return;
                }
                if (licensePanelType == GuiWindowStore.StorePanelType.Treasure_Buy)
                {
                    if (!LicenseManager.GetIsAvailable(this.ID, false))
                    {
                        this.Background.color = store.ColorUnavailable;
                    }
                    else
                    {
                        this.Background.color = store.ColorAvailable;
                    }
                }
                else
                {
                    if (LicenseManager.GetIsLicensed(this.ID))
                    {
                        this.Background.color = store.ColorOwned;
                    }
                    else if (!LicenseManager.GetIsAvailable(this.ID, false))
                    {
                        this.Background.color = store.ColorUnavailable;
                    }
                    else
                    {
                        this.Background.color = store.ColorAvailable;
                    }
                    if (this.ID.ToLower().Equals(Constants.IAP_LICENSE_BUNDLE_ROTR))
                    {
                        if (LicenseManager.GetIsLicensed(this.ID))
                        {
                            this.Background.color = store.ColorOwned;
                        }
                        else if (!LicenseManager.GetIsAvailable(this.ID, true))
                        {
                            this.Background.color = store.ColorUnavailable;
                        }
                        else
                        {
                            this.Background.color = store.ColorAvailable;
                        }
                    }
                    else if (this.ID.ToLower().Equals(Constants.IAP_LICENSE_DUMMY))
                    {
                        this.Background.color = store.ColorAvailable;
                    }
                }
            }
        }
    }

    public void Show(bool isVisible)
    {
        base.gameObject.SetActive(isVisible);
    }

    public void Tap()
    {
        UI.Sound.Play(this.ClickSound);
    }

    public string ID { get; set; }
}

