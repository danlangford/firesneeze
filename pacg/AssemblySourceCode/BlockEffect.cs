using System;
using UnityEngine;

public abstract class BlockEffect : Block
{
    [Tooltip("if not empty will overwrite the effect's source")]
    public string CustomSource;
    [Tooltip("number of turns")]
    public int Duration = 1;
    [Tooltip("if true will set the duration to the infinite sentinal")]
    public bool Permanent;
    [Tooltip("Filter to use for the effect applied")]
    public CardSelector Selector;
    [Tooltip("if true will get the current Card component as this effect's ID, else will get the Turn's current card. If there is no attached Card will default to current turn's card/custom source")]
    public bool SourceIsThisCard = true;
    [Tooltip("this effect applies to which characters?")]
    public DamageTargetType Target = DamageTargetType.Player;

    protected BlockEffect()
    {
    }

    private Effect CreateEffect(string sourceID) => 
        this.CreateEffect(sourceID, this.Duration, (this.Selector != null) ? this.Selector.ToFilter() : CardFilter.Empty);

    protected abstract Effect CreateEffect(string source, int duration, CardFilter filter);
    private string GetSourceID()
    {
        string iD;
        if (this.SourceIsThisCard && (base.Card != null))
        {
            iD = base.Card.ID;
        }
        else if (!string.IsNullOrEmpty(this.CustomSource))
        {
            iD = this.CustomSource;
        }
        else
        {
            iD = Turn.Card.ID;
        }
        if (Turn.CheckBoard.Get<string>("BlockPendingCard") != null)
        {
            iD = Turn.CheckBoard.Get<string>("BlockPendingCard");
        }
        return iD;
    }

    public override void Invoke()
    {
        if (this.Permanent)
        {
            this.Duration = Effect.DurationPermament;
        }
        string sourceID = this.GetSourceID();
        switch (this.Target)
        {
            case DamageTargetType.Player:
                Turn.Owner.ApplyEffect(this.CreateEffect(sourceID));
                break;

            case DamageTargetType.Location:
                for (int i = 0; i < Party.Characters.Count; i++)
                {
                    if (Party.Characters[i].Location == Location.Current.ID)
                    {
                        Party.Characters[i].ApplyEffect(this.CreateEffect(sourceID));
                    }
                }
                break;

            case DamageTargetType.Party:
                Scenario.Current.ApplyEffect(this.CreateEffect(sourceID));
                break;
        }
    }

    public void RemoveEffect()
    {
        string sourceID = this.GetSourceID();
        switch (this.Target)
        {
            case DamageTargetType.Player:
                Turn.Owner.RemoveEffect(sourceID);
                break;

            case DamageTargetType.Location:
                for (int i = 0; i < Party.Characters.Count; i++)
                {
                    if (Party.Characters[i].Location == Location.Current.ID)
                    {
                        Party.Characters[i].RemoveEffect(sourceID);
                    }
                }
                break;

            case DamageTargetType.Party:
                Scenario.Current.RemoveEffect(sourceID);
                break;
        }
    }
}

