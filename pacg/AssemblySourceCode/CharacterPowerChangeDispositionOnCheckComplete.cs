using System;
using UnityEngine;

public class CharacterPowerChangeDispositionOnCheckComplete : CharacterPower
{
    [Tooltip("the new disposition to set the monster that was evaded")]
    public DispositionType Disposition;
    [Tooltip("the card will be visible in the deck you evade it to")]
    public bool Known = true;
    [Tooltip("prompt for button that will not change anything")]
    public StrRefType NoChange;
    [Tooltip("prompt for button that will change disposition of enemy")]
    public StrRefType YesChange;

    private void CharacterPowerChangeDispositionOnCheckComplete_No()
    {
        Turn.State = GameStateType.Post;
    }

    private void CharacterPowerChangeDispositionOnCheckComplete_Yes()
    {
        Turn.Card.Disposition = this.Disposition;
        Turn.Card.Known = this.Known;
        Turn.State = GameStateType.Post;
    }

    public override bool IsValid()
    {
        if (Rules.IsCardSummons(Turn.Card))
        {
            return false;
        }
        return base.IsConditionValid(Turn.Card);
    }

    public override void OnCheckCompleted()
    {
        if (this.IsValid())
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.Popup.Clear();
                window.Popup.SetDeckPosition(DeckType.Character);
                window.Popup.Add(this.NoChange.ToString(), new TurnStateCallback(this, "CharacterPowerChangeDispositionOnCheckComplete_No"));
                window.Popup.Add(this.YesChange.ToString(), new TurnStateCallback(this, "CharacterPowerChangeDispositionOnCheckComplete_Yes"));
                Turn.State = GameStateType.Popup;
            }
        }
    }
}

