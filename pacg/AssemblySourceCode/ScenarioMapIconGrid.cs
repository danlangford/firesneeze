using System;
using UnityEngine;

public class ScenarioMapIconGrid : MonoBehaviour
{
    private ScenarioMapIcon[] icons;
    [Tooltip("amount of world space to inset every 2nd row")]
    public float Inset = 1.5f;
    [Tooltip("amount of world space between each token")]
    public Vector2 Padding = Vector2.zero;
    [Tooltip("scale of each token")]
    public float Scale = 1f;

    private ScenarioMapIcon FindMapIcon(string id)
    {
        for (int i = 0; i < this.icons.Length; i++)
        {
            if ((this.icons[i] != null) && (this.icons[i].ID == id))
            {
                return this.icons[i];
            }
        }
        return null;
    }

    private Vector3 GetIconPosition(ScenarioMapIcon icon, int c)
    {
        Vector2 iconSize = this.GetIconSize(icon);
        if (c < 3)
        {
            return (base.transform.position + new Vector3(c * (iconSize.x + this.Padding.x), 0f, 0f));
        }
        if ((c >= 3) && (c <= 4))
        {
            return (base.transform.position + new Vector3(this.Inset + ((c - 3) * (iconSize.x + this.Padding.x)), -1f * (iconSize.y + this.Padding.y), 0f));
        }
        return (base.transform.position + new Vector3((c - 5) * (iconSize.x + this.Padding.x), -2f * (iconSize.y + this.Padding.y), 0f));
    }

    private Vector3 GetIconScale(ScenarioMapIcon icon) => 
        new Vector3(this.Scale, this.Scale, this.Scale);

    private Vector2 GetIconSize(ScenarioMapIcon icon)
    {
        Vector3 localScale = icon.transform.localScale;
        icon.transform.localScale = this.GetIconScale(icon);
        Vector2 size = icon.Size;
        icon.transform.localScale = localScale;
        return size;
    }

    public void Initialize()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            this.icons = new ScenarioMapIcon[window.mapPanel.Icons.Count];
            for (int i = 0; i < window.mapPanel.Icons.Count; i++)
            {
                this.icons[i] = UnityEngine.Object.Instantiate<GameObject>(window.mapPanel.Icons[i].gameObject).GetComponent<ScenarioMapIcon>();
                if (this.icons[i] != null)
                {
                    this.icons[i].transform.parent = base.transform;
                }
            }
        }
    }

    public void Show(bool isVisible)
    {
        if (isVisible)
        {
            int num = 0;
            for (int i = 0; i < Scenario.Current.Locations.Length; i++)
            {
                if (Scenario.Current.IsLocationValid(Scenario.Current.Locations[i].LocationName))
                {
                    ScenarioMapIcon icon = this.FindMapIcon(Scenario.Current.Locations[i].LocationName);
                    if (icon != null)
                    {
                        icon.gameObject.transform.position = this.GetIconPosition(icon, num++);
                        icon.gameObject.transform.localScale = this.GetIconScale(icon);
                        icon.SortingOrder = 0x6f;
                        icon.Refresh(true);
                        icon.FadeIn(0.15f);
                    }
                }
            }
        }
    }

    public ScenarioMapIcon[] Icons =>
        this.icons;
}

