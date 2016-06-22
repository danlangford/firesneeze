using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Character : MonoBehaviour
{
    private int[] AttributeRank = new int[Constants.NUM_ATTRIBUTES];
    [Tooltip("which campaign set does this character belong")]
    public CampaignType Campaign;
    private int[] CardMarks = new int[Constants.NUM_CARD_TYPES];
    private int[] CardRank = new int[Constants.NUM_CARD_TYPES];
    [Tooltip("only used to enter starting card types")]
    public CardValueType[] Cards;
    private string[] CardsAtScenarioStart = new string[0];
    public DiceType CharismaDie = DiceType.D4;
    public ClassType Class = ClassType.Fighter;
    public DiceType ConstitutionDie = DiceType.D4;
    public DiceType DexterityDie = DiceType.D4;
    [Tooltip("X character name from XML file")]
    public string DisplayName;
    [Tooltip("X character summary from XML file")]
    public string DisplayText;
    private EffectList Effects = new EffectList();
    public CardType FavoredCard;
    public GenderType Gender = GenderType.Male;
    [Tooltip("number of cards in a hand; increases with levelup")]
    public int HandSize = 4;
    [Tooltip("unique; used to lookup text in XML file")]
    public string ID;
    public DiceType IntelligenceDie = DiceType.D4;
    private List<string> KnownPowers = new List<string>(0x19);
    [Tooltip("maximum attribute values gained during levelup")]
    public AttributeValueType[] LevelupAttributesMax;
    [Tooltip("maximum deck card count increases gained through levelup")]
    public CardValueType[] LevelupCardsMax;
    [Tooltip("maximum hand size gained during levelup (varies with roles)")]
    public int LevelupHandSizeMax;
    private Deck myBury;
    private Deck myDeck;
    private Deck myDiscard;
    private Deck myHand;
    private List<CharacterPower> myPowers = new List<CharacterPower>(0x19);
    private Deck myRecharge;
    [Tooltip("name assigned by player during character creation")]
    public string NickName;
    [Tooltip("\"body shot\" portrait drawn on side of screen")]
    public Sprite PortraitAvatar;
    [Tooltip("\"framed head shot\" portrait drawn on the switch screen")]
    public Sprite PortraitLarge;
    [Tooltip("tiny \"head shot\" shown on map buttons")]
    public Sprite PortraitLocation;
    [Tooltip("\"head shot\" portrait drawn on character buttons")]
    public Sprite PortraitSmall;
    [Tooltip("\"head shot\" portrait used when dead")]
    public Sprite PortraitSmallDead;
    [Tooltip("\"head shot\" portrait used when unavailable")]
    public Sprite PortraitSmallUnavailable;
    public bool ProficientWithHeavyArmors;
    public bool ProficientWithLightArmors;
    public bool ProficientWithWeapons;
    public RaceType Race = RaceType.Human;
    private List<string> Rewards = new List<string>(10);
    [Tooltip("role choices available during levelup")]
    public string[] Roles;
    [Tooltip("X character deck set name from XML file")]
    public string Set;
    private int[] SkillRank = new int[Constants.NUM_SKILLS];
    [Tooltip("only used to enter starting skills")]
    public SkillValueType[] Skills;
    [Tooltip("only used to enter cards in deck at start")]
    public string[] StartingCards;
    [Tooltip("powers this character currently has; changes with levelup")]
    public string[] StartingPowers;
    public DiceType StrengthDie = DiceType.D4;
    public DiceType WisdomDie = DiceType.D4;

    public void AddReward(Reward reward)
    {
        if (((reward != null) && !string.IsNullOrEmpty(reward.ID)) && !this.Rewards.Contains(reward.ID))
        {
            this.Rewards.Add(reward.ID);
        }
    }

    public void ApplyEffect(Effect e)
    {
        this.Effects.Add(e);
        e.OnEffectStarted(this);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (Turn.Character.ID == this.ID)
            {
                window.effectsPanel.Flash();
            }
            window.effectsPanel.Refresh();
        }
    }

    private void Awake()
    {
        this.Alive = true;
        this.Active = ActiveType.Active;
        this.Selected = false;
        this.HordeFightLeft = 0;
        foreach (SkillValueType type in this.Skills)
        {
            this.SetSkillRank(type.skill, type.rank);
        }
        foreach (CardValueType type2 in this.Cards)
        {
            this.CardRank[(int) type2.card] = type2.rank;
        }
        this.KnownPowers.AddRange(this.StartingPowers);
        this.SynchronizeKnownPowers();
        this.myDeck = this.FindDeckChild("Deck");
        this.myHand = this.FindDeckChild("Hand");
        this.myDiscard = this.FindDeckChild("Discard");
        this.myBury = this.FindDeckChild("Bury");
        this.myRecharge = this.FindDeckChild("Recharge");
    }

    public void BuildDeck()
    {
        this.Deck.Clear();
        for (int i = 0; i < this.StartingCards.Length; i++)
        {
            Card card = CardTable.Create(this.StartingCards[i]);
            this.Deck.Add(card);
        }
        this.Deck.Shuffle();
        Settings.Debug.SetupPlayerDeck(this.Deck);
    }

    public void BuildHand()
    {
        this.Hand.Clear();
        this.Hand.Add(this.Deck.Draw(this.FavoredCard));
        for (int i = 1; i < this.HandSize; i++)
        {
            this.Hand.Add(this.Deck.Draw());
        }
        this.Hand.Shuffle();
        Settings.Debug.SetupPlayerHand(this.Hand);
    }

    public bool CanPlayCard()
    {
        bool flag = false;
        if (this.Alive && (this.Hand != null))
        {
            Turn.Select(this, true);
            for (int i = 0; i < this.Hand.Count; i++)
            {
                if (this.Hand[i].IsAnyActionValid())
                {
                    flag = true;
                    break;
                }
            }
            Turn.Select(this, false);
        }
        return flag;
    }

    public bool CanPlayPower()
    {
        bool flag = false;
        if (this.Alive)
        {
            Turn.Select(this, true);
            for (int i = 0; i < this.Powers.Count; i++)
            {
                if (this.Powers[i].IsValid())
                {
                    flag = true;
                    break;
                }
            }
            Turn.Select(this, false);
        }
        return flag;
    }

    public void Clear()
    {
        this.Effects.Clear();
        for (int i = 0; i < this.CardMarks.Length; i++)
        {
            this.CardMarks[i] = 0;
        }
        this.HordeFightLeft = 0;
        this.Selected = false;
    }

    public void ClearCardType(CardType type)
    {
        this.CardMarks[(int) type] = 0;
    }

    private void ClearCardTypes()
    {
        this.ClearCardType(CardType.Ally);
        this.ClearCardType(CardType.Armor);
        this.ClearCardType(CardType.Blessing);
        this.ClearCardType(CardType.Item);
        this.ClearCardType(CardType.Spell);
        this.ClearCardType(CardType.Weapon);
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

    public DiceType GetAttributeDice(AttributeType attribute)
    {
        for (int i = 0; i < this.Effects.Count; i++)
        {
            if (this.Effects[i].Type == EffectType.AttributeChange)
            {
                EffectAttributeChange change = this.Effects[i] as EffectAttributeChange;
                if ((change != null) && (change.Attribute == attribute))
                {
                    return change.DiceType;
                }
            }
        }
        if (attribute == AttributeType.Strength)
        {
            return this.StrengthDie;
        }
        if (attribute == AttributeType.Dexterity)
        {
            return this.DexterityDie;
        }
        if (attribute == AttributeType.Constitution)
        {
            return this.ConstitutionDie;
        }
        if (attribute == AttributeType.Intelligence)
        {
            return this.IntelligenceDie;
        }
        if (attribute == AttributeType.Wisdom)
        {
            return this.WisdomDie;
        }
        if (attribute == AttributeType.Charisma)
        {
            return this.CharismaDie;
        }
        return DiceType.D0;
    }

    public int GetAttributeMaxRank(AttributeType type)
    {
        for (int i = 0; i < this.LevelupAttributesMax.Length; i++)
        {
            if (this.LevelupAttributesMax[i].attribute == type)
            {
                return this.LevelupAttributesMax[i].Rank;
            }
        }
        return 0;
    }

    public int GetAttributeRank(AttributeType attribute) => 
        this.AttributeRank[(int) attribute];

    public SkillCheckType GetBestSkillCheck(SkillCheckType[] checks)
    {
        SkillCheckType none = SkillCheckType.None;
        float minValue = float.MinValue;
        for (int i = 0; i < checks.Length; i++)
        {
            SkillCheckType skill = checks[i];
            float num3 = Rules.GetDiceWeight(this.GetSkillDice(skill)) + Rules.GetCheckBonus(skill);
            if (num3 > minValue)
            {
                none = skill;
                minValue = num3;
            }
        }
        return none;
    }

    public SkillCheckValueType GetBestSkillCheck(SkillCheckValueType[] checks)
    {
        SkillCheckValueType type = new SkillCheckValueType(SkillCheckType.None, 0);
        float minValue = float.MinValue;
        for (int i = 0; i < checks.Length; i++)
        {
            SkillCheckValueType type2 = checks[i];
            float num3 = (Rules.GetDiceWeight(this.GetSkillDice(type2.skill)) + Rules.GetCheckBonus(type2.skill)) - type2.rank;
            if (num3 > minValue)
            {
                type = type2;
                minValue = num3;
            }
        }
        return type;
    }

    public int GetCardMaxRank(CardType type)
    {
        for (int i = 0; i < this.LevelupCardsMax.Length; i++)
        {
            if (this.LevelupCardsMax[i].card == type)
            {
                return this.LevelupCardsMax[i].rank;
            }
        }
        return 0;
    }

    public int GetCardMinRank(CardType type)
    {
        for (int i = 0; i < this.Cards.Length; i++)
        {
            if (this.Cards[i].card == type)
            {
                return this.Cards[i].rank;
            }
        }
        return 0;
    }

    public int GetCardRank(CardType type) => 
        this.CardRank[(int) type];

    public int GetCheckModifier()
    {
        int num = 0;
        for (int i = 0; i < this.Effects.Count; i++)
        {
            if ((this.Effects[i].Type == EffectType.ModifyCheck) || (this.Effects[i].Type == EffectType.Haunt))
            {
                EffectModifyCheck check = this.Effects[i] as EffectModifyCheck;
                if (check != null)
                {
                    num += check.GetCheckModifier();
                }
            }
        }
        return num;
    }

    public SkillCheckType GetCombatSkill()
    {
        if (this.GetSkillRank(SkillCheckType.Melee) > 0)
        {
            return SkillCheckType.Melee;
        }
        return SkillCheckType.Strength;
    }

    public Deck GetDeck(DeckType deckType)
    {
        if (deckType == DeckType.Character)
        {
            return this.Deck;
        }
        if (deckType == DeckType.Bury)
        {
            return this.Bury;
        }
        if (deckType == DeckType.Discard)
        {
            return this.Discard;
        }
        if (deckType == DeckType.Hand)
        {
            return this.Hand;
        }
        return null;
    }

    public int GetDifficultyModifier()
    {
        int num = 0;
        for (int i = 0; i < this.Effects.Count; i++)
        {
            if (this.Effects[i].Type == EffectType.ModifyDifficulty)
            {
                EffectModifyDifficulty difficulty = this.Effects[i] as EffectModifyDifficulty;
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
        if ((index >= 0) && (index < this.Effects.Count))
        {
            return this.Effects[index];
        }
        return null;
    }

    public Effect GetEffect(string ID)
    {
        for (int i = 0; i < this.Effects.Count; i++)
        {
            if (this.Effects[i].source == ID)
            {
                return this.Effects[i];
            }
        }
        return null;
    }

    public string GetEffectText()
    {
        string str = null;
        for (int i = 0; i < this.Effects.Count; i++)
        {
            str = str + this.Effects[i].GetDisplayText() + Environment.NewLine;
        }
        return str;
    }

    public int GetNumberDiscardableCards()
    {
        int num = 0;
        for (int i = 0; i < this.Hand.Count; i++)
        {
            if (!this.Hand[i].Displayed)
            {
                num++;
            }
        }
        return num;
    }

    public int GetNumEffects() => 
        this.Effects.Count;

    public int GetNumEffectsOfType(EffectType effectType)
    {
        int num = 0;
        for (int i = 0; i < this.Effects.Count; i++)
        {
            if (this.Effects[i].Type == effectType)
            {
                num++;
            }
        }
        return num;
    }

    public Power GetPower(string id)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            if (this.Powers[i].ID == id)
            {
                return this.Powers[i];
            }
        }
        return null;
    }

    public int GetSkillBonus(SkillCheckType skill)
    {
        bool flag = (skill == SkillCheckType.Combat) && (Turn.CombatSkill == SkillCheckType.Melee);
        SkillCheckType type = skill;
        if (skill == SkillCheckType.Combat)
        {
            skill = Turn.CombatSkill;
        }
        SkillType type2 = skill.ToSkillType();
        int skillRank = this.GetSkillRank(type2);
        if (skill == SkillCheckType.Strength)
        {
            skillRank += this.GetAttributeRank(AttributeType.Strength);
        }
        if (skill == SkillCheckType.Dexterity)
        {
            skillRank += this.GetAttributeRank(AttributeType.Dexterity);
        }
        if (skill == SkillCheckType.Constitution)
        {
            skillRank += this.GetAttributeRank(AttributeType.Constitution);
        }
        if (skill == SkillCheckType.Intelligence)
        {
            skillRank += this.GetAttributeRank(AttributeType.Intelligence);
        }
        if (skill == SkillCheckType.Wisdom)
        {
            skillRank += this.GetAttributeRank(AttributeType.Wisdom);
        }
        if (skill == SkillCheckType.Charisma)
        {
            skillRank += this.GetAttributeRank(AttributeType.Charisma);
        }
        if (flag)
        {
            skillRank += this.GetAttributeRank(AttributeType.Strength);
        }
        else
        {
            for (int k = 0; k < this.Skills.Length; k++)
            {
                if (this.Skills[k].skill == type2)
                {
                    skillRank += this.GetAttributeRank(this.Skills[k].attribute);
                    break;
                }
            }
        }
        for (int i = 0; i < this.Powers.Count; i++)
        {
            CharacterPowerAddSkillDice component = this.Powers[i].GetComponent<CharacterPowerAddSkillDice>();
            if (((component != null) && component.IsValid()) && component.Passive)
            {
                skillRank += component.DiceBonus;
            }
        }
        for (int j = 0; j < this.Effects.Count; j++)
        {
            if (this.Effects[j].Type == EffectType.BoostCheck)
            {
                EffectBoostCheck check = this.Effects[j] as EffectBoostCheck;
                if (check != null)
                {
                    SkillCheckType[] skills = new SkillCheckType[] { skill, type };
                    skillRank += check.GetSkillModifier(skills);
                }
            }
            if (this.Effects[j].Type == EffectType.ModifySkill)
            {
                EffectModifySkill skill2 = this.Effects[j] as EffectModifySkill;
                if (skill2 != null)
                {
                    skillRank += skill2.GetSkillModifier(skill);
                }
            }
            if (this.Effects[j].Type == EffectType.ModifyAttribute)
            {
                EffectModifyAttribute attribute = this.Effects[j] as EffectModifyAttribute;
                if (attribute != null)
                {
                    bool flag2 = false;
                    if (!flag2 && flag)
                    {
                        skillRank += attribute.GetAttributeModifier(AttributeType.Strength);
                        flag2 = true;
                    }
                    if (!flag2)
                    {
                        for (int m = 0; m < this.Skills.Length; m++)
                        {
                            if (this.Skills[m].skill == type2)
                            {
                                AttributeType type3 = this.Skills[m].attribute;
                                skillRank += attribute.GetAttributeModifier(type3);
                                flag2 = true;
                                break;
                            }
                        }
                    }
                    if (!flag2)
                    {
                        if (skill == SkillCheckType.Strength)
                        {
                            skillRank += attribute.GetAttributeModifier(AttributeType.Strength);
                        }
                        if (skill == SkillCheckType.Dexterity)
                        {
                            skillRank += attribute.GetAttributeModifier(AttributeType.Dexterity);
                        }
                        if (skill == SkillCheckType.Constitution)
                        {
                            skillRank += attribute.GetAttributeModifier(AttributeType.Constitution);
                        }
                        if (skill == SkillCheckType.Intelligence)
                        {
                            skillRank += attribute.GetAttributeModifier(AttributeType.Intelligence);
                        }
                        if (skill == SkillCheckType.Wisdom)
                        {
                            skillRank += attribute.GetAttributeModifier(AttributeType.Wisdom);
                        }
                        if (skill == SkillCheckType.Charisma)
                        {
                            skillRank += attribute.GetAttributeModifier(AttributeType.Charisma);
                        }
                    }
                }
            }
        }
        return skillRank;
    }

    public DiceType GetSkillDice(SkillCheckType skill)
    {
        if (skill == SkillCheckType.Combat)
        {
            skill = Turn.CombatSkill;
        }
        if (skill == SkillCheckType.None)
        {
            return DiceType.D0;
        }
        for (int i = 0; i < this.Effects.Count; i++)
        {
            if (this.Effects[i].Type == EffectType.SkillChange)
            {
                EffectChangeSkill skill2 = this.Effects[i] as EffectChangeSkill;
                if ((skill2 != null) && (skill2.Skill == skill.ToSkillType()))
                {
                    return this.GetAttributeDice(skill2.Attribute);
                }
            }
        }
        if (skill == SkillCheckType.Strength)
        {
            return this.Strength;
        }
        if (skill == SkillCheckType.Constitution)
        {
            return this.Constitution;
        }
        if (skill == SkillCheckType.Dexterity)
        {
            return this.Dexterity;
        }
        if (skill == SkillCheckType.Wisdom)
        {
            return this.Wisdom;
        }
        if (skill == SkillCheckType.Intelligence)
        {
            return this.Intelligence;
        }
        if (skill == SkillCheckType.Charisma)
        {
            return this.Charisma;
        }
        for (int j = 0; j < this.Skills.Length; j++)
        {
            if (this.Skills[j].skill == skill.ToSkillType())
            {
                return this.GetAttributeDice(this.Skills[j].attribute);
            }
        }
        return DiceType.D4;
    }

    public int GetSkillRank(SkillCheckType skill)
    {
        for (int i = 0; i < this.Effects.Count; i++)
        {
            if (this.Effects[i].Type == EffectType.SkillChange)
            {
                EffectChangeSkill skill2 = this.Effects[i] as EffectChangeSkill;
                if ((skill2 != null) && (skill2.Skill == skill.ToSkillType()))
                {
                    return skill2.Amount;
                }
            }
        }
        return this.SkillRank[(int) skill.ToSkillType()];
    }

    public int GetSkillRank(SkillType skill)
    {
        for (int i = 0; i < this.Effects.Count; i++)
        {
            if (this.Effects[i].Type == EffectType.SkillChange)
            {
                EffectChangeSkill skill2 = this.Effects[i] as EffectChangeSkill;
                if ((skill2 != null) && (skill2.Skill == skill))
                {
                    return skill2.Amount;
                }
            }
        }
        return this.SkillRank[(int) skill];
    }

    public bool HasCompleted(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }
        return this.Rewards.Contains(id);
    }

    public bool HasPower(string id) => 
        ((id != null) && this.KnownPowers.Contains(id));

    public bool HasReward(Reward reward)
    {
        if (reward == null)
        {
            return false;
        }
        if (string.IsNullOrEmpty(reward.ID))
        {
            return false;
        }
        return this.Rewards.Contains(reward.ID);
    }

    public bool HasStarterCard(string id)
    {
        if (this.CardsAtScenarioStart == null)
        {
            return true;
        }
        for (int i = 0; i < this.CardsAtScenarioStart.Length; i++)
        {
            if (this.CardsAtScenarioStart[i] == id)
            {
                return true;
            }
        }
        return false;
    }

    public void Initialize()
    {
        this.BuildDeck();
        this.BuildHand();
    }

    public bool IsCardTypeMarked(CardType type)
    {
        if (Rules.IsUnlimitedPlayPossible(type))
        {
            return false;
        }
        return (this.CardMarks[(int) type] > 0);
    }

    public bool IsOverHandSize() => 
        (this.GetNumberDiscardableCards() > this.HandSize);

    public bool IsSkillCheckAutomatic(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            if (this.Powers[i] is CharacterPowerSucceedSkillCheck)
            {
                CharacterPowerSucceedSkillCheck check = this.Powers[i] as CharacterPowerSucceedSkillCheck;
                if (check.Resolve(card))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Levelup(ProficencyType proficency)
    {
        if (proficency == ProficencyType.Weapons)
        {
            this.ProficientWithWeapons = true;
        }
        if (proficency == ProficencyType.LightArmor)
        {
            this.ProficientWithLightArmors = true;
        }
        if (proficency == ProficencyType.HeavyArmor)
        {
            this.ProficientWithHeavyArmors = true;
        }
    }

    public void Levelup(int delta)
    {
        this.HandSize += delta;
    }

    public void Levelup(string power)
    {
        if (!this.KnownPowers.Contains(power))
        {
            PowerTableEntry entry = PowerTable.Get(power);
            if (entry != null)
            {
                for (int i = 0; i < this.KnownPowers.Count; i++)
                {
                    PowerTableEntry entry2 = PowerTable.Get(this.KnownPowers[i]);
                    if ((entry2 != null) && (entry2.Family == entry.Family))
                    {
                        this.KnownPowers.Insert(i, power);
                        break;
                    }
                }
                if (!this.KnownPowers.Contains(power))
                {
                    this.KnownPowers.Add(power);
                }
            }
        }
        this.SynchronizeKnownPowers();
    }

    public void Levelup(AttributeType attribute, int delta)
    {
        this.AttributeRank[(int) attribute] += delta;
    }

    public void Levelup(CardType card, int delta)
    {
        this.CardRank[(int) card] += delta;
    }

    public void LevelupRole(RoleTableEntry role)
    {
        for (int i = 0; i < this.Roles.Length; i++)
        {
            if (RoleTable.Get(this.Roles[i]).Equals(role))
            {
                this.Role = i;
                this.LevelupHandSizeMax = role.HandSize;
            }
        }
    }

    public void LockInDisplayed(bool confirmed)
    {
        for (int i = 0; i < this.Hand.Count; i++)
        {
            if (this.Hand[i].Displayed)
            {
                this.Hand[i].Locked = confirmed;
            }
        }
    }

    public void MarkCardType(CardType type, bool isMarked)
    {
        if (isMarked)
        {
            if ((Turn.State != GameStateType.Finish) && (Turn.State != GameStateType.Setup))
            {
                this.CardMarks[(int) type]++;
            }
        }
        else if (this.CardMarks[(int) type] > 0)
        {
            this.CardMarks[(int) type]--;
        }
    }

    public void OnCardDeactivated(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            this.Powers[i].OnCardDeactivated(card);
        }
    }

    public void OnCardDefeated(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            this.Powers[i].OnCardDefeated(card);
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
            this.Powers[i].OnCardDiscarded(card);
        }
    }

    public void OnCardEncountered(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            this.Powers[i].OnCardEncountered(card);
        }
    }

    public void OnCardEvaded(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            this.Powers[i].OnCardEvaded(card);
        }
    }

    public void OnCardPlayed(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            this.Powers[i].OnCardPlayed(card);
        }
        Event[] components = card.GetComponents<Event>();
        for (int j = 0; j < components.Length; j++)
        {
            if (components[j].IsEventImplemented(EventType.OnCardPlayed))
            {
                Game.Events.Add(EventCallbackType.Card, card.ID, EventType.OnCardPlayed, j, components[j].Stateless, card);
            }
        }
    }

    public void OnCardUndefeated(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            this.Powers[i].OnCardUndefeated(card);
        }
    }

    public void OnCheckCompleted()
    {
        this.Effects.OnCheckCompleted();
        for (int i = 0; i < this.Powers.Count; i++)
        {
            this.Powers[i].OnCheckCompleted();
        }
        this.ClearCardTypes();
        this.LockInDisplayed(true);
    }

    public void OnEncounterComplete()
    {
        this.Effects.OnEncounterComplete();
    }

    public void OnLoadData()
    {
        byte[] buffer;
        if (Game.GetObjectData(this.ID, out buffer))
        {
            ByteStream bs = new ByteStream(buffer);
            if (bs != null)
            {
                this.OnLoadData(bs);
            }
        }
    }

    public void OnLoadData(ByteStream bs)
    {
        bs.ReadInt();
        this.NickName = bs.ReadString();
        this.Alive = bs.ReadBool();
        this.Active = (ActiveType) bs.ReadInt();
        this.HordeFightLeft = bs.ReadInt();
        this.XP = bs.ReadInt();
        this.XPX = bs.ReadInt();
        this.HandSize = bs.ReadInt();
        this.FavoredCard = (CardType) bs.ReadInt();
        this.ProficientWithWeapons = bs.ReadBool();
        this.ProficientWithLightArmors = bs.ReadBool();
        this.ProficientWithHeavyArmors = bs.ReadBool();
        this.Location = bs.ReadString();
        this.Role = bs.ReadInt();
        this.AttributeRank = bs.ReadIntArray();
        this.SkillRank = bs.ReadIntArray();
        this.CardRank = bs.ReadIntArray();
        this.CardMarks = bs.ReadIntArray();
        this.Deck.FromStream(bs);
        this.Discard.FromStream(bs);
        this.Bury.FromStream(bs);
        this.Hand.FromStream(bs);
        this.Recharge.FromStream(bs);
        this.CardsAtScenarioStart = bs.ReadStringArray();
        this.Rewards.Clear();
        this.Rewards.AddRange(bs.ReadStringArray());
        this.KnownPowers.Clear();
        this.KnownPowers.AddRange(bs.ReadStringArray());
        this.SynchronizeKnownPowers();
        this.Effects.FromArray(bs.ReadByteArray());
        this.ReadSkillsArray(bs);
    }

    public void OnLocationExplored(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            this.Powers[i].OnLocationExplored(card);
        }
    }

    public void OnPlayerDamaged(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            this.Powers[i].OnPlayerDamaged(card);
        }
    }

    public void OnSaveData()
    {
        ByteStream bs = new ByteStream();
        if (bs != null)
        {
            this.OnSaveData(bs);
            Game.SetObjectData(this.ID, bs.ToArray());
        }
    }

    public void OnSaveData(ByteStream bs)
    {
        bs.WriteInt(1);
        bs.WriteString(this.NickName);
        bs.WriteBool(this.Alive);
        bs.WriteInt((int) this.Active);
        bs.WriteInt(this.HordeFightLeft);
        bs.WriteInt(this.XP);
        bs.WriteInt(this.XPX);
        bs.WriteInt(this.HandSize);
        bs.WriteInt((int) this.FavoredCard);
        bs.WriteBool(this.ProficientWithWeapons);
        bs.WriteBool(this.ProficientWithLightArmors);
        bs.WriteBool(this.ProficientWithHeavyArmors);
        bs.WriteString(this.Location);
        bs.WriteInt(this.Role);
        bs.WriteIntArray(this.AttributeRank);
        bs.WriteIntArray(this.SkillRank);
        bs.WriteIntArray(this.CardRank);
        bs.WriteIntArray(this.CardMarks);
        this.Deck.ToStream(bs);
        this.Discard.ToStream(bs);
        this.Bury.ToStream(bs);
        this.Hand.ToStream(bs);
        this.Recharge.ToStream(bs);
        bs.WriteStringArray(this.CardsAtScenarioStart);
        bs.WriteStringArray(this.Rewards.ToArray());
        bs.WriteStringArray(this.KnownPowers.ToArray());
        bs.WriteByteArray(this.Effects.ToArray());
        this.WriteSkillsArray(bs);
    }

    public void OnScenarioExit()
    {
        this.XPX = 0;
    }

    public void OnScenarioStart()
    {
        this.Deck.Shuffle();
        this.XPX = 0;
        this.CardsAtScenarioStart = new string[this.Deck.Count + this.Hand.Count];
        for (int i = 0; i < this.Deck.Count; i++)
        {
            this.CardsAtScenarioStart[i] = this.Deck[i].ID;
        }
        for (int j = 0; j < this.Hand.Count; j++)
        {
            this.CardsAtScenarioStart[this.Deck.Count + j] = this.Hand[j].ID;
        }
        for (int k = Scenario.Current.Powers.Count - 1; k >= 0; k--)
        {
            if (!Scenario.Current.Powers[k].IsHiddenPower())
            {
                Effect e = new EffectScenarioPower(Scenario.Current.Powers[k].ID, Effect.DurationPermament);
                this.ApplyEffect(e);
            }
        }
    }

    public void OnSecondCombat(Card card)
    {
        for (int i = 0; i < this.Powers.Count; i++)
        {
            this.Powers[i].OnSecondCombat(card);
        }
    }

    public void OnStepCompleted()
    {
        this.ClearCardTypes();
    }

    public void OnTurnCompleted()
    {
        this.Effects.OnTurnCompleted();
    }

    private void ReadSkillsArray(ByteStream bs)
    {
        int num = bs.ReadInt();
        this.Skills = new SkillValueType[num];
        for (int i = 0; i < num; i++)
        {
            this.Skills[i] = new SkillValueType { 
                skill = (SkillType) bs.ReadInt(),
                rank = bs.ReadInt(),
                attribute = (AttributeType) bs.ReadInt()
            };
        }
    }

    public void RemoveEffect(Effect e)
    {
        this.Effects.Remove(e);
    }

    public void RemoveEffect(string ID)
    {
        for (int i = 0; i < this.Effects.Count; i++)
        {
            if (this.Effects[i].source == ID)
            {
                this.Effects.RemoveAt(i);
                return;
            }
        }
    }

    public void ResetDeck()
    {
        this.Deck.Combine(this.Hand);
        this.Deck.Combine(this.Bury);
        this.Deck.Combine(this.Discard);
        this.Deck.Combine(this.Recharge);
        for (int i = 0; i < this.Deck.Count; i++)
        {
            this.Deck[i].Clear();
        }
    }

    public void SetSkillRank(SkillType skill, int rank)
    {
        this.SkillRank[(int) skill] = rank;
    }

    private void SynchronizeKnownPowers()
    {
        this.myPowers.Clear();
        for (int i = 0; i < this.KnownPowers.Count; i++)
        {
            PowerTableEntry entry = PowerTable.Get(this.KnownPowers[i]);
            if (entry != null)
            {
                if (entry.Modifier)
                {
                    CharacterPower[] collection = CharacterPower.Learn(this, this.KnownPowers[i]);
                    if (collection != null)
                    {
                        this.myPowers.AddRange(collection);
                    }
                }
                else
                {
                    int rank = entry.Rank;
                    for (int k = 0; k < this.KnownPowers.Count; k++)
                    {
                        PowerTableEntry entry2 = PowerTable.Get(this.KnownPowers[k]);
                        if (((entry2 != null) && !entry2.Modifier) && (entry2.Family == entry.Family))
                        {
                            rank = Mathf.Max(rank, entry2.Rank);
                        }
                    }
                    if (entry.Rank == rank)
                    {
                        CharacterPower[] powerArray2 = CharacterPower.Learn(this, this.KnownPowers[i]);
                        if (powerArray2 != null)
                        {
                            this.myPowers.AddRange(powerArray2);
                        }
                    }
                    else
                    {
                        CharacterPower.Forget(this, this.KnownPowers[i]);
                    }
                }
            }
        }
        for (int j = 0; j < this.myPowers.Count; j++)
        {
            this.myPowers[j].Initialize(this);
        }
    }

    private void WriteSkillsArray(ByteStream bs)
    {
        bs.WriteInt(this.Skills.Length);
        for (int i = 0; i < this.Skills.Length; i++)
        {
            bs.WriteInt((int) this.Skills[i].skill);
            bs.WriteInt(this.Skills[i].rank);
            bs.WriteInt((int) this.Skills[i].attribute);
        }
    }

    public ActiveType Active { get; set; }

    public bool Alive { get; set; }

    public Deck Bury =>
        this.myBury;

    public bool CanMove
    {
        get
        {
            if (Location.Current.ID == this.Location)
            {
                if ((Location.Current.Deck.Count > 0) && (Location.Current.Deck[0].Blocker == BlockerType.Movement))
                {
                    return false;
                }
            }
            else if (Location.IsBlocked(this.Location))
            {
                return false;
            }
            for (int i = 0; i < Scenario.Current.Powers.Count; i++)
            {
                ScenarioPowerMoveRestriction component = Scenario.Current.Powers[i].GetComponent<ScenarioPowerMoveRestriction>();
                if ((component != null) && component.IsLocationValid(this.Location))
                {
                    return false;
                }
            }
            return true;
        }
    }

    public DiceType Charisma =>
        this.GetAttributeDice(AttributeType.Charisma);

    public DiceType Constitution =>
        this.GetAttributeDice(AttributeType.Constitution);

    public Deck Deck =>
        this.myDeck;

    public DiceType Dexterity =>
        this.GetAttributeDice(AttributeType.Dexterity);

    public Deck Discard =>
        this.myDiscard;

    public Deck Hand =>
        this.myHand;

    public int HordeFightLeft { get; set; }

    public DiceType Intelligence =>
        this.GetAttributeDice(AttributeType.Intelligence);

    public int Level =>
        Rules.GetLevelFromExperiencePoints(this.XP);

    public string Location { get; set; }

    public List<CharacterPower> Powers =>
        this.myPowers;

    public Deck Recharge =>
        this.myRecharge;

    public int Role { get; private set; }

    public bool Selected { get; set; }

    public DiceType Strength =>
        this.GetAttributeDice(AttributeType.Strength);

    public int Tier =>
        Rules.GetTierFromExperiencePoints(this.XP);

    public DiceType Wisdom =>
        this.GetAttributeDice(AttributeType.Wisdom);

    public int XP { get; set; }

    public int XPX { get; set; }
}

