using OEIFormats.FlowCharts;
using System;
using UnityEngine;

public static class Conditionals
{
    [ScriptParam0("Card ID", "ID of the card", "ID"), ConditionalScript("Campaign Has Encountered", @"Conditionals\Campaign")]
    public static bool CampaignHasEncountered(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }
        return Campaign.IsCardEncountered(id);
    }

    [ConditionalScript("Character Turn", @"Conditionals\Character"), ScriptParam0("Character ID", "ID of the character or EMPTY for the current character", "ID")]
    public static bool CharacterTurn(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }
        if (Turn.Owner == null)
        {
            return false;
        }
        return (Turn.Owner.ID == id);
    }

    public static bool Invoke(CutsceneActor actor, ConditionalCall call)
    {
        bool flag = false;
        if (call.Data.FullName.Contains("PartyHasCard") && (call.Data.Parameters.Count >= 1))
        {
            flag = PartyHasCard(call.Data.Parameters[0]);
        }
        if (call.Data.FullName.Contains("PartyOfOne"))
        {
            flag = PartyOfOne();
        }
        if (call.Data.FullName.Contains("PartyContains") && (call.Data.Parameters.Count >= 1))
        {
            flag = PartyContains(call.Data.Parameters[0]);
        }
        if (call.Data.FullName.Contains("CharacterTurn"))
        {
            if ((call.Data.Parameters.Count >= 1) && (call.Data.Parameters[0] != "ID"))
            {
                flag = CharacterTurn(call.Data.Parameters[0]);
            }
            else if (actor != null)
            {
                flag = CharacterTurn(actor.ID);
            }
        }
        if (call.Data.FullName.Contains("CampaignHasEncountered") && (call.Data.Parameters.Count >= 1))
        {
            flag = CampaignHasEncountered(call.Data.Parameters[0]);
        }
        if (call.Data.FullName.Contains("ScenarioChampion") && (call.Data.Parameters.Count >= 2))
        {
            flag = ScenarioChampion(call.Data.Parameters[0], call.Data.Parameters[1]);
        }
        return flag;
    }

    [ConditionalScript("Party Contains", @"Conditionals\Party"), ScriptParam0("Character ID", "ID of the character", "ID")]
    public static bool PartyContains(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }
        return (Party.IndexOf(id) >= 0);
    }

    [ConditionalScript("Party Has Card", @"Conditionals\Cards"), ScriptParam0("Card ID", "ID of the card", "ID")]
    public static bool PartyHasCard(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (Party.Characters[i].Deck[id] != null)
                {
                    return true;
                }
                if (Party.Characters[i].Hand[id] != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    [ConditionalScript("Party Of One", @"Conditionals\Party")]
    public static bool PartyOfOne() => 
        (Party.Characters.Count == 1);

    [ScriptParam0("Scenario ID", "ID of the scenario", "ID"), ScriptParam1("Character ID", "ID of the character", "ID"), ConditionalScript("Scenario Champion", @"Conditionals\Campaign")]
    public static bool ScenarioChampion(string scenarioId, string characterId)
    {
        if (!string.IsNullOrEmpty(scenarioId))
        {
            if (string.IsNullOrEmpty(characterId))
            {
                return false;
            }
            string scenarioChampion = Campaign.GetScenarioChampion(scenarioId);
            Character character = Party.Characters[scenarioChampion];
            if ((character != null) && character.Alive)
            {
                return (character.ID == characterId);
            }
            GameObject obj2 = Resources.Load<GameObject>("Blueprints/Scenarios/" + scenarioId);
            if (obj2 != null)
            {
                Scenario component = obj2.GetComponent<Scenario>();
                if (component != null)
                {
                    ScenarioPropertyChampion champion = component.GetComponent<ScenarioPropertyChampion>();
                    if (champion != null)
                    {
                        return (champion.GetChampion() == characterId);
                    }
                }
            }
        }
        return false;
    }
}

