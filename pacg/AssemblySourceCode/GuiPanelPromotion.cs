using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelPromotion : GuiPanelBackStack
{
    [Tooltip("reference to the body label on this panel")]
    public GuiLabel BodyLabel;
    [Tooltip("reference to the close button on this panel")]
    public GuiButton CloseButton;
    [Tooltip("reference to the quit button on this panel")]
    public GuiButton CornerCloseButton;
    [Tooltip("reference to the quick store button on this panel")]
    public GuiButton GoToStoreButton;
    [Tooltip("reference to the header label on this panel")]
    public GuiLabel HeaderLabel;
    [Tooltip("reference to the image on this panel")]
    public GuiImage Image;

    private void OnBuyNowButtonPushed()
    {
        if (!UI.Busy)
        {
            this.Show(false);
            Game.UI.ShowStoreWindow(LicenseID, (LicenseType) ((int) Enum.Parse(typeof(LicenseType), LicenseType)));
        }
    }

    private void OnCloseButtonPushed()
    {
        if (!UI.Busy)
        {
            this.Show(false);
        }
    }

    public override void Rebind()
    {
        this.CornerCloseButton.Rebind();
        this.CloseButton.Rebind();
        this.GoToStoreButton.Rebind();
    }

    public override void Refresh()
    {
        base.Refresh();
        this.CloseButton.Show(LicenseManager.GetIsLicensed(LicenseID));
        this.GoToStoreButton.Show(!LicenseManager.GetIsLicensed(LicenseID));
        if ((ImagePath == null) || (ImagePath.Length == 0))
        {
            ImagePath = "Art/Adventures/AD1T_Tutorial";
        }
        if (HeaderID != 0)
        {
            this.HeaderLabel.Text = UI.Text(HeaderID);
        }
        else
        {
            this.HeaderLabel.Text = "SPECIAL";
        }
        if (BodyID != 0)
        {
            this.BodyLabel.Text = UI.Text(BodyID);
        }
        else
        {
            this.BodyLabel.Text = "Rise of the Runelords Bundle now available in the store!";
        }
        if (ImagePath.Length > 0)
        {
            GameObject obj2 = Resources.Load<GameObject>(ImagePath);
            if (obj2 != null)
            {
                SpriteRenderer componentInChildren = obj2.GetComponentInChildren<SpriteRenderer>();
                if (componentInChildren != null)
                {
                    this.Image.Image = componentInChildren.sprite;
                }
            }
        }
    }

    public override void Show(bool isVisible)
    {
        if (isVisible && Game.UI.NetworkTooltip.Visible)
        {
            Game.UI.NetworkTooltip.Show(false);
        }
        base.Show(isVisible);
        if (isVisible)
        {
            this.Refresh();
            UI.Busy = false;
        }
    }

    public static void Synchronize(Dictionary<string, string> data)
    {
        string str;
        if (data.TryGetValue("headerID", out str) && (str != null))
        {
            HeaderID = int.Parse(str);
        }
        if (data.TryGetValue("bodyID", out str) && (str != null))
        {
            BodyID = int.Parse(str);
        }
        if (data.TryGetValue("imagePath", out str) && (str != null))
        {
            ImagePath = str;
        }
        if (data.TryGetValue("licenseID", out str) && (str != null))
        {
            LicenseID = str;
        }
        if (data.TryGetValue("licenseType", out str) && (str != null))
        {
            LicenseType = str;
        }
        if (!PlayerPrefs.HasKey("PromoPanelHeader") || (PlayerPrefs.GetInt("PromoPanelHeader") != HeaderID))
        {
            PlayerPrefs.SetInt("PromoPanelHeader", HeaderID);
            GuiWindowMainMenu current = GuiWindow.Current as GuiWindowMainMenu;
            if (current != null)
            {
                if ((current.statusPanel != null) && current.statusPanel.Visible)
                {
                    current.statusPanel.Show(false);
                }
                if ((((current.promoPanel != null) && !current.promoPanel.Visible) && ((current.outOfDatePanel != null) && !current.outOfDatePanel.Visible)) && Settings.Debug.OnLoginPanels)
                {
                    current.promoPanel.Show(true);
                }
            }
        }
    }

    public static int BodyID
    {
        [CompilerGenerated]
        get => 
            <BodyID>k__BackingField;
        [CompilerGenerated]
        set
        {
            <BodyID>k__BackingField = value;
        }
    }

    public override bool Fullscreen =>
        true;

    public static int HeaderID
    {
        [CompilerGenerated]
        get => 
            <HeaderID>k__BackingField;
        [CompilerGenerated]
        set
        {
            <HeaderID>k__BackingField = value;
        }
    }

    public static string ImagePath
    {
        [CompilerGenerated]
        get => 
            <ImagePath>k__BackingField;
        [CompilerGenerated]
        set
        {
            <ImagePath>k__BackingField = value;
        }
    }

    public static string LicenseID
    {
        [CompilerGenerated]
        get => 
            <LicenseID>k__BackingField;
        [CompilerGenerated]
        set
        {
            <LicenseID>k__BackingField = value;
        }
    }

    public static string LicenseType
    {
        [CompilerGenerated]
        get => 
            <LicenseType>k__BackingField;
        [CompilerGenerated]
        set
        {
            <LicenseType>k__BackingField = value;
        }
    }
}

