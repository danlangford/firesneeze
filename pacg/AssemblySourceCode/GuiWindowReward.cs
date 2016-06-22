using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiWindowReward : GuiWindow
{
    [Tooltip("text field containing the card count")]
    public GuiLabel CardCount;
    [Tooltip("reference to the 7 card stacks in our hierarchy. 1 for each player (6-player game), 1 for communism (everybody's)")]
    public GuiLayout[] cardLayouts;
    private Reward currentReward;
    [Tooltip("animation to play before showing the screen")]
    public GameObject IntroAnimation;
    [Tooltip("length of intro animation in seconds")]
    public float IntroAnimationLength = 2f;
    private int[] numRewardsGiven;
    [Tooltip("reference to the gold panel in this scene")]
    public GuiPanelRewardGold panelGold;
    [Tooltip("reference to the levelup panel in this scene (used during quests)\t")]
    public GuiPanelLevelup panelLevelup;
    [Tooltip("reference to the \"message panel\" in our hierarchy")]
    public GuiPanelMessage panelMessage;
    [Tooltip("reference to the party panel in this scene")]
    public GuiPanelPartyLine panelParty;
    [Tooltip("reference to the proceed button in our hierarchy")]
    public GuiButton proceedButton;
    [Tooltip("reference to the disabled proceed button in our hierarchy")]
    public GuiButton proceedDisabledButton;
    private int rewardLevel;
    private int rewardStep;
    private CardType[] selectedCardFeat;
    private string[] selectedPowerFeat;
    private AttributeType[] selectedSkillFeat;

    private Reward AdvanceToNextReward()
    {
        Reward nextReward = this.GetNextReward();
        if (nextReward != null)
        {
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                Party.Characters[i].Active = !this.IsRewardOutstanding(i) ? ActiveType.Inactive : ActiveType.Active;
            }
            this.panelParty.Refresh();
        }
        return nextReward;
    }

    private void Clear()
    {
        if (this.currentReward != null)
        {
            this.currentReward.Show(false);
        }
        for (int i = 0; i < this.numRewardsGiven.Length; i++)
        {
            this.numRewardsGiven[i] = 0;
        }
    }

    private void DeliverAllRewards()
    {
        if (!Rules.IsQuestRewardAllowed())
        {
            for (int j = 0; j < Party.Characters.Count; j++)
            {
                this.rewardLevel = 1;
                if (this.IsRewardEligible(j))
                {
                    Party.Characters[j].AddReward(Scenario.Current.Reward);
                }
                this.rewardLevel = 2;
                if (this.IsRewardEligible(j))
                {
                    Party.Characters[j].AddReward(Adventure.Current.Reward);
                }
                this.rewardLevel = 3;
                if (this.IsRewardEligible(j))
                {
                    Party.Characters[j].AddReward(AdventurePath.Current.Reward);
                }
            }
        }
        Turn.Number = 0;
        Game.Network.OnScenarioComplete(Scenario.Current);
        if (!Rules.IsQuestRewardAllowed())
        {
            Campaign.SetRewarded(Scenario.Current);
        }
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Vault.Contains(Party.Characters[i].NickName))
            {
                Vault.Add(Party.Characters[i].NickName, Party.Characters[i]);
            }
        }
    }

    private void DeliverRewardCards()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            for (int j = this.cardLayouts[i].Deck.Count - 1; j >= 0; j--)
            {
                Card card = this.cardLayouts[i].Deck[j];
                Party.Characters[i].Deck.Add(card);
                card.Show(true);
                LeanTween.scale(card.gameObject, new Vector3(0f, 0f, 1f), 0.2f);
            }
        }
    }

    private bool GetAllRewardsGiven()
    {
        if (this.currentReward == null)
        {
            return true;
        }
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (this.IsRewardOutstanding(i))
            {
                return false;
            }
        }
        return this.currentReward.GetAllRewardsGiven();
    }

    private Reward GetNextReward()
    {
        this.Clear();
        if (Rules.IsQuestRewardAllowed())
        {
            return this.panelLevelup.GetNextReward();
        }
        if (this.rewardLevel == 0)
        {
            this.rewardLevel = 1;
            this.currentReward = Scenario.Current.Reward;
            if (!this.GetAllRewardsGiven())
            {
                this.panelMessage.Show(this.currentReward.Message.ToString());
                this.currentReward.Initialize(this);
                return this.currentReward;
            }
        }
        if (this.rewardLevel == 1)
        {
            this.rewardLevel = 2;
            this.currentReward = Adventure.Current.Reward;
            if (!this.GetAllRewardsGiven())
            {
                this.panelMessage.Show(this.currentReward.Message.ToString());
                this.currentReward.Initialize(this);
                return this.currentReward;
            }
        }
        if (this.rewardLevel == 2)
        {
            this.rewardLevel = 3;
            this.currentReward = AdventurePath.Current.Reward;
            if (!this.GetAllRewardsGiven())
            {
                this.currentReward.Initialize(this);
                this.panelMessage.Show(this.currentReward.Message.ToString());
                return this.currentReward;
            }
        }
        return null;
    }

    private bool IsRewardEligible(int n)
    {
        if (Rules.IsQuestRewardAllowed())
        {
            return this.panelLevelup.IsRewardEligible(n);
        }
        if (((this.currentReward != null) && !this.currentReward.Player) && !Party.Characters[n].Alive)
        {
            return false;
        }
        if ((this.currentReward == null) && !Party.Characters[n].Alive)
        {
            return false;
        }
        if (!this.IsRewardRequirementsMet(n))
        {
            return false;
        }
        if (Party.Characters[n].HasReward(this.currentReward))
        {
            return false;
        }
        return true;
    }

    private bool IsRewardOutstanding(int n)
    {
        if (Rules.IsQuestRewardAllowed())
        {
            return this.panelLevelup.IsRewardOutstanding(n);
        }
        if (this.currentReward == null)
        {
            return false;
        }
        if (!this.IsRewardEligible(n))
        {
            return false;
        }
        if (this.currentReward.IsSelected(n))
        {
            return false;
        }
        if (this.currentReward.HasReward(n))
        {
            return false;
        }
        return true;
    }

    private bool IsRewardRequirementsMet(int n)
    {
        bool flag = true;
        switch (this.rewardLevel)
        {
            case 2:
                for (int i = 0; i < Adventure.Current.Scenarios.Length; i++)
                {
                    string id = Adventure.Current.Scenarios[i];
                    if (!Party.Characters[n].HasCompleted(id) && (Scenario.Current.ID != id))
                    {
                        flag = false;
                    }
                }
                return flag;

            case 3:
                for (int j = 0; j < AdventurePath.Current.Adventures.Length; j++)
                {
                    string str2 = AdventurePath.Current.Adventures[j];
                    if (!Party.Characters[n].HasCompleted(str2) && (Adventure.Current.ID != str2))
                    {
                        flag = false;
                    }
                }
                return flag;
        }
        return flag;
    }

    private void OnCharacterSwitch()
    {
        if (Game.GameType == GameType.LocalMultiPlayer)
        {
            Game.UI.SwitchPanel.Show(true);
        }
        else
        {
            this.OnCharacterSwitchDone();
        }
    }

    private void OnCharacterSwitchDone()
    {
        this.UpdatePortraits();
        if (this.currentReward != null)
        {
            if (this.IsRewardEligible(Turn.Number) && !this.currentReward.HasReward(Turn.Number))
            {
                this.currentReward.SetReward(this.numRewardsGiven[Turn.Number]);
                this.currentReward.Show(true);
            }
            else
            {
                this.currentReward.Show(false);
            }
        }
    }

    public void OnCommunismChosen(Card card)
    {
        if (card != null)
        {
            this.cardLayouts[this.cardLayouts.Length - 1].Deck.Add(card);
            card.Show(true);
            this.cardLayouts[this.cardLayouts.Length - 1].Refresh();
            this.Refresh();
        }
    }

    private void OnMenuButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            Game.UI.OptionsPanel.Show(true);
        }
    }

    private void OnProceedButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            UI.Busy = true;
            LeanTween.scale(this.proceedButton.gameObject, new Vector3(1.1f, 1.1f, 1f), 0.1f).setLoopPingPong().setLoopCount(2).setOnComplete(new Action(this.ProceedButtonPushed));
        }
    }

    private void OnProceedDisabledButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            UI.Busy = true;
            LeanTween.scale(this.proceedDisabledButton.gameObject, new Vector3(1.1f, 1.1f, 1f), 0.1f).setLoopPingPong().setLoopCount(2).setOnComplete(new Action(this.ProceedDisabledButtonPushed));
        }
    }

    public void OnRewardChosen(AttributeType attribute)
    {
        this.numRewardsGiven[Turn.Number]++;
        this.currentReward.Select(attribute);
        this.Refresh();
    }

    public void OnRewardChosen(Card card)
    {
        if (card != null)
        {
            this.numRewardsGiven[Turn.Number]++;
            this.cardLayouts[Turn.Number].Deck.Add(card);
            card.Show(true);
            this.cardLayouts[Turn.Number].Refresh();
            this.currentReward.Select(card);
            UI.Sound.Play(SoundEffectType.RewardCardSlideDownToTray);
            this.Refresh();
        }
    }

    public void OnRewardChosen(CardType card)
    {
        this.numRewardsGiven[Turn.Number]++;
        this.currentReward.Select(card);
        this.Refresh();
    }

    public void OnRewardChosen(ProficencyType type)
    {
        this.numRewardsGiven[Turn.Number]++;
        this.currentReward.Select(type);
        this.Refresh();
    }

    public void OnRewardChosen(RoleTableEntry role)
    {
        this.currentReward.Select(role);
        this.Refresh();
    }

    public void OnRewardChosen(int handsize)
    {
        this.numRewardsGiven[Turn.Number]++;
        this.currentReward.Select(1);
        this.Refresh();
    }

    public void OnRewardChosen(string id)
    {
        this.numRewardsGiven[Turn.Number]++;
        this.currentReward.Select(id);
        this.Refresh();
    }

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        this.panelParty.Pause(isPaused);
    }

    private void ProceedButtonPushed()
    {
        UI.Busy = false;
        this.DeliverRewardCards();
        if (this.currentReward != null)
        {
            this.currentReward.Deliver();
        }
        this.currentReward = this.AdvanceToNextReward();
        if (this.currentReward == null)
        {
            this.DeliverAllRewards();
            Scenario.Current.Exit();
        }
        else
        {
            this.proceedButton.Show(false);
            this.proceedDisabledButton.Show(true);
            float initializationTime = this.currentReward.GetInitializationTime();
            this.panelParty.SuspendHighlight(initializationTime);
            LeanTween.delayedCall(initializationTime, new Action(this.OnCharacterSwitch));
        }
    }

    private void ProceedDisabledButtonPushed()
    {
        UI.Busy = false;
    }

    public override void Refresh()
    {
        this.panelParty.Refresh();
        this.UpdateProceedButton();
        this.UpdatePortraits();
    }

    private void RewardSequenceController()
    {
        base.StartCoroutine(this.StartRewardSequenceCoroutine());
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.panelParty.Show(isVisible);
        if (this.currentReward != null)
        {
            this.currentReward.SetReward(this.numRewardsGiven[Turn.Number]);
            this.currentReward.Show(isVisible);
        }
    }

    private void ShowCardLayouts(bool isVisible)
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Vector3 characterPosition = this.panelParty.GetCharacterPosition(Party.Characters[i].ID);
            this.cardLayouts[i].transform.position = new Vector3(characterPosition.x, this.panelParty.transform.position.y - 0.1f, characterPosition.z);
            this.cardLayouts[i].Show(true);
        }
        this.cardLayouts[this.cardLayouts.Length - 1].transform.position = new Vector3(0f, this.panelParty.transform.position.y - 0.1f, 0f);
    }

    [DebuggerHidden]
    private IEnumerator ShowIntroAnimation(float length) => 
        new <ShowIntroAnimation>c__Iterator8D { 
            length = length,
            <$>length = length,
            <>f__this = this
        };

    protected override void Start()
    {
        base.Start();
        Game.Save();
        this.panelParty.Initialize();
        this.panelLevelup.Initialize();
        this.rewardStep = 0;
        this.rewardLevel = 0;
        this.numRewardsGiven = new int[Party.Characters.Count];
        this.proceedButton.Show(false);
        this.proceedDisabledButton.Show(true);
        if (this.IntroAnimation != null)
        {
            base.StartCoroutine(this.ShowIntroAnimation(this.IntroAnimationLength));
        }
    }

    [DebuggerHidden]
    private IEnumerator StartRewardSequenceCoroutine() => 
        new <StartRewardSequenceCoroutine>c__Iterator8E { <>f__this = this };

    private void SwitchToFirstValidPartyMember()
    {
        int num = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (this.IsRewardEligible(i))
            {
                num = i;
                break;
            }
        }
        Turn.Current = num;
        Turn.Number = num;
        this.OnCharacterSwitch();
    }

    private void UpdatePortraits()
    {
        this.CardCount.Text = Turn.Character.Deck.Count.ToString();
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            this.panelParty.SetCharacterMarker(Party.Characters[i].ID, this.IsRewardOutstanding(i));
        }
    }

    private void UpdateProceedButton()
    {
        bool isVisible = (this.rewardStep > 3) && this.GetAllRewardsGiven();
        this.proceedButton.Show(isVisible);
        this.proceedDisabledButton.Show(!isVisible);
    }

    public Reward Reward =>
        this.currentReward;

    public override WindowType Type =>
        WindowType.Reward;

    [CompilerGenerated]
    private sealed class <ShowIntroAnimation>c__Iterator8D : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal float <$>length;
        internal GuiWindowReward <>f__this;
        internal float length;

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
                    UI.Busy = true;
                    this.<>f__this.IntroAnimation.SetActive(true);
                    this.$current = new WaitForSeconds(this.length);
                    this.$PC = 1;
                    return true;

                case 1:
                    UI.Busy = false;
                    this.<>f__this.RewardSequenceController();
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

    [CompilerGenerated]
    private sealed class <StartRewardSequenceCoroutine>c__Iterator8E : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiWindowReward <>f__this;
        internal Prize <prize>__0;
        internal Prize <prize>__1;
        internal float <time>__2;

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
                    if (this.<>f__this.rewardStep != 0)
                    {
                        break;
                    }
                    this.<>f__this.rewardStep++;
                    if ((!Game.Network.Connected || Game.Network.OutOfDate) || !Game.Network.HasNetworkConnection)
                    {
                        break;
                    }
                    this.<>f__this.panelGold.Show(true);
                    goto Label_0278;

                case 1:
                    this.<>f__this.ShowCardLayouts(true);
                    this.<>f__this.SwitchToFirstValidPartyMember();
                    this.$current = new WaitForSeconds(this.<>f__this.currentReward.GetShowTime());
                    this.$PC = 2;
                    goto Label_027A;

                case 2:
                    Tutorial.Notify(TutorialEventType.ScreenRewardShown);
                    this.<>f__this.Refresh();
                    goto Label_0271;

                default:
                    goto Label_0278;
            }
            if (this.<>f__this.rewardStep == 1)
            {
                this.<>f__this.rewardStep++;
                if (Rules.IsQuestRewardAllowed())
                {
                    this.<>f__this.panelLevelup.Show(true);
                    goto Label_0278;
                }
            }
            if (this.<>f__this.rewardStep == 2)
            {
                this.<>f__this.rewardStep++;
                this.<prize>__0 = Scenario.Current.GetComponent<Prize>();
                if ((this.<prize>__0 != null) && this.<prize>__0.IsPrizeAllowed())
                {
                    this.<prize>__0.Deliver();
                    goto Label_0278;
                }
            }
            if (this.<>f__this.rewardStep == 3)
            {
                this.<>f__this.rewardStep++;
                this.<prize>__1 = Adventure.Current.GetComponent<Prize>();
                if ((this.<prize>__1 != null) && this.<prize>__1.IsPrizeAllowed())
                {
                    this.<prize>__1.Deliver();
                    goto Label_0278;
                }
            }
            this.<>f__this.currentReward = this.<>f__this.AdvanceToNextReward();
            if (this.<>f__this.currentReward != null)
            {
                this.<time>__2 = this.<>f__this.currentReward.GetInitializationTime();
                this.<>f__this.panelParty.SuspendHighlight(this.<time>__2);
                this.$current = new WaitForSeconds(this.<time>__2);
                this.$PC = 1;
                goto Label_027A;
            }
            this.<>f__this.ProceedButtonPushed();
        Label_0271:
            this.$PC = -1;
        Label_0278:
            return false;
        Label_027A:
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

