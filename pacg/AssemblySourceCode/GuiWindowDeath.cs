using System;
using UnityEngine;

public class GuiWindowDeath : GuiWindow
{
    [Tooltip("art in our hierarchy shown for losing a game against the flood")]
    public GameObject artForFlood;
    [Tooltip("art in our hierarchy shown for any other reason")]
    public GameObject artForOther;
    [Tooltip("art in our hierarchy shown when the entire party is dead")]
    public GameObject artForPartyWipe;
    [Tooltip("art in our hierarchy shown when the blessing deck is empty")]
    public GameObject artForTimeout;

    protected override void Awake()
    {
        base.Awake();
        this.HideAllCards();
        if (Party.CountLivingMembers() == 0)
        {
            this.artForPartyWipe.SetActive(true);
        }
        else if (Scenario.Current.GetEffect(EffectType.AcquiredOutOfTotal) != null)
        {
            this.artForFlood.SetActive(true);
        }
        else if (Scenario.Current.Blessings.Count <= 0)
        {
            this.artForTimeout.SetActive(true);
        }
        else
        {
            this.artForOther.SetActive(true);
        }
    }

    private void HideAllCards()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            for (int j = 0; j < Party.Characters[i].Hand.Count; j++)
            {
                Party.Characters[i].Hand[j].Show(false);
            }
        }
    }

    private void OnExitButtonPushed()
    {
        Scenario.Current.Exit();
    }

    public override WindowType Type =>
        WindowType.Death;
}

