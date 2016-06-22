using System;

public class GuiPanelCharacterCardsLevelup : GuiPanelCharacterCards
{
    private int points;

    private GuiCardLine GetSelectedLine()
    {
        RewardFeat reward = (UI.Window as GuiWindowReward).Reward as RewardFeat;
        if ((reward != null) && reward.IsSelected(Turn.Number))
        {
            switch (reward.GetSelectedCard())
            {
                case CardType.Weapon:
                    return base.WeaponLine;

                case CardType.Spell:
                    return base.SpellLine;

                case CardType.Armor:
                    return base.ArmorLine;

                case CardType.Item:
                    return base.ItemLine;

                case CardType.Ally:
                    return base.AllyLine;

                case CardType.Blessing:
                    return base.BlessingLine;
            }
        }
        return null;
    }

    private bool IsLevelupPossible(CardType card, int rank)
    {
        if (this.points <= 0)
        {
            return false;
        }
        int cardRank = Turn.Character.GetCardRank(card);
        int cardMaxRank = Turn.Character.GetCardMaxRank(card);
        return (cardRank < cardMaxRank);
    }

    private void Levelup(GuiCardLine cardLine, CardType type)
    {
        GuiCardLine selectedLine = this.GetSelectedLine();
        if (selectedLine != cardLine)
        {
            if (selectedLine != null)
            {
                selectedLine.Unselect(Turn.Character.GetCardMinRank(type), Turn.Character.GetCardRank(selectedLine.Type), Turn.Character.GetCardMaxRank(type));
            }
            cardLine.Select(Turn.Character.GetCardMinRank(type), Turn.Character.GetCardRank(type) + 1, Turn.Character.GetCardMaxRank(type));
            GuiWindowReward window = UI.Window as GuiWindowReward;
            if (window != null)
            {
                window.OnRewardChosen(type);
            }
        }
    }

    private void OnLevelupAllyButtonPushed()
    {
        this.Levelup(base.AllyLine, CardType.Ally);
    }

    private void OnLevelupArmorButtonPushed()
    {
        this.Levelup(base.ArmorLine, CardType.Armor);
    }

    private void OnLevelupBlessingButtonPushed()
    {
        this.Levelup(base.BlessingLine, CardType.Blessing);
    }

    private void OnLevelupItemButtonPushed()
    {
        this.Levelup(base.ItemLine, CardType.Item);
    }

    private void OnLevelupSpellButtonPushed()
    {
        this.Levelup(base.SpellLine, CardType.Spell);
    }

    private void OnLevelupWeaponButtonPushed()
    {
        this.Levelup(base.WeaponLine, CardType.Weapon);
    }

    public override void Refresh()
    {
        base.Refresh();
        GuiCardLine selectedLine = this.GetSelectedLine();
        if (selectedLine != null)
        {
            selectedLine.Select(Turn.Character.GetCardMinRank(selectedLine.Type), Turn.Character.GetCardRank(selectedLine.Type) + 1, Turn.Character.GetCardMaxRank(selectedLine.Type));
        }
    }

    protected override void RefreshLine(CardType type, GuiCardLine cardLine)
    {
        base.RefreshLine(type, cardLine);
        cardLine.UpgradeButton.Glow(false);
        cardLine.UpgradeButton.Show(this.IsLevelupPossible(type, base.Character.GetCardRank(type)));
        cardLine.UpgradeButton.Refresh();
    }

    public void SetPoints(int n)
    {
        this.points = n;
    }
}

