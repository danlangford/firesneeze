using System;
using UnityEngine;

public class GuiPanelParty : GuiPanel
{
    [Tooltip("reference to the hi-lights of active characters during targeting")]
    public SpriteRenderer[] activeHilites;
    [Tooltip("reference to the large portrait in the UI frame")]
    public SpriteRenderer avatarPortrait;
    [Tooltip("reference to the bar background in the UI frame")]
    public SpriteRenderer background;
    [Tooltip("pointer to the sprites for backgrounds based on # chars in party (1-6)")]
    public Sprite[] backgroundSprites;
    [Tooltip("reference to the ! icons in our hierarchy")]
    public GameObject[] characterAidMarks;
    [Tooltip("reference to character button array in our hierarchy")]
    public GuiButton[] characterButtons;
    [Tooltip("reference to the icons in our hierarchy that show of the character is in the same location as the primary character")]
    public GameObject[] characterHereMarks;
    [Tooltip("the prefab used to hilight the current character")]
    public GameObject characterHilite;
    private GuiPanelDice dicePanel;
    [Tooltip("reference to the \"ask for help\" button in our hierarchy")]
    public GuiButton helpButton;
    private int lastTextUpdate;

    private CharacterPower GetShortcutPower(Character character)
    {
        for (int i = 0; i < character.Powers.Count; i++)
        {
            if (character.Powers[i].IsShortcutAvailable())
            {
                return character.Powers[i];
            }
        }
        return null;
    }

    public override void Initialize()
    {
        this.background.sprite = this.backgroundSprites[Party.Characters.Count];
        for (int i = 0; i < this.characterButtons.Length; i++)
        {
            if (i < Party.Characters.Count)
            {
                this.characterButtons[i].Image = Party.Characters[i].PortraitSmall;
            }
            else
            {
                this.characterButtons[i].Show(false);
            }
        }
        for (int j = 0; j < this.characterAidMarks.Length; j++)
        {
            this.characterAidMarks[j].SetActive(false);
        }
        for (int k = 0; k < this.characterHereMarks.Length; k++)
        {
            this.characterHereMarks[k].SetActive(false);
        }
        if (Game.GameType == GameType.LocalSinglePlayer)
        {
            this.helpButton.Show(false);
        }
        if (Game.GameType == GameType.LocalMultiPlayer)
        {
            this.helpButton.Text = UI.Text(0x1b6);
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            this.dicePanel = window.dicePanel;
        }
    }

    private bool IsInCheck()
    {
        if (this.dicePanel.Rolling)
        {
            return false;
        }
        if ((!Rules.IsCombatCheck() && !Rules.IsNonCombatCheck()) && !Rules.IsDamageCheck())
        {
            return false;
        }
        return true;
    }

    private bool IsSelectionValid(int n)
    {
        if (UI.Window.Paused)
        {
            return false;
        }
        if (UI.Busy)
        {
            return false;
        }
        if (n >= Party.Characters.Count)
        {
            return false;
        }
        if (!Party.Characters[n].Alive)
        {
            return false;
        }
        return true;
    }

    private void OnCharacter1ButtonPushed()
    {
        this.SelectCharacter(0);
    }

    private void OnCharacter2ButtonPushed()
    {
        this.SelectCharacter(1);
    }

    private void OnCharacter3ButtonPushed()
    {
        this.SelectCharacter(2);
    }

    private void OnCharacter4ButtonPushed()
    {
        this.SelectCharacter(3);
    }

    private void OnCharacter5ButtonPushed()
    {
        this.SelectCharacter(4);
    }

    private void OnCharacter6ButtonPushed()
    {
        this.SelectCharacter(5);
    }

    private void OnHelpButtonPushed()
    {
        if ((!UI.Window.Paused && !UI.Busy) && (Game.GameType == GameType.LocalMultiPlayer))
        {
            this.helpButton.Show(false);
            if (Turn.Number == Turn.Current)
            {
                Turn.SwitchType = SwitchType.AidAll;
                Turn.Iterators.Start(TurnStateIteratorType.Aid);
            }
            else
            {
                Turn.SwitchType = SwitchType.Aid;
                Game.UI.SwitchPanel.Show(true);
            }
        }
    }

    public override void Refresh()
    {
        LeanTween.moveLocal(this.characterHilite, this.characterButtons[Turn.Current].transform.localPosition, 0.2f).setEase(LeanTweenType.easeInOutQuad);
        if (this.avatarPortrait != null)
        {
            this.avatarPortrait.sprite = Party.Characters[Turn.Number].PortraitAvatar;
        }
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (!Party.Characters[i].Alive)
            {
                this.characterButtons[i].Image = Party.Characters[i].PortraitSmallDead;
            }
            this.characterHereMarks[i].SetActive(((Party.Characters[i].Location == Turn.Owner.Location) && (i != Turn.Current)) && Party.Characters[i].Alive);
        }
    }

    private void RefreshHelpButton(bool isHelpButtonAvailable, bool isInCheck)
    {
        this.helpButton.Show(isHelpButtonAvailable);
        if (isHelpButtonAvailable && (Turn.Number != this.lastTextUpdate))
        {
            this.lastTextUpdate = Turn.Number;
            if (Turn.Number == Turn.Current)
            {
                this.helpButton.Text = UI.Text(0x1b6);
            }
            else if (isInCheck)
            {
                this.helpButton.Text = UI.Text(360);
            }
            else
            {
                this.helpButton.Text = UI.Text(0x1df);
            }
        }
    }

    private void SelectCharacter(int n)
    {
        if ((((this.IsSelectionValid(n) && (Turn.Number != n)) && (Turn.SwitchType != SwitchType.AidAll)) && (Turn.SwitchType != SwitchType.Aid)) && (Turn.State != GameStateType.Share))
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.UnZoomCard();
                window.DropCard();
            }
            Turn.Number = n;
            if (window != null)
            {
                window.Refresh();
            }
            Turn.Refresh();
            if (Turn.Map)
            {
                window.mapPanel.Seek(Turn.Character.Location);
            }
            Tutorial.Notify(TutorialEventType.TurnCharacterChanged);
        }
    }

    public void ShowHighlights(bool visible)
    {
        for (int i = 0; i < this.activeHilites.Length; i++)
        {
            this.activeHilites[i].enabled = visible;
        }
    }

    private void Update()
    {
        bool isHelpButtonAvailable = false;
        bool isInCheck = this.IsInCheck();
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Turn.Current == i)
            {
                this.characterAidMarks[i].SetActive(false);
            }
            else
            {
                bool flag3 = Party.Characters[i].CanPlayCard() || Party.Characters[i].CanPlayPower();
                bool flag4 = isInCheck && flag3;
                this.characterAidMarks[i].SetActive(flag4);
                if (flag3)
                {
                    isHelpButtonAvailable = isInCheck || (Turn.Number != Turn.Current);
                }
            }
        }
        if (((Game.GameType == GameType.LocalMultiPlayer) && (Turn.SwitchType == SwitchType.None)) && !Turn.IsSwitchingCharacters())
        {
            this.RefreshHelpButton(isHelpButtonAvailable, isInCheck);
        }
    }
}

