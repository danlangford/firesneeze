using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelDice : GuiPanel
{
    [Tooltip("play this sound if flat bonus increases beyond the character's base")]
    public AudioClip ApplyNumberBuff;
    [Tooltip("reference to the dice bonus label within our hierarchy")]
    public GuiLabel DiceBonusLabel;
    [Tooltip("reference to the dice bonus label animator within our hierarchy")]
    public Animator DiceBonusLabelAnimator;
    private Transform DiceHolder;
    private DiceInstance[] diceInstances;
    public Color DiceLoseClor = Color.magenta;
    [Tooltip("prefab for D10 dice model")]
    public GameObject DiceModelD10;
    [Tooltip("prefab for D12 dice model")]
    public GameObject DiceModelD12;
    [Tooltip("prefab for D4  dice model")]
    public GameObject DiceModelD4;
    [Tooltip("prefab for D6  dice model")]
    public GameObject DiceModelD6;
    [Tooltip("prefab for D8  dice model")]
    public GameObject DiceModelD8;
    [Tooltip("prefab for D10 \"red\" enemy dice model")]
    public GameObject DiceModelE10;
    [Tooltip("prefab for D12 \"red\" enemy dice model")]
    public GameObject DiceModelE12;
    [Tooltip("prefab for D4 \"red\" enemy dice model")]
    public GameObject DiceModelE4;
    [Tooltip("prefab for D6 \"red\" enemy dice model")]
    public GameObject DiceModelE6;
    [Tooltip("prefab for D8 \"red\" enemy dice model")]
    public GameObject DiceModelE8;
    private static Vector2[] diceOffsets;
    private Transform DiceSpawner;
    [Tooltip("reference to the total label within our hierarchy")]
    public GuiLabel DiceTotalLabel;
    [Tooltip("reference to the dice total loss vfx within our hierarchy")]
    public GameObject DiceTotalLossVfx;
    [Tooltip("reference to the dice total number change vfx within our hierarchy")]
    public GameObject DiceTotalNumberChangeVfx;
    [Tooltip("reference to the dice total win vfx within our hierarchy")]
    public GameObject DiceTotalWinVfx;
    public Color DiceWinColor = Color.green;
    private bool isDiceShowing;
    private bool isNewCheck;
    private bool isPanelShowing;
    private bool isRolling;
    private int lastDiceBonus;
    private const int MAX_DISPLAY_DICE = 0x1a;
    private GameObject[] numberChangeVfxs = new GameObject[0x1a];
    [Tooltip("reference to the check panel background art in this scene")]
    public GameObject PanelBackground;
    [Tooltip("reference to the pass button within our hierarchy")]
    public GuiButton PassButton;
    [Tooltip("play this sound if the player failed (red vfx)")]
    public AudioClip PlayerLose;
    [Tooltip("play this sound if the player succeeded (blue vfx)")]
    public AudioClip PlayerWin;
    [Tooltip("prefab for D12 auto-randomized damage")]
    public GameObject RandomizedDamageD12;
    [Tooltip("prefab for D4 auto-randomized damage")]
    public GameObject RandomizedDamageD4;
    [Tooltip("prefab for D6 auto-randomized damage")]
    public GameObject RandomizedDamageD6;
    [Tooltip("prefab for auto-randomized difficulty")]
    public GameObject RandomizedDifficulty;
    [Tooltip("prefab for auto-randomized powers")]
    public GameObject RandomizedPowers;
    [Tooltip("reference to the reroll button within our hierarchy")]
    public GuiButton RerollButton;
    [Tooltip("play this sound if 7 or more dice are rolled")]
    public AudioClip RollDiceLargeSound;
    [Tooltip("play this sound if 4 to 6 dice are rolled")]
    public AudioClip RollDiceMediumSound;
    [Tooltip("play this sound when 3 or less dice are rolled")]
    public AudioClip RollDiceSmallSound;
    [Tooltip("reference to the button that pops down the skills menu")]
    public GuiButtonRegion SkillsButton;
    [Tooltip("reference to the skills panel in this scene")]
    public GuiPanelSkills SkillsPanel;

    private bool ActivateReroll()
    {
        if ((Turn.State == GameStateType.Combat) || (Turn.State == GameStateType.Reroll))
        {
            this.isRolling = false;
            Turn.State = GameStateType.Reroll;
            return true;
        }
        if ((Turn.State != GameStateType.Roll) && (Turn.State != GameStateType.Close))
        {
            return false;
        }
        this.isRolling = false;
        Turn.PushReturnState();
        Turn.State = GameStateType.RollAgain;
        return true;
    }

    private void CalculateDiceAnalytics(int roll, DiceType dice, ref List<string> rolls, ref Dictionary<string, int> rollData)
    {
        string str;
        int num;
        if (rollData.ContainsKey("min"))
        {
            Dictionary<string, int> dictionary;
            num = dictionary[str];
            (dictionary = rollData)[str = "min"] = num + 1;
        }
        else
        {
            rollData.Add("min", 1);
        }
        if (rollData.ContainsKey("max"))
        {
            Dictionary<string, int> dictionary2;
            num = dictionary2[str];
            (dictionary2 = rollData)[str = "max"] = num + 1;
        }
        else
        {
            rollData.Add("max", (int) dice);
        }
        if (rollData.ContainsKey(dice.ToString().ToLower()))
        {
            Dictionary<string, int> dictionary3;
            num = dictionary3[str];
            (dictionary3 = rollData)[str = dice.ToString().ToLower()] = num + 1;
        }
        else
        {
            rollData.Add(dice.ToString().ToLower(), 1);
        }
        rolls.Add(dice.ToString().ToLower() + ":" + roll);
    }

    public override void Clear()
    {
        this.ShowCheck(false);
        this.SkillsPanel.Clear();
        this.DiceTotalLabel.Clear();
        this.DiceBonusLabel.Clear();
        this.DiceBonusLabel.Show(false);
        this.isRolling = false;
        this.lastDiceBonus = 0;
        this.Refresh();
    }

    private int CountMyDice(DiceType type)
    {
        int num = 0;
        for (int i = 0; i < this.diceInstances.Length; i++)
        {
            if ((this.diceInstances[i] != null) && ((this.diceInstances[i].type == type) || (type == DiceType.D0)))
            {
                num++;
            }
        }
        return num;
    }

    private int CountTurnDice(DiceType type)
    {
        int num = 0;
        for (int i = 0; i < Turn.Dice.Count; i++)
        {
            if ((((DiceType) Turn.Dice[i]) == type) || (type == DiceType.D0))
            {
                num++;
            }
        }
        return num;
    }

    private void CreateDice(DiceType type, GameObject model)
    {
        if (model != null)
        {
            int index = this.FindEmptySlot();
            if ((index >= 0) && (index < this.diceInstances.Length))
            {
                GameObject go = UnityEngine.Object.Instantiate(model, this.DiceSpawner.position, Quaternion.identity) as GameObject;
                if (go != null)
                {
                    Vector3 offScreen;
                    this.diceInstances[index] = new DiceInstance(go, type);
                    go.name = model.name;
                    go.transform.parent = this.DiceHolder;
                    if (this.IsDiceVisible())
                    {
                        offScreen = new Vector3(diceOffsets[index].x + this.Offset.x, diceOffsets[index].y + this.Offset.y, this.DiceSpawner.position.z);
                    }
                    else
                    {
                        offScreen = this.OffScreen;
                    }
                    LeanTween.moveLocal(go, offScreen, 0.3f).setEase(LeanTweenType.easeOutQuad);
                }
            }
        }
    }

    private void DestroyDice(DiceType type)
    {
        for (int i = 0; i < this.diceInstances.Length; i++)
        {
            if ((this.diceInstances[i] != null) && (this.diceInstances[i].type == type))
            {
                LeanTween.move(this.diceInstances[i].gameObject, this.DiceSpawner.position, 0.3f).setEase(LeanTweenType.easeInQuad).setOnComplete(new Action<object>(this.DestroyDiceComplete)).setOnCompleteParam(this.diceInstances[i].gameObject);
                this.diceInstances[i] = null;
                break;
            }
        }
    }

    private void DestroyDiceComplete(object x)
    {
        GameObject obj2 = x as GameObject;
        if (obj2 != null)
        {
            UnityEngine.Object.Destroy(obj2);
        }
    }

    private void DestroyDiceWithDifferentModel(DiceType type, GameObject model)
    {
        for (int i = 0; i < this.diceInstances.Length; i++)
        {
            if (((this.diceInstances[i] != null) && (this.diceInstances[i].type == type)) && (this.diceInstances[i].model.name != model.name))
            {
                this.DestroyDice(type);
                break;
            }
        }
    }

    public void Fade(bool isVisible, float time)
    {
        if (isVisible && !this.isDiceShowing)
        {
            this.Show(true);
        }
        for (int i = 0; i < this.diceInstances.Length; i++)
        {
            if (this.diceInstances[i] != null)
            {
                this.diceInstances[i].model.Fade(isVisible, time);
            }
        }
        if (Turn.DiceBonus != 0)
        {
            this.DiceBonusLabel.Fade(isVisible, time);
        }
        this.isDiceShowing = isVisible;
    }

    private int FindEmptySlot()
    {
        if ((Turn.RollReason != RollType.PlayerControlled) && (Turn.RollReason != RollType.StandardEnemyDice))
        {
            for (int j = 0; j < this.diceInstances.Length; j++)
            {
                if ((((j == 0) || (j == 6)) || ((j == 10) || (j == 0x16))) && (this.diceInstances[j] == null))
                {
                    return j;
                }
            }
        }
        for (int i = 1; i < this.diceInstances.Length; i++)
        {
            if (this.diceInstances[i] == null)
            {
                return i;
            }
        }
        return 0;
    }

    private GameObject GetDiceModel(DiceType type)
    {
        DiceType type3;
        switch (Turn.RollReason)
        {
            case RollType.PlayerControlled:
                switch (type)
                {
                    case DiceType.D4:
                        return this.DiceModelD4;

                    case DiceType.D6:
                        return this.DiceModelD6;

                    case DiceType.D8:
                        return this.DiceModelD8;

                    case DiceType.D10:
                        return this.DiceModelD10;

                    case DiceType.D12:
                        return this.DiceModelD12;
                }
                goto Label_014C;

            case RollType.EnemyDamage:
                switch (type)
                {
                    case DiceType.D4:
                        return this.RandomizedDamageD4;

                    case DiceType.D6:
                        return this.RandomizedDamageD6;
                }
                break;

            case RollType.EnemyIncreaseDifficulty:
                type3 = type;
                if (type3 == DiceType.D4)
                {
                    return this.RandomizedDifficulty;
                }
                goto Label_014C;

            case RollType.EnemyRandomPower:
                type3 = type;
                if (type3 == DiceType.D4)
                {
                    return this.RandomizedPowers;
                }
                goto Label_014C;

            case RollType.StandardEnemyDice:
                switch (type)
                {
                    case DiceType.D4:
                        return this.DiceModelE4;

                    case DiceType.D6:
                        return this.DiceModelE6;

                    case DiceType.D8:
                        return this.DiceModelE8;

                    case DiceType.D10:
                        return this.DiceModelE10;

                    case DiceType.D12:
                        return this.DiceModelE12;
                }
                goto Label_014C;

            default:
                goto Label_014C;
        }
        if (type3 == DiceType.D12)
        {
            return this.RandomizedDamageD12;
        }
    Label_014C:
        return null;
    }

    private CardPropertyModifyDice GetDiceModifier(string ID)
    {
        Card card = null;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (window.layoutReveal.Deck[ID] != null)
            {
                card = window.layoutReveal.Deck[ID];
            }
            if ((card == null) && (window.layoutDiscard.Deck[ID] != null))
            {
                card = window.layoutDiscard.Deck[ID];
            }
        }
        if ((card != null) && (card.GetComponent<CardPropertyModifyDice>() != null))
        {
            return card.GetComponent<CardPropertyModifyDice>();
        }
        return null;
    }

    public override void Initialize()
    {
        this.DiceHolder = base.transform.FindChild("Dice");
        this.DiceSpawner = base.transform.FindChild("Spawn");
        this.InitializeDiceOffsets();
        this.InitializeDiceInstances();
        this.PanelBackground.SetActive(false);
        this.SkillsButton.Show(false);
        this.Clear();
    }

    private void InitializeDiceInstances()
    {
        this.diceInstances = new DiceInstance[0x1a];
        for (int i = 0; i < this.diceInstances.Length; i++)
        {
            this.diceInstances[i] = null;
        }
    }

    private void InitializeDiceOffsets()
    {
        if (diceOffsets == null)
        {
            diceOffsets = new Vector2[] { 
                new Vector2(0f, 0f), new Vector2(-1.4f, -0.81f), new Vector2(-0.07f, -1.62f), new Vector2(1.4f, -0.86f), new Vector2(-1.46f, 0.83f), new Vector2(1.43f, 0.8f), new Vector2(0.01f, 1.6f), new Vector2(-2.76f, -0.01f), new Vector2(2.8f, 0f), new Vector2(-1.5f, -2.4f), new Vector2(1.28f, -2.4f), new Vector2(-2.81f, -1.62f), new Vector2(2.69f, -1.76f), new Vector2(2.8f, 1.58f), new Vector2(4f, -1f), new Vector2(4.04f, -2.62f),
                new Vector2(-2.83f, 1.46f), new Vector2(4.04f, -2.62f), new Vector2(-1.48f, 2.42f), new Vector2(4.18f, 0.78f), new Vector2(-4.22f, 0.55f), new Vector2(-4.22f, -1.03f), new Vector2(-0.04f, 3.15f), new Vector2(-3.99f, -2.59f), new Vector2(4.13f, 2.42f), new Vector2(-5.52f, -1.98f)
            };
        }
    }

    public bool IsDiceVisible() => 
        this.isDiceShowing;

    private void OnPassButtonPushed()
    {
        if (!UI.Window.Paused)
        {
            this.PassButton.Show(false);
            Turn.Pass = false;
            Turn.TargetType = TargetType.AnotherAtLocation;
            Turn.Dice.Clear();
            this.Refresh();
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.CancelAllPowers(true, true);
            }
            Turn.PushCancelDestination(new TurnStateCallback(TurnStateCallbackType.Global, "EventPassCheck_Cancel"));
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "EventPassCheck_Proceed"));
            GameStateTarget.DisplayText = StringTableManager.GetHelperText(0x2a);
            Turn.State = GameStateType.Target;
        }
    }

    private void OnRerollButtonPushed()
    {
        if (!UI.Window.Paused)
        {
            this.isRolling = false;
            if (Rules.IsRerollForced(Turn.Card))
            {
                EventCombatReroll component = Turn.Card.GetComponent<EventCombatReroll>();
                if (component != null)
                {
                    component.CombatReroll();
                    Turn.Defeat = false;
                }
                this.Roll();
            }
            else
            {
                Card card = Turn.Owner.Hand[Turn.Reroll];
                if (card != null)
                {
                    GuiWindowLocation window = UI.Window as GuiWindowLocation;
                    if (window != null)
                    {
                        window.DiscardToLayout(card);
                    }
                }
                Turn.Reroll = Guid.Empty;
                this.Roll();
            }
        }
    }

    private void OnSkillsButtonPushed()
    {
        if (!UI.Window.Paused && !this.SkillsPanel.Busy)
        {
            this.SkillsPanel.Show(true);
            this.SkillsButton.Show(false);
        }
    }

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        if (this.isRolling)
        {
            for (int i = 0; i < this.diceInstances.Length; i++)
            {
                if (this.diceInstances[i] != null)
                {
                    this.diceInstances[i].model.Pause(isPaused);
                }
            }
        }
    }

    private void PlayDiceSound(int diceCount)
    {
        if (diceCount <= 3)
        {
            UI.Sound.Play(this.RollDiceSmallSound);
        }
        else if (diceCount <= 6)
        {
            UI.Sound.Play(this.RollDiceMediumSound);
        }
        else
        {
            UI.Sound.Play(this.RollDiceLargeSound);
        }
    }

    public override void Refresh()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Validate();
        }
        Turn.DiceBonus -= this.CountMyDice(DiceType.D0) * Turn.LastDiceModifier;
        Turn.LastDiceModifier = Rules.GetDiceModifier(DiceType.D0);
        Turn.DiceBonus += Turn.Dice.Count * Turn.LastDiceModifier;
        this.SynchronizeAllDice();
        if (((Turn.DiceBonus != 0) && (Turn.Dice.Count != 0)) && this.isDiceShowing)
        {
            this.RefreshBonusText(Turn.DiceBonus);
        }
        else
        {
            this.DiceBonusLabel.Clear();
            this.DiceBonusLabel.gameObject.SetActive(false);
        }
        this.PassButton.Show(Rules.IsPassCheckPossible());
    }

    private void RefreshBonusText(int bonus)
    {
        if (bonus > 0)
        {
            this.DiceBonusLabel.Text = "+ " + bonus.ToString();
        }
        else
        {
            this.DiceBonusLabel.Text = bonus.ToString();
        }
        if (!this.DiceBonusLabel.Visible)
        {
            this.DiceBonusLabel.gameObject.SetActive(true);
            this.DiceBonusLabel.Fade(true, 0.15f);
        }
        if (this.lastDiceBonus != bonus)
        {
            int skillBonus = Turn.Character.GetSkillBonus(Turn.Check);
            if ((this.lastDiceBonus != 0) || (bonus != skillBonus))
            {
                int num2 = (this.lastDiceBonus != 0) ? (this.lastDiceBonus - bonus) : bonus;
                this.DiceBonusLabelAnimator.SetInteger("Amount", num2);
                this.DiceBonusLabelAnimator.SetTrigger("Update");
                if (this.lastDiceBonus < bonus)
                {
                    UI.Sound.Play(this.ApplyNumberBuff);
                }
            }
        }
        this.lastDiceBonus = bonus;
    }

    public void Resolve()
    {
        Location.Current.OnDiceRolled();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.powersPanel.OnDiceRolled();
        }
        Tutorial.Notify(TutorialEventType.DiceRolled);
        if (Turn.State != GameStateType.Null)
        {
            this.ResolveDice();
        }
    }

    public void ResolveDice()
    {
        Rules.EnforceLegalDice();
        Turn.Dice.Clear();
        Turn.LastCheckDice = null;
        this.Clear();
        Turn.DiceBonus = 0;
        Turn.LastCheckBonus = 0;
        Turn.Resolve();
        if (Turn.Card != null)
        {
            Turn.Card.Animate(AnimationType.Focus, false);
        }
    }

    public void Roll()
    {
        this.Roll(Vector2.zero);
    }

    public void Roll(Vector2 direction)
    {
        if (!this.isRolling)
        {
            this.isRolling = true;
            this.Refresh();
            Turn.DiceRolls++;
            switch (Turn.RollReason)
            {
                case RollType.PlayerControlled:
                case RollType.StandardEnemyDice:
                    Game.Instance.StartCoroutine(this.RollNormalDice(direction));
                    break;

                case RollType.EnemyDamage:
                case RollType.EnemyIncreaseDifficulty:
                    Game.Instance.StartCoroutine(this.RollDamageDice(direction));
                    break;

                case RollType.EnemyRandomPower:
                    Game.Instance.StartCoroutine(this.RollPowersDice(direction));
                    break;
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator RollDamageDice(Vector2 direction) => 
        new <RollDamageDice>c__Iterator55 { 
            direction = direction,
            <$>direction = direction,
            <>f__this = this
        };

    private void RollDice(bool isRolling, Vector2 direction)
    {
        for (int i = 0; i < this.diceInstances.Length; i++)
        {
            if (this.diceInstances[i] != null)
            {
                DiceModel model = this.diceInstances[i].model;
                if (model != null)
                {
                    model.Roll(isRolling, direction);
                }
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator RollNormalDice(Vector2 direction) => 
        new <RollNormalDice>c__Iterator56 { 
            direction = direction,
            <$>direction = direction,
            <>f__this = this
        };

    [DebuggerHidden]
    private IEnumerator RollPowersDice(Vector2 direction) => 
        new <RollPowersDice>c__Iterator54 { 
            direction = direction,
            <$>direction = direction,
            <>f__this = this
        };

    public void Scamper(Vector2 offset, float time)
    {
        this.Offset += offset;
        for (int i = 0; i < this.diceInstances.Length; i++)
        {
            if (this.diceInstances[i] != null)
            {
                Vector3 to = this.diceInstances[i].gameObject.transform.localPosition + new Vector3(offset.x, offset.y, 0f);
                LeanTween.moveLocal(this.diceInstances[i].gameObject, to, time).setEase(LeanTweenType.easeInOutQuad);
            }
        }
    }

    public void SetCheck(Card card, SkillCheckValueType[] checks, SkillCheckType skill)
    {
        if (!Turn.Evade && !Turn.Defeat)
        {
            this.Show(true);
        }
        if ((checks.Length == 0) && (skill == SkillCheckType.None))
        {
            for (int i = 0; i < Turn.Owner.Powers.Count; i++)
            {
                CharacterPowerReplaceChecks checks2 = Turn.Owner.Powers[i] as CharacterPowerReplaceChecks;
                if (((checks2 != null) && checks2.ReplaceEmptyCheck) && checks2.IsValid())
                {
                    checks2.Activate();
                    return;
                }
            }
        }
        this.isNewCheck = true;
        if (Turn.Checks != null)
        {
            this.isNewCheck = !(Turn.Checks.SequenceEqual<SkillCheckValueType>(checks) && (Turn.Check == skill));
        }
        Turn.Checks = checks;
        Turn.LastCheck = Turn.Check;
        Turn.Check = skill;
        if (card != null)
        {
            Turn.DiceTarget = Rules.GetCheckValue(card, Turn.Check);
        }
        if (Turn.LastCheckDice != null)
        {
            for (int j = 0; j < Turn.LastCheckDice.Length; j++)
            {
                Turn.Dice.Remove(Turn.LastCheckDice[j]);
            }
        }
        Turn.DiceBonus -= Turn.LastCheckBonus;
        Turn.LastCheckDice = Rules.GetCheckDice(Turn.Check);
        if (Turn.LastCheckDice != null)
        {
            for (int k = 0; k < Turn.LastCheckDice.Length; k++)
            {
                Turn.Dice.Add(Turn.LastCheckDice[k]);
            }
        }
        Turn.LastCheckBonus = Rules.GetCheckBonus(Turn.Check);
        Turn.DiceBonus += Turn.LastCheckBonus;
        if (this.isNewCheck)
        {
            if (!Turn.Validate && (Turn.LastCheck != SkillCheckType.None))
            {
                UI.Sound.Play(SoundEffectType.SkillUsedChangedCheckToDefeat);
            }
            Turn.Validate = true;
            if (Turn.Check != skill)
            {
                Turn.Check = skill;
                if (card != null)
                {
                    Turn.DiceTarget = Rules.GetCheckValue(card, Turn.Check);
                }
            }
        }
        this.SetTitle(card, Turn.Check, Turn.DiceTarget);
        this.ShowCheck(true);
        this.Refresh();
    }

    public void SetDiceText(StrRefType[] powersText)
    {
        for (int i = 0; i < this.diceInstances.Length; i++)
        {
            if (this.diceInstances[i] != null)
            {
                RandomizedPowersDiceModel model = this.diceInstances[i].model as RandomizedPowersDiceModel;
                if (model != null)
                {
                    for (int j = 0; j < model.powerLabels.Length; j++)
                    {
                        model.powerLabels[j].Text = powersText[j].ToString();
                    }
                }
            }
        }
    }

    public void SetTitle(Card card, SkillCheckType skillType, int skillTarget)
    {
        this.SkillsPanel.SetTitle(card, skillType, skillTarget);
    }

    public override void Show(bool isVisible)
    {
        if (isVisible)
        {
            for (int i = 0; i < this.diceInstances.Length; i++)
            {
                if (this.diceInstances[i] != null)
                {
                    this.diceInstances[i].model.Reset();
                    Vector3 to = new Vector3(diceOffsets[i].x + this.Offset.x, diceOffsets[i].y + this.Offset.y, this.DiceSpawner.position.z);
                    LeanTween.moveLocal(this.diceInstances[i].gameObject, to, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                }
            }
            if (Turn.DiceBonus != 0)
            {
                this.DiceBonusLabel.gameObject.SetActive(true);
            }
        }
        else
        {
            for (int j = 0; j < this.diceInstances.Length; j++)
            {
                if (this.diceInstances[j] != null)
                {
                    this.diceInstances[j].model.Reset();
                    Vector3 offScreen = this.OffScreen;
                    LeanTween.moveLocal(this.diceInstances[j].gameObject, offScreen, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                }
            }
            if (Turn.DiceBonus != 0)
            {
                this.DiceBonusLabel.gameObject.SetActive(false);
            }
        }
        this.isDiceShowing = isVisible;
    }

    public void ShowCheck(bool isVisible)
    {
        if (isVisible)
        {
            if (!this.isPanelShowing)
            {
                this.SkillsButton.Show(true);
                this.PanelBackground.SetActive(isVisible);
                this.PanelBackground.GetComponent<Animator>().SetBool("Show", true);
                this.PanelBackground.GetComponent<Animator>().SetBool("Hide", false);
                UI.Sound.Play(SoundEffectType.SkillPanelAppear);
                if (Rules.IsPassCheckPossible())
                {
                    this.PassButton.Show(true);
                }
                this.isPanelShowing = true;
            }
        }
        else if (this.isPanelShowing)
        {
            this.SkillsButton.Show(false);
            this.PanelBackground.GetComponent<Animator>().SetBool("Hide", true);
            this.PanelBackground.GetComponent<Animator>().SetBool("Show", false);
            UI.Sound.Play(SoundEffectType.SkillPanelHide);
            this.PassButton.Show(false);
            this.isPanelShowing = false;
        }
    }

    public void ShowRerollButton(bool isVisible)
    {
        this.RerollButton.Text = StringTableManager.GetHelperText(40);
        this.RerollButton.Show(isVisible);
    }

    public void ShowResolveResult(bool isResolveSuccess)
    {
        if (isResolveSuccess)
        {
            UI.Sound.Play(this.PlayerWin);
        }
        else
        {
            UI.Sound.Play(this.PlayerLose);
        }
        if ((Turn.State == GameStateType.Roll) && (Turn.Damage > 0))
        {
            VisualEffectType cardLoseVfx = Rules.GetCardLoseVfx(Turn.DamageTraits);
            UI.Sound.Play(cardLoseVfx.ToSoundtype());
            VisualEffect.ApplyToPlayer(cardLoseVfx, 1.3f);
            Turn.Card.Animate(AnimationType.Attack, true);
        }
        if (Turn.State == GameStateType.Combat)
        {
            if (isResolveSuccess)
            {
                if ((Turn.Card.Type != CardType.Barrier) && Turn.Card.IsBane())
                {
                    VisualEffectType cardWinVfx = Rules.GetCardWinVfx(Turn.DamageTraits);
                    UI.Sound.Play(cardWinVfx.ToSoundtype());
                    VisualEffect.ApplyToCard(cardWinVfx, Turn.Card, 1.3f);
                    Turn.Card.Animate(AnimationType.Damaged, true);
                }
            }
            else if ((Turn.Card.Type != CardType.Barrier) && Turn.Card.IsBane())
            {
                UI.Sound.Play(VisualEffectType.CardLoseEnemy.ToSoundtype());
                VisualEffect.ApplyToPlayer(VisualEffectType.CardLoseEnemy, 1.3f);
                Turn.Card.Animate(AnimationType.Attack, true);
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator ShowRollResult(int total) => 
        new <ShowRollResult>c__Iterator57 { 
            total = total,
            <$>total = total,
            <>f__this = this
        };

    public void ShowSuccessButton(bool isVisible)
    {
        this.RerollButton.Text = StringTableManager.GetHelperText(0x29);
        this.RerollButton.Show(isVisible);
    }

    private GameObject SpawnNumberChangeVfx(int roll)
    {
        GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(this.DiceTotalNumberChangeVfx);
        if (obj2 != null)
        {
            obj2.SetActive(true);
            GuiLabel componentInChildren = obj2.GetComponentInChildren<GuiLabel>();
            if (componentInChildren != null)
            {
                componentInChildren.Text = roll.ToString();
            }
            obj2.SetActive(false);
        }
        return obj2;
    }

    private void SynchronizeAllDice()
    {
        bool flag = false;
        flag |= this.SynchronizeDice(DiceType.D4, this.GetDiceModel(DiceType.D4));
        flag |= this.SynchronizeDice(DiceType.D6, this.GetDiceModel(DiceType.D6));
        flag |= this.SynchronizeDice(DiceType.D8, this.GetDiceModel(DiceType.D8));
        flag |= this.SynchronizeDice(DiceType.D10, this.GetDiceModel(DiceType.D10));
        if ((flag | this.SynchronizeDice(DiceType.D12, this.GetDiceModel(DiceType.D12))) && !this.isNewCheck)
        {
            if ((Turn.RollReason == RollType.PlayerControlled) || (Turn.RollReason == RollType.StandardEnemyDice))
            {
                UI.Sound.Play(SoundEffectType.DiceAdded);
            }
            else
            {
                UI.Sound.Play(SoundEffectType.AutoDiceAdded);
            }
        }
        this.isNewCheck = false;
    }

    private bool SynchronizeDice(DiceType type, GameObject model)
    {
        this.DestroyDiceWithDifferentModel(type, model);
        int num = this.CountTurnDice(type);
        int num2 = this.CountMyDice(type);
        if (num2 < num)
        {
            for (int i = 0; i < (num - num2); i++)
            {
                this.CreateDice(type, model);
            }
            return true;
        }
        if (num2 > num)
        {
            for (int j = 0; j < (num2 - num); j++)
            {
                this.DestroyDice(type);
            }
        }
        return false;
    }

    public Vector3 OffScreen =>
        (this.DiceSpawner.position + new Vector3(4f, -4f, 0f));

    public Vector2 Offset { get; set; }

    public bool Ready
    {
        get
        {
            if (base.Locked)
            {
                return false;
            }
            if (Turn.State == GameStateType.Penalty)
            {
                return false;
            }
            if (Turn.State == GameStateType.Sacrifice)
            {
                return false;
            }
            if (Turn.State == GameStateType.Power)
            {
                return false;
            }
            return ((Turn.State != GameStateType.Reroll) && (Turn.State != GameStateType.RollAgain));
        }
    }

    public bool Rolling =>
        this.isRolling;

    [CompilerGenerated]
    private sealed class <RollDamageDice>c__Iterator55 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Vector2 <$>direction;
        internal GuiPanelDice <>f__this;
        internal int <bonus>__3;
        internal int <diceCount>__0;
        internal int <diceTotal>__1;
        internal int <i>__7;
        internal int <mods>__2;
        internal int <roll>__8;
        internal Dictionary<string, int> <rollData>__5;
        internal List<string> <rolls>__4;
        internal bool <sendAnalytic>__6;
        internal int <total>__9;
        internal Vector2 direction;

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
                    this.<diceCount>__0 = 0;
                    this.<diceTotal>__1 = 0;
                    UI.Window.Pause(true);
                    this.<mods>__2 = 0;
                    this.<bonus>__3 = 0;
                    this.<rolls>__4 = new List<string>();
                    this.<rollData>__5 = new Dictionary<string, int>();
                    this.<sendAnalytic>__6 = false;
                    this.<i>__7 = 0;
                    while (this.<i>__7 < this.<>f__this.diceInstances.Length)
                    {
                        if (this.<>f__this.diceInstances[this.<i>__7] != null)
                        {
                            this.<roll>__8 = Rules.RollDice(this.<>f__this.diceInstances[this.<i>__7].model.diceType);
                            this.<sendAnalytic>__6 = true;
                            this.<>f__this.CalculateDiceAnalytics(this.<roll>__8, this.<>f__this.diceInstances[this.<i>__7].model.diceType, ref this.<rolls>__4, ref this.<rollData>__5);
                            this.<>f__this.diceInstances[this.<i>__7].model.Side = this.<roll>__8;
                            this.<diceTotal>__1 += this.<roll>__8;
                            this.<diceCount>__0++;
                        }
                        this.<i>__7++;
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1.9f));
                    this.$PC = 1;
                    goto Label_02B7;

                case 1:
                    this.<>f__this.RollDice(true, this.direction);
                    this.<>f__this.PlayDiceSound(Turn.Dice.Count);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1.5f));
                    this.$PC = 2;
                    goto Label_02B7;

                case 2:
                    this.<>f__this.RollDice(false, this.direction);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1f));
                    this.$PC = 3;
                    goto Label_02B7;

                case 3:
                    this.<total>__9 = Turn.Roll(this.<diceCount>__0, this.<diceTotal>__1);
                    if (this.<sendAnalytic>__6)
                    {
                        AnalyticsManager.OnDieRoll("damage", this.<rollData>__5, this.<rolls>__4, this.<mods>__2, this.<bonus>__3, this.<total>__9);
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.ShowRollResult(this.<total>__9));
                    this.$PC = 4;
                    goto Label_02B7;

                case 4:
                    UI.Window.Pause(false);
                    this.<>f__this.Resolve();
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_02B7:
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

    [CompilerGenerated]
    private sealed class <RollNormalDice>c__Iterator56 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Vector2 <$>direction;
        internal GuiPanelDice <>f__this;
        internal int <bonus>__5;
        internal int <diceCount>__0;
        internal int <diceTotal>__1;
        internal int <i>__11;
        internal int <i>__13;
        internal int <i>__9;
        internal CardPropertyModifyDice <modifier>__2;
        internal bool <ModifierAffectedRoll>__3;
        internal int <mods>__4;
        internal int <roll>__10;
        internal Dictionary<string, int> <rollData>__7;
        internal List<string> <rolls>__6;
        internal bool <sendAnalytic>__8;
        internal int <total>__12;
        internal Vector2 direction;

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
                    this.<diceCount>__0 = 0;
                    this.<diceTotal>__1 = 0;
                    UI.Window.Pause(true);
                    this.<modifier>__2 = this.<>f__this.GetDiceModifier(Turn.Weapon1);
                    this.<ModifierAffectedRoll>__3 = false;
                    this.<mods>__4 = 0;
                    this.<bonus>__5 = 0;
                    this.<rolls>__6 = new List<string>();
                    this.<rollData>__7 = new Dictionary<string, int>();
                    this.<sendAnalytic>__8 = false;
                    this.<i>__9 = 0;
                    while (this.<i>__9 < this.<>f__this.diceInstances.Length)
                    {
                        if (this.<>f__this.diceInstances[this.<i>__9] != null)
                        {
                            this.<roll>__10 = Rules.RollDice(this.<>f__this.diceInstances[this.<i>__9].model.diceType);
                            this.<sendAnalytic>__8 = true;
                            this.<>f__this.CalculateDiceAnalytics(this.<roll>__10, this.<>f__this.diceInstances[this.<i>__9].model.diceType, ref this.<rolls>__6, ref this.<rollData>__7);
                            this.<>f__this.diceInstances[this.<i>__9].model.Side = this.<roll>__10;
                            if ((this.<modifier>__2 != null) && this.<modifier>__2.Affects(this.<roll>__10, this.<>f__this.diceInstances[this.<i>__9].model.diceType))
                            {
                                this.<mods>__4 += this.<modifier>__2.AdjustedValue - this.<roll>__10;
                                this.<roll>__10 = this.<modifier>__2.AdjustedValue;
                                this.<ModifierAffectedRoll>__3 = true;
                                this.<>f__this.numberChangeVfxs[this.<i>__9] = this.<>f__this.SpawnNumberChangeVfx(this.<roll>__10);
                            }
                            this.<diceTotal>__1 += this.<roll>__10;
                            this.<diceCount>__0++;
                        }
                        this.<i>__9++;
                    }
                    this.<>f__this.RollDice(true, this.direction);
                    this.<>f__this.PlayDiceSound(Turn.Dice.Count);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1.25f));
                    this.$PC = 1;
                    goto Label_050E;

                case 1:
                    this.<>f__this.RollDice(false, this.direction);
                    this.<i>__11 = 0;
                    while (this.<i>__11 < this.<>f__this.numberChangeVfxs.Length)
                    {
                        if (this.<>f__this.numberChangeVfxs[this.<i>__11] != null)
                        {
                            Geometry.SetLayerRecursively(this.<>f__this.numberChangeVfxs[this.<i>__11], Constants.LAYER_POPUP);
                            this.<>f__this.numberChangeVfxs[this.<i>__11].transform.parent = this.<>f__this.diceInstances[this.<i>__11].gameObject.transform.GetChild(0);
                            this.<>f__this.numberChangeVfxs[this.<i>__11].transform.localPosition = Vector3.zero;
                            this.<>f__this.numberChangeVfxs[this.<i>__11].SetActive(true);
                        }
                        this.<i>__11++;
                    }
                    this.<bonus>__5 = this.<diceTotal>__1;
                    this.<total>__12 = Turn.Roll(this.<diceCount>__0, this.<diceTotal>__1);
                    this.<bonus>__5 -= this.<total>__12;
                    if (this.<sendAnalytic>__8)
                    {
                        AnalyticsManager.OnDieRoll("normal", this.<rollData>__7, this.<rolls>__6, this.<mods>__4, this.<bonus>__5, this.<total>__12);
                    }
                    if ((this.<modifier>__2 == null) || !this.<ModifierAffectedRoll>__3)
                    {
                        break;
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1f));
                    this.$PC = 2;
                    goto Label_050E;

                case 2:
                    this.<modifier>__2.ProcessDiscard();
                    UI.Sound.Play(SoundEffectType.NumberChangeVfx);
                    break;

                case 3:
                    this.<i>__13 = 0;
                    while (this.<i>__13 < this.<>f__this.numberChangeVfxs.Length)
                    {
                        if (this.<>f__this.numberChangeVfxs[this.<i>__13] != null)
                        {
                            UnityEngine.Object.Destroy(this.<>f__this.numberChangeVfxs[this.<i>__13]);
                        }
                        this.<i>__13++;
                    }
                    if (Rules.IsRerollPossible(this.<total>__12) && this.<>f__this.ActivateReroll())
                    {
                        UI.Window.Pause(false);
                    }
                    else
                    {
                        UI.Window.Pause(false);
                        this.<>f__this.Resolve();
                        this.$PC = -1;
                    }
                    goto Label_050C;

                default:
                    goto Label_050C;
            }
            this.$current = Game.Instance.StartCoroutine(this.<>f__this.ShowRollResult(this.<total>__12));
            this.$PC = 3;
            goto Label_050E;
        Label_050C:
            return false;
        Label_050E:
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

    [CompilerGenerated]
    private sealed class <RollPowersDice>c__Iterator54 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Vector2 <$>direction;
        internal GuiPanelDice <>f__this;
        internal int <bonus>__3;
        internal int <diceCount>__0;
        internal int <diceTotal>__1;
        internal int <i>__7;
        internal int <mods>__2;
        internal int <roll>__8;
        internal Dictionary<string, int> <rollData>__5;
        internal List<string> <rolls>__4;
        internal bool <sendAnalytic>__6;
        internal int <total>__9;
        internal Vector2 direction;

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
                    this.<diceCount>__0 = 0;
                    this.<diceTotal>__1 = 0;
                    UI.Window.Pause(true);
                    this.<mods>__2 = 0;
                    this.<bonus>__3 = 0;
                    this.<rolls>__4 = new List<string>();
                    this.<rollData>__5 = new Dictionary<string, int>();
                    this.<sendAnalytic>__6 = false;
                    this.<i>__7 = 0;
                    while (this.<i>__7 < this.<>f__this.diceInstances.Length)
                    {
                        if (this.<>f__this.diceInstances[this.<i>__7] != null)
                        {
                            this.<roll>__8 = Rules.RollDice(this.<>f__this.diceInstances[this.<i>__7].model.diceType);
                            this.<sendAnalytic>__6 = true;
                            this.<>f__this.CalculateDiceAnalytics(this.<roll>__8, this.<>f__this.diceInstances[this.<i>__7].model.diceType, ref this.<rolls>__4, ref this.<rollData>__5);
                            this.<>f__this.diceInstances[this.<i>__7].model.Side = this.<roll>__8;
                            this.<diceTotal>__1 += this.<roll>__8;
                            this.<diceCount>__0++;
                        }
                        this.<i>__7++;
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1.9f));
                    this.$PC = 1;
                    goto Label_02B7;

                case 1:
                    this.<>f__this.RollDice(true, this.direction);
                    this.<>f__this.PlayDiceSound(Turn.Dice.Count);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(2.6f));
                    this.$PC = 2;
                    goto Label_02B7;

                case 2:
                    this.<>f__this.RollDice(false, this.direction);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(3f));
                    this.$PC = 3;
                    goto Label_02B7;

                case 3:
                    this.<total>__9 = Turn.Roll(this.<diceCount>__0, this.<diceTotal>__1);
                    if (this.<sendAnalytic>__6)
                    {
                        AnalyticsManager.OnDieRoll("powers", this.<rollData>__5, this.<rolls>__4, this.<mods>__2, this.<bonus>__3, this.<total>__9);
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.ShowRollResult(this.<total>__9));
                    this.$PC = 4;
                    goto Label_02B7;

                case 4:
                    UI.Window.Pause(false);
                    this.<>f__this.Resolve();
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_02B7:
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

    [CompilerGenerated]
    private sealed class <ShowRollResult>c__Iterator57 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal int <$>total;
        internal GuiPanelDice <>f__this;
        internal GameObject <diceTotalVfx>__1;
        internal bool <isResolveSuccess>__0;
        internal int total;

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
                    this.<isResolveSuccess>__0 = this.total >= Turn.DiceTarget;
                    if (this.<isResolveSuccess>__0)
                    {
                        this.<diceTotalVfx>__1 = this.<>f__this.DiceTotalWinVfx;
                        this.<>f__this.DiceTotalLabel.Color = this.<>f__this.DiceWinColor;
                        break;
                    }
                    this.<diceTotalVfx>__1 = this.<>f__this.DiceTotalLossVfx;
                    this.<>f__this.DiceTotalLabel.Color = this.<>f__this.DiceLoseClor;
                    break;

                case 1:
                    LeanTween.scale(this.<>f__this.DiceTotalLabel.gameObject, new Vector3(0.1f, 0.1f, 0.1f), 0.2f).setEase(LeanTweenType.easeInOutQuad);
                    this.<>f__this.DiceTotalLabel.Clear();
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.2f));
                    this.$PC = 2;
                    goto Label_01F1;

                case 2:
                    this.<diceTotalVfx>__1.SetActive(false);
                    this.$PC = -1;
                    goto Label_01EF;

                default:
                    goto Label_01EF;
            }
            this.<>f__this.ShowResolveResult(this.<isResolveSuccess>__0);
            this.<>f__this.DiceTotalLabel.Text = this.total.ToString();
            Geometry.SetLayerRecursively(this.<>f__this.DiceTotalLabel.gameObject, Constants.LAYER_POPUP);
            this.<diceTotalVfx>__1.SetActive(true);
            Geometry.SetLayerRecursively(this.<diceTotalVfx>__1.gameObject, Constants.LAYER_POPUP);
            LeanTween.scale(this.<>f__this.DiceTotalLabel.gameObject, new Vector3(0.15f, 0.15f, 0.15f), 1f).setEase(LeanTweenType.easeInOutQuad);
            this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(2f));
            this.$PC = 1;
            goto Label_01F1;
        Label_01EF:
            return false;
        Label_01F1:
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

    private class DiceInstance
    {
        public GameObject gameObject;
        public DiceModel model;
        public DiceType type;

        public DiceInstance(GameObject go, DiceType type)
        {
            this.type = type;
            this.gameObject = go;
            this.model = go.GetComponent<DiceModel>();
        }
    }
}

