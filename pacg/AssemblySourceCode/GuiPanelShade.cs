using System;
using UnityEngine;

public class GuiPanelShade : GuiPanel
{
    [Tooltip("cached reference to our location shade sprite renderers. Shade highlights the location power events proceed button (Shadow Clock)")]
    public SpriteRenderer[] LocationShade;
    [Tooltip("cached reference to our partyoverlay shade sprite renderers")]
    public SpriteRenderer[] PartyOverlayShade = new SpriteRenderer[3];
    [Tooltip("cached reference to our target shade sprite renderer")]
    public SpriteRenderer[] TargetShade = new SpriteRenderer[1];
    [Tooltip("cached reference to our turn shade sprite renderer")]
    public SpriteRenderer[] TurnShade = new SpriteRenderer[1];

    public void Hide()
    {
        for (int i = 0; i < this.TurnShade.Length; i++)
        {
            this.TurnShade[i].enabled = false;
        }
        for (int j = 0; j < this.TargetShade.Length; j++)
        {
            this.TargetShade[j].enabled = false;
        }
    }

    public override void Initialize()
    {
        this.Hide();
    }

    public override void Refresh()
    {
        if (Turn.Number != Turn.Current)
        {
            this.Show(this.TurnShade, true, 1f, 0.25f);
        }
        else if (Turn.State == GameStateType.Target)
        {
            this.Show(this.TargetShade, true, 1f, 0.25f);
        }
        else if (Turn.State == GameStateType.Proceed)
        {
            this.Show(this.LocationShade, true, 1f, 0.25f);
        }
        else
        {
            this.Show(this.TurnShade, false, 0f, 0.25f);
            this.Show(this.TargetShade, false, 0f, 0.25f);
            this.Show(this.LocationShade, false);
        }
    }

    public void Show(SpriteRenderer[] shades, bool visible)
    {
        for (int i = 0; i < shades.Length; i++)
        {
            shades[i].enabled = visible;
        }
    }

    public void Show(SpriteRenderer[] shades, bool visible, float opacity, float tweenTime)
    {
        for (int i = 0; i < shades.Length; i++)
        {
            if (visible)
            {
                LeanTween.cancel(shades[i].gameObject, false);
                if (!shades[i].enabled)
                {
                    shades[i].enabled = true;
                    LeanTween.alpha(shades[i].gameObject, opacity, tweenTime);
                }
            }
            else if (shades[i].enabled)
            {
                LeanTween.cancel(shades[i].gameObject, false);
                LeanTween.alpha(shades[i].gameObject, opacity, tweenTime).setOnComplete(new Action(this.Hide));
            }
        }
    }
}

