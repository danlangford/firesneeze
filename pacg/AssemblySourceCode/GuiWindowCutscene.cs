using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiWindowCutscene : GuiWindow
{
    [Tooltip("reference to the single \"player reply\" button in our hiearchy")]
    public GuiButton continueButton;
    private readonly float FadeDelay = 0.2f;
    private CutsceneActor lastSpeaker;
    [Tooltip("reference to the location closed overlay in this scene")]
    public GameObject locationClosedOverlay;
    [Tooltip("reference to the alert panel in this scene")]
    public GuiPanelCutsceneAlert panelAlert;
    [Tooltip("reference to the intertitle panel in this scene")]
    public GuiPanelIntertitle panelIntertitle;
    private Cutscene scene;
    [Tooltip("reference to the skip button in our hierarchy")]
    public GuiButton skipButton;
    [Tooltip("reference to the speaker name label in our hierarchy")]
    public GuiLabel speakerName;
    [Tooltip("reference to the speaker name background art")]
    public GameObject speakerNameArt;
    [Tooltip("reference to the speaker text label in our hierarchy")]
    public GuiLabel speakerText;
    private readonly float SwitchTransitionDelay = 0.05f;

    protected override void Awake()
    {
        base.Awake();
        this.scene = Cutscene.Create(Scenario.Current);
        if (this.scene == null)
        {
            base.gameObject.SetActive(false);
            if (Scenario.Current.Complete)
            {
                if (Scenario.Current.Rewardable)
                {
                    Game.UI.ShowRewardScene();
                }
                else
                {
                    Scenario.Current.Exit();
                }
            }
            else
            {
                Game.UI.ShowSetupScene();
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator End() => 
        new <End>c__Iterator88 { <>f__this = this };

    private CutsceneActor GetActor(string tag)
    {
        GameObject obj2 = GameObject.Find("Actors/" + tag);
        if (obj2 != null)
        {
            return obj2.GetComponent<CutsceneActor>();
        }
        return null;
    }

    private void OnContinueButtonPushed()
    {
        if (((this.scene != null) && !UI.Busy) && !UI.Window.Paused)
        {
            if (this.scene.Conversation.Continue())
            {
                this.Refresh();
            }
            else
            {
                base.StartCoroutine(this.End());
            }
        }
    }

    private void OnMenuButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            Game.UI.OptionsPanel.Show(true);
        }
    }

    private void OnSkipButtonPushed()
    {
        if (((this.scene != null) && !UI.Busy) && !UI.Window.Paused)
        {
            base.StartCoroutine(this.End());
        }
    }

    public void Play()
    {
        this.skipButton.Fade(true, this.FadeDelay);
        this.continueButton.Fade(true, this.FadeDelay);
        this.OnContinueButtonPushed();
    }

    public override void Refresh()
    {
        base.StartCoroutine(this.SpeakNextLine());
    }

    [DebuggerHidden]
    private IEnumerator SpeakNextLine() => 
        new <SpeakNextLine>c__Iterator87 { <>f__this = this };

    protected override void Start()
    {
        base.Start();
        this.scene.Play();
    }

    public Cutscene Scene =>
        this.scene;

    public override WindowType Type =>
        WindowType.Cutscene;

    [CompilerGenerated]
    private sealed class <End>c__Iterator88 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiWindowCutscene <>f__this;

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
                    this.<>f__this.speakerName.Fade(false, this.<>f__this.FadeDelay);
                    this.<>f__this.speakerText.Fade(false, this.<>f__this.FadeDelay);
                    this.<>f__this.continueButton.Fade(false, this.<>f__this.FadeDelay);
                    this.<>f__this.skipButton.Fade(false, this.<>f__this.FadeDelay);
                    if (this.<>f__this.lastSpeaker != null)
                    {
                        this.<>f__this.lastSpeaker.Show(ActorPositionType.None);
                    }
                    this.$current = new WaitForSeconds(this.<>f__this.FadeDelay + 0.05f);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.scene.Stop();
                    UI.Busy = false;
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
    private sealed class <SpeakNextLine>c__Iterator87 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiWindowCutscene <>f__this;
        internal CutsceneActor <newSpeaker>__1;
        internal string <speakerTag>__0;
        internal ActorPositionType <talkPosition>__2;

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
                    this.<speakerTag>__0 = this.<>f__this.scene.Conversation.GetSpeakerTag();
                    this.<newSpeaker>__1 = this.<>f__this.GetActor(this.<speakerTag>__0);
                    UI.Busy = true;
                    this.<>f__this.speakerText.Fade(false, this.<>f__this.FadeDelay);
                    if (this.<>f__this.lastSpeaker != this.<newSpeaker>__1)
                    {
                        this.<>f__this.speakerName.Fade(false, this.<>f__this.FadeDelay);
                    }
                    if ((this.<>f__this.lastSpeaker == null) || ((this.<newSpeaker>__1 != null) && (this.<>f__this.lastSpeaker.ID == this.<newSpeaker>__1.ID)))
                    {
                        break;
                    }
                    this.<>f__this.lastSpeaker.Show(ActorPositionType.None);
                    this.$current = new WaitForSeconds(CutsceneActor.AnimationDuration);
                    this.$PC = 1;
                    goto Label_02C5;

                case 1:
                    this.<>f__this.lastSpeaker.Mood = ActorMoodType.Neutral;
                    this.$current = new WaitForSeconds(this.<>f__this.SwitchTransitionDelay);
                    this.$PC = 2;
                    goto Label_02C5;

                case 2:
                    break;

                case 3:
                    goto Label_01AF;

                default:
                    goto Label_02C3;
            }
            if (this.<newSpeaker>__1 != null)
            {
                this.<talkPosition>__2 = this.<newSpeaker>__1.GetTalkPosition();
                this.<newSpeaker>__1.Show(this.<talkPosition>__2);
                this.$current = new WaitForSeconds(CutsceneActor.AnimationDuration);
                this.$PC = 3;
                goto Label_02C5;
            }
        Label_01AF:
            this.<>f__this.speakerText.Text = this.<>f__this.scene.Conversation.GetSpeakerText();
            this.<>f__this.speakerText.Fade(true, this.<>f__this.FadeDelay);
            if (this.<newSpeaker>__1 != null)
            {
                if (this.<>f__this.lastSpeaker != this.<newSpeaker>__1)
                {
                    this.<>f__this.speakerName.Text = this.<newSpeaker>__1.DisplayName;
                    this.<>f__this.speakerName.Fade(true, this.<>f__this.FadeDelay);
                }
            }
            else
            {
                this.<>f__this.speakerName.Text = string.Empty;
                this.<>f__this.speakerName.Fade(true, this.<>f__this.FadeDelay);
            }
            this.<>f__this.speakerNameArt.SetActive(this.<newSpeaker>__1 != null);
            this.<>f__this.lastSpeaker = this.<newSpeaker>__1;
            UI.Busy = false;
            this.$PC = -1;
        Label_02C3:
            return false;
        Label_02C5:
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

