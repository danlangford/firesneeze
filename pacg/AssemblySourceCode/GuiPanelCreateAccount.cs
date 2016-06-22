using System;
using UnityEngine;

public class GuiPanelCreateAccount : GuiPanelBackStack
{
    private Animator activeAnimator;
    private GuiLabel activeLabel;
    [Tooltip("reference to the quit button on this panel")]
    public GuiButtonMovable CloseButton;
    [Tooltip("reference to the close button on this panel")]
    public GuiButtonMovable CornerCloseButton;
    private string email = string.Empty;
    [Tooltip("reference to the email label on this panel")]
    public GuiLabel Email;
    [Tooltip("reference to the email switch input  button on this panel")]
    public GuiButtonMovable EmailButton;
    [Tooltip("reference to the highlight renderer for the email button on this panel")]
    public SpriteRenderer EmailButtonHighlight;
    [Tooltip("reference to the email field I beam animator")]
    public Animator EmailIBeam;
    private string firstPassword = string.Empty;
    [Tooltip("reference to the first password label on this panel")]
    public GuiLabel FirstPassword;
    [Tooltip("reference to the first password switch input  button on this panel")]
    public GuiButtonMovable FirstPasswordButton;
    [Tooltip("reference to the highlight renderer for the first password button on this panel")]
    public SpriteRenderer FirstPasswordButtonHighlight;
    [Tooltip("reference to the first password field I beam animator")]
    public Animator FirstPasswordIBeam;
    private TouchScreenKeyboard keyboard;
    [Tooltip("reference to where the panel should be when a mobile keyboard is active")]
    public Transform KeyboardLocation;
    [Tooltip("reference to the close button on this panel")]
    public GuiButtonMovable LoginButton;
    [Tooltip("reference to the message label on this panel")]
    public GuiLabel MessageLabel;
    [Tooltip("reference to the \"no connection\" error string")]
    public StrRefType NoConnectionString;
    [Tooltip("reference to where the panel should be when a mobile keyboard is NOT active")]
    public Transform NoKeyboardLocation;
    private string secondPassord = string.Empty;
    [Tooltip("reference to the second password label on this panel")]
    public GuiLabel SecondPassword;
    [Tooltip("reference to the second password switch input  button on this panel")]
    public GuiButtonMovable SecondPasswordButton;
    [Tooltip("reference to the highlight renderer for the second password button on this panel")]
    public SpriteRenderer SecondPasswordButtonHighlight;
    [Tooltip("reference to the second password field I beam animator")]
    public Animator SecondPasswordIBeam;
    private string username = string.Empty;
    [Tooltip("reference to the name label on this panel")]
    public GuiLabel Username;
    [Tooltip("reference to the username switch input button on this panel")]
    public GuiButtonMovable UsernameButton;
    [Tooltip("reference to the highlight renderer for the username button on this panel")]
    public SpriteRenderer UsernameButtonHighlight;
    [Tooltip("reference to the username field I beam animator")]
    public Animator UsernameIBeam;

    private void ChangeActiveLabel()
    {
        if (this.activeLabel == this.Username)
        {
            this.activeLabel = this.Email;
            this.SetHighlight(this.EmailButtonHighlight);
        }
        else if (this.activeLabel == this.Email)
        {
            this.activeLabel = this.FirstPassword;
            this.SetHighlight(this.FirstPasswordButtonHighlight);
        }
        else if (this.activeLabel == this.FirstPassword)
        {
            this.activeLabel = this.SecondPassword;
            this.SetHighlight(this.SecondPasswordButtonHighlight);
        }
        else if (this.activeLabel == this.SecondPassword)
        {
            this.activeLabel = this.Username;
            this.SetHighlight(this.UsernameButtonHighlight);
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
        this.username = string.Empty;
        this.email = string.Empty;
        this.firstPassword = string.Empty;
        this.secondPassord = string.Empty;
        this.Username.Text = string.Empty;
        this.Email.Text = string.Empty;
        this.FirstPassword.Text = string.Empty;
        this.SecondPassword.Text = string.Empty;
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
                    case '\n':
                    case '\r':
                        this.OnRegisterButtonPushed();
                        break;

                    case '\b':
                        if ((this.activeLabel == this.Email) && (this.email.Length > 0))
                        {
                            this.email = this.email.Substring(0, this.email.Length - 1);
                            this.SetAnimator(this.EmailIBeam);
                            this.activeLabel.Text = this.email;
                        }
                        else if ((this.activeLabel == this.Username) && (this.username.Length > 0))
                        {
                            this.username = this.username.Substring(0, this.username.Length - 1);
                            this.SetAnimator(this.UsernameIBeam);
                            this.activeLabel.Text = this.username;
                        }
                        else if ((this.activeLabel == this.FirstPassword) && (this.firstPassword.Length > 0))
                        {
                            this.firstPassword = this.firstPassword.Substring(0, this.firstPassword.Length - 1);
                            string str = string.Empty;
                            for (int j = 0; j < this.firstPassword.Length; j++)
                            {
                                str = str + "*";
                            }
                            this.SetAnimator(this.FirstPasswordIBeam);
                            this.activeLabel.Text = str;
                        }
                        else if ((this.activeLabel == this.SecondPassword) && (this.secondPassord.Length > 0))
                        {
                            this.secondPassord = this.secondPassord.Substring(0, this.secondPassord.Length - 1);
                            string str2 = string.Empty;
                            for (int k = 0; k < this.secondPassord.Length; k++)
                            {
                                str2 = str2 + "*";
                            }
                            this.SetAnimator(this.SecondPasswordIBeam);
                            this.activeLabel.Text = str2;
                        }
                        break;

                    default:
                        if (this.activeLabel == this.Email)
                        {
                            if (this.email.Length < Constants.MAX_EMAIL_LENGTH)
                            {
                                this.email = this.email + ch;
                            }
                            this.activeLabel.Text = this.email;
                            this.SetAnimator(this.EmailIBeam);
                        }
                        else if (this.activeLabel == this.Username)
                        {
                            if (this.username.Length < Constants.MAX_USERNAME_LENGTH)
                            {
                                this.username = this.username + ch;
                            }
                            this.activeLabel.Text = this.username;
                            this.SetAnimator(this.UsernameIBeam);
                        }
                        else if (this.activeLabel == this.FirstPassword)
                        {
                            if (this.firstPassword.Length < Constants.MAX_PASSWORD_LENGTH)
                            {
                                this.firstPassword = this.firstPassword + ch;
                            }
                            string str3 = string.Empty;
                            for (int m = 0; m < this.firstPassword.Length; m++)
                            {
                                str3 = str3 + "*";
                            }
                            this.activeLabel.Text = str3;
                        }
                        else if (this.activeLabel == this.SecondPassword)
                        {
                            if (this.secondPassord.Length < Constants.MAX_PASSWORD_LENGTH)
                            {
                                this.secondPassord = this.secondPassord + ch;
                            }
                            string str4 = string.Empty;
                            for (int n = 0; n < this.secondPassord.Length; n++)
                            {
                                str4 = str4 + "*";
                            }
                            this.activeLabel.Text = str4;
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
                this.ShiftKeyboard(false);
            }
            else if (this.keyboard.active)
            {
                if (!UI.Busy)
                {
                    if (this.activeLabel == this.Username)
                    {
                        if (this.keyboard.text.Length > Constants.MAX_USERNAME_LENGTH)
                        {
                            this.keyboard.text = this.keyboard.text.Substring(0, Constants.MAX_USERNAME_LENGTH);
                        }
                        string str = string.Empty;
                        for (int i = 0; i < this.keyboard.text.Length; i++)
                        {
                            str = str + this.keyboard.text[i];
                        }
                        this.SetAnimator(this.UsernameIBeam);
                        this.activeLabel.Text = str;
                        this.username = this.keyboard.text;
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
                    else if (this.activeLabel == this.FirstPassword)
                    {
                        if (this.keyboard.text.Length > Constants.MAX_PASSWORD_LENGTH)
                        {
                            this.keyboard.text = this.keyboard.text.Substring(0, Constants.MAX_PASSWORD_LENGTH);
                        }
                        string str3 = string.Empty;
                        for (int k = 0; k < this.keyboard.text.Length; k++)
                        {
                            str3 = str3 + "*";
                        }
                        this.SetAnimator(this.FirstPasswordIBeam);
                        this.activeLabel.Text = str3;
                        this.firstPassword = this.keyboard.text;
                    }
                    else if (this.activeLabel == this.SecondPassword)
                    {
                        if (this.keyboard.text.Length > Constants.MAX_PASSWORD_LENGTH)
                        {
                            this.keyboard.text = this.keyboard.text.Substring(0, Constants.MAX_PASSWORD_LENGTH);
                        }
                        string str4 = string.Empty;
                        for (int m = 0; m < this.keyboard.text.Length; m++)
                        {
                            str4 = str4 + "*";
                        }
                        this.SetAnimator(this.SecondPasswordIBeam);
                        this.activeLabel.Text = str4;
                        this.secondPassord = this.keyboard.text;
                    }
                    this.ShiftKeyboard(true);
                }
            }
            else
            {
                this.ShiftKeyboard(false);
            }
        }
        else
        {
            this.ShiftKeyboard(false);
        }
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

    private void OnFirstPasswordButtonPushed()
    {
        UI.Busy = true;
        if ((this.keyboard != null) && this.keyboard.active)
        {
            this.keyboard.text = this.firstPassword;
        }
        this.SetAnimator(this.FirstPasswordIBeam);
        this.activeLabel = this.FirstPassword;
        this.SetHighlight(this.FirstPasswordButtonHighlight);
        UI.Busy = false;
        this.CheckKeyboard();
    }

    private void OnRegisterButtonPushed()
    {
        if (!UI.Busy)
        {
            if (!Game.Network.HasNetworkConnection)
            {
                SetMessage(this.NoConnectionString.ToString());
            }
            else if (this.InputIsValid)
            {
                this.Register();
            }
        }
    }

    private void OnSecondPasswordButtonPushed()
    {
        UI.Busy = true;
        if ((this.keyboard != null) && this.keyboard.active)
        {
            this.keyboard.text = this.secondPassord;
        }
        this.SetAnimator(this.SecondPasswordIBeam);
        this.activeLabel = this.SecondPassword;
        this.SetHighlight(this.SecondPasswordButtonHighlight);
        UI.Busy = false;
        this.CheckKeyboard();
    }

    private void OnUsernameButtonPushed()
    {
        UI.Busy = true;
        if ((this.keyboard != null) && this.keyboard.active)
        {
            this.keyboard.text = this.username;
        }
        this.activeLabel = this.Username;
        this.SetAnimator(this.UsernameIBeam);
        this.SetHighlight(this.UsernameButtonHighlight);
        UI.Busy = false;
        this.CheckKeyboard();
    }

    public override void Refresh()
    {
        this.Clear();
    }

    private void Register()
    {
        Debug.Log("Username[" + this.username + "] Email[" + this.email + "] Password[" + this.firstPassword + "]");
        PlayFabLoginCalls.RegisterNewPlayfabAccount(this.username, this.firstPassword, this.secondPassord, this.email);
    }

    private void SetAnimator(Animator animator)
    {
        this.activeAnimator = animator;
        if (this.EmailIBeam != null)
        {
            this.EmailIBeam.gameObject.SetActive(this.EmailIBeam == this.activeAnimator);
        }
        if (this.UsernameIBeam != null)
        {
            this.UsernameIBeam.gameObject.SetActive(this.UsernameIBeam == this.activeAnimator);
        }
        if (this.FirstPasswordIBeam != null)
        {
            this.FirstPasswordIBeam.gameObject.SetActive(this.FirstPasswordIBeam == this.activeAnimator);
        }
        if (this.SecondPasswordIBeam != null)
        {
            this.SecondPasswordIBeam.gameObject.SetActive(this.SecondPasswordIBeam == this.activeAnimator);
        }
    }

    private void SetHighlight(SpriteRenderer sr)
    {
        this.UsernameButtonHighlight.gameObject.SetActive(sr == this.UsernameButtonHighlight);
        this.EmailButtonHighlight.gameObject.SetActive(sr == this.EmailButtonHighlight);
        this.FirstPasswordButtonHighlight.gameObject.SetActive(sr == this.FirstPasswordButtonHighlight);
        this.SecondPasswordButtonHighlight.gameObject.SetActive(sr == this.SecondPasswordButtonHighlight);
    }

    public static void SetMessage(string str)
    {
        GuiWindow current = GuiWindow.Current;
        if (current != null)
        {
            GuiWindowMainMenu menu = current as GuiWindowMainMenu;
            if (menu != null)
            {
                menu.createAccountPanel.MessageLabel.Text = str;
            }
        }
    }

    private void ShiftKeyboard(bool up)
    {
        if (up && (base.transform.position != this.KeyboardLocation.position))
        {
            base.transform.position = this.KeyboardLocation.position;
        }
        else if (!up && (base.transform.position != this.NoKeyboardLocation.position))
        {
            base.transform.position = this.NoKeyboardLocation.position;
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
            this.firstPassword = string.Empty;
            this.secondPassord = string.Empty;
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
            this.activeLabel = this.Username;
            this.SetAnimator(this.UsernameIBeam);
            this.SetHighlight(this.UsernameButtonHighlight);
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
            if (this.Username.Text.Length < 6)
            {
                SetMessage("Username must be 6 characters or more!");
                return false;
            }
            if (!RegexUtilities.IsEmail(this.Email.Text))
            {
                SetMessage("Email is invalid!");
                return false;
            }
            if ((this.FirstPassword.Text.Length < 6) || (this.SecondPassword.Text.Length < 6))
            {
                SetMessage("Password must be 6 characters or more!");
                return false;
            }
            if (this.FirstPassword.Text != this.SecondPassword.Text)
            {
                SetMessage("Passwords must match!");
                return false;
            }
            return true;
        }
    }
}

