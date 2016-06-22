using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class GameState
{
    protected GameState()
    {
    }

    protected void ApplyDecorations(bool isDecorating)
    {
        if (isDecorating)
        {
            for (int i = 0; i < Turn.Owner.Hand.Count; i++)
            {
                string decoration = null;
                for (int j = 0; j < Turn.Owner.GetNumEffects(); j++)
                {
                    decoration = Turn.Owner.GetEffect(j).GetCardDecoration(Turn.Owner.Hand[i]);
                    if (decoration != null)
                    {
                        Turn.Owner.Hand[i].Decorations.Add(decoration, CardSideType.Front, null, 0f);
                        break;
                    }
                }
                if (decoration == null)
                {
                    for (int k = 0; k < Turn.Owner.Hand[i].Powers.Length; k++)
                    {
                        decoration = Turn.Owner.Hand[i].Powers[k].GetCardDecoration(Turn.Owner.Hand[i]);
                        if (decoration != null)
                        {
                            bool flag = true;
                            if (decoration == "Blueprints/Gui/Vfx_Card_Notice_NotAllowed")
                            {
                                for (int m = 0; m < Turn.Owner.Hand[i].Powers.Length; m++)
                                {
                                    if (Turn.Owner.Hand[i].Powers[m].IsValid(Turn.Owner.Hand[i]))
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                            if (flag)
                            {
                                Turn.Owner.Hand[i].Decorations.Add(decoration, CardSideType.Front, null, 0f);
                                Turn.Owner.Hand[i].SortingOrder = Turn.Owner.Hand[i].SortingOrder;
                                break;
                            }
                        }
                    }
                    if (decoration == null)
                    {
                        Turn.Owner.Hand[i].Decorations.Clear();
                    }
                }
            }
        }
        else
        {
            for (int n = 0; n < Turn.Owner.Hand.Count; n++)
            {
                Turn.Owner.Hand[n].Decorations.Clear();
            }
        }
    }

    public virtual void Cancel()
    {
    }

    public void Commit()
    {
        if (this.Persistent)
        {
            Game.Save();
        }
    }

    public virtual void Enter()
    {
        this.Resolved = false;
        this.SaveRechargableCards();
        this.ProcessLayoutDecks();
        this.ApplyDecorations(true);
        this.ShowAidButton();
        Party.AutoActivateAbilities();
    }

    public virtual void Exit(GameStateType nextState)
    {
        this.ApplyDecorations(false);
    }

    protected virtual string GetHelpText() => 
        null;

    protected void HideTopCard(ActionType deckType, Deck deck)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if ((deck.Count > 0) && (window != null))
        {
            window.GlowLayoutDeck(deckType, false);
            Card card = deck[0];
            card.Side = CardSideType.Front;
            card.Show(false);
        }
    }

    public virtual bool IsActionAllowed(ActionType action, Card card) => 
        false;

    protected bool IsAnyActionPossible()
    {
        for (int i = 0; i < Turn.Owner.Hand.Count; i++)
        {
            if (Turn.Owner.Hand[i].IsAnyActionValid())
            {
                return true;
            }
        }
        for (int j = 0; j < Turn.Owner.Powers.Count; j++)
        {
            if (Turn.Owner.Powers[j].IsValid() && !Turn.Owner.Powers[j].Passive)
            {
                return true;
            }
        }
        for (int k = 0; k < Location.Current.Powers.Count; k++)
        {
            if (Location.Current.Powers[k].IsValid() && !Location.Current.Powers[k].Passive)
            {
                return true;
            }
        }
        for (int m = 0; m < Scenario.Current.Powers.Count; m++)
        {
            if (Scenario.Current.Powers[m].IsValid() && !Scenario.Current.Powers[m].Passive)
            {
                return true;
            }
        }
        return false;
    }

    public virtual bool IsCancelAllowed()
    {
        if (this.Busy)
        {
            return false;
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            return !window.dicePanel.Rolling;
        }
        return true;
    }

    protected bool IsCurrentState() => 
        (Turn.State == this.Type);

    public virtual bool IsProceedAllowed()
    {
        if (this.Busy)
        {
            return false;
        }
        if (Turn.SwitchType != SwitchType.AidAll)
        {
            if (Turn.SwitchType == SwitchType.Aid)
            {
                return true;
            }
            if (Turn.Character.ID != Turn.Owner.ID)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsResolved() => 
        this.Resolved;

    public virtual bool IsResolveSuccess() => 
        false;

    public virtual bool IsState(GameStateType type) => 
        (this.Type == type);

    protected void Message(int number)
    {
        string helperText = StringTableManager.GetHelperText(number);
        this.Message(helperText);
    }

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

    public virtual void Proceed()
    {
    }

    protected void ProcessLayoutDecks()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ProcessLayoutDecks();
        }
    }

    public virtual void Refresh()
    {
        this.ApplyDecorations(true);
    }

    public virtual void Resolve()
    {
        this.Resolved = true;
        this.SaveRechargableCards();
        this.ProcessLayoutDecks();
    }

    protected void SaveRechargableCards()
    {
        if (Turn.EmptyLayoutDecks)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.ProcessRechargableCards();
            }
        }
    }

    protected void ShowAidButton()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (((window != null) && ((Turn.SwitchType == SwitchType.AidAll) || (Turn.SwitchType == SwitchType.Aid))) && (window != null))
        {
            window.ShowProceedAidButton(true);
        }
    }

    protected void ShowTopCard(ActionType deckType, Deck deck)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if ((deck.Count > 0) && (window != null))
        {
            window.GlowLayoutDeck(deckType, true);
            Card card = deck[0];
            card.transform.localScale = deck.Layout.Size;
            card.transform.position = deck.Layout.transform.position;
            card.SortingOrder = 1;
            card.Side = CardSideType.Back;
            card.Show(true);
            Geometry.SetLayerRecursively(card.gameObject, Constants.LAYER_CARD);
        }
    }

    [DebuggerHidden]
    protected static IEnumerator WaitForTime(float time) => 
        new <WaitForTime>c__Iterator2F { 
            time = time,
            <$>time = time
        };

    protected bool Busy { get; set; }

    protected virtual bool Persistent =>
        true;

    protected bool Resolved { get; set; }

    public virtual GameStateType Type =>
        GameStateType.None;

    [CompilerGenerated]
    private sealed class <WaitForTime>c__Iterator2F : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal float <$>time;
        internal GuiWindowLocation <window>__0;
        internal float time;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<window>__0 = UI.Window as GuiWindowLocation;
                    if (this.<window>__0 == null)
                    {
                        this.$current = new WaitForSeconds(this.time);
                        this.$PC = 2;
                        goto Label_00BB;
                    }
                    break;

                case 1:
                    break;

                case 2:
                    goto Label_00B2;

                default:
                    goto Label_00B9;
            }
            if (this.time > 0f)
            {
                if (this.<window>__0.Visible)
                {
                    this.time -= Time.deltaTime;
                }
                this.$current = null;
                this.$PC = 1;
                goto Label_00BB;
            }
        Label_00B2:
            this.$PC = -1;
        Label_00B9:
            return false;
        Label_00BB:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }
}

