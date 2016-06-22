using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiLayoutLocation : GuiLayout
{
    [CompilerGenerated]
    private static Action <>f__am$cache0;

    private void Confirm()
    {
        this.Refresh();
        if (<>f__am$cache0 == null)
        {
            <>f__am$cache0 = () => Turn.Proceed();
        }
        LeanTween.delayedCall(0.1f, <>f__am$cache0);
    }

    public override void Display()
    {
        Card turnCard = this.GetTurnCard();
        if (this.IsRefreshAllowed(turnCard))
        {
            LeanTween.cancel(turnCard.gameObject);
            turnCard.transform.position = base.transform.position;
            turnCard.transform.localScale = this.Scale;
            turnCard.SortingOrder = 2;
            turnCard.Side = CardSideType.Front;
            turnCard.Show(true);
            turnCard.Animate(AnimationType.Focus, true);
            Geometry.SetLayerRecursively(turnCard.gameObject, Constants.LAYER_CARD);
        }
    }

    public void Explore()
    {
        UI.Window.Pause(false);
        if (Turn.Card != this.GetTurnCard())
        {
            Card turnCard = this.GetTurnCard();
            if (turnCard != null)
            {
                turnCard.Deck.Move(turnCard.Deck.IndexOf(turnCard), 0);
            }
        }
        Turn.Number = Turn.Current;
        UI.Window.Refresh();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowExploreButton(false);
            if (Turn.Map)
            {
                window.ShowMap(false);
            }
        }
        Tutorial.Notify(TutorialEventType.LocationExplored);
        UI.Sound.Play(SoundEffectType.Explore);
        Turn.Card.OnExplored();
        Turn.Explore = false;
        Turn.CountExplores++;
        Turn.Phase = TurnPhaseType.Explore;
        Turn.CombatStage = TurnCombatStageType.PreEncounter;
        Turn.EncounteredGuid = Turn.Card.GUID;
        this.Show(true);
        this.Refresh();
        if (window != null)
        {
            window.ProcessRechargableCards();
        }
        Turn.PushStateDestination(GameStateType.Encounter);
        Turn.State = GameStateType.Recharge;
    }

    private void ExploreFromDrop()
    {
        base.StartCoroutine(this.ExploreFromDropCoroutine());
    }

    [DebuggerHidden]
    private IEnumerator ExploreFromDropCoroutine() => 
        new <ExploreFromDropCoroutine>c__Iterator4B { <>f__this = this };

    private Card GetTurnCard()
    {
        if (Location.Current.Deck.Count <= 0)
        {
            return null;
        }
        for (int i = 0; i < Location.Current.Deck.Count; i++)
        {
            if ((CardPropertyBlocker.IsExploreBlocked(Location.Current.Deck[i]) && (i < Turn.CountExplores)) && !Turn.EncounteredGuid.Equals(Location.Current.Deck[i].GUID))
            {
                if (i == (Location.Current.Deck.Count - 1))
                {
                    return null;
                }
            }
            else
            {
                return Location.Current.Deck[i];
            }
        }
        return Location.Current.Deck[0];
    }

    public void GlowText(bool isGlowing)
    {
        if (isGlowing && (!this.IsRefreshAllowed(Turn.Card) || !Turn.Card.Visible))
        {
            isGlowing = false;
        }
        string decoration = "Blueprints/Gui/Vfx_Card_GlowText_Normal";
        if (Turn.Card.ArtFormat == CardFormatType.Long)
        {
            decoration = "Blueprints/Gui/Vfx_Card_GlowText_Large";
        }
        if (isGlowing)
        {
            GameObject obj2 = Turn.Card.Decorations.Add(decoration, CardSideType.Front, null, this.Scale.x);
            if (obj2 != null)
            {
                Animator component = obj2.GetComponent<Animator>();
                if (component != null)
                {
                    component.SetTrigger("Glow");
                }
            }
        }
        else
        {
            Turn.Card.Decorations.Remove(decoration);
        }
    }

    private bool IsRefreshAllowed(Card card)
    {
        if (card == null)
        {
            return false;
        }
        if (this.Deck == null)
        {
            return false;
        }
        if (this.Deck.Count == 0)
        {
            return false;
        }
        if (!base.Visible)
        {
            return false;
        }
        if (Turn.FocusedCard != null)
        {
            return false;
        }
        if ((Party.Characters[Turn.Number].Location != Party.Characters[Turn.InitialCharacter].Location) && (Turn.CloseType == CloseType.None))
        {
            return false;
        }
        if (card.GUID != Turn.EncounteredGuid)
        {
            return false;
        }
        return true;
    }

    public override bool OnGuiDrag(Card card) => 
        false;

    public override bool OnGuiDrop(Card card)
    {
        if (card.Deck != Location.Current.Deck)
        {
            return false;
        }
        if (!Turn.Explore)
        {
            return false;
        }
        LeanTween.cancel(card.gameObject);
        if (Vector3.Distance(base.transform.position, card.transform.position) > 7f)
        {
            LeanTween.scale(card.gameObject, this.Scale, 0.5f).setEase(LeanTweenType.easeOutQuad);
            card.MoveCard(base.transform.position, 0.5f, SoundEffectType.None).setOnComplete(new Action(this.ExploreFromDrop)).setEase(LeanTweenType.easeOutCirc);
        }
        else
        {
            card.MoveCard(base.transform.position, 0.2f, SoundEffectType.None).setOnComplete(new Action(this.ExploreFromDrop)).setEase(LeanTweenType.easeOutQuad);
            LeanTween.scale(card.gameObject, this.Scale, 0.2f).setEase(LeanTweenType.easeOutQuad);
        }
        UI.Window.Pause(true);
        return true;
    }

    public override void OnLoadData()
    {
        byte[] buffer;
        bool flag = false;
        if ((!string.IsNullOrEmpty(base.GUID) && (this.Deck != null)) && Game.GetObjectData(base.GUID, out buffer))
        {
            ByteStream stream = new ByteStream(buffer);
            if (stream != null)
            {
                stream.ReadInt();
                flag = stream.ReadBool();
            }
        }
        if (flag)
        {
            this.Show(true);
            this.Display();
        }
    }

    public override void OnSaveData()
    {
        if (!string.IsNullOrEmpty(base.GUID) && (this.Deck != null))
        {
            ByteStream stream = new ByteStream();
            if (stream != null)
            {
                stream.WriteInt(1);
                stream.WriteBool(this.Displaying);
                Game.SetObjectData(base.GUID, stream.ToArray());
            }
        }
    }

    public override void Refresh()
    {
        Card turnCard = this.GetTurnCard();
        if (this.IsRefreshAllowed(turnCard))
        {
            LeanTween.cancel(turnCard.gameObject);
            turnCard.transform.position = base.transform.position;
            turnCard.transform.localScale = this.Scale;
            turnCard.SortingOrder = 2;
            turnCard.Side = CardSideType.Front;
            turnCard.Show(true);
            turnCard.Animate(AnimationType.Focus, true);
            Geometry.SetLayerRecursively(turnCard.gameObject, Constants.LAYER_CARD);
        }
    }

    public void ShowPreludeFX(bool isVisible)
    {
        string decoration = "Blueprints/Gui/Vfx_Card_Locked";
        if (!this.IsRefreshAllowed(Turn.Card))
        {
            isVisible = false;
        }
        if (isVisible)
        {
            GameObject obj2 = Turn.Card.Decorations.Add(decoration, CardSideType.Front, "Text/Marker - Locked Art", this.Scale.x);
            if (obj2 != null)
            {
                Animator component = obj2.GetComponent<Animator>();
                if (component != null)
                {
                    component.SetBool("bShow", true);
                }
            }
        }
        else
        {
            GameObject obj3 = Turn.Card.Decorations.Get(decoration);
            if (obj3 != null)
            {
                Animator animator2 = obj3.GetComponent<Animator>();
                if (animator2 != null)
                {
                    animator2.SetBool("bShow", false);
                }
            }
        }
    }

    private bool Displaying =>
        ((this.Deck.Count > 0) && (this.Deck[0].Visible && (this.Deck[0].Side == CardSideType.Front)));

    [CompilerGenerated]
    private sealed class <ExploreFromDropCoroutine>c__Iterator4B : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiLayoutLocation <>f__this;

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
                    this.$current = null;
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.Explore();
                    this.$PC = -1;
                    break;
            }
            return false;
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

