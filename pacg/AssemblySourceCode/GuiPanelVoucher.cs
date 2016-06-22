using System;
using UnityEngine;

public class GuiPanelVoucher : GuiPanelBackStack
{
    private Animator activeAnimator;
    [Tooltip("reference to the \"already redeemed code\" error string")]
    public StrRefType AlreadyRedeemedString;
    private int attempts;
    private string code = string.Empty;
    [Tooltip("reference to the code input button on this panel")]
    public GuiButton CodeButton;
    [Tooltip("reference to the background sprite highlight for the code input button")]
    public SpriteRenderer CodeButtonHighlight;
    [Tooltip("reference to the code field I beam animator")]
    public Animator CodeIBeam;
    [Tooltip("reference to the code label on this panel")]
    public GuiLabel CodeLabel;
    [Tooltip("reference to the close button on this panel")]
    public GuiButton CornerCloseButton;
    [Tooltip("reference to the \"couldn't find code\" error string")]
    public StrRefType CouldntFindCodeString;
    [Tooltip("reference to the \"invalid code\" error string")]
    public StrRefType InvalidCodeString;
    private TouchScreenKeyboard keyboard;
    private bool locked;
    [Tooltip("reference to the information message label on this panel")]
    public GuiLabel MessageLabel;
    [Tooltip("reference to the \"no connection\" error string")]
    public StrRefType NoConnectionString;
    [Tooltip("reference to the \"not logged in\" error string")]
    public StrRefType NotLoggedInString;
    [Tooltip("reference to the \"please wait\" string")]
    public StrRefType PleaseWaitString;
    [Tooltip("reference to the redeem button on this panel")]
    public GuiButton RedeemButton;
    [Tooltip("reference to the \"successfully redeemed code\" result string")]
    public StrRefType SuccessString;
    private float timer;
    [Tooltip("reference to the \"too many attempts\" string")]
    public StrRefType TooManyAttemptsString;

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
        this.CodeLabel.Text = string.Empty;
        this.code = string.Empty;
        this.MessageLabel.Text = string.Empty;
    }

    private void InputLoopDesktop()
    {
        if ((Input.inputString != null) && !this.locked)
        {
            for (int i = 0; i < Input.inputString.Length; i++)
            {
                char ch = Input.inputString[i];
                switch (ch)
                {
                    case '\b':
                        if (this.code.Length > 0)
                        {
                            this.code = this.code.Substring(0, this.code.Length - 1);
                            this.SetAnimator(this.CodeIBeam);
                            this.CodeLabel.Text = this.code;
                        }
                        break;

                    case '\n':
                    case '\r':
                        this.Redeem();
                        break;

                    default:
                        if (this.code.Length < Constants.MAX_VOUCHER_LENGTH)
                        {
                            this.code = this.code + ch;
                            this.MessageLabel.Text = string.Empty;
                        }
                        this.SetAnimator(this.CodeIBeam);
                        this.CodeLabel.Text = this.code;
                        break;
                }
            }
        }
    }

    private void InputLoopMobile()
    {
        if ((((this.keyboard != null) && !this.locked) && (!this.keyboard.done && this.keyboard.active)) && !UI.Busy)
        {
            if (this.keyboard.text.Length > Constants.MAX_VOUCHER_LENGTH)
            {
                this.keyboard.text = this.keyboard.text.Substring(0, Constants.MAX_VOUCHER_LENGTH);
            }
            string str = string.Empty;
            for (int i = 0; i < this.keyboard.text.Length; i++)
            {
                str = str + this.keyboard.text[i];
            }
            this.SetAnimator(this.CodeIBeam);
            if (!this.CodeLabel.Text.Equals(str))
            {
                this.MessageLabel.Text = string.Empty;
            }
            this.CodeLabel.Text = str;
            this.code = this.keyboard.text;
        }
    }

    private void OnCloseButtonPushed()
    {
        if (!UI.Busy && !this.locked)
        {
            this.Show(false);
        }
    }

    private void OnCodeButtonPushed()
    {
        if (!UI.Busy && !this.locked)
        {
            if ((this.keyboard != null) && this.keyboard.active)
            {
                this.keyboard.text = this.code;
            }
            this.SetAnimator(this.CodeIBeam);
            this.SetHighlight(this.CodeButtonHighlight);
            this.CheckKeyboard();
        }
    }

    private void OnRedeemButtonPushed()
    {
        if (!UI.Busy && !this.locked)
        {
            if (!Game.Network.HasNetworkConnection)
            {
                SetMessage(this.NoConnectionString.ToString());
            }
            else
            {
                this.Redeem();
            }
        }
    }

    public override void Rebind()
    {
        this.CornerCloseButton.Rebind();
        this.CodeButton.Rebind();
        this.RedeemButton.Rebind();
    }

    private void Redeem()
    {
        if (!Game.Network.Connected)
        {
            this.Clear();
            this.SetMessageLabel(ResultType.NotLoggedIn);
        }
        else if (!Game.Network.HasNetworkConnection)
        {
            this.Clear();
            this.SetMessageLabel(ResultType.NoConnection);
        }
        else if (this.CodeLabel.Text.Length < 6)
        {
            this.Clear();
            this.SetMessageLabel(ResultType.InvalidCode);
        }
        else if (!this.CodeLabel.Text.Contains("-"))
        {
            this.Clear();
            SetMessage("Code must use hyphens.");
        }
        else if ((this.attempts > 1) && (this.timer > 0f))
        {
            this.SetMessageLabel(ResultType.TooManyAttempts);
        }
        else
        {
            this.attempts++;
            this.timer = 5f;
            this.locked = true;
            Game.Network.RedeemVoucher(this.CodeLabel.Text, "Purchasable");
            this.CodeLabel.Text = string.Empty;
            this.code = string.Empty;
            this.SetMessageLabel(ResultType.PleaseWait);
        }
    }

    public override void Refresh()
    {
        this.Clear();
    }

    private void SetAnimator(Animator animator)
    {
        this.activeAnimator = animator;
        if (this.CodeIBeam != null)
        {
            this.CodeIBeam.gameObject.SetActive(this.CodeIBeam == this.activeAnimator);
        }
    }

    private void SetHighlight(SpriteRenderer sr)
    {
        this.CodeButtonHighlight.gameObject.SetActive(sr == this.CodeButtonHighlight);
    }

    public static void SetMessage(ResultType type)
    {
        Game.UI.VoucherPanel.Clear();
        Game.UI.VoucherPanel.SetMessageLabel(type);
    }

    public static void SetMessage(string str)
    {
        Game.UI.VoucherPanel.Clear();
        Game.UI.VoucherPanel.MessageLabel.Text = str;
    }

    public void SetMessageLabel(ResultType type)
    {
        if (type == ResultType.NotLoggedIn)
        {
            this.MessageLabel.Text = this.NotLoggedInString.ToString();
        }
        else if (type == ResultType.NoConnection)
        {
            this.MessageLabel.Text = this.NoConnectionString.ToString();
        }
        else if (type == ResultType.AlreadyRedeemed)
        {
            this.MessageLabel.Text = this.AlreadyRedeemedString.ToString();
        }
        else if (type == ResultType.CouldntFindCode)
        {
            this.MessageLabel.Text = this.CouldntFindCodeString.ToString();
        }
        else if (type == ResultType.InvalidCode)
        {
            this.MessageLabel.Text = this.InvalidCodeString.ToString();
        }
        else if (type == ResultType.Success)
        {
            this.MessageLabel.Text = this.SuccessString.ToString();
        }
        else if (type == ResultType.PleaseWait)
        {
            this.MessageLabel.Text = this.PleaseWaitString.ToString();
        }
        else if (type == ResultType.TooManyAttempts)
        {
            this.MessageLabel.Text = this.TooManyAttemptsString.ToString();
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.timer = 0f;
        this.attempts = 0;
        if (base.Owner != null)
        {
            base.Owner.Pause(isVisible);
        }
        else
        {
            UI.Window.Pause(isVisible);
        }
        if (isVisible)
        {
            if (Device.GetIsIphone() || Device.GetIsAndroid())
            {
                TouchScreenKeyboard.hideInput = true;
                this.keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.Default, false, false, false, false, string.Empty);
            }
            this.Refresh();
            UI.Busy = false;
            this.SetAnimator(this.CodeIBeam);
            this.SetHighlight(this.CodeButtonHighlight);
        }
        else
        {
            base.Owner = null;
            this.code = string.Empty;
            if ((this.keyboard != null) && this.keyboard.active)
            {
                this.keyboard.active = false;
            }
        }
    }

    public static void Unlock()
    {
        Game.UI.VoucherPanel.locked = false;
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
        if (this.timer > 0f)
        {
            this.timer -= Time.deltaTime;
        }
        if ((this.timer <= 0f) && (this.attempts > 0))
        {
            this.attempts = 0;
        }
        base.Update();
    }

    public override bool Fullscreen =>
        true;

    public enum ResultType
    {
        PleaseWait,
        NotLoggedIn,
        NoConnection,
        InvalidCode,
        AlreadyRedeemed,
        CouldntFindCode,
        TooManyAttempts,
        Success
    }
}

