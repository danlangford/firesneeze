using System;
using UnityEngine;

public class RewardPanelFeat : GuiPanel
{
    [Tooltip("reference to the vfx to hilight the given tab")]
    public GameObject ButtonHilight;
    [Tooltip("reference to skills sub-panel 1 in our hierarchy")]
    public GuiPanelCharacterSkillsLevelup Pane1;
    [Tooltip("reference to powers sub-panel 2 in our hierarchy")]
    public GuiPanelCharacterPowersLevelup Pane2;
    [Tooltip("reference to cards sub-panel 3 in our hierarchy")]
    public GuiPanelCharacterCardsLevelup Pane3;
    [Tooltip("reference to completion sub-panel 4 in our hierarchy")]
    public GuiPanelCharacterComplete Pane4;
    [Tooltip("reference to quest sub-panel 5 in our hierarchy")]
    public GuiPanelCharacterQuests Pane5;
    [Tooltip("reference to roles sub-panel 5 in our hierarchy")]
    public GuiPanelRoles Pane6;
    [Tooltip("reference to the screen animator in this scene")]
    public Animator ScreenAnimator;

    private void HilightTab(GuiButton button)
    {
        this.ButtonHilight.transform.position = button.transform.position;
    }

    public void HilightTabs(bool isVisible)
    {
        this.ButtonHilight.SetActive(isVisible);
        if (this.Pane1.Visible)
        {
            this.HilightTab(this.Pane1.TabButton);
        }
        if (this.Pane2.Visible)
        {
            this.HilightTab(this.Pane2.TabButton);
        }
        if (this.Pane3.Visible)
        {
            this.HilightTab(this.Pane3.TabButton);
        }
        if (this.Pane4.Visible)
        {
            this.HilightTab(this.Pane4.TabButton);
        }
        if (this.Pane5.Visible)
        {
            this.HilightTab(this.Pane4.TabButton);
        }
    }

    public override void Initialize()
    {
        this.Pane1.Initialize();
        this.Pane2.Initialize();
        this.Pane3.Initialize();
        this.Pane4.Initialize();
        this.Pane5.Initialize();
        this.Pane6.Initialize();
        this.ButtonHilight.SetActive(false);
        this.ScreenAnimator.SetTrigger("Feat");
        UI.Lock(2f);
    }

    private void OnPane1ButtonPushed()
    {
        this.Pane1.Show(true);
        this.Pane2.Show(false);
        this.Pane3.Show(false);
        this.Pane4.Show(false);
        this.Pane5.Show(false);
        this.Pane1.Refresh();
    }

    private void OnPane2ButtonPushed()
    {
        this.Pane1.Show(false);
        this.Pane2.Show(true);
        this.Pane3.Show(false);
        this.Pane4.Show(false);
        this.Pane5.Show(false);
        this.Pane2.Reset();
        this.Pane2.Refresh();
    }

    private void OnPane3ButtonPushed()
    {
        this.Pane1.Show(false);
        this.Pane2.Show(false);
        this.Pane3.Show(true);
        this.Pane4.Show(false);
        this.Pane5.Show(false);
    }

    private void OnPane4ButtonPushed()
    {
        if (!Rules.IsQuestRewardAllowed())
        {
            this.Pane1.Show(false);
            this.Pane2.Show(false);
            this.Pane3.Show(false);
            this.Pane4.Show(true);
            this.Pane5.Show(false);
        }
        else
        {
            this.Pane1.Show(false);
            this.Pane2.Show(false);
            this.Pane3.Show(false);
            this.Pane4.Show(false);
            this.Pane5.Show(true);
        }
    }

    private void OnPane6ButtonPushed()
    {
        this.Pane1.Show(false);
        this.Pane2.Show(true);
        this.Pane3.Show(false);
        this.Pane4.Show(false);
        this.Pane5.Show(false);
        this.Pane6.Show(true);
        this.Pane2.Reset();
        this.Pane2.Refresh();
        this.Pane6.Refresh();
        this.Pane6.Role0Button.Locked = true;
    }

    public override void Refresh()
    {
        this.Pane1.Refresh();
        this.Pane1.TabButton.Refresh();
        this.Pane2.Refresh();
        this.Pane2.TabButton.Refresh();
        this.Pane3.Refresh();
        this.Pane3.TabButton.Refresh();
        this.Pane4.Refresh();
        this.Pane4.TabButton.Refresh();
        this.Pane5.Refresh();
        this.Pane5.TabButton.Refresh();
        this.Pane6.Refresh();
    }

    public void ShowCardsPane()
    {
        this.Pane1.SetPoints(0);
        this.Pane2.SetPoints(0);
        this.Pane3.SetPoints(1);
        this.Pane3.Refresh();
        this.OnPane3ButtonPushed();
    }

    public void ShowCompletionPane()
    {
        this.Pane4.Refresh();
        this.Pane5.Refresh();
        this.OnPane4ButtonPushed();
    }

    public void ShowPowersPane()
    {
        this.Pane1.SetPoints(0);
        this.Pane2.SetPoints(1);
        this.Pane3.SetPoints(0);
        this.Pane2.Refresh();
        this.OnPane2ButtonPushed();
    }

    public void ShowRolesPane()
    {
        this.OnPane6ButtonPushed();
    }

    public void ShowSkillsPane()
    {
        this.Pane1.SetPoints(1);
        this.Pane2.SetPoints(0);
        this.Pane3.SetPoints(0);
        this.Pane1.Refresh();
        this.OnPane1ButtonPushed();
    }
}

