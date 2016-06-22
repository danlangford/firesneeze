using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelTempClose : GuiPanel
{
    [Tooltip("reference to the available icons in this scene (should be 8)")]
    public GameObject[] AvailableIcons;
    [Tooltip("reference to the character token disabled overlays in this scene")]
    public GameObject[] CharacterDoneHilites;
    [Tooltip("reference to the character token hilite in this scene")]
    public GameObject CharacterHilite;
    [Tooltip("reference to the character sheet panel in this scene")]
    public GuiPanelCharacter CharacterPanel;
    [Tooltip("reference to the button that closes the selected location")]
    public GuiButton CloseButton;
    [Tooltip("reference to the button that exits temp close")]
    public GuiButton DoneButton;
    [Tooltip("reference to the icon layout grid in our hierarchy")]
    public ScenarioMapIconGrid IconGrid;
    private bool isLoading;
    private bool isZoomed;
    [Tooltip("reference to the map location hilite in this scene")]
    public GameObject LocationHilite;
    [Tooltip("reference to the location panel in our hierarchy")]
    public GuiPanelLocation LocationPanel;
    private string selectedCharacter;
    private string selectedLocation;
    private TKTapRecognizer tapRecognizer;
    private Card villainCard;
    [Tooltip("reference to the marker where the fake villain card is spawned")]
    public GameObject VillainCardMarker;
    [Tooltip("reference to the icon representing the villain's current location")]
    public GameObject VillainLocationIcon;

    private void AskCloseLocation(string id)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Turn.SwitchCharacter(Party.IndexOf(id));
            Turn.Current = Turn.Number;
            Location.Load(Turn.Character.Location);
            window.Refresh();
            Turn.State = GameStateType.AskClose;
            string helperText = StringTableManager.GetHelperText(0x56);
            window.messagePanel.Show(helperText);
        }
    }

    private void ClearCloseAttempted()
    {
        Turn.BlackBoard.Set<int>("TempCloseAttempted", 0);
    }

    private ScenarioMapIcon FindMapIcon(string id)
    {
        for (int i = 0; i < this.IconGrid.Icons.Length; i++)
        {
            if ((this.IconGrid.Icons[i] != null) && (this.IconGrid.Icons[i].ID == id))
            {
                return this.IconGrid.Icons[i];
            }
        }
        return null;
    }

    private bool GetCloseAttempted(Character c)
    {
        int index = Party.IndexOf(c.ID);
        return Turn.BlackBoard.GetBitFlag("TempCloseAttempted", index);
    }

    private string GetNextCharacter(string id)
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if ((Party.Characters[i].Alive && (Party.Characters[i].Location == id)) && ((Party.Characters[i].ID == this.selectedCharacter) && !this.GetCloseAttempted(Party.Characters[i])))
            {
                return this.selectedCharacter;
            }
        }
        for (int j = 0; j < Party.Characters.Count; j++)
        {
            if ((Party.Characters[j].Alive && (Party.Characters[j].Location == id)) && !this.GetCloseAttempted(Party.Characters[j]))
            {
                return Party.Characters[j].ID;
            }
        }
        return null;
    }

    private string GetNextLocation()
    {
        for (int i = 0; i < Scenario.Current.Locations.Length; i++)
        {
            string locationName = Scenario.Current.Locations[i].LocationName;
            if (this.IsClosePossible(locationName))
            {
                return locationName;
            }
        }
        return null;
    }

    public override void Initialize()
    {
        this.IconGrid.Initialize();
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.gestureRecognizedEvent += delegate (TKTapRecognizer r) {
            if (!this.isLoading)
            {
                this.OnGuiTap(this.tapRecognizer.touchLocation());
            }
        };
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.tapRecognizer.enabled = false;
        this.LocationPanel.Owner = this;
    }

    private bool IsCloseFailed(string id)
    {
        if (Scenario.Current.IsLocationClosed(id))
        {
            return false;
        }
        int num = Location.CountCharactersAtLocation(id);
        if (num <= 0)
        {
            return false;
        }
        for (int i = 0; i < Scenario.Current.Powers.Count; i++)
        {
            ScenarioPowerCannotTempClose component = Scenario.Current.Powers[i].GetComponent<ScenarioPowerCannotTempClose>();
            if ((component != null) && component.IsLocationValid(id))
            {
                return true;
            }
        }
        int num3 = 0;
        for (int j = 0; j < Party.Characters.Count; j++)
        {
            if ((Party.Characters[j].Location == id) && this.GetCloseAttempted(Party.Characters[j]))
            {
                num3++;
            }
        }
        return (num3 >= num);
    }

    private bool IsClosePossible(string id)
    {
        if (Scenario.Current.IsLocationClosePossible(id, CloseType.Temporary))
        {
            if (id == Turn.BlackBoard.Get<string>("VillainLocationID"))
            {
                return false;
            }
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (((Party.Characters[i].Location == id) && Party.Characters[i].Alive) && !this.GetCloseAttempted(Party.Characters[i]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsLoadingRequired()
    {
        if (this.selectedCharacter != null)
        {
            int index = Party.IndexOf(this.selectedCharacter);
            return (Party.Characters[index].Location != Location.Current.ID);
        }
        return true;
    }

    private void OnCharacterSheetButtonPushed()
    {
        if (!base.Paused && !this.isZoomed)
        {
            this.Pause(true);
            this.Show(false);
            this.CharacterPanel.Owner = this;
            this.CharacterPanel.Show(true);
        }
    }

    private void OnCloseButtonPushed()
    {
        if ((!base.Paused && !this.isZoomed) && ((this.selectedCharacter != null) && (Turn.Iterators.Current != null)))
        {
            base.StartCoroutine(this.OnCloseButtonPushed_Coroutine());
        }
    }

    [DebuggerHidden]
    private IEnumerator OnCloseButtonPushed_Coroutine() => 
        new <OnCloseButtonPushed_Coroutine>c__Iterator77 { <>f__this = this };

    private void OnDoneButtonPushed()
    {
        if ((!base.Paused && !this.isZoomed) && (Turn.Iterators.Current != null))
        {
            base.StartCoroutine(this.OnDoneButtonPushed_Coroutine());
        }
    }

    [DebuggerHidden]
    private IEnumerator OnDoneButtonPushed_Coroutine() => 
        new <OnDoneButtonPushed_Coroutine>c__Iterator78 { <>f__this = this };

    private void OnGuiTap(Vector2 touchPos)
    {
        if (this.isZoomed)
        {
            this.isZoomed = false;
            this.ZoomVillainCard(false);
        }
        else
        {
            Card topCard = GuiLayout.GetTopCard(touchPos);
            if ((topCard != null) && (topCard == this.villainCard))
            {
                this.isZoomed = true;
                this.ZoomVillainCard(true);
            }
            RaycastHit2D hitd = Physics2D.Raycast(base.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_MAP);
            if (hitd != 0)
            {
                CharacterTokenMap component = hitd.collider.transform.GetComponent<CharacterTokenMap>();
                if (component != null)
                {
                    this.SelectCharacter(component);
                }
                ScenarioMapIcon icon = hitd.collider.transform.GetComponent<ScenarioMapIcon>();
                if (icon != null)
                {
                    UI.Sound.Play(icon.ClickSound);
                    this.LocationPanel.Show(icon.ID);
                }
            }
        }
    }

    private void OnLocationChanged(string id)
    {
        this.SelectLocation(id);
    }

    private void OnMenuButtonPushed()
    {
        if (!base.Paused && !this.isZoomed)
        {
            Game.UI.OptionsPanel.Owner = this;
            Game.UI.OptionsPanel.Show(true);
        }
    }

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        this.tapRecognizer.enabled = !isPaused;
    }

    public override void Refresh()
    {
        for (int i = 0; i < this.AvailableIcons.Length; i++)
        {
            this.AvailableIcons[i].SetActive(false);
        }
        for (int j = 0; j < this.CharacterDoneHilites.Length; j++)
        {
            this.CharacterDoneHilites[j].SetActive(false);
        }
        if (this.IconGrid.Icons != null)
        {
            this.VillainLocationIcon.SetActive(false);
            string str = Turn.BlackBoard.Get<string>("VillainLocationID");
            for (int k = 0; k < this.IconGrid.Icons.Length; k++)
            {
                if ((this.IconGrid.Icons[k] != null) && (this.IconGrid.Icons[k].ID == str))
                {
                    this.VillainLocationIcon.SetActive(true);
                    float y = 0.24f;
                    this.VillainLocationIcon.transform.position = this.IconGrid.Icons[k].transform.position + new Vector3(0f, y, 0f);
                    Vector3 localScale = this.VillainLocationIcon.transform.localScale;
                    this.VillainLocationIcon.transform.localScale = Vector3.zero;
                    LeanTween.scale(this.VillainLocationIcon, localScale, 0.2f).setEase(LeanTweenType.easeInOutQuad);
                    break;
                }
            }
            for (int m = 0; m < this.IconGrid.Icons.Length; m++)
            {
                if (((this.IconGrid.Icons[m] != null) && Scenario.Current.IsLocationValid(this.IconGrid.Icons[m].ID)) && this.IsClosePossible(this.IconGrid.Icons[m].ID))
                {
                    for (int num6 = 0; num6 < this.AvailableIcons.Length; num6++)
                    {
                        if (!this.AvailableIcons[num6].activeSelf)
                        {
                            this.AvailableIcons[num6].SetActive(true);
                            this.AvailableIcons[num6].transform.position = this.IconGrid.Icons[m].transform.position;
                            break;
                        }
                    }
                }
            }
            for (int n = 0; n < this.IconGrid.Icons.Length; n++)
            {
                if (((this.IconGrid.Icons[n] != null) && Scenario.Current.IsLocationValid(this.IconGrid.Icons[n].ID)) && this.IsCloseFailed(this.IconGrid.Icons[n].ID))
                {
                    Scenario.Current.CloseLocation(this.IconGrid.Icons[n].ID, CloseType.Impossible);
                    this.IconGrid.Icons[n].Refresh(false);
                }
            }
            for (int num8 = 0; num8 < this.IconGrid.Icons.Length; num8++)
            {
                if ((this.IconGrid.Icons[num8] != null) && Scenario.Current.IsLocationValid(this.IconGrid.Icons[num8].ID))
                {
                    for (int num9 = 0; num9 < Party.Characters.Count; num9++)
                    {
                        if (Party.Characters[num9].Location == this.IconGrid.Icons[num8].ID)
                        {
                            CharacterTokenMap characterToken = this.IconGrid.Icons[num8].GetCharacterToken(Party.Characters[num9]);
                            if ((characterToken != null) && this.GetCloseAttempted(Party.Characters[num9]))
                            {
                                characterToken.Interactive = false;
                                for (int num10 = 0; num10 < this.CharacterDoneHilites.Length; num10++)
                                {
                                    if (!this.CharacterDoneHilites[num10].activeSelf)
                                    {
                                        this.CharacterDoneHilites[num10].SetActive(true);
                                        this.CharacterDoneHilites[num10].transform.position = characterToken.transform.position;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            for (int num11 = 0; num11 < this.IconGrid.Icons.Length; num11++)
            {
                if (((this.IconGrid.Icons[num11] != null) && Scenario.Current.IsLocationValid(this.IconGrid.Icons[num11].ID)) && this.IsClosePossible(this.IconGrid.Icons[num11].ID))
                {
                    for (int num12 = 0; num12 < Party.Characters.Count; num12++)
                    {
                        CharacterTokenMap map2 = this.IconGrid.Icons[num11].GetCharacterToken(Party.Characters[num12]);
                        if (map2 != null)
                        {
                            map2.Interactive = !this.GetCloseAttempted(Party.Characters[num12]);
                        }
                    }
                }
            }
        }
    }

    private void SelectCharacter(CharacterTokenMap token)
    {
        this.selectedCharacter = token.ID;
        ScenarioMapIcon componentInParent = token.GetComponentInParent<ScenarioMapIcon>();
        if (componentInParent != null)
        {
            UI.Sound.Play(componentInParent.ClickSound);
            this.LocationPanel.Show(componentInParent.ID);
        }
    }

    private void SelectLocation(string id)
    {
        ScenarioMapIcon icon = this.FindMapIcon(id);
        if (icon != null)
        {
            this.selectedLocation = icon.ID;
            this.LocationHilite.transform.parent = icon.transform;
            this.LocationHilite.transform.localPosition = Vector3.zero;
            this.LocationHilite.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            this.LocationHilite.SetActive(true);
            if (this.IsClosePossible(this.selectedLocation))
            {
                this.CloseButton.Disable(false);
                this.CloseButton.Tint(Color.white);
                this.selectedCharacter = this.GetNextCharacter(this.selectedLocation);
                CharacterTokenMap characterToken = icon.GetCharacterToken(Party.Find(this.selectedCharacter));
                if (characterToken != null)
                {
                    this.CharacterHilite.transform.parent = characterToken.transform;
                    this.CharacterHilite.transform.localPosition = Vector3.zero;
                    this.CharacterHilite.SetActive(true);
                }
            }
            else
            {
                this.CloseButton.Disable(true);
                this.CloseButton.Tint(Color.gray);
                this.selectedCharacter = null;
                this.CharacterHilite.SetActive(false);
            }
        }
    }

    private void SetCloseAttempted(Character c)
    {
        int index = Party.IndexOf(c.ID);
        Turn.BlackBoard.SetBitFlag("TempCloseAttempted", index);
    }

    public override void Show(bool isVisible)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Show("UI_BottomLeft", !isVisible);
            window.Show("UI_Right", !isVisible);
            window.Show("UI_Center", !isVisible);
        }
        if (isVisible)
        {
            this.isLoading = false;
        }
        UI.Window.Pause(isVisible);
        this.tapRecognizer.enabled = isVisible;
        this.IconGrid.Show(isVisible);
        base.Show(isVisible);
        if (isVisible)
        {
            string nextLocation = this.GetNextLocation();
            if (nextLocation == null)
            {
                nextLocation = Location.Current.ID;
            }
            this.LocationPanel.Show(nextLocation);
        }
        if (isVisible)
        {
            this.Refresh();
        }
        if (isVisible && (this.villainCard == null))
        {
            string villain = Scenario.Current.Villain;
            if (Scenario.Current.IsCurrentVillain(Turn.Card.ID))
            {
                villain = Turn.Card.ID;
            }
            this.villainCard = CardTable.Create(villain);
            if (this.villainCard != null)
            {
                this.villainCard.transform.parent = base.transform;
                this.villainCard.transform.position = this.VillainCardMarker.transform.position;
                this.villainCard.transform.localScale = this.VillainCardMarker.transform.localScale;
                this.villainCard.SortingOrder = 200;
                this.villainCard.Show(CardSideType.Front);
            }
        }
        if (!isVisible)
        {
            if (this.villainCard != null)
            {
                this.villainCard.Destroy();
            }
            this.villainCard = null;
        }
        if (!isVisible)
        {
            Game.UI.OptionsPanel.Owner = null;
        }
        if ((!isVisible && (window != null)) && ((window.layoutSummoner.Card != null) && Scenario.Current.IsCurrentVillain(window.layoutSummoner.Card.ID)))
        {
            window.layoutSummoner.Clear();
        }
        if (!isVisible)
        {
            this.LocationHilite.transform.parent = base.transform;
            this.CharacterHilite.transform.parent = base.transform;
        }
        if (isVisible)
        {
            Tutorial.Notify(TutorialEventType.ScreenTempCloseShown);
        }
        if (!isVisible)
        {
            Tutorial.Notify(TutorialEventType.ScreenWasClosed);
        }
    }

    private void ZoomVillainCard(bool isZoomed)
    {
        if (this.villainCard != null)
        {
            if (isZoomed)
            {
                UI.Sound.Play(SoundEffectType.CardZoom);
                this.villainCard.Animate(AnimationType.Focus, true);
                this.villainCard.MoveCard(Vector3.zero, 0.25f).setEase(LeanTweenType.easeOutQuad);
                LeanTween.scale(this.villainCard.gameObject, Device.GetCardZoomScale(), 0.25f).setEase(LeanTweenType.easeOutQuad);
            }
            else
            {
                this.villainCard.Animate(AnimationType.Focus, false);
                this.villainCard.MoveCard(this.VillainCardMarker.transform.position, 0.25f).setEase(LeanTweenType.easeOutQuad);
                LeanTween.scale(this.villainCard.gameObject, this.VillainCardMarker.transform.localScale, 0.25f).setEase(LeanTweenType.easeOutQuad);
            }
        }
    }

    public override uint zIndex =>
        Constants.ZINDEX_PANEL_POPUP;

    [CompilerGenerated]
    private sealed class <OnCloseButtonPushed_Coroutine>c__Iterator77 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelTempClose <>f__this;
        internal bool <isLoadingRequired>__0;

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
                    this.<>f__this.isLoading = true;
                    this.<isLoadingRequired>__0 = this.<>f__this.IsLoadingRequired();
                    if (!this.<isLoadingRequired>__0)
                    {
                        break;
                    }
                    Game.UI.ShowWaitScreen(true);
                    this.$current = new WaitForSeconds(0.6f);
                    this.$PC = 1;
                    goto Label_0122;

                case 1:
                    break;

                case 2:
                    goto Label_00CF;

                case 3:
                    goto Label_010D;

                default:
                    goto Label_0120;
            }
            this.<>f__this.SetCloseAttempted(Party.Find(this.<>f__this.selectedCharacter));
            this.<>f__this.AskCloseLocation(this.<>f__this.selectedCharacter);
            if (Turn.IsSwitchingCharacters())
            {
                this.$current = new WaitForSeconds(0.2f);
                this.$PC = 2;
                goto Label_0122;
            }
        Label_00CF:
            this.<>f__this.Show(false);
            if (this.<isLoadingRequired>__0)
            {
                Game.UI.ShowWaitScreen(false);
                this.$current = new WaitForSeconds(0.6f);
                this.$PC = 3;
                goto Label_0122;
            }
        Label_010D:
            this.<>f__this.isLoading = false;
            this.$PC = -1;
        Label_0120:
            return false;
        Label_0122:
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
    private sealed class <OnDoneButtonPushed_Coroutine>c__Iterator78 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelTempClose <>f__this;
        internal bool <isLoadingRequired>__0;

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
                    this.<>f__this.isLoading = true;
                    this.<isLoadingRequired>__0 = this.<>f__this.IsLoadingRequired();
                    if (!this.<isLoadingRequired>__0)
                    {
                        break;
                    }
                    Game.UI.ShowWaitScreen(true);
                    this.$current = new WaitForSeconds(UI.WaitScreenAnimationLength);
                    this.$PC = 1;
                    goto Label_00EB;

                case 1:
                    break;

                case 2:
                    goto Label_00D6;

                default:
                    goto Label_00E9;
            }
            this.<>f__this.Show(false);
            Scenario.Current.ResetImpossibleLocationClosures();
            this.<>f__this.ClearCloseAttempted();
            Turn.Iterators.Current.End();
            if (this.<isLoadingRequired>__0)
            {
                Game.UI.ShowWaitScreen(false);
                this.$current = new WaitForSeconds(UI.WaitScreenAnimationLength);
                this.$PC = 2;
                goto Label_00EB;
            }
        Label_00D6:
            this.<>f__this.isLoading = false;
            this.$PC = -1;
        Label_00E9:
            return false;
        Label_00EB:
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

