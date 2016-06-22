using System;
using UnityEngine;

public class GuiPanelCharacterCards : GuiPanel
{
    [Tooltip("reference to the ally card rank line in this panel")]
    public GuiCardLine AllyLine;
    [Tooltip("reference to the armor card rank line in this panel")]
    public GuiCardLine ArmorLine;
    [Tooltip("reference to the blessing card rank line in this panel")]
    public GuiCardLine BlessingLine;
    protected Character currentCharacter;
    [Tooltip("reference to the favored card label in this panel")]
    public GuiLabel FavoredCardLabel;
    [Tooltip("reference to the item card rank line in this panel")]
    public GuiCardLine ItemLine;
    [Tooltip("reference to the spell card rank line in this panel")]
    public GuiCardLine SpellLine;
    [Tooltip("reference to the tab button that brought us here")]
    public GuiButton TabButton;
    [Tooltip("reference to the weapon card rank line in this panel")]
    public GuiCardLine WeaponLine;

    public override void Initialize()
    {
    }

    public override void Refresh()
    {
        this.RefreshAllLines();
        this.RefreshFavoredCard();
    }

    protected void RefreshAllLines()
    {
        this.RefreshLine(CardType.Weapon, this.WeaponLine);
        this.RefreshLine(CardType.Spell, this.SpellLine);
        this.RefreshLine(CardType.Armor, this.ArmorLine);
        this.RefreshLine(CardType.Item, this.ItemLine);
        this.RefreshLine(CardType.Ally, this.AllyLine);
        this.RefreshLine(CardType.Blessing, this.BlessingLine);
    }

    protected void RefreshFavoredCard()
    {
        if (this.Character.FavoredCard == CardType.None)
        {
            this.FavoredCardLabel.Text = StringTableManager.Get("Rules", 50) + ": " + UI.Text(0x22f);
        }
        else
        {
            this.FavoredCardLabel.Text = StringTableManager.Get("Rules", 50) + ": " + this.Character.FavoredCard.ToText();
        }
    }

    protected virtual void RefreshLine(CardType type, GuiCardLine cardLine)
    {
        cardLine.Show(true);
        cardLine.Type = type;
        cardLine.SetRank(this.Character.GetCardMinRank(type), this.Character.GetCardRank(type), this.Character.GetCardMaxRank(type));
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.TabButton.Glow(isVisible);
        if (isVisible)
        {
            this.Refresh();
        }
    }

    public Character Character
    {
        get
        {
            if (this.currentCharacter != null)
            {
                return this.currentCharacter;
            }
            if (Party.Characters.Count > Turn.Number)
            {
                return Party.Characters[Turn.Number];
            }
            return null;
        }
        set
        {
            this.currentCharacter = value;
        }
    }

    public override uint zIndex =>
        Constants.ZINDEX_PANEL_FULL;
}

