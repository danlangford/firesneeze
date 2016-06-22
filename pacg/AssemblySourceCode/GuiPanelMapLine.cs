using System;
using UnityEngine;

public class GuiPanelMapLine : GuiElement
{
    private ScenarioMapIcon endIcon;
    [Tooltip("material used to draw this line")]
    public Material hiliteMaterial;
    [Tooltip("renderer used to draw this line")]
    public LineRenderer line;
    [Tooltip("material used to hilite this line")]
    public Material normalMaterial;
    private ScenarioMapIcon startIcon;

    public static GuiPanelMapLine Create(ScenarioMapIcon start, ScenarioMapIcon end)
    {
        GameObject original = Resources.Load<GameObject>("Art/MapLines/Map_LinkedLocation_Line");
        if (original != null)
        {
            GameObject obj3 = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
            if (obj3 != null)
            {
                GuiPanelMapLine component = obj3.GetComponent<GuiPanelMapLine>();
                if (component != null)
                {
                    component.name = "Line_" + start.ID + "_" + end.ID;
                    component.startIcon = start;
                    component.endIcon = end;
                    component.line.sortingOrder = -12;
                    return component;
                }
            }
        }
        return null;
    }

    public void Fade(float alpha, float duration)
    {
        if (this.line != null)
        {
            LeanTween.alpha(this.line.gameObject, alpha, duration);
        }
    }

    private Vector3 GetIconPosition(ScenarioMapIcon icon)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            return window.mapPanel.GetIconPosition(icon);
        }
        return icon.transform.position;
    }

    public void Glow(bool isGlowing)
    {
        if (isGlowing)
        {
            this.line.material = this.hiliteMaterial;
        }
        else
        {
            this.line.material = this.normalMaterial;
        }
    }

    private bool IsIconVisible(ScenarioMapIcon icon)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            return window.mapPanel.IsIconVisible(icon);
        }
        return true;
    }

    public bool Match(ScenarioMapIcon icon)
    {
        if ((icon.ID != this.startIcon.ID) && (icon.ID != this.endIcon.ID))
        {
            return false;
        }
        return true;
    }

    public bool Match(ScenarioMapIcon icon1, ScenarioMapIcon icon2) => 
        (((icon1.ID == this.startIcon.ID) && (icon2.ID == this.endIcon.ID)) || ((icon1.ID == this.endIcon.ID) && (icon2.ID == this.startIcon.ID)));

    public override void Refresh()
    {
        if (this.line != null)
        {
            this.line.SetPosition(0, this.GetIconPosition(this.startIcon));
            this.line.SetPosition(1, this.GetIconPosition(this.endIcon));
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible && (this.line != null))
        {
            this.Refresh();
        }
        if (this.line != null)
        {
            this.line.gameObject.SetActive(isVisible);
        }
    }
}

