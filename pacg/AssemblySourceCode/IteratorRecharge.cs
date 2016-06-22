using System;

public class IteratorRecharge : TurnStateIterator
{
    private ActionType[] AdditionalRechargeOptions()
    {
        for (int i = 0; i < Turn.Character.Powers.Count; i++)
        {
            if (Turn.Character.Powers[i] is CharacterPowerAlternateRecharge)
            {
                CharacterPowerAlternateRecharge recharge = Turn.Character.Powers[i] as CharacterPowerAlternateRecharge;
                if (recharge.IsValid())
                {
                    return recharge.AdditionalOptions;
                }
            }
        }
        return null;
    }

    private void DisplayCard(Card card)
    {
        if (card != null)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                card.SortingOrder = 3;
                card.Show(true);
                card.MoveCard(window.layoutLocation.transform.position, 0.3f);
                LeanTween.scale(card.gameObject, window.layoutLocation.Scale, 0.3f).setEase(LeanTweenType.easeInOutQuad);
            }
        }
    }

    public override void End()
    {
        base.End();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.messagePanel.Clear();
        }
        if (Location.Current.ID != Turn.Character.Location)
        {
            Location.Load(Turn.Character.Location);
        }
        base.RefreshLocationWindow();
        Turn.Proceed();
    }

    public override void Invoke()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Turn.Current = Turn.Number;
            if (Location.Current.ID != Turn.Character.Location)
            {
                Location.Load(Turn.Character.Location);
            }
            base.RefreshLocationWindow();
            window.ProcessRechargableCards();
            window.ProcessLayoutDecks();
            if ((Turn.EmptyLayoutDecks && (Turn.ReturnState != GameStateType.Recharge)) && (Turn.State == GameStateType.Recharge))
            {
                Turn.ClearCheckData();
            }
            if (Party.Characters[Turn.Number].Recharge.Count > 0)
            {
                Card card = Party.Characters[Turn.Number].Recharge[0];
                if (!Rules.IsRechargePossible(Party.Characters[Turn.Number], card))
                {
                    VisualEffect.ApplyToCard(VisualEffectType.CardBanishFromDiscard, card, 3f);
                    UI.Sound.Play(SoundEffectType.BoonFailAcquireBanish);
                    Campaign.Box.Add(card, false);
                    Turn.Iterators.Invoke();
                }
                else if (!this.PlayerMadeChoice())
                {
                    CardPower playedCardPower = card.GetPlayedCardPower();
                    string uIText = StringTableManager.GetUIText(0x10d);
                    if (playedCardPower != null)
                    {
                        uIText = playedCardPower.RechargeAction.ToText();
                    }
                    string text = StringTableManager.GetUIText(0x10c);
                    CardPropertyRecharge component = card.GetComponent<CardPropertyRecharge>();
                    if (component != null)
                    {
                        text = component.SuccessDestination.ToText();
                    }
                    this.DisplayCard(card);
                    window.Popup.Clear();
                    window.Popup.Add(uIText, new TurnStateCallback(TurnStateCallbackType.Global, "GameStateRecharge_Ask_No"));
                    window.Popup.Add(text, new TurnStateCallback(TurnStateCallbackType.Global, "GameStateRecharge_Ask_Yes"));
                    ActionType[] typeArray = this.AdditionalRechargeOptions();
                    if (typeArray != null)
                    {
                        for (int i = 0; i < typeArray.Length; i++)
                        {
                            window.Popup.Add(typeArray[i].ToText(), new TurnStateCallback(TurnStateCallbackType.Global, "GameStateRecharge_Ask_" + typeArray[i].ToString()));
                        }
                    }
                    window.Popup.SetDeckPosition(DeckType.Location);
                    UI.Sound.Play(SoundEffectType.RechargePopup);
                    Turn.State = GameStateType.Popup;
                }
                else if (this.PlayerMadeYesChoice())
                {
                    if (Rules.IsCheckAutomatic(card))
                    {
                        this.DisplayCard(card);
                        GameStateRecharge.StartRechargeSuccess(card);
                    }
                    else
                    {
                        if (Turn.State != GameStateType.Recharge)
                        {
                            Turn.State = GameStateType.Recharge;
                        }
                        this.DisplayCard(card);
                        string helperText = StringTableManager.GetHelperText(0x57);
                        base.Message(string.Format(helperText, card.DisplayName));
                        window.ShowProceedButton(false);
                        Party.OnStepCompleted();
                        this.SetupDicePanel(window.dicePanel, card);
                    }
                }
                else
                {
                    GameStateRecharge.StartRechargeFail(card);
                }
            }
            else
            {
                Turn.Iterators.Next();
            }
        }
    }

    public override bool Next() => 
        this.NextCharacterToRecharge();

    private bool NextCharacterToRecharge()
    {
        if (Game.GameType != GameType.LocalMultiPlayer)
        {
            int num = 0;
            int number = Turn.Number;
            while (num++ < Party.Characters.Count)
            {
                number++;
                if (number >= Party.Characters.Count)
                {
                    number = 0;
                }
                if ((number == base.InitialCharacter) && !Rules.IsRechargeNecessary())
                {
                    return false;
                }
                if ((Turn.Character.Alive && (Party.Characters[number].Recharge.Count > 0)) && Rules.IsRechargePossible(Party.Characters[number], Party.Characters[number].Recharge[0]))
                {
                    if (base.modifyPermission)
                    {
                        Turn.SwitchCharacter(number);
                    }
                    return true;
                }
            }
        }
        return false;
    }

    private bool PlayerMadeChoice() => 
        (((Turn.BlackBoard.Get<bool>("GameStateRecharge_Ask_No") || Turn.BlackBoard.Get<bool>("GameStateRecharge_Ask_Yes")) || Turn.BlackBoard.Get<bool>("GameStateRecharge_Ask_" + ActionType.Shuffle.ToString())) ? true : Turn.BlackBoard.Get<bool>("GameStateRecharge_Ask_" + ActionType.Top.ToString()));

    private bool PlayerMadeYesChoice() => 
        ((Turn.BlackBoard.Get<bool>("GameStateRecharge_Ask_Yes") || Turn.BlackBoard.Get<bool>("GameStateRecharge_Ask_" + ActionType.Shuffle.ToString())) ? true : Turn.BlackBoard.Get<bool>("GameStateRecharge_Ask_" + ActionType.Top.ToString()));

    private void SetupDicePanel(GuiPanelDice dicePanel, Card card)
    {
        SkillCheckValueType[] checks = card.Recharge;
        CardPropertyRecharge component = card.GetComponent<CardPropertyRecharge>();
        if (component != null)
        {
            CardPower playedCardPower = card.GetPlayedCardPower();
            if (!component.AllowedRecharge.Contains(playedCardPower.RechargeAction))
            {
                checks = new SkillCheckValueType[0];
            }
        }
        SkillCheckValueType bestSkillCheck = Turn.Character.GetBestSkillCheck(checks);
        dicePanel.Show(true);
        dicePanel.SetCheck(card, checks, bestSkillCheck.skill);
    }

    public override void Start()
    {
        base.Start();
    }

    public override TurnStateIteratorType Type =>
        TurnStateIteratorType.Recharge;
}

