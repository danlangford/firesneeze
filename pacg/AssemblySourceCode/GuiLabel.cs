using System;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class GuiLabel : GuiElement
{
    private static char[] buffer = new char[0x400];
    [Tooltip("max number of pixels wide; larger will scale text down to fit")]
    public int FitWidth;
    private bool isInitialized;
    [Tooltip("maximum number of pixels per line for word wrap (zero means infinite)")]
    public float LineWidth;
    [Tooltip("maximum number of lines before text is truncated (zero means infinite)")]
    public int MaxLines;
    [Tooltip("text to display on this label")]
    public StrRefType Message;
    private TextMesh myTextMesh;
    [Tooltip("sorting layer"), SortingLayer]
    public int SortingLayer;
    [Tooltip("sorting order within layer")]
    public int SortingOrder;

    protected override void Awake()
    {
        base.Awake();
        this.SetupText();
        if (!this.isInitialized && !this.Message.IsNullOrEmpty())
        {
            this.Text = this.Message.ToString();
        }
        this.SetupTextEffects();
    }

    public void Clear()
    {
        if (this.myTextMesh != null)
        {
            this.myTextMesh.text = string.Empty;
        }
        this.Refresh();
    }

    public void Fade(bool isVisible, float time)
    {
        if (isVisible)
        {
            this.myTextMesh.GetComponent<Renderer>().material.color = new UnityEngine.Color(this.myTextMesh.color.r, this.myTextMesh.color.g, this.myTextMesh.color.b, 0f);
            LeanTween.cancel(this.myTextMesh.gameObject);
            LeanTween.alpha(this.myTextMesh.gameObject, 1f, time);
        }
        else
        {
            LeanTween.cancel(this.myTextMesh.gameObject);
            LeanTween.alpha(this.myTextMesh.gameObject, 0f, time);
        }
    }

    private int GetGlyphWidth(char ch)
    {
        CharacterInfo info;
        if ((this.myTextMesh != null) && this.myTextMesh.font.GetCharacterInfo(ch, out info, this.myTextMesh.fontSize, this.myTextMesh.fontStyle))
        {
            return Mathf.RoundToInt((float) info.advance);
        }
        return 0;
    }

    public int GetLineCount()
    {
        int num = 1;
        for (int i = 0; i < this.Text.Length; i++)
        {
            if (this.Text[i] == '\n')
            {
                num++;
            }
        }
        return num;
    }

    public int GetLinePosition(string substring)
    {
        int num = 0;
        if (this.Text != null)
        {
            int index = this.Text.IndexOf(substring);
            for (int i = 0; i < index; i++)
            {
                if (this.Text[i] == '\n')
                {
                    num++;
                }
            }
        }
        return num;
    }

    public int GetSectionLines(string substring)
    {
        int num = 1;
        if (this.Text != null)
        {
            int index = this.Text.IndexOf(substring);
            if (index == -1)
            {
                return num;
            }
            for (int i = index; i < this.Text.Length; i++)
            {
                if ((((i + 1) < this.Text.Length) && (this.Text[i] == '\n')) && (this.Text[i + 1] == '\n'))
                {
                    return num;
                }
                if (this.Text[i] == '\n')
                {
                    num++;
                }
            }
        }
        return num;
    }

    private static char[] GetTemporaryMemoryBuffer(int length)
    {
        if (length > buffer.Length)
        {
            return new char[length];
        }
        Array.Clear(buffer, 0, buffer.Length);
        return buffer;
    }

    public override void Refresh()
    {
        TextOutline component = base.GetComponent<TextOutline>();
        if (component != null)
        {
            component.Refresh(this);
        }
    }

    public void Scale(int maxWidth)
    {
        if (this.myTextMesh != null)
        {
            float num = 0f;
            for (int i = 0; i < this.myTextMesh.text.Length; i++)
            {
                CharacterInfo info;
                if (this.myTextMesh.font.GetCharacterInfo(this.myTextMesh.text[i], out info, this.myTextMesh.fontSize, this.myTextMesh.fontStyle))
                {
                    num += info.advance;
                }
            }
            this.myTextMesh.characterSize = (num <= maxWidth) ? 1f : (((float) maxWidth) / num);
        }
    }

    private void SetupFontTexture(string text)
    {
        if (this.myTextMesh != null)
        {
            this.myTextMesh.font.RequestCharactersInTexture(text, this.myTextMesh.fontSize, this.myTextMesh.fontStyle);
        }
    }

    private void SetupText()
    {
        this.myTextMesh = base.GetComponent<TextMesh>();
        if (this.myTextMesh != null)
        {
            this.myTextMesh.GetComponent<Renderer>().sortingOrder = this.SortingOrder;
            this.myTextMesh.GetComponent<Renderer>().sortingLayerID = this.SortingLayer;
        }
    }

    private void SetupTextEffects()
    {
        TextOutline component = base.GetComponent<TextOutline>();
        if (component != null)
        {
            component.Initialize(this);
            component.Refresh(this);
        }
    }

    private string WordWrap(string text)
    {
        if (this.LineWidth <= 0f)
        {
            return text;
        }
        if ((text == null) || (text.Length <= 0))
        {
            return string.Empty;
        }
        this.SetupFontTexture(text);
        char[] temporaryMemoryBuffer = GetTemporaryMemoryBuffer(text.Length + 1);
        int length = 0;
        int num2 = 0;
        int num3 = 0;
        int num4 = 0;
        int num5 = 0;
        char ch = '\n';
        bool flag = false;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (char.IsWhiteSpace(c) || (i == (text.Length - 1)))
            {
                if ((num3 + num4) > this.LineWidth)
                {
                    if (length > 0)
                    {
                        temporaryMemoryBuffer[length++] = '\n';
                        num2++;
                    }
                    num3 = num4;
                }
                else
                {
                    if (length > 0)
                    {
                        temporaryMemoryBuffer[length++] = ch;
                    }
                    if (ch != '\n')
                    {
                        num3 += this.GetGlyphWidth(ch);
                    }
                    num3 += num4;
                }
                if ((this.MaxLines > 0) && (num2 >= this.MaxLines))
                {
                    break;
                }
                for (int j = num5; j > 0; j--)
                {
                    temporaryMemoryBuffer[length++] = text[i - j];
                }
                if (i == (text.Length - 1))
                {
                    temporaryMemoryBuffer[length++] = text[i];
                }
                if (c == '\n')
                {
                    num3 = 0;
                    num2++;
                }
                num5 = 0;
                num4 = 0;
                ch = c;
            }
            else
            {
                num5++;
                if (c == '<')
                {
                    flag = true;
                }
                if (!flag)
                {
                    num4 += this.GetGlyphWidth(c);
                }
                if (c == '>')
                {
                    flag = false;
                }
            }
        }
        return new string(temporaryMemoryBuffer, 0, length);
    }

    public TextAlignment Alignment
    {
        get => 
            this.myTextMesh.alignment;
        set
        {
            this.myTextMesh.alignment = value;
            if (value == TextAlignment.Center)
            {
                this.myTextMesh.anchor = TextAnchor.MiddleCenter;
            }
            if (value == TextAlignment.Left)
            {
                this.myTextMesh.anchor = TextAnchor.MiddleLeft;
            }
            if (value == TextAlignment.Right)
            {
                this.myTextMesh.anchor = TextAnchor.MiddleRight;
            }
            this.Refresh();
        }
    }

    public UnityEngine.Color Color
    {
        get => 
            this.myTextMesh.color;
        set
        {
            if (this.myTextMesh == null)
            {
                this.SetupText();
            }
            if (this.myTextMesh != null)
            {
                this.myTextMesh.color = value;
                Renderer component = this.myTextMesh.GetComponent<Renderer>();
                if ((component != null) && (component.material.color != value))
                {
                    component.material.color = value;
                }
            }
        }
    }

    public Vector2 Size
    {
        get
        {
            Bounds bounds = this.myTextMesh.GetComponent<Renderer>().bounds;
            return new Vector2(bounds.extents.x, bounds.extents.y);
        }
    }

    public string Text
    {
        get => 
            this.myTextMesh?.text;
        set
        {
            if (this.myTextMesh == null)
            {
                this.SetupText();
            }
            if (this.myTextMesh != null)
            {
                this.myTextMesh.text = this.WordWrap(value);
            }
            if (this.FitWidth > 0)
            {
                this.Scale(this.FitWidth);
            }
            this.Refresh();
            this.isInitialized = true;
        }
    }
}

