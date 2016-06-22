using System;
using UnityEngine;

public class GuiPanelPhases : GuiPanel
{
    public Sprite ExploreDisabledSprite;
    public Sprite ExploreEnabledSprite;
    [Tooltip("reference to the explore sprite in this panel")]
    public SpriteRenderer ExploreIcon;
    private Animator myAnimator;
    private TurnPhaseType panelPhase;

    public override void Initialize()
    {
        this.myAnimator = base.GetComponent<Animator>();
        this.Set(Turn.Phase);
    }

    public void Set(TurnPhaseType phase)
    {
        if ((this.myAnimator != null) && (this.panelPhase != phase))
        {
            this.panelPhase = phase;
            this.myAnimator.SetInteger("Phase", (int) phase);
            this.myAnimator.SetTrigger("ChangePhase");
        }
        this.Exploring = phase == TurnPhaseType.Explore;
    }

    public bool Exploring
    {
        set
        {
            if (value)
            {
                this.ExploreIcon.sprite = this.ExploreEnabledSprite;
            }
            else
            {
                this.ExploreIcon.sprite = this.ExploreDisabledSprite;
            }
        }
    }
}

