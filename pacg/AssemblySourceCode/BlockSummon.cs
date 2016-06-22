using System;
using UnityEngine;

public class BlockSummon : Block
{
    [Tooltip("the type of encounter this card performs")]
    public EncounterType EncounterType;
    [Tooltip("if true, the summons will immediately attack")]
    public bool Immediate = true;
    [Tooltip("does the summoned card goto the top or bottom of the deck?")]
    public DeckPositionType Position = DeckPositionType.Top;
    [Tooltip("defines the monster to summon")]
    public SummonsSelector Summons;

    private void BlockSummon_Invoke()
    {
        this.Invoke();
    }

    public override void Invoke()
    {
        Card card = this.Summons.Summon();
        if (card != null)
        {
            Location.Current.Deck.Add(card, this.Position);
            if (this.EncounterType != EncounterType.None)
            {
                Turn.EncounterType = this.EncounterType;
            }
            if (this.Immediate)
            {
                GuiWindowLocation window = UI.Window as GuiWindowLocation;
                if (window != null)
                {
                    window.layoutLocation.Show(true);
                    window.layoutLocation.Display();
                    Turn.State = GameStateType.Encounter;
                }
            }
        }
    }

    public override bool Stateless
    {
        get
        {
            if (this.Immediate)
            {
                return false;
            }
            return true;
        }
    }
}

