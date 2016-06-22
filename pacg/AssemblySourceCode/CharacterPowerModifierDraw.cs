using System;
using UnityEngine;

public class CharacterPowerModifierDraw : BaseCharacterPowerMod
{
    [Tooltip("the text on the button to say no I don't want to draw")]
    public StrRefType NoDraw;
    [Tooltip("if true the player doesn't have to draw a card")]
    public bool Optional;
    [Tooltip("the text on the button to say yes draw a card")]
    public StrRefType YesDraw;

    public override void Activate()
    {
        if (this.Optional)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                Turn.PushReturnState();
                window.Popup.Clear();
                window.Popup.SetDeckPosition(DeckType.Character);
                window.Popup.Add(this.NoDraw.ToString(), new TurnStateCallback(base.GetComponent<CharacterPower>(), "PowerModifierDraw_Return"));
                window.Popup.Add(this.YesDraw.ToString(), new TurnStateCallback(base.GetComponent<CharacterPower>(), "PowerModifierDraw_Draw"));
                Turn.State = GameStateType.Popup;
            }
        }
        else
        {
            Turn.Character.Hand.Add(Turn.Character.Deck.Draw());
        }
    }

    private void PowerModifierDraw_Draw()
    {
        Turn.Character.Hand.Add(Turn.Character.Deck.Draw());
        this.PowerModifierDraw_Return();
    }

    private void PowerModifierDraw_Return()
    {
        Turn.ReturnToReturnState();
    }
}

