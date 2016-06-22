using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelMenuPopup : GuiPanel
{
    [Tooltip("reference to the menu buttons in this panel")]
    public GuiButton[] buttons;
    private DeckType deckPosition;
    [Tooltip("unique; used for saving and loading")]
    public string GUID;
    private List<PopupMenuItem> items = new List<PopupMenuItem>(2);

    public bool Add(string text, TurnStateCallback callback)
    {
        if (this.items.Count < this.buttons.Length)
        {
            PopupMenuItem item = new PopupMenuItem(text, callback);
            this.items.Add(item);
            return true;
        }
        return false;
    }

    public override void Clear()
    {
        this.items.Clear();
    }

    public override void Initialize()
    {
        this.Show(false);
    }

    [DebuggerHidden]
    private IEnumerator InvokeMenuItem(int n) => 
        new <InvokeMenuItem>c__Iterator64 { 
            n = n,
            <$>n = n,
            <>f__this = this
        };

    public void InvokePower(int n)
    {
        if (n < this.items.Count)
        {
            this.items[n].Invoke();
        }
    }

    private void MoveMenuPosition(float x, float y)
    {
        base.transform.localPosition = new Vector3(x, y, 0f);
        for (int i = 0; i < this.buttons.Length; i++)
        {
            this.buttons[i].Refresh();
        }
    }

    public void OnLoadData()
    {
        if (!string.IsNullOrEmpty(this.GUID))
        {
            byte[] buffer;
            if (Game.GetObjectData(this.GUID, out buffer))
            {
                ByteStream bs = new ByteStream(buffer);
                if (bs != null)
                {
                    bs.ReadInt();
                    this.deckPosition = (DeckType) bs.ReadInt();
                    int num = bs.ReadInt();
                    for (int i = 0; i < num; i++)
                    {
                        string text = bs.ReadString();
                        TurnStateCallback callback = TurnStateCallback.FromStream(bs);
                        PopupMenuItem item = new PopupMenuItem(text, callback);
                        this.items.Add(item);
                    }
                }
            }
            this.Refresh();
        }
    }

    private void OnMenuButton1Pushed()
    {
        if (!UI.Busy)
        {
            Game.Instance.StartCoroutine(this.InvokeMenuItem(0));
        }
    }

    private void OnMenuButton2Pushed()
    {
        if (!UI.Busy)
        {
            Game.Instance.StartCoroutine(this.InvokeMenuItem(1));
        }
    }

    private void OnMenuButton3Pushed()
    {
        if (!UI.Busy)
        {
            Game.Instance.StartCoroutine(this.InvokeMenuItem(2));
        }
    }

    private void OnMenuButton4Pushed()
    {
        if (!UI.Busy)
        {
            Game.Instance.StartCoroutine(this.InvokeMenuItem(3));
        }
    }

    public void OnSaveData()
    {
        if (!string.IsNullOrEmpty(this.GUID))
        {
            ByteStream bs = new ByteStream();
            if (bs != null)
            {
                bs.WriteInt(1);
                bs.WriteInt((int) this.deckPosition);
                bs.WriteInt(this.items.Count);
                for (int i = 0; i < this.items.Count; i++)
                {
                    bs.WriteString(this.items[i].Text);
                    WriteTurnStateCallback(bs, this.items[i].Callback);
                }
                Game.SetObjectData(this.GUID, bs.ToArray());
            }
        }
    }

    private void PlayButtonAnimation(GuiButton button, string animation)
    {
        Animator component = button.transform.parent.GetComponent<Animator>();
        if (component != null)
        {
            component.SetTrigger(animation);
        }
    }

    private void ResetButtonAnimation(GuiButton button, string animation)
    {
        Animator component = button.transform.parent.GetComponent<Animator>();
        if (component != null)
        {
            component.SetBool(animation, false);
        }
    }

    private void RotateTail(int amount)
    {
        Transform transform = this.buttons[0].transform.FindChild("popup_options_tail");
        if (transform != null)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, (float) amount);
        }
    }

    public void SetDeckPosition(ActionType action)
    {
        if (action == ActionType.Discard)
        {
            this.SetDeckPosition(DeckType.Discard);
        }
        else
        {
            this.SetDeckPosition(DeckType.None);
        }
    }

    public void SetDeckPosition(DeckType deck)
    {
        if (deck == DeckType.Discard)
        {
            this.RotateTail(20);
            this.MoveMenuPosition(-2f, -2f);
        }
        else if (deck == DeckType.Location)
        {
            this.RotateTail(-130);
            this.MoveMenuPosition(0f, 1f);
        }
        else if (deck == DeckType.Character)
        {
            this.RotateTail(150);
            this.MoveMenuPosition(2f, -2f);
        }
        else
        {
            this.RotateTail(90);
            this.MoveMenuPosition(0f, 0f);
        }
        this.deckPosition = deck;
    }

    public override void Show(bool isVisible)
    {
        if (isVisible)
        {
            if (this.items.Count > 0)
            {
                base.Show(isVisible);
                this.SetDeckPosition(this.deckPosition);
                for (int i = 0; i < this.buttons.Length; i++)
                {
                    this.ShowButton(this.buttons[i], false);
                }
                for (int j = 0; j < this.items.Count; j++)
                {
                    this.buttons[j].Text = this.items[j].Text;
                    this.ShowButton(this.buttons[j], true);
                    this.PlayButtonAnimation(this.buttons[j], "Start");
                }
                UI.Busy = false;
            }
        }
        else
        {
            base.Show(isVisible);
        }
        UI.Window.Pause(isVisible);
    }

    private void ShowButton(GuiButton button, bool isVisible)
    {
        button.Show(isVisible);
        if (button.transform.parent != null)
        {
            button.transform.parent.gameObject.SetActive(isVisible);
        }
    }

    private static void WriteTurnStateCallback(ByteStream bs, TurnStateCallback callback)
    {
        if (callback == null)
        {
            bs.WriteBool(false);
        }
        else
        {
            callback.ToStream(bs);
        }
    }

    public int Count =>
        this.items.Count;

    [CompilerGenerated]
    private sealed class <InvokeMenuItem>c__Iterator64 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal int <$>n;
        internal GuiPanelMenuPopup <>f__this;
        internal int <i>__0;
        internal int n;

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
                    this.<>f__this.ResetButtonAnimation(this.<>f__this.buttons[this.n], "Start");
                    this.<>f__this.PlayButtonAnimation(this.<>f__this.buttons[this.n], "Selected");
                    this.<i>__0 = 0;
                    while (this.<i>__0 < this.<>f__this.buttons.Length)
                    {
                        if (this.<i>__0 != this.n)
                        {
                            this.<>f__this.ResetButtonAnimation(this.<>f__this.buttons[this.<i>__0], "Start");
                            this.<>f__this.PlayButtonAnimation(this.<>f__this.buttons[this.<i>__0], "NotSelected");
                        }
                        this.<i>__0++;
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.75f));
                    this.$PC = 1;
                    return true;

                case 1:
                    if (this.n < this.<>f__this.items.Count)
                    {
                        this.<>f__this.items[this.n].Invoke();
                    }
                    UI.Busy = false;
                    this.$PC = -1;
                    break;
            }
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

    private class PopupMenuItem
    {
        private TurnStateCallback callback;
        private string text;

        public PopupMenuItem(string text, TurnStateCallback callback)
        {
            this.text = text;
            this.callback = callback;
        }

        public void Invoke()
        {
            this.callback.Invoke();
        }

        public TurnStateCallback Callback =>
            this.callback;

        public string Text =>
            this.text;
    }
}

