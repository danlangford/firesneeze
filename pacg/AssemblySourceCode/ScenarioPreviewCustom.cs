using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScenarioPreviewCustom : MonoBehaviour
{
    protected ScenarioPreviewCustom()
    {
    }

    public abstract int AddHenchmanToList(int henchmenCount, Func<string, int, Card> createCard, List<Card> destination);
    protected void AddTypeDecoration(Card card, CardType type)
    {
        GameObject obj2 = card.Decorations.Add("Art/Cards/Card_TypeIcon", CardSideType.Back, null, 1f);
        Transform transform = obj2.transform.FindChild("CardTypeIcon_" + type.ToString());
        transform.gameObject.SetActive(true);
        SpriteRenderer component = transform.GetComponent<SpriteRenderer>();
        component.sortingOrder += card.SortingOrder;
        SpriteRenderer local2 = obj2.transform.FindChild("Shadow").GetComponent<SpriteRenderer>();
        local2.sortingOrder += card.SortingOrder;
    }

    public abstract int MaxHenchmen();
    protected string TypeToFakeCardID(CardType type)
    {
        switch (type)
        {
            case CardType.Ally:
                return "AL1B_Sage";

            case CardType.Armor:
                return "AR1B_LeatherArmor";

            case CardType.Barrier:
                return "BX1B_ExplosiveRunes";

            case CardType.Blessing:
                return "BL1B_BlessingOfTorag";

            case CardType.Item:
                return "IT1B_Codex";

            case CardType.Monster:
                return "MO1B_Spectre";

            case CardType.Spell:
                return "SP1B_LightningTouch";

            case CardType.Weapon:
                return "WP1B_ShortSword";
        }
        return "HE1B_Bandit";
    }
}

