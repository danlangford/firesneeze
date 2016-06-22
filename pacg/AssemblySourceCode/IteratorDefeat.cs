using System;

public class IteratorDefeat : TurnStateIterator
{
    public override void End()
    {
        base.End();
        Turn.EncounterType = EncounterType.None;
        Turn.SwitchType = SwitchType.None;
        Turn.DamageTargetType = DamageTargetType.Player;
        Turn.CombatStage = TurnCombatStageType.Encounter;
        Turn.PushStateDestination(GameStateType.Post);
        Turn.State = GameStateType.Recharge;
    }

    public override void Invoke()
    {
        Turn.Current = Turn.Number;
        base.RefreshLocationWindow();
        Turn.ClearCheckData();
        Turn.ClearCombatData();
        Turn.ClearEncounterData();
        Turn.PushStateDestination(GameStateType.Combat);
        Turn.State = GameStateType.Recharge;
    }

    public override bool Next() => 
        base.NextCharacterAtLocation(Location.Current.ID);

    public override void Start()
    {
        base.Start();
    }

    public override TurnStateIteratorType Type =>
        TurnStateIteratorType.Defeat;
}

