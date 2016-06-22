using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelSkills : GuiPanel
{
    [Tooltip("reference to the \"check type\" label in the art panel")]
    public GuiLabel CardCheckLabel;
    [Tooltip("reference to the \"check target value\" label in the art panel")]
    public GuiLabel CardTargetLabel;
    [Tooltip("reference to the \"number increase\" animator in the art panel")]
    public Animator CardTargetLabelAnimator;
    [Tooltip("reference to the \"card title\" label in the art panel")]
    public GuiLabel CardTitleLabel;
    [Tooltip("sprite for D10 dice")]
    public Sprite DiceImageD10;
    [Tooltip("sprite for D12 dice")]
    public Sprite DiceImageD12;
    [Tooltip("sprite for D4 dice")]
    public Sprite DiceImageD4;
    [Tooltip("sprite for D6 dice")]
    public Sprite DiceImageD6;
    [Tooltip("sprite for D8 dice")]
    public Sprite DiceImageD8;
    [Tooltip("reference to the dice panel in this window")]
    public GuiPanelDice DicePanel;
    private SkillCheckType lastSkillCheck;
    private int lastTargetValue;
    [Tooltip("reference to the animator for this panel")]
    public Animator PanelAnimator;
    [Tooltip("references to the small red circles beside each skill name")]
    public GuiImage[] SkillArt;
    [Tooltip("references to the bonus value labels on this panel")]
    public GuiLabel[] SkillBonusLabels;
    [Tooltip("references to the skill buttons on this panel")]
    public GuiButton[] SkillButtons;
    [Tooltip("references to the dice icon beside the skill")]
    public GuiImage[] SkillDice;
    [Tooltip("references to the skill name labels on this panel")]
    public GuiLabel[] SkillLabels;
    [Tooltip("references to the target number labels on this panel")]
    public GuiLabel[] SkillTargetLabels;
    private TKTapRecognizer tapRecognizer;

    private void Animate(bool isVisible)
    {
        if (isVisible)
        {
            this.PanelAnimator.SetTrigger("Open");
            UI.Sound.Play(SoundEffectType.SkillPanelExpand);
        }
        else
        {
            this.PanelAnimator.SetTrigger("Close");
            UI.Sound.Play(SoundEffectType.SkillPanelCollapse);
        }
    }

    public override void Clear()
    {
        this.lastSkillCheck = SkillCheckType.None;
        this.lastTargetValue = 0;
        this.CardTitleLabel.Clear();
        this.CardCheckLabel.Clear();
        this.CardTargetLabel.Clear();
    }

    private void GlowButton(GuiButton button, bool isGlowing)
    {
        Animator component = button.GetComponent<Animator>();
        if (component != null)
        {
            component.SetBool("Glow", isGlowing);
        }
    }

    public override void Initialize()
    {
        base.Show(false);
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.zIndex = 1;
        this.tapRecognizer.gestureRecognizedEvent += r => this.OnGuiTap(this.tapRecognizer.touchLocation());
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.tapRecognizer.enabled = false;
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        if (!this.Busy && !UI.Window.Paused)
        {
            this.Show(false);
            this.DicePanel.SkillsButton.Show(true);
        }
    }

    private void OnSkillButton0Pushed()
    {
        if (!UI.Window.Paused)
        {
            this.SwitchSkillCheck(0);
        }
    }

    private void OnSkillButton1Pushed()
    {
        if (!UI.Window.Paused)
        {
            this.SwitchSkillCheck(1);
        }
    }

    private void OnSkillButton2Pushed()
    {
        if (!UI.Window.Paused)
        {
            this.SwitchSkillCheck(2);
        }
    }

    private void OnSkillButton3Pushed()
    {
        if (!UI.Window.Paused)
        {
            this.SwitchSkillCheck(3);
        }
    }

    public void SetTitle(Card card, SkillCheckType skillType, int skillTarget)
    {
        if (card != null)
        {
            Turn.CheckName = card.DisplayName.ToUpper();
        }
        if (Turn.CheckName != null)
        {
            this.CardTitleLabel.Text = Turn.CheckName.ToUpper();
        }
        this.CardCheckLabel.Text = skillType.ToText().ToUpper();
        this.CardTargetLabel.Text = skillTarget.ToString();
        if (((this.lastTargetValue != 0) && (this.lastTargetValue != skillTarget)) && ((this.lastSkillCheck == SkillCheckType.None) || (this.lastSkillCheck == skillType)))
        {
            this.CardTargetLabelAnimator.SetInteger("Amount", skillTarget - this.lastTargetValue);
            this.CardTargetLabelAnimator.SetTrigger("Update");
        }
        this.lastTargetValue = skillTarget;
        this.lastSkillCheck = skillType;
    }

    private void SetupSkillButtons()
    {
        for (int i = 0; i < this.SkillButtons.Length; i++)
        {
            this.SkillButtons[i].Show(false);
        }
        for (int j = 0; j < this.SkillLabels.Length; j++)
        {
            this.SkillLabels[j].Show(false);
        }
        for (int k = 0; k < this.SkillArt.Length; k++)
        {
            this.SkillArt[k].Show(false);
        }
        for (int m = 0; m < this.SkillTargetLabels.Length; m++)
        {
            this.SkillTargetLabels[m].Show(false);
        }
        for (int n = 0; n < this.SkillBonusLabels.Length; n++)
        {
            this.SkillBonusLabels[n].Show(false);
        }
        for (int num6 = 0; num6 < this.SkillDice.Length; num6++)
        {
            this.SkillDice[num6].Show(false);
        }
        if (Turn.Checks != null)
        {
            for (int num7 = 0; num7 < Turn.Checks.Length; num7++)
            {
                this.SkillLabels[num7].Text = Turn.Checks[num7].skill.ToText();
                this.SkillLabels[num7].Show(true);
                this.SkillArt[num7].Show(true);
                this.SkillButtons[num7].Show(true);
                this.GlowButton(this.SkillButtons[num7], Turn.Check == Turn.Checks[num7].skill);
                this.SkillTargetLabels[num7].Text = Turn.Checks[num7].Rank.ToString();
                this.SkillTargetLabels[num7].Show(true);
                this.SkillBonusLabels[num7].Text = "+ " + (Rules.GetCheckBonus(Turn.Checks[num7].skill) + Rules.GetAbilityCheckBonus());
                this.SkillBonusLabels[num7].Show(true);
            }
        }
        if (Turn.Checks != null)
        {
            for (int num8 = 0; num8 < Turn.Checks.Length; num8++)
            {
                DiceType skillDice = Turn.Owner.GetSkillDice(Turn.Checks[num8].skill);
                if ((skillDice == DiceType.D0) && (Turn.Dice.Count > 0))
                {
                    skillDice = Turn.Dice[0];
                }
                if (skillDice == DiceType.D4)
                {
                    this.SkillDice[num8].Image = this.DiceImageD4;
                }
                if (skillDice == DiceType.D6)
                {
                    this.SkillDice[num8].Image = this.DiceImageD6;
                }
                if (skillDice == DiceType.D8)
                {
                    this.SkillDice[num8].Image = this.DiceImageD8;
                }
                if (skillDice == DiceType.D10)
                {
                    this.SkillDice[num8].Image = this.DiceImageD10;
                }
                if (skillDice == DiceType.D12)
                {
                    this.SkillDice[num8].Image = this.DiceImageD12;
                }
                this.SkillDice[num8].Show(true);
            }
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.SetupSkillButtons();
        this.tapRecognizer.enabled = isVisible;
        this.Animate(isVisible);
        if (isVisible)
        {
            LeanTween.delayedCall(0.15f, () => this.DicePanel.Scamper(new Vector2(0f, -2f), 0.25f));
        }
        else
        {
            this.DicePanel.Scamper(new Vector2(0f, 2f), 0.25f);
        }
        this.Busy = true;
        LeanTween.delayedCall(0.5f, (Action) (() => (this.Busy = false)));
    }

    private void SwitchSkillCheck(int n)
    {
        if ((Turn.Checks != null) && (Turn.Checks.Length > n))
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                if (((Turn.Checks[n].skill != Turn.Check) && !Turn.Defeat) && !Turn.Evade)
                {
                    UI.Sound.Play(SoundEffectType.SkillPanelSkillChange);
                }
                Card card = Turn.Card;
                if ((Turn.EncounteredGuid != card.GUID) || (Turn.EncounteredGuid == Guid.Empty))
                {
                    card = null;
                }
                window.dicePanel.SetCheck(card, Turn.Checks, Turn.Checks[n].skill);
                this.SetupSkillButtons();
            }
        }
    }

    public bool Busy { get; private set; }
}

