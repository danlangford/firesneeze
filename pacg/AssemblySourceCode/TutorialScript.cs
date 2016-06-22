using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    [Tooltip("id of the adventure to use during this tutorial")]
    public string Adventure;
    [Tooltip("id of the adventure path to use during this tutorial")]
    public string AdventurePath;
    [Tooltip("ids of party members involved in this scenario")]
    public string[] Characters;
    private TutorialMessage displayedMessage;
    private bool isMessagePending;
    [Tooltip("ordered list of tutorial messages to be displayed")]
    public TutorialMessage[] Messages;
    [Tooltip("ordered list of tutorial messages always displayed")]
    public TutorialMessage[] Overrides;
    [Tooltip("id of the scenario to use during this tutorial")]
    public string Scenario;
    [Tooltip("id of starting location in the scenario")]
    public string StartLocation;
    [Tooltip("ordered list of general tips to be displayed")]
    public TutorialMessage[] Tips;

    private void AddCharacterToParty(string id)
    {
        Character member = CharacterTable.Create(id);
        member.Location = this.StartLocation;
        Party.Add(member);
    }

    public void Clear()
    {
        this.isMessagePending = false;
        this.displayedMessage = null;
        this.Step = 0;
        for (int i = 0; i < this.Messages.Length; i++)
        {
            this.Messages[i].Clear();
        }
        for (int j = 0; j < this.Overrides.Length; j++)
        {
            this.Overrides[j].Clear();
        }
        for (int k = 0; k < this.Tips.Length; k++)
        {
            this.Tips[k].Clear();
        }
    }

    private bool IsMessageHidden(TutorialEventType eventType) => 
        ((eventType == TutorialEventType.LocationExplored) || ((eventType == TutorialEventType.DiceRolled) || ((eventType == TutorialEventType.TurnProceeded) || ((eventType == TutorialEventType.ScreenWasClosed) || ((eventType == TutorialEventType.ScreenSwitchShown) || ((eventType == TutorialEventType.ScreenRulesShown) || (((this.displayedMessage != null) && (eventType == TutorialEventType.TurnCharacterChanged)) && (this.displayedMessage.id == 30))))))));

    private bool IsMessageSuppressed(TutorialEventType eventType)
    {
        if (Turn.IsSwitchingCharacters())
        {
            return true;
        }
        if (this.displayedMessage != null)
        {
            if ((eventType == TutorialEventType.TurnResolved) && (this.displayedMessage.Trigger == TutorialEventType.CardDefeated))
            {
                return true;
            }
            if ((eventType == TutorialEventType.TurnResolved) && (this.displayedMessage.Trigger == TutorialEventType.CardUndefeated))
            {
                return true;
            }
        }
        return false;
    }

    public void Notify(TutorialEventType eventType)
    {
        if (!this.IsMessageSuppressed(eventType))
        {
            if (this.IsMessageHidden(eventType))
            {
                Tutorial.Hide();
                this.displayedMessage = null;
            }
            if (!this.isMessagePending)
            {
                if (Tutorial.Running)
                {
                    if (this.Step >= this.Messages.Length)
                    {
                        return;
                    }
                    if ((this.Messages[this.Step].Trigger != eventType) && this.Messages[this.Step].Optional)
                    {
                        for (int k = this.Step; k < this.Messages.Length; k++)
                        {
                            if (this.Messages[k].Trigger == eventType)
                            {
                                this.Step = k;
                                break;
                            }
                            if (!this.Messages[k].Optional)
                            {
                                break;
                            }
                        }
                    }
                    if ((this.Messages[this.Step].Trigger == eventType) && this.Messages[this.Step].IsConditionValid())
                    {
                        int num4;
                        base.StartCoroutine(this.ShowMessage(this.Messages[this.Step]));
                        this.Step = (num4 = this.Step) + 1;
                        if (this.Messages[num4].id > 0)
                        {
                            return;
                        }
                    }
                    for (int j = 0; j < this.Overrides.Length; j++)
                    {
                        if ((this.Overrides[j].Trigger == eventType) && this.Overrides[j].IsConditionValid())
                        {
                            this.Overrides[j].Repeat = true;
                            base.StartCoroutine(this.ShowMessage(this.Overrides[j]));
                            return;
                        }
                    }
                }
                for (int i = 0; i < this.Tips.Length; i++)
                {
                    if ((this.Tips[i].Trigger == eventType) && this.Tips[i].IsDisplayPossible())
                    {
                        base.StartCoroutine(this.ShowMessage(this.Tips[i]));
                        break;
                    }
                }
            }
        }
    }

    public void Run(int slot)
    {
        this.Clear();
        Party.Clear();
        for (int i = 0; i < this.Characters.Length; i++)
        {
            this.AddCharacterToParty(this.Characters[i]);
        }
        for (int j = 0; j < Party.Characters.Count; j++)
        {
            Party.Characters[j].BuildDeck();
        }
        AdventurePath.Current = AdventurePathTable.Create(this.AdventurePath);
        Adventure.Current = AdventureTable.Create(this.Adventure);
        Scenario.Current = ScenarioTable.Create(this.Scenario);
        Game.Play(GameType.LocalSinglePlayer, slot, WindowType.Cutscene, Scenario.Current.StartLocation, false);
        Scenario.Current.Initialize();
    }

    [DebuggerHidden]
    private IEnumerator ShowMessage(TutorialMessage message) => 
        new <ShowMessage>c__IteratorAA { 
            message = message,
            <$>message = message,
            <>f__this = this
        };

    public int Step { get; set; }

    [CompilerGenerated]
    private sealed class <ShowMessage>c__IteratorAA : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal TutorialMessage <$>message;
        internal TutorialScript <>f__this;
        internal TutorialMessage message;

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
                    this.<>f__this.isMessagePending = true;
                    if (this.message.Delay <= 0f)
                    {
                        break;
                    }
                    Tutorial.Hide();
                    this.$current = new WaitForSeconds(this.message.Delay);
                    this.$PC = 1;
                    return true;

                case 1:
                    break;

                default:
                    goto Label_00A3;
            }
            this.message.Show();
            this.message.Invoke();
            this.<>f__this.displayedMessage = this.message;
            this.<>f__this.isMessagePending = false;
            this.$PC = -1;
        Label_00A3:
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

