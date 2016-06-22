using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelPartyLine : GuiPanel
{
    [Tooltip("reference to the party member buttons in this hierarchy")]
    public GuiButtonRegion[] Buttons;
    [Tooltip("dead characters will be tinted")]
    public Color colorDead = Color.gray;
    [Tooltip("unavailable characters will be tinted")]
    public Color colorUnavailable = Color.red;
    [Tooltip("left margin in world units")]
    public float leftMargin;
    [Tooltip("right margin in world units")]
    public float rightMargin;

    [DebuggerHidden]
    private IEnumerator DisableTrailRenderer(TrailRenderer r, float time) => 
        new <DisableTrailRenderer>c__Iterator66 { 
            r = r,
            time = time,
            <$>r = r,
            <$>time = time
        };

    private int GetCharacterIndex(string id)
    {
        for (int i = 0; i < this.Buttons.Length; i++)
        {
            if (this.Buttons[i].name == id)
            {
                return i;
            }
        }
        return -1;
    }

    public Vector3 GetCharacterPosition(string id)
    {
        int characterIndex = this.GetCharacterIndex(id);
        if (characterIndex >= 0)
        {
            return this.Buttons[characterIndex].transform.position;
        }
        return base.transform.position;
    }

    private float GetIconWidth(int partySize, float marginTotal)
    {
        if (partySize <= 4)
        {
            return 3.6f;
        }
        return (2.5f - (marginTotal / ((float) partySize)));
    }

    private Vector3 GetLayoutPosition(GuiButtonRegion button, int n, int max)
    {
        float iconWidth = this.GetIconWidth(Party.Characters.Count, this.leftMargin + this.rightMargin);
        float num2 = ((((-1f * (((float) max) / 2f)) * iconWidth) + 1f) + (this.leftMargin / 2f)) - (this.rightMargin / 2f);
        return new Vector3(num2 + (n * iconWidth), button.transform.localPosition.y, button.transform.localPosition.z);
    }

    public override void Initialize()
    {
        for (int i = 0; i < this.Buttons.Length; i++)
        {
            this.Buttons[i].Show(false);
        }
        this.Layout();
        this.Refresh();
    }

    private void Layout()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            int characterIndex = this.GetCharacterIndex(Party.Characters[i].ID);
            if (characterIndex >= 0)
            {
                this.Buttons[characterIndex].Show(true);
                this.Buttons[characterIndex].transform.localPosition = this.GetLayoutPosition(this.Buttons[characterIndex], i, Party.Characters.Count);
                this.Buttons[characterIndex].Refresh();
                this.SetCharacterMarker(Party.Characters[i].ID, false);
                this.ShowButtonSymbol(this.Buttons[characterIndex], "symbol_dead", false);
                this.ShowButtonSymbol(this.Buttons[characterIndex], "symbol_Unavailable", false);
            }
        }
    }

    private void OnAmiriButtonPushed()
    {
        this.SelectCharacter("CH1C_Amiri");
    }

    private void OnEzrenButtonPushed()
    {
        this.SelectCharacter("CH1B_Ezren");
    }

    private void OnHarskButtonPushed()
    {
        this.SelectCharacter("CH1B_Harsk");
    }

    private void OnKyraButtonPushed()
    {
        this.SelectCharacter("CH1B_Kyra");
    }

    private void OnLemButtonPushed()
    {
        this.SelectCharacter("CH1B_Lem");
    }

    private void OnLiniButtonPushed()
    {
        this.SelectCharacter("CH1C_Lini");
    }

    private void OnMerisielButtonPushed()
    {
        this.SelectCharacter("CH1B_Merisiel");
    }

    private void OnSajanButtonPushed()
    {
        this.SelectCharacter("CH1C_Sajan");
    }

    private void OnSeelahButtonPushed()
    {
        this.SelectCharacter("CH1C_Seelah");
    }

    private void OnSeoniButtonPushed()
    {
        this.SelectCharacter("CH1B_Seoni");
    }

    private void OnValerosButtonPushed()
    {
        this.SelectCharacter("CH1B_Valeros");
    }

    public override void Refresh()
    {
        for (int i = 0; i < this.Buttons.Length; i++)
        {
            Character character = Party.Find(this.Buttons[i].name);
            if (character != null)
            {
                if (!character.Alive)
                {
                    this.Buttons[i].Tint(this.colorDead);
                    this.ShowButtonSymbol(this.Buttons[i], "symbol_dead", true);
                }
                else if (character.Active != ActiveType.Active)
                {
                    this.Buttons[i].Tint(this.colorUnavailable);
                    this.ShowButtonSymbol(this.Buttons[i], "symbol_Unavailable", true);
                }
                else
                {
                    this.Buttons[i].Tint(Color.white);
                    this.ShowButtonSymbol(this.Buttons[i], "symbol_dead", false);
                    this.ShowButtonSymbol(this.Buttons[i], "symbol_Unavailable", false);
                }
            }
        }
        for (int j = 0; j < this.Buttons.Length; j++)
        {
            Transform transform = this.Buttons[j].transform.FindChild("Highlight");
            if (transform != null)
            {
                transform.gameObject.SetActive(this.Buttons[j].name == Turn.Character.ID);
            }
        }
    }

    private void SelectCharacter(string id)
    {
        if ((!UI.Zoomed && !UI.Busy) && !UI.Window.Paused)
        {
            int index = Party.IndexOf(id);
            if (index >= 0)
            {
                if (Turn.Number != index)
                {
                    Turn.Number = index;
                    this.Refresh();
                }
                UI.Window.SendMessage("OnCharacterSwitch", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void SetCharacterMarker(string id, bool isVisible)
    {
        int characterIndex = this.GetCharacterIndex(id);
        if (characterIndex >= 0)
        {
            this.ShowButtonSymbol(this.Buttons[characterIndex], "exclaim", isVisible);
        }
    }

    public void SetCharacterTint(string id, Color color)
    {
        int characterIndex = this.GetCharacterIndex(id);
        if (characterIndex >= 0)
        {
            this.Buttons[characterIndex].Tint(color);
        }
    }

    private void ShowButtonSymbol(GuiButtonRegion button, string symbol, bool isVisible)
    {
        Transform transform = button.transform.FindChild(symbol);
        if (transform != null)
        {
            transform.gameObject.SetActive(isVisible);
        }
    }

    public void ShowPoofEffect(string id)
    {
        int characterIndex = this.GetCharacterIndex(id);
        if (characterIndex >= 0)
        {
            Transform transform = this.Buttons[characterIndex].transform.FindChild("Particles - CardPoof");
            if (transform != null)
            {
                VisualEffect.Start(transform.gameObject);
            }
        }
    }

    public void SuspendHighlight(float time)
    {
        for (int i = 0; i < this.Buttons.Length; i++)
        {
            Transform transform = this.Buttons[i].transform.FindChild("Highlight");
            if ((transform != null) && (this.Buttons[i].name == Turn.Character.ID))
            {
                TrailRenderer componentInChildren = transform.GetComponentInChildren<TrailRenderer>();
                Game.Instance.StartCoroutine(this.DisableTrailRenderer(componentInChildren, time));
                break;
            }
        }
    }

    [CompilerGenerated]
    private sealed class <DisableTrailRenderer>c__Iterator66 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal TrailRenderer <$>r;
        internal float <$>time;
        internal TrailRenderer r;
        internal float time;

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
                    if (this.r == null)
                    {
                        break;
                    }
                    this.r.Clear();
                    this.r.time = -this.r.time;
                    this.$current = new WaitForSeconds(this.time);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.r.time = -this.r.time;
                    break;

                default:
                    goto Label_008F;
            }
            this.$PC = -1;
        Label_008F:
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

