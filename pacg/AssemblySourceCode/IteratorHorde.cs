using System;

public class IteratorHorde : TurnStateIterator
{
    public override void End()
    {
        base.End();
        Turn.OptionalTarget = TargetPanelType.None;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Turn.SummonsType = SummonsType.None;
            Turn.SummonsMonster = null;
            Turn.Summons = false;
            Turn.CombatSkill = Turn.Character.GetCombatSkill();
            Location.Load(Turn.Character.Location);
            window.layoutSummoner.Animate(ActionType.Banish);
            base.RefreshLocationWindow();
            if (base.HasPostEvent)
            {
                if (!Rules.IsEncounterInCurrentLocation())
                {
                    Location.Load(Turn.EncounteredLocation);
                    Turn.Card.OnPostHorde(Turn.Card);
                }
                else
                {
                    Turn.Card.OnPostHorde(Turn.Card);
                }
            }
        }
        this.FinishDispose();
    }

    protected virtual void FinishDispose()
    {
        if ((Turn.State != GameStateType.Combat) && !base.HasPostEvent)
        {
            Turn.PushStateDestination(GameStateType.Dispose);
            Turn.State = GameStateType.Recharge;
        }
    }

    private int FirstValidCharacter()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Party.Characters[i].HordeFightLeft > 0)
            {
                return i;
            }
        }
        return -1;
    }

    public override void Invoke()
    {
        Turn.ClearEncounterData();
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "EventHorde_Target"));
        if (!this.IsTargetNecessary())
        {
            Turn.Target = this.FirstValidCharacter();
            Turn.GotoStateDestination();
        }
        else
        {
            GameStateTarget.DisplayText = StringTableManager.GetUIText(0x170);
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (Party.Characters[i].HordeFightLeft > 0)
                {
                    Party.Characters[i].Active = ActiveType.Active;
                }
                else
                {
                    Party.Characters[i].Active = !this.IsValidTarget(Party.Characters[i]) ? ActiveType.Inactive : ActiveType.Locked;
                }
            }
            if (CardTable.Get(Turn.SummonsMonster) != null)
            {
                string str;
                bool flag = true;
                for (int j = 0; j < Party.Characters.Count; j++)
                {
                    if (Party.Characters[j].Active == ActiveType.Locked)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    str = string.Format(StringTableManager.GetHelperText(0x81), CardTable.Get(Turn.SummonsMonster).Name);
                }
                else
                {
                    str = string.Format(StringTableManager.GetHelperText(0x89), CardTable.Get(Turn.SummonsMonster).Name);
                }
                Turn.SetStateData(new TurnStateData(str));
            }
            Turn.State = GameStateType.Target;
        }
    }

    private bool IsTargetNecessary()
    {
        int num = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if ((Party.Characters[i].HordeFightLeft > 0) || this.IsValidTarget(Party.Characters[i]))
            {
                num++;
            }
        }
        return (num > 1);
    }

    private bool IsValidTarget(Character c)
    {
        if (c.Alive)
        {
            switch (Turn.SummonsLocation)
            {
                case LocationType.None:
                    return c.Equals(Turn.Owner);

                case LocationType.Current:
                    return c.Location.Equals(Turn.Owner.Location);

                case LocationType.Open:
                    return !Scenario.Current.IsLocationClosed(c.Location);

                case LocationType.Closed:
                    return Scenario.Current.IsLocationClosed(c.Location);

                case LocationType.All:
                    return true;

                case LocationType.CurrentOther:
                    return (c.Location.Equals(Turn.Owner.Location) && (c.ID != Turn.Owner.ID));
            }
        }
        return false;
    }

    public override bool Next()
    {
        int num = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Party.Characters[i].HordeFightLeft > 0)
            {
                num++;
            }
        }
        return (num > 0);
    }

    public override void Start()
    {
        base.Start();
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (this.IsValidTarget(Party.Characters[i]))
            {
                Party.Characters[i].HordeFightLeft = 1;
            }
            else
            {
                Party.Characters[i].HordeFightLeft = 0;
            }
        }
    }

    public override TurnStateIteratorType Type =>
        TurnStateIteratorType.Horde;
}

