using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AdventureMapButton : MonoBehaviour
{
    [Tooltip("sound to play when button is pushed")]
    public AudioClip ClickSound;
    [Tooltip("name of event to send to invoke")]
    public string EventName;
    private SpriteRenderer mySprite;
    private TextMesh myTextMesh;
    private GuiWindow myWindow;

    public void Awake()
    {
        this.myWindow = UI.Window;
        this.mySprite = Geometry.GetSubComponent<SpriteRenderer>(this);
        this.myTextMesh = Geometry.GetSubComponent<TextMesh>(this);
        this.SetupText();
    }

    private void SetupText()
    {
        if ((this.myTextMesh != null) && (this.mySprite != null))
        {
            this.myTextMesh.GetComponent<Renderer>().sortingLayerID = this.mySprite.sortingLayerID;
            this.myTextMesh.GetComponent<Renderer>().sortingOrder = this.mySprite.sortingOrder + 1;
        }
    }

    public void Show(bool isVisible)
    {
        base.gameObject.SetActive(isVisible);
    }

    public void Tap()
    {
        UI.Sound.Play(this.ClickSound);
        if (this.myWindow != null)
        {
            this.myWindow.SendMessage(this.EventName);
        }
    }

    public string Text
    {
        get => 
            this.myTextMesh.text;
        set
        {
            this.myTextMesh.text = value;
        }
    }
}

