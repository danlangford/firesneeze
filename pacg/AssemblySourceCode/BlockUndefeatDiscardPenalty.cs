using System;
using UnityEngine;

public class BlockUndefeatDiscardPenalty : Block
{
    [Tooltip("if true will banish the card after this event")]
    public bool BanishAfterPenalty = true;

    public override void Invoke()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (Turn.NumCombatUndefeats > 0)
            {
                Turn.State = GameStateType.Null;
                for (int i = 0; i < Party.Characters.Count; i++)
                {
                    if (Party.Characters[i].Location == Location.Current.ID)
                    {
                        Turn.Number = i;
                        Turn.Current = i;
                        for (int j = 0; j < Turn.NumCombatUndefeats; j++)
                        {
                            window.Discard(Party.Characters[i].Deck[0]);
                        }
                    }
                }
                Turn.ClearCheckData();
                Turn.ClearCombatData();
                window.Refresh();
            }
            Turn.Number = Turn.InitialCharacter;
            Turn.Current = Turn.Number;
            Turn.NumCombatUndefeats = 0;
            Turn.NumCombatEvades = 0;
            if (this.BanishAfterPenalty)
            {
                Turn.Card.Disposition = DispositionType.Banish;
            }
            Turn.State = GameStateType.Dispose;
        }
    }

    public override bool Stateless =>
        false;
}

