using System;

public class ScenarioTableEntry
{
    public int descriptionStrRef;
    public int henchmenStrRef;
    public string id;
    public int nameStrRef;
    public int number;
    public int powersStrRef;
    public int rewardStrRef;
    public string set;
    public int villainStrRef;

    public string Description =>
        StringTableManager.Get(ScenarioTable.Name, this.descriptionStrRef);

    public string Henchmen =>
        StringTableManager.Get(ScenarioTable.Name, this.henchmenStrRef);

    public string Name =>
        StringTableManager.Get(ScenarioTable.Name, this.nameStrRef);

    public string Powers =>
        StringTableManager.Get(ScenarioTable.Name, this.powersStrRef);

    public string Reward =>
        StringTableManager.Get(ScenarioTable.Name, this.rewardStrRef);

    public string Villain =>
        StringTableManager.Get(ScenarioTable.Name, this.villainStrRef);
}

