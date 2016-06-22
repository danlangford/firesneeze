using System;

public class IteratorClose : TurnStateIterator
{
    public override void End()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutLocation.Show(true);
        }
        base.End();
        Turn.Iterators.Remove(TurnStateIteratorType.Close);
        Turn.CloseType = CloseType.None;
        Turn.SummonsType = SummonsType.None;
        Turn.EncounterType = EncounterType.None;
        Turn.CombatSkill = Turn.Character.GetCombatSkill();
        Location.Load(Turn.Character.Location);
        base.Message(null);
        GameStateVillain.Encounter(Turn.Card);
        base.RefreshLocationWindow();
    }

    public override void Invoke()
    {
        Turn.State = GameStateType.TempClose;
    }

    public override bool Next() => 
        (Turn.CloseType == CloseType.Temporary);

    public override void Start()
    {
        base.Start();
        Turn.CloseType = CloseType.Temporary;
        Turn.BlackBoard.Set<string>("VillainLocationID", Location.Current.ID);
        this.Invoke();
    }

    public override TurnStateIteratorType Type =>
        TurnStateIteratorType.Close;
}

