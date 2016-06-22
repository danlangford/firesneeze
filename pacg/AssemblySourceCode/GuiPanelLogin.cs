using System;
using UnityEngine;

public class GuiPanelLogin : GuiPanelBackStack
{
    private Animator activeAnimator;
    private GuiLabel activeLabel;
    [Tooltip("reference to the quit button on this panel")]
    public GuiButton CloseButton;
    [Tooltip("reference to the close button on this panel")]
    public GuiButton CornerCloseButton;
    private string email = string.Empty;
    [Tooltip("reference to the email label on this panel")]
    public GuiLabel Email;
    [Tooltip("reference to the email switch input  button on this panel")]
    public GuiButton EmailButton;
    [Tooltip("reference to the background sprite highlight for the email button")]
    public SpriteRenderer EmailButtonHighlight;
    [Tooltip("reference to the email field I beam animator")]
    public Animator EmailIBeam;
    [Tooltip("reference to the forgot password button on this panel")]
    public GuiButton ForgotButton;
    private TouchScreenKeyboard keyboard;
    [Tooltip("reference to the close button on this panel")]
    public GuiButton LoginButton;
    [Tooltip("reference to the message label on this panel")]
    public GuiLabel MessageLabel;
    [Tooltip("reference to the \"no connection\" error string")]
    public StrRefType NoConnectionString;
    private string password = string.Empty;
    [Tooltip("reference to the first password label on this panel")]
    public GuiLabel Password;
    [Tooltip("reference to the first password switch input  button on this panel")]
    public GuiButton PasswordButton;
    [Tooltip("reference to the background sprite highlight for the password button")]
    public SpriteRenderer PasswordButtonHighlight;
    [Tooltip("reference to the password field I beam animator")]
    public Animator PasswordIBeam;

    private void ChangeActiveLabel()
    {
        if (this.activeLabel == this.Email)
        {
            this.SetAnimator(this.PasswordIBeam);
            this.activeLabel = this.Password;
            this.SetHighlight(this.PasswordButtonHighlight);
        }
        else if (this.activeLabel == this.Password)
        {
            this.SetAnimator(this.EmailIBeam);
            this.activeLabel = this.Email;
            this.SetHighlight(this.EmailButtonHighlight);
        }
        this.CheckKeyboard();
    }

    private void CheckKeyboard()
    {
        if ((Device.GetIsIphone() || Device.GetIsAndroid()) && ((this.keyboard == null) || ((this.keyboard != null) && !this.keyboard.active)))
        {
            TouchScreenKeyboard.hideInput = false;
            this.keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.Default, false, false, false, false, string.Empty);
        }
    }

    public override void Clear()
    {
        this.Email.Text = string.Empty;
        this.Password.Text = string.Empty;
        this.MessageLabel.Text = string.Empty;
    }

    private void InputLoopDesktop()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            this.ChangeActiveLabel();
        }
        else if (Input.inputString != null)
        {
            for (int i = 0; i < Input.inputString.Length; i++)
            {
                char ch = Input.inputString[i];
                switch (ch)
                {
                    case '\b':
                        if ((this.activeLabel == this.Email) && (this.email.Length > 0))
                        {
                            this.email = this.email.Substring(0, this.email.Length - 1);
                            this.SetAnimator(this.EmailIBeam);
                            this.activeLabel.Text = this.email;
                        }
                        else if ((this.activeLabel == this.Password) && (this.password.Length > 0))
                        {
                            this.password = this.password.Substring(0, this.password.Length - 1);
                            string str = string.Empty;
                            for (int j = 0; j < this.password.Length; j++)
                            {
                                str = str + "*";
                            }
                            this.SetAnimator(this.PasswordIBeam);
                            this.activeLabel.Text = str;
                        }
                        break;

                    case '\n':
                    case '\r':
                        this.OnLoginButtonPushed();
                        break;

                    default:
                        if (this.activeLabel == this.Email)
                        {
                            if (this.email.Length < Constants.MAX_EMAIL_LENGTH)
                            {
                                this.email = this.email + ch;
                            }
                            this.SetAnimator(this.EmailIBeam);
                            this.activeLabel.Text = this.email;
                        }
                        else if (this.activeLabel == this.Password)
                        {
                            if (this.password.Length < Constants.MAX_PASSWORD_LENGTH)
                            {
                                this.password = this.password + ch;
                            }
                            string str2 = string.Empty;
                            for (int k = 0; k < this.password.Length; k++)
                            {
                                str2 = str2 + "*";
                            }
                            this.SetAnimator(this.PasswordIBeam);
                            this.activeLabel.Text = str2;
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
                if (this.keyboard.wasCanceled)
                {
                }
            }
            else if (this.keyboard.active && !UI.Busy)
            {
                if (this.activeLabel == this.Password)
                {
                    if (this.keyboard.text.Length > Constants.MAX_PASSWORD_LENGTH)
                    {
                        this.keyboard.text = this.keyboard.text.Substring(0, Constants.MAX_PASSWORD_LENGTH);
                    }
                    string str = string.Empty;
                    for (int i = 0; i < this.keyboard.text.Length; i++)
                    {
                        str = str + "*";
                    }
                    this.SetAnimator(this.PasswordIBeam);
                    this.activeLabel.Text = str;
                    this.password = this.keyboard.text;
                }
                else if (this.activeLabel == this.Email)
                {
                    if (this.keyboard.text.Length > Constants.MAX_EMAIL_LENGTH)
                    {
                        this.keyboard.text = this.keyboard.text.Substring(0, Constants.MAX_EMAIL_LENGTH);
                    }
                    string str2 = string.Empty;
                    for (int j = 0; j < this.keyboard.text.Length; j++)
                    {
                        str2 = str2 + this.keyboard.text[j];
                    }
                    this.SetAnimator(this.EmailIBeam);
                    this.activeLabel.Text = str2;
                    this.email = this.keyboard.text;
                }
            }
        }
    }

    private void Login()
    {
        PlayFabLoginCalls.LoginWithEmail(this.Email.Text, this.password);
    }

    private void OnCloseButtonPushed()
    {
        if (!UI.Busy)
        {
            this.Show(false);
        }
    }

    private void OnEmailButtonPushed()
    {
        UI.Busy = true;
        if ((this.keyboard != null) && this.keyboard.active)
        {
            this.keyboard.text = this.email;
        }
        this.activeLabel = this.Email;
        this.SetAnimator(this.EmailIBeam);
        this.SetHighlight(this.EmailButtonHighlight);
        UI.Busy = false;
        this.CheckKeyboard();
    }

    private void OnForgotButtonPushed()
    {
        if (!UI.Busy)
        {
            if ((this.Email.Text.Length > 5) && this.Email.Text.Contains("@"))
            {
                PlayFabLoginCalls.SendAccountRecoveryEmail(this.Email.Text);
                SetMessage("Recovery instructions sent to email!");
            }
            else
            {
                SetMessage("Invalid email!");
            }
        }
    }

    private void OnLoginButtonPushed()
    {
        if (!UI.Busy)
        {
            if (!Game.Network.HasNetworkConnection)
            {
                SetMessage(this.NoConnectionString.ToString());
            }
            else if (this.InputIsValid)
            {
                this.Login();
            }
        }
    }

    private void OnPasswordButtonPushed()
    {
        UI.Busy = true;
        if ((this.keyboard != null) && this.keyboard.active)
        {
            this.keyboard.text = this.password;
        }
        this.activeLabel = this.Password;
        this.SetAnimator(this.PasswordIBeam);
        this.SetHighlight(this.PasswordButtonHighlight);
        UI.Busy = false;
        this.CheckKeyboard();
    }

    public override void Rebind()
    {
        this.CornerCloseButton.Rebind();
        this.CloseButton.Rebind();
        this.LoginButton.Rebind();
        this.EmailButton.Rebind();
        this.PasswordButton.Rebind();
        this.ForgotButton.Rebind();
    }

    public override void Refresh()
    {
        this.Clear();
    }

    private void SetAnimator(Animator animator)
    {
        this.activeAnimator = animator;
        if (this.EmailIBeam != null)
        {
            this.EmailIBeam.gameObject.SetActive(this.EmailIBeam == this.activeAnimator);
        }
        if (this.PasswordIBeam != null)
        {
            this.PasswordIBeam.gameObject.SetActive(this.PasswordIBeam == this.activeAnimator);
        }
    }

    private void SetHighlight(SpriteRenderer sr)
    {
        this.PasswordButtonHighlight.gameObject.SetActive(sr == this.PasswordButtonHighlight);
        this.EmailButtonHighlight.gameObject.SetActive(sr == this.EmailButtonHighlight);
    }

    public static void SetMessage(string str)
    {
        GuiWindow current = GuiWindow.Current;
        if (current != null)
        {
            GuiWindowMainMenu menu = current as GuiWindowMainMenu;
            if (menu != null)
            {
                menu.loginPanel.MessageLabel.Text = str;
            }
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (base.Owner != null)
        {
            base.Owner.Pause(isVisible);
        }
        else
        {
            UI.Window.Pause(isVisible);
        }
        if (!isVisible)
        {
            base.Owner = null;
            this.email = string.Empty;
            this.password = string.Empty;
            if ((this.keyboard != null) && this.keyboard.active)
            {
                this.keyboard.active = false;
            }
        }
        else
        {
            if (Device.GetIsIphone() || Device.GetIsAndroid())
            {
                TouchScreenKeyboard.hideInput = true;
                this.keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.Default, false, false, false, false, string.Empty);
            }
            this.Refresh();
            UI.Busy = false;
            this.activeLabel = this.Email;
            this.SetAnimator(this.EmailIBeam);
            this.SetHighlight(this.EmailButtonHighlight);
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

    private bool InputIsValid
    {
        get
        {
            this.Email.Text = this.Email.Text.Trim();
            if (!RegexUtilities.IsEmail(this.Email.Text))
            {
                SetMessage("Email is invalid!");
                return false;
            }
            if (this.Password.Text.Length < 6)
            {
                SetMessage("Password must be 6 characters or more!");
                return false;
            }
            return true;
        }
    }
}

