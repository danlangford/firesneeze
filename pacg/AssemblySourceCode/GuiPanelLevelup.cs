using System;
using System.Collections.Generic;
using UnityEngine;

public class GuiPanelLevelup : GuiPanel
{
    private int currentRewardIndex;
    private List<string>[] grants;
    [Tooltip("screen positions of character icons")]
    public Transform[] IconPositionMarkers;
    private List<Reward> rewards;
    private RewardIconExperience[] xpIcons;

    private Vector3 GetIconPosition(int i)
    {
        Transform transform = this.IconPositionMarkers[Party.Characters.Count - 1];
        return transform.GetChild(i).position;
    }

    public Reward GetNextReward()
    {
        for (int i = this.currentRewardIndex; i < this.rewards.Count; i++)
        {
            this.currentRewardIndex = i;
            if (this.IsRewardOutstanding())
            {
                this.rewards[this.currentRewardIndex].Initialize(UI.Window as GuiWindowReward);
                return this.rewards[this.currentRewardIndex];
            }
        }
        return null;
    }

    public override void Initialize()
    {
        if (Rules.IsQuestRewardAllowed())
        {
            this.rewards = new List<Reward>(5);
            this.grants = new List<string>[Constants.MAX_PARTY_MEMBERS];
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                this.grants[i] = new List<string>(5);
            }
            GameObject original = Resources.Load<GameObject>("Blueprints/Gui/Rewards_Character_XP");
            if (original != null)
            {
                this.xpIcons = new RewardIconExperience[Party.Characters.Count];
                for (int j = 0; j < Party.Characters.Count; j++)
                {
                    Vector3 iconPosition = this.GetIconPosition(j);
                    GameObject obj3 = UnityEngine.Object.Instantiate(original, iconPosition, Quaternion.identity) as GameObject;
                    if (obj3 != null)
                    {
                        obj3.transform.parent = base.transform;
                        this.xpIcons[j] = obj3.GetComponent<RewardIconExperience>();
                        if (this.xpIcons[j] != null)
                        {
                            this.xpIcons[j].Owner = this;
                            this.xpIcons[j].Character = Party.Characters[j];
                        }
                    }
                }
            }
        }
        base.Show(false);
    }

    public bool IsRewardEligible(int n)
    {
        if ((this.currentRewardIndex < 0) || (this.currentRewardIndex >= this.rewards.Count))
        {
            return false;
        }
        if (this.rewards[this.currentRewardIndex] == null)
        {
            return false;
        }
        if (!Party.Characters[n].Alive)
        {
            return false;
        }
        if (!this.grants[n].Contains(this.rewards[this.currentRewardIndex].name))
        {
            return false;
        }
        if (this.rewards[this.currentRewardIndex].HasReward(n))
        {
            return false;
        }
        return true;
    }

    private bool IsRewardOutstanding()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (this.IsRewardOutstanding(i))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsRewardOutstanding(int n)
    {
        if ((this.currentRewardIndex < 0) || (this.currentRewardIndex >= this.rewards.Count))
        {
            return false;
        }
        if (this.rewards[this.currentRewardIndex] == null)
        {
            return false;
        }
        if (!Party.Characters[n].Alive)
        {
            return false;
        }
        if (!this.grants[n].Contains(this.rewards[this.currentRewardIndex].name))
        {
            return false;
        }
        if (this.rewards[this.currentRewardIndex].IsSelected(n))
        {
            return false;
        }
        if (this.rewards[this.currentRewardIndex].HasReward(n))
        {
            return false;
        }
        return true;
    }

    private void OnLevelupContinueButtonPushed()
    {
        this.Show(false);
        UI.Window.Refresh();
        UI.Window.SendMessage("RewardSequenceController");
    }

    public Reward QueueReward(int n, string id)
    {
        this.grants[n].Add(id);
        for (int i = 0; i < this.rewards.Count; i++)
        {
            if (this.rewards[i].name == id)
            {
                return this.rewards[i];
            }
        }
        GameObject prefab = Resources.Load<GameObject>("Blueprints/Quests/Rewards/" + id);
        if (prefab != null)
        {
            GameObject obj3 = Geometry.CreateObject(prefab, id);
            if (obj3 != null)
            {
                Reward component = obj3.GetComponent<Reward>();
                if (component != null)
                {
                    if (component.Priority > 0)
                    {
                        this.rewards.Insert(0, component);
                        return component;
                    }
                    this.rewards.Add(component);
                }
                return component;
            }
        }
        return null;
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            for (int i = 0; i < this.xpIcons.Length; i++)
            {
                if (this.xpIcons[i] != null)
                {
                    this.xpIcons[i].Levelup();
                }
            }
        }
    }

    public override uint zIndex =>
        Constants.ZINDEX_PANEL_POPUP;
}

