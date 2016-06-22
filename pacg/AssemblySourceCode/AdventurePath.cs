using System;
using UnityEngine;

public class AdventurePath : MonoBehaviour
{
    [Tooltip("ordered list of adventure IDs in this adventure path")]
    public string[] Adventures;
    [Tooltip("which campaign does this card belong to (\"Runelords\", etc.)")]
    public CampaignType Campaign;
    private static AdventurePath currentAdventurePath;
    [Tooltip("X adventure name from XML file")]
    public string DisplayName;
    [Tooltip("unique; used to lookup text in XML file")]
    public string ID;
    [Tooltip("X reward text from XML file")]
    public string RewardText;
    [Tooltip("X adventure deck set name from XML file")]
    public string Set;

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

    public static AdventurePath Current
    {
        get => 
            currentAdventurePath;
        set
        {
            currentAdventurePath = value;
        }
    }

    public Reward Reward =>
        base.GetComponent<Reward>();
}

