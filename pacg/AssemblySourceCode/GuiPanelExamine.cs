using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelExamine : GuiPanel
{
    [Tooltip("reference to the examine layout in this hierarchy (up)")]
    public GuiLayoutExamine Layout;
    [Tooltip("reference to the location layout in this scene")]
    public GuiLayoutLocation LocationLayout;

    private void MoveCardToHand(Character character, Card card)
    {
        card.Show(CardSideType.Front);
        character.Hand.Add(card);
        character.Hand.Layout.Refresh();
    }

    private void OnActionButtonPushed()
    {
        if (!UI.Busy)
        {
            if (this.Layout.Action == ExamineActionType.Evade)
            {
                this.Layout.Shuffle = true;
            }
            if (this.Layout.Action == ExamineActionType.Acquire)
            {
                this.Layout.Deck[0].Revealed = true;
            }
            float delayTime = this.Layout.Close();
            if (this.Layout.Action != ExamineActionType.None)
            {
                LeanTween.delayedCall(delayTime, new Action(this.OnActionButtonPushed_Finish));
            }
            else
            {
                Open = false;
            }
        }
    }

    private void OnActionButtonPushed_Finish()
    {
        this.OnAnyButtonPushed(this.Layout.ActionCallback, this.Layout.Action);
    }

    private void OnAlternateActionButtonPushed()
    {
        if (!UI.Busy)
        {
            if (this.Layout.AlternateAction == ExamineActionType.Evade)
            {
                this.Layout.Shuffle = true;
            }
            if (this.Layout.AlternateAction == ExamineActionType.Acquire)
            {
                this.Layout.Deck[0].Revealed = true;
            }
            float delayTime = this.Layout.Close();
            if (this.Layout.AlternateAction != ExamineActionType.None)
            {
                LeanTween.delayedCall(delayTime, new Action(this.OnAlternateButtonPushed_Finished));
            }
            else
            {
                Open = false;
            }
        }
    }

    private void OnAlternateButtonPushed_Finished()
    {
        this.OnAnyButtonPushed(this.Layout.ActionCallback, this.Layout.AlternateAction);
    }

    private void OnAnyButtonPushed(TurnStateCallback callback, ExamineActionType action)
    {
        if (callback != null)
        {
            callback.Invoke();
        }
        switch (action)
        {
            case ExamineActionType.Encounter:
                if (Turn.State == GameStateType.Examine)
                {
                    this.LocationLayout.Show(true);
                    this.LocationLayout.Refresh();
                    Turn.EmptyLayoutDecks = false;
                    Turn.PushStateDestination(GameStateType.Encounter);
                    Turn.State = GameStateType.Recharge;
                }
                break;

            case ExamineActionType.Acquire:
                if (Turn.State == GameStateType.Examine)
                {
                    if (this.Layout.AcquireDestination == DeckType.Hand)
                    {
                        this.LocationLayout.Show(true);
                        this.LocationLayout.Refresh();
                        Turn.State = GameStateType.Acquire;
                    }
                    if (this.Layout.AcquireDestination == DeckType.Character)
                    {
                        GuiWindowLocation window = UI.Window as GuiWindowLocation;
                        window.Recharge(Location.Current.Deck[0], DeckPositionType.Shuffle);
                        window.layoutRecharge.Shuffle(Turn.Character.Deck.Count);
                        Turn.Disposed = true;
                        Turn.ReturnToReturnState();
                    }
                }
                break;

            case ExamineActionType.Draw:
                if (Turn.State == GameStateType.Examine)
                {
                    UI.Sound.Play(SoundEffectType.DrawCardStart);
                    this.LocationLayout.Refresh();
                    this.MoveCardToHand(Turn.Character, Turn.Character.Deck[0]);
                    Turn.Character.Hand.Layout.Refresh();
                    Turn.ReturnToReturnState();
                }
                break;

            case ExamineActionType.Evade:
                if (this.Layout.Shuffle)
                {
                    this.Layout.Deck.Shuffle();
                }
                if (Turn.State == GameStateType.Examine)
                {
                    Turn.ReturnToReturnState();
                }
                break;

            case ExamineActionType.Recharge:
                if (Turn.State == GameStateType.Examine)
                {
                    this.LocationLayout.Refresh();
                    Turn.Character.Deck.Add(Turn.Character.Deck[0], DeckPositionType.Bottom);
                    Turn.Character.Deck[Turn.Character.Deck.Count - 1].Known = false;
                    Turn.ReturnToReturnState();
                }
                break;

            case ExamineActionType.ToggleBottomToUnderTop:
                Location.Current.Deck.Add(Location.Current.Deck[Location.Current.Deck.Count - 1], DeckPositionType.UnderTop);
                Location.Current.Deck[1].Known = true;
                if (this.Layout.AlternateAction == ExamineActionType.ToggleBottomToUnderTop)
                {
                    this.Layout.AlternateAction = ExamineActionType.ToggleUnderTopToBottom;
                }
                if (this.Layout.Action == ExamineActionType.ToggleBottomToUnderTop)
                {
                    this.Layout.Action = ExamineActionType.ToggleUnderTopToBottom;
                }
                Turn.ReturnToReturnState();
                break;

            case ExamineActionType.ToggleUnderTopToBottom:
                if (Location.Current.Deck.Count > 1)
                {
                    Location.Current.Deck.Add(Location.Current.Deck[1], DeckPositionType.Bottom);
                }
                if (this.Layout.AlternateAction == ExamineActionType.ToggleUnderTopToBottom)
                {
                    this.Layout.AlternateAction = ExamineActionType.ToggleBottomToUnderTop;
                }
                if (this.Layout.Action == ExamineActionType.ToggleUnderTopToBottom)
                {
                    this.Layout.Action = ExamineActionType.ToggleBottomToUnderTop;
                }
                Turn.ReturnToReturnState();
                break;
        }
        this.Layout.Clear();
        Open = false;
        Turn.Commit();
        UI.Window.Refresh();
    }

    private void OnCloseButtonPushed()
    {
        if (!UI.Busy)
        {
            LeanTween.delayedCall(this.Layout.Close(), new Action(this.OnCloseButtonPushed_Finish));
        }
    }

    private void OnCloseButtonPushed_Finish()
    {
        if (this.Layout.CloseCallback != null)
        {
            this.Layout.CloseCallback.Invoke();
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Refresh();
            window.layoutLocation.Refresh();
        }
        if (Turn.State == GameStateType.Examine)
        {
            Turn.Proceed();
        }
        this.Layout.Clear();
        Open = false;
        Turn.Commit();
    }

    public static bool Open
    {
        [CompilerGenerated]
        get => 
            <Open>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Open>k__BackingField = value;
        }
    }

    public override uint zIndex =>
        30;
}

