using System;
using System.Collections.Generic;
using UnityEngine.Analytics;

public static class AnalyticsManager
{
    private static Dictionary<string, object> eventData = new Dictionary<string, object>();

    public static void OnBaneEncountered(string cardId, bool isDefeated)
    {
        eventData["card_id"] = cardId;
        eventData["is_defeated"] = isDefeated;
        SendAnalyticsEvent("bane_encountered", eventData, EventType.Player);
    }

    public static void OnBoonEncountered(string cardId, bool isAquired)
    {
        eventData["card_id"] = cardId;
        eventData["is_aquired"] = isAquired;
        SendAnalyticsEvent("boon_encountered", eventData, EventType.Player);
    }

    public static void OnCardAction(Card card, ActionType action)
    {
        OnCardAction(card.ID, action.ToString(), (Adventure.Current == null) ? "N/A" : Adventure.Current.ID, (Scenario.Current == null) ? "N/A" : Scenario.Current.ID, (Turn.Card == null) ? "N/A" : Turn.Card.ID);
    }

    private static void OnCardAction(string cardId, string actionId, string adventureId, string scenarioId, string encounteredCardId)
    {
        eventData["card_id"] = cardId;
        eventData["action_id"] = actionId;
        eventData["adventure_id"] = adventureId;
        eventData["scenario_id"] = scenarioId;
        eventData["encountered_id"] = encounteredCardId;
        SendAnalyticsEvent("card_action", eventData, EventType.Player);
    }

    public static void OnCardDefeated(Card card)
    {
        if (card.IsBoon())
        {
            OnBoonEncountered(card.ID, true);
        }
        else if (card.IsBane())
        {
            OnBaneEncountered(card.ID, true);
        }
    }

    public static void OnCardUndefeated(Card card)
    {
        if (card.IsBoon())
        {
            OnBoonEncountered(card.ID, false);
        }
        else if (card.IsBane())
        {
            OnBaneEncountered(card.ID, false);
        }
    }

    public static void OnCharacterDeath(string characterId, string scenarioId, int difficulty)
    {
        eventData["scenario_id"] = scenarioId;
        eventData["difficulty"] = "diff_" + difficulty;
        eventData["character_id"] = characterId;
        SendAnalyticsEvent("character_death", eventData, EventType.Player);
    }

    public static void OnDieRoll(string rollType, Dictionary<string, int> rollData, List<string> rolls, int mods, int bonus, int total)
    {
        eventData["rollType"] = rollType;
        eventData["d2"] = !rollData.ContainsKey("d2") ? 0 : rollData["d2"];
        eventData["d3"] = !rollData.ContainsKey("d3") ? 0 : rollData["d3"];
        eventData["d4"] = !rollData.ContainsKey("d4") ? 0 : rollData["d4"];
        eventData["d6"] = !rollData.ContainsKey("d6") ? 0 : rollData["d6"];
        eventData["d8"] = !rollData.ContainsKey("d8") ? 0 : rollData["d8"];
        eventData["d10"] = !rollData.ContainsKey("d10") ? 0 : rollData["d10"];
        eventData["d12"] = !rollData.ContainsKey("d12") ? 0 : rollData["d12"];
        eventData["min"] = !rollData.ContainsKey("min") ? 0 : rollData["min"];
        eventData["max"] = !rollData.ContainsKey("max") ? 0 : rollData["max"];
        eventData["mods"] = mods;
        eventData["bonus"] = bonus;
        eventData["total"] = total;
        eventData["rolls"] = string.Join(",", rolls.ToArray());
        SendAnalyticsEvent("die_rolled", eventData, EventType.Player);
    }

    public static void OnEndGame(float timeSpent)
    {
        eventData["time_spent"] = timeSpent;
        SendAnalyticsEvent("game_end", eventData, EventType.Player);
    }

    public static void OnLogin()
    {
        SendAnalyticsEvent("player_login");
    }

    public static void OnLogout()
    {
        SendAnalyticsEvent("player_logout");
    }

    public static void OnPurchasedItem(string itemId, int amount)
    {
        eventData["item_id"] = itemId;
        eventData["item_cost"] = amount;
        SendAnalyticsEvent("purchased_item", eventData, EventType.Player);
    }

    public static void OnScenarioComplete(float timeSpent)
    {
        eventData["adventure_id"] = Adventure.Current.ID;
        eventData["scenario_id"] = Scenario.Current.ID;
        eventData["difficulty"] = "dif_" + Scenario.Current.Difficulty;
        eventData["gold_earned"] = Scenario.Current.GPX;
        eventData["xp_earned"] = TotalPartyXPX();
        eventData["is_victorious"] = Scenario.Current.IsScenarioWon();
        eventData["time_spent"] = timeSpent;
        eventData["party"] = Party.GetMemberList();
        SendAnalyticsEvent("scenario_complete", eventData, EventType.Player);
    }

    public static void OnScenarioStarted()
    {
        eventData["adventure_id"] = Adventure.Current.ID;
        eventData["scenario_id"] = Scenario.Current.ID;
        eventData["difficulty"] = "diff_" + Scenario.Current.Difficulty;
        eventData["party"] = Party.GetMemberList();
        SendAnalyticsEvent("scenario_start", eventData, EventType.Player);
    }

    public static void OnStartGame()
    {
        SendAnalyticsEvent("game_start");
    }

    public static void OnTutorialCompleted(float timeSpent)
    {
        eventData["time_spent"] = timeSpent;
        SendAnalyticsEvent("tutorial_complete", eventData, EventType.Player);
    }

    public static void OnTutorialStarted()
    {
        SendAnalyticsEvent("tutorial_start");
    }

    public static void OnVillainEscape(string cardId)
    {
        eventData["card_id"] = cardId;
        SendAnalyticsEvent("villain_escaped", eventData, EventType.Player);
    }

    private static void SendAnalyticsEvent(string eventName)
    {
        SendAnalyticsEvent(eventName, eventData, EventType.Default);
    }

    private static void SendAnalyticsEvent(string eventName, EventType et)
    {
        SendAnalyticsEvent(eventName, eventData, et);
    }

    private static void SendAnalyticsEvent(string eventName, Dictionary<string, object> eventData, EventType et)
    {
        UnityEngine.Analytics.Analytics.CustomEvent(eventName, eventData);
        if (et == EventType.Title)
        {
            SendTitleAnalyticsEvent(eventName, eventData);
        }
        else if (et == EventType.Player)
        {
            SendPlayerAnalyticsEvent(eventName, eventData);
        }
        else if (et == EventType.Character)
        {
            SendCharacterAnalyticsEvent(eventName, eventData);
        }
        eventData.Clear();
    }

    private static void SendCharacterAnalyticsEvent(string eventName, Dictionary<string, object> eventData)
    {
    }

    private static void SendPlayerAnalyticsEvent(string eventName, Dictionary<string, object> eventData)
    {
    }

    private static void SendTitleAnalyticsEvent(string eventName, Dictionary<string, object> eventData)
    {
    }

    private static int TotalPartyXPX()
    {
        int num = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            num += Party.Characters[i].XPX;
        }
        return num;
    }

    private enum EventType
    {
        Default,
        Character,
        Player,
        Title
    }
}

