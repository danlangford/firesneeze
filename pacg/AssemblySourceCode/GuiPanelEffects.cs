using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelEffects : GuiPanel
{
    [Tooltip("reference to the animator for this panel")]
    public UnityEngine.Animator Animator;
    private const float BUTTONHEIGHT = 0.875f;
    [Tooltip("references to the effect buttons on this panel")]
    public GuiButton[] EffectButtons;
    [Tooltip("reference to the flash vfx in this panel")]
    public GameObject FlashVfx;
    private bool isPanelBusy;
    private bool isPanelFlashing;
    private bool isPanelOpen;
    [Tooltip("reference to the art on this panel that indicates no effects")]
    public GameObject NoEffectsMessage;
    private TKTapRecognizer tapRecognizer;
    [Tooltip("reference to the popup-tooltip on this panel")]
    public GuiPanelTooltip Tooltip;
    private List<Card> zoomedCard = new List<Card>(2);

    [DebuggerHidden]
    private IEnumerator ClosePanelCoroutine() => 
        new <ClosePanelCoroutine>c__Iterator5A { <>f__this = this };

    private void DisplayCardDetails(string id, int column, int maxColumn, float sizeMultiplier)
    {
        UI.Window.Pause(true);
        Card item = CardTable.Create(id);
        if (item != null)
        {
            item.transform.position = base.transform.position;
            Vector3 vector = (Vector3) (item.Size * sizeMultiplier);
            Vector3 zero = Vector3.zero;
            if (maxColumn >= 2)
            {
                zero = new Vector3(0.1f * vector.x, 0.1f * vector.y, 0f);
            }
            item.transform.localScale = new Vector3(0f, 0f, 1f);
            item.Show(CardSideType.Front);
            item.OnGuiZoom(true, (Vector3) ((zero + (((Vector3.left * column) * vector.x) * 0.12f)) + (((Vector3.down * column) * vector.y) * 0.12f)), sizeMultiplier);
            item.SortingOrder += column;
            this.zoomedCard.Add(item);
        }
        this.ListenForTapsAfter(0.5f);
    }

    [DebuggerHidden]
    private IEnumerator DisplayCardDetailsClose() => 
        new <DisplayCardDetailsClose>c__Iterator5B { <>f__this = this };

    private void DisplayCharacterDetails(string id)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            char[] separator = new char[] { '/' };
            string[] strArray = id.Split(separator);
            if (strArray.Length >= 2)
            {
                Character c = Party.Find(strArray[0]);
                if (c != null)
                {
                    this.Show(false);
                    window.Pause(true);
                    window.Show(false);
                    window.characterPanel.Show(true);
                    window.characterPanel.Refresh(c);
                    window.characterPanel.SelectPower(strArray[1]);
                }
            }
        }
    }

    private void DisplayEffectDetails(int n)
    {
        Effect e = this.GetEffect(n);
        if ((e != null) && e.ShowSources)
        {
            if (this.IsCardEffect(e))
            {
                float sizeMultiplier = Mathf.Max((float) (1f - (0.1f * e.sources.Count)), (float) 0.7f);
                for (int i = 0; i < e.sources.Count; i++)
                {
                    this.DisplayCardDetails(e.sources[i], i, e.sources.Count, sizeMultiplier);
                }
            }
            else if (IsCharacterEffect(e))
            {
                this.DisplayCharacterDetails(e.source);
            }
            else if (this.IsScenarioEffect(e))
            {
                this.DisplayScenarioDetails(e.source);
            }
        }
    }

    private void DisplayScenarioDetails(string id)
    {
        UI.Window.Pause(true);
        for (int i = 0; i < Scenario.Current.Powers.Count; i++)
        {
            if (Scenario.Current.Powers[i].ID == id)
            {
                this.Tooltip.Text = Scenario.Current.Powers[i].Description;
                this.Tooltip.Show(true);
                break;
            }
        }
        this.ListenForTapsAfter(0.5f);
    }

    [DebuggerHidden]
    private IEnumerator DisplayScenarioDetailsClose() => 
        new <DisplayScenarioDetailsClose>c__Iterator5C { <>f__this = this };

    private void FadeEffectButton(GuiButton button, bool isVisible, float time)
    {
        button.Fade(isVisible, time);
        Transform transform = button.transform.FindChild("Icon");
        if (transform != null)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if ((child != null) && child.gameObject.activeInHierarchy)
                {
                    float to = !isVisible ? ((float) 0) : ((float) 1);
                    LeanTween.alpha(child.gameObject, to, time);
                }
            }
        }
    }

    public void Flash()
    {
        if (!this.isPanelFlashing)
        {
            base.StartCoroutine(this.FlashCoroutine());
        }
    }

    [DebuggerHidden]
    private IEnumerator FlashCoroutine() => 
        new <FlashCoroutine>c__Iterator58 { <>f__this = this };

    private Effect GetEffect(int index)
    {
        Effect effect = Turn.Character.GetEffect(index);
        if (effect == null)
        {
            effect = Scenario.Current.GetEffect((int) (index - Turn.Character.GetNumEffects()));
        }
        return effect;
    }

    private Transform GetEffectButtonIcon(Transform root, Effect e)
    {
        if (e != null)
        {
            switch (e.GetEffectButtonIcon())
            {
                case CardType.Ally:
                    return root.FindChild("CardTypeIcon_Ally_small");

                case CardType.Armor:
                    return root.FindChild("CardTypeIcon_Armor_small");

                case CardType.Barrier:
                    return root.FindChild("CardTypeIcon_Barrier_small");

                case CardType.Blessing:
                    return root.FindChild("CardTypeIcon_Blessing_small");

                case CardType.Henchman:
                case CardType.Monster:
                case CardType.Villain:
                case CardType.Loot:
                    return root.FindChild("CardTypeIcon_Monster_small");

                case CardType.Item:
                    return root.FindChild("CardTypeIcon_Item_small");

                case CardType.Spell:
                    return root.FindChild("CardTypeIcon_Spell_small");

                case CardType.Weapon:
                    return root.FindChild("CardTypeIcon_Weapon_small");

                case CardType.Location:
                    return root.FindChild("EffectType_Location");

                case CardType.Character:
                    return root.FindChild("EffectType_HeroAbility");

                case CardType.Scenario:
                    return root.FindChild("EffectType_Scenario");
            }
        }
        return null;
    }

    private int GetTotalEffects()
    {
        if (this.ScenarioButton != null)
        {
            return (this.GetTotalExpandedEffects() + 1);
        }
        return this.GetTotalExpandedEffects();
    }

    private int GetTotalExpandedEffects() => 
        (Turn.Character.GetNumEffects() + Scenario.Current.GetNumEffects());

    private void HideEffectDetails()
    {
        if (this.zoomedCard.Count > 0)
        {
            base.StartCoroutine(this.DisplayCardDetailsClose());
        }
        else
        {
            base.StartCoroutine(this.DisplayScenarioDetailsClose());
        }
    }

    public override void Initialize()
    {
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.gestureRecognizedEvent += delegate (TKTapRecognizer r) {
            if (!UI.Busy)
            {
                this.OnGuiTap(this.tapRecognizer.touchLocation());
            }
        };
        this.tapRecognizer.zIndex = 100;
        this.tapRecognizer.enabled = false;
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        ScenarioPropertyEffectPanelButton component = Scenario.Current.GetComponent<ScenarioPropertyEffectPanelButton>();
        if (component != null)
        {
            this.ScenarioButton = UnityEngine.Object.Instantiate<GuiButton>(component.EffectButton);
            this.ScenarioButton.transform.parent = base.gameObject.transform;
            this.ScenarioButton.transform.localPosition = Vector3.zero;
            this.ScenarioButton.Show(true);
        }
    }

    private bool IsCardEffect(Effect e)
    {
        if (e == null)
        {
            return false;
        }
        if (e.Type == EffectType.ScenarioPower)
        {
            return false;
        }
        if (string.IsNullOrEmpty(e.source))
        {
            return false;
        }
        if (e.source.StartsWith("CH"))
        {
            return false;
        }
        if (e.source.StartsWith("LO"))
        {
            return false;
        }
        if (e.source.StartsWith("SC"))
        {
            return false;
        }
        return true;
    }

    public static bool IsCharacterEffect(Effect e)
    {
        if (e == null)
        {
            return false;
        }
        if (e.Type == EffectType.ScenarioPower)
        {
            return false;
        }
        if (string.IsNullOrEmpty(e.source))
        {
            return false;
        }
        return e.source.StartsWith("CH");
    }

    private bool IsScenarioEffect(Effect e)
    {
        if (e == null)
        {
            return false;
        }
        if (e.Type == EffectType.ScenarioPower)
        {
            return true;
        }
        if (string.IsNullOrEmpty(e.source))
        {
            return false;
        }
        return e.source.StartsWith("SC");
    }

    private void ListenForTapsAfter(float delay)
    {
        LeanTween.delayedCall(delay, (Action) (() => (this.tapRecognizer.enabled = true)));
    }

    private void OnCloseButtonPushed()
    {
        if (!UI.Window.Paused && !this.isPanelBusy)
        {
            base.StartCoroutine(this.ClosePanelCoroutine());
        }
    }

    private void OnEffectButton01Pushed()
    {
        this.OnEffectButtonPushed(1);
    }

    private void OnEffectButton02Pushed()
    {
        this.OnEffectButtonPushed(2);
    }

    private void OnEffectButton03Pushed()
    {
        this.OnEffectButtonPushed(3);
    }

    private void OnEffectButton04Pushed()
    {
        this.OnEffectButtonPushed(4);
    }

    private void OnEffectButton05Pushed()
    {
        this.OnEffectButtonPushed(5);
    }

    private void OnEffectButton06Pushed()
    {
        this.OnEffectButtonPushed(6);
    }

    private void OnEffectButton07Pushed()
    {
        this.OnEffectButtonPushed(7);
    }

    private void OnEffectButton08Pushed()
    {
        this.OnEffectButtonPushed(8);
    }

    private void OnEffectButton09Pushed()
    {
        this.OnEffectButtonPushed(9);
    }

    private void OnEffectButton10Pushed()
    {
        this.OnEffectButtonPushed(10);
    }

    private void OnEffectButton11Pushed()
    {
        this.OnEffectButtonPushed(11);
    }

    private void OnEffectButton12Pushed()
    {
        this.OnEffectButtonPushed(12);
    }

    private void OnEffectButtonPushed(int n)
    {
        if (!UI.Window.Paused && !this.isPanelBusy)
        {
            if (!this.isPanelOpen)
            {
                if (this.GetTotalEffects() > 1)
                {
                    base.StartCoroutine(this.OpenPanelCoroutine());
                }
                else
                {
                    this.DisplayEffectDetails(n - 1);
                }
            }
            else
            {
                this.DisplayEffectDetails(n - 1);
            }
        }
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        if (!this.isPanelBusy)
        {
            this.HideEffectDetails();
        }
    }

    [DebuggerHidden]
    private IEnumerator OpenPanelCoroutine() => 
        new <OpenPanelCoroutine>c__Iterator59 { <>f__this = this };

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        this.EffectButtons[0].Disable(isPaused);
    }

    public override void Refresh()
    {
        if (!this.isPanelOpen)
        {
            if (this.GetTotalEffects() > 0)
            {
                this.NoEffectsMessage.SetActive(false);
                if (this.ScenarioButton != null)
                {
                    this.ShowEffectButton(this.ScenarioButton, true);
                }
                else
                {
                    this.ShowEffectButton(0, true);
                }
            }
            else
            {
                this.NoEffectsMessage.SetActive(true);
                this.ShowEffectButton(0, false);
            }
        }
        else if (!this.isPanelBusy)
        {
            base.StartCoroutine(this.ClosePanelCoroutine());
        }
    }

    private void RefreshButtons()
    {
        for (int i = 0; i < this.EffectButtons.Length; i++)
        {
            this.EffectButtons[i].Refresh();
        }
    }

    public void RefreshTopButton()
    {
        if (this.ScenarioButton != null)
        {
            this.ScenarioButton.Refresh();
        }
        else
        {
            this.EffectButtons[0].Refresh();
        }
    }

    public override void Show(bool isVisible)
    {
        if (!isVisible)
        {
            for (int i = 0; i < this.EffectButtons.Length; i++)
            {
                this.EffectButtons[i].transform.localPosition = Vector3.zero;
                this.ShowEffectButton(i, false);
            }
            this.RefreshButtons();
            this.isPanelOpen = false;
            this.isPanelBusy = false;
            this.tapRecognizer.enabled = false;
            this.Refresh();
        }
    }

    private void ShowEffectButton(GuiButton button, bool isVisible)
    {
        button.Show(isVisible);
        this.FadeEffectButton(button, isVisible, 0.25f);
    }

    private void ShowEffectButton(int i, bool isVisible)
    {
        if ((i >= 0) && (i < this.EffectButtons.Length))
        {
            Effect e = this.GetEffect(i);
            if (e != null)
            {
                this.EffectButtons[i].Text = e.GetDisplayText();
            }
            this.ShowEffectButtonIcon(this.EffectButtons[i], e);
            this.ShowEffectButton(this.EffectButtons[i], isVisible);
        }
    }

    private void ShowEffectButtonIcon(GuiButton button, Effect e)
    {
        Transform root = button.transform.FindChild("Icon");
        if (root != null)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                root.GetChild(i).gameObject.SetActive(false);
            }
            Transform effectButtonIcon = this.GetEffectButtonIcon(root, e);
            if (effectButtonIcon != null)
            {
                effectButtonIcon.gameObject.SetActive(true);
            }
        }
    }

    public GuiButton ScenarioButton { get; set; }

    [CompilerGenerated]
    private sealed class <ClosePanelCoroutine>c__Iterator5A : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelEffects <>f__this;
        internal int <i>__1;
        internal int <i>__2;
        internal int <numEffects>__0;

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
                    this.<numEffects>__0 = this.<>f__this.GetTotalExpandedEffects();
                    this.<>f__this.Animator.SetInteger("EffectCount", this.<numEffects>__0);
                    this.<>f__this.Animator.SetTrigger("Close");
                    this.<>f__this.isPanelBusy = true;
                    this.<i>__1 = 0;
                    while (this.<i>__1 < this.<>f__this.EffectButtons.Length)
                    {
                        this.<>f__this.FadeEffectButton(this.<>f__this.EffectButtons[this.<i>__1], false, 0.25f);
                        LeanTween.moveLocal(this.<>f__this.EffectButtons[this.<i>__1].gameObject, Vector3.zero, 0.25f).setEase(LeanTweenType.easeOutQuad);
                        this.<i>__1++;
                    }
                    this.$current = new WaitForSeconds(0.3f);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<i>__2 = 0;
                    while (this.<i>__2 < this.<>f__this.EffectButtons.Length)
                    {
                        this.<>f__this.ShowEffectButton(this.<i>__2, false);
                        this.<i>__2++;
                    }
                    this.<>f__this.isPanelBusy = false;
                    this.<>f__this.isPanelOpen = false;
                    this.<>f__this.Refresh();
                    this.<>f__this.RefreshButtons();
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
    private sealed class <DisplayCardDetailsClose>c__Iterator5B : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelEffects <>f__this;
        internal int <i>__0;
        internal int <i>__1;

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
                    if (this.<>f__this.zoomedCard.Count <= 0)
                    {
                        break;
                    }
                    this.<>f__this.isPanelBusy = true;
                    this.<i>__0 = 0;
                    while (this.<i>__0 < this.<>f__this.zoomedCard.Count)
                    {
                        this.<>f__this.zoomedCard[this.<i>__0].OnGuiZoom(false);
                        this.<i>__0++;
                    }
                    this.$current = new WaitForSeconds(0.25f);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<i>__1 = this.<>f__this.zoomedCard.Count - 1;
                    while (this.<i>__1 >= 0)
                    {
                        this.<>f__this.zoomedCard[this.<i>__1].Destroy();
                        this.<i>__1--;
                    }
                    this.<>f__this.zoomedCard.Clear();
                    UI.Window.Pause(false);
                    this.<>f__this.tapRecognizer.enabled = false;
                    this.<>f__this.isPanelBusy = false;
                    break;

                default:
                    goto Label_0142;
            }
            this.$PC = -1;
        Label_0142:
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
    private sealed class <DisplayScenarioDetailsClose>c__Iterator5C : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelEffects <>f__this;

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
                    this.<>f__this.isPanelBusy = true;
                    this.<>f__this.Tooltip.Show(false);
                    this.$current = new WaitForSeconds(0.25f);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.Tooltip.Text = null;
                    UI.Window.Pause(false);
                    this.<>f__this.tapRecognizer.enabled = false;
                    this.<>f__this.isPanelBusy = false;
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
    private sealed class <FlashCoroutine>c__Iterator58 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelEffects <>f__this;

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
                    this.<>f__this.isPanelFlashing = true;
                    VisualEffect.Start(this.<>f__this.FlashVfx);
                    this.$current = new WaitForSeconds(1f);
                    this.$PC = 1;
                    return true;

                case 1:
                    VisualEffect.Stop(this.<>f__this.FlashVfx);
                    this.<>f__this.isPanelFlashing = false;
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
    private sealed class <OpenPanelCoroutine>c__Iterator59 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelEffects <>f__this;
        internal int <i>__1;
        internal int <i>__4;
        internal int <maxi>__3;
        internal int <numEffects>__0;
        internal float <top>__2;
        internal Vector3 <v>__6;
        internal float <y>__5;

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
                    this.<numEffects>__0 = this.<>f__this.GetTotalExpandedEffects();
                    this.<>f__this.Animator.SetInteger("EffectCount", this.<numEffects>__0);
                    this.<>f__this.Animator.SetTrigger("Open");
                    this.<>f__this.isPanelBusy = true;
                    this.<i>__1 = 0;
                    while (this.<i>__1 < this.<>f__this.EffectButtons.Length)
                    {
                        this.<>f__this.EffectButtons[this.<i>__1].transform.localPosition = Vector3.zero;
                        this.<i>__1++;
                    }
                    this.<top>__2 = 0f;
                    if ((this.<numEffects>__0 % 2) == 0)
                    {
                        this.<top>__2 = (0.875f * (this.<numEffects>__0 - 1)) / 2f;
                    }
                    else
                    {
                        this.<top>__2 = 0.875f * (this.<numEffects>__0 / 2);
                    }
                    this.<maxi>__3 = Mathf.Min(this.<>f__this.EffectButtons.Length, this.<numEffects>__0);
                    this.<i>__4 = 0;
                    while (this.<i>__4 < this.<maxi>__3)
                    {
                        this.<>f__this.ShowEffectButton(this.<i>__4, true);
                        this.<y>__5 = this.<top>__2 - (0.875f * this.<i>__4);
                        this.<v>__6 = new Vector3(0f, this.<y>__5, 0f);
                        LeanTween.moveLocal(this.<>f__this.EffectButtons[this.<i>__4].gameObject, this.<v>__6, 0.25f).setEase(LeanTweenType.easeOutQuad);
                        this.<i>__4++;
                    }
                    if (this.<>f__this.ScenarioButton != null)
                    {
                        this.<>f__this.ShowEffectButton(this.<>f__this.ScenarioButton, false);
                    }
                    this.$current = new WaitForSeconds(0.3f);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.RefreshButtons();
                    this.<>f__this.isPanelBusy = false;
                    this.<>f__this.isPanelOpen = true;
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

