using System;
using UnityEngine;

public class TurnStateCallback
{
    private Card CallbackCard;
    [Tooltip("some callbacks use cards/powers; this is the respective ID")]
    public string CallbackCardID;
    [Tooltip("identifies the specific object (location, character) within the type")]
    public string CallbackID;
    [Tooltip("the name of the function to be invoked")]
    public string CallbackName;
    [Tooltip("some callbacks just goto states instead of functions")]
    public GameStateType CallbackState;
    [Tooltip("the type of callback")]
    public TurnStateCallbackType CallbackType;

    public TurnStateCallback(GameStateType state)
    {
        this.CallbackName = null;
        this.CallbackType = TurnStateCallbackType.State;
        this.CallbackID = null;
        this.CallbackCard = null;
        this.CallbackCardID = null;
        this.CallbackState = state;
    }

    public TurnStateCallback(Card card, string function)
    {
        this.CallbackName = function;
        this.CallbackType = TurnStateCallbackType.Card;
        this.CallbackID = null;
        this.CallbackCard = card;
        this.CallbackCardID = card.ID;
        this.CallbackState = GameStateType.None;
    }

    public TurnStateCallback(CardPower power, string function)
    {
        this.CallbackName = function;
        this.CallbackType = TurnStateCallbackType.Card;
        this.CallbackID = null;
        this.CallbackCard = power.GetComponent<Card>();
        this.CallbackCardID = this.CallbackCard.ID;
        this.CallbackState = GameStateType.None;
    }

    public TurnStateCallback(CharacterPower power, string function)
    {
        this.CallbackName = function;
        this.CallbackType = TurnStateCallbackType.Character;
        this.CallbackID = power.Character.ID;
        this.CallbackCard = null;
        this.CallbackCardID = power.ID;
        this.CallbackState = GameStateType.None;
    }

    public TurnStateCallback(LocationPower power, string function)
    {
        this.CallbackName = function;
        this.CallbackType = TurnStateCallbackType.Location;
        this.CallbackID = power.LocationID;
        this.CallbackCard = null;
        this.CallbackCardID = null;
        this.CallbackState = GameStateType.None;
    }

    public TurnStateCallback(string id, string function)
    {
        this.CallbackName = function;
        this.CallbackType = TurnStateCallbackType.Card;
        this.CallbackID = null;
        this.CallbackCard = null;
        this.CallbackCardID = id;
        this.CallbackState = GameStateType.None;
    }

    public TurnStateCallback(TurnStateCallbackType type, string function)
    {
        this.CallbackName = function;
        this.CallbackType = type;
        this.CallbackID = null;
        this.CallbackCard = null;
        this.CallbackCardID = null;
        this.CallbackState = GameStateType.None;
    }

    private Card FindCardObject(Guid guid)
    {
        Card focusedCard = null;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (focusedCard == null)
            {
                focusedCard = window.layoutDiscard.Deck[guid];
            }
            if (focusedCard == null)
            {
                focusedCard = window.layoutBanish.Deck[guid];
            }
            if (focusedCard == null)
            {
                focusedCard = window.layoutBury.Deck[guid];
            }
            if (focusedCard == null)
            {
                focusedCard = window.layoutRecharge.Deck[guid];
            }
            if (focusedCard == null)
            {
                focusedCard = Turn.Character.Discard[guid];
            }
            if (focusedCard == null)
            {
                focusedCard = Turn.Character.Bury[guid];
            }
            if (focusedCard == null)
            {
                focusedCard = Turn.Character.Hand[guid];
            }
        }
        if (((focusedCard == null) && (Turn.FocusedCard != null)) && (Turn.FocusedCard.GUID == guid))
        {
            focusedCard = Turn.FocusedCard;
        }
        return focusedCard;
    }

    private Card FindCardObject(string ID)
    {
        Card card = null;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (card == null)
            {
                card = window.layoutDiscard.Deck[ID];
            }
            if (card == null)
            {
                card = window.layoutBanish.Deck[ID];
            }
            if (card == null)
            {
                card = window.layoutBury.Deck[ID];
            }
            if (card == null)
            {
                card = window.layoutRecharge.Deck[ID];
            }
            if (card == null)
            {
                card = Turn.Character.Discard[ID];
            }
            if (card == null)
            {
                card = Turn.Character.Bury[ID];
            }
            if (card == null)
            {
                card = Turn.Character.Hand[ID];
            }
            if (card == null)
            {
                card = Location.Current.Deck[ID];
            }
        }
        return card;
    }

    public static TurnStateCallback FromStream(ByteStream bs)
    {
        if (!bs.ReadBool())
        {
            return null;
        }
        bs.ReadInt();
        TurnStateCallbackType type = (TurnStateCallbackType) bs.ReadInt();
        string function = bs.ReadString();
        TurnStateCallback callback = new TurnStateCallback(type, function) {
            CallbackID = bs.ReadString(),
            CallbackCardID = bs.ReadString(),
            CallbackState = (GameStateType) bs.ReadInt()
        };
        if (bs.ReadBool())
        {
            callback.CallbackCard = callback.FindCardObject(bs.ReadGuid());
        }
        return callback;
    }

    public void Invoke()
    {
        switch (this.CallbackType)
        {
            case TurnStateCallbackType.Location:
            {
                if (this.CallbackID == null)
                {
                    this.CallbackID = Location.Current.ID;
                }
                GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(this.CallbackID);
                if (locationPowersRoot != null)
                {
                    locationPowersRoot.SendMessage(this.CallbackName, SendMessageOptions.DontRequireReceiver);
                }
                if (Location.Current.ID == this.CallbackID)
                {
                    Location.Current.SendMessage(this.CallbackName, SendMessageOptions.DontRequireReceiver);
                }
                break;
            }
            case TurnStateCallbackType.Card:
                if (this.CallbackCard == null)
                {
                    Card card = this.FindCardObject(this.CallbackCardID);
                    if (card != null)
                    {
                        card.SendMessage(this.CallbackName, SendMessageOptions.DontRequireReceiver);
                    }
                    break;
                }
                this.CallbackCard.SendMessage(this.CallbackName, SendMessageOptions.DontRequireReceiver);
                break;

            case TurnStateCallbackType.Character:
                this.SendMessageToPower(Party.Characters[this.CallbackID]);
                break;

            case TurnStateCallbackType.Global:
                Game.Instance.SendMessage(this.CallbackName, SendMessageOptions.DontRequireReceiver);
                break;

            case TurnStateCallbackType.Scenario:
                for (int i = 0; i < Scenario.Current.Powers.Count; i++)
                {
                    Scenario.Current.Powers[i].SendMessage(this.CallbackName, SendMessageOptions.DontRequireReceiver);
                }
                break;

            case TurnStateCallbackType.State:
                Turn.State = this.CallbackState;
                break;
        }
    }

    public static bool IsCallbackRunning(MonoBehaviour mb)
    {
        Component[] components = mb.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == mb)
            {
                return (Turn.BlackBoard.Get<int>("TurnStateCallbackIndex") == i);
            }
        }
        return false;
    }

    private bool SendMessageToPower(Character currentCharacter)
    {
        bool flag = this.CallbackCardID == null;
        for (int i = 0; i < currentCharacter.Powers.Count; i++)
        {
            if ((this.CallbackCardID == null) || (currentCharacter.Powers[i].ID == this.CallbackCardID))
            {
                currentCharacter.Powers[i].SendMessage(this.CallbackName, SendMessageOptions.DontRequireReceiver);
                flag = true;
            }
        }
        return flag;
    }

    public static void SetActiveCallback(MonoBehaviour mb)
    {
        int num = 0;
        if (mb != null)
        {
            Component[] components = mb.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == mb)
                {
                    num = i;
                    break;
                }
            }
        }
        Turn.BlackBoard.Set<int>("TurnStateCallbackIndex", num);
    }

    public void ToStream(ByteStream bs)
    {
        bs.WriteBool(true);
        bs.WriteInt(1);
        bs.WriteInt((int) this.CallbackType);
        bs.WriteString(this.CallbackName);
        bs.WriteString(this.CallbackID);
        bs.WriteString(this.CallbackCardID);
        bs.WriteInt((int) this.CallbackState);
        bs.WriteBool(this.CallbackCard != null);
        if (this.CallbackCard != null)
        {
            bs.WriteGuid(this.CallbackCard.GUID);
        }
    }

    public override string ToString()
    {
        if (this.CallbackType == TurnStateCallbackType.State)
        {
            return $"[TurnStateCallback] {this.CallbackType} {this.CallbackState}";
        }
        return $"[TurnStateCallback] {this.CallbackType} {this.CallbackID} {this.CallbackName} {this.CallbackCardID}";
    }
}

