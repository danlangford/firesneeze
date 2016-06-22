using System;
using UnityEngine;

public class GuiPanelStoreCharactersInfo : GuiPanelBackStack
{
    public GuiPanelStoreCharacters CharacterPanel;
    private Character currentCharacter;
    private int currentPane;
    private FingerGesture finger;
    [Tooltip("reference to biography sub-panel in our hierarchy")]
    public GuiPanelCharacterBio Pane0;
    [Tooltip("reference to skills sub-panel 1 in our hierarchy")]
    public GuiPanelCharacterSkills Pane1;
    [Tooltip("reference to powers sub-panel 2 in our hierarchy")]
    public GuiPanelCharacterPowers Pane2;
    [Tooltip("reference to cards sub-panel 3 in our hierarchy")]
    public GuiPanelCharacterCards Pane3;
    [Tooltip("reference to the corner close button on the 'see more' panel in the scene")]
    public GuiButton SeeMoreCloseButton;
    [Tooltip("reference to the store manager window in the scene")]
    public GuiWindowStore StoreManager;
    public GuiButton Tab0Button;
    public GuiButton Tab1Button;
    public GuiButton Tab2Button;
    public GuiButton Tab3Button;

    public void DisplayCharacterDetails(Character c)
    {
        if (c != null)
        {
            CharacterTableEntry entry = CharacterTable.Get(c.ID);
            c.DisplayName = entry.Name;
            c.Set = entry.set;
            c.DisplayText = entry.Description;
            this.Pane0.Character = c;
            this.Pane1.Character = c;
            this.Pane2.Character = c;
            this.Pane3.Character = c;
            if (this.currentPane == 0)
            {
                this.Pane0.Refresh();
            }
            if (this.currentPane == 1)
            {
                this.Pane1.Refresh();
            }
            this.Pane2.Show(this.currentPane == 2);
            this.Pane2.RolePanel.gameObject.SetActive(this.currentPane == 2);
            this.Pane2.Refresh();
            if (this.currentPane == 3)
            {
                this.Pane3.Refresh();
            }
        }
    }

    public override void Initialize()
    {
        this.Pane0.Initialize();
        this.Pane0.Show(true);
        this.Pane1.Initialize();
        this.Pane1.Show(false);
        this.Pane2.Initialize();
        this.Pane2.Show(false);
        this.Pane3.Initialize();
        this.Pane3.Show(false);
        this.Pane2.RolePanel.gameObject.SetActive(false);
        this.Show(false);
    }

    private void OnPane0ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.currentPane = 0;
            this.Pane0.Show(true);
            this.Pane1.Show(false);
            this.Pane2.Show(false);
            this.Pane2.RolePanel.gameObject.SetActive(false);
            this.Pane3.Show(false);
        }
    }

    private void OnPane1ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.currentPane = 1;
            this.Pane0.Show(false);
            this.Pane1.Show(true);
            this.Pane2.Show(false);
            this.Pane2.RolePanel.gameObject.SetActive(false);
            this.Pane3.Show(false);
        }
    }

    private void OnPane2ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.currentPane = 2;
            this.Pane0.Show(false);
            this.Pane1.Show(false);
            this.Pane2.Show(true);
            this.Pane2.RolePanel.gameObject.SetActive(true);
            this.Pane3.Show(false);
        }
    }

    private void OnPane3ButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            this.currentPane = 3;
            this.Pane0.Show(false);
            this.Pane1.Show(false);
            this.Pane2.Show(false);
            this.Pane2.RolePanel.gameObject.SetActive(false);
            this.Pane3.Show(true);
        }
    }

    private void OnSeeMoreCloseButtonPushed()
    {
        if (!UI.Busy)
        {
            this.CharacterPanel.CloseSeeMorePanel();
        }
    }

    public override void Refresh()
    {
        this.Clear();
    }

    private string Select()
    {
        for (int i = 0; i < LicenseTable.Count; i++)
        {
            string iD = LicenseTable.Key(i);
            LicenseTableEntry entry = LicenseTable.Get(iD);
            if ((entry != null) && (entry.Type == LicenseType.Character))
            {
                return iD;
            }
        }
        return null;
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            this.Refresh();
        }
        else
        {
            this.SeeMoreCloseButton.Show(true);
            UI.Busy = false;
        }
    }

    public override bool Fullscreen =>
        true;
}

