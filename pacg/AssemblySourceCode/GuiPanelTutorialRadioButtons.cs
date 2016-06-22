using System;
using UnityEngine;

public class GuiPanelTutorialRadioButtons : GuiPanelTutorial
{
    private int currentPopup;
    [Tooltip("the popups to display")]
    public GameObject[] Popups;
    [Tooltip("buttons to refresh every time you refresh this panel")]
    public GuiButton[] RefreshButtons;
    [Tooltip("reference to the character powers panel in this scene")]
    public GuiPanelRoles rolesPanel;

    private void OnRoleButton1Pushed()
    {
        this.SelectRole(1);
    }

    private void OnRoleButton2Pushed()
    {
        this.SelectRole(2);
    }

    private void OnRuleButtonPushed()
    {
        GuiPanelOptionsMenu optionsPanel = Game.UI.OptionsPanel as GuiPanelOptionsMenu;
        if (optionsPanel != null)
        {
            optionsPanel.OnRulesSheetButtonPushed();
            optionsPanel.RulesPanel.TogglePane(2);
            optionsPanel.RulesPanel.SetRightPanelText(GlossaryCategory.Terms, 0x29);
        }
    }

    private void OnToggleForward()
    {
        this.Popups[this.currentPopup].SetActive(false);
        this.currentPopup++;
        if (this.currentPopup >= this.Popups.Length)
        {
            this.currentPopup = 0;
        }
        this.Popups[this.currentPopup].SetActive(true);
    }

    private void OnTogglePrev()
    {
        this.Popups[this.currentPopup].SetActive(false);
        this.currentPopup--;
        if (this.currentPopup < 0)
        {
            this.currentPopup = this.Popups.Length - 1;
        }
        this.Popups[this.currentPopup].SetActive(true);
    }

    public override void Refresh()
    {
        base.Refresh();
        for (int i = 0; i < this.RefreshButtons.Length; i++)
        {
            this.RefreshButtons[i].Refresh();
        }
    }

    private void SelectRole(int role)
    {
        this.Show(false);
        this.rolesPanel.ShowRolePowers(role);
        this.rolesPanel.Refresh();
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.Popups[this.currentPopup].SetActive(isVisible);
        UI.Window.Pause(isVisible && this.Visible);
    }

    public override uint zIndex =>
        (Constants.ZINDEX_PANEL_POPUP + 100);
}

