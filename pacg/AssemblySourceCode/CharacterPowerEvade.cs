using System;
using UnityEngine;

public class CharacterPowerEvade : CharacterPower
{
    [Tooltip("should the evaded card be shuffled automatically")]
    public bool Shuffle = true;

    public override void Activate()
    {
        base.PowerBegin();
        base.Activate();
        if (!this.Shuffle && !Rules.IsCardSummons(Turn.Card))
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                Turn.PushReturnState();
                window.Popup.Clear();
                window.Popup.Add(StringTableManager.GetUIText(0x1de), new TurnStateCallback(this, "CharacterPowerEvade_Evade"));
                window.Popup.Add(StringTableManager.GetUIText(0x1dd), new TurnStateCallback(this, "CharacterPowerEvade_OnTop"));
                window.Popup.SetDeckPosition(DeckType.Location);
                Turn.State = GameStateType.Popup;
            }
        }
        else
        {
            this.CharacterPowerEvade_Evade();
        }
    }

    private void CharacterPowerEvade_Evade()
    {
        Turn.Evade = true;
        if (Turn.State == GameStateType.Popup)
        {
            Turn.ReturnToReturnState();
        }
        base.ShowDice(false);
        Turn.Validate = true;
        base.ShowProceedButton(true);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Validate();
            window.ShowCancelButton(true);
        }
    }

    private void CharacterPowerEvade_OnTop()
    {
        Turn.Card.Disposition = DispositionType.Top;
        this.CharacterPowerEvade_Evade();
    }

    public override void Deactivate()
    {
        this.PowerEnd();
        Turn.Card.Disposition = DispositionType.None;
        base.Deactivate();
        base.ShowDice(true);
        Turn.Evade = false;
        base.ShowProceedButton(false);
        if ((Turn.State == GameStateType.EvadeOption) || (Turn.State == GameStateType.Horde))
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.ShowEncounterButton(true);
            }
        }
    }

    public override bool IsLegalActivation() => 
        !Turn.Defeat;

    public override bool IsValid()
    {
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        return Rules.IsEvadePossible(Turn.Card);
    }

    public override void OnCheckCompleted()
    {
        this.PowerEnd();
    }

    public override PowerType Type =>
        PowerType.Evade;
}

