using System;

public class GameStateReroll : GameStateCombat
{
    private readonly ActionType[] PlayLayouts = new ActionType[] { ActionType.Reveal, ActionType.Recharge, ActionType.Discard, ActionType.Bury, ActionType.Banish };

    public override void Enter()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            for (int i = 0; i < this.PlayLayouts.Length; i++)
            {
                this.LockInCards(window.GetLayoutDeck(this.PlayLayouts[i]).Deck, true);
            }
            if (Rules.IsRerollForced(Turn.Card))
            {
                window.ShowProceedButton(false);
                window.ShowCancelButton(false);
            }
            else
            {
                window.ShowProceedButton(true);
            }
            if (Rules.IsRerollAutomatic(Turn.Character.Hand) && !Rules.IsRerollForced(Turn.Card))
            {
                window.messagePanel.Show(StringTableManager.GetHelperText(0x29));
            }
            else if (!Rules.IsRerollPower())
            {
                if (!Turn.IsCardRerollEmpty() || Rules.IsRerollForced(Turn.Card))
                {
                    window.dicePanel.ShowRerollButton(true);
                }
            }
            else
            {
                CharacterPowerReroll reroll = null;
                for (int j = 0; j < Turn.Owner.Powers.Count; j++)
                {
                    reroll = Turn.Owner.Powers[j] as CharacterPowerReroll;
                    if (reroll != null)
                    {
                        break;
                    }
                }
                if (reroll != null)
                {
                    window.messagePanel.Show(reroll.HelperText.ToString());
                }
                else
                {
                    window.messagePanel.Show(StringTableManager.GetHelperText(0x74));
                }
            }
        }
    }

    public override void Exit(GameStateType nextState)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowProceedButton(false);
            window.dicePanel.ShowRerollButton(false);
            window.messagePanel.Clear();
        }
    }

    public override bool IsActionAllowed(ActionType action, Card card) => 
        card.IsActionAllowed(action);

    private void LockInCards(Deck deck, bool lockIn)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            if (deck[i].PlayedPower >= 0)
            {
                if (!lockIn && !deck[i].Displayed)
                {
                    deck[i].Locked = lockIn;
                }
                else
                {
                    deck[i].Locked = lockIn;
                }
            }
        }
    }

    public override void Proceed()
    {
        if (Turn.IsResolveSuccess() && Rules.IsRerollForced(Turn.Card))
        {
            Turn.EmptyLayoutDecks = false;
            Turn.State = GameStateType.Null;
            Turn.State = GameStateType.Reroll;
        }
        else
        {
            Turn.EmptyLayoutDecks = true;
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                if ((!Turn.IsResolveSuccess() && !Turn.IsCardRerollEmpty()) && Rules.WasRerollForced())
                {
                    Turn.Reroll = Guid.Empty;
                    window.dicePanel.ShowRerollButton(true);
                }
                else
                {
                    window.dicePanel.Resolve();
                }
            }
        }
    }

    public override GameStateType Type =>
        GameStateType.Reroll;
}

