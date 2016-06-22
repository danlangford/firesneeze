using System;
using UnityEngine;

public class GuiPanelRewardGold : GuiPanel
{
    [Tooltip("the continue button in our hierarchy")]
    public GuiButton ContinueButton;
    [Tooltip("the amount of gold given")]
    public GuiLabel GoldAmount;
    [Tooltip("reference to the animator in our hierarchy")]
    public Animator GoldAnimator;
    private int goldCurrentAmount;
    private int goldCurrentTally;
    [Tooltip("the player's current gold amount")]
    public GuiLabel GoldOwned;
    private int goldOwnedAmount;
    [Tooltip("the source of the gold: monsters, scenarios, adventure, etc")]
    public GuiLabel GoldSource;
    [Tooltip("the accumulating total amount of gold given (\"reminder\")")]
    public GuiLabel GoldTally;

    private int GetCurrentGold()
    {
        if (Game.Network.Connected && !Game.Network.OutOfDate)
        {
            return Game.Network.CurrentUser.Gold;
        }
        return 0;
    }

    private void GiveGold(int gp, string sourceText)
    {
        this.GoldSource.Text = sourceText;
        this.GoldAmount.Text = gp.ToString();
        this.goldCurrentAmount = gp;
        this.goldOwnedAmount = this.GetCurrentGold();
        this.GoldOwned.Text = this.goldOwnedAmount.ToString();
    }

    private void GiveGoldController()
    {
        this.GoldAnimator.SetTrigger("Start");
        Game.Network.GetScenarioGold(Scenario.Current.ID, Scenario.Current.Difficulty, delegate (int gold) {
            if (this.GoldSource != null)
            {
                string sourceText = "No network connection";
                if (gold > 0)
                {
                    sourceText = UI.Text(0x18c);
                }
                else if (gold == 0)
                {
                    sourceText = UI.Text(0x25d);
                }
                else
                {
                    gold = 0;
                }
                this.GiveGold(gold, sourceText);
            }
        });
    }

    private void OnAnimationEventDone()
    {
        this.ContinueButton.Rebind();
    }

    private void OnAnimationEventFinished()
    {
        this.Show(false);
        UI.Window.SendMessage("RewardSequenceController");
    }

    private void OnAnimationEventGiveGold()
    {
        this.goldCurrentTally += this.goldCurrentAmount;
        this.goldOwnedAmount += this.goldCurrentAmount;
        this.GoldTally.Text = this.goldCurrentTally.ToString();
        this.GoldOwned.Text = this.goldOwnedAmount.ToString();
        UI.Sound.Play(SoundEffectType.MajorGold);
        this.GoldAnimator.SetBool("AddMore", false);
    }

    private void OnContinueButtonPushed()
    {
        this.GoldAnimator.SetTrigger("Continue");
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            this.GiveGoldController();
        }
    }

    public override bool Fullscreen =>
        true;
}

