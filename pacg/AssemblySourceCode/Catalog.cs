using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName="Catalog", menuName="Store/Catalog")]
public class Catalog : ScriptableObject
{
    public List<StoreItem> Adventures;
    public List<StoreItem> Characters;
    public List<StoreItem> Gold;
    public List<StoreItem> Promotionals;
    public List<StoreItem> SeasonPasses;
    public StoreItem TreasureChest;
}

