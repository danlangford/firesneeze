using System;
using System.Text;
using UnityEngine;

public class GuiPanelScenarioDifficulty : GuiPanelBackStack
{
    [Tooltip("reference to the \"standard\" difficulty button on this panel")]
    public GuiButton Button0;
    [Tooltip("reference to the \"hero\" difficulty button on this panel")]
    public GuiButton Button1;
    [Tooltip("reference to the \"legend\" difficulty button on this panel")]
    public GuiButton Button2;
    [Tooltip("pointer to the small difficulty 0 button image")]
    public Sprite DifficultyButtonImage0;
    [Tooltip("pointer to the small difficulty 1 button image")]
    public Sprite DifficultyButtonImage1;
    [Tooltip("pointer to the small difficulty 2 button image")]
    public Sprite DifficultyButtonImage2;
    [Tooltip("reference to the first text line on this panel")]
    public GuiLabel DifficultyLine1;
    [Tooltip("reference to the second text line on this panel")]
    public GuiLabel DifficultyLine2;
    [Tooltip("reference to the third text line on this panel (gold)")]
    public GuiLabel DifficultyLine3;
    [Tooltip("reference to the title label on this panel")]
    public GuiLabel DifficultyTitle;
    private string selectedScenario;

    public Sprite GetDifficultyImage(int level)
    {
        if (level <= 0)
        {
            return this.DifficultyButtonImage0;
        }
        if (level == 1)
        {
            return this.DifficultyButtonImage1;
        }
        if (level >= 2)
        {
            return this.DifficultyButtonImage2;
        }
        return null;
    }

    public int GetDifficultyLevel(string ID)
    {
        int scenarioDifficulty = Campaign.GetScenarioDifficulty(ID);
        if (!Campaign.IsScenarioComplete(ID) || (scenarioDifficulty < 0))
        {
            return 0;
        }
        if (scenarioDifficulty == 0)
        {
            return 1;
        }
        return 2;
    }

    public string GetDifficultyName(int level)
    {
        switch (level)
        {
            case 0:
                return UI.Text(0x1c3);

            case 1:
                return UI.Text(0x1c4);

            case 2:
                return UI.Text(0x1c5);
        }
        return null;
    }

    public string GetDifficultyText(string ID, int level)
    {
        StringBuilder builder = new StringBuilder();
        if (!string.IsNullOrEmpty(ID))
        {
            GameObject obj2 = Resources.Load<GameObject>("Blueprints/Scenarios/" + ID);
            if (obj2 != null)
            {
                Scenario component = obj2.GetComponent<Scenario>();
                if (component != null)
                {
                    for (int i = 0; i < component.StartingPowers.Length; i++)
                    {
                        ScenarioPowerValueType type = component.StartingPowers[i];
                        if ((type.Active && (type.Difficulty == level)) && !string.IsNullOrEmpty(type.Description))
                        {
                            builder.AppendLine(type.Description);
                            break;
                        }
                    }
                }
            }
        }
        if (level == 1)
        {
            builder.AppendLine(UI.Text(460));
        }
        if (level == 2)
        {
            builder.AppendLine(UI.Text(0x1ce));
            builder.AppendLine(UI.Text(0x1cd));
        }
        return builder.ToString();
    }

    public override void Initialize()
    {
        base.Initialize();
        this.Show(false);
    }

    private void OnCloseButtonPushed()
    {
        this.Show(false);
        this.selectedScenario = null;
    }

    private void OnDifficultyButton0Pushed()
    {
        this.SetDifficultyLevel(0);
    }

    private void OnDifficultyButton1Pushed()
    {
        this.SetDifficultyLevel(1);
    }

    private void OnDifficultyButton2Pushed()
    {
        this.SetDifficultyLevel(2);
    }

    private void Refresh(int n)
    {
        if (n == 0)
        {
            this.DifficultyTitle.Text = UI.Text(0x1c3);
            this.DifficultyLine1.Text = UI.Text(0x1c9);
            this.DifficultyLine2.Text = UI.Text(0x1ca);
            this.DifficultyLine3.Text = string.Empty;
        }
        if (n == 1)
        {
            this.DifficultyTitle.Text = UI.Text(0x1c4);
            this.DifficultyLine1.Text = UI.Text(0x1cb);
            this.DifficultyLine2.Text = UI.Text(460);
            this.DifficultyLine3.Text = string.Empty;
        }
        if (n == 2)
        {
            this.DifficultyTitle.Text = UI.Text(0x1c5);
            this.DifficultyLine1.Text = UI.Text(0x1cd);
            this.DifficultyLine2.Text = UI.Text(0x1ce);
            this.DifficultyLine3.Text = string.Empty;
        }
        if (!string.IsNullOrEmpty(this.selectedScenario))
        {
            Game.Network.GetScenarioGold(this.selectedScenario, n, delegate (int gold) {
                if (this.DifficultyLine3 != null)
                {
                    string str = UI.Text(0x23b);
                    if (gold > 0)
                    {
                        str = gold + " " + UI.Text(0x1e7);
                    }
                    else if (gold == 0)
                    {
                        str = UI.Text(0x25d);
                    }
                    this.DifficultyLine3.Text = str;
                }
            });
        }
    }

    private void SetDifficultyLevel(int level)
    {
        this.Refresh(level);
        GuiWindowAdventure window = UI.Window as GuiWindowAdventure;
        if (window != null)
        {
            window.SetScenarioDifficulty(level, true);
        }
        else
        {
            GuiWindowQuest quest = UI.Window as GuiWindowQuest;
            if (quest != null)
            {
                quest.SetScenarioDifficulty(level, true);
            }
        }
    }

    public void Show(string id, int level)
    {
        this.Show(true);
        this.selectedScenario = id;
        this.Refresh(level);
        if (Game.GameMode == GameModeType.Story)
        {
            int complete = Conquests.GetComplete(id);
            this.Button0.Disable(false);
            this.Button1.Disable(complete < 0);
            this.Button2.Disable(complete < 1);
        }
        if ((Game.GameMode != GameModeType.Story) || !Settings.Debug.StoryMode)
        {
            this.Button0.Disable(false);
            this.Button1.Disable(false);
            this.Button2.Disable(false);
        }
        if (level <= 0)
        {
            this.Button0.Glow(true);
        }
        if (level == 1)
        {
            this.Button1.Glow(true);
        }
        if (level >= 2)
        {
            this.Button2.Glow(true);
        }
    }

    public override uint zIndex =>
        Constants.ZINDEX_PANEL_POPUP;
}

