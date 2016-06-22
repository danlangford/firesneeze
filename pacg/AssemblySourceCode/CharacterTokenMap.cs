using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(SpriteRenderer))]
public class CharacterTokenMap : MonoBehaviour
{
    public void Clear()
    {
        this.ID = null;
        this.Image = null;
        this.Interactive = false;
    }

    public void Show(bool isVisible)
    {
        base.gameObject.SetActive(isVisible);
    }

    public Character Character
    {
        set
        {
            if (value != null)
            {
                this.ID = value.ID;
                this.Image = value.PortraitLocation;
            }
            else
            {
                this.ID = null;
                this.Image = null;
            }
        }
    }

    public bool Empty =>
        (this.ID == null);

    public string ID { get; private set; }

    private Sprite Image
    {
        set
        {
            SpriteRenderer component = base.GetComponent<SpriteRenderer>();
            if (component != null)
            {
                component.sprite = value;
            }
        }
    }

    public bool Interactive
    {
        get
        {
            BoxCollider2D component = base.GetComponent<BoxCollider2D>();
            return ((component != null) && component.enabled);
        }
        set
        {
            BoxCollider2D component = base.GetComponent<BoxCollider2D>();
            if (component != null)
            {
                component.enabled = value;
            }
        }
    }
}

