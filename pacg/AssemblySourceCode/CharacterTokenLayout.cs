using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTokenLayout : MonoBehaviour
{
    [Tooltip("layout tokens centered or towards the right")]
    public bool Center = true;
    [Tooltip("layout tokens horizontally or vertically")]
    public bool Horizontal = true;
    [Tooltip("amount of world space between each token")]
    public float Padding = 0.1f;
    [Tooltip("scale of each token")]
    public float Scale = 1f;
    private List<CharacterToken> tokens;

    public Vector3 GetTokenPosition(CharacterToken token)
    {
        if ((this.Tokens != null) && (token != null))
        {
            for (int i = 0; i < this.Tokens.Count; i++)
            {
                if (this.Tokens[i].ID == token.ID)
                {
                    return this.GetTokenPosition(i);
                }
            }
        }
        return base.transform.position;
    }

    public Vector3 GetTokenPosition(int i)
    {
        if (this.Horizontal && this.Center)
        {
            return this.GetTokenPositionHorizontalCenter(i);
        }
        if (this.Horizontal && !this.Center)
        {
            return this.GetTokenPositionHorizontalRight(i);
        }
        if (!this.Horizontal && this.Center)
        {
            return this.GetTokenPositionVerticalCenter(i);
        }
        return this.GetTokenPositionVerticalRight(i);
    }

    private Vector3 GetTokenPositionHorizontalCenter(int i)
    {
        float num = this.GetTokenSize(i).x + this.Padding;
        Vector3 vector = base.transform.position - new Vector3(num * Geometry.GetMidPoint(this.Tokens.Count), 0f, 0f);
        return (vector + new Vector3(num * i, 0f, 0f));
    }

    private Vector3 GetTokenPositionHorizontalRight(int i)
    {
        float num = this.GetTokenSize(i).x + this.Padding;
        return (base.transform.position + new Vector3(num * i, 0f, 0f));
    }

    private Vector3 GetTokenPositionVerticalCenter(int i)
    {
        float num = this.GetTokenSize(i).y + this.Padding;
        Vector3 vector = base.transform.position + new Vector3(0f, num * Geometry.GetMidPoint(this.Tokens.Count), 0f);
        return (vector - new Vector3(0f, num * i, 0f));
    }

    private Vector3 GetTokenPositionVerticalRight(int i)
    {
        float num = this.GetTokenSize(i).y + this.Padding;
        return (base.transform.position - new Vector3(0f, num * i, 0f));
    }

    public Vector3 GetTokenScale(int i) => 
        new Vector3(this.Scale, this.Scale, 1f);

    private Vector2 GetTokenSize(int i)
    {
        CharacterToken token = this.Tokens[i];
        Vector3 localScale = token.transform.localScale;
        token.transform.localScale = this.GetTokenScale(i);
        Vector2 size = token.Size;
        token.transform.localScale = localScale;
        return size;
    }

    public void Initialize(List<CharacterToken> tokens)
    {
        this.Tokens = tokens;
        this.Layout();
    }

    public void Layout()
    {
        if (this.Tokens != null)
        {
            for (int i = 0; i < this.Tokens.Count; i++)
            {
                this.Tokens[i].transform.position = this.GetTokenPosition(i);
                this.Tokens[i].transform.localScale = this.GetTokenScale(i);
            }
        }
    }

    public void Refresh()
    {
        if (this.Tokens != null)
        {
            for (int i = 0; i < this.Tokens.Count; i++)
            {
                Vector3[] to = Geometry.GetCurve(this.Tokens[i].transform.position, this.GetTokenPosition(i), 0f);
                LeanTween.move(this.Tokens[i].gameObject, to, 0.3f);
                LeanTween.scale(this.Tokens[i].gameObject, this.GetTokenScale(i), 0.3f);
            }
        }
    }

    public List<CharacterToken> Tokens
    {
        get => 
            this.tokens;
        set
        {
            this.tokens = value;
            for (int i = 0; i < this.Tokens.Count; i++)
            {
                this.Tokens[i].Layout = this;
            }
        }
    }
}

