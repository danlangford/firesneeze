using System;
using UnityEngine;

public class GuiLayoutCounter : GuiElement
{
    [Tooltip("reference to a label in our hierarchy")]
    public GuiLabel label;
    private Deck myDeck;

    public void Display(int number)
    {
        this.label.Text = number.ToString();
    }

    private void OnDeckChanged(object sender, EventArgsCard e)
    {
        this.Refresh();
    }

    private void OnDestroy()
    {
        if (this.Deck != null)
        {
            this.myDeck.Changed -= new Deck.EventHandlerDeckChanged(this.OnDeckChanged);
        }
    }

    public override void Refresh()
    {
        if (this.Deck != null)
        {
            this.label.Text = this.Deck.Count.ToString();
        }
    }

    public Deck Deck
    {
        get => 
            this.myDeck;
        set
        {
            if (this.myDeck != null)
            {
                this.myDeck.Changed -= new Deck.EventHandlerDeckChanged(this.OnDeckChanged);
            }
            this.myDeck = value;
            if (this.myDeck != null)
            {
                this.myDeck.Changed += new Deck.EventHandlerDeckChanged(this.OnDeckChanged);
                this.Refresh();
            }
        }
    }
}

