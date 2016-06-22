using System;
using System.Collections.Generic;
using UnityEngine;

public class GuiPanelCharacterPowersLevelup : GuiPanelCharacterPowers
{
    [Tooltip("reference to the check mark icon in this hierarchy")]
    public Animator CheckMarkIcon;
    [Tooltip("can only increase hand size once per level")]
    public GuiButton LevelupHandSizeButton;
    [Tooltip("one button for each possible power")]
    public GuiButton[] LevelupPowerButtons = new GuiButton[5];
    private string[] LevelupPowerIDs = new string[10];
    [Tooltip("one button for each possible proficiency: LA, HA, W")]
    public GuiButton[] LevelupProficiencyButtons = new GuiButton[3];
    private int points;

    private GuiButton GetPowerButton(string id)
    {
        for (int i = 0; i < this.LevelupPowerIDs.Length; i++)
        {
            if (this.LevelupPowerIDs[i] == id)
            {
                return this.LevelupPowerButtons[i];
            }
        }
        return null;
    }

    private GuiButton GetProficiencyButton(ProficencyType type)
    {
        if (type == ProficencyType.LightArmor)
        {
            return this.LevelupProficiencyButtons[0];
        }
        if (type == ProficencyType.HeavyArmor)
        {
            return this.LevelupProficiencyButtons[1];
        }
        if (type == ProficencyType.Weapons)
        {
            return this.LevelupProficiencyButtons[2];
        }
        return null;
    }

    private GuiButton GetSelectedButton()
    {
        RewardFeat reward = (UI.Window as GuiWindowReward).Reward as RewardFeat;
        if ((reward != null) && reward.IsSelected(Turn.Number))
        {
            ProficencyType selectedProficiency = reward.GetSelectedProficiency();
            if (selectedProficiency != ProficencyType.None)
            {
                return this.GetProficiencyButton(selectedProficiency);
            }
            if (reward.GetSelectedHandSize() > 0)
            {
                return this.LevelupHandSizeButton;
            }
            string selectedPower = reward.GetSelectedPower();
            if (selectedPower != null)
            {
                return this.GetPowerButton(selectedPower);
            }
        }
        return null;
    }

    private bool IsLevelupHandSizePossible(int rank)
    {
        if (this.points <= 0)
        {
            return false;
        }
        int handSize = Turn.Character.HandSize;
        int levelupHandSizeMax = Turn.Character.LevelupHandSizeMax;
        if (base.Role != null)
        {
            levelupHandSizeMax = base.Role.HandSize;
        }
        if (rank > levelupHandSizeMax)
        {
            return false;
        }
        return ((rank - handSize) == 1);
    }

    private bool IsLevelupPowerPossible(string id)
    {
        if (this.points <= 0)
        {
            return false;
        }
        PowerTableEntry entry = PowerTable.Get(id);
        if (entry == null)
        {
            return false;
        }
        if ((entry.Rank > 0) && !Turn.Character.HasPower(entry.Requires))
        {
            return false;
        }
        if (Turn.Character.HasPower(id))
        {
            return false;
        }
        return true;
    }

    private bool IsLevelupProficiencyPossible(ProficencyType type)
    {
        if (this.points <= 0)
        {
            return false;
        }
        return (((type == ProficencyType.Weapons) && !Turn.Character.ProficientWithWeapons) || (((type == ProficencyType.LightArmor) && !Turn.Character.ProficientWithLightArmors) || ((type == ProficencyType.HeavyArmor) && !Turn.Character.ProficientWithHeavyArmors)));
    }

    private void OnLevelupHandSizeButtonPushed()
    {
        this.SelectPower(this.LevelupHandSizeButton);
        GuiWindowReward window = UI.Window as GuiWindowReward;
        if (window != null)
        {
            window.OnRewardChosen((int) (Turn.Character.HandSize + 1));
        }
    }

    private void OnLevelupHeavyArmorProficiencyButtonPushed()
    {
        this.SelectPower(this.LevelupProficiencyButtons[1]);
        GuiWindowReward window = UI.Window as GuiWindowReward;
        if (window != null)
        {
            window.OnRewardChosen(ProficencyType.HeavyArmor);
        }
    }

    private void OnLevelupLightArmorProficiencyButtonPushed()
    {
        this.SelectPower(this.LevelupProficiencyButtons[0]);
        GuiWindowReward window = UI.Window as GuiWindowReward;
        if (window != null)
        {
            window.OnRewardChosen(ProficencyType.LightArmor);
        }
    }

    private void OnLevelupPower10ButtonPushed()
    {
        this.OnLevelupPowerButtonPushed(9);
    }

    private void OnLevelupPower1ButtonPushed()
    {
        this.OnLevelupPowerButtonPushed(0);
    }

    private void OnLevelupPower2ButtonPushed()
    {
        this.OnLevelupPowerButtonPushed(1);
    }

    private void OnLevelupPower3ButtonPushed()
    {
        this.OnLevelupPowerButtonPushed(2);
    }

    private void OnLevelupPower4ButtonPushed()
    {
        this.OnLevelupPowerButtonPushed(3);
    }

    private void OnLevelupPower5ButtonPushed()
    {
        this.OnLevelupPowerButtonPushed(4);
    }

    private void OnLevelupPower6ButtonPushed()
    {
        this.OnLevelupPowerButtonPushed(5);
    }

    private void OnLevelupPower7ButtonPushed()
    {
        this.OnLevelupPowerButtonPushed(6);
    }

    private void OnLevelupPower8ButtonPushed()
    {
        this.OnLevelupPowerButtonPushed(7);
    }

    private void OnLevelupPower9ButtonPushed()
    {
        this.OnLevelupPowerButtonPushed(8);
    }

    private void OnLevelupPowerButtonPushed(int powerIndex)
    {
        this.SelectPower(this.LevelupPowerButtons[powerIndex]);
        GuiWindowReward window = UI.Window as GuiWindowReward;
        if (window != null)
        {
            window.OnRewardChosen(this.LevelupPowerIDs[powerIndex]);
        }
    }

    private void OnLevelupWeaponProficiencyButtonPushed()
    {
        this.SelectPower(this.LevelupProficiencyButtons[2]);
        GuiWindowReward window = UI.Window as GuiWindowReward;
        if (window != null)
        {
            window.OnRewardChosen(ProficencyType.Weapons);
        }
    }

    public override void Refresh()
    {
        base.Refresh();
        this.LevelupHandSizeButton.Refresh();
        for (int i = 0; i < this.LevelupProficiencyButtons.Length; i++)
        {
            this.LevelupProficiencyButtons[i].Refresh();
        }
        for (int j = 0; j < this.LevelupPowerButtons.Length; j++)
        {
            this.LevelupPowerButtons[j].Refresh();
        }
        GuiButton selectedButton = this.GetSelectedButton();
        if (selectedButton != null)
        {
            this.CheckMarkIcon.gameObject.SetActive(true);
            this.CheckMarkIcon.transform.position = selectedButton.transform.position;
            this.CheckMarkIcon.SetBool("Select", true);
        }
        else
        {
            this.CheckMarkIcon.gameObject.SetActive(false);
        }
    }

    public override void Refresh(int n)
    {
        base.Refresh(n);
        GuiWindowReward window = UI.Window as GuiWindowReward;
        if (window != null)
        {
            window.OnRewardChosen(base.Role);
        }
    }

    protected override void RefreshHandSize()
    {
        base.RefreshHandSize();
        this.LevelupHandSizeButton.Show(false);
        for (int i = 0; i < base.HandImages.Length; i++)
        {
            int rank = Turn.Character.HandSize + i;
            if (this.IsLevelupHandSizePossible(rank))
            {
                this.LevelupHandSizeButton.transform.position = base.HandImages[i].transform.position;
                this.LevelupHandSizeButton.Show(this.points > 0);
                break;
            }
        }
    }

    protected override void RefreshPowers()
    {
        base.RefreshPowers();
        for (int i = 0; i < this.LevelupPowerButtons.Length; i++)
        {
            this.LevelupPowerButtons[i].Show(false);
        }
        for (int j = 0; j < this.LevelupPowerIDs.Length; j++)
        {
            this.LevelupPowerIDs[j] = null;
        }
        base.PowerNameLabel.Clear();
        base.PowerDescriptionLabel.Clear();
        List<string> list = new List<string>();
        for (int k = 0; k < base.Role.Powers.Length; k++)
        {
            PowerTableEntry entry = PowerTable.Get(base.Role.Powers[k]);
            if ((entry != null) && !list.Contains(entry.Family))
            {
                list.Add(entry.Family);
            }
        }
        int index = 0;
        int num5 = 0;
        for (int m = 0; m < list.Count; m++)
        {
            string str = list[m];
            for (int n = 0; n < base.Role.Powers.Length; n++)
            {
                string iD = base.Role.Powers[n];
                PowerTableEntry entry2 = PowerTable.Get(iD);
                if ((entry2 != null) && (entry2.Family == str))
                {
                    if (((num5 < base.PowerImages.Length) && this.IsLevelupPowerPossible(iD)) && (index < this.LevelupPowerButtons.Length))
                    {
                        this.LevelupPowerButtons[index].transform.position = base.PowerImages[num5].transform.position;
                        this.LevelupPowerButtons[index].Refresh();
                        this.LevelupPowerButtons[index].Show(this.points > 0);
                        this.LevelupPowerIDs[index] = iD;
                        index++;
                    }
                    num5++;
                }
            }
        }
    }

    protected override void RefreshProficiencies()
    {
        base.RefreshProficiencies();
        for (int i = 0; i < this.LevelupProficiencyButtons.Length; i++)
        {
            this.LevelupProficiencyButtons[i].Show(false);
        }
        for (int j = 0; j < base.Role.Proficiencies.Length; j++)
        {
            if (this.IsLevelupProficiencyPossible(base.Role.Proficiencies[j]))
            {
                GuiButton proficiencyButton = this.GetProficiencyButton(base.Role.Proficiencies[j]);
                if (proficiencyButton != null)
                {
                    int proficencyIndex = base.GetProficencyIndex(base.Role.Proficiencies[j]);
                    proficiencyButton.transform.position = base.ProficencyImages[proficencyIndex].transform.position;
                    proficiencyButton.Show(this.points > 0);
                }
            }
        }
    }

    public override void Reset()
    {
        base.powerScroller.Reset();
        if ((base.PowerIcons.Length > 0) && (base.PowerIcons[0] != null))
        {
            base.LeftMargin = base.PowerIcons[0].transform.localPosition.x;
        }
    }

    private void SelectPower(GuiButton button)
    {
        this.CheckMarkIcon.transform.position = button.transform.position;
        this.CheckMarkIcon.gameObject.SetActive(true);
        this.CheckMarkIcon.SetBool("Select", true);
    }

    public void SetPoints(int n)
    {
        this.points = n;
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (!isVisible)
        {
            this.CheckMarkIcon.gameObject.SetActive(false);
        }
    }
}

