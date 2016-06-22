using System;
using UnityEngine;

public class BlockMoveCard : Block
{
    [Tooltip("source deck")]
    public DeckType From;
    [Tooltip("if true, the screen will refresh after the move finishes")]
    public bool Refresh = true;
    [Tooltip("selects the cards that are moved (null or empty means all)")]
    public CardSelector Selector;
    [Tooltip("maximum number of cards to examine in from deck before stopping")]
    public int SelectorNumber;
    [Tooltip("limits selection to this deck position (implies single card)")]
    public DeckPositionType SelectorPosition;
    [Tooltip("destination deck")]
    public DeckType To;
    [Tooltip("position within the destination deck")]
    public DeckPositionType ToPosition;
    [Tooltip("set to true to use the damage a character is taking to determine the number of cards")]
    public bool UseDamageTotal;
    [Tooltip("set to true to use the dice total to determine the number of cards")]
    public bool UseDiceTotal;

    public override void Invoke()
    {
        Deck deck = this.From.GetDeck();
        if (deck != null)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                int num = 0;
                if (this.UseDiceTotal)
                {
                    this.SelectorNumber = Turn.DiceTotal;
                }
                if (this.UseDamageTotal)
                {
                    this.SelectorNumber = Turn.Damage;
                }
                for (int i = deck.Count - 1; i >= 0; i--)
                {
                    Card card = deck[i];
                    if (this.SelectorPosition != DeckPositionType.None)
                    {
                        if (((this.SelectorPosition == DeckPositionType.Top) && (i >= this.SelectorNumber)) || ((this.SelectorPosition == DeckPositionType.Bottom) && (i <= ((deck.Count - this.SelectorNumber) - 1))))
                        {
                            continue;
                        }
                        if (this.SelectorPosition == DeckPositionType.Shuffle)
                        {
                            int num3 = UnityEngine.Random.Range(0, deck.Count);
                            card = deck[num3];
                            if (i >= this.SelectorNumber)
                            {
                                continue;
                            }
                        }
                    }
                    if (((this.Selector == null) || this.Selector.Match(card)) && ((this.From != DeckType.Revealed) || card.Revealed))
                    {
                        num++;
                        card.Revealed = this.To == DeckType.Revealed;
                        if (this.To == DeckType.Discard)
                        {
                            window.Discard(card);
                        }
                        if (this.To == DeckType.Bury)
                        {
                            window.Bury(card);
                        }
                        if (this.To == DeckType.Hand)
                        {
                            window.Draw(card);
                            if (num == 1)
                            {
                                UI.Sound.Play(SoundEffectType.DrawCardStart);
                            }
                            else
                            {
                                UI.Sound.Play(SoundEffectType.GenericFlickCard);
                            }
                        }
                        if (this.To == DeckType.Character)
                        {
                            window.Recharge(card, this.ToPosition);
                        }
                        if (this.To == DeckType.Banish)
                        {
                            Campaign.Box.Add(card, false);
                        }
                        if (this.To == DeckType.Location)
                        {
                            Location.Current.Deck.Add(card, this.ToPosition);
                        }
                    }
                }
                if (this.Refresh)
                {
                    window.Refresh();
                }
                if (((num < this.SelectorNumber) && (this.From == DeckType.Character)) && (this.To == DeckType.Hand))
                {
                    Turn.Character.Alive = false;
                    Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "Death_GetNextCharacter"));
                    Turn.State = GameStateType.Death;
                }
            }
        }
    }
}

