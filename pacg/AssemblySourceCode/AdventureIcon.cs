using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AdventureIcon : MonoBehaviour
{
    [Tooltip("sound played when this icon is touched")]
    public AudioClip ClickSound;
    [Tooltip("the world-space height of this icon")]
    public float Height = 2f;
    [Tooltip("reference to the image in our hierarchy")]
    public GuiImage Image;
    [Tooltip("reference to the title in our hierarchy")]
    public GuiLabel TitleLabel;

    public static AdventureIcon Create(AdventureTableEntry entry)
    {
        string path = "Blueprints/Gui/List_Adventure";
        GameObject original = Resources.Load<GameObject>(path);
        if (original != null)
        {
            GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(original);
            if (obj3 != null)
            {
                obj3.name = original.name;
                AdventureIcon component = obj3.GetComponent<AdventureIcon>();
                if (component != null)
                {
                    component.TitleLabel.Text = entry.Name;
                }
                return component;
            }
        }
        return null;
    }

    public void LoadImage(string id)
    {
        string path = "Blueprints/Icons/Adventures/" + id;
        SpriteRenderer renderer = Game.Cache.Get<SpriteRenderer>(path);
        if (renderer != null)
        {
            this.Image.Image = renderer.sprite;
        }
        else
        {
            this.Image.Image = null;
        }
    }

    public void Show(bool isVisible)
    {
        base.gameObject.SetActive(isVisible);
    }

    public void Tap()
    {
        UI.Sound.Play(this.ClickSound);
    }

    public string ID { get; set; }
}

