using System;
using UnityEngine;

public class GuiPanelSettings : GuiPanelBackStack
{
    [Tooltip("reference to the logged in account label on this panel")]
    public GuiLabel AccountLabel;
    [Tooltip("reference to the close button on this panel")]
    public GuiButton CloseButton;
    [Tooltip("reference to the use-collection-cards button on this panel")]
    public GuiButton CollectionCardsButton;
    [Tooltip("reference to the forum button on this panel")]
    public GuiButton ForumButton;
    [Tooltip("reference to the advanced graphics button on this panel")]
    public GuiButton GraphicsButton;
    [Tooltip("reference to the music slider on this panel")]
    public GuiSlider MusicSlider;
    [Tooltip("reference to the pass-and-play button on this panel")]
    public GuiButton PassAndPlayButton;
    [Tooltip("reference to the sound slider on this panel")]
    public GuiSlider SoundSlider;
    [Tooltip("reference to the tutorial messages button on this panel")]
    public GuiButton TutorialButton;
    [Tooltip("reference to the build version label on this panel")]
    public GuiLabel VersionLabel;
    [Tooltip("reference to the enter-voucher button on this panel")]
    public GuiButton VoucherButton;

    private void OnCloseButtonPushed()
    {
        Settings.Save();
        this.Show(false);
        if (UI.Window.Type != WindowType.MainMenu)
        {
            Game.UI.OptionsPanel.Show(true);
        }
    }

    private void OnCollectionCardsButtonPushed()
    {
        Settings.UseCollectionCardsInStoryMode = !Settings.UseCollectionCardsInStoryMode;
        this.Refresh();
    }

    private void OnEnterVoucherButtonPushed()
    {
        this.Show(false);
        Game.UI.VoucherPanel.Show(true);
    }

    private void OnGraphicsButtonPushed()
    {
        if (Settings.GraphicsLevel == 0)
        {
            Settings.GraphicsLevel = 1;
        }
        else
        {
            Settings.GraphicsLevel = 0;
        }
        if (UI.Window.Type == WindowType.MainMenu)
        {
            UI.Window.Refresh();
        }
        if (UI.Window.Type == WindowType.Location)
        {
            (UI.Window as GuiWindowLocation).statusPanel.Refresh();
        }
        this.Refresh();
    }

    private void OnMusicSliderMoved()
    {
        Settings.MusicVolume = this.MusicSlider.Value;
        Settings.Save();
        this.Refresh();
    }

    private void OnPassAndPlayButtonPushed()
    {
        if (Game.GameType == GameType.LocalMultiPlayer)
        {
            Game.GameType = GameType.LocalSinglePlayer;
            Turn.Switch = 0;
        }
        else
        {
            Game.GameType = GameType.LocalMultiPlayer;
            Turn.Switch = Turn.Number;
        }
        this.Refresh();
    }

    private void OnSoundSliderMoved()
    {
        Settings.Volume = this.SoundSlider.Value;
        Settings.Save();
        this.Refresh();
    }

    private void OnSupportButtonPushed()
    {
        if (!UI.Busy && !UI.Busy)
        {
            Application.OpenURL(Constants.SUPPORT_FORUMS_LINK);
        }
    }

    private void OnTutorialButtonPushed()
    {
        if (Settings.TutorialLevel > 0)
        {
            Settings.TutorialLevel = 0;
        }
        else
        {
            Settings.TutorialLevel = 1;
        }
        if (Settings.TutorialLevel >= 1)
        {
            Tutorial.Clear();
        }
        this.Refresh();
    }

    public override void Rebind()
    {
        this.CloseButton.Rebind();
        this.ForumButton.Rebind();
        this.TutorialButton.Rebind();
        this.PassAndPlayButton.Rebind();
        this.GraphicsButton.Rebind();
        this.CollectionCardsButton.Rebind();
        this.VoucherButton.Rebind();
        this.SoundSlider.Rebind();
        this.MusicSlider.Rebind();
    }

    public override void Refresh()
    {
        if (Settings.TutorialLevel > 0)
        {
            this.TutorialButton.Text = UI.Text(0x1f9);
            this.TutorialButton.Glow(true);
        }
        else
        {
            this.TutorialButton.Text = UI.Text(0x1fa);
            this.TutorialButton.Glow(false);
        }
        if (Game.GameType == GameType.LocalMultiPlayer)
        {
            this.PassAndPlayButton.Text = UI.Text(0x1f9);
            this.PassAndPlayButton.Glow(true);
        }
        else
        {
            this.PassAndPlayButton.Text = UI.Text(0x1fa);
            this.PassAndPlayButton.Glow(false);
        }
        if (Settings.GraphicsLevel > 0)
        {
            this.GraphicsButton.Text = UI.Text(0x1f9);
            this.GraphicsButton.Glow(true);
        }
        else
        {
            this.GraphicsButton.Text = UI.Text(0x1fa);
            this.GraphicsButton.Glow(false);
        }
        if (Settings.UseCollectionCardsInStoryMode)
        {
            this.CollectionCardsButton.Text = UI.Text(0x1f9);
            this.CollectionCardsButton.Glow(true);
        }
        else
        {
            this.CollectionCardsButton.Text = UI.Text(0x1fa);
            this.CollectionCardsButton.Glow(false);
        }
        this.SoundSlider.Value = Settings.Volume;
        this.MusicSlider.Value = Settings.MusicVolume;
        this.PassAndPlayButton.Disable(Tutorial.Running);
        this.TutorialButton.Disable(Tutorial.Running);
        this.VersionLabel.Text = "VER-" + Game.Instance.BuildNumber;
        this.AccountLabel.Text = "PFID-" + Game.Network.CurrentUser.Id;
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            this.Refresh();
        }
    }

    public override bool Fullscreen =>
        true;
}

