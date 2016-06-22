using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(SpriteRenderer))]
public class CharacterPowerIcon : MonoBehaviour
{
    [Tooltip("sound made when this icon is tapped")]
    public AudioClip ClickSound;
    [Tooltip("optional: description text for this power")]
    public StrRefType DescriptionText;
    [Tooltip("optional: name text for this power")]
    public StrRefType NameText;

    public void Show(bool isVisible)
    {
        base.gameObject.SetActive(isVisible);
    }

    public void Tap()
    {
        UI.Sound.Play(this.ClickSound);
    }

    public string Description
    {
        get
        {
            if (!this.DescriptionText.IsNullOrEmpty())
            {
                return this.DescriptionText.ToString();
            }
            if (this.ID != null)
            {
                PowerTableEntry entry = PowerTable.Get(this.ID);
                if (entry != null)
                {
                    return entry.Description;
                }
            }
            return null;
        }
    }

    public string ID { get; set; }

    public Sprite Image
    {
        get => 
            base.GetComponent<SpriteRenderer>().sprite;
        set
        {
            base.GetComponent<SpriteRenderer>().sprite = value;
        }
    }

    public string Name
    {
        get
        {
            if (!this.NameText.IsNullOrEmpty())
            {
                return this.NameText.ToString();
            }
            if (this.ID != null)
            {
                PowerTableEntry entry = PowerTable.Get(this.ID);
                if (entry != null)
                {
                    return entry.Name;
                }
            }
            return null;
        }
    }
}

