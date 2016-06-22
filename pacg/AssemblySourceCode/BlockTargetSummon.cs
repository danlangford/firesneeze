using System;
using UnityEngine;

public class BlockTargetSummon : Block
{
    [Tooltip("can you evade the summoning?")]
    public bool CanEvade;
    [Tooltip("the message displayed as you are targetting a victim to summon and encounter")]
    public StrRefType Message;
    [Tooltip("make the summon encounter for another random target character at this location")]
    public bool RandomTarget;
    [Tooltip("the monster to summon")]
    public SummonsSelector Summons;
    [Tooltip("the valid targets for the summon")]
    public TargetType TargetRange;

    private void BlockTargetSummon_Proceed()
    {
        Turn.EmptyLayoutDecks = true;
        Turn.SwitchCharacter(Turn.Target);
        Turn.Current = Turn.Number;
        Turn.Card.Show(false);
        Turn.State = GameStateType.Horde;
        Turn.EvadeDeclined = !this.CanEvade;
        Turn.Proceed();
    }

    private void BlockTargetSummon_Start()
    {
        if (this.RandomTarget)
        {
            string location = Turn.Owner.Location;
            if (this.TargetRange == TargetType.AnotherAtLocation)
            {
                Turn.Owner.Location = null;
            }
            int randomCharacterAtLocation = Location.GetRandomCharacterAtLocation(Location.Current.ID);
            Turn.Owner.Location = location;
            if (randomCharacterAtLocation >= 0)
            {
                Turn.Target = randomCharacterAtLocation;
                this.BlockTargetSummon_Proceed();
            }
        }
        else if (Rules.IsTargetRequired(this.TargetRange))
        {
            this.BlockTargetSummon_Target();
        }
        else
        {
            Turn.Target = Turn.Current;
            this.BlockTargetSummon_Proceed();
        }
    }

    private void BlockTargetSummon_Target()
    {
        Turn.EmptyLayoutDecks = false;
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "BlockTargetSummon_Proceed"));
        Turn.TargetType = this.TargetRange;
        GameStateTarget.DisplayText = this.Message.ToString();
        Turn.State = GameStateType.Target;
        Turn.EmptyLayoutDecks = true;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(false);
        }
    }

    public override void Invoke()
    {
        if (this.IsValid(Turn.Card))
        {
            Turn.SummonsType = SummonsType.Target;
            Turn.SummonsMonster = SummonsSelector.GetSummonsMonster(this.Summons.ID);
            this.BlockTargetSummon_Start();
        }
    }

    private bool IsValid(Card card)
    {
        if (Rules.IsCardSummons(card))
        {
            return false;
        }
        if ((this.RandomTarget && (this.TargetRange == TargetType.AnotherAtLocation)) && (Location.CountCharactersAtLocation(Location.Current.ID) <= 1))
        {
            return false;
        }
        return true;
    }

    public override bool Stateless =>
        false;
}

