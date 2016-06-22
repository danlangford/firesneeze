using System;

public class GameStateRollAgain : GameStateRoll
{
    public override void Enter()
    {
        base.Enter();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowProceedButton(true);
            if (Rules.IsRerollAutomatic(Turn.Character.Hand))
            {
                window.messagePanel.Show(StringTableManager.GetHelperText(0x29));
            }
            else if (!Rules.IsRerollPower())
            {
                if (Turn.Reroll != Guid.Empty)
                {
                    window.dicePanel.ShowRerollButton(true);
                }
            }
            else
            {
                CharacterPowerReroll reroll = null;
                for (int i = 0; i < Turn.Owner.Powers.Count; i++)
                {
                    reroll = Turn.Owner.Powers[i] as CharacterPowerReroll;
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
        }
    }

    public override bool IsActionAllowed(ActionType action, Card card)
    {
        for (int i = 0; i < card.Powers.Length; i++)
        {
            if ((card.Powers[i].Type == PowerType.Reroll) && card.Powers[i].IsActionAllowed(action, card))
            {
                return true;
            }
        }
        return false;
    }

    public override void Proceed()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Turn.ReturnToReturnState();
            window.dicePanel.Resolve();
        }
    }

    public override GameStateType Type =>
        GameStateType.RollAgain;
}

