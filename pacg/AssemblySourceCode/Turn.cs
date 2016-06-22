using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Turn
{
    private static BlackBoard blackBoard = new BlackBoard("_TURN_BB");
    private static CallbackStack callbacks = new CallbackStack();
    private static CallbackStack cancelStates = new CallbackStack(2);
    private static BlackBoard checkBoard = new BlackBoard("_TURN_CB");
    private static SkillCheckType combatSkill;
    private static int current;
    private static List<TraitType> damageList = new List<TraitType>(5);
    private static List<DamageData> damageQueue = new List<DamageData>(4);
    private static List<DiceType> diceList = new List<DiceType>(0x19);
    private static int diceTarget = 0;
    private static TurnStateData flexStateData = null;
    private static Card focusedCard = null;
    private static readonly string ID = "_TURN";
    private static int initialcharacter;
    private static TurnStateIteratorStack iterators = new TurnStateIteratorStack();
    private static int lastNumberValue = 0;
    private static int number;
    private static TurnPhaseType phase = TurnPhaseType.Blessing;
    private static TurnPowerList powerList = new TurnPowerList();
    private static List<GameStateType> returnStates = new List<GameStateType>(4);
    private static List<SkillCheckType> skillList = new List<SkillCheckType>(5);
    private static GameStateType state = GameStateType.Blessing;
    private static GameState stateDelegate = null;

    static Turn()
    {
        Clear();
    }

    public static void AddTrait(TraitType trait)
    {
        DamageTraits.Add(trait);
        Validate = true;
    }

    public static void AddTraits(TraitType[] traits)
    {
        DamageTraits.AddRange(traits);
        if (traits.Length > 0)
        {
            Validate = true;
        }
    }

    public static void Cancel()
    {
        if (stateDelegate != null)
        {
            stateDelegate.Cancel();
        }
    }

    public static void CancelAllPowers(bool allCharacters, bool includeAutomatic)
    {
        powerList.CancelCharacterPowers(allCharacters, includeAutomatic);
        powerList.CancelLocationPowers();
    }

    private static void Clear()
    {
        ClearCheckData();
        ClearCombatData();
        ClearEncounterData();
        ClearDamageData();
        TurnStackFrame.Clear();
        Started = false;
        Target = 0;
        Close = false;
        CloseType = CloseType.None;
        Explore = false;
        EncounterType = EncounterType.None;
        End = false;
        Discard = false;
        Pass = false;
        Phase = TurnPhaseType.None;
        RechargeReason = GameReasonType.None;
        EndReason = GameReasonType.None;
        SummonsType = SummonsType.None;
        SummonsMonster = null;
        SummonsSource = null;
        SummonsLocation = LocationType.None;
        SwitchType = SwitchType.None;
        TargetType = TargetType.None;
        returnStates.Clear();
        RechargePositionType = DeckPositionType.None;
        OptionalTarget = TargetPanelType.None;
        DiceTotal = 0;
        DiceTarget = 0;
        damageQueue.Clear();
        DamageTargetAmount = 0;
        LastCombatResult = CombatResultType.None;
        NumCombatUndefeats = 0;
        NumCombatEvades = 0;
        LastCombatDamage = 0;
        cancelStates.Clear();
        PendingDoneEvent = null;
        EmptyLayoutDecks = true;
        DiceRolls = 0;
        CountExplores = 0;
        BlackBoard.Clear();
        callbacks.Clear();
        iterators.Clear();
        Game.Events.Clear();
        powerList.Clear();
        DestructiveActionsCount = 0;
        EncounteredLocation = string.Empty;
    }

    public static void ClearCheckData()
    {
        if ((CombatStage == TurnCombatStageType.PreEncounter) || (CombatStage == TurnCombatStageType.None))
        {
            Defeat = false;
        }
        RollReason = RollType.PlayerControlled;
        Reroll = Guid.Empty;
        LastCheck = SkillCheckType.None;
        LastCheckDice = null;
        LastCheckBonus = 0;
        LastDiceModifier = 0;
        CheckBoard.Clear();
        Dice.Clear();
        CheckParticipants.Clear();
        DiceBonus = 0;
        BonusCheckDice = 0;
        Check = SkillCheckType.None;
        Checks = null;
        CheckName = null;
        powerList.Clear(PowerCooldownType.Check);
    }

    public static void ClearCombatData()
    {
        Armour = null;
        Shield = null;
        Weapon1 = null;
        Weapon2 = null;
        WeaponHands = 0;
        WeaponRanged = false;
        WeaponUnarmed = false;
        Spell = null;
        Item = null;
        if (Party.Characters.Count > 0)
        {
            combatSkill = Owner.GetCombatSkill();
        }
        DiceTargetAdjustment = 0;
    }

    public static void ClearDamageData()
    {
        Damage = 0;
        DamageDiscarded = 0;
        CombatDelta = 0;
        DamageTraits.Clear();
        DamageTargetType = DamageTargetType.None;
        DamageFromEnemy = false;
        DamageReduction = true;
        PriorityCardType = CardType.None;
    }

    public static void ClearEncounterData()
    {
        CombatStage = TurnCombatStageType.None;
        NumCheckSequences = 0;
        CombatCheckSequence = 1;
        if (!Summons)
        {
            EvadeDeclined = false;
        }
        Evade = false;
        Banish = false;
        Defeat = false;
        Pass = true;
        Summons = false;
        Disposed = false;
        powerList.Clear(PowerCooldownType.Encounter);
        EncounteredGuid = Guid.Empty;
        if (Scenario.Current != null)
        {
            Scenario.Current.OnEncounterComplete();
        }
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Party.Characters[i].OnEncounterComplete();
        }
    }

    public static void ClearStartTurnData()
    {
        powerList.Clear(PowerCooldownType.StartAndEndTurn);
    }

    public static void ClearTurnData()
    {
        powerList.Clear(PowerCooldownType.EndTurn);
        powerList.Clear(PowerCooldownType.StartAndEndTurn);
    }

    public static void Commit()
    {
        if (stateDelegate != null)
        {
            stateDelegate.Commit();
        }
    }

    public static DamageData DequeueData()
    {
        DamageData data = null;
        if (damageQueue.Count > 0)
        {
            data = damageQueue[0];
            damageQueue.RemoveAt(0);
            Damage = data.Damage;
            damageList = data.DamageTraits;
            DamageFromEnemy = data.DamageFromEnemy;
            DamageReduction = data.DamageReduction;
            DamageTargetType = data.DamageTargetType;
            PriorityCardType = data.PriorityCardType;
        }
        return data;
    }

    public static void EnqueueDamageData()
    {
        if (Damage > 0)
        {
            DamageData item = new DamageData {
                Damage = Damage,
                DamageTraits = new List<TraitType>(DamageTraits),
                DamageFromEnemy = DamageFromEnemy,
                DamageReduction = DamageReduction,
                DamageTargetType = DamageTargetType,
                PriorityCardType = PriorityCardType
            };
            damageQueue.Add(item);
            ClearDamageData();
        }
    }

    public static TurnStateData GetStateData() => 
        flexStateData;

    public static void GotoCancelDestination()
    {
        TurnStateCallback callback = PopCancelDestination();
        if (callback != null)
        {
            SetStateData(null);
            PopStateDestination();
            callback.Invoke();
        }
    }

    public static void GotoStateDestination()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if ((window != null) && EmptyLayoutDecks)
        {
            window.ProcessRechargableCards();
            window.ProcessLayoutDecks();
        }
        TurnStateCallback callback = callbacks.Pop();
        SetStateData(null);
        if (callback != null)
        {
            PopCancelDestination();
            callback.Invoke();
        }
    }

    public static bool HasDamageData() => 
        (damageQueue.Count > 0);

    public static bool IsActionAllowed(ActionType action, Card card)
    {
        if (card == null)
        {
            return false;
        }
        if (card.Deck == null)
        {
            return false;
        }
        if (card.Deck == Location.Current.Deck)
        {
            return false;
        }
        if (card.Displayed)
        {
            return false;
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (window.dicePanel.Rolling)
            {
                return false;
            }
            if (window.mapPanel.Visible)
            {
                return false;
            }
            if ((Explore && ((state == GameStateType.Setup) || (state == GameStateType.Finish))) && (((window.layoutDiscard.Deck.Count > 0) || (window.layoutRecharge.Deck.Count > 0)) || ((window.layoutBury.Deck.Count > 0) || (window.layoutBanish.Deck.Count > 0))))
            {
                return false;
            }
        }
        return stateDelegate?.IsActionAllowed(action, card);
    }

    public static bool IsCancelAllowed() => 
        ((stateDelegate != null) && stateDelegate.IsCancelAllowed());

    public static bool IsCardRerollEmpty() => 
        (Reroll == Guid.Empty);

    public static bool IsIteratorInProgress() => 
        (Iterators.Count > 0);

    public static bool IsPowerActive(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }
        return powerList.Contains(id);
    }

    public static bool IsProceedAllowed() => 
        ((stateDelegate != null) && stateDelegate.IsProceedAllowed());

    public static bool IsResolved() => 
        ((stateDelegate != null) && stateDelegate.IsResolved());

    public static bool IsResolveSuccess() => 
        ((stateDelegate != null) && stateDelegate.IsResolveSuccess());

    public static bool IsState(GameStateType type) => 
        ((stateDelegate != null) && stateDelegate.IsState(type));

    public static bool IsSwitchingCharacters() => 
        GuiPanelSwitch.Switching;

    public static void MarkPowerActive(Power power, bool isActive)
    {
        if (power != null)
        {
            if (isActive)
            {
                powerList.Add(power);
                PowerLink component = power.GetComponent<PowerLink>();
                if (component != null)
                {
                    component.Activate(power);
                }
            }
            else
            {
                powerList.Remove(power);
                PowerLink link2 = power.GetComponent<PowerLink>();
                if (link2 != null)
                {
                    link2.Deactivate(power);
                }
            }
        }
    }

    public static void Next()
    {
        Party.OnTurnCompleted();
        Clear();
        current = Party.GetNextLivingMemberIndex(current);
        Number = current;
    }

    public static void OnLoadData()
    {
        byte[] buffer;
        if (Game.GetObjectData(ID, out buffer))
        {
            ByteStream bs = new ByteStream(buffer);
            OnLoadData(bs);
            BlackBoard.OnLoadData();
            CheckBoard.OnLoadData();
        }
    }

    public static void OnLoadData(ByteStream bs)
    {
        if (bs != null)
        {
            bs.ReadInt();
            Started = bs.ReadBool();
            Disposed = bs.ReadBool();
            EmptyLayoutDecks = bs.ReadBool();
            RollReason = (RollType) bs.ReadInt();
            state = (GameStateType) bs.ReadInt();
            Number = bs.ReadInt();
            Current = bs.ReadInt();
            Switch = bs.ReadInt();
            InitialCharacter = bs.ReadInt();
            InitialLocation = bs.ReadString();
            DiceRolls = bs.ReadInt();
            int capacity = bs.ReadInt();
            for (int i = 0; i < capacity; i++)
            {
                Dice.Add((DiceType) bs.ReadInt());
            }
            DiceBonus = bs.ReadInt();
            BonusCheckDice = bs.ReadInt();
            diceTarget = bs.ReadInt();
            DiceTargetAdjustment = bs.ReadInt();
            DiceTotal = bs.ReadInt();
            LastCombatResult = (CombatResultType) bs.ReadInt();
            NumCombatUndefeats = bs.ReadInt();
            NumCombatEvades = bs.ReadInt();
            LastCombatDamage = bs.ReadInt();
            capacity = bs.ReadInt();
            Checks = new SkillCheckValueType[capacity];
            for (int j = 0; j < capacity; j++)
            {
                SkillCheckType skill = (SkillCheckType) bs.ReadInt();
                int rank = bs.ReadInt();
                Checks[j] = new SkillCheckValueType(skill, rank);
            }
            Check = (SkillCheckType) bs.ReadInt();
            capacity = bs.ReadInt();
            for (int k = 0; k < capacity; k++)
            {
                CheckParticipants.Add((SkillCheckType) bs.ReadInt());
            }
            CheckName = bs.ReadString();
            NumCheckSequences = bs.ReadInt();
            CombatCheckSequence = bs.ReadInt();
            combatSkill = (SkillCheckType) bs.ReadInt();
            Weapon1 = bs.ReadString();
            Weapon2 = bs.ReadString();
            WeaponHands = bs.ReadInt();
            WeaponRanged = bs.ReadBool();
            WeaponUnarmed = bs.ReadBool();
            Item = bs.ReadString();
            Spell = bs.ReadString();
            Armour = bs.ReadString();
            Shield = bs.ReadString();
            Explore = bs.ReadBool();
            End = bs.ReadBool();
            Discard = bs.ReadBool();
            Close = bs.ReadBool();
            Pass = bs.ReadBool();
            bs.ReadBool();
            OptionalTarget = (TargetPanelType) bs.ReadInt();
            EvadeDeclined = bs.ReadBool();
            Evade = bs.ReadBool();
            Banish = bs.ReadBool();
            Defeat = bs.ReadBool();
            Phase = (TurnPhaseType) bs.ReadInt();
            CombatStage = (TurnCombatStageType) bs.ReadInt();
            Map = bs.ReadBool();
            Target = bs.ReadInt();
            RechargeReason = (GameReasonType) bs.ReadInt();
            EndReason = (GameReasonType) bs.ReadInt();
            Summons = bs.ReadBool();
            SummonsMonster = bs.ReadString();
            SummonsSource = bs.ReadString();
            SummonsType = (SummonsType) bs.ReadInt();
            EncounterType = (EncounterType) bs.ReadInt();
            SummonsLocation = (LocationType) bs.ReadInt();
            CloseType = (CloseType) bs.ReadInt();
            Damage = bs.ReadInt();
            DamageDiscarded = bs.ReadInt();
            CombatDelta = bs.ReadInt();
            capacity = bs.ReadInt();
            for (int m = 0; m < capacity; m++)
            {
                DamageTraits.Add((TraitType) bs.ReadInt());
            }
            DamageReduction = bs.ReadBool();
            DamageTargetType = (DamageTargetType) bs.ReadInt();
            DamageTargetAmount = bs.ReadInt();
            DamageFromEnemy = bs.ReadBool();
            capacity = bs.ReadInt();
            for (int n = 0; n < capacity; n++)
            {
                DamageData item = new DamageData(bs);
                damageQueue.Add(item);
            }
            RechargePositionType = (DeckPositionType) bs.ReadInt();
            PriorityCardType = (CardType) bs.ReadInt();
            LastCheck = (SkillCheckType) bs.ReadInt();
            capacity = bs.ReadInt();
            LastCheckDice = new DiceType[capacity];
            for (int num8 = 0; num8 < capacity; num8++)
            {
                LastCheckDice[num8] = (DiceType) bs.ReadInt();
            }
            LastCheckBonus = bs.ReadInt();
            LastDiceModifier = bs.ReadInt();
            SwitchType = (SwitchType) bs.ReadInt();
            TargetType = (TargetType) bs.ReadInt();
            CountExplores = bs.ReadInt();
            if (bs.ReadBool())
            {
                FocusedCard = CardData.CardFromStream(bs);
                FocusedCard.Show(false);
            }
            flexStateData = TurnStateData.FromStream(bs);
            PendingDoneEvent = TurnStateCallback.FromStream(bs);
            cancelStates = CallbackStack.FromStream(bs);
            powerList = TurnPowerList.FromStream(bs);
            callbacks = CallbackStack.FromStream(bs);
            iterators = TurnStateIteratorStack.FromStream(bs);
            capacity = bs.ReadInt();
            returnStates = new List<GameStateType>(capacity);
            for (int num9 = 0; num9 < capacity; num9++)
            {
                GameStateType type2 = (GameStateType) bs.ReadInt();
                returnStates.Add(type2);
            }
            DestructiveActionsCount = bs.ReadInt();
            EncounteredGuid = bs.ReadGuid();
            EncounteredLocation = bs.ReadString();
            Reroll = bs.ReadGuid();
        }
    }

    public static void OnSaveData()
    {
        ByteStream bs = new ByteStream();
        OnSaveData(bs);
        Game.SetObjectData(ID, bs.ToArray());
        BlackBoard.OnSaveData();
        CheckBoard.OnSaveData();
    }

    public static void OnSaveData(ByteStream bs)
    {
        if (bs != null)
        {
            bs.WriteInt(1);
            bs.WriteBool(Started);
            bs.WriteBool(Disposed);
            bs.WriteBool(EmptyLayoutDecks);
            bs.WriteInt((int) RollReason);
            bs.WriteInt((int) State);
            bs.WriteInt(Number);
            bs.WriteInt(Current);
            bs.WriteInt(Switch);
            bs.WriteInt(InitialCharacter);
            bs.WriteString(InitialLocation);
            bs.WriteInt(DiceRolls);
            bs.WriteInt(Dice.Count);
            for (int i = 0; i < Dice.Count; i++)
            {
                bs.WriteInt(Dice[i]);
            }
            bs.WriteInt(DiceBonus);
            bs.WriteInt(BonusCheckDice);
            bs.WriteInt(diceTarget);
            bs.WriteInt(DiceTargetAdjustment);
            bs.WriteInt(DiceTotal);
            bs.WriteInt((int) LastCombatResult);
            bs.WriteInt(NumCombatUndefeats);
            bs.WriteInt(NumCombatEvades);
            bs.WriteInt(LastCombatDamage);
            WriteCheckArray(bs, Checks);
            bs.WriteInt((int) Check);
            bs.WriteInt(CheckParticipants.Count);
            for (int j = 0; j < CheckParticipants.Count; j++)
            {
                bs.WriteInt(CheckParticipants[j]);
            }
            bs.WriteString(CheckName);
            bs.WriteInt(NumCheckSequences);
            bs.WriteInt(CombatCheckSequence);
            bs.WriteInt((int) CombatSkill);
            bs.WriteString(Weapon1);
            bs.WriteString(Weapon2);
            bs.WriteInt(WeaponHands);
            bs.WriteBool(WeaponRanged);
            bs.WriteBool(WeaponUnarmed);
            bs.WriteString(Item);
            bs.WriteString(Spell);
            bs.WriteString(Armour);
            bs.WriteString(Shield);
            bs.WriteBool(Explore);
            bs.WriteBool(End);
            bs.WriteBool(Discard);
            bs.WriteBool(Close);
            bs.WriteBool(Pass);
            bs.WriteBool(false);
            bs.WriteInt((int) OptionalTarget);
            bs.WriteBool(EvadeDeclined);
            bs.WriteBool(Evade);
            bs.WriteBool(Banish);
            bs.WriteBool(Defeat);
            bs.WriteInt((int) Phase);
            bs.WriteInt((int) CombatStage);
            bs.WriteBool(Map);
            bs.WriteInt(Target);
            bs.WriteInt((int) RechargeReason);
            bs.WriteInt((int) EndReason);
            bs.WriteBool(Summons);
            bs.WriteString(SummonsMonster);
            bs.WriteString(SummonsSource);
            bs.WriteInt((int) SummonsType);
            bs.WriteInt((int) EncounterType);
            bs.WriteInt((int) SummonsLocation);
            bs.WriteInt((int) CloseType);
            bs.WriteInt(Damage);
            bs.WriteInt(DamageDiscarded);
            bs.WriteInt(CombatDelta);
            bs.WriteInt(DamageTraits.Count);
            for (int k = 0; k < DamageTraits.Count; k++)
            {
                bs.WriteInt(DamageTraits[k]);
            }
            bs.WriteBool(DamageReduction);
            bs.WriteInt((int) DamageTargetType);
            bs.WriteInt(DamageTargetAmount);
            bs.WriteBool(DamageFromEnemy);
            bs.WriteInt(damageQueue.Count);
            for (int m = 0; m < damageQueue.Count; m++)
            {
                damageQueue[m].SaveData(bs);
            }
            bs.WriteInt((int) RechargePositionType);
            bs.WriteInt((int) PriorityCardType);
            bs.WriteInt((int) LastCheck);
            WriteDiceArray(bs, LastCheckDice);
            bs.WriteInt(LastCheckBonus);
            bs.WriteInt(LastDiceModifier);
            bs.WriteInt((int) SwitchType);
            bs.WriteInt((int) TargetType);
            bs.WriteInt(CountExplores);
            bs.WriteBool(focusedCard != null);
            if (focusedCard != null)
            {
                CardData.CardToStream(focusedCard, bs);
            }
            WriteTurnStateData(bs, flexStateData);
            WriteTurnStateCallback(bs, PendingDoneEvent);
            cancelStates.ToStream(bs);
            powerList.ToStream(bs);
            callbacks.ToStream(bs);
            iterators.ToStream(bs);
            WriteReturnStates(bs, returnStates);
            bs.WriteInt(DestructiveActionsCount);
            bs.WriteGuid(EncounteredGuid);
            bs.WriteString(EncounteredLocation);
            bs.WriteGuid(Reroll);
        }
    }

    public static void Park(bool isParked)
    {
        if (isParked)
        {
            TurnStackFrame.Push();
        }
        else
        {
            TurnStackFrame.Pop();
        }
    }

    public static TurnStateCallback PeekCancelDestination() => 
        cancelStates.Peek();

    public static TurnStateCallback PeekStateDestination() => 
        callbacks.Peek();

    public static TurnStateCallback PopCancelDestination() => 
        cancelStates.Pop();

    public static GameStateType PopReturnState()
    {
        GameStateType none = GameStateType.None;
        if (returnStates.Count > 0)
        {
            none = returnStates[returnStates.Count - 1];
            returnStates.RemoveAt(returnStates.Count - 1);
        }
        return none;
    }

    public static TurnStateCallback PopStateDestination() => 
        callbacks.Pop();

    public static void Proceed()
    {
        if (stateDelegate != null)
        {
            stateDelegate.Proceed();
        }
    }

    public static void PushCancelDestination(GameStateType next)
    {
        PushCancelDestination(new TurnStateCallback(next));
    }

    public static void PushCancelDestination(TurnStateCallback callback)
    {
        cancelStates.Push(callback);
    }

    public static void PushReturnState()
    {
        PushReturnState(State);
    }

    public static void PushReturnState(GameStateType state)
    {
        if (returnStates.Count <= CallbackStack.MAX_STACK_SIZE)
        {
            returnStates.Add(state);
        }
    }

    public static void PushStateDestination(GameStateType state)
    {
        TurnStateCallback callback = new TurnStateCallback(state);
        callbacks.Push(callback);
    }

    public static void PushStateDestination(TurnStateCallback callback)
    {
        callbacks.Push(callback);
    }

    public static void Refresh()
    {
        if (stateDelegate != null)
        {
            stateDelegate.Refresh();
        }
    }

    public static void RemoveTraits(TraitType[] traits)
    {
        for (int i = 0; i < traits.Length; i++)
        {
            DamageTraits.Remove(traits[i]);
        }
        if (traits.Length > 0)
        {
            Validate = true;
        }
    }

    public static void Reset()
    {
        Clear();
        Number = 0;
        Current = 0;
        Switch = 0;
        InitialCharacter = 0;
        InitialLocation = null;
        state = GameStateType.Blessing;
        stateDelegate = null;
        phase = TurnPhaseType.None;
        Map = false;
    }

    public static void Resolve()
    {
        if ((Checks != null) && (Checks.Length > 0))
        {
            Party.OnCheckCompleted();
        }
        if (stateDelegate != null)
        {
            stateDelegate.Resolve();
        }
    }

    public static void ReturnToReturnState()
    {
        GameStateType type = PopReturnState();
        if (type != GameStateType.None)
        {
            State = type;
        }
    }

    public static int Roll(int numDice, int diceTotal)
    {
        DiceTotal = Mathf.Max(diceTotal + DiceBonus, 0);
        return DiceTotal;
    }

    public static void Select(Character character, bool isSelected)
    {
        if (isSelected)
        {
            lastNumberValue = Number;
            number = Party.IndexOf(character.ID);
        }
        else
        {
            number = lastNumberValue;
        }
    }

    public static void SetEncounteredInformation()
    {
        if (!Rules.IsCardSummons(Card))
        {
            EncounteredLocation = Location.Current.ID;
        }
        EncounteredGuid = Card.GUID;
    }

    public static void SetStateData(TurnStateData data)
    {
        flexStateData = data;
    }

    public static void SwitchCharacter(int number)
    {
        if (((Number != number) && (number >= 0)) && (number < Party.Characters.Count))
        {
            Number = number;
            if (Game.GameType == GameType.LocalMultiPlayer)
            {
                Switch = number;
                Game.UI.SwitchPanel.Show(true);
            }
            else
            {
                UI.Window.Refresh();
            }
        }
    }

    private static void WriteCheckArray(ByteStream bs, SkillCheckValueType[] checks)
    {
        if (checks == null)
        {
            bs.WriteInt(0);
        }
        else
        {
            bs.WriteInt(checks.Length);
            for (int i = 0; i < checks.Length; i++)
            {
                bs.WriteInt((int) checks[i].skill);
                bs.WriteInt(checks[i].rank);
            }
        }
    }

    private static void WriteDiceArray(ByteStream bs, DiceType[] dice)
    {
        if (dice == null)
        {
            bs.WriteInt(0);
        }
        else
        {
            bs.WriteInt(dice.Length);
            for (int i = 0; i < dice.Length; i++)
            {
                bs.WriteInt((int) dice[i]);
            }
        }
    }

    private static void WriteReturnStates(ByteStream bs, IList<GameStateType> states)
    {
        if (states == null)
        {
            bs.WriteInt(0);
        }
        else
        {
            bs.WriteInt(states.Count);
            for (int i = 0; i < states.Count; i++)
            {
                bs.WriteInt(states[i]);
            }
        }
    }

    private static void WriteTraitArray(ByteStream bs, TraitType[] traits)
    {
        if (traits == null)
        {
            bs.WriteInt(0);
        }
        else
        {
            bs.WriteInt(traits.Length);
            for (int i = 0; i < traits.Length; i++)
            {
                bs.WriteInt((int) traits[i]);
            }
        }
    }

    private static void WriteTurnStateCallback(ByteStream bs, TurnStateCallback callback)
    {
        if (callback == null)
        {
            bs.WriteBool(false);
        }
        else
        {
            callback.ToStream(bs);
        }
    }

    private static void WriteTurnStateData(ByteStream bs, TurnStateData data)
    {
        if (data == null)
        {
            bs.WriteBool(false);
        }
        else
        {
            data.ToStream(bs);
        }
    }

    public static string Armour
    {
        [CompilerGenerated]
        get => 
            <Armour>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Armour>k__BackingField = value;
        }
    }

    public static bool Banish
    {
        [CompilerGenerated]
        get => 
            <Banish>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Banish>k__BackingField = value;
        }
    }

    public static BlackBoard BlackBoard =>
        blackBoard;

    public static int BonusCheckDice
    {
        [CompilerGenerated]
        get => 
            <BonusCheckDice>k__BackingField;
        [CompilerGenerated]
        set
        {
            <BonusCheckDice>k__BackingField = value;
        }
    }

    public static bool Canceling
    {
        [CompilerGenerated]
        get => 
            <Canceling>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Canceling>k__BackingField = value;
        }
    }

    public static Card Card
    {
        get
        {
            if (FocusedCard != null)
            {
                return FocusedCard;
            }
            if (Location.Current == null)
            {
                return null;
            }
            if (Location.Current.Deck == null)
            {
                return null;
            }
            if ((State == GameStateType.Recharge) && (Owner.Recharge.Count > 0))
            {
                return Owner.Recharge[0];
            }
            if (State == GameStateType.Close)
            {
                return Location.Current.Card;
            }
            if (Location.Current.Deck.Count <= 0)
            {
                return Location.Current.Card;
            }
            return Location.Current.Deck[0];
        }
    }

    public static Character Character =>
        Party.Characters[Number];

    public static SkillCheckType Check
    {
        [CompilerGenerated]
        get => 
            <Check>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Check>k__BackingField = value;
        }
    }

    public static BlackBoard CheckBoard =>
        checkBoard;

    public static string CheckName
    {
        [CompilerGenerated]
        get => 
            <CheckName>k__BackingField;
        [CompilerGenerated]
        set
        {
            <CheckName>k__BackingField = value;
        }
    }

    public static List<SkillCheckType> CheckParticipants =>
        skillList;

    public static SkillCheckValueType[] Checks
    {
        [CompilerGenerated]
        get => 
            <Checks>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Checks>k__BackingField = value;
        }
    }

    public static bool Close
    {
        [CompilerGenerated]
        get => 
            <Close>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Close>k__BackingField = value;
        }
    }

    public static CloseType CloseType
    {
        [CompilerGenerated]
        get => 
            <CloseType>k__BackingField;
        [CompilerGenerated]
        set
        {
            <CloseType>k__BackingField = value;
        }
    }

    public static int CombatCheckSequence
    {
        [CompilerGenerated]
        get => 
            <CombatCheckSequence>k__BackingField;
        [CompilerGenerated]
        set
        {
            <CombatCheckSequence>k__BackingField = value;
        }
    }

    public static int CombatDelta
    {
        [CompilerGenerated]
        get => 
            <CombatDelta>k__BackingField;
        [CompilerGenerated]
        set
        {
            <CombatDelta>k__BackingField = value;
        }
    }

    public static SkillCheckType CombatSkill
    {
        get => 
            combatSkill;
        set
        {
            if (combatSkill != value)
            {
                Validate = true;
            }
            combatSkill = value;
        }
    }

    public static TurnCombatStageType CombatStage
    {
        [CompilerGenerated]
        get => 
            <CombatStage>k__BackingField;
        [CompilerGenerated]
        set
        {
            <CombatStage>k__BackingField = value;
        }
    }

    public static int CountExplores
    {
        [CompilerGenerated]
        get => 
            <CountExplores>k__BackingField;
        [CompilerGenerated]
        set
        {
            <CountExplores>k__BackingField = value;
        }
    }

    public static int Current
    {
        get => 
            current;
        set
        {
            current = value;
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.shadePanel.Refresh();
            }
        }
    }

    public static int Damage
    {
        [CompilerGenerated]
        get => 
            <Damage>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Damage>k__BackingField = value;
        }
    }

    public static int DamageDiscarded
    {
        [CompilerGenerated]
        get => 
            <DamageDiscarded>k__BackingField;
        [CompilerGenerated]
        set
        {
            <DamageDiscarded>k__BackingField = value;
        }
    }

    public static bool DamageFromEnemy
    {
        [CompilerGenerated]
        get => 
            <DamageFromEnemy>k__BackingField;
        [CompilerGenerated]
        set
        {
            <DamageFromEnemy>k__BackingField = value;
        }
    }

    public static bool DamageReduction
    {
        [CompilerGenerated]
        get => 
            <DamageReduction>k__BackingField;
        [CompilerGenerated]
        set
        {
            <DamageReduction>k__BackingField = value;
        }
    }

    public static int DamageTargetAmount
    {
        [CompilerGenerated]
        get => 
            <DamageTargetAmount>k__BackingField;
        [CompilerGenerated]
        set
        {
            <DamageTargetAmount>k__BackingField = value;
        }
    }

    public static DamageTargetType DamageTargetType
    {
        [CompilerGenerated]
        get => 
            <DamageTargetType>k__BackingField;
        [CompilerGenerated]
        set
        {
            <DamageTargetType>k__BackingField = value;
        }
    }

    public static List<TraitType> DamageTraits =>
        damageList;

    public static bool Defeat
    {
        [CompilerGenerated]
        get => 
            <Defeat>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Defeat>k__BackingField = value;
        }
    }

    public static int DestructiveActionsCount
    {
        [CompilerGenerated]
        get => 
            <DestructiveActionsCount>k__BackingField;
        [CompilerGenerated]
        set
        {
            <DestructiveActionsCount>k__BackingField = value;
        }
    }

    public static List<DiceType> Dice =>
        diceList;

    public static int DiceBonus
    {
        [CompilerGenerated]
        get => 
            <DiceBonus>k__BackingField;
        [CompilerGenerated]
        set
        {
            <DiceBonus>k__BackingField = value;
        }
    }

    public static int DiceRolls
    {
        [CompilerGenerated]
        get => 
            <DiceRolls>k__BackingField;
        [CompilerGenerated]
        set
        {
            <DiceRolls>k__BackingField = value;
        }
    }

    public static int DiceTarget
    {
        get => 
            (diceTarget + DiceTargetAdjustment);
        set
        {
            diceTarget = value;
        }
    }

    public static int DiceTargetAdjustment
    {
        [CompilerGenerated]
        get => 
            <DiceTargetAdjustment>k__BackingField;
        [CompilerGenerated]
        set
        {
            <DiceTargetAdjustment>k__BackingField = value;
        }
    }

    public static int DiceTotal
    {
        [CompilerGenerated]
        get => 
            <DiceTotal>k__BackingField;
        [CompilerGenerated]
        private set
        {
            <DiceTotal>k__BackingField = value;
        }
    }

    public static bool Discard
    {
        [CompilerGenerated]
        get => 
            <Discard>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Discard>k__BackingField = value;
        }
    }

    public static bool Disposed
    {
        [CompilerGenerated]
        get => 
            <Disposed>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Disposed>k__BackingField = value;
        }
    }

    public static bool EmptyLayoutDecks
    {
        [CompilerGenerated]
        get => 
            <EmptyLayoutDecks>k__BackingField;
        [CompilerGenerated]
        set
        {
            <EmptyLayoutDecks>k__BackingField = value;
        }
    }

    public static Guid EncounteredGuid
    {
        [CompilerGenerated]
        get => 
            <EncounteredGuid>k__BackingField;
        [CompilerGenerated]
        set
        {
            <EncounteredGuid>k__BackingField = value;
        }
    }

    public static string EncounteredLocation
    {
        [CompilerGenerated]
        get => 
            <EncounteredLocation>k__BackingField;
        [CompilerGenerated]
        set
        {
            <EncounteredLocation>k__BackingField = value;
        }
    }

    public static EncounterType EncounterType
    {
        [CompilerGenerated]
        get => 
            <EncounterType>k__BackingField;
        [CompilerGenerated]
        set
        {
            <EncounterType>k__BackingField = value;
        }
    }

    public static bool End
    {
        [CompilerGenerated]
        get => 
            <End>k__BackingField;
        [CompilerGenerated]
        set
        {
            <End>k__BackingField = value;
        }
    }

    public static GameReasonType EndReason
    {
        [CompilerGenerated]
        get => 
            <EndReason>k__BackingField;
        [CompilerGenerated]
        set
        {
            <EndReason>k__BackingField = value;
        }
    }

    public static bool Evade
    {
        [CompilerGenerated]
        get => 
            <Evade>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Evade>k__BackingField = value;
        }
    }

    public static bool EvadeDeclined
    {
        [CompilerGenerated]
        get => 
            <EvadeDeclined>k__BackingField;
        [CompilerGenerated]
        set
        {
            <EvadeDeclined>k__BackingField = value;
        }
    }

    public static bool Explore
    {
        [CompilerGenerated]
        get => 
            <Explore>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Explore>k__BackingField = value;
        }
    }

    public static Card FocusedCard
    {
        get => 
            focusedCard;
        set
        {
            focusedCard = value;
            if (value != null)
            {
                focusedCard.SortingOrder = 10;
                GuiWindowLocation window = UI.Window as GuiWindowLocation;
                if (window != null)
                {
                    value.MoveCard(window.layoutLocation.transform.position, 0.3f, SoundEffectType.None).setEase(LeanTweenType.easeInOutQuad);
                    LeanTween.scale(value.gameObject, window.layoutLocation.Scale, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                }
            }
        }
    }

    public static int InitialCharacter
    {
        get => 
            initialcharacter;
        set
        {
            initialcharacter = value;
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.shadePanel.Refresh();
            }
        }
    }

    public static string InitialLocation
    {
        [CompilerGenerated]
        get => 
            <InitialLocation>k__BackingField;
        [CompilerGenerated]
        set
        {
            <InitialLocation>k__BackingField = value;
        }
    }

    public static string Item
    {
        [CompilerGenerated]
        get => 
            <Item>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Item>k__BackingField = value;
        }
    }

    public static TurnStateIteratorStack Iterators =>
        iterators;

    public static SkillCheckType LastCheck
    {
        [CompilerGenerated]
        get => 
            <LastCheck>k__BackingField;
        [CompilerGenerated]
        set
        {
            <LastCheck>k__BackingField = value;
        }
    }

    public static int LastCheckBonus
    {
        [CompilerGenerated]
        get => 
            <LastCheckBonus>k__BackingField;
        [CompilerGenerated]
        set
        {
            <LastCheckBonus>k__BackingField = value;
        }
    }

    public static DiceType[] LastCheckDice
    {
        [CompilerGenerated]
        get => 
            <LastCheckDice>k__BackingField;
        [CompilerGenerated]
        set
        {
            <LastCheckDice>k__BackingField = value;
        }
    }

    public static int LastCombatDamage
    {
        [CompilerGenerated]
        get => 
            <LastCombatDamage>k__BackingField;
        [CompilerGenerated]
        set
        {
            <LastCombatDamage>k__BackingField = value;
        }
    }

    public static CombatResultType LastCombatResult
    {
        [CompilerGenerated]
        get => 
            <LastCombatResult>k__BackingField;
        [CompilerGenerated]
        set
        {
            <LastCombatResult>k__BackingField = value;
        }
    }

    public static int LastDiceModifier
    {
        [CompilerGenerated]
        get => 
            <LastDiceModifier>k__BackingField;
        [CompilerGenerated]
        set
        {
            <LastDiceModifier>k__BackingField = value;
        }
    }

    public static bool Map
    {
        [CompilerGenerated]
        get => 
            <Map>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Map>k__BackingField = value;
        }
    }

    public static int Number
    {
        get => 
            number;
        set
        {
            number = value;
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.shadePanel.Refresh();
            }
        }
    }

    public static int NumCheckSequences
    {
        [CompilerGenerated]
        get => 
            <NumCheckSequences>k__BackingField;
        [CompilerGenerated]
        set
        {
            <NumCheckSequences>k__BackingField = value;
        }
    }

    public static int NumCombatEvades
    {
        [CompilerGenerated]
        get => 
            <NumCombatEvades>k__BackingField;
        [CompilerGenerated]
        set
        {
            <NumCombatEvades>k__BackingField = value;
        }
    }

    public static int NumCombatUndefeats
    {
        [CompilerGenerated]
        get => 
            <NumCombatUndefeats>k__BackingField;
        [CompilerGenerated]
        set
        {
            <NumCombatUndefeats>k__BackingField = value;
        }
    }

    public static TurnOperationType Operation
    {
        [CompilerGenerated]
        get => 
            <Operation>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Operation>k__BackingField = value;
        }
    }

    public static TargetPanelType OptionalTarget
    {
        [CompilerGenerated]
        get => 
            <OptionalTarget>k__BackingField;
        [CompilerGenerated]
        set
        {
            <OptionalTarget>k__BackingField = value;
        }
    }

    public static Character Owner =>
        Party.Characters[Current];

    public static bool Pass
    {
        [CompilerGenerated]
        get => 
            <Pass>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Pass>k__BackingField = value;
        }
    }

    public static TurnStateCallback PendingDoneEvent
    {
        [CompilerGenerated]
        get => 
            <PendingDoneEvent>k__BackingField;
        [CompilerGenerated]
        set
        {
            <PendingDoneEvent>k__BackingField = value;
        }
    }

    public static TurnPhaseType Phase
    {
        get => 
            phase;
        set
        {
            phase = value;
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.phasesPanel.Set(phase);
            }
        }
    }

    public static CardType PriorityCardType
    {
        [CompilerGenerated]
        get => 
            <PriorityCardType>k__BackingField;
        [CompilerGenerated]
        set
        {
            <PriorityCardType>k__BackingField = value;
        }
    }

    public static DeckPositionType RechargePositionType
    {
        [CompilerGenerated]
        get => 
            <RechargePositionType>k__BackingField;
        [CompilerGenerated]
        set
        {
            <RechargePositionType>k__BackingField = value;
        }
    }

    public static GameReasonType RechargeReason
    {
        [CompilerGenerated]
        get => 
            <RechargeReason>k__BackingField;
        [CompilerGenerated]
        set
        {
            <RechargeReason>k__BackingField = value;
        }
    }

    public static Guid Reroll
    {
        [CompilerGenerated]
        get => 
            <Reroll>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Reroll>k__BackingField = value;
        }
    }

    public static GameStateType ReturnState
    {
        get
        {
            if (returnStates.Count > 0)
            {
                return returnStates[returnStates.Count - 1];
            }
            return GameStateType.None;
        }
    }

    public static RollType RollReason
    {
        [CompilerGenerated]
        get => 
            <RollReason>k__BackingField;
        [CompilerGenerated]
        set
        {
            <RollReason>k__BackingField = value;
        }
    }

    public static string Shield
    {
        [CompilerGenerated]
        get => 
            <Shield>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Shield>k__BackingField = value;
        }
    }

    public static string Spell
    {
        [CompilerGenerated]
        get => 
            <Spell>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Spell>k__BackingField = value;
        }
    }

    public static bool Started
    {
        [CompilerGenerated]
        get => 
            <Started>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Started>k__BackingField = value;
        }
    }

    public static GameStateType State
    {
        get => 
            state;
        set
        {
            bool flag = state == value;
            if (stateDelegate != null)
            {
                stateDelegate.Exit(value);
            }
            state = value;
            stateDelegate = GameStateFactory.Create(state);
            if (stateDelegate != null)
            {
                stateDelegate.Enter();
            }
            if ((stateDelegate != null) && !flag)
            {
                stateDelegate.Commit();
            }
        }
    }

    public static bool Summons
    {
        [CompilerGenerated]
        get => 
            <Summons>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Summons>k__BackingField = value;
        }
    }

    public static LocationType SummonsLocation
    {
        [CompilerGenerated]
        get => 
            <SummonsLocation>k__BackingField;
        [CompilerGenerated]
        set
        {
            <SummonsLocation>k__BackingField = value;
        }
    }

    public static string SummonsMonster
    {
        [CompilerGenerated]
        get => 
            <SummonsMonster>k__BackingField;
        [CompilerGenerated]
        set
        {
            <SummonsMonster>k__BackingField = value;
        }
    }

    public static string SummonsSource
    {
        [CompilerGenerated]
        get => 
            <SummonsSource>k__BackingField;
        [CompilerGenerated]
        set
        {
            <SummonsSource>k__BackingField = value;
        }
    }

    public static SummonsType SummonsType
    {
        [CompilerGenerated]
        get => 
            <SummonsType>k__BackingField;
        [CompilerGenerated]
        set
        {
            <SummonsType>k__BackingField = value;
        }
    }

    public static int Switch
    {
        [CompilerGenerated]
        get => 
            <Switch>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Switch>k__BackingField = value;
        }
    }

    public static SwitchType SwitchType
    {
        [CompilerGenerated]
        get => 
            <SwitchType>k__BackingField;
        [CompilerGenerated]
        set
        {
            <SwitchType>k__BackingField = value;
        }
    }

    public static int Target
    {
        [CompilerGenerated]
        get => 
            <Target>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Target>k__BackingField = value;
        }
    }

    public static TargetType TargetType
    {
        [CompilerGenerated]
        get => 
            <TargetType>k__BackingField;
        [CompilerGenerated]
        set
        {
            <TargetType>k__BackingField = value;
        }
    }

    public static bool Validate
    {
        [CompilerGenerated]
        get => 
            <Validate>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Validate>k__BackingField = value;
        }
    }

    public static string Weapon1
    {
        [CompilerGenerated]
        get => 
            <Weapon1>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Weapon1>k__BackingField = value;
        }
    }

    public static string Weapon2
    {
        [CompilerGenerated]
        get => 
            <Weapon2>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Weapon2>k__BackingField = value;
        }
    }

    public static int WeaponHands
    {
        [CompilerGenerated]
        get => 
            <WeaponHands>k__BackingField;
        [CompilerGenerated]
        set
        {
            <WeaponHands>k__BackingField = value;
        }
    }

    public static bool WeaponRanged
    {
        [CompilerGenerated]
        get => 
            <WeaponRanged>k__BackingField;
        [CompilerGenerated]
        set
        {
            <WeaponRanged>k__BackingField = value;
        }
    }

    public static bool WeaponUnarmed
    {
        [CompilerGenerated]
        get => 
            <WeaponUnarmed>k__BackingField;
        [CompilerGenerated]
        set
        {
            <WeaponUnarmed>k__BackingField = value;
        }
    }
}

