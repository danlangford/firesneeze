using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Scenario : MonoBehaviour
{
    [Tooltip("card art")]
    public GameObject Art;
    private BlackBoard blackBoard = new BlackBoard("_SC_BB");
    [Tooltip("which campaign does this card belong to (\"Runelords\", etc.)")]
    public CampaignType Campaign;
    private static Scenario currentScenario;
    [Tooltip("X scenario name from XML file")]
    public string DisplayName;
    [Tooltip("X scenario summary from XML file")]
    public string DisplayText;
    [Tooltip("X during this scenario text from XML file")]
    public string DuringText;
    private EffectList Effects = new EffectList(1);
    private string endLocation;
    [Tooltip("the henchmen used in this scenario")]
    public string[] Henchmen;
    [Tooltip("unique; used to lookup text in XML file")]
    public string ID;
    [Tooltip("name of the intro animation prefab in blueprints used during setup")]
    public string Intro;
    private bool isComplete;
    private List<string> KnownPowers = new List<string>(5);
    [Tooltip("the location card IDs used in this scenario")]
    public LocationValueType[] Locations;
    private ScenarioLocationCache locationsCache;
    [Tooltip("name of the map prefab in blueprints/maps used in this scenario")]
    public string Map;
    private Deck myBlessingsDeck;
    private Deck myBlessingsDiscard;
    private Deck myLocationsDeck;
    private List<ScenarioPower> myPowers = new List<ScenarioPower>(5);
    [Tooltip("the deck number of this scenario")]
    public int Number;
    private int numVillainDefeats;
    private int numVillainEncounters;
    [Tooltip("ID of the powers that can be applied randomly to this scenario")]
    public ScenarioPowerValueType[] RandomPowers;
    [Tooltip("X reward text from XML file")]
    public string RewardText;
    [Tooltip("X character deck set name from XML file")]
    public string Set;
    [Tooltip("ID of the powers for this scenario")]
    public ScenarioPowerValueType[] StartingPowers;
    private Timer timeSpentInScenario = new Timer();
    [Tooltip("the villain used in this scenario")]
    public string[] Villains;

    public void AddCardCount(string locID, CardType type, int amount)
    {
        if ((this.GetCardCount(locID, type) <= 0) && (amount < 0))
        {
            this.SetCardCount(locID, CardType.None, this.GetCardCount(locID, CardType.None) + amount);
        }
        else
        {
            this.SetCardCount(locID, type, this.GetCardCount(locID, type) + amount);
        }
    }

    public void AddPower(string id)
    {
        Transform transform = base.transform.FindChild("Powers");
        if (transform != null)
        {
            bool flag = false;
            if (id == "$Random")
            {
                flag = true;
                id = this.ChooseRandomPower();
            }
            if ((id != null) && !this.KnownPowers.Contains(id))
            {
                this.KnownPowers.Add(id);
                ScenarioPower[] powerArray = ScenarioPower.Create(id);
                if (powerArray != null)
                {
                    for (int i = 0; i < powerArray.Length; i++)
                    {
                        powerArray[i].Wildcard = flag;
                        powerArray[i].transform.parent = transform;
                        this.myPowers.Add(powerArray[i]);
                    }
                }
            }
        }
    }

    public void ApplyEffect(Effect effect)
    {
        this.Effects.Add(effect);
        effect.OnEffectStarted(this);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.effectsPanel.Refresh();
        }
    }

    private void Awake()
    {
        this.myBlessingsDeck = this.FindDeckChild("Deck - Blessings");
        this.myBlessingsDiscard = this.FindDeckChild("Deck - Discard");
        this.myLocationsDeck = this.FindDeckChild("Deck - Locations");
        this.locationsCache = Geometry.CreateChild(base.gameObject, "Locations").AddComponent<ScenarioLocationCache>();
    }

    private void BuildBlessingsDeck()
    {
        int num = Constants.NUM_CARDS_IN_BLESSINGS_DECK;
        for (int i = 0; i < this.Powers.Count; i++)
        {
            ScenarioPowerDeckSize size = this.Powers[i] as ScenarioPowerDeckSize;
            if (size != null)
            {
                num += size.GetSizeModifier(DeckType.Blessings);
            }
        }
        for (int j = 0; j < num; j++)
        {
            Card card = Campaign.Box.Draw(CardType.Blessing);
            this.myBlessingsDeck.Add(card);
        }
        this.myBlessingsDeck.Shuffle();
        Settings.Debug.SetupBlessingsDeck(this.myBlessingsDeck);
    }

    protected virtual void BuildBox()
    {
        Campaign.Box.Clear();
        if (Adventure.Current.Number >= 0)
        {
            Campaign.LoadBox("1B", "B");
        }
        if (Adventure.Current.Number >= 1)
        {
            Campaign.LoadBox("11", "1");
        }
        if (Adventure.Current.Number >= 2)
        {
            Campaign.LoadBox("12", "2");
        }
        if (Adventure.Current.Number >= 3)
        {
            Campaign.LoadBox("13", "3");
        }
        if (Adventure.Current.Number >= 4)
        {
            Campaign.LoadBox("14", "4");
        }
        if (Adventure.Current.Number >= 5)
        {
            Campaign.LoadBox("15", "5");
        }
        if (Adventure.Current.Number >= 6)
        {
            Campaign.LoadBox("16", "6");
        }
        if (Settings.UseCollectionCardsInStoryMode)
        {
            int b = (3 * this.GetNumLocations()) - 1;
            for (int j = 0; j <= Adventure.Current.Number; j++)
            {
                string set = (j != 0) ? j.ToString() : "B";
                foreach (CollectionEntry entry in Collection.GetEntries(set))
                {
                    int num3 = Mathf.Min(entry.quantity, b);
                    for (int k = 0; k < num3; k++)
                    {
                        CardIdentity identity = new CardIdentity(entry.id, set);
                        Campaign.Box.Push(identity, true);
                    }
                }
            }
        }
        if (LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_CH11))
        {
            Campaign.LoadBox("1C", "C");
        }
        if (LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_BUNDLE_ROTR))
        {
            Campaign.LoadBox("1P", "P");
        }
        Campaign.Box.Shuffle();
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            for (int m = 0; m < Party.Characters[i].Deck.Count; m++)
            {
                Campaign.Box.Remove(Party.Characters[i].Deck[m]);
            }
        }
        if (Adventure.Current.Number >= 3)
        {
            float num7 = ((Adventure.Current.Number - 3) * 0.5f) + ((Current.Number - 1) * 0.1f);
            num7 = Mathf.Clamp(num7, 0f, 0.9f);
            Campaign.Box.Cull(num7, CardRankType.Basic);
        }
        if (Adventure.Current.Number >= 5)
        {
            float num8 = ((Adventure.Current.Number - 5) * 0.5f) + ((Current.Number - 1) * 0.1f);
            num8 = Mathf.Clamp(num8, 0f, 0.9f);
            Campaign.Box.Cull(num8, CardRankType.Elite);
        }
        int num9 = Campaign.Box.Filter(CardType.Blessing, BoosterFilterType.On);
        if (num9 < Constants.MIN_BLESSINGS_IN_BOX)
        {
            CardIdentity identity2 = new CardIdentity("BL1B_BlessingOfTheGods");
            for (int n = 0; n < (Constants.MIN_BLESSINGS_IN_BOX - num9); n++)
            {
                Campaign.Box.Push(identity2, true);
            }
        }
    }

    private void BuildLocationsCache()
    {
        for (int i = 0; i < this.Locations.Length; i++)
        {
            if (Party.Characters.Count >= this.Locations[i].PlayerCount)
            {
                this.locationsCache.Add(this.Locations[i].LocationName);
            }
        }
    }

    private void BuildLocationsDeck()
    {
        for (int i = 0; i < this.Locations.Length; i++)
        {
            if (Party.Characters.Count >= this.Locations[i].PlayerCount)
            {
                Card card = LocationTable.CreateCard(this.Locations[i].LocationName);
                card.SortingOrder = 2;
                this.myLocationsDeck.Add(card);
            }
        }
    }

    private string ChooseRandomPower()
    {
        if (Settings.Debug.Wildcard1 != null)
        {
            string str = Settings.Debug.Wildcard1;
            Settings.Debug.Wildcard1 = null;
            return str;
        }
        if (Settings.Debug.Wildcard2 != null)
        {
            string str2 = Settings.Debug.Wildcard2;
            Settings.Debug.Wildcard2 = null;
            return str2;
        }
        List<string> list = new List<string>(this.RandomPowers.Length + Adventure.Current.RandomPowers.Length);
        for (int i = 0; i < Adventure.Current.RandomPowers.Length; i++)
        {
            if ((Adventure.Current.RandomPowers[i].Active && (Adventure.Current.RandomPowers[i].Difficulty == this.Difficulty)) && !this.KnownPowers.Contains(Adventure.Current.RandomPowers[i].ID))
            {
                list.Add(Adventure.Current.RandomPowers[i].ID);
            }
        }
        for (int j = 0; j < this.RandomPowers.Length; j++)
        {
            if (this.RandomPowers[j].Active)
            {
                if ((this.RandomPowers[j].Difficulty == this.Difficulty) && !this.KnownPowers.Contains(this.RandomPowers[j].ID))
                {
                    list.Add(this.RandomPowers[j].ID);
                }
            }
            else
            {
                list.Remove(this.RandomPowers[j].ID);
            }
        }
        if (list.Count > 0)
        {
            int num3 = UnityEngine.Random.Range(0, list.Count);
            return list[num3];
        }
        return null;
    }

    public void CloseLocation(string locID, CloseType type)
    {
        if (this.locationsCache[locID] != null)
        {
            ScenarioLocation location = this.locationsCache[locID];
            location.Closed = type;
        }
    }

    private void DistributeBadGuys()
    {
        List<string> list = new List<string>(this.Locations.Length);
        for (int i = 0; i < this.Villains.Length; i++)
        {
            Campaign.Box.Pull(this.Villains[i]);
            list.Add(this.Villains[i]);
        }
        int numLocations = this.GetNumLocations();
        for (int j = 0; j < (numLocations - Current.Villains.Length); j++)
        {
            if (j < this.Henchmen.Length)
            {
                Campaign.Box.Pull(this.Henchmen[j]);
                list.Add(this.Henchmen[j]);
            }
        }
        int num4 = numLocations - (this.Henchmen.Length + 1);
        for (int k = 0; k < num4; k++)
        {
            Campaign.Box.Pull(this.Henchmen[this.Henchmen.Length - 1]);
            list.Add(this.Henchmen[this.Henchmen.Length - 1]);
        }
        list.Shuffle<string>();
        int num6 = 0;
        for (int m = 0; m < this.locationsCache.Count; m++)
        {
            CardIdentity card = new CardIdentity(list[num6++], this.Set);
            Location.Distribute(this.locationsCache[m].ID, card, DeckPositionType.Shuffle, false);
        }
    }

    public void End()
    {
        Campaign.Box.Combine(this.Blessings);
        Campaign.Box.Combine(this.Discard);
        for (int i = 0; i < this.Locations.Length; i++)
        {
            if (Current.IsLocationValid(this.Locations[i].LocationName))
            {
                Location.Clear(this.Locations[i].LocationName);
            }
        }
        for (int j = 0; j < this.locationsCache.Count; j++)
        {
            this.locationsCache[j].Clear();
        }
        for (int k = 0; k < Party.Characters.Count; k++)
        {
            Party.Characters[k].ResetDeck();
        }
        Turn.Reset();
        for (int m = 0; m < Party.Characters.Count; m++)
        {
            Party.Characters[m].Clear();
        }
        ScenarioAltFinishBase component = base.GetComponent<ScenarioAltFinishBase>();
        if (component != null)
        {
            component.ScenarioCleanup();
        }
    }

    public virtual void Exit()
    {
        this.OnScenarioExit();
        AnalyticsManager.OnScenarioComplete((float) this.timeSpentInScenario.Stop());
        this.GPX = 0;
        this.Forfeit = false;
        this.blackBoard.Clear();
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Party.Characters[i].OnScenarioExit();
        }
        bool isAnyoneDead = false;
        if (Campaign.PermaDeath)
        {
            for (int j = Party.Characters.Count - 1; j >= 0; j--)
            {
                Party.Characters[j].Location = null;
                Party.Characters[j].Active = ActiveType.Active;
                if (!Party.Characters[j].Alive)
                {
                    Campaign.Deaths.Add(Party.Characters[j].NickName);
                    for (int k = 0; k < Party.Characters[j].Deck.Count; k++)
                    {
                        Campaign.Distributions.Add(Party.Characters[j].Deck[k].ID);
                    }
                    UnityEngine.Object.Destroy(Party.Characters[j].gameObject);
                    Party.Remove(Party.Characters[j]);
                    isAnyoneDead = true;
                }
            }
        }
        else
        {
            for (int m = 0; m < Party.Characters.Count; m++)
            {
                Party.Characters[m].Location = null;
                Party.Characters[m].Active = ActiveType.Active;
                Party.Characters[m].Alive = true;
            }
        }
        this.ExitToNextScene(isAnyoneDead);
    }

    protected virtual void ExitToNextScene(bool isAnyoneDead)
    {
        if (isAnyoneDead)
        {
            Game.UI.ShowCreatePartyScene();
        }
        else
        {
            Game.UI.ShowSelectCardScene();
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

    public int GetCardCount(string locID)
    {
        if (this.locationsCache[locID] != null)
        {
            ScenarioLocation location = this.locationsCache[locID];
            if (location != null)
            {
                return location.GetCardCount();
            }
        }
        return 0;
    }

    public int GetCardCount(string locID, CardType type)
    {
        if (this.locationsCache[locID] != null)
        {
            ScenarioLocation location = this.locationsCache[locID];
            if (location != null)
            {
                return location.GetCardCount(type);
            }
        }
        return 0;
    }

    public int GetCheckBonus(SkillCheckType skill)
    {
        int num = 0;
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    num += components[j].GetCheckBonus();
                }
            }
        }
        return num;
    }

    public DiceType GetCheckDice(SkillCheckType skill)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    DiceType checkDice = components[j].GetCheckDice();
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
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int k = 0; k < components.Length; k++)
                {
                    num += components[k].GetCheckModifier();
                }
            }
        }
        for (int j = 0; j < this.Effects.Count; j++)
        {
            if (this.Effects[j].Type == EffectType.ModifyDifficulty)
            {
                EffectModifyDifficulty difficulty = this.Effects[j] as EffectModifyDifficulty;
                if (difficulty != null)
                {
                    num += difficulty.GetCheckModifier(Turn.Card);
                }
            }
        }
        return num;
    }

    public Effect GetEffect(EffectType effectType)
    {
        for (int i = 0; i < this.Effects.Count; i++)
        {
            if (this.Effects[i].Type == effectType)
            {
                return this.Effects[i];
            }
        }
        return null;
    }

    public Effect GetEffect(int index)
    {
        if ((index < this.Effects.Count) && (index >= 0))
        {
            return this.Effects[index];
        }
        return null;
    }

    public CloseType GetLocationCloseType(string locID)
    {
        if (this.locationsCache[locID] != null)
        {
            ScenarioLocation location = this.locationsCache[locID];
            return location.Closed;
        }
        return CloseType.None;
    }

    public List<LocationPower> GetLocationPowers(string id)
    {
        if (this.locationsCache[id] != null)
        {
            return this.locationsCache[id].Powers;
        }
        return null;
    }

    public GameObject GetLocationPowersRoot(string id)
    {
        if (this.locationsCache[id] != null)
        {
            return this.locationsCache[id].PowersRoot;
        }
        return null;
    }

    public int GetNumEffects() => 
        this.Effects.Count;

    public int GetNumLocations()
    {
        int num = 0;
        for (int i = 0; i < Current.Locations.Length; i++)
        {
            if (Current.IsLocationValid(Current.Locations[i].LocationName))
            {
                num++;
            }
        }
        return num;
    }

    public int GetNumOpenLocations()
    {
        int num = 0;
        for (int i = 0; i < Current.Locations.Length; i++)
        {
            if (Current.IsLocationValid(Current.Locations[i].LocationName))
            {
                string locationName = Current.Locations[i].LocationName;
                if (!Current.IsLocationClosed(locationName))
                {
                    num++;
                }
            }
        }
        return num;
    }

    private void InformPlayers()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Party.Characters[i].OnScenarioStart();
        }
    }

    public void Initialize()
    {
        for (int i = 0; i < this.StartingPowers.Length; i++)
        {
            if (this.StartingPowers[i].Active && ((this.StartingPowers[i].Difficulty == this.Difficulty) || ((this.StartingPowers[i].Difficulty == 0) && (i > 0))))
            {
                this.AddPower(this.StartingPowers[i].ID);
            }
        }
        if (this.Difficulty >= 1)
        {
            this.AddPower("$Random");
        }
        if (this.Difficulty >= 2)
        {
            this.AddPower("$Random");
        }
        this.BuildBox();
        this.InitializeLocations();
        this.BuildLocationsCache();
        this.BuildLocationsDeck();
        this.BuildBlessingsDeck();
        EventDistributeCard[] components = base.GetComponents<EventDistributeCard>();
        if ((components != null) && (components.Length > 0))
        {
            for (int j = 0; j < components.Length; j++)
            {
                components[j].DistributeBadGuys();
            }
        }
        else
        {
            this.DistributeBadGuys();
        }
        this.InformPlayers();
        GuiWindowLocation.StartingNewGame = true;
    }

    private void InitializeLocations()
    {
        for (int i = 0; i < this.Locations.Length; i++)
        {
            if (Party.Characters.Count >= this.Locations[i].PlayerCount)
            {
                Location.Initialize(this.Locations[i].LocationName);
            }
        }
    }

    public bool IsCurrentVillain(string villain)
    {
        for (int i = 0; i < this.Villains.Length; i++)
        {
            if (this.Villains[i] == villain)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsLocationClosed(string locID)
    {
        if (this.locationsCache[locID] != null)
        {
            ScenarioLocation location = this.locationsCache[locID];
            return ((location.Closed != CloseType.None) && (location.Closed != CloseType.Impossible));
        }
        return false;
    }

    public bool IsLocationClosePossible(string locID, CloseType type)
    {
        if (type == CloseType.Temporary)
        {
            for (int i = 0; i < this.Powers.Count; i++)
            {
                ScenarioPowerCannotTempClose component = this.Powers[i].GetComponent<ScenarioPowerCannotTempClose>();
                if (component != null)
                {
                    return (!component.IsLocationValid(locID) && !this.IsLocationClosed(locID));
                }
            }
        }
        return !this.IsLocationClosed(locID);
    }

    public bool IsLocationExplored(string locID)
    {
        if (this.locationsCache[locID] != null)
        {
            ScenarioLocation location = this.locationsCache[locID];
            if (location != null)
            {
                return location.Explored;
            }
        }
        return false;
    }

    public bool IsLocationLinked(string locID, string linkID)
    {
        if (this.IsLocationValid(locID) && this.IsLocationValid(linkID))
        {
            for (int i = 0; i < this.Locations.Length; i++)
            {
                if (this.Locations[i].LocationName == locID)
                {
                    return this.Locations[i].IsLinked(linkID);
                }
            }
        }
        return false;
    }

    public bool IsLocationValid(string locID)
    {
        for (int i = 0; i < this.Locations.Length; i++)
        {
            if ((this.Locations[i].LocationName == locID) && (Party.Characters.Count >= this.Locations[i].PlayerCount))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsScenarioOver()
    {
        ScenarioAltFinishBase component = base.GetComponent<ScenarioAltFinishBase>();
        if (component != null)
        {
            return component.IsScenarioOver();
        }
        return (this.GetNumOpenLocations() <= 0);
    }

    public bool IsScenarioWon()
    {
        if (this.Forfeit)
        {
            return false;
        }
        ScenarioAltFinishBase component = base.GetComponent<ScenarioAltFinishBase>();
        if (component != null)
        {
            return component.IsScenarioWon();
        }
        return (this.GetNumOpenLocations() <= 0);
    }

    public void OnAfterEncounter()
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnAfterExplore))
                    {
                        components[j].OnAfterExplore();
                    }
                }
            }
        }
    }

    public void OnBeforeTurnStart()
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].Type == EventType.OnBeforeTurnStart)
                    {
                        Game.Events.Add(EventCallbackType.Scenario, components[j].name, EventType.OnBeforeTurnStart, j, components[j].Stateless);
                    }
                }
            }
        }
    }

    public void OnCardBeforeAct(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnCardBeforeAct))
                    {
                        Game.Events.Add(EventCallbackType.Scenario, components[j].name, EventType.OnCardBeforeAct, j, components[j].Stateless, card);
                    }
                }
            }
        }
    }

    public void OnCardDefeated(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int k = 0; k < components.Length; k++)
                {
                    if (components[k].IsEventImplemented(EventType.OnCardDefeated))
                    {
                        Game.Events.Add(EventCallbackType.Scenario, components[k].name, EventType.OnCardDefeated, k, components[k].Stateless, card);
                    }
                }
            }
        }
        for (int j = 0; j < this.Effects.Count; j++)
        {
            if ((this.Effects[j].Type == EffectType.OnDefeatHeal) && this.Effects[j].filter.Match(card))
            {
                this.Effects[j].Invoke();
            }
        }
    }

    public void OnCardDiscarded(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnCardDiscarded))
                    {
                        components[j].OnCardDiscarded(card);
                    }
                }
            }
        }
    }

    public void OnCardEncountered(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnCardEncountered))
                    {
                        Game.Events.Add(EventCallbackType.Scenario, components[j].name, EventType.OnCardEncountered, j, components[j].Stateless, card);
                    }
                }
            }
        }
    }

    public void OnCardEvaded(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnCardEvaded))
                    {
                        Game.Events.Add(EventCallbackType.Scenario, components[j].name, EventType.OnCardEvaded, j, components[j].Stateless, card);
                    }
                }
            }
        }
    }

    public void OnCardPlayed(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnCardPlayed))
                    {
                        Game.Events.Add(EventCallbackType.Scenario, components[j].name, EventType.OnCardPlayed, j, components[j].Stateless, card);
                    }
                }
            }
        }
    }

    public void OnCardRecharged(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnCardRecharged))
                    {
                        components[j].OnCardRecharged(card);
                    }
                }
            }
        }
    }

    public void OnCardUndefeated(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnCardUndefeated))
                    {
                        Game.Events.Add(EventCallbackType.Scenario, components[j].name, EventType.OnCardUndefeated, j, components[j].Stateless, card);
                    }
                }
            }
        }
    }

    public void OnCheckCompleted()
    {
        this.Effects.OnCheckCompleted();
    }

    public void OnCombatResolved(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnCombatResolved))
                    {
                        components[j].OnCombatResolved();
                    }
                }
            }
        }
    }

    public void OnDamageTaken(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnDamageTaken))
                    {
                        Game.Events.Add(EventCallbackType.Scenario, components[j].name, EventType.OnDamageTaken, j, components[j].Stateless, card);
                    }
                }
            }
        }
    }

    public void OnEncounterComplete()
    {
        this.Effects.OnEncounterComplete();
    }

    public void OnEndOfEndTurn()
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnEndOfEndTurn))
                    {
                        Turn.DestructiveActionsCount++;
                        Game.Events.Add(EventCallbackType.Scenario, components[j].name, EventType.OnEndOfEndTurn, j, components[j].Stateless);
                    }
                }
            }
        }
    }

    public virtual void OnLoadData()
    {
        byte[] buffer;
        if (Game.GetObjectData(this.ID, out buffer))
        {
            ByteStream bs = new ByteStream(buffer);
            if (bs != null)
            {
                bs.ReadInt();
                this.Difficulty = bs.ReadInt();
                this.isComplete = bs.ReadBool();
                this.endLocation = bs.ReadString();
                this.numVillainDefeats = bs.ReadInt();
                this.numVillainEncounters = bs.ReadInt();
                this.locationsCache.FromStream(bs);
                this.Blessings.FromStream(bs);
                this.Discard.FromStream(bs);
                string[] strArray = bs.ReadStringArray();
                this.KnownPowers.Clear();
                for (int i = 0; i < strArray.Length; i++)
                {
                    this.AddPower(strArray[i]);
                }
                this.Effects.FromArray(bs.ReadByteArray());
                this.ReadLocationDeck(bs);
                this.timeSpentInScenario.Start(bs.ReadInt());
                this.GPX = bs.ReadInt();
            }
        }
        this.blackBoard.OnLoadData();
    }

    public bool OnLocationChange()
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnLocationChange) && components[j].IsEventValid(Turn.Card))
                    {
                        EventCallback callback = new EventCallback {
                            CallbackType = EventCallbackType.Scenario,
                            CallbackID = components[j].name,
                            CallbackEvent = EventType.OnLocationChange,
                            CallbackPosition = j,
                            Stateless = components[j].Stateless,
                            CallbackCardId = null
                        };
                        if (Game.Events.IsEventRunning)
                        {
                            Game.Events.Top(callback);
                            return true;
                        }
                        Game.Events.Top(callback);
                        return false;
                    }
                }
            }
        }
        return false;
    }

    public void OnPlayerDamaged(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnPlayerDamaged))
                    {
                        Game.Events.Add(EventCallbackType.Scenario, components[j].name, EventType.OnPlayerDamaged, j, components[j].Stateless, card);
                    }
                }
            }
        }
    }

    public virtual void OnSaveData()
    {
        ByteStream bs = new ByteStream();
        if (bs != null)
        {
            bs.WriteInt(1);
            bs.WriteInt(this.Difficulty);
            bs.WriteBool(this.isComplete);
            bs.WriteString(this.endLocation);
            bs.WriteInt(this.numVillainDefeats);
            bs.WriteInt(this.numVillainEncounters);
            this.locationsCache.ToStream(bs);
            this.Blessings.ToStream(bs);
            this.Discard.ToStream(bs);
            bs.WriteStringArray(this.KnownPowers.ToArray());
            bs.WriteByteArray(this.Effects.ToArray());
            bs.WriteStringArray(this.LocationCards.GetCardList());
            bs.WriteInt(this.timeSpentInScenario.TimeInMinutes);
            bs.WriteInt(this.GPX);
            Game.SetObjectData(this.ID, bs.ToArray());
        }
        this.blackBoard.OnSaveData();
    }

    public void OnScenarioExit()
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnScenarioExit))
                    {
                        components[j].OnScenarioExit();
                    }
                }
            }
        }
    }

    public void OnTurnEnded()
    {
        this.Effects.OnTurnCompleted();
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnTurnEnded))
                    {
                        Turn.DestructiveActionsCount++;
                        Game.Events.Add(EventCallbackType.Scenario, components[j].name, EventType.OnTurnEnded, j, components[j].Stateless);
                    }
                }
            }
        }
    }

    public void OnTurnStarted()
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].Type == EventType.OnTurnStarted)
                    {
                        Game.Events.Add(EventCallbackType.Scenario, components[j].name, EventType.OnTurnStarted, j, components[j].Stateless);
                    }
                }
            }
        }
    }

    public void OnVillainIntroduced(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            Event[] components = this.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnVillainIntroduced))
                    {
                        components[j].OnVillainIntroduced(card);
                    }
                }
            }
        }
    }

    public void Play()
    {
        Turn.Reset();
        this.Complete = false;
        Game.UI.ShowCutsceneScene();
        this.timeSpentInScenario.Start(0);
        Campaign.OnScenarioStarted();
        AnalyticsManager.OnScenarioStarted();
    }

    private void ReadLocationDeck(ByteStream bs)
    {
        string[] strArray = bs.ReadStringArray();
        for (int i = 0; i < strArray.Length; i++)
        {
            Card card = LocationTable.CreateCard(this.Locations[i].LocationName);
            card.SortingOrder = 2;
            this.myLocationsDeck.Add(card);
        }
    }

    public void RemoveEffect(string sourceID)
    {
        for (int i = 0; i < this.Effects.Count; i++)
        {
            if (this.Effects[i].source == sourceID)
            {
                this.Effects.RemoveAt(i);
                return;
            }
        }
    }

    public void ResetImpossibleLocationClosures()
    {
        for (int i = 0; i < this.locationsCache.Count; i++)
        {
            if (this.locationsCache[i].Closed == CloseType.Impossible)
            {
                this.locationsCache[i].Closed = CloseType.None;
            }
        }
    }

    public void ResetTemporaryLocationClosures()
    {
        for (int i = 0; i < this.locationsCache.Count; i++)
        {
            if (this.locationsCache[i].Closed == CloseType.Temporary)
            {
                this.locationsCache[i].Closed = CloseType.None;
            }
        }
    }

    public void SetCardCount(string locID, CardType type, int n)
    {
        if (this.locationsCache[locID] != null)
        {
            ScenarioLocation location = this.locationsCache[locID];
            if (location != null)
            {
                location.SetCardCount(type, Mathf.Max(n, 0));
            }
        }
    }

    public void SetLocationExplored(string locID, bool isExplored)
    {
        if (this.locationsCache[locID] != null)
        {
            ScenarioLocation location = this.locationsCache[locID];
            if (location != null)
            {
                location.Explored = isExplored;
            }
        }
    }

    private void Update()
    {
        GuiWindow window = UI.Window;
        if (((window.Type == WindowType.Location) || (window.Type == WindowType.Cutscene)) || (window.Type == WindowType.Scenario))
        {
            this.timeSpentInScenario.Tick(Time.deltaTime);
        }
    }

    public BlackBoard BlackBoard =>
        this.blackBoard;

    public Deck Blessings =>
        this.myBlessingsDeck;

    public bool Complete
    {
        get => 
            (this.isComplete || this.IsScenarioWon());
        set
        {
            this.isComplete = value;
            if (this.isComplete)
            {
                this.EndLocation = Location.Current.ID;
                Campaign.SetScenarioComplete(this);
                Campaign.SetScenarioChampion(this, Turn.Owner.ID);
                if (this.Number == Adventure.Current.Scenarios.Length)
                {
                    Adventure.Current.Completed = true;
                }
                Game.Rewards.OnScenarioCompleted(this);
            }
        }
    }

    public static Scenario Current
    {
        get => 
            currentScenario;
        set
        {
            currentScenario = value;
        }
    }

    public int Difficulty { get; set; }

    public Deck Discard =>
        this.myBlessingsDiscard;

    public string EndLocation
    {
        get => 
            this.endLocation;
        private set
        {
            this.endLocation = value;
        }
    }

    public string FirstLocation
    {
        get
        {
            if (this.StartLocation != null)
            {
                return this.StartLocation;
            }
            for (int i = 0; i < this.Locations.Length; i++)
            {
                if (this.IsLocationValid(this.Locations[i].LocationName))
                {
                    return this.Locations[i].LocationName;
                }
            }
            return null;
        }
    }

    public bool Forfeit { get; set; }

    public int GPX { get; set; }

    public bool Linear =>
        (this.Difficulty >= 2);

    public Deck LocationCards =>
        this.myLocationsDeck;

    public int NumVillainDefeats
    {
        get => 
            this.numVillainDefeats;
        set
        {
            this.numVillainDefeats = value;
        }
    }

    public int NumVillainEncounters
    {
        get => 
            this.numVillainEncounters;
        set
        {
            this.numVillainEncounters = value;
        }
    }

    public List<ScenarioPower> Powers =>
        this.myPowers;

    public Reward Reward =>
        base.GetComponent<Reward>();

    public virtual bool Rewardable =>
        true;

    public string StartLocation
    {
        get
        {
            ScenarioPropertyStartLocation component = base.GetComponent<ScenarioPropertyStartLocation>();
            if (component != null)
            {
                return component.ID;
            }
            for (int i = 0; i < this.Powers.Count; i++)
            {
                component = this.Powers[i].GetComponent<ScenarioPropertyStartLocation>();
                if (component != null)
                {
                    return component.ID;
                }
            }
            return null;
        }
    }

    public string Villain
    {
        get
        {
            if (this.Villains.Length > 0)
            {
                return this.Villains[0];
            }
            return string.Empty;
        }
    }
}

