using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiWindowCollection : GuiWindow
{
    [Tooltip("reference to the adventure panel in our hierarchy")]
    public GuiPanel adventuresPanel;
    [Tooltip("reference to the cards panel in our hierarchy")]
    public GuiPanel cardsPanel;
    [Tooltip("reference to the locations panel in our hierarchy")]
    public GuiPanel locationsPanel;
    private int screenNumber;
    [Tooltip("reference to the large animated title in our hierarchy")]
    public GuiLabel titleLabel;

    private void Clear()
    {
        this.CloseSubWindows();
        if (this.screenNumber == 1)
        {
            this.cardsPanel.Show(false);
        }
        if (this.screenNumber == 2)
        {
            this.locationsPanel.Show(false);
        }
        if (this.screenNumber == 3)
        {
            this.adventuresPanel.Show(false);
        }
    }

    private void CloseSubWindows()
    {
        Tutorial.Hide();
        if (this.screenNumber == 1)
        {
            (this.cardsPanel as GuiPanelCollectionCards).CloseSubWindows();
        }
    }

    private void OnAdventureScreenButtonPushed()
    {
        if ((!UI.Busy && !base.Paused) && (this.screenNumber != 3))
        {
            this.Clear();
            this.adventuresPanel.Show(true);
            this.screenNumber = 3;
        }
    }

    private void OnBackButtonPushed()
    {
        if (!UI.Busy && !base.Paused)
        {
            this.CloseSubWindows();
            switch (ExitScene)
            {
                case WindowType.Adventure:
                    Game.UI.ShowAdventureScene();
                    break;

                case WindowType.Scenario:
                    Game.UI.ShowSetupScene();
                    break;

                case WindowType.Reward:
                    Game.UI.ShowRewardScene();
                    break;

                case WindowType.SelectCards:
                    Game.UI.ShowSelectCardScene();
                    break;

                case WindowType.CreateParty:
                    Game.UI.ShowCreatePartyScene();
                    break;

                case WindowType.Location:
                    if (!Game.Instance.Reload())
                    {
                        Game.UI.ShowMainMenu();
                    }
                    break;

                case WindowType.Cutscene:
                    Game.UI.ShowCutsceneScene();
                    break;

                default:
                    Game.UI.ShowMainMenu();
                    break;
            }
            ExitScene = WindowType.None;
        }
    }

    private void OnCardScreenButtonPushed()
    {
        if ((!UI.Busy && !base.Paused) && (this.screenNumber != 1))
        {
            this.Clear();
            this.cardsPanel.Show(true);
            this.screenNumber = 1;
        }
    }

    private void OnLocationScreenButtonPushed()
    {
        if ((!UI.Busy && !base.Paused) && (this.screenNumber != 2))
        {
            this.Clear();
            this.locationsPanel.Show(true);
            this.screenNumber = 2;
        }
    }

    private void OnMenuButtonPushed()
    {
        if (!UI.Busy && !base.Paused)
        {
            this.CloseSubWindows();
            Game.UI.OptionsPanel.Show(true);
        }
    }

    private void OnStoreButtonPushed()
    {
        if ((!UI.Busy && !base.Paused) && !Settings.Debug.DemoMode)
        {
            this.CloseSubWindows();
            Game.UI.ShowStoreWindow();
        }
    }

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        this.locationsPanel.Pause(isPaused);
        this.adventuresPanel.Pause(isPaused);
        this.cardsPanel.Pause(isPaused);
    }

    [DebuggerHidden]
    private IEnumerator ShowCardPanelAfterDelay() => 
        new <ShowCardPanelAfterDelay>c__Iterator86 { <>f__this = this };

    protected override void Start()
    {
        base.Start();
        this.adventuresPanel.Initialize();
        this.locationsPanel.Initialize();
        this.cardsPanel.Initialize();
        base.StartCoroutine(this.ShowCardPanelAfterDelay());
    }

    private void Update()
    {
        if (!GuiPanel.IsFullScreenPanelShowing() && Device.GetIsBackButtonPushed())
        {
            this.OnBackButtonPushed();
        }
    }

    public static WindowType ExitScene
    {
        [CompilerGenerated]
        get => 
            <ExitScene>k__BackingField;
        [CompilerGenerated]
        set
        {
            <ExitScene>k__BackingField = value;
        }
    }

    public override WindowType Type =>
        WindowType.Collection;

    [CompilerGenerated]
    private sealed class <ShowCardPanelAfterDelay>c__Iterator86 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiWindowCollection <>f__this;

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
                    this.<>f__this.titleLabel.Text = (Campaign.Box.Count > 0) ? UI.Text(430) : UI.Text(0x166);
                    this.$current = new WaitForSeconds(2f);
                    this.$PC = 1;
                    return true;

                case 1:
                    UI.Busy = false;
                    this.<>f__this.OnCardScreenButtonPushed();
                    if (Campaign.Box.Count > 0)
                    {
                        Tutorial.Notify(TutorialEventType.ScreenCollectionVaultShown);
                        break;
                    }
                    Tutorial.Notify(TutorialEventType.ScreenCollectionGalleryShown);
                    break;

                default:
                    goto Label_00BD;
            }
            this.$PC = -1;
        Label_00BD:
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

