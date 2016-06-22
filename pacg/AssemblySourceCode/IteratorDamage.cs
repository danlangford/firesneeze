using System;

public class IteratorDamage : TurnStateIterator
{
    public override void End()
    {
        base.End();
        Turn.SwitchType = SwitchType.None;
        Turn.DamageTargetType = DamageTargetType.Player;
        if (Turn.Character.Location != Location.Current.ID)
        {
            Location.Load(Turn.Character.Location);
        }
        if (Event.Finished())
        {
            if ((Turn.CombatStage == TurnCombatStageType.PreEncounter) || (Turn.CombatStage == TurnCombatStageType.None))
            {
                Turn.PushStateDestination(GameStateType.Combat);
            }
            else if (Turn.CombatStage == TurnCombatStageType.Done)
            {
                Turn.PushStateDestination(GameStateType.Done);
            }
            else
            {
                Turn.PushStateDestination(GameStateType.Post);
            }
            Turn.State = GameStateType.Recharge;
        }
        Turn.DamageTargetType = DamageTargetType.Player;
        Turn.Damage = 0;
        Turn.DamageTraits.Clear();
        Turn.DamageReduction = false;
        Turn.PriorityCardType = CardType.None;
        Turn.DiceBonus = 0;
        Event.Done();
    }

    public override void Invoke()
    {
        Turn.Current = Turn.Number;
        Location.Load(Turn.Character.Location);
        base.RefreshLocationWindow();
        Turn.Damage = Turn.DamageTargetAmount;
        Turn.DamageReduction = true;
        if (Turn.Damage > 0)
        {
            VisualEffectType cardLoseVfx = Rules.GetCardLoseVfx(Turn.DamageTraits);
            UI.Sound.Play(cardLoseVfx.ToSoundtype());
            VisualEffect.ApplyToPlayer(cardLoseVfx, 1.3f);
            Turn.Card.Animate(AnimationType.Attack, true);
        }
        if (Turn.CombatStage == TurnCombatStageType.PreEncounter)
        {
            Turn.State = GameStateType.Ambush;
        }
        else
        {
            Turn.State = GameStateType.Damage;
        }
    }

    public override bool IsValid()
    {
        if ((Turn.DamageTargetType == DamageTargetType.Location) && (Location.CountCharactersAtLocation(Location.Current.ID) <= 1))
        {
            return false;
        }
        if (Party.CountLivingMembers() <= 0)
        {
            return false;
        }
        return base.IsValid();
    }

    public override bool Next() => 
        (((Turn.DamageTargetType == DamageTargetType.Location) && base.NextCharacterAtLocation(Location.Current.ID)) || ((Turn.DamageTargetType == DamageTargetType.Party) && base.NextCharacterInParty()));

    public override void Start()
    {
        base.Start();
    }

    public override TurnStateIteratorType Type =>
        TurnStateIteratorType.Damage;
}

