using System;
using UnityEngine;

public class EventDefeatedVillainUnderJorgenfist : EventDefeatedVillain
{
    [Tooltip("after boosting the current location with the set aside villain run this block")]
    public Block AfterBoostBlock;
    [Tooltip("the name of the effect when the first villain is set aside")]
    public StrRefType EffectName;

    private void DefeatedVillainUnderJorgenfist_Finish()
    {
        Effect effect = Scenario.Current.GetEffect(EffectType.Nameable);
        Card card = Campaign.Box.Draw(effect.source);
        Location.Current.Deck.Add(card, DeckPositionType.Top);
        this.AfterBoostBlock.Invoke();
        Event.Done();
    }

    public override bool IsAnimationPossible(AnimationType animation)
    {
        if (animation == AnimationType.Undefeated)
        {
            return false;
        }
        return true;
    }

    protected override bool IsEarlyEndGameNecessary(Card card)
    {
        Effect effect = Scenario.Current.GetEffect(EffectType.Nameable);
        return (((effect != null) && (effect.source == card.ID)) && base.IsEarlyEndGameNecessary(card));
    }

    public override void OnCardDefeated(Card card)
    {
        Effect effect = Scenario.Current.GetEffect(EffectType.Nameable);
        if (effect == null)
        {
            Turn.CloseType = CloseType.Villain;
            Effect effect2 = new EffectNameable(card.ID, Effect.DurationPermament, this.EffectName.id);
            Scenario.Current.ApplyEffect(effect2);
            Scenario.Current.ResetTemporaryLocationClosures();
            Campaign.Box.Push(card, true);
            card.Disposition = DispositionType.SetAside;
            Turn.PushStateDestination(GameStateType.Done);
            Turn.PushStateDestination(GameStateType.Recharge);
            Turn.PushStateDestination(GameStateType.Closing);
            Turn.State = GameStateType.Dispose;
        }
        else if ((effect.source == card.ID) || (Scenario.Current.GetNumOpenLocations() > 1))
        {
            base.OnCardDefeated(card);
        }
        else
        {
            Scenario.Current.ResetTemporaryLocationClosures();
            card.Disposition = DispositionType.Banish;
            Turn.PushStateDestination(new TurnStateCallback(card, "DefeatedVillainUnderJorgenfist_Finish"));
            Turn.State = GameStateType.Dispose;
        }
    }
}

