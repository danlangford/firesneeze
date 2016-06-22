using System;
using UnityEngine;

public class GuiPanelCharacterComplete : GuiPanel
{
    [Tooltip("references to the adventure buttons on this panel")]
    public GuiButton[] AdventureButtons;
    [Tooltip("references to the adventure pips on this panel")]
    public GuiImage[] AdventureCheckmarks;
    [Tooltip("reference to the adventure highlight on this panel")]
    public GameObject AdventureHighlight;
    private string[] AdventureSets;
    [Tooltip("reference to the current adventure marker on this panel")]
    public GuiImage CurrentAdventureMarker;
    protected Character currentCharacter;
    [Tooltip("refernece to the current scenario marker on this panel")]
    public GuiImage CurrentScenarioMarker;
    [Tooltip("references to the scenario checkmark icons on this panel")]
    public GuiImage[] ScenarioCheckmarks;
    [Tooltip("references to the scenario labels on this panel")]
    public GuiLabel[] ScenarioLabels;
    [Tooltip("reference to the tab button that brought us here")]
    public GuiButton TabButton;

    public override void Clear()
    {
        for (int i = 0; i < this.AdventureButtons.Length; i++)
        {
            this.AdventureButtons[i].Show(false);
        }
        for (int j = 0; j < this.ScenarioLabels.Length; j++)
        {
            this.ScenarioLabels[j].Show(false);
        }
        for (int k = 0; k < this.ScenarioCheckmarks.Length; k++)
        {
            this.ScenarioCheckmarks[k].Show(false);
        }
        this.CurrentAdventureMarker.Show(false);
        this.CurrentScenarioMarker.Show(false);
        this.AdventureHighlight.SetActive(false);
    }

    public override void Initialize()
    {
        this.AdventureSets = new string[this.AdventureButtons.Length];
        this.Clear();
        this.ShowAdventures();
    }

    private void OnAdventureButton0Pushed()
    {
        this.SelectAdventure(0);
    }

    private void OnAdventureButton1Pushed()
    {
        this.SelectAdventure(1);
    }

    private void OnAdventureButton2Pushed()
    {
        this.SelectAdventure(2);
    }

    private void OnAdventureButton3Pushed()
    {
        this.SelectAdventure(3);
    }

    private void OnAdventureButton4Pushed()
    {
        this.SelectAdventure(4);
    }

    private void OnAdventureButton5Pushed()
    {
        this.SelectAdventure(5);
    }

    private void OnAdventureButton6Pushed()
    {
        this.SelectAdventure(6);
    }

    public override void Refresh()
    {
        if (this.Character != null)
        {
            this.Clear();
            this.ShowAdventures();
            this.SelectCurrentAdventure();
        }
    }

    private void SelectAdventure(int number)
    {
        if (!base.Paused)
        {
            if ((number >= 0) && (number < this.AdventureButtons.Length))
            {
                GuiButton button = this.AdventureButtons[number];
                this.AdventureHighlight.transform.position = new Vector3(this.AdventureHighlight.transform.position.x, button.transform.position.y, this.AdventureHighlight.transform.position.z);
                this.AdventureHighlight.SetActive(true);
            }
            else
            {
                this.AdventureHighlight.SetActive(false);
            }
            string set = "B";
            if (number > 0)
            {
                set = number.ToString();
            }
            this.ShowScenarios(set);
        }
    }

    private void SelectCurrentAdventure()
    {
        int number = 0;
        if (Adventure.Current != null)
        {
            number = Adventure.Current.Number;
        }
        this.SelectAdventure(number);
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.TabButton.Glow(isVisible);
        if (isVisible)
        {
            this.Refresh();
        }
    }

    private void ShowAdventures()
    {
        for (int i = 0; i < this.AdventureButtons.Length; i++)
        {
            this.AdventureCheckmarks[i].Show(false);
            this.AdventureButtons[i].Show(false);
        }
        this.CurrentAdventureMarker.Show(false);
        int index = 0;
        for (int j = 0; j < AdventureTable.Count; j++)
        {
            AdventureTableEntry entry = AdventureTable.Get(j);
            if (((entry != null) && (entry.set != "0")) && (index < this.AdventureButtons.Length))
            {
                this.AdventureSets[index] = entry.set;
                this.AdventureCheckmarks[index].Show(true);
                this.AdventureButtons[index].Show(true);
                this.AdventureButtons[index].Text = entry.Name;
                if ((Adventure.Current != null) && (Adventure.Current.ID == entry.id))
                {
                    this.CurrentAdventureMarker.transform.position = this.AdventureCheckmarks[index].transform.position;
                    this.CurrentAdventureMarker.Show(true);
                }
                index++;
            }
        }
    }

    private void ShowScenarios(string set)
    {
        for (int i = 0; i < this.ScenarioLabels.Length; i++)
        {
            this.ScenarioCheckmarks[i].Show(false);
            this.ScenarioLabels[i].Show(false);
        }
        this.CurrentScenarioMarker.Show(false);
        for (int j = 0; j < ScenarioTable.Count; j++)
        {
            ScenarioTableEntry entry = ScenarioTable.Get(j);
            if ((entry != null) && (entry.set == set))
            {
                int number = entry.number;
                if ((number > 0) && (number < this.ScenarioLabels.Length))
                {
                    this.ScenarioLabels[number].Text = entry.Name;
                    this.ScenarioLabels[number].Show(true);
                    this.ScenarioCheckmarks[number].Show(this.Character.HasCompleted(entry.id));
                    if ((Scenario.Current != null) && (Scenario.Current.ID == entry.id))
                    {
                        this.CurrentScenarioMarker.transform.position = this.ScenarioCheckmarks[number].transform.position;
                        this.CurrentScenarioMarker.Show(true);
                    }
                }
            }
        }
    }

    public Character Character
    {
        get
        {
            if (this.currentCharacter != null)
            {
                return this.currentCharacter;
            }
            if (Party.Characters.Count > Turn.Number)
            {
                return Party.Characters[Turn.Number];
            }
            return null;
        }
        set
        {
            this.currentCharacter = value;
        }
    }

    public override uint zIndex =>
        Constants.ZINDEX_PANEL_FULL;
}

