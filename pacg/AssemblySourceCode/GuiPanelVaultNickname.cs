using System;
using UnityEngine;

public class GuiPanelVaultNickname : GuiPanelBackStack
{
    private CharacterToken currentToken;
    private TouchScreenKeyboard keyboard;
    [Tooltip("reference to the nickname label in our hierarchy")]
    public GuiLabel Nickname;

    public override void Initialize()
    {
        this.Show(false);
    }

    private void InputLoopDesktop()
    {
        if (Input.inputString != null)
        {
            for (int i = 0; i < Input.inputString.Length; i++)
            {
                char ch = Input.inputString[i];
                switch (ch)
                {
                    case '\b':
                        if (this.Nickname.Text.Length > 0)
                        {
                            this.Nickname.Text = this.Nickname.Text.Substring(0, this.Nickname.Text.Length - 1);
                        }
                        break;

                    case '\n':
                    case '\r':
                        this.OnYesButtonPushed();
                        break;

                    default:
                        if (this.Nickname.Text.Length < Constants.MAX_NICKNAME_LENGTH)
                        {
                            this.Nickname.Text = this.Nickname.Text + ch;
                        }
                        break;
                }
            }
        }
    }

    private void InputLoopMobile()
    {
        if (this.keyboard != null)
        {
            if (this.keyboard.done)
            {
                if (!this.keyboard.wasCanceled)
                {
                    this.OnYesButtonPushed();
                }
            }
            else if (this.keyboard.active)
            {
                if ((this.keyboard.text != null) && (this.keyboard.text.Length > Constants.MAX_NICKNAME_LENGTH))
                {
                    this.keyboard.text = this.keyboard.text.Substring(0, Constants.MAX_NICKNAME_LENGTH);
                }
                this.Nickname.Text = this.keyboard.text;
            }
        }
    }

    private void OnCloseButtonPushed()
    {
        this.OnNoButtonPushed();
    }

    private void OnNoButtonPushed()
    {
        this.currentToken = null;
        this.Show(false);
    }

    private void OnYesButtonPushed()
    {
        if (this.currentToken != null)
        {
            if (Vault.Contains(this.Nickname.Text))
            {
                return;
            }
            Vault.Remove(this.currentToken.Character.NickName);
            this.currentToken.Character.NickName = this.Nickname.Text;
            this.currentToken.Text.Name.Text = this.Nickname.Text;
            Vault.Add(this.currentToken.Character.NickName, this.currentToken.Character);
        }
        this.currentToken = null;
        this.Show(false);
    }

    public void Show(CharacterToken token)
    {
        this.currentToken = token;
        this.Show(true);
        if (Device.GetIsIphone() || Device.GetIsAndroid())
        {
            TouchScreenKeyboard.hideInput = true;
            this.keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.Default, false, false, false, false, string.Empty);
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (!isVisible)
        {
            UI.Sound.Play(SoundEffectType.GenericLayoutTrayClose);
            if (this.keyboard != null)
            {
                this.keyboard.active = false;
            }
        }
        else
        {
            UI.Sound.Play(SoundEffectType.GenericLayoutTrayOpen);
        }
    }

    protected override void Update()
    {
        if (Device.GetIsIphone() || Device.GetIsAndroid())
        {
            this.InputLoopMobile();
        }
        else
        {
            this.InputLoopDesktop();
        }
        base.Update();
    }

    public override bool Fullscreen =>
        true;
}

