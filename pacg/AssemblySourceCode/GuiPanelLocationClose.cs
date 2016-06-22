using System;
using UnityEngine;

public class GuiPanelLocationClose : GuiPanel
{
    [Tooltip("reference to the location icon in this panel")]
    public GuiImage ClosingIcon;
    [Tooltip("reference to the closing text label in this panel")]
    public GuiLabel ClosingTextLabel;
    [Tooltip("reference to the yes button in this panel which is disabled if the location is impossible to close")]
    public GuiButton YesButton;

    private void OnNoButtonPushed()
    {
        this.Show(false);
        Location.Current.OnLocationCloseAttempted();
        if ((Turn.CloseType == CloseType.Temporary) || (Turn.CloseType == CloseType.CloseInsideTempClose))
        {
            Turn.Proceed();
        }
        else
        {
            if (Turn.Close)
            {
                Turn.CloseType = CloseType.None;
            }
            Turn.Close = false;
        }
        if (Turn.State == GameStateType.ClosePrompt)
        {
            Turn.Proceed();
        }
    }

    private void OnYesButtonPushed()
    {
        if (Location.Current.IsClosePossible(Turn.Owner))
        {
            this.Show(false);
            Turn.State = GameStateType.Close;
        }
    }

    public void Show(CloseType closeType)
    {
        Turn.CloseType = closeType;
        this.Show(closeType != CloseType.None);
        if (!Location.Current.IsClosePossible(Turn.Character) && (closeType != CloseType.Temporary))
        {
            Turn.Close = false;
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.locationPanel.GlowLocationClosePossible(Location.Current.IsClosePossible(Turn.Owner), closeType != CloseType.None);
            if (!Location.Current.IsClosePossible(Turn.Owner) && (closeType != CloseType.None))
            {
                window.messagePanel.Show(StringTableManager.GetHelperText(0x85));
            }
            else
            {
                window.messagePanel.Clear();
            }
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            this.ClosingTextLabel.Text = Location.Current.ClosingText;
            this.ClosingIcon.Image = Location.Current.Icon;
            Tutorial.Notify(TutorialEventType.ScreenCloseLocationShown);
            this.YesButton.Disable(!Location.Current.IsClosePossible(Turn.Owner));
            UI.Sound.Play(SoundEffectType.CloseLocPromptAppear);
        }
        else
        {
            this.ClosingTextLabel.Clear();
        }
        this.ShowVillain(isVisible);
    }

    private void ShowVillain(bool isVisible)
    {
        if (Turn.CloseType == CloseType.Temporary)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                if (isVisible)
                {
                    if (Scenario.Current.IsCurrentVillain(Turn.Card.ID))
                    {
                        window.layoutSummoner.Show(Turn.Card.ID);
                    }
                    else
                    {
                        window.layoutSummoner.Show(Scenario.Current.Villain);
                    }
                }
                else
                {
                    window.layoutSummoner.Clear();
                }
            }
        }
    }

    public override uint zIndex =>
        Constants.ZINDEX_PANEL_POPUP;
}

