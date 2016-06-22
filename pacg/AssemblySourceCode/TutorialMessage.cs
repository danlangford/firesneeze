using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class TutorialMessage
{
    public string Art;
    public TutorialCommand[] Commands;
    public TutorialCondition[] Conditions;
    public float Delay;
    [HideInInspector]
    public bool Expanded;
    public int id;
    public string Label;
    public bool Optional;
    public bool Sticky;
    public TutorialEventType Trigger;
    public float XOffset = 0.2f;
    public float YOffset = 0.3f;

    public void Clear()
    {
        this.Displayed = false;
        this.Repeat = false;
    }

    private Vector3 GetBoxPosition()
    {
        Vector3 position = new Vector3(this.XOffset, this.YOffset, 0f);
        Vector3 vector2 = UI.Camera.ViewportToWorldPoint(position);
        return new Vector3(vector2.x, vector2.y, 0f);
    }

    public void Invoke()
    {
        for (int i = 0; i < this.Commands.Length; i++)
        {
            if (this.Commands[i] != null)
            {
                this.Commands[i].Invoke();
            }
        }
    }

    public bool IsConditionValid()
    {
        bool flag = true;
        for (int i = 0; i < this.Conditions.Length; i++)
        {
            if (this.Conditions[i] != null)
            {
                bool flag2 = this.Conditions[i].Evaluate();
                if (this.Conditions[i].Not)
                {
                    flag2 = !flag2;
                }
                flag = flag && flag2;
            }
        }
        return flag;
    }

    public bool IsDisplayPossible()
    {
        if (this.Displayed)
        {
            return false;
        }
        if ((Settings.TutorialLevel <= 0) && !this.Sticky)
        {
            return false;
        }
        if (!this.IsConditionValid())
        {
            return false;
        }
        return true;
    }

    public void Show()
    {
        if (this.id >= 0)
        {
            Tutorial.Hide();
            if (string.IsNullOrEmpty(this.Art))
            {
                Game.UI.TutorialPopup.transform.position = this.GetBoxPosition();
                Game.UI.TutorialPopup.Show(this.id);
            }
            else
            {
                this.ShowCustomPanel(this.Art, this.id);
            }
        }
        if (!this.Repeat)
        {
            this.Displayed = true;
        }
    }

    private void ShowCustomPanel(string resourceName, int msgID)
    {
        GameObject prefab = Resources.Load<GameObject>("Blueprints/Gui/" + resourceName);
        if (prefab != null)
        {
            GameObject obj3 = Game.Instance.Create(prefab);
            if (obj3 != null)
            {
                obj3.transform.parent = Tutorial.Script.transform;
                GuiPanelTutorial component = obj3.GetComponent<GuiPanelTutorial>();
                if (component != null)
                {
                    Game.UI.TutorialPopupOverlay = component;
                    Game.UI.TutorialPopupOverlay.transform.position = this.GetBoxPosition();
                    component.Popup.Display(msgID);
                }
            }
        }
    }

    public bool Displayed { get; set; }

    public bool Repeat { get; set; }
}

