using System;
using UnityEngine;

public class RewardFeat : Reward
{
    [Tooltip("if true, the player picks a card feat")]
    public bool Card;
    private RewardPanelFeat panel;
    [Tooltip("if true, the player picks a power feat")]
    public bool Power;
    [Tooltip("if true, the player picks a non-starter role card")]
    public bool Role;
    private CardType[] selectedCardFeat;
    private bool[] selectedFeat;
    private int[] selectedHandSize;
    private string[] selectedPowerFeat;
    private ProficencyType[] selectedProficiency;
    private RoleTableEntry[] selectedRole;
    private AttributeType[] selectedSkillFeat;
    [Tooltip("if true, the player picks a skill feat")]
    public bool Skill;

    public override void Deliver()
    {
        if (this.Card)
        {
            for (int i = 0; i < this.selectedCardFeat.Length; i++)
            {
                if (this.selectedCardFeat[i] != CardType.None)
                {
                    Party.Characters[i].Levelup(this.selectedCardFeat[i], 1);
                }
            }
        }
        if (this.Skill)
        {
            for (int j = 0; j < this.selectedSkillFeat.Length; j++)
            {
                if (this.selectedSkillFeat[j] != AttributeType.None)
                {
                    Party.Characters[j].Levelup(this.selectedSkillFeat[j], 1);
                }
            }
        }
        if (this.Power)
        {
            for (int k = 0; k < this.selectedPowerFeat.Length; k++)
            {
                if (this.selectedPowerFeat[k] != null)
                {
                    Party.Characters[k].Levelup(this.selectedPowerFeat[k]);
                }
            }
            for (int m = 0; m < this.selectedHandSize.Length; m++)
            {
                if (this.selectedHandSize[m] > 0)
                {
                    Party.Characters[m].Levelup(this.selectedHandSize[m]);
                }
            }
            for (int n = 0; n < this.selectedProficiency.Length; n++)
            {
                if (this.selectedProficiency[n] != ProficencyType.None)
                {
                    Party.Characters[n].Levelup(this.selectedProficiency[n]);
                }
            }
        }
        if (this.Role)
        {
            for (int num6 = 0; num6 < this.selectedRole.Length; num6++)
            {
                if (this.selectedRole[num6] != null)
                {
                    Party.Characters[num6].LevelupRole(this.selectedRole[num6]);
                }
            }
        }
    }

    protected override void Generate()
    {
        if (this.Card)
        {
            this.panel.ShowCardsPane();
        }
        if (this.Skill)
        {
            this.panel.ShowSkillsPane();
        }
        if (this.Power)
        {
            this.panel.ShowPowersPane();
        }
        if (this.Role)
        {
            this.panel.ShowRolesPane();
        }
    }

    public override float GetInitializationTime() => 
        2f;

    protected override string GetRewardPanelName() => 
        "Reward_Prefab_CharacterCard";

    public CardType GetSelectedCard() => 
        this.selectedCardFeat[Turn.Number];

    public int GetSelectedHandSize() => 
        this.selectedHandSize[Turn.Number];

    public string GetSelectedPower() => 
        this.selectedPowerFeat[Turn.Number];

    public ProficencyType GetSelectedProficiency() => 
        this.selectedProficiency[Turn.Number];

    public RoleTableEntry GetSelectedRole(int index) => 
        this.selectedRole[index];

    public AttributeType GetSelectedSkill() => 
        this.selectedSkillFeat[Turn.Number];

    public override void Initialize(GuiWindowReward window)
    {
        base.Initialize(window);
        RewardPanelFeat[] componentsInChildren = base.myPanel.GetComponentsInChildren<RewardPanelFeat>(true);
        this.panel = componentsInChildren[0];
        this.panel.Initialize();
        this.selectedFeat = new bool[Party.Characters.Count];
        for (int i = 0; i < this.selectedFeat.Length; i++)
        {
            this.selectedFeat[i] = false;
        }
        this.selectedCardFeat = new CardType[Party.Characters.Count];
        for (int j = 0; j < this.selectedCardFeat.Length; j++)
        {
            this.selectedCardFeat[j] = CardType.None;
        }
        this.selectedSkillFeat = new AttributeType[Party.Characters.Count];
        for (int k = 0; k < this.selectedSkillFeat.Length; k++)
        {
            this.selectedSkillFeat[k] = AttributeType.None;
        }
        this.selectedPowerFeat = new string[Party.Characters.Count];
        for (int m = 0; m < this.selectedPowerFeat.Length; m++)
        {
            this.selectedPowerFeat[m] = null;
        }
        this.selectedProficiency = new ProficencyType[Party.Characters.Count];
        for (int n = 0; n < this.selectedProficiency.Length; n++)
        {
            this.selectedProficiency[n] = ProficencyType.None;
        }
        this.selectedHandSize = new int[Party.Characters.Count];
        for (int num6 = 0; num6 < this.selectedHandSize.Length; num6++)
        {
            this.selectedHandSize[num6] = 0;
        }
        this.selectedRole = new RoleTableEntry[Party.Characters.Count];
        for (int num7 = 0; num7 < this.selectedRole.Length; num7++)
        {
            this.selectedRole[num7] = null;
        }
    }

    public override bool IsSelected(int n)
    {
        if (this.selectedFeat == null)
        {
            return false;
        }
        return ((!this.Role || ((this.selectedRole[n] != null) && !RoleTable.Get(Party.Characters[n].Roles[0]).Equals(this.selectedRole[n]))) && this.selectedFeat[n]);
    }

    public override void Refresh()
    {
        this.panel.HilightTabs(true);
        this.panel.Refresh();
    }

    public override void Select(AttributeType attribute)
    {
        this.selectedFeat[Turn.Number] = true;
        this.selectedSkillFeat[Turn.Number] = attribute;
    }

    public override void Select(CardType card)
    {
        this.selectedFeat[Turn.Number] = true;
        this.selectedCardFeat[Turn.Number] = card;
    }

    public override void Select(ProficencyType proficiency)
    {
        this.selectedFeat[Turn.Number] = true;
        this.selectedProficiency[Turn.Number] = proficiency;
        this.selectedHandSize[Turn.Number] = 0;
        this.selectedPowerFeat[Turn.Number] = null;
    }

    public override void Select(RoleTableEntry role)
    {
        this.selectedRole[Turn.Number] = role;
    }

    public override void Select(int handSizeDelta)
    {
        this.selectedFeat[Turn.Number] = true;
        this.selectedHandSize[Turn.Number] = handSizeDelta;
        this.selectedPowerFeat[Turn.Number] = null;
        this.selectedProficiency[Turn.Number] = ProficencyType.None;
    }

    public override void Select(string power)
    {
        this.selectedFeat[Turn.Number] = true;
        this.selectedPowerFeat[Turn.Number] = power;
        this.selectedHandSize[Turn.Number] = 0;
        this.selectedProficiency[Turn.Number] = ProficencyType.None;
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            this.panel.Refresh();
        }
    }
}

