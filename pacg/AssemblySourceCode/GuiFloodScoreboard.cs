using System;
using UnityEngine;

public class GuiFloodScoreboard : GuiButton
{
    [Tooltip("text number displaying our score")]
    public GuiLabel AlliesScore;
    [Tooltip("text number displaying the enemy's score")]
    public GuiLabel EnemyScore;

    public override void Refresh()
    {
        base.Refresh();
        this.UpdateLabels();
    }

    protected override void Start()
    {
        base.Start();
        this.UpdateLabels();
    }

    private void UpdateLabels()
    {
        Effect effect = Scenario.Current.GetEffect(EffectType.Nameable);
        if (effect == null)
        {
            this.EnemyScore.Text = 0.ToString();
        }
        else
        {
            this.EnemyScore.Text = (effect.sources.Count - 1).ToString();
        }
        effect = Scenario.Current.GetEffect(EffectType.AcquiredOutOfTotal);
        if ((effect == null) || (effect.source == null))
        {
            this.AlliesScore.Text = 0.ToString();
        }
        else
        {
            this.AlliesScore.Text = effect.sources.Count.ToString();
        }
    }
}

