using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class TurnStateIterator
{
    [Tooltip("when check iterator finishes it might need this to do the callback destinations")]
    public TurnStateCallbackType CallBackType;
    protected string InitialCard;
    protected int InitialCharacter;
    protected string InitialLocation;
    protected bool modifyPermission = true;

    protected TurnStateIterator()
    {
    }

    public virtual void End()
    {
        Turn.SwitchCharacter(this.InitialCharacter);
        Turn.Current = Turn.Number;
        Turn.CombatSkill = Turn.Character.GetCombatSkill();
        this.RefreshLocationWindow();
    }

    public static TurnStateIterator FromStream(ByteStream bs)
    {
        if (bs.ReadBool())
        {
            bs.ReadInt();
            TurnStateIterator iterator = TurnStateIteratorFactory.Create((TurnStateIteratorType) bs.ReadInt());
            iterator.InitialCharacter = bs.ReadInt();
            iterator.HasPostEvent = bs.ReadBool();
            iterator.CallBackType = (TurnStateCallbackType) bs.ReadInt();
            iterator.InitialLocation = bs.ReadString();
            iterator.InitialCard = bs.ReadString();
            return iterator;
        }
        return null;
    }

    public virtual bool HasNext()
    {
        this.modifyPermission = false;
        bool flag = this.Next();
        this.modifyPermission = true;
        return flag;
    }

    public virtual void Invoke()
    {
    }

    protected virtual bool IsRefreshAllowed() => 
        true;

    public virtual bool IsType(TurnStateIteratorType type) => 
        (this.Type == type);

    public virtual bool IsValid() => 
        true;

    protected void Message(string text)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (text != null)
            {
                window.messagePanel.Show(text);
            }
            else
            {
                window.messagePanel.Clear();
            }
        }
    }

    public virtual bool Next() => 
        false;

    protected bool NextCharacterAtAnyOpenLocation()
    {
        int number = Turn.Number;
        int num2 = 0;
        while (num2++ < Party.Characters.Count)
        {
            number++;
            if (number >= Party.Characters.Count)
            {
                number = 0;
            }
            if (number == this.InitialCharacter)
            {
                return false;
            }
            if (Party.Characters[number].Alive && !Scenario.Current.IsLocationClosed(Party.Characters[number].Location))
            {
                if (this.modifyPermission)
                {
                    Turn.SwitchCharacter(number);
                }
                return true;
            }
        }
        return false;
    }

    protected bool NextCharacterAtAnyOpenLocationExcept(string exception, CloseType type)
    {
        int number = Turn.Number;
        int num2 = 0;
        while (num2++ < Party.Characters.Count)
        {
            number++;
            if (number >= Party.Characters.Count)
            {
                number = 0;
            }
            if (number == this.InitialCharacter)
            {
                return false;
            }
            if ((Party.Characters[number].Alive && Scenario.Current.IsLocationClosePossible(Party.Characters[number].Location, type)) && !Party.Characters[number].Location.Equals(exception))
            {
                if (this.modifyPermission)
                {
                    Turn.SwitchCharacter(number);
                }
                return true;
            }
        }
        return false;
    }

    protected bool NextCharacterAtLocation(string locID)
    {
        int number = Turn.Number;
        int num2 = 0;
        while (num2++ < Party.Characters.Count)
        {
            number++;
            if (number >= Party.Characters.Count)
            {
                number = 0;
            }
            if (number == this.InitialCharacter)
            {
                return false;
            }
            if (Party.Characters[number].Alive && Party.Characters[number].Location.Equals(locID))
            {
                if (this.modifyPermission)
                {
                    Turn.SwitchCharacter(number);
                }
                return true;
            }
        }
        return false;
    }

    protected bool NextCharacterInParty()
    {
        int number = Turn.Number;
        int num2 = 0;
        while (num2++ < Party.Characters.Count)
        {
            number++;
            if (number >= Party.Characters.Count)
            {
                number = 0;
            }
            if (number == this.InitialCharacter)
            {
                return false;
            }
            if (Party.Characters[number].Alive)
            {
                if (this.modifyPermission)
                {
                    Turn.SwitchCharacter(number);
                }
                return true;
            }
        }
        return false;
    }

    protected bool NextSelectedCharacterInParty()
    {
        int number = Turn.Number;
        int num2 = 0;
        while (num2++ < Party.Characters.Count)
        {
            number++;
            if (number >= Party.Characters.Count)
            {
                number = 0;
            }
            if (number == this.InitialCharacter)
            {
                return false;
            }
            if (Party.Characters[number].Alive && Party.Characters[number].Selected)
            {
                if (this.modifyPermission)
                {
                    Turn.SwitchCharacter(number);
                }
                return true;
            }
        }
        return false;
    }

    protected void RefreshDicePanel()
    {
        if (this.IsRefreshAllowed())
        {
            Turn.DiceTarget = 0;
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.dicePanel.Refresh();
            }
        }
    }

    protected void RefreshLocationCard()
    {
        if (this.IsRefreshAllowed())
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.layoutLocation.Show(true);
                window.layoutLocation.Refresh();
            }
        }
    }

    protected void RefreshLocationWindow()
    {
        if (this.IsRefreshAllowed())
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.ProcessLayoutDecks();
                window.Refresh();
            }
        }
    }

    public virtual void Start()
    {
        this.InitialCharacter = Turn.Number;
        this.InitialLocation = Party.Characters[this.InitialCharacter].Location;
        this.InitialCard = Turn.Card.ID;
    }

    public void ToStream(ByteStream bs)
    {
        bs.WriteBool(true);
        bs.WriteInt(1);
        bs.WriteInt((int) this.Type);
        bs.WriteInt(this.InitialCharacter);
        bs.WriteBool(this.HasPostEvent);
        bs.WriteInt((int) this.CallBackType);
        bs.WriteString(this.InitialLocation);
        bs.WriteString(this.InitialCard);
    }

    public bool HasPostEvent { get; set; }

    public virtual TurnStateIteratorType Type =>
        TurnStateIteratorType.None;
}

