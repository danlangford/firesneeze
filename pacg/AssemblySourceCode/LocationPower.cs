using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class LocationPower : Power
{
    [Tooltip("when can this power be used?")]
    public LocationPowerType Situation;
    [Tooltip("can this power be used after the location is closed?")]
    public bool UsefulWhenClosed;

    protected LocationPower()
    {
    }

    public static GameObject Create(string ID, GameObject parent)
    {
        GameObject original = Resources.Load<GameObject>("Blueprints/Powers/" + ID);
        if (original != null)
        {
            GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(original);
            if (obj3 != null)
            {
                obj3.name = original.name;
                obj3.transform.parent = parent.transform;
                return obj3;
            }
        }
        return null;
    }

    protected override void GlowText(bool isGlowing)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (!isGlowing)
            {
                window.locationPanel.GlowText(TextHilightType.None);
            }
            else if (this.Situation == LocationPowerType.AtThisLocation)
            {
                window.locationPanel.GlowText(TextHilightType.AtThisLocation);
            }
            else if (this.Situation == LocationPowerType.WhenClosing)
            {
                window.locationPanel.GlowText(TextHilightType.WhenClosing);
            }
            else if (this.Situation == LocationPowerType.WhenPermanentlyClosed)
            {
                window.locationPanel.GlowText(TextHilightType.WhenPermanentlyClosed);
            }
        }
    }

    protected override void GlowText(bool isGlowing, float duration)
    {
        this.GlowText(isGlowing);
        if (isGlowing)
        {
            LeanTween.delayedCall(duration, () => this.GlowText(false));
        }
    }

    public virtual void OnCardPlayed(Card card)
    {
    }

    public string LocationID { get; set; }

    public virtual PowerType Type =>
        PowerType.None;
}

