using System;
using UnityEngine;

public class EventCallback
{
    [Tooltip("optional: the ID of the card used as a parameter to the event")]
    public string CallbackCardId;
    [Tooltip("the type defines the function to be invoked")]
    public EventType CallbackEvent;
    [Tooltip("the card/location/power/character ID where the callback code lives (owner)")]
    public string CallbackID;
    [Tooltip("the component position within the owner (an owner can have several events of the same type)")]
    public int CallbackPosition;
    [Tooltip("the type of callback: card/location/scenario/character")]
    public EventCallbackType CallbackType;
    [Tooltip("true when this callback does not manipulate the turn state")]
    public bool Stateless = true;

    private Card FindCardObject(string ID)
    {
        Card focusedCard = null;
        if (!string.IsNullOrEmpty(ID))
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window == null)
            {
                return focusedCard;
            }
            if (((focusedCard == null) && (Turn.FocusedCard != null)) && (Turn.FocusedCard.ID == ID))
            {
                focusedCard = Turn.FocusedCard;
            }
            if (focusedCard == null)
            {
                focusedCard = Location.Current.Deck[ID];
            }
            if (focusedCard == null)
            {
                focusedCard = window.layoutDiscard.Deck[ID];
            }
            if (focusedCard == null)
            {
                focusedCard = window.layoutBanish.Deck[ID];
            }
            if (focusedCard == null)
            {
                focusedCard = window.layoutBury.Deck[ID];
            }
            if (focusedCard == null)
            {
                focusedCard = window.layoutRecharge.Deck[ID];
            }
            if (focusedCard == null)
            {
                Card card = window.layoutSummoner.Card;
                if ((card != null) && (card.ID == ID))
                {
                    focusedCard = card;
                }
            }
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (focusedCard == null)
                {
                    focusedCard = Party.Characters[i].Discard[ID];
                }
                if (focusedCard == null)
                {
                    focusedCard = Party.Characters[i].Recharge[ID];
                }
                if (focusedCard == null)
                {
                    focusedCard = Party.Characters[i].Bury[ID];
                }
                if (focusedCard == null)
                {
                    focusedCard = Party.Characters[i].Hand[ID];
                }
                if (focusedCard == null)
                {
                    focusedCard = Party.Characters[i].Deck[ID];
                }
            }
        }
        return focusedCard;
    }

    private Character FindCharacterObject(string ID)
    {
        if (!string.IsNullOrEmpty(ID))
        {
            return Party.Find(ID);
        }
        return null;
    }

    public static EventCallback FromStream(ByteStream bs)
    {
        if (bs.ReadBool())
        {
            bs.ReadInt();
            return new EventCallback { 
                CallbackType = (EventCallbackType) bs.ReadInt(),
                CallbackID = bs.ReadString(),
                CallbackEvent = (EventType) bs.ReadInt(),
                CallbackPosition = bs.ReadInt(),
                Stateless = bs.ReadBool(),
                CallbackCardId = bs.ReadString()
            };
        }
        return null;
    }

    public void Invoke()
    {
        if (this.CallbackType == EventCallbackType.Location)
        {
            GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(this.CallbackID);
            if (locationPowersRoot != null)
            {
                this.Run(locationPowersRoot.gameObject);
            }
        }
        if (this.CallbackType == EventCallbackType.Scenario)
        {
            for (int i = 0; i < Scenario.Current.Powers.Count; i++)
            {
                if (Scenario.Current.Powers[i].name == this.CallbackID)
                {
                    this.Run(Scenario.Current.Powers[i].gameObject);
                }
            }
        }
        if (this.CallbackType == EventCallbackType.Character)
        {
            Character character = this.FindCharacterObject(this.CallbackID);
            if (character != null)
            {
                this.Run(character.gameObject);
            }
        }
        if (this.CallbackType == EventCallbackType.Card)
        {
            Card card = this.FindCardObject(this.CallbackID);
            if (card != null)
            {
                this.Run(card.gameObject);
            }
        }
        if (this.CallbackType == EventCallbackType.CharacterPower)
        {
            for (int j = 0; j < Party.Characters.Count; j++)
            {
                Power power = Party.Characters[j].GetPower(this.CallbackID);
                if (power != null)
                {
                    this.Run(power.gameObject);
                    break;
                }
            }
        }
    }

    private void Run(GameObject owner)
    {
        if (owner != null)
        {
            Event[] components = owner.GetComponents<Event>();
            if ((components != null) && (components.Length > this.CallbackPosition))
            {
                Card card = this.FindCardObject(this.CallbackCardId);
                if (this.CallbackEvent == EventType.OnCardDestroyed)
                {
                    components[this.CallbackPosition].OnCardDestroyed(card);
                }
                if (this.CallbackEvent == EventType.OnCardActivated)
                {
                    components[this.CallbackPosition].OnCardActivated(card);
                }
                if (this.CallbackEvent == EventType.OnCardDeactivated)
                {
                    components[this.CallbackPosition].OnCardDeactivated(card);
                }
                if (this.CallbackEvent == EventType.OnCardDefeated)
                {
                    components[this.CallbackPosition].OnCardDefeated(card);
                }
                if (this.CallbackEvent == EventType.OnCardDiscarded)
                {
                    components[this.CallbackPosition].OnCardDiscarded(card);
                }
                if (this.CallbackEvent == EventType.OnCardBuried)
                {
                    components[this.CallbackPosition].OnCardBuried(card);
                }
                if (this.CallbackEvent == EventType.OnCardEncountered)
                {
                    components[this.CallbackPosition].OnCardEncountered(card);
                }
                if (this.CallbackEvent == EventType.OnCardPlayed)
                {
                    components[this.CallbackPosition].OnCardPlayed(card);
                }
                if (this.CallbackEvent == EventType.OnCardRecharged)
                {
                    components[this.CallbackPosition].OnCardRecharged(card);
                }
                if (this.CallbackEvent == EventType.OnCardUndefeated)
                {
                    components[this.CallbackPosition].OnCardUndefeated(card);
                }
                if (this.CallbackEvent == EventType.OnCardUndefeatedSequence)
                {
                    components[this.CallbackPosition].OnCardUndefeatedSequence(card);
                }
                if (this.CallbackEvent == EventType.OnCombatEnd)
                {
                    components[this.CallbackPosition].OnCombatEnd(card);
                }
                if (this.CallbackEvent == EventType.OnCardEvaded)
                {
                    components[this.CallbackPosition].OnCardEvaded(card);
                }
                if (this.CallbackEvent == EventType.OnSecondCombat)
                {
                    components[this.CallbackPosition].OnSecondCombat(card);
                }
                if (this.CallbackEvent == EventType.OnPostHorde)
                {
                    components[this.CallbackPosition].OnPostHorde(card);
                }
                if (this.CallbackEvent == EventType.OnHandReset)
                {
                    components[this.CallbackPosition].OnHandReset();
                }
                if (this.CallbackEvent == EventType.OnPlayerDamaged)
                {
                    components[this.CallbackPosition].OnPlayerDamaged(card);
                }
                if (this.CallbackEvent == EventType.OnDamageTaken)
                {
                    components[this.CallbackPosition].OnDamageTaken(card);
                }
                if (this.CallbackEvent == EventType.OnLocationChange)
                {
                    components[this.CallbackPosition].OnLocationChange();
                }
                if (this.CallbackEvent == EventType.OnExamineAnyLocation)
                {
                    components[this.CallbackPosition].OnExamineAnyLocation();
                }
                if (this.CallbackEvent == EventType.OnLocationCloseAttempted)
                {
                    components[this.CallbackPosition].OnLocationCloseAttempted();
                }
                if (this.CallbackEvent == EventType.OnLocationClosed)
                {
                    components[this.CallbackPosition].OnLocationClosed();
                }
                if (this.CallbackEvent == EventType.OnTurnEnded)
                {
                    components[this.CallbackPosition].OnTurnEnded();
                }
                if (this.CallbackEvent == EventType.OnEndOfEndTurn)
                {
                    components[this.CallbackPosition].OnEndOfTurnEnded();
                }
                if (this.CallbackEvent == EventType.OnTurnStarted)
                {
                    components[this.CallbackPosition].OnTurnStarted();
                }
                if (this.CallbackEvent == EventType.OnPostAct)
                {
                    components[this.CallbackPosition].OnPostAct();
                }
                if (this.CallbackEvent == EventType.OnCardBeforeAct)
                {
                    components[this.CallbackPosition].OnBeforeAct();
                }
                if (this.CallbackEvent == EventType.OnBeforeTurnStart)
                {
                    components[this.CallbackPosition].OnBeforeTurnStart();
                }
                if (this.CallbackEvent == EventType.OnCallback)
                {
                    components[this.CallbackPosition].OnCallback();
                }
            }
        }
    }

    public void ToStream(ByteStream bs)
    {
        bs.WriteBool(true);
        bs.WriteInt(1);
        bs.WriteInt((int) this.CallbackType);
        bs.WriteString(this.CallbackID);
        bs.WriteInt((int) this.CallbackEvent);
        bs.WriteInt(this.CallbackPosition);
        bs.WriteBool(this.Stateless);
        bs.WriteString(this.CallbackCardId);
    }

    public override string ToString()
    {
        if (this.Stateless)
        {
            return $"[EventCallback-] {this.CallbackType} {this.CallbackID} {this.CallbackEvent} @ {this.CallbackPosition}";
        }
        return $"[EventCallback+] {this.CallbackType} {this.CallbackID} {this.CallbackEvent} @ {this.CallbackPosition}";
    }
}

