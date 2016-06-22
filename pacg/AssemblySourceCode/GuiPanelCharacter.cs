using System;
using System.Text;
using UnityEngine;

public class GuiPanelCharacter : GuiPanelBackStack
{
    [Tooltip("reference to the avatar animator in this scene")]
    public Animator AvatarAnimator;
    [Tooltip("references to the avatar images in this scene")]
    public GameObject[] Avatars;
    [Tooltip("B or C")]
    public GuiImage CharacterDeckImage;
    [Tooltip("character name")]
    public GuiLabel CharacterNameLabel;
    [Tooltip("character traits text")]
    public GuiLabel CharacterTraitsLabel;
    [Tooltip("it says \"CHARACTER\"")]
    public GuiLabel CharacterTypeLabel;
    [Tooltip("reference to the close button in our hierarchy")]
    public GuiButton CloseButton;
    private int currentPane = 1;
    public GameObject GlowButtonVfx;
    public GameObject GlowPortraitVfx;
    public GameObject GlowTabVfx;
    [Tooltip("reference to skills sub-panel 1 in our hierarchy")]
    public GuiPanelCharacterSkills Pane1;
    [Tooltip("reference to powers sub-panel 2 in our hierarchy")]
    public GuiPanelCharacterPowers Pane2;
    [Tooltip("reference to cards sub-panel 3 in our hierarchy")]
    public GuiPanelCharacterCards Pane3;
    [Tooltip("reference to completed sub-panel 4 in our hierarchy")]
    public GuiPanelCharacterComplete Pane4;
    [Tooltip("reference to the quests sub-panel 5 in our hierarchy")]
    public GuiPanelCharacterQuests Pane5;
    [Tooltip("references to the 6 party face buttons in our hierarchy")]
    public GuiButton[] PartyButtons;
    [Tooltip("sound played when a new character is displayed")]
    public AudioClip SelectCharacterSound;
    [Tooltip("pointer to the B graphic used for deck type")]
    public Sprite SetB;
    [Tooltip("pointer to the C graphic used for deck type")]
    public Sprite SetC;
    [Tooltip("references to the 4 tab buttons in our hierarchy")]
    public GuiButton[] TabButtons;

    private void CloseSubWindows()
    {
        this.Pane2.RolePanel.Show(false);
    }

    private Vector3 GetLayoutPosition(GuiButton button, int n, int max)
    {
        float num = 1.5f;
        float num2 = -(((float) max) / 2f) * num;
        return new Vector3(num2 + (n * num), button.transform.localPosition.y, button.transform.localPosition.z);
    }

    private static string GetTraitText(Character character)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine(character.Gender.ToText().ToUpper());
        builder.AppendLine(character.Race.ToText().ToUpper());
        builder.AppendLine(character.Class.ToText().ToUpper());
        return builder.ToString();
    }

    public override void Initialize()
    {
        this.SetupHeadingText();
        this.SetupTabText();
        this.Pane1.Initialize();
        this.Pane2.Initialize();
        this.Pane3.Initialize();
        this.Pane4.Initialize();
        this.Pane5.Initialize();
        this.Show(false);
    }

    private void OnCharacter1ButtonPushed()
    {
        if (!base.Paused)
        {
            this.SelectCharacter(0);
            Tutorial.Notify(TutorialEventType.PanelCharacterSheetCharacter1);
        }
    }

    private void OnCharacter2ButtonPushed()
    {
        if (!base.Paused)
        {
            this.SelectCharacter(1);
            Tutorial.Notify(TutorialEventType.PanelCharacterSheetCharacter2);
        }
    }

    private void OnCharacter3ButtonPushed()
    {
        if (!base.Paused)
        {
            this.SelectCharacter(2);
            Tutorial.Notify(TutorialEventType.PanelCharacterSheetCharacter3);
        }
    }

    private void OnCharacter4ButtonPushed()
    {
        if (!base.Paused)
        {
            this.SelectCharacter(3);
            Tutorial.Notify(TutorialEventType.PanelCharacterSheetCharacter4);
        }
    }

    private void OnCharacter5ButtonPushed()
    {
        if (!base.Paused)
        {
            this.SelectCharacter(4);
            Tutorial.Notify(TutorialEventType.PanelCharacterSheetCharacter5);
        }
    }

    private void OnCharacter6ButtonPushed()
    {
        if (!base.Paused)
        {
            this.SelectCharacter(5);
            Tutorial.Notify(TutorialEventType.PanelCharacterSheetCharacter6);
        }
    }

    private void OnCloseButtonPushed()
    {
        if (!base.Paused && !this.CloseButton.Locked)
        {
            this.Show(false);
            if (base.Owner != null)
            {
                base.Owner.Pause(false);
                base.Owner.Show(true);
                base.Owner = null;
            }
            else
            {
                UI.Window.Pause(false);
                UI.Window.Show(true);
            }
        }
    }

    private void OnMenuButtonPushed()
    {
        if (!base.Paused)
        {
            this.CloseSubWindows();
            Game.UI.OptionsPanel.Owner = this;
            Game.UI.OptionsPanel.Show(true);
        }
    }

    private void OnPane1ButtonPushed()
    {
        if (!base.Paused)
        {
            this.currentPane = 1;
            this.Pane1.Show(true);
            this.Pane2.Show(false);
            this.Pane2.RolePanel.gameObject.SetActive(false);
            this.Pane3.Show(false);
            this.Pane4.Show(false);
            this.Pane5.Show(false);
            Tutorial.Notify(TutorialEventType.PanelCharacterSheetSkillsShown);
        }
    }

    private void OnPane2ButtonPushed()
    {
        if (!base.Paused)
        {
            this.currentPane = 2;
            this.Pane1.Show(false);
            this.Pane2.Show(true);
            this.Pane2.Refresh();
            this.Pane2.RolePanel.gameObject.SetActive(true);
            this.Pane3.Show(false);
            this.Pane4.Show(false);
            this.Pane5.Show(false);
            Tutorial.Notify(TutorialEventType.PanelCharacterSheetPowersShown);
        }
    }

    private void OnPane3ButtonPushed()
    {
        if (!base.Paused)
        {
            this.currentPane = 3;
            this.Pane1.Show(false);
            this.Pane2.Show(false);
            this.Pane2.RolePanel.gameObject.SetActive(false);
            this.Pane3.Show(true);
            this.Pane4.Show(false);
            this.Pane5.Show(false);
            Tutorial.Notify(TutorialEventType.PanelCharacterSheetCardsShown);
        }
    }

    private void OnPane4ButtonPushed()
    {
        if (!base.Paused)
        {
            if (!Rules.IsQuestRewardAllowed())
            {
                this.currentPane = 4;
                this.Pane1.Show(false);
                this.Pane2.Show(false);
                this.Pane2.RolePanel.gameObject.SetActive(false);
                this.Pane3.Show(false);
                this.Pane4.Show(true);
                this.Pane5.Show(false);
                Tutorial.Notify(TutorialEventType.PanelCharacterSheetCompleteShown);
            }
            else
            {
                this.currentPane = 5;
                this.Pane1.Show(false);
                this.Pane2.Show(false);
                this.Pane2.RolePanel.gameObject.SetActive(false);
                this.Pane3.Show(false);
                this.Pane4.Show(false);
                this.Pane5.Show(true);
                Tutorial.Notify(TutorialEventType.PanelCharacterSheetQuestsShown);
            }
        }
    }

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        this.Pane1.Pause(isPaused);
        this.Pane2.Pause(isPaused);
        this.Pane3.Pause(isPaused);
        this.Pane4.Pause(isPaused);
        this.Pane5.Pause(isPaused);
    }

    public void Refresh(Character c)
    {
        for (int i = 0; i < this.Avatars.Length; i++)
        {
            this.Avatars[i].SetActive(false);
        }
        for (int j = 0; j < this.Avatars.Length; j++)
        {
            if (this.Avatars[j].name == c.ID)
            {
                this.Avatars[j].SetActive(true);
                break;
            }
        }
        this.CharacterNameLabel.Text = c.DisplayName.ToUpper();
        this.CharacterTraitsLabel.Text = GetTraitText(c);
        this.CharacterDeckImage.Image = (c.Set != "C") ? this.SetB : this.SetC;
        this.Pane1.Character = c;
        this.Pane2.Character = c;
        this.Pane3.Character = c;
        this.Pane4.Character = c;
        this.Pane5.Character = c;
    }

    private void SelectCharacter(int n)
    {
        this.Refresh(Party.Characters[n]);
        if (this.currentPane == 1)
        {
            this.Pane1.Refresh();
        }
        this.Pane2.Show(this.currentPane == 2);
        this.Pane2.Refresh();
        if (this.currentPane == 3)
        {
            this.Pane3.Refresh();
        }
        if (this.currentPane == 4)
        {
            this.Pane4.Refresh();
        }
        if (this.currentPane == 5)
        {
            this.Pane5.Refresh();
        }
        this.AvatarAnimator.SetTrigger("SwitchCharacter");
        UI.Sound.Play(this.SelectCharacterSound);
    }

    public void SelectPower(string id)
    {
        this.OnPane2ButtonPushed();
        this.Pane2.Focus(id);
    }

    private void SetupHeadingText()
    {
        this.CharacterTypeLabel.Text = "CHARACTER";
    }

    private void SetupPartyButtons()
    {
        for (int i = 0; i < this.PartyButtons.Length; i++)
        {
            if (i < Party.Characters.Count)
            {
                this.PartyButtons[i].Image = Party.Characters[i].PortraitSmall;
                this.PartyButtons[i].transform.position = this.GetLayoutPosition(this.PartyButtons[i], i + 1, Party.Characters.Count);
                this.PartyButtons[i].Refresh();
            }
            else
            {
                this.PartyButtons[i].Show(false);
            }
        }
    }

    private void SetupTabText()
    {
        if (this.TabButtons.Length > 3)
        {
            if (Game.GameMode == GameModeType.Quest)
            {
                this.TabButtons[3].Text = "ADVANCEMENT";
            }
            else
            {
                this.TabButtons[3].Text = UI.Text(0x148);
            }
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            UI.Sound.Play(SoundEffectType.CharacterMenuOpen);
            this.Refresh(Turn.Character);
            this.SetupPartyButtons();
            this.OnPane1ButtonPushed();
            Tutorial.Notify(TutorialEventType.ScreenCharacterSheetShown);
        }
        else
        {
            Tutorial.Notify(TutorialEventType.ScreenWasClosed);
            this.Pane1.Show(false);
            this.Pane2.Show(false);
            this.Pane3.Show(false);
            this.Pane4.Show(false);
            this.Pane5.Show(false);
        }
        base.ShowWindowButtons(!isVisible);
    }

    public override bool Fullscreen =>
        true;
}

