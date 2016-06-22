using System;
using UnityEngine;

public class SummonsSelector : Selector
{
    [Tooltip("added to the difficulty check")]
    public int CheckModifier;
    [Tooltip("card ID of creature to summon")]
    public string ID;

    public Card Draw() => 
        DrawCardFromBox(this.ID);

    private static Card DrawCardFromBox(string id)
    {
        if (id == "$Random")
        {
            return GetRandomMonster();
        }
        if (id == "$Turn.Card")
        {
            id = Turn.Card.ID;
        }
        Card card = Campaign.Box.Draw(id);
        if (card == null)
        {
            card = CardTable.Create(id);
            if (card != null)
            {
                card.Clone = true;
            }
        }
        return card;
    }

    private static Card GetRandomMonster()
    {
        if (Settings.Debug.Summons != null)
        {
            Card card = Campaign.Box.Draw(Settings.Debug.Summons);
            if (card == null)
            {
                card = CardTable.Create(Settings.Debug.Summons);
            }
            if (card != null)
            {
                return card;
            }
        }
        return Campaign.Box.Draw(CardType.Monster);
    }

    public static string GetSummonsMonster(string id)
    {
        if (id == "$Turn.Card")
        {
            return Turn.Card.ID;
        }
        return id;
    }

    public Card Summon()
    {
        if (this.CheckModifier != 0)
        {
            Turn.Character.ApplyEffect(new EffectModifyDifficulty(Effect.GetEffectID(this), Effect.DurationCheck, this.CheckModifier, SkillCheckType.None, CardFilter.Empty, -1, false));
        }
        return Summon(this.ID);
    }

    public static Card Summon(string id)
    {
        Card card = DrawCardFromBox(id);
        if (card != null)
        {
            Turn.EvadeDeclined = false;
            Turn.Summons = true;
            Turn.SummonsSource = Turn.Card.ID;
            Turn.SummonsMonster = card.ID;
            Turn.EncounteredGuid = card.GUID;
            card.Decorations.Add("Blueprints/Gui/Vfx_Card_Notice_Summoned", CardSideType.Front, null, 0f);
        }
        return card;
    }
}

