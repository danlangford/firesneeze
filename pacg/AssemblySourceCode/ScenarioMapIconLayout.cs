using System;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioMapIconLayout : MonoBehaviour
{
    private readonly Vector2[,] Grid;
    private ScenarioMapIcon[] icons;
    [Tooltip("scale of each token")]
    public float Scale = 1f;
    [Tooltip("relative padding of grid")]
    public float Stretch = 1.2f;
    private float xNudge;
    private float yNudge;

    public ScenarioMapIconLayout()
    {
        Vector2[] vectorArray1 = new Vector2[,] { { new Vector2(-1.5f, 1f), new Vector2(-0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(1.5f, 1f) }, { new Vector2(-1.5f, 0.5f), new Vector2(-0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(1.5f, 0.5f) }, { new Vector2(-1.5f, -0.5f), new Vector2(-0.5f, -0.5f), new Vector2(0.5f, -0.5f), new Vector2(1.5f, -0.5f) }, { new Vector2(-1.5f, -1f), new Vector2(-0.5f, -1f), new Vector2(0.5f, -1f), new Vector2(1.5f, -1f) } };
        this.Grid = vectorArray1;
    }

    private Vector3 Draw3(int i)
    {
        this.xNudge = 1.5f;
        this.yNudge = -2f;
        if (i == 0)
        {
            return this.GridAt(0, 1);
        }
        if (i == 1)
        {
            return this.GridAt(1, 0);
        }
        if (i == 2)
        {
            return this.GridAt(1, 2);
        }
        return base.transform.position;
    }

    private Vector3 Draw4(int i)
    {
        this.xNudge = 1.5f;
        this.yNudge = 0f;
        if (i == 0)
        {
            return this.GridAt(1, 0);
        }
        if (i == 1)
        {
            return this.GridAt(2, 0);
        }
        if (i == 2)
        {
            return this.GridAt(2, 2);
        }
        if (i == 3)
        {
            return this.GridAt(1, 2);
        }
        return base.transform.position;
    }

    private Vector3 Draw5(int i)
    {
        this.xNudge = 1.5f;
        this.yNudge = -0.5f;
        if (i == 0)
        {
            return this.GridAt(0, 1);
        }
        if (i == 1)
        {
            return this.GridAt(1, 0);
        }
        if (i == 2)
        {
            return this.GridAt(2, 0);
        }
        if (i == 3)
        {
            return this.GridAt(2, 2);
        }
        if (i == 4)
        {
            return this.GridAt(1, 2);
        }
        return base.transform.position;
    }

    private Vector3 Draw6(int i)
    {
        this.xNudge = 1f;
        this.yNudge = 0f;
        if (i == 0)
        {
            return this.GridAt(0, 1);
        }
        if (i == 1)
        {
            return this.GridAt(1, 0);
        }
        if (i == 2)
        {
            return this.GridAt(2, 0);
        }
        if (i == 3)
        {
            return this.GridAt(3, 1);
        }
        if (i == 4)
        {
            return this.GridAt(2, 2);
        }
        if (i == 5)
        {
            return this.GridAt(1, 2);
        }
        return base.transform.position;
    }

    private Vector3 Draw7(int i)
    {
        this.xNudge = 0f;
        this.yNudge = 0f;
        if (i == 0)
        {
            return this.GridAt(0, 1);
        }
        if (i == 1)
        {
            return this.GridAt(1, 0);
        }
        if (i == 2)
        {
            return this.GridAt(2, 0);
        }
        if (i == 3)
        {
            return this.GridAt(3, 1);
        }
        if (i == 4)
        {
            return this.GridAt(3, 2);
        }
        if (i == 5)
        {
            return this.GridAt(2, 3);
        }
        if (i == 6)
        {
            return this.GridAt(1, 3);
        }
        return base.transform.position;
    }

    private Vector3 Draw8(int i)
    {
        this.xNudge = 0f;
        this.yNudge = 0f;
        if (i == 0)
        {
            return this.GridAt(0, 1);
        }
        if (i == 1)
        {
            return this.GridAt(1, 0);
        }
        if (i == 2)
        {
            return this.GridAt(2, 0);
        }
        if (i == 3)
        {
            return this.GridAt(3, 1);
        }
        if (i == 4)
        {
            return this.GridAt(3, 2);
        }
        if (i == 5)
        {
            return this.GridAt(2, 3);
        }
        if (i == 6)
        {
            return this.GridAt(1, 3);
        }
        if (i == 7)
        {
            return this.GridAt(0, 2);
        }
        return base.transform.position;
    }

    private Vector3 GetIconPosition(int i, int n)
    {
        if (n <= 3)
        {
            return this.Draw3(i);
        }
        if (n == 4)
        {
            return this.Draw4(i);
        }
        if (n == 5)
        {
            return this.Draw5(i);
        }
        if (n == 6)
        {
            return this.Draw6(i);
        }
        if (n == 7)
        {
            return this.Draw7(i);
        }
        if (n >= 8)
        {
            return this.Draw8(i);
        }
        return base.transform.position;
    }

    private Vector3 GetIconScale(int i) => 
        new Vector3(this.Scale, this.Scale, this.Scale);

    private Vector2 GetIconSize(int i)
    {
        ScenarioMapIcon icon = this.icons[i];
        Vector3 localScale = icon.transform.localScale;
        icon.transform.localScale = this.GetIconScale(i);
        Vector2 size = icon.Size;
        icon.transform.localScale = localScale;
        return size;
    }

    private Vector3 GridAt(int x, int y)
    {
        Vector2 iconSize = this.GetIconSize(0);
        Vector2 vector2 = new Vector2(((iconSize.x * this.Stretch) * this.Grid[x, y].x) + this.xNudge, ((iconSize.y * this.Stretch) * this.Grid[x, y].y) + this.yNudge);
        return new Vector3(base.transform.position.x + vector2.x, base.transform.position.y + vector2.y, base.transform.position.z);
    }

    private void Initialize()
    {
        this.icons = UnityEngine.Object.FindObjectsOfType<ScenarioMapIcon>();
    }

    private void RefreshMapLines()
    {
        if (Scenario.Current.Linear)
        {
            GuiWindowScenario window = UI.Window as GuiWindowScenario;
            if (window != null)
            {
                window.MapPanel.RefreshIconLines();
            }
        }
    }

    public void Show(bool isVisible)
    {
        if (isVisible)
        {
            if (this.icons == null)
            {
                this.Initialize();
            }
            List<ScenarioMapIcon> list = this.Sort(new List<ScenarioMapIcon>(this.icons));
            for (int i = 0; i < list.Count; i++)
            {
                if (Scenario.Current.IsLocationValid(list[i].ID))
                {
                    LeanTween.move(list[i].gameObject, this.GetIconPosition(i, list.Count), 0.25f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(new Action(this.RefreshMapLines));
                    LeanTween.scale(list[i].gameObject, this.GetIconScale(i), 0.25f).setEase(LeanTweenType.easeInOutQuad);
                }
            }
        }
        else
        {
            for (int j = 0; j < this.icons.Length; j++)
            {
                if (Scenario.Current.IsLocationValid(this.icons[j].ID))
                {
                    LeanTween.move(this.icons[j].gameObject, this.icons[j].StartPosition, 0.25f).setEase(LeanTweenType.easeInOutQuad);
                    LeanTween.scale(this.icons[j].gameObject, this.icons[j].StartScale, 0.25f).setEase(LeanTweenType.easeInOutQuad);
                }
            }
        }
    }

    private List<ScenarioMapIcon> Sort(List<ScenarioMapIcon> source)
    {
        List<ScenarioMapIcon> sorted = new List<ScenarioMapIcon>(source.Count);
        List<string> visited = new List<string>(source.Count);
        foreach (ScenarioMapIcon icon in source)
        {
            this.Visit(icon, visited, sorted);
        }
        return sorted;
    }

    private void Visit(ScenarioMapIcon item, List<string> visited, List<ScenarioMapIcon> sorted)
    {
        if (!visited.Contains(item.ID))
        {
            visited.Add(item.ID);
            for (int i = 0; i < Scenario.Current.Locations.Length; i++)
            {
                if (Scenario.Current.IsLocationValid(Scenario.Current.Locations[i].LocationName) && Scenario.Current.Locations[i].IsLinked(item.ID))
                {
                    GuiWindowScenario window = UI.Window as GuiWindowScenario;
                    if (window != null)
                    {
                        this.Visit(window.MapPanel.GetMapIcon(Scenario.Current.Locations[i].LocationName), visited, sorted);
                    }
                }
            }
            sorted.Add(item);
        }
    }
}

