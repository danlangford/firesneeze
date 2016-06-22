using System;
using UnityEngine;

public class TutorialCommandLockWindow : TutorialCommand
{
    [Header("[Other Buttons] - Select to Lock; Unselect to Unlock")]
    public bool BlessingButton;
    [Header("[Turn Buttons] - Select to Lock; Unselect to Unlock")]
    public bool CancelButton;
    public bool CharacterPowers;
    public bool CharacterSheetButton;
    public bool DiscardButton;
    [Header("[Encounter Buttons] - Select to Lock; Unselect to Unlock")]
    public bool EncounterButton;
    public bool EndTurnButton;
    public bool ExamineButton;
    public bool GiveButton;
    [Header("[Power Buttons] - Select to Lock; Unselect to Unlock")]
    public bool LocationPowers;
    [Header("[Phase Buttons] - Select to Lock; Unselect to Unlock")]
    public bool MoveButton;
    [Header("[Party Buttons] - Select to Lock; Unselect to Unlock")]
    public bool PartyPanel;
    public bool ProceedButton;
    public bool SummonButton;
    [Header("[Zoom Buttons] - Select to Lock; Unselect to Unlock")]
    public bool ZoomBanishButton;
    public bool ZoomBuryButton;
    public bool ZoomDiscardButton;
    public bool ZoomDisplayButton;
    public bool ZoomRechargeButton;
    public bool ZoomRevealButton;

    public override void Invoke()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.cancelButton.Locked = this.CancelButton;
            window.proceedButton.Locked = this.ProceedButton;
            window.examineButton.Locked = this.ExamineButton;
            window.encounterButton.Locked = this.EncounterButton;
            window.characterSheetButton.Locked = this.CharacterSheetButton;
            window.zoomPanel.zoomBanishButton.Locked = this.ZoomBanishButton;
            window.zoomPanel.zoomBuryButton.Locked = this.ZoomBuryButton;
            window.zoomPanel.zoomDiscardButton.Locked = this.ZoomDiscardButton;
            window.zoomPanel.zoomDisplayButton.Locked = this.ZoomDisplayButton;
            window.zoomPanel.zoomRechargeButton.Locked = this.ZoomRechargeButton;
            window.zoomPanel.zoomRevealButton.Locked = this.ZoomRevealButton;
            window.commandsPanel.MoveButton.Locked = this.MoveButton;
            window.commandsPanel.GiveButton.Locked = this.GiveButton;
            window.commandsPanel.DiscardButton.Locked = this.DiscardButton;
            window.commandsPanel.EndTurnButton.Locked = this.EndTurnButton;
            for (int i = 0; i < window.powersPanel.CharacterButtons.Length; i++)
            {
                window.powersPanel.CharacterButtons[i].Locked = this.CharacterPowers;
            }
            for (int j = 0; j < window.powersPanel.LocationButtons.Length; j++)
            {
                window.powersPanel.LocationButtons[j].Locked = this.LocationPowers;
            }
            for (int k = 0; k < window.powersPanel.ScenarioButtons.Length; k++)
            {
                window.powersPanel.ScenarioButtons[k].Locked = this.LocationPowers;
            }
            for (int m = 0; m < window.partyPanel.characterButtons.Length; m++)
            {
                window.partyPanel.characterButtons[m].Locked = this.PartyPanel;
            }
            window.partyPanel.helpButton.Locked = this.PartyPanel;
            window.blessingsPanel.blessingsButton.Locked = this.CharacterSheetButton;
        }
    }
}

