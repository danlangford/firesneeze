using System;
using System.Collections.Generic;
using UnityEngine;

public class GuiPanelLoad : GuiPanelBackStack
{
    private List<GameSaveIcon> Icons = new List<GameSaveIcon>(20);
    private readonly float iconWidth = 5f;
    [Tooltip("reference to the \"new game\" prefab in this scene")]
    public GameSaveIcon NewGameIcon;
    private TKPanRecognizer panRecognizer;
    [Tooltip("reference to the \"ask\" panel prefab in this scene")]
    public GuiPanelMenuAsk Popup;

    private void CleanupProfiles()
    {
        if (this.Icons != null)
        {
            for (int i = this.Icons.Count - 1; i >= 0; i--)
            {
                if (this.Icons[i].Locked)
                {
                    UnityEngine.Object.Destroy(this.Icons[i].gameObject);
                    this.Icons.RemoveAt(i);
                }
            }
        }
    }

    private GameObject GetIconTemplate(Transform root)
    {
        if (this.Icons.Count > 1)
        {
            return this.Icons[1].gameObject;
        }
        GameSaveIcon[] componentsInChildren = root.GetComponentsInChildren<GameSaveIcon>(true);
        if (componentsInChildren.Length > 1)
        {
            return componentsInChildren[1].gameObject;
        }
        return null;
    }

    private Vector3 GetLayoutPosition(int i)
    {
        float num = UI.Camera.ViewportToWorldPoint(Vector3.zero).x + (this.iconWidth / 2f);
        float x = base.transform.position.x - num;
        return ((base.transform.position - new Vector3(x, 0f, 0f)) + ((Vector3) (i * new Vector3(this.iconWidth, 0f, 0f))));
    }

    public override void Initialize()
    {
        this.panRecognizer = new TKPanRecognizer(Device.GetMinimumDragDistance());
        this.panRecognizer.zIndex = 2;
        this.panRecognizer.gestureRecognizedEvent += delegate (TKPanRecognizer r) {
            if (!UI.Busy)
            {
                this.OnGuiPan(this.panRecognizer.deltaTranslation);
            }
        };
        this.panRecognizer.gestureCompleteEvent += delegate (TKPanRecognizer r) {
            if (!UI.Busy)
            {
                this.OnGuiPanStop(this.panRecognizer.deltaTranslation);
            }
        };
        TouchKit.addGestureRecognizer(this.panRecognizer);
        this.panRecognizer.enabled = false;
    }

    private void InitializeIcons()
    {
        this.Icons.Clear();
        Transform root = base.transform.FindChild("Icons");
        this.Icons.AddRange(root.GetComponentsInChildren<GameSaveIcon>(true));
        int num = 0;
        for (int i = GameDirectory.FirstSlot; i <= GameDirectory.LastSlot; i++)
        {
            if (num < this.Icons.Count)
            {
                this.Icons[num].Show(false);
                this.Icons[num].Owner = this;
                this.Icons[num].Load(i);
                num++;
            }
        }
        int index = -1;
        for (int j = 0; j < this.Icons.Count; j++)
        {
            if (this.Icons[j].Empty)
            {
                index = j;
                break;
            }
        }
        this.NewGameIcon.Show(index >= 0);
        if (index >= 0)
        {
            this.NewGameIcon.Load(this.Icons[index].Slot);
            this.NewGameIcon.Owner = this;
            this.Icons.RemoveAt(index);
            this.Icons.Insert(0, this.NewGameIcon);
        }
        if (Settings.DebugMode)
        {
            for (int n = 0; n < ProfileTable.Count; n++)
            {
                GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(this.GetIconTemplate(root));
                if (obj2 != null)
                {
                    GameSaveIcon component = obj2.GetComponent<GameSaveIcon>();
                    if (component != null)
                    {
                        component.Load(ProfileTable.Key(n), ProfileTable.Get(n));
                        component.Locked = true;
                        this.Icons.Add(component);
                    }
                }
            }
        }
        for (int k = this.Icons.Count - 1; k >= 1; k--)
        {
            if (this.Icons[k].Empty)
            {
                this.Icons[k].Show(false);
                this.Icons.RemoveAt(k);
            }
        }
        for (int m = 0; m < this.Icons.Count; m++)
        {
            this.Icons[m].transform.position = this.GetLayoutPosition(m);
        }
    }

    private void OnCloseButtonPushed()
    {
        if (!base.Paused)
        {
            this.Pause(true);
            this.Show(false);
            UI.Window.Pause(false);
            UI.Window.Show(true);
        }
    }

    private void OnGuiPan(Vector2 deltaTranslation)
    {
        if (this.Icons.Count > 1)
        {
            float x = Geometry.GetPanDistance(0f, 5f, 1f, this.Icons[0].transform, deltaTranslation, this.Icons[this.Icons.Count - 1].transform);
            for (int i = 0; i < this.Icons.Count; i++)
            {
                GameSaveIcon icon = this.Icons[i];
                icon.transform.position += new Vector3(x, 0f, 0f);
            }
        }
    }

    private void OnGuiPanStop(Vector2 deltaTranslation)
    {
        if (this.Icons.Count > 1)
        {
            float x = Geometry.GetPanStopDistance(0f, 5f, 1f, this.Icons[0].transform, deltaTranslation, this.Icons[this.Icons.Count - 1].transform);
            for (int i = 0; i < this.Icons.Count; i++)
            {
                GameSaveIcon icon = this.Icons[i];
                Vector3 to = icon.transform.position + new Vector3(x, 0f, 0f);
                LeanTween.move(icon.gameObject, to, 0.3f).setEase(LeanTweenType.easeOutQuad);
            }
            LeanTween.delayedCall(0.3f, new Action(this.RefreshButtonPositions));
        }
    }

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        this.panRecognizer.enabled = !isPaused;
    }

    public override void Refresh()
    {
        for (int i = this.Icons.Count - 1; i >= 1; i--)
        {
            if (this.Icons[i].Empty)
            {
                this.Icons[i].Show(false);
                this.Icons.RemoveAt(i);
            }
        }
        for (int j = 0; j < this.Icons.Count; j++)
        {
            this.Icons[j].Show(true);
            LeanTween.move(this.Icons[j].gameObject, this.GetLayoutPosition(j), 0.15f).setEase(LeanTweenType.easeInOutQuad);
        }
        LeanTween.delayedCall(0.15f, new Action(this.RefreshButtonPositions));
    }

    private void RefreshButtonPositions()
    {
        for (int i = 0; i < this.Icons.Count; i++)
        {
            this.Icons[i].Refresh();
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.Pause(!isVisible);
        if (isVisible)
        {
            this.CleanupProfiles();
            this.InitializeIcons();
            this.Refresh();
        }
        else
        {
            this.CleanupProfiles();
        }
    }
}

