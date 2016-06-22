using System;
using UnityEngine;

public class CharacterTokenSlotLayout : MonoBehaviour
{
    [Tooltip("reference to the label which displays the party tier")]
    public GuiLabel PartyTierLabel;
    [Tooltip("references to slots in the scene ordered from left to right")]
    public CharacterTokenSlot[] Slots;

    private string GetPartyTier()
    {
        int a = 0;
        for (int i = 0; i < this.Slots.Length; i++)
        {
            if (((this.Slots[i] != null) && (this.Slots[i].Token != null)) && (this.Slots[i].Token.Character != null))
            {
                a = Mathf.Max(a, this.Slots[i].Token.Character.Tier);
            }
        }
        return Rules.GetTierName(a);
    }

    public void Refresh()
    {
        bool flag = false;
        for (int i = 0; i < this.Slots.Length; i++)
        {
            if (flag)
            {
                if (this.Slots[i].Token != null)
                {
                    this.Slots[i].Token.OnGuiDrop(this.Slots[i - 1]);
                    this.Slots[i - 1].Token = this.Slots[i].Token;
                    this.Slots[i].Token = null;
                }
            }
            else if (this.Slots[i].Token == null)
            {
                flag = true;
            }
        }
        bool flag2 = false;
        for (int j = 0; j < this.MaxSlots; j++)
        {
            if (this.Slots[j].Token == null)
            {
                if (flag2)
                {
                    this.Slots[j].Show(false);
                }
                else
                {
                    flag2 = true;
                    this.Slots[j].Show(true);
                }
            }
        }
        for (int k = 0; k < this.Slots.Length; k++)
        {
            this.Slots[k].Refresh();
        }
        this.PartyTierLabel.Text = UI.Text(0x1d0) + " " + this.GetPartyTier();
    }

    private void Start()
    {
        for (int i = 0; i < this.Slots.Length; i++)
        {
            this.Slots[i].Layout = this;
        }
        this.Refresh();
    }

    private int MaxSlots =>
        6;
}

