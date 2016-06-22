using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Location : MonoBehaviour
{
    [Tooltip("background art")]
    public GameObject Art;
    [Tooltip("close via banish using a card matching this selector")]
    public CardSelector Banish;
    [Tooltip("close via invoking a block")]
    public Block BlockClose;
    [Tooltip("which campaign does this card belong to (\"Runelords\", etc.)")]
    public CampaignType Campaign;
    [Tooltip("card types in this location's deck")]
    public CardValueType[] Cards;
    [Tooltip("close via these checks (optional)")]
    public SkillCheckValueType[] Checks;
    [Tooltip("X \"when perm closed\" from XML file")]
    public string ClosedText;
    [Tooltip("X \"when closing\" from XML file")]
    public string ClosingText;
    private static Location currentLocation;
    [Tooltip("X name from XML file")]
    public string DisplayName;
    [Tooltip("X summary from XML file")]
    public string DisplayText;
    [Tooltip("close via encountering the next card in the location deck")]
    public bool Encounter;
    [Tooltip("pointer to the small icon for this location")]
    public Sprite Icon;
    [Tooltip("unique; used to lookup text in XML file")]
    public string ID;
    private bool isClosed;
    private bool isClosedThisTurn;
    [Tooltip("keep these cards when closed (null means none)")]
    public CardSelector Keepers;
    [Tooltip("X \"at this location\" from XML file")]
    public string LocationText;
    private GameObject model;
    [Tooltip("background ambient music")]
    public AudioClip Music;
    private Deck myDeck;
    [Tooltip("X character deck set name from XML file")]
    public string Set;
    [Tooltip("if true this location has an event that must be resolved before you corner the villain and win the game")]
    public bool StopsVillainCorner;
    [Tooltip("close via summon monster (optional)")]
    public SummonsSelector Summons;

    private void Awake()
    {
        this.myDeck = this.FindDeckChild("Deck");
        LocationTableEntry entry = LocationTable.Get(this.ID);
        if (entry != null)
        {
            this.Set = entry.set;
            this.DisplayName = entry.Name;
            this.LocationText = entry.Location;
            this.ClosingText = entry.Closing;
            this.ClosedText = entry.Closed;
        }
        this.OnLoadData();
        if (this.myDeck != null)
        {
            this.myDeck.Changed += new Deck.EventHandlerDeckChanged(this.OnDeckChangedSetCardCount);
        }
        Current = this;
        this.model = Geometry.CreateChildObject(base.gameObject, this.Art, "Background");
        if (this.model != null)
        {
            this.DisplayClosedBackground(this.Closed);
        }
        Settings.Debug.SetupLocationDeck(this.Deck);
        Tutorial.Notify(TutorialEventType.LocationLoaded);
        UI.Sound.MusicPlay(this.Music, true, true);
    }

    public bool CanPlayPower()
    {
        EventEncounteredBoostLocation component = base.GetComponent<EventEncounteredBoostLocation>();
        if ((component != null) && component.IsEventValid(Turn.Card))
        {
            return true;
        }
        for (int i = 0; i < this.Powers.Count; i++)
        {
            if (this.Powers[i].IsValid())
            {
                return true;
            }
        }
        return false;
    }

    public static void Clear(string locID)
    {
        byte[] buffer;
        if (Game.GetObjectData(locID, out buffer))
        {
            ByteStream bs = new ByteStream(buffer);
            if (bs != null)
            {
                bs.ReadInt();
                bs.ReadBool();
                bs.ReadBool();
                bs.ReadInt();
                int num = bs.ReadInt();
                for (int i = 0; i < num; i++)
                {
                    CardData data = CardData.FromStream(bs);
                    CardIdentity identity = new CardIdentity(data.ID, data.Set);
                    Campaign.Box.Push(identity, true);
                }
            }
        }
        Game.SetObjectData(locID, null);
    }

    public static int CountCharactersAtLocation(string ID)
    {
        int num = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if ((Party.Characters[i].Location == ID) && Party.Characters[i].Alive)
            {
                num++;
            }
        }
        return num;
    }

    public void DisplayClosedBackground(bool isVisible)
    {
        if (UI.Window.Type == WindowType.Location)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.locationClosedOverlay.SetActive(isVisible);
            }
        }
        if (UI.Window.Type == WindowType.Cutscene)
        {
            GuiWindowCutscene cutscene = UI.Window as GuiWindowCutscene;
            if (cutscene != null)
            {
                cutscene.locationClosedOverlay.SetActive(isVisible);
            }
        }
    }

    public static void Distribute(string locID, Card card, DeckPositionType position, bool known)
    {
        if (Current.ID == locID)
        {
            Current.Deck.Add(card, position);
            CardType type = !known ? CardType.None : card.Type;
            Scenario.Current.AddCardCount(locID, type, 1);
        }
        else
        {
            CardIdentity identity = new CardIdentity(card);
            Distribute(locID, identity, position, known);
        }
    }

    public static void Distribute(string locID, CardIdentity card, DeckPositionType position, bool known)
    {
        byte[] buffer;
        if (Game.GetObjectData(locID, out buffer))
        {
            ByteStream bs = new ByteStream(buffer);
            int num = bs.ReadInt();
            bool flag = bs.ReadBool();
            bool flag2 = bs.ReadBool();
            int num2 = bs.ReadInt();
            int capacity = bs.ReadInt();
            List<CardData> list = new List<CardData>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                list.Add(CardData.FromStream(bs));
            }
            CardData item = new CardData(card.ID, card.Set);
            if (position == DeckPositionType.Top)
            {
                list.Insert(0, item);
            }
            else
            {
                list.Add(item);
            }
            if (position == DeckPositionType.Shuffle)
            {
                list.Shuffle<CardData>();
                for (int k = 0; k < list.Count; k++)
                {
                    list[k].Known = false;
                }
            }
            CardType type = !known ? CardType.None : CardTable.LookupCardType(card.ID);
            Scenario.Current.AddCardCount(locID, type, 1);
            ByteStream stream2 = new ByteStream();
            stream2.WriteInt(num);
            stream2.WriteBool(flag);
            stream2.WriteBool(flag2);
            stream2.WriteInt(num2);
            stream2.WriteInt(list.Count);
            for (int j = 0; j < list.Count; j++)
            {
                list[j].ToStream(stream2);
            }
            Game.SetObjectData(locID, stream2.ToArray());
        }
    }

    private Deck FindDeckChild(string deckname)
    {
        Transform transform = base.transform.FindChild(deckname);
        if (transform != null)
        {
            return transform.GetComponent<Deck>();
        }
        return null;
    }

    public int GetCheckBonus(SkillCheckType skill)
    {
        int num = 0;
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components == null)
            {
                return num;
            }
            for (int i = 0; i < components.Length; i++)
            {
                num += components[i].GetCheckBonus();
            }
        }
        return num;
    }

    public DiceType GetCheckDice(SkillCheckType skill)
    {
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    DiceType checkDice = components[i].GetCheckDice();
                    if (checkDice != DiceType.D0)
                    {
                        return checkDice;
                    }
                }
            }
        }
        return DiceType.D0;
    }

    public int GetCheckModifier()
    {
        int num = 0;
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components == null)
            {
                return num;
            }
            for (int i = 0; i < components.Length; i++)
            {
                num += components[i].GetCheckModifier();
            }
        }
        return num;
    }

    public static int GetRandomCharacterAtLocation(string ID)
    {
        int max = CountCharactersAtLocation(ID);
        int num2 = UnityEngine.Random.Range(0, max);
        max = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if ((Party.Characters[i].Location == ID) && Party.Characters[i].Alive)
            {
                if (max == num2)
                {
                    return i;
                }
                max++;
            }
        }
        return -1;
    }

    public bool HasPower(LocationPowerType type)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            if (this.Powers[i].Situation == type)
            {
                return true;
            }
        }
        return false;
    }

    public static void Initialize(string locID)
    {
        GameObject obj2 = Resources.Load<GameObject>("Blueprints/Locations/" + locID);
        if (obj2 != null)
        {
            Location component = obj2.GetComponent<Location>();
            if (component != null)
            {
                List<CardIdentity> list = new List<CardIdentity>(component.Cards.Length);
                for (int i = 0; i < component.Cards.Length; i++)
                {
                    for (int j = 0; j < component.Cards[i].rank; j++)
                    {
                        CardIdentity item = Campaign.Box.Pull(component.Cards[i].card);
                        list.Add(item);
                    }
                }
                list.Shuffle<CardIdentity>();
                ByteStream bs = new ByteStream();
                if (bs != null)
                {
                    bs.WriteInt(1);
                    bs.WriteBool(false);
                    bs.WriteBool(false);
                    bs.WriteInt(1);
                    bs.WriteInt(list.Count);
                    for (int k = 0; k < list.Count; k++)
                    {
                        new CardData(list[k].ID, list[k].Set).ToStream(bs);
                    }
                    Game.SetObjectData(locID, bs.ToArray());
                }
            }
        }
    }

    public static bool IsBlocked(string locID)
    {
        byte[] buffer;
        if (Game.GetObjectData(locID, out buffer))
        {
            ByteStream bs = new ByteStream(buffer);
            if (bs != null)
            {
                bs.ReadInt();
                bs.ReadBool();
                bs.ReadBool();
                bs.ReadInt();
                if (bs.ReadInt() > 0)
                {
                    return (CardData.FromStream(bs).Blocker == BlockerType.Movement);
                }
            }
        }
        return false;
    }

    public bool IsClosePossible(Character character)
    {
        if (this.Closed)
        {
            return false;
        }
        if (this.Checks.Length > 0)
        {
            return true;
        }
        if (this.Summons != null)
        {
            return true;
        }
        if (this.Banish != null)
        {
            return (this.Banish.Filter(character.Hand) > 0);
        }
        return ((this.BlockClose != null) || (this.Encounter || true));
    }

    public static void Load(string ID)
    {
        if ((Current == null) || (Current.ID != ID))
        {
            LoadLocation(ID);
        }
    }

    [DebuggerHidden]
    public static IEnumerator LoadAsync(string ID) => 
        new <LoadAsync>c__Iterator94 { 
            ID = ID,
            <$>ID = ID
        };

    private static void LoadLocation(string ID)
    {
        if (Current != null)
        {
            Current.OnLocationExit();
            Current.OnSaveData();
            UnityEngine.Object.Destroy(Current.gameObject);
            Current = null;
        }
        GC.Collect();
        Resources.UnloadUnusedAssets();
        GameObject original = (GameObject) Resources.Load("Blueprints/Locations/" + ID, typeof(GameObject));
        if (original != null)
        {
            GameObject obj3 = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
            obj3.name = original.name;
            Current = obj3.GetComponent<Location>();
        }
        Settings.Debug.OnLocationLoaded();
    }

    public void Move()
    {
        Current.OnSaveData();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowMap(true);
        }
    }

    public void OnAfterEncounter()
    {
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].IsEventImplemented(EventType.OnAfterExplore))
                    {
                        components[i].OnAfterExplore();
                    }
                }
            }
        }
    }

    public void OnCardBeforeAct(Card card)
    {
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].IsEventImplemented(EventType.OnCardBeforeAct))
                    {
                        Game.Events.Add(EventCallbackType.Location, Current.ID, EventType.OnCardBeforeAct, i, components[i].Stateless, card);
                    }
                }
            }
        }
    }

    public void OnCardDefeated(Card card)
    {
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].IsEventImplemented(EventType.OnCardDefeated))
                    {
                        Game.Events.Add(EventCallbackType.Location, Current.ID, EventType.OnCardDefeated, i, components[i].Stateless, card);
                    }
                }
            }
        }
    }

    public void OnCardDiscarded(Card card)
    {
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Party.Characters[Turn.Number].Location);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].IsEventImplemented(EventType.OnCardDiscarded))
                    {
                        components[i].OnCardDiscarded(card);
                    }
                }
            }
        }
    }

    public void OnCardEncountered(Card card)
    {
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].IsEventImplemented(EventType.OnCardEncountered))
                    {
                        Game.Events.Add(EventCallbackType.Location, Current.ID, EventType.OnCardEncountered, i, components[i].Stateless, card);
                    }
                }
            }
        }
    }

    public void OnCardEvaded(Card card)
    {
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].IsEventImplemented(EventType.OnCardEvaded))
                    {
                        Game.Events.Add(EventCallbackType.Location, Current.ID, EventType.OnCardEvaded, i, components[i].Stateless, card);
                    }
                }
            }
        }
    }

    public void OnCardPlayed(Card card)
    {
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Party.Characters[Turn.Number].Location);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].IsEventImplemented(EventType.OnCardPlayed))
                    {
                        Game.Events.Add(EventCallbackType.Location, Party.Characters[Turn.Number].Location, EventType.OnCardPlayed, i, components[i].Stateless, card);
                    }
                }
            }
        }
    }

    public void OnCardUndefeated(Card card)
    {
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].IsEventImplemented(EventType.OnCardUndefeated))
                    {
                        Game.Events.Add(EventCallbackType.Location, Current.ID, EventType.OnCardUndefeated, i, components[i].Stateless, card);
                    }
                }
            }
        }
    }

    public bool OnCombatEnd(Card card)
    {
        bool flag = true;
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components == null)
            {
                return flag;
            }
            for (int i = 0; i < components.Length; i++)
            {
                if (!components[i].OnCombatEnd(card))
                {
                    flag = false;
                }
            }
        }
        return flag;
    }

    public void OnCombatResolved(Card card)
    {
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].IsEventImplemented(EventType.OnCombatResolved))
                    {
                        components[i].OnCombatResolved();
                    }
                    components[i].EndGameIfNecessary(card);
                }
            }
        }
    }

    private void OnDeckChangedSetCardCount(object sender, EventArgsCard e)
    {
        Scenario.Current.AddCardCount(this.ID, e.CardType, e.ChangeInSize);
    }

    private void OnDestroy()
    {
        if (this.myDeck != null)
        {
            this.myDeck.Changed -= new Deck.EventHandlerDeckChanged(this.OnDeckChangedSetCardCount);
        }
    }

    public void OnDiceRolled()
    {
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].IsEventImplemented(EventType.OnDiceRolled))
                    {
                        components[i].OnDiceRolled();
                    }
                }
            }
        }
    }

    public void OnEndOfEndTurn()
    {
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].IsEventImplemented(EventType.OnEndOfEndTurn))
                    {
                        Turn.DestructiveActionsCount++;
                        Game.Events.Add(EventCallbackType.Location, Current.ID, EventType.OnEndOfEndTurn, i, components[i].Stateless);
                    }
                }
            }
        }
    }

    public void OnExamineAnyLocation()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Card card = window.layoutSummoner.Card;
            if (card != null)
            {
                card.OnExamineAnyLocation();
            }
        }
    }

    public void OnHandReset()
    {
        this.isClosedThisTurn = false;
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].IsEventImplemented(EventType.OnHandReset))
                    {
                        Game.Events.Add(EventCallbackType.Location, Current.ID, EventType.OnHandReset, i, components[i].Stateless);
                    }
                }
            }
        }
    }

    public void OnLoadData()
    {
        byte[] buffer;
        if (Game.GetObjectData(this.ID, out buffer))
        {
            ByteStream bs = new ByteStream(buffer);
            if (bs != null)
            {
                bs.ReadInt();
                this.isClosed = bs.ReadBool();
                this.isClosedThisTurn = bs.ReadBool();
                this.Deck.FromStream(bs);
            }
        }
    }

    public void OnLocationCloseAttempted()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Card card = window.layoutSummoner.Card;
            if (card != null)
            {
                card.OnLocationCloseAttempted();
            }
        }
    }

    public void OnLocationExit()
    {
        this.isClosedThisTurn = false;
    }

    public void OnLocationExplored(Card card)
    {
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].IsEventImplemented(EventType.OnLocationExplored))
                    {
                        components[i].OnLocationExplored(card);
                    }
                }
            }
        }
    }

    public void OnSaveData()
    {
        ByteStream bs = new ByteStream();
        if (bs != null)
        {
            bs.WriteInt(1);
            bs.WriteBool(this.Closed);
            bs.WriteBool(this.ClosedThisTurn);
            this.Deck.ToStream(bs);
            Game.SetObjectData(this.ID, bs.ToArray());
        }
    }

    public void OnTurnEnded()
    {
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].IsEventImplemented(EventType.OnTurnEnded))
                    {
                        Turn.DestructiveActionsCount++;
                        Game.Events.Add(EventCallbackType.Location, Current.ID, EventType.OnTurnEnded, i, components[i].Stateless);
                    }
                }
            }
        }
    }

    public void OnTurnStarted()
    {
        this.isClosedThisTurn = false;
        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(Current.ID);
        if (locationPowersRoot != null)
        {
            Event[] components = locationPowersRoot.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].IsEventImplemented(EventType.OnTurnStarted))
                    {
                        Game.Events.Add(EventCallbackType.Location, Current.ID, EventType.OnTurnStarted, i, components[i].Stateless);
                    }
                }
            }
        }
    }

    public static string[] Peek(string locID, int number)
    {
        byte[] buffer;
        string[] strArray = new string[number];
        if (Game.GetObjectData(locID, out buffer))
        {
            ByteStream bs = new ByteStream(buffer);
            ByteStream stream2 = new ByteStream();
            if (bs == null)
            {
                return strArray;
            }
            stream2.WriteInt(bs.ReadInt());
            stream2.WriteBool(bs.ReadBool());
            stream2.WriteBool(bs.ReadBool());
            stream2.WriteInt(bs.ReadInt());
            int num = bs.ReadInt();
            stream2.WriteInt(num);
            for (int i = 0; i < num; i++)
            {
                CardData data = CardData.FromStream(bs);
                if (i < number)
                {
                    data.Known = true;
                    strArray[i] = data.ID;
                }
                data.ToStream(stream2);
            }
            Game.SetObjectData(locID, stream2.ToArray());
        }
        return strArray;
    }

    public static void Remove(string locID, string cardID)
    {
        byte[] buffer;
        if (Game.GetObjectData(locID, out buffer))
        {
            ByteStream bs = new ByteStream(buffer);
            ByteStream stream2 = new ByteStream();
            if (bs != null)
            {
                stream2.WriteInt(bs.ReadInt());
                stream2.WriteBool(bs.ReadBool());
                stream2.WriteBool(bs.ReadBool());
                stream2.WriteInt(bs.ReadInt());
                int capacity = bs.ReadInt();
                List<CardData> list = new List<CardData>(capacity);
                for (int i = 0; i < capacity; i++)
                {
                    CardData item = CardData.FromStream(bs);
                    if (item.ID != cardID)
                    {
                        list.Add(item);
                    }
                }
                stream2.WriteInt(capacity);
                for (int j = 0; j < list.Count; j++)
                {
                    list[j].ToStream(stream2);
                }
            }
            Game.SetObjectData(locID, stream2.ToArray());
        }
    }

    public void Show(bool isVisible)
    {
        if (this.model != null)
        {
            this.model.SetActive(isVisible);
        }
    }

    public Card Card =>
        Scenario.Current.LocationCards[this.ID];

    public bool Closed
    {
        get => 
            this.isClosed;
        set
        {
            if (value)
            {
                if (Turn.CloseType == CloseType.Temporary)
                {
                    UI.Sound.Play(SoundEffectType.LocationClosedTemp);
                    Scenario.Current.CloseLocation(this.ID, CloseType.Temporary);
                }
                else
                {
                    bool flag = true;
                    for (int i = 0; i < this.Deck.Count; i++)
                    {
                        if (((this.Deck[i].Type == CardType.Villain) && (this.Deck[i].Disposition != DispositionType.Banish)) && (this.Deck[i].Disposition != DispositionType.Destroy))
                        {
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        Scenario.Current.CloseLocation(this.ID, CloseType.Permanent);
                        this.isClosed = true;
                        this.isClosedThisTurn = true;
                        Game.Network.OnLocationClosed(this);
                        Tutorial.Notify(TutorialEventType.LocationClosed);
                    }
                    if (this.Closed)
                    {
                        UI.Sound.Play(SoundEffectType.LocationClosedPerm);
                        this.DisplayClosedBackground(true);
                    }
                    if (this.Closed)
                    {
                        GameObject locationPowersRoot = Scenario.Current.GetLocationPowersRoot(this.ID);
                        if (locationPowersRoot != null)
                        {
                            Event[] components = locationPowersRoot.GetComponents<Event>();
                            for (int j = 0; j < components.Length; j++)
                            {
                                if (components[j].IsEventImplemented(EventType.OnLocationClosed))
                                {
                                    Game.Events.Add(EventCallbackType.Location, Current.ID, EventType.OnLocationClosed, j, components[j].Stateless);
                                }
                            }
                        }
                    }
                    if (this.Closed)
                    {
                        GuiWindowLocation window = UI.Window as GuiWindowLocation;
                        if (window != null)
                        {
                            window.locationPanel.Refresh();
                        }
                    }
                }
            }
            else
            {
                this.isClosed = false;
            }
        }
    }

    public bool ClosedThisTurn =>
        this.isClosedThisTurn;

    public static Location Current
    {
        get => 
            currentLocation;
        set
        {
            currentLocation = value;
        }
    }

    public Deck Deck =>
        this.myDeck;

    public static string Destination
    {
        [CompilerGenerated]
        get => 
            <Destination>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Destination>k__BackingField = value;
        }
    }

    public List<LocationPower> Powers =>
        Scenario.Current.GetLocationPowers(this.ID);

    public static bool StartInMap
    {
        [CompilerGenerated]
        get => 
            <StartInMap>k__BackingField;
        [CompilerGenerated]
        set
        {
            <StartInMap>k__BackingField = value;
        }
    }

    [CompilerGenerated]
    private sealed class <LoadAsync>c__Iterator94 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal string <$>ID;
        internal string ID;

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
                    if ((Location.Current == null) || (Location.Current.ID != this.ID))
                    {
                        if (!Turn.IsSwitchingCharacters())
                        {
                            Game.UI.ShowWaitScreen(true);
                            this.$current = new WaitForSeconds(UI.WaitScreenAnimationLength);
                            this.$PC = 1;
                            return true;
                        }
                        break;
                    }
                    break;

                case 1:
                    Location.LoadLocation(this.ID);
                    Game.UI.ShowWaitScreen(false);
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

