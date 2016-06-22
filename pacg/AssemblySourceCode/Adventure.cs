using System;
using UnityEngine;

public class Adventure : MonoBehaviour
{
    [Tooltip("two character suffix for the box to load")]
    public string Box;
    [Tooltip("which campaign does this adventure belong to (\"Runelords\", etc.)")]
    public CampaignType Campaign;
    private static Adventure currentAdventure;
    [Tooltip("X adventure name from XML file")]
    public string DisplayName;
    [Tooltip("X adventure summary from XML file")]
    public string DisplayText;
    [Tooltip("unique; used to lookup text in XML file")]
    public string ID;
    [Tooltip("the deck number of this adventure (0-6)")]
    public int Number;
    [Tooltip("the ID of the previous adventure")]
    public string Prerequisite;
    [Tooltip("list of random powers granted to scenarios")]
    public ScenarioPowerValueType[] RandomPowers;
    [Tooltip("if any selectors match, the given card is removed from the game instead of boxed")]
    public CardSelector[] RemoveInsteadOfBox;
    [Tooltip("X reward text from XML file")]
    public string RewardText;
    [Tooltip("ordered list of scenario IDs used in this adventure")]
    public string[] Scenarios;
    [Tooltip("X adventure deck set name from XML file")]
    public string Set;
    [Tooltip("the ID of the next adventure")]
    public string Successor;

    public DispositionType GetBoxDisposition(Card card)
    {
        if (card.Clone)
        {
            return DispositionType.RemoveFromTheGame;
        }
        return this.GetBoxDisposition(card.ID);
    }

    public DispositionType GetBoxDisposition(string cardID)
    {
        for (int i = 0; i < this.RemoveInsteadOfBox.Length; i++)
        {
            if (this.RemoveInsteadOfBox[i].Match(cardID))
            {
                return DispositionType.RemoveFromTheGame;
            }
        }
        return DispositionType.Box;
    }

    private bool GetIsLicensed() => 
        (((this.Campaign == CampaignType.RiseOfTheRunelords) && (this.Number < 1)) || LicenseManager.GetIsLicensed(this.License));

    public bool GetIsSupported() => 
        (((this.Campaign == CampaignType.RiseOfTheRunelords) && (this.Number < 1)) || LicenseManager.GetIsSupported(this.License));

    public void OnLoadData()
    {
        byte[] buffer;
        if (Game.GetObjectData(this.ID, out buffer))
        {
            ByteStream stream = new ByteStream(buffer);
            if (stream != null)
            {
                stream.ReadInt();
            }
        }
    }

    public void OnSaveData()
    {
        ByteStream stream = new ByteStream();
        if (stream != null)
        {
            stream.WriteInt(1);
            Game.SetObjectData(this.ID, stream.ToArray());
        }
    }

    public bool Available
    {
        get
        {
            if (!this.Supported || !this.Purchased)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(this.Prerequisite))
            {
                return Campaign.IsAdventureComplete(this.Prerequisite);
            }
            return true;
        }
    }

    public bool Completed
    {
        get => 
            Campaign.IsAdventureComplete(this.ID);
        set
        {
            if ((value && !string.IsNullOrEmpty(this.Successor)) && !Campaign.IsAdventureComplete(this.Successor))
            {
                Campaign.SetAdventureUnlocked(this.Successor, true);
            }
            Campaign.SetAdventureComplete(this.ID);
        }
    }

    public static Adventure Current
    {
        get => 
            currentAdventure;
        set
        {
            currentAdventure = value;
        }
    }

    public string License
    {
        get
        {
            if (this.Campaign == CampaignType.RiseOfTheRunelords)
            {
                if (this.Number == 1)
                {
                    return Constants.IAP_LICENSE_AD11;
                }
                if (this.Number == 2)
                {
                    return Constants.IAP_LICENSE_AD12;
                }
                if (this.Number == 3)
                {
                    return Constants.IAP_LICENSE_AD13;
                }
                if (this.Number == 4)
                {
                    return Constants.IAP_LICENSE_AD14;
                }
                if (this.Number == 5)
                {
                    return Constants.IAP_LICENSE_AD15;
                }
                if (this.Number == 6)
                {
                    return Constants.IAP_LICENSE_AD16;
                }
            }
            return null;
        }
    }

    public bool Purchased =>
        this.GetIsLicensed();

    public virtual int Rank =>
        this.Number;

    public Reward Reward =>
        base.GetComponent<Reward>();

    public bool Supported =>
        this.GetIsSupported();
}

