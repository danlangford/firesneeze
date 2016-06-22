using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Tooltip("pointer to the art prefab for the front of this card")]
    public GameObject Art1;
    [Tooltip("pointer to the art prefab for the back of this card (optional)")]
    public GameObject Art2;
    [Tooltip("should the text layout in \"long\" format or normal?")]
    public CardFormatType ArtFormat;
    [Tooltip("which campaign does this card belong to (\"Runelords\", etc.)")]
    public CampaignType Campaign;
    [Tooltip("checks to defeat or acquire this card")]
    public SkillCheckValueType[] Checks1;
    [Tooltip("2nd set of checks to defeat or acquire this card")]
    public SkillCheckValueType[] Checks2;
    [Tooltip("optional gold price for this card (used during salvage)")]
    public int Cost;
    [Tooltip("X the name displayed at the top of this card")]
    public string DisplayName;
    [Tooltip("X power text")]
    public string DisplayText;
    [Tooltip("pointer to the small sprite for this card")]
    public Sprite Icon;
    [Tooltip("unique; used to find the art prefab and text in XML file")]
    public string ID;
    private bool isDragging;
    private bool isReturning;
    private bool isZoomed;
    private Vector3 lastPosition;
    private Vector3 lastScale;
    private int lastSortingOrder;
    private CardPropertyCopy myCopyProperty;
    private Deck myDeck;
    private DecorationManager myDecorations;
    private GameObject myGlow;
    private CardPower[] myPowers;
    private List<CardPlayedInfo> playedPowers;
    [Tooltip("how common is this card? used to determine the gold value for this card")]
    public RarityType Rarity = RarityType.Common;
    [Tooltip("checks to recharge this card after discard")]
    public SkillCheckValueType[] Recharge;
    [Tooltip("X indicates which set this card belongs to (\"B\"ase, etc.)")]
    public string Set;
    private CardSideType side = CardSideType.Front;
    [Tooltip("used to check to see if the henchman is a barrier or if any card has a subtype")]
    public CardType SubType;
    [Tooltip("traits for this card (\"Basic\", \"Animal\", etc.)")]
    public TraitType[] Traits;
    [Tooltip("type of card (\"Weapon\", \"Blessing\", etc.)")]
    public CardType Type;

    public void ActionActivate(ActionType action)
    {
        int bestPowerIndex = this.GetBestPowerIndex(action);
        this.PlayedOwner = Turn.Character.ID;
        if (bestPowerIndex >= 0)
        {
            this.OnCardActivated(this);
            this.Powers[bestPowerIndex].Activate(this);
            this.SetPowerInfo(bestPowerIndex, this);
        }
        else
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.Popup.Clear();
                for (int i = 0; i < this.Powers.Length; i++)
                {
                    if (((this.Powers[i].Action == action) || Rules.IsUnlimitedPlayPossible(this, this.Powers[i].Action)) && this.Powers[i].IsActionAllowed(action, this))
                    {
                        window.Popup.Add(this.Powers[i].DisplayText, new TurnStateCallback(this, "CardPowerPopup_" + i));
                    }
                }
                if (window.Popup.Count == 1)
                {
                    window.Popup.InvokePower(0);
                }
                else if (window.Popup.Count >= 2)
                {
                    window.Pause(true);
                    window.Popup.SetDeckPosition(action);
                    window.Popup.Show(true);
                    window.dicePanel.Fade(false, 0.15f);
                }
            }
        }
    }

    public bool ActionDeactivate(bool deactivateAllCardPowers = true)
    {
        int num;
        string str;
        bool flag = false;
        int num2 = this.PlayedPowers.Count + 1;
        int num3 = 0;
        do
        {
            Card card;
            CardPower playedCardPower = this.GetPlayedCardPower(out num, out card);
            if ((playedCardPower != null) && playedCardPower.IsPowerDeactivationAllowed(this))
            {
                if (card == this)
                {
                    this.OnCardDeactivated();
                }
                this.RemovePowerInfo(num, card);
                if (card != this)
                {
                    GuiWindowLocation window = UI.Window as GuiWindowLocation;
                    if (window != null)
                    {
                        card.ActionDeactivate(false);
                        flag = true;
                    }
                }
                else
                {
                    playedCardPower.Deactivate(this);
                    flag = true;
                }
            }
            num3++;
        }
        while ((num3 < num2) && deactivateAllCardPowers);
        if (flag)
        {
            this.ReturnCard(this);
        }
        Power playedPower = this.GetPlayedPower(out num, out str);
        if (playedPower != null)
        {
            this.RemovePowerInfo(num, str);
            if (Turn.IsPowerActive(playedPower.ID))
            {
                playedPower.Deactivate();
            }
            this.ReturnCard(this);
        }
        return (flag || (playedPower != null));
    }

    public void Animate(AnimationType animation, bool state)
    {
        if (animation == AnimationType.Focus)
        {
            Transform transform = base.transform.FindChild("Front/Art/Art_Holder");
            if (transform != null)
            {
                Animator component = transform.GetComponent<Animator>();
                if (component != null)
                {
                    component.SetBool("Focus", state);
                }
            }
        }
        else
        {
            Transform transform2 = base.transform.FindChild("Front/Art");
            if (transform2 != null)
            {
                Animator animator2 = transform2.GetComponent<Animator>();
                if (animator2 != null)
                {
                    if (animation == AnimationType.Attack)
                    {
                        animator2.SetTrigger("AttackPlayer");
                    }
                    else if (animation == AnimationType.Damaged)
                    {
                        animator2.SetTrigger("Damaged");
                    }
                    else if (animation == AnimationType.Defeated)
                    {
                        animator2.SetTrigger("Defeated");
                    }
                    else if ((animation == AnimationType.Undefeated) && ((this.Type == CardType.Barrier) || this.IsBoon()))
                    {
                        animator2.SetTrigger("GotoLocation");
                    }
                    else if (animation == AnimationType.Undefeated)
                    {
                        animator2.SetTrigger("Undefeated");
                    }
                    else if (animation == AnimationType.Escape)
                    {
                        animator2.SetTrigger("Escape");
                    }
                    else if (animation == AnimationType.FlipToFront)
                    {
                        animator2.SetTrigger("CardShow");
                    }
                    else if (animation == AnimationType.Acquire)
                    {
                        animator2.SetTrigger("Acquire");
                    }
                }
            }
        }
    }

    public void Animations(bool isEnabled)
    {
        Transform transform = base.transform.FindChild("Front/Art/Art_Holder");
        if (transform != null)
        {
            Animator component = transform.GetComponent<Animator>();
            if (component != null)
            {
                component.enabled = isEnabled;
            }
        }
        Transform transform2 = base.transform.FindChild("Front/Art");
        if (transform2 != null)
        {
            Animator animator2 = transform2.GetComponent<Animator>();
            if (animator2 != null)
            {
                animator2.enabled = isEnabled;
            }
        }
    }

    private void Awake()
    {
        this.myPowers = base.GetComponents<CardPower>();
        this.myCopyProperty = base.GetComponent<CardPropertyCopy>();
        this.Clear();
        this.lastPosition = base.transform.position;
        this.lastScale = base.transform.localScale;
        this.SetUpCheckArray(this.Checks1);
        this.SetUpCheckArray(this.Checks2);
    }

    private void CardPowerPopup_0()
    {
        this.CardPowerPopup_Handler(0);
    }

    private void CardPowerPopup_1()
    {
        this.CardPowerPopup_Handler(1);
    }

    private void CardPowerPopup_2()
    {
        this.CardPowerPopup_Handler(2);
    }

    private void CardPowerPopup_3()
    {
        this.CardPowerPopup_Handler(3);
    }

    private void CardPowerPopup_4()
    {
        this.CardPowerPopup_Handler(4);
    }

    private void CardPowerPopup_5()
    {
        this.CardPowerPopup_Handler(5);
    }

    private void CardPowerPopup_6()
    {
        this.CardPowerPopup_Handler(6);
    }

    private void CardPowerPopup_7()
    {
        this.CardPowerPopup_Handler(7);
    }

    private void CardPowerPopup_8()
    {
        this.CardPowerPopup_Handler(8);
    }

    private void CardPowerPopup_9()
    {
        this.CardPowerPopup_Handler(9);
    }

    private void CardPowerPopup_Handler(int n)
    {
        if ((n >= 0) && (n < this.Powers.Length))
        {
            this.PlayedOwner = Turn.Character.ID;
            this.OnCardActivated(this);
            this.Powers[n].Activate(this);
            this.SetPowerInfo(n, this);
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Pause(false);
            window.Popup.Show(false);
            if (!Turn.Defeat && !Turn.Evade)
            {
                window.dicePanel.Fade(true, 0.15f);
                window.dicePanel.Show(true);
            }
            window.dicePanel.Refresh();
        }
    }

    public void Clear()
    {
        this.Disposition = DispositionType.None;
        this.Blocker = BlockerType.None;
        this.Known = false;
        this.Revealed = false;
        this.Displayed = false;
        this.Locked = false;
        this.Side = CardSideType.Front;
        this.PreviousDeck = null;
        this.Shared = false;
        this.SharedPriority = 0;
        this.SharedPower = null;
        this.PlayedOwner = null;
        this.ResetPlayedPowersInfo();
        if (this.Decorations != null)
        {
            this.Decorations.Clear();
        }
    }

    public void Destroy()
    {
        this.OnDestroyed();
        UnityEngine.Object.Destroy(base.gameObject);
    }

    public int GetAllowedPowersCount(ActionType action)
    {
        int num = 0;
        for (int i = 0; i < this.Powers.Length; i++)
        {
            if (((this.Powers[i].Action == action) || Rules.IsUnlimitedPlayPossible(this, this.Powers[i].Action)) && this.Powers[i].IsActionPossible(action, this))
            {
                num++;
            }
        }
        return num;
    }

    public VisualEffectType GetAnimationVfx(AnimationType animation)
    {
        if (animation != AnimationType.Undefeated)
        {
            return VisualEffectType.None;
        }
        if (this.HasTrait(TraitType.Incorporeal))
        {
            return VisualEffectType.CardUndefeatedGhost;
        }
        return VisualEffectType.CardUndefeated;
    }

    public int GetBaseCheckModifier()
    {
        int num = 0;
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                num += components[i].GetBaseCheckModifier();
            }
        }
        return num;
    }

    private int GetBestPowerIndex(ActionType action)
    {
        int index = -1;
        for (int i = 0; i < this.Powers.Length; i++)
        {
            if (((this.Powers[i].Action == action) || Rules.IsUnlimitedPlayPossible(this, this.Powers[i].Action)) && this.Powers[i].IsActionPossible(action, this))
            {
                if (index == -1)
                {
                    index = i;
                }
                else if (this.Powers[i].IsEqualOrBetter(this.Powers[index]))
                {
                    index = i;
                }
                else if (!this.Powers[index].IsEqualOrBetter(this.Powers[i]))
                {
                    return -1;
                }
            }
        }
        return index;
    }

    public int GetCheckModifier()
    {
        int num = 0;
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                num += components[i].GetCheckModifier();
            }
        }
        return num;
    }

    public int GetCurrentLowestCheck()
    {
        int b = (this.Checks.Length <= 0) ? 0 : this.Checks[0].Rank;
        for (int i = 0; i < this.Checks.Length; i++)
        {
            b = Mathf.Min(Rules.GetCheckValue(this, this.Checks[i].skill), b);
        }
        return b;
    }

    public int GetDamageAmount(int damage)
    {
        CardPropertyDamageAmount component = base.GetComponent<CardPropertyDamageAmount>();
        if (component != null)
        {
            return component.Activate(damage);
        }
        return damage;
    }

    public bool GetDamageReduction()
    {
        CardPropertyDamage component = base.GetComponent<CardPropertyDamage>();
        if (component != null)
        {
            return component.Reducible;
        }
        return true;
    }

    public TraitType[] GetDamageTraits()
    {
        CardPropertyDamage component = base.GetComponent<CardPropertyDamage>();
        if (component != null)
        {
            return component.Traits;
        }
        return new TraitType[] { TraitType.Combat };
    }

    public int GetHighestCheck()
    {
        int b = 0;
        int combatCheckSequence = Turn.CombatCheckSequence;
        Turn.CombatCheckSequence = 1;
        for (int i = 0; i < this.Checks1.Length; i++)
        {
            if (combatCheckSequence == 1)
            {
                b = Mathf.Max(Rules.GetCheckValue(this, this.Checks1[i].skill), b);
            }
            else
            {
                b = Mathf.Max(Rules.GetCheckValue(this, this.Checks1[i].Rank), b);
            }
        }
        Turn.CombatCheckSequence = 2;
        for (int j = 0; j < this.Checks2.Length; j++)
        {
            if (combatCheckSequence == 2)
            {
                b = Mathf.Max(Rules.GetCheckValue(this, this.Checks2[j].skill), b);
            }
            else
            {
                b = Mathf.Max(Rules.GetCheckValue(this, this.Checks2[j].Rank), b);
            }
        }
        Turn.CombatCheckSequence = combatCheckSequence;
        return b;
    }

    public int GetIndexOf(CardPower power)
    {
        for (int i = 0; i < this.myPowers.Length; i++)
        {
            if (this.myPowers[i] == power)
            {
                return i;
            }
        }
        return -1;
    }

    public CardPower GetPlayedCardPower()
    {
        int num;
        Card card;
        return this.GetPlayedCardPower(out num, out card);
    }

    public CardPower GetPlayedCardPower(out int playedPower, out Card playedPowerOwner)
    {
        playedPower = -1;
        playedPowerOwner = null;
        for (int i = 0; i < this.PlayedPowers.Count; i++)
        {
            if ((this.PlayedPowers[i].PlayedPower >= 0) && (this.PlayedPowers[i].PlayedPowerOwner != null))
            {
                if (this.PlayedPowers[i].PlayedPowerOwner == this)
                {
                    playedPower = this.PlayedPowers[i].PlayedPower;
                    playedPowerOwner = this.PlayedPowers[i].PlayedPowerOwner;
                    return this.Powers[playedPower];
                }
                GuiWindowLocation window = UI.Window as GuiWindowLocation;
                if (window != null)
                {
                    playedPower = this.PlayedPowers[i].PlayedPower;
                    playedPowerOwner = this.PlayedPowers[i].PlayedPowerOwner;
                    if (playedPowerOwner != null)
                    {
                        return this.PlayedPowers[i].PlayedPowerOwner.Powers[playedPower];
                    }
                }
            }
        }
        return null;
    }

    public CharacterPower GetPlayedCharacterPower(out int playedPower, out string playedPowerOwnerID)
    {
        playedPower = -1;
        playedPowerOwnerID = null;
        for (int i = 0; i < this.PlayedPowers.Count; i++)
        {
            if (!string.IsNullOrEmpty(this.PlayedPowers[i].PlayedPowerOwnerID) && this.PlayedPowers[i].PlayedPowerOwnerID.StartsWith("CH"))
            {
                Character character = Party.Find(this.PlayedOwner);
                if (((character != null) && (this.PlayedPowers[i].PlayedPower >= 0)) && (this.PlayedPowers[i].PlayedPower < character.Powers.Count))
                {
                    playedPower = this.PlayedPowers[i].PlayedPower;
                    playedPowerOwnerID = this.PlayedPowers[i].PlayedPowerOwnerID;
                    if (character.Powers[playedPower].Cancellable)
                    {
                        return character.Powers[playedPower];
                    }
                }
            }
        }
        return null;
    }

    public LocationPower GetPlayedLocationPower(out int playedPower, out string playedPowerOwnerID)
    {
        playedPower = -1;
        playedPowerOwnerID = null;
        for (int i = 0; i < this.PlayedPowers.Count; i++)
        {
            if (((!string.IsNullOrEmpty(this.PlayedPowers[i].PlayedPowerOwnerID) && this.PlayedPowers[i].PlayedPowerOwnerID.StartsWith("LO")) && ((this.PlayedPowers[i].PlayedPowerOwnerID == Location.Current.ID) && (this.PlayedPowers[i].PlayedPower >= 0))) && (this.PlayedPowers[i].PlayedPower < Location.Current.Powers.Count))
            {
                playedPower = this.PlayedPowers[i].PlayedPower;
                playedPowerOwnerID = this.PlayedPowers[i].PlayedPowerOwnerID;
                if (Location.Current.Powers[playedPower].Cancellable)
                {
                    return Location.Current.Powers[playedPower];
                }
            }
        }
        return null;
    }

    public Power GetPlayedPower(out int playedPower, out string playedPowerOwnerID)
    {
        Power playedCharacterPower = this.GetPlayedCharacterPower(out playedPower, out playedPowerOwnerID);
        if (playedCharacterPower != null)
        {
            return playedCharacterPower;
        }
        return this.GetPlayedLocationPower(out playedPower, out playedPowerOwnerID);
    }

    public CardPower[] GetPowers() => 
        this.myPowers;

    public void Glow(bool isVisible)
    {
        if (this.myGlow == null)
        {
            Transform transform = base.transform.FindChild("Front/Art/card_shape_plate");
            if (transform != null)
            {
                this.myGlow = transform.gameObject;
            }
        }
        if (this.myGlow != null)
        {
            float num = !isVisible ? 0f : 1f;
            this.myGlow.GetComponent<Renderer>().material.SetFloat("_Enabled", num);
        }
    }

    public bool HasTrait(TraitType trait)
    {
        for (int i = 0; i < this.Traits.Length; i++)
        {
            if (this.Traits[i] == trait)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsActionAllowed(ActionType action)
    {
        for (int i = 0; i < this.Powers.Length; i++)
        {
            if (this.Powers[i].IsActionAllowed(action, this))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsActionValid(ActionType action)
    {
        for (int i = 0; i < this.Powers.Length; i++)
        {
            if (this.Powers[i].IsActionValid(action, this))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsAnyActionValid() => 
        (this.IsActionValid(ActionType.Banish) || (this.IsActionValid(ActionType.Bury) || (this.IsActionValid(ActionType.Discard) || (this.IsActionValid(ActionType.Display) || (this.IsActionValid(ActionType.Recharge) || this.IsActionValid(ActionType.Reveal))))));

    public bool IsBane() => 
        ((((this.Type == CardType.Barrier) || (this.Type == CardType.Henchman)) || (this.Type == CardType.Monster)) || (this.Type == CardType.Villain));

    public bool IsBlocker() => 
        (this.Blocker != BlockerType.None);

    public bool IsBoon() => 
        (((((this.Type == CardType.Ally) || (this.Type == CardType.Armor)) || ((this.Type == CardType.Blessing) || (this.Type == CardType.Item))) || (this.Type == CardType.Spell)) || (this.Type == CardType.Weapon));

    public bool IsDefeatable()
    {
        CardPropertyRequiredDamageTrait component = base.GetComponent<CardPropertyRequiredDamageTrait>();
        if (component != null)
        {
            return component.IsDefeatable();
        }
        return true;
    }

    public bool IsEnemy() => 
        ((((this.Type == CardType.Henchman) && (this.SubType != CardType.Barrier)) || (this.Type == CardType.Monster)) || ((this.Type == CardType.Villain) && (this.SubType != CardType.Barrier)));

    public bool IsPowerAlreadyActivated(CardPower power)
    {
        for (int i = 0; i < this.PlayedPowers.Count; i++)
        {
            if ((this.PlayedPowers[i].PlayedPowerOwner == this) && (this.PlayedPowers[i].PlayedPower == this.GetIndexOf(power)))
            {
                return true;
            }
        }
        return false;
    }

    public LTDescr MoveCard(Vector3 destination, float time) => 
        this.MoveCard(destination, time, SoundEffectType.GenericCardMoved);

    public LTDescr MoveCard(Vector3[] destinations, float time) => 
        this.MoveCard(destinations, time, SoundEffectType.GenericCardMoved);

    public LTDescr MoveCard(Vector3 destination, float time, SoundEffectType sound)
    {
        if (sound != SoundEffectType.None)
        {
            UI.Sound.Play(sound);
        }
        return LeanTween.move(base.gameObject, destination, time);
    }

    public LTDescr MoveCard(Vector3[] destinations, float time, SoundEffectType sound)
    {
        if (sound != SoundEffectType.None)
        {
            UI.Sound.Play(sound);
        }
        return LeanTween.move(base.gameObject, destinations, time);
    }

    public void OnBeforeAct()
    {
        Scenario.Current.OnCardBeforeAct(this);
        Location.Current.OnCardBeforeAct(this);
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnCardBeforeAct))
                {
                    Game.Events.Add(EventCallbackType.Card, this.ID, EventType.OnCardBeforeAct, i, components[i].Stateless, this);
                }
            }
        }
        if ((this.Type == CardType.Villain) || (this.Type == CardType.Henchman))
        {
            Campaign.SetCardEncountered(this.ID);
        }
    }

    private void OnCardActivated(Card card)
    {
        Event[] components = Turn.Card.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                components[i].OnCardActivated(card);
            }
        }
        components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int j = 0; j < components.Length; j++)
            {
                components[j].OnCardActivated(card);
            }
        }
    }

    public void OnCardBuried(Card card)
    {
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnCardBuried))
                {
                    Game.Events.Add(EventCallbackType.Card, card.ID, EventType.OnCardBuried, i, components[i].Stateless, card);
                }
            }
        }
    }

    public void OnCardDeactivated()
    {
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int j = 0; j < components.Length; j++)
            {
                components[j].OnCardDeactivated(this);
            }
        }
        components = Turn.Card.GetComponents<Event>();
        if (components != null)
        {
            for (int k = 0; k < components.Length; k++)
            {
                components[k].OnCardDeactivated(this);
            }
        }
        for (int i = 0; i < Location.Current.Powers.Count; i++)
        {
            components = Location.Current.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int m = 0; m < components.Length; m++)
                {
                    components[m].OnCardDeactivated(this);
                }
            }
        }
    }

    public bool OnCombatEnd()
    {
        if ((!Turn.Evade && !Turn.Defeat) && (this.NumCheckSequences > Turn.CombatCheckSequence))
        {
            return false;
        }
        if (Turn.EncounterType == EncounterType.LocationDefeat)
        {
            return false;
        }
        if (Turn.EncounterType == EncounterType.ReEncounter)
        {
            return false;
        }
        return Location.Current.OnCombatEnd(this);
    }

    public void OnCombatResolved()
    {
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnCombatResolved) && components[i].IsEventValid(this))
                {
                    components[i].OnCombatResolved();
                }
                components[i].EndGameIfNecessary(this);
            }
        }
    }

    public void OnDamageTaken(Card card)
    {
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnDamageTaken))
                {
                    Game.Events.Add(EventCallbackType.Card, Turn.Card.ID, EventType.OnDamageTaken, i, components[i].Stateless, card);
                }
            }
        }
    }

    public void OnDefeated()
    {
        if ((Turn.CombatCheckSequence < this.NumCheckSequences) && !Turn.Defeat)
        {
            this.Disposition = DispositionType.None;
        }
        else if (this.IsBoon())
        {
            this.Disposition = DispositionType.Acquire;
        }
        else
        {
            this.Disposition = DispositionType.Banish;
        }
        Scenario.Current.OnCardDefeated(this);
        Location.Current.OnCardDefeated(this);
        Turn.Character.OnCardDefeated(this);
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnCardDefeated))
                {
                    Game.Events.Add(EventCallbackType.Card, this.ID, EventType.OnCardDefeated, i, components[i].Stateless, this);
                }
            }
        }
        Game.Rewards.OnCardDefeated(this);
        Tutorial.Notify(TutorialEventType.CardDefeated);
        AnalyticsManager.OnCardDefeated(this);
        Game.Network.OnCardDefeated(this);
    }

    public void OnDestroyed()
    {
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnCardDestroyed))
                {
                    components[i].OnCardDestroyed(this);
                }
            }
        }
    }

    public void OnEncountered()
    {
        Scenario.Current.OnCardEncountered(this);
        Location.Current.OnCardEncountered(this);
        Party.OnCardEncountered(this);
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnCardEncountered))
                {
                    Game.Events.Add(EventCallbackType.Card, this.ID, EventType.OnCardEncountered, i, components[i].Stateless, this);
                }
            }
        }
    }

    public void OnEndOfEndTurn()
    {
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnEndOfEndTurn))
                {
                    Turn.DestructiveActionsCount++;
                    Game.Events.Add(EventCallbackType.Card, this.ID, EventType.OnEndOfEndTurn, i, components[i].Stateless);
                }
            }
        }
    }

    public void OnEvaded()
    {
        Turn.LastCombatResult = CombatResultType.None;
        if (Rules.IsCardSummons(this))
        {
            this.Disposition = DispositionType.Banish;
        }
        else if (this.Disposition == DispositionType.None)
        {
            this.Disposition = DispositionType.Shuffle;
        }
        Scenario.Current.OnCardEvaded(this);
        Location.Current.OnCardEvaded(this);
        Turn.Character.OnCardEvaded(this);
    }

    public void OnExamineAnyLocation()
    {
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnExamineAnyLocation))
                {
                    Game.Events.Add(EventCallbackType.Card, this.ID, EventType.OnExamineAnyLocation, i, components[i].Stateless, this);
                }
            }
        }
    }

    public void OnExplored()
    {
        Location.Current.OnLocationExplored(this);
        Party.OnLocationExplored(this);
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnLocationExplored))
                {
                    components[i].OnLocationExplored(this);
                }
            }
        }
    }

    public void OnGuiDrag()
    {
        if (!this.isDragging && !this.isReturning)
        {
            this.isDragging = true;
            this.isReturning = false;
            UI.Sound.Play(SoundEffectType.CardGrab);
            if (!base.gameObject.activeSelf)
            {
                this.Show(true);
            }
            if (!LeanTween.isTweening(base.gameObject) && !this.isZoomed)
            {
                this.lastPosition = base.transform.position;
                this.lastScale = base.transform.localScale;
            }
            if (this.isZoomed)
            {
                this.OnGuiZoom(false);
            }
            if (this.SortingOrder != Constants.SPRITE_SORTING_DRAG)
            {
                this.lastSortingOrder = this.SortingOrder;
            }
            this.SortingOrder = Constants.SPRITE_SORTING_DRAG;
            Vector3 to = new Vector3(0.35f, 0.35f, 1f);
            LeanTween.cancel(base.gameObject);
            LeanTween.scale(base.gameObject, to, 0.2f).setEase(LeanTweenType.easeOutQuad);
        }
    }

    public float OnGuiDrop(bool isValid)
    {
        float time = 0f;
        UI.Sound.Play(SoundEffectType.CardDrop);
        if (isValid)
        {
            this.isReturning = false;
            this.isDragging = false;
            this.isZoomed = false;
            return time;
        }
        this.isReturning = true;
        this.isDragging = false;
        Vector3 lastPosition = this.lastPosition;
        Vector3 lastScale = this.lastScale;
        LeanTween.cancel(base.gameObject);
        if (this.isZoomed)
        {
            lastPosition = Vector3.zero;
            lastScale = Device.GetCardZoomScale();
        }
        time = Geometry.GetTweenTime(base.transform.position, lastPosition, 0.25f);
        this.MoveCard(lastPosition, time, SoundEffectType.None).setEase(LeanTweenType.easeOutQuad).setOnComplete(new Action(this.OnGuiDropFinished));
        LeanTween.scale(base.gameObject, lastScale, time).setEase(LeanTweenType.easeOutQuad);
        return time;
    }

    private void OnGuiDropFinished()
    {
        if (this != null)
        {
            if (this.isZoomed)
            {
                this.SortingOrder = Constants.SPRITE_SORTING_ZOOM;
            }
            else
            {
                this.SortingOrder = this.lastSortingOrder;
            }
            this.isReturning = false;
        }
    }

    public float OnGuiZoom(bool isZoomed) => 
        this.OnGuiZoom(isZoomed, Vector3.zero, 1f);

    public float OnGuiZoom(bool isZoomed, Vector3 goalPosition, float sizeMultiplier)
    {
        float time = 0f;
        if (isZoomed)
        {
            this.isZoomed = true;
            UI.Sound.Play(SoundEffectType.CardZoom);
            UI.Sound.Play(SoundEffectType.GenericFlickCard);
            this.Animate(AnimationType.Focus, true);
            if (!LeanTween.isTweening(base.gameObject) && !this.isDragging)
            {
                this.lastPosition = base.transform.position;
                this.lastScale = base.transform.localScale;
            }
            this.isDragging = false;
            LeanTween.cancel(base.gameObject);
            if (this.SortingOrder != Constants.SPRITE_SORTING_ZOOM)
            {
                this.lastSortingOrder = this.SortingOrder;
            }
            this.SortingOrder = Constants.SPRITE_SORTING_ZOOM;
            time = Geometry.GetTweenTime(base.transform.position, goalPosition, 0.25f);
            Vector3[] destinations = Geometry.GetCurve(base.transform.position, goalPosition, 1f);
            this.MoveCard(destinations, time, SoundEffectType.None).setEase(LeanTweenType.easeOutQuad);
            LeanTween.scale(base.gameObject, (Vector3) (Device.GetCardZoomScale() * sizeMultiplier), time).setEase(LeanTweenType.easeOutQuad);
            return time;
        }
        this.isZoomed = false;
        UI.Sound.Play(SoundEffectType.CardUnzoom);
        this.Animate(AnimationType.Focus, false);
        return this.OnGuiDrop(false);
    }

    public void OnLocationCloseAttempted()
    {
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnLocationCloseAttempted))
                {
                    Game.Events.Add(EventCallbackType.Card, this.ID, EventType.OnLocationCloseAttempted, i, components[i].Stateless, this);
                }
            }
        }
    }

    public void OnPlayerDamaged(Card card)
    {
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnPlayerDamaged))
                {
                    Game.Events.Add(EventCallbackType.Card, Turn.Card.ID, EventType.OnPlayerDamaged, i, components[i].Stateless, card);
                }
            }
        }
    }

    public void OnPostAct()
    {
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnPostAct))
                {
                    Game.Events.Add(EventCallbackType.Card, this.ID, EventType.OnPostAct, i, components[i].Stateless, this);
                }
            }
        }
    }

    public void OnPostHorde(Card card)
    {
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnPostHorde))
                {
                    Game.Events.Add(EventCallbackType.Card, card.ID, EventType.OnPostHorde, i, components[i].Stateless, card);
                }
            }
        }
    }

    public void OnSecondCombat(Card card)
    {
        Turn.Character.OnSecondCombat(this);
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnSecondCombat))
                {
                    Game.Events.Add(EventCallbackType.Card, card.ID, EventType.OnSecondCombat, i, components[i].Stateless, card);
                }
            }
        }
    }

    public void OnTurnEnded()
    {
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnTurnEnded))
                {
                    Turn.DestructiveActionsCount++;
                    Game.Events.Add(EventCallbackType.Card, this.ID, EventType.OnTurnEnded, i, components[i].Stateless);
                }
            }
        }
    }

    public void OnUndefeated()
    {
        if (Turn.CombatCheckSequence < this.NumCheckSequences)
        {
            this.Disposition = DispositionType.None;
        }
        else if (this.IsBoon() || Rules.IsCardSummons(Turn.Card))
        {
            this.Disposition = DispositionType.Banish;
        }
        else
        {
            this.Disposition = DispositionType.Shuffle;
        }
        Scenario.Current.OnCardUndefeated(this);
        Location.Current.OnCardUndefeated(this);
        Turn.Character.OnCardUndefeated(this);
        Event[] components = base.GetComponents<Event>();
        if (components != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnCardUndefeated))
                {
                    Game.Events.Add(EventCallbackType.Card, this.ID, EventType.OnCardUndefeated, i, components[i].Stateless, this);
                }
            }
        }
        Tutorial.Notify(TutorialEventType.CardUndefeated);
        AnalyticsManager.OnCardUndefeated(this);
        Game.Network.OnCardUndefeated(this);
    }

    public void RemovePowerInfo(int playedPower, Card playedPowerOwner)
    {
        for (int i = 0; i < this.PlayedPowers.Count; i++)
        {
            if ((this.PlayedPowers[i].PlayedPower == playedPower) && (this.PlayedPowers[i].PlayedPowerOwner == playedPowerOwner))
            {
                this.PlayedPowers.RemoveAt(i);
                return;
            }
        }
    }

    public void RemovePowerInfo(int playedPower, string playedPowerOwnerID)
    {
        for (int i = 0; i < this.PlayedPowers.Count; i++)
        {
            if ((this.PlayedPowers[i].PlayedPower == playedPower) && (this.PlayedPowers[i].PlayedPowerOwnerID == playedPowerOwnerID))
            {
                this.PlayedPowers.RemoveAt(i);
                return;
            }
        }
    }

    public void ResetLastPositionAndScale()
    {
        this.lastPosition = base.transform.position;
        this.lastScale = base.transform.localScale;
    }

    private void ResetPlayedPowersInfo()
    {
        if (this.playedPowers != null)
        {
            this.playedPowers.Clear();
        }
    }

    public void ReturnCard(Card card)
    {
        if (!card.Revealed && !card.Displayed)
        {
            if (!string.IsNullOrEmpty(card.PlayedOwner))
            {
                if (card.PreviousDeck == null)
                {
                    Party.Characters[card.PlayedOwner].Hand.Add(card);
                }
                else
                {
                    card.PreviousDeck.Add(card);
                }
            }
            if (string.IsNullOrEmpty(card.PlayedOwner) || card.PlayedOwner.Equals(Turn.Character.ID))
            {
                card.Show(true);
            }
        }
        else
        {
            card.Revealed = false;
            card.Displayed = false;
        }
    }

    public void SetLastPosition(Vector3 pos)
    {
        this.lastPosition = pos;
    }

    public void SetPowerInfo(int playedPower, Card playedPowerOwner)
    {
        CardPlayedInfo item = new CardPlayedInfo {
            PlayedPower = playedPower,
            PlayedPowerOwner = playedPowerOwner,
            PlayedPowerOwnerID = playedPowerOwner.ID
        };
        this.PlayedPowers.Add(item);
    }

    public void SetPowerInfo(int playedPower, string playedPowerOwnerID)
    {
        CardPlayedInfo item = new CardPlayedInfo {
            PlayedPower = playedPower,
            PlayedPowerOwnerID = playedPowerOwnerID
        };
        this.PlayedPowers.Add(item);
    }

    public void SetPowerInfo(int playedPower, Guid playedPowerOwnerGuid, string playedPowerOwnerID)
    {
        CardPlayedInfo item = new CardPlayedInfo {
            PlayedPower = playedPower,
            PlayedPowerOwnerGuid = playedPowerOwnerGuid,
            PlayedPowerOwnerID = playedPowerOwnerID
        };
        this.PlayedPowers.Add(item);
    }

    private void SetUpCheckArray(SkillCheckValueType[] checks)
    {
        for (int i = 0; i < checks.Length; i++)
        {
            checks[i].Bonus = this.GetBaseCheckModifier();
        }
    }

    public void Show(CardSideType side)
    {
        this.side = side;
        this.Show(true);
    }

    public void Show(bool isVisible)
    {
        Transform transform = base.transform.FindChild("Front");
        Transform transform2 = base.transform.FindChild("Back");
        this.Visible = isVisible;
        if (isVisible)
        {
            if (this.Side == CardSideType.Front)
            {
                if (transform != null)
                {
                    transform.gameObject.SetActive(true);
                }
                if (transform2 != null)
                {
                    transform2.gameObject.SetActive(false);
                }
            }
            else if (this.Side == CardSideType.Back)
            {
                if (transform != null)
                {
                    transform.gameObject.SetActive(false);
                }
                if (transform2 != null)
                {
                    transform2.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (transform != null)
            {
                transform.gameObject.SetActive(false);
            }
            if (transform2 != null)
            {
                transform2.gameObject.SetActive(false);
            }
        }
    }

    public void Tint(Color color)
    {
        SpriteRenderer[] componentsInChildren = base.GetComponentsInChildren<SpriteRenderer>(true);
        if (componentsInChildren != null)
        {
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].color = color;
            }
        }
    }

    public BlockerType Blocker { get; set; }

    public SkillCheckValueType[] Checks
    {
        get
        {
            if (((Turn.CombatCheckSequence == 2) && (this.Checks2 != null)) && (this.Checks2.Length > 0))
            {
                return this.Checks2;
            }
            return this.Checks1;
        }
    }

    public bool Clone { get; set; }

    public Deck Deck
    {
        get => 
            this.myDeck;
        set
        {
            this.myDeck = value;
        }
    }

    public DecorationManager Decorations
    {
        get
        {
            if (this.myDecorations == null)
            {
                this.myDecorations = new DecorationManager();
                this.myDecorations.Owner = base.gameObject;
            }
            return this.myDecorations;
        }
    }

    public bool Displayed { get; set; }

    public DispositionType Disposition { get; set; }

    public Guid GUID { get; set; }

    public bool Known { get; set; }

    public GuiLayout Layout
    {
        get
        {
            if (this.Deck != null)
            {
                return this.Deck.Layout;
            }
            return null;
        }
    }

    public bool Locked { get; set; }

    public int NumCheckSequences
    {
        get
        {
            if ((this.Checks2 != null) && (this.Checks2.Length > 0))
            {
                return 2;
            }
            return 1;
        }
    }

    public bool Played
    {
        get
        {
            if ((this.PlayedPower >= 0) && (this.PlayedPowerOwner == this))
            {
                return true;
            }
            for (int i = 1; i < this.playedPowers.Count; i++)
            {
                if ((this.playedPowers[i].PlayedPower >= 0) && (this.playedPowers[i].PlayedPowerOwner == this))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public string PlayedOwner { get; set; }

    public int PlayedPower
    {
        get
        {
            if ((this.PlayedPowers != null) && (this.PlayedPowers.Count != 0))
            {
                return this.PlayedPowers[0].PlayedPower;
            }
            return -1;
        }
    }

    public Card PlayedPowerOwner
    {
        get
        {
            if ((this.PlayedPowers != null) && (this.PlayedPowers.Count != 0))
            {
                return this.PlayedPowers[0].PlayedPowerOwner;
            }
            return null;
        }
    }

    public string PlayedPowerOwnerID
    {
        get
        {
            if ((this.PlayedPowers != null) && (this.PlayedPowers.Count != 0))
            {
                return this.PlayedPowers[0].PlayedPowerOwnerID;
            }
            return null;
        }
    }

    public List<CardPlayedInfo> PlayedPowers
    {
        get
        {
            if (this.playedPowers == null)
            {
                this.playedPowers = new List<CardPlayedInfo>(4);
            }
            return this.playedPowers;
        }
    }

    public CardPower[] Powers
    {
        get
        {
            if ((this.myCopyProperty != null) && (this.myCopyProperty.Powers != null))
            {
                return this.myCopyProperty.Powers;
            }
            return this.myPowers;
        }
    }

    public Deck PreviousDeck { get; set; }

    public bool Revealed { get; set; }

    public bool Shareable
    {
        get
        {
            PowerConditionProficiency component = base.GetComponent<PowerConditionProficiency>();
            if ((component == null) || component.Evaluate(this))
            {
                for (int i = 0; i < this.Powers.Length; i++)
                {
                    if (this.Powers[i].Use != UseType.OnTurn)
                    {
                        return true;
                    }
                }
                for (int j = 0; j < this.Powers.Length; j++)
                {
                    if (this.Powers[j].Aid != AidType.None)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public bool Shared { get; set; }

    public string SharedPower { get; set; }

    public int SharedPriority { get; set; }

    public CardSideType Side
    {
        get => 
            this.side;
        set
        {
            this.side = value;
        }
    }

    public Vector2 Size
    {
        get
        {
            float x = 0f;
            float y = 0f;
            Transform transform = base.transform.FindChild("Front/Art");
            if (transform != null)
            {
                BoxCollider2D component = transform.GetComponent<BoxCollider2D>();
                if (component != null)
                {
                    x = component.size.x * base.transform.localScale.x;
                    y = component.size.y * base.transform.localScale.y;
                }
            }
            return new Vector2(x, y);
        }
    }

    public string SortingLayer
    {
        get
        {
            Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
            if (componentsInChildren.Length > 0)
            {
                return componentsInChildren[0].sortingLayerName;
            }
            return null;
        }
        set
        {
            Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].sortingLayerName = value;
            }
        }
    }

    public int SortingOrder
    {
        get
        {
            Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
            if (componentsInChildren.Length > 0)
            {
                return componentsInChildren[0].sortingOrder;
            }
            return 0;
        }
        set
        {
            Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].sortingOrder = value;
            }
        }
    }

    public bool Visible { get; private set; }

    public bool Zoomed =>
        this.isZoomed;
}

