using System;
using UnityEngine;

public class GuiPanelRoles : GuiPanel
{
    private bool isPanelOpen;
    [Tooltip("reference to the button that shows/hides everything")]
    public GuiButtonRegion MainButton;
    [Tooltip("reference to the animator attached to this panel")]
    public Animator panelAnimator;
    [Tooltip("reference to the character powers panel in this scene")]
    public GuiPanelCharacterPowers powersPanel;
    [Tooltip("reference to the \"default\" role button")]
    public GuiButton Role0Button;
    [Tooltip("reference to the first role button")]
    public GuiButton Role1Button;
    [Tooltip("reference to the second role button")]
    public GuiButton Role2Button;
    [Tooltip("reference to the label that displays the selected role name")]
    public GuiLabel RoleNameLabel;
    [Tooltip("reference to the roles tutorial intro that explains roles")]
    public GuiPanelTutorialRadioButtons RolesIntro;

    private string GetRoleDescription(int n)
    {
        if ((n >= 0) && (n < this.powersPanel.Character.Roles.Length))
        {
            RoleTableEntry entry = RoleTable.Get(this.powersPanel.Character.Roles[n]);
            if (entry != null)
            {
                return entry.Description;
            }
        }
        return null;
    }

    private string GetRoleName(int n)
    {
        if ((n >= 0) && (n < this.powersPanel.Character.Roles.Length))
        {
            RoleTableEntry entry = RoleTable.Get(this.powersPanel.Character.Roles[n]);
            if (entry != null)
            {
                return entry.Name;
            }
        }
        return null;
    }

    private bool IsRewardSelectionRequired(int n)
    {
        if (n == 0)
        {
            GuiWindowReward window = UI.Window as GuiWindowReward;
            if (window != null)
            {
                RewardFeat reward = window.Reward as RewardFeat;
                if ((reward != null) && reward.Role)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnRole0ButtonPushed()
    {
        if (!base.Paused)
        {
            this.ShowRolePowers(0);
        }
    }

    private void OnRole1ButtonPushed()
    {
        if (!base.Paused)
        {
            this.ShowRolePowers(1);
        }
    }

    private void OnRole2ButtonPushed()
    {
        if (!base.Paused)
        {
            this.ShowRolePowers(2);
        }
    }

    private void OnRolesButtonPushed()
    {
        if (!base.Paused)
        {
            this.isPanelOpen = true;
            this.Role0Button.Text = this.GetRoleName(0);
            this.Role1Button.Text = this.GetRoleName(1);
            this.Role2Button.Text = this.GetRoleName(2);
            this.panelAnimator.SetTrigger("Open");
            if (this.RolesIntro != null)
            {
                this.RolesIntro.Show(false);
            }
            this.Role0Button.Refresh();
            this.Role1Button.Refresh();
            this.Role2Button.Refresh();
        }
    }

    private void PlayIdleAnimation(int role)
    {
        if ((role == 1) || (role == 2))
        {
            this.panelAnimator.Play("roles_small_idle" + role.ToString());
        }
        else
        {
            this.panelAnimator.Play("roles_small_idle");
        }
        this.panelAnimator.SetInteger("SelectedRole", role);
        if (this.isPanelOpen)
        {
            this.panelAnimator.SetTrigger("Close");
        }
    }

    public override void Refresh()
    {
        if (this.MainButton != null)
        {
            this.MainButton.Refresh();
        }
        if (this.RolesIntro != null)
        {
            this.RolesIntro.Refresh();
        }
        this.ShowCurrentRole();
    }

    public override void Show(bool isVisible)
    {
        if (isVisible)
        {
            base.Show(isVisible);
            this.ShowCurrentRole();
        }
        else
        {
            if (this.MainButton != null)
            {
                this.MainButton.Show(true);
            }
            if (this.isPanelOpen)
            {
                this.panelAnimator.SetTrigger("Close");
                this.isPanelOpen = false;
            }
            else if (this.panelAnimator.gameObject.activeInHierarchy)
            {
                this.ShowCurrentRole();
            }
        }
    }

    public void ShowCurrentRole()
    {
        GuiWindowReward window = UI.Window as GuiWindowReward;
        if (window != null)
        {
            RewardFeat reward = window.Reward as RewardFeat;
            if ((reward != null) && (reward.GetSelectedRole(Turn.Number) != null))
            {
                RoleTableEntry selectedRole = reward.GetSelectedRole(Turn.Number);
                for (int i = 0; i < this.powersPanel.Character.Roles.Length; i++)
                {
                    if (RoleTable.Get(this.powersPanel.Character.Roles[i]).Equals(selectedRole))
                    {
                        this.ShowRolePowers(i);
                        this.PlayIdleAnimation(i);
                        return;
                    }
                }
            }
        }
        if (this.powersPanel.Character != null)
        {
            this.ShowRolePowers(this.powersPanel.Character.Role);
            this.PlayIdleAnimation(this.powersPanel.Character.Role);
        }
    }

    private void ShowCurrentRoleTitle(RoleTableEntry entry)
    {
        if ((this.RoleNameLabel != null) && (entry != null))
        {
            this.RoleNameLabel.Text = entry.Name;
        }
    }

    public void ShowRolePowers(int n)
    {
        if (this.IsRewardSelectionRequired(n) && (this.RolesIntro != null))
        {
            this.RolesIntro.Show(true);
        }
        this.powersPanel.Refresh(n);
        this.powersPanel.PowerDescriptionLabel.Text = this.GetRoleDescription(n);
        if ((n >= 0) && (n < this.powersPanel.Character.Roles.Length))
        {
            RoleTableEntry entry = RoleTable.Get(this.powersPanel.Character.Roles[n]);
            this.ShowCurrentRoleTitle(entry);
        }
        this.panelAnimator.SetInteger("SelectedRole", n);
        if (this.isPanelOpen)
        {
            this.panelAnimator.SetTrigger("Close");
        }
        this.isPanelOpen = false;
    }

    public override uint zIndex =>
        Constants.ZINDEX_PANEL_POPUP;
}

