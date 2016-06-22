using System;

public class CardTableEntry
{
    public int descriptionStrRef;
    public string id;
    public int nameStrRef;
    public string set;
    public TraitType[] traits;
    public CardType type2 = CardType.None;

    public string Description =>
        StringTableManager.Get(CardTable.Name, this.descriptionStrRef);

    public string Name =>
        StringTableManager.Get(CardTable.Name, this.nameStrRef);
}

