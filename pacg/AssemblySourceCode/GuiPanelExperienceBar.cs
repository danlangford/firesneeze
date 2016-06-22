using System;
using UnityEngine;

public class GuiPanelExperienceBar : GuiPanel
{
    [Tooltip("reference to the scaled xp bar in our hierarchy")]
    public Transform Bar;
    [Tooltip("referecne to the xp bar root in the scene")]
    public GameObject BarRoot;
    [Tooltip("reference to the cards button on the menu")]
    public GuiButton CardsButton;
    private CharacterToken currentToken;
    [Tooltip("reference to the delete button on the menu")]
    public GuiButton DeleteButton;
    private bool isRosterMenuShowing;
    [Tooltip("reference to the character level label")]
    public GuiLabel LevelLabel;
    [Tooltip("reference to our button that opens the roster popup menu")]
    public GuiButton MenuButton;
    [Tooltip("reference to the nickname button on the menu")]
    public GuiButton NicknameButton;
    [Tooltip("reference to the nickname panel in the scene")]
    public GuiPanelVaultNickname NicknamePanel;
    [Tooltip("reference to the \"in a party\" label on the menu")]
    public GuiLabel PartyStatusLabel;
    [Tooltip("reference to the yes/no ask menu in the scene")]
    public GuiPanelMenuAsk Popup;
    [Tooltip("reference to the current vs next level label")]
    public GuiLabel RatioLabel;
    [Tooltip("reference to the roster menu animator")]
    public Animator RosterMenuAnimator;
    [Tooltip("reference to the card tray in the scene")]
    public GuiLayoutTray Tray;

    private void Delete_No_Callback()
    {
        UI.Sound.Play(SoundEffectType.GenericLayoutTrayClose);
        this.Popup.Show(false);
    }

    private void Delete_Yes_Callback()
    {
        if (this.currentToken != null)
        {
            GuiWindowCreateParty window = UI.Window as GuiWindowCreateParty;
            if (window != null)
            {
                window.VaultPanel.HighlightToken(null);
            }
            Vault.Remove(this.currentToken.Character.NickName);
            this.currentToken.Select(false);
            UnityEngine.Object.DestroyImmediate(this.currentToken.gameObject);
            UI.Window.Refresh();
        }
        UI.Sound.Play(SoundEffectType.GenericLayoutTrayClose);
        this.Popup.Show(false);
    }

    public override void Initialize()
    {
        this.NicknamePanel.Initialize();
        this.Tray.Initialize();
        this.Tray.Show(false);
        base.Show(false);
    }

    private bool IsTokenInVault(CharacterToken token)
    {
        if (token == null)
        {
            return false;
        }
        if (token.Character == null)
        {
            return false;
        }
        return Vault.Contains(token.Character.NickName);
    }

    private bool IsTokenLocked(CharacterToken token)
    {
        if (token == null)
        {
            return false;
        }
        if (token.Character == null)
        {
            return false;
        }
        return Vault.IsLocked(token.Character.NickName);
    }

    private bool IsTokenSelected(CharacterToken token)
    {
        if (token == null)
        {
            return false;
        }
        if (token.Slot == null)
        {
            return false;
        }
        return (token.Slot is CharacterTokenSlotPartyMember);
    }

    private void OnCardsButtonPushed()
    {
        if (this.currentToken != null)
        {
            if (this.currentToken.Character.Deck.Count <= 0)
            {
                this.currentToken.Character.BuildDeck();
            }
            for (int i = 0; i < this.currentToken.Character.Deck.Count; i++)
            {
                this.Tray.Position(this.currentToken.Character.Deck[i]);
            }
            this.Tray.Margins = Device.GetMarginSize();
            this.Tray.Modal = true;
            this.Tray.Deck = this.currentToken.Character.Deck;
            this.Tray.CardAction = ActionType.Share;
            this.Tray.Show(true);
        }
        this.ShowRosterMenu(false);
    }

    private void OnCloseButtonPushed()
    {
        this.ShowRosterMenu(false);
    }

    private void OnDeleteButtonPushed()
    {
        this.ShowRosterMenu(false);
        this.Popup.Owner = this;
        UI.Sound.Play(SoundEffectType.GenericLayoutTrayOpen);
        this.Popup.Show(true);
        this.Popup.MessageText = "Delete this character?";
        this.Popup.YesButtonText = UI.Text(0x12e);
        this.Popup.YesButtonCallback = "Delete_Yes_Callback";
        this.Popup.NoButtonText = UI.Text(0x12f);
        this.Popup.NoButtonCallback = "Delete_No_Callback";
    }

    private void OnNicknameButtonPushed()
    {
        this.NicknamePanel.Show(this.currentToken);
        this.ShowRosterMenu(false);
    }

    private void OnRosterButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.ShowRosterMenu(true);
        }
    }

    public override void Refresh()
    {
        if (this.currentToken != null)
        {
            bool flag = (!this.IsTokenSelected(this.currentToken) && this.IsTokenInVault(this.currentToken)) && !this.IsTokenLocked(this.currentToken);
            this.DeleteButton.Disable(!flag);
            bool flag2 = !this.IsTokenLocked(this.currentToken);
            this.NicknameButton.Disable(!flag2);
            this.PartyStatusLabel.Show(this.IsTokenLocked(this.currentToken));
        }
    }

    public void Show(CharacterToken token)
    {
        base.Show(true);
        this.ShowRosterMenuButton(true);
        this.currentToken = token;
        int num = token.Character.XP - Rules.GetExperiencePointsForLevel(token.Character.Level);
        int experiencePointsForLevel = Rules.GetExperiencePointsForLevel(token.Character.Level + 1);
        int num3 = Rules.GetExperiencePointsForLevel(token.Character.Level + 1) - Rules.GetExperiencePointsForLevel(token.Character.Level);
        float x = Mathf.Clamp01(((float) num) / ((float) num3));
        this.LevelLabel.Text = token.Character.Level.ToString();
        object[] objArray1 = new object[] { token.Character.XP, " / ", experiencePointsForLevel, " XP" };
        this.RatioLabel.Text = string.Concat(objArray1);
        this.Bar.localScale = new Vector3(x, this.Bar.localScale.y, this.Bar.localScale.z);
        this.BarRoot.SetActive(Game.GameMode == GameModeType.Quest);
        this.ShowRosterMenu(false);
    }

    private void ShowRosterMenu(bool isVisible)
    {
        if (isVisible && !this.isRosterMenuShowing)
        {
            this.RosterMenuAnimator.SetTrigger("Open");
            this.isRosterMenuShowing = true;
            UI.Sound.Play(SoundEffectType.GenericLayoutTrayOpen);
            LeanTween.delayedCall(0.05f, new Action(this.Refresh));
        }
        if (!isVisible && this.isRosterMenuShowing)
        {
            this.RosterMenuAnimator.SetTrigger("Close");
            UI.Sound.Play(SoundEffectType.GenericLayoutTrayClose);
            this.isRosterMenuShowing = false;
        }
    }

    private void ShowRosterMenuButton(bool isVisible)
    {
        Transform parent = this.MenuButton.transform.parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(isVisible);
        }
    }
}

