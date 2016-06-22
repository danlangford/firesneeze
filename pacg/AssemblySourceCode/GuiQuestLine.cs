using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiQuestLine : GuiElement
{
    [Tooltip("reference to the reward label in our hierarchy")]
    public GuiLabel descriptionLabel;
    [Tooltip("reference to the icon in our hierarchy")]
    public GuiImage iconImage;
    [Tooltip("reference to the level label in our hierarchy")]
    public GuiLabel levelLabel;
    [Tooltip("sprite used for incomplete levels")]
    public Sprite lockedPipImage;
    [Tooltip("sprite used for completed levels")]
    public Sprite unlockedPipImage;

    private string GetDescriptionText(string id)
    {
        GameObject obj2 = Resources.Load<GameObject>("Blueprints/Quests/Rewards/" + id);
        if (obj2 != null)
        {
            Reward component = obj2.GetComponent<Reward>();
            if (component != null)
            {
                return component.Description.ToString();
            }
        }
        return UI.Text(0);
    }

    public void Initialize(int level, string id)
    {
        this.Level = level;
        this.levelLabel.Text = level.ToString();
        this.descriptionLabel.Text = this.GetDescriptionText(id);
        if (UI.Window.Type == WindowType.CreateParty)
        {
            this.SetSorting("Default", -95);
        }
    }

    private void SetSorting(string layer, int order)
    {
        Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            componentsInChildren[i].sortingLayerName = layer;
            componentsInChildren[i].sortingOrder = order;
            if (componentsInChildren[i].name == "Background")
            {
                componentsInChildren[i].sortingOrder = order - 1;
            }
        }
    }

    public bool Complete
    {
        set
        {
            if (value)
            {
                this.iconImage.Image = this.unlockedPipImage;
            }
            else
            {
                this.iconImage.Image = this.lockedPipImage;
            }
        }
    }

    public int Level { get; private set; }
}

