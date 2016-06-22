using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelStoreOverlay : GuiPanelBackStack
{
    private GuiButton activeButton;
    [Tooltip("reference to the adventures button in this panel")]
    public GuiButton AdventuresButton;
    [Tooltip("reference to the back button in this panel")]
    public GuiButton BackButton;
    [Tooltip("reference to the characters button in this panel")]
    public GuiButton CharactersButton;
    public GameObject chestVFX;
    public float chestVFXDuration;
    [Tooltip("reference to the gold button in this panel")]
    public GuiButton GoldButton;
    public GameObject goldVFX;
    public float goldVFXDuration;
    [Tooltip("reference to the store home button in this panel")]
    public GuiButton HomeButton;
    [Tooltip("reference to the normal sprite for most buttons to use")]
    public Sprite normalButtonSprite;
    [Tooltip("reference to the specials button in this panel")]
    public GuiButton SpecialsButton;
    [Tooltip("reference to the store manager window in the scene")]
    public GuiWindowStore StoreManager;
    private TKTapRecognizer tapRecognizer;
    [Tooltip("reference to the treasure purchase button in this panel")]
    public GuiButton TreasureBuyButton;
    [Tooltip("reference to the treasure reveal button in this panel")]
    public GuiButton TreasureRevealButton;

    private void BackButtonPushed()
    {
        UI.Busy = true;
        if ((this.StoreManager.ActivePanelType == GuiWindowStore.StorePanelType.Treasure_Open) && this.StoreManager.treasureRevealPanel.IsCardsOnScreen())
        {
            this.StoreManager.treasureRevealPanel.RevealAll(delegate {
                if (this.StoreManager.ActivePanelType != GuiWindowStore.StorePanelType.Start)
                {
                    this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Start);
                }
                else
                {
                    this.StoreManager.CloseStore();
                }
                this.ButtonGlow(this.HomeButton);
                this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Start);
            });
        }
        else
        {
            if (this.StoreManager.ActivePanelType != GuiWindowStore.StorePanelType.Start)
            {
                this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Start);
            }
            else
            {
                this.StoreManager.CloseStore();
            }
            this.ButtonGlow(this.HomeButton);
            this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Start);
        }
    }

    private void ButtonGlow(GuiButton button)
    {
        this.SpecialsButton.Glow(this.SpecialsButton == button);
        this.CharactersButton.Glow(this.CharactersButton == button);
        this.AdventuresButton.Glow(this.AdventuresButton == button);
        this.GoldButton.Glow(this.GoldButton == button);
        this.TreasureBuyButton.Glow(this.TreasureBuyButton == button);
        this.HomeButton.Glow(this.HomeButton == button);
        this.activeButton = button;
    }

    public void ButtonGlow(GuiWindowStore.StorePanelType panelType)
    {
        if (panelType == GuiWindowStore.StorePanelType.Adventures)
        {
            this.ButtonGlow(this.AdventuresButton);
        }
        else if (panelType == GuiWindowStore.StorePanelType.Characters)
        {
            this.ButtonGlow(this.CharactersButton);
        }
        else if (panelType == GuiWindowStore.StorePanelType.Gold)
        {
            this.ButtonGlow(this.GoldButton);
        }
        else if (panelType == GuiWindowStore.StorePanelType.Specials)
        {
            this.ButtonGlow(this.SpecialsButton);
        }
        else if (panelType == GuiWindowStore.StorePanelType.Start)
        {
            this.ButtonGlow(this.HomeButton);
        }
        else if (panelType == GuiWindowStore.StorePanelType.Treasure_Buy)
        {
            this.ButtonGlow(this.TreasureBuyButton);
        }
        else if (panelType == GuiWindowStore.StorePanelType.Treasure_Open)
        {
            this.ButtonGlow(this.TreasureRevealButton);
        }
    }

    public void ExternalSpecialsButton()
    {
        this.OnSpecialsButtonPushed();
    }

    public override void Initialize()
    {
        this.HomeButton.Glow(true);
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.zIndex = 3;
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.tapRecognizer.enabled = false;
    }

    private void OnAdventuresButtonPushed()
    {
        if (!UI.Busy && Game.Network.Connected)
        {
            if (this.activeButton != this.AdventuresButton)
            {
                UI.Busy = true;
                if ((this.StoreManager.ActivePanelType == GuiWindowStore.StorePanelType.Treasure_Open) && this.StoreManager.treasureRevealPanel.IsCardsOnScreen())
                {
                    this.StoreManager.treasureRevealPanel.RevealAll(() => this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Adventures));
                }
                else
                {
                    this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Adventures);
                }
            }
            this.ButtonGlow(this.AdventuresButton);
        }
        else if (!Game.Network.Connected)
        {
            Game.UI.NetworkTooltip.ShowMessage(GuiPanelNetworkTooltip.MessageType.MustBeLoggedIn);
        }
    }

    private void OnBackButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            UI.Busy = true;
            LeanTween.scale(this.BackButton.gameObject, new Vector3(1.1f, 1.1f, 1f), 0.1f).setLoopPingPong().setLoopCount(2).setOnComplete(new Action(this.BackButtonPushed));
        }
    }

    private void OnCharactersButtonPushed()
    {
        if (!UI.Busy && Game.Network.Connected)
        {
            if (this.activeButton != this.CharactersButton)
            {
                UI.Busy = true;
                if ((this.StoreManager.ActivePanelType == GuiWindowStore.StorePanelType.Treasure_Open) && this.StoreManager.treasureRevealPanel.IsCardsOnScreen())
                {
                    this.StoreManager.treasureRevealPanel.RevealAll(() => this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Characters));
                }
                else
                {
                    this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Characters);
                }
            }
            this.ButtonGlow(this.CharactersButton);
        }
        else if (!Game.Network.Connected)
        {
            Game.UI.NetworkTooltip.ShowMessage(GuiPanelNetworkTooltip.MessageType.MustBeLoggedIn);
        }
    }

    private void OnCloseButtonPushed()
    {
        if (!UI.Busy)
        {
            UI.Busy = true;
            if ((this.StoreManager.ActivePanelType == GuiWindowStore.StorePanelType.Treasure_Open) && this.StoreManager.treasureRevealPanel.IsCardsOnScreen())
            {
                this.StoreManager.treasureRevealPanel.RevealAll(() => this.StoreManager.CloseStore());
            }
            else
            {
                this.StoreManager.CloseStore();
            }
        }
    }

    private void OnGoldButtonPushed()
    {
        if (!UI.Busy && Game.Network.Connected)
        {
            if (this.activeButton != this.GoldButton)
            {
                UI.Busy = true;
                if ((this.StoreManager.ActivePanelType == GuiWindowStore.StorePanelType.Treasure_Open) && this.StoreManager.treasureRevealPanel.IsCardsOnScreen())
                {
                    this.StoreManager.treasureRevealPanel.RevealAll(() => this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Gold));
                }
                else
                {
                    this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Gold);
                }
            }
            this.ButtonGlow(this.GoldButton);
        }
        else if (!Game.Network.Connected)
        {
            Game.UI.NetworkTooltip.ShowMessage(GuiPanelNetworkTooltip.MessageType.MustBeLoggedIn);
        }
    }

    private void OnHomeButtonPushed()
    {
        if (!UI.Busy)
        {
            if (this.activeButton != this.HomeButton)
            {
                UI.Busy = true;
                if ((this.StoreManager.ActivePanelType == GuiWindowStore.StorePanelType.Treasure_Open) && this.StoreManager.treasureRevealPanel.IsCardsOnScreen())
                {
                    this.StoreManager.treasureRevealPanel.RevealAll(() => this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Start));
                }
                else
                {
                    this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Start);
                }
            }
            this.ButtonGlow(this.HomeButton);
        }
    }

    private void OnSpecialsButtonPushed()
    {
        if (!UI.Busy)
        {
            if (this.activeButton != this.SpecialsButton)
            {
                UI.Busy = true;
                if ((this.StoreManager.ActivePanelType == GuiWindowStore.StorePanelType.Treasure_Open) && this.StoreManager.treasureRevealPanel.IsCardsOnScreen())
                {
                    this.StoreManager.treasureRevealPanel.RevealAll(() => this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Specials));
                }
                else
                {
                    this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Specials);
                }
            }
            this.ButtonGlow(this.SpecialsButton);
        }
    }

    private void OnTreasureChestButtonPushed()
    {
        if (!UI.Busy && Game.Network.Connected)
        {
            if ((Game.Network.CurrentUser.Chests != 0) && (this.StoreManager.ActivePanelType != GuiWindowStore.StorePanelType.Treasure_Open))
            {
                UI.Busy = true;
                this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Treasure_Open);
                this.ButtonGlow(this.TreasureRevealButton);
            }
            else if (this.StoreManager.ActivePanelType != GuiWindowStore.StorePanelType.Treasure_Buy)
            {
                this.OnTreasureChestBuyButtonPushed();
            }
            else
            {
                UI.Busy = false;
            }
        }
        else if (!Game.Network.Connected)
        {
            Game.UI.NetworkTooltip.ShowMessage(GuiPanelNetworkTooltip.MessageType.MustBeLoggedIn);
        }
    }

    private void OnTreasureChestBuyButtonPushed()
    {
        if (!UI.Busy && Game.Network.Connected)
        {
            if (this.activeButton != this.TreasureBuyButton)
            {
                UI.Busy = true;
                if ((this.StoreManager.ActivePanelType == GuiWindowStore.StorePanelType.Treasure_Open) && this.StoreManager.treasureRevealPanel.IsCardsOnScreen())
                {
                    this.StoreManager.treasureRevealPanel.RevealAll(() => this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Treasure_Buy));
                }
                else
                {
                    this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Treasure_Buy);
                }
            }
            this.ButtonGlow(this.TreasureBuyButton);
        }
        else if (!Game.Network.Connected)
        {
            Game.UI.NetworkTooltip.ShowMessage(GuiPanelNetworkTooltip.MessageType.MustBeLoggedIn);
        }
    }

    public override void Refresh()
    {
        base.Refresh();
        this.ButtonGlow(this.StoreManager.ActivePanelType);
        if (Game.Network.Connected)
        {
            this.AdventuresButton.Image = this.normalButtonSprite;
            this.CharactersButton.Image = this.normalButtonSprite;
            this.TreasureBuyButton.Image = this.normalButtonSprite;
            this.GoldButton.Image = this.normalButtonSprite;
        }
        else
        {
            this.AdventuresButton.Image = this.AdventuresButton.ImageDisabled;
            this.CharactersButton.Image = this.AdventuresButton.ImageDisabled;
            this.TreasureBuyButton.Image = this.AdventuresButton.ImageDisabled;
            this.GoldButton.Image = this.AdventuresButton.ImageDisabled;
        }
    }

    private void ResetVfx(GameObject vfx)
    {
        vfx.SetActive(false);
        vfx.transform.localScale = Vector3.one;
        vfx.transform.rotation = Quaternion.identity;
        vfx.transform.localScale = Vector3.one;
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.tapRecognizer.enabled = isVisible;
        if (isVisible)
        {
            this.Refresh();
        }
        else
        {
            UI.Busy = false;
        }
    }

    public void ShowChestVFX()
    {
        UI.Sound.Play(SoundEffectType.StorePurchase);
        this.ShowVFX(this.chestVFX);
    }

    public void ShowGoldVFX()
    {
        UI.Sound.Play(SoundEffectType.StorePurchase);
        this.ShowVFX(this.goldVFX);
    }

    private void ShowVFX(GameObject vfx)
    {
        float chestVFXDuration = this.chestVFXDuration;
        if (vfx != null)
        {
            vfx.transform.localScale = Vector3.one;
            vfx.transform.rotation = Quaternion.identity;
            VisualEffect.Start(vfx);
            if (chestVFXDuration > 0f)
            {
                base.StartCoroutine(this.StopVfxCoroutine(vfx, chestVFXDuration));
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator StopVfxCoroutine(GameObject vfx, float delay) => 
        new <StopVfxCoroutine>c__Iterator70 { 
            delay = delay,
            vfx = vfx,
            <$>delay = delay,
            <$>vfx = vfx,
            <>f__this = this
        };

    public static GuiPanelStoreOverlay Current
    {
        get
        {
            GuiWindow current = GuiWindow.Current;
            if (current != null)
            {
                GuiWindowStore store = current as GuiWindowStore;
                if (store != null)
                {
                    return store.overlayPanel;
                }
            }
            return null;
        }
    }

    [CompilerGenerated]
    private sealed class <StopVfxCoroutine>c__Iterator70 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal float <$>delay;
        internal GameObject <$>vfx;
        internal GuiPanelStoreOverlay <>f__this;
        internal float delay;
        internal GameObject vfx;

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
                    this.$current = new WaitForSeconds(this.delay);
                    this.$PC = 1;
                    goto Label_0083;

                case 1:
                    VisualEffect.Stop(this.vfx);
                    this.$current = new WaitForSeconds(0.5f);
                    this.$PC = 2;
                    goto Label_0083;

                case 2:
                    this.<>f__this.ResetVfx(this.vfx);
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_0083:
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

    public delegate void RevealAllTreasureChestCardsCallback();
}

