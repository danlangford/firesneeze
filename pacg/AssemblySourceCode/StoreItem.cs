using System;
using UnityEngine;

public class StoreItem : MonoBehaviour
{
    [Tooltip("which campaign this product belongs to")]
    public CampaignType Campaign;
    [Tooltip("the category of product")]
    public Store.ItemCategory Category = Store.ItemCategory.Invalid;
    [Tooltip("how much this costs in RM or gold (ex 4.99)")]
    public float Cost;
    [Tooltip("whether this product uses RM or gold")]
    public Store.CurrencyCategory Currency;
    [Tooltip("amount of gold (if any) this rewards")]
    public int GoldToReceive;
    [Tooltip("unique; used in coordination with PlayFab Online Catalog")]
    public string ID;
    [Tooltip("which license (if any) this product belongs to")]
    public string License;
    private Adventure myAdventure;
    private Character myCharacter;
    [Tooltip("text to display for the description of this product")]
    public StrRefType ProductDescription;
    [Tooltip("text to display for the product name, only used for non-adventures and non-characters which use their tables")]
    public StrRefType ProductName;
    [Tooltip("if user can hold more than one of these")]
    public bool Stackable;

    public string GetDisplayName
    {
        get
        {
            Store.ItemCategory category = this.Category;
            if (category != Store.ItemCategory.Adventure)
            {
                if (category != Store.ItemCategory.Character)
                {
                    return this.ProductName.ToString();
                }
            }
            else
            {
                if (this.myAdventure == null)
                {
                    this.myAdventure = AdventureTable.Create(this.ID);
                }
                return this.myAdventure.DisplayName;
            }
            if (this.myCharacter == null)
            {
                this.myCharacter = CharacterTable.Create(this.ID);
            }
            return this.myCharacter.DisplayName;
        }
    }
}

