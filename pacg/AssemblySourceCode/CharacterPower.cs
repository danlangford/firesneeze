using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class CharacterPower : Power
{
    [Tooltip("can this power be shared in multiplayer games?")]
    public bool Shareable;

    protected CharacterPower()
    {
    }

    public override void Activate()
    {
        base.Activate();
        Turn.CheckBoard.Set<bool>("GameStateRecharge_KeepLayout", true);
    }

    protected bool ActivateBestModifier(ref TraitType[] traits)
    {
        for (int i = 0; i < this.Character.Powers.Count; i++)
        {
            if (this.Character.Powers[i].Modifies(base.ID))
            {
                CharacterPowerModifier modifier = this.Character.Powers[i] as CharacterPowerModifier;
                if (modifier != null)
                {
                    TraitType[] typeArray = modifier.CombineModTraits(traits);
                    if (!Rules.IsImmune(Turn.Card, typeArray))
                    {
                        traits = typeArray;
                        return true;
                    }
                    typeArray = modifier.RevertModTraits(traits);
                    if (!Rules.IsImmune(Turn.Card, typeArray))
                    {
                        traits = typeArray;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static CharacterPower[] Create(string ID)
    {
        GameObject original = Resources.Load<GameObject>("Blueprints/Powers/" + ID);
        if (original != null)
        {
            GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(original);
            if (obj3 != null)
            {
                obj3.name = original.name;
                return obj3.GetComponents<CharacterPower>();
            }
        }
        return null;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        base.ShowCancelButton(false);
        this.HideModifiers();
        this.PowerEnd();
    }

    public static void Forget(Character character, string ID)
    {
        Transform transform = character.transform.FindChild("Powers/" + ID);
        if (transform != null)
        {
            UnityEngine.Object.Destroy(transform.gameObject);
        }
    }

    protected Card GetCardPlayed(ActionType PenaltyDeck)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            string str = Turn.CheckBoard.Get<string>("PenaltyGuid");
            GuiLayout layoutDeck = window.GetLayoutDeck(PenaltyDeck);
            if (layoutDeck != null)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    Card card = layoutDeck.Deck[new Guid(str)];
                    if (card != null)
                    {
                        return card;
                    }
                }
                int index = layoutDeck.IndexOf(this.Character.Powers.IndexOf(this), null);
                if (index >= 0)
                {
                    return layoutDeck.Deck[index];
                }
                index = layoutDeck.IndexOf(-1, null);
                if (index >= 0)
                {
                    return layoutDeck.Deck[index];
                }
                if (layoutDeck.Deck.Count > 0)
                {
                    return layoutDeck.Deck[layoutDeck.Deck.Count - 1];
                }
            }
        }
        return null;
    }

    public CharacterPowerModifier GetPowerModifier(int n)
    {
        if (this.Character != null)
        {
            int num = 0;
            for (int i = 0; i < this.Character.Powers.Count; i++)
            {
                if (this.Character.Powers[i].Modifies(base.ID))
                {
                    CharacterPowerModifier modifier = this.Character.Powers[i] as CharacterPowerModifier;
                    if ((modifier != null) && (num++ == n))
                    {
                        return modifier;
                    }
                }
            }
        }
        return null;
    }

    private void HideModifiers()
    {
        if (!Turn.IsPowerActive(base.ID) && (this.GetPowerModifier(0) != null))
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.powersPanel.Cancel();
            }
        }
    }

    public virtual void Initialize(Character self)
    {
    }

    protected void InitializeModifier(Character self, ref TraitType[] traits)
    {
        for (int i = 0; i < self.Powers.Count; i++)
        {
            if (self.Powers[i].Modifies(base.ID))
            {
                CharacterPowerModifier modifier = self.Powers[i] as CharacterPowerModifier;
                if (modifier != null)
                {
                    traits = modifier.CombineModTraits(traits);
                }
            }
        }
    }

    protected bool IsCardPlayable(CardType cardType)
    {
        EffectCardRestriction effect = Turn.Character.GetEffect(EffectType.CardRestriction) as EffectCardRestriction;
        if (effect != null)
        {
            return !effect.Match(cardType);
        }
        return true;
    }

    public virtual bool IsLegalActivation()
    {
        if (!this.IsPowerValid())
        {
            return false;
        }
        return (!Turn.Evade && !Turn.Defeat);
    }

    protected bool IsModifierInTraits(CharacterPowerModifier mod, TraitType[] currentTraits)
    {
        TraitType[] typeArray = currentTraits.Intersect<TraitType>(mod.GetCardTraits()).ToArray<TraitType>();
        TraitType[] typeArray2 = mod.GetCardTraits().ToArray<TraitType>();
        return (typeArray.Length == typeArray2.Length);
    }

    protected virtual bool IsPowerValid()
    {
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        return true;
    }

    protected bool IsSharingValid(int numCardsRequired = 0)
    {
        if (Game.GameType == GameType.LocalSinglePlayer)
        {
            return true;
        }
        if (Turn.Number == Turn.Switch)
        {
            return true;
        }
        if (!this.Shareable)
        {
            return false;
        }
        int num = 0;
        for (int i = 0; i < Turn.Character.Hand.Count; i++)
        {
            if (GameStateShare.IsCardSharedAsFuel(Turn.Character.Hand[i]))
            {
                num++;
            }
        }
        return (num >= numCardsRequired);
    }

    public virtual bool IsShortcutAvailable() => 
        false;

    public static CharacterPower[] Learn(Character character, string ID) => 
        character.transform.FindChild("Powers/" + ID)?.GetComponents<CharacterPower>();

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

    public virtual bool Modifies(string id) => 
        false;

    public virtual void OnCardDeactivated(Card card)
    {
    }

    public virtual void OnCardDefeated(Card card)
    {
    }

    public virtual void OnCardDiscarded(Card card)
    {
    }

    public virtual void OnCardEncountered(Card card)
    {
    }

    public virtual void OnCardEvaded(Card card)
    {
    }

    public virtual void OnCardPlayed(Card card)
    {
    }

    public virtual void OnCardUndefeated(Card card)
    {
    }

    public virtual void OnCheckCompleted()
    {
    }

    public virtual void OnLocationExplored(Card card)
    {
    }

    public virtual void OnPlayerDamaged(Card card)
    {
    }

    public virtual void OnSecondCombat(Card card)
    {
    }

    protected override void PowerEnd()
    {
        base.PowerEnd();
        this.HideModifiers();
    }

    protected void SetModifierTraits(bool active, CharacterPowerModifier mod, ref TraitType[] traits)
    {
        if (active)
        {
            if (Turn.IsPowerActive(base.ID))
            {
                Turn.RemoveTraits(traits);
                Turn.AddTraits(mod.CombineModTraits(traits));
            }
            traits = mod.CombineModTraits(traits);
        }
        else
        {
            if (Turn.IsPowerActive(base.ID))
            {
                Turn.RemoveTraits(traits);
                Turn.AddTraits(mod.RevertModTraits(traits));
            }
            traits = mod.RevertModTraits(traits);
        }
    }

    protected void ShowProceedButton(bool isVisible)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowProceedButton(isVisible);
        }
    }

    [DebuggerHidden]
    protected IEnumerator WaitForTime(float time) => 
        new <WaitForTime>c__Iterator17 { 
            time = time,
            <$>time = time
        };

    public virtual bool Automatic =>
        false;

    public Character Character { get; private set; }

    public virtual PowerType Type =>
        PowerType.None;

    [CompilerGenerated]
    private sealed class <WaitForTime>c__Iterator17 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal float <$>time;
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
                case 1:
                    if (this.time > 0f)
                    {
                        if (UI.Window.Visible)
                        {
                            this.time -= Time.deltaTime;
                        }
                        this.$current = null;
                        this.$PC = 1;
                        return true;
                    }
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

