using OEIFormats.FlowCharts;
using System;
using UnityEngine;

public static class Scripts
{
    [ScriptParam0("Actor ID", "ID of the actor", "ID"), Script("Actor Mood - Concerned", @"Scripts\Conversation")]
    public static bool ActorMoodConcerned(string actorID)
    {
        CutsceneActor actor = GetActor(actorID);
        if (actor != null)
        {
            actor.Mood = ActorMoodType.Concerned;
            return true;
        }
        return false;
    }

    [ScriptParam0("Actor ID", "ID of the actor", "ID"), Script("Actor Mood - Neutral", @"Scripts\Conversation")]
    public static bool ActorMoodNeutral(string actorID)
    {
        CutsceneActor actor = GetActor(actorID);
        if (actor != null)
        {
            actor.Mood = ActorMoodType.Neutral;
            return true;
        }
        return false;
    }

    private static CutsceneActor GetActor(string tag)
    {
        GameObject obj2 = GameObject.Find("Actors/" + tag);
        if (obj2 != null)
        {
            return obj2.GetComponent<CutsceneActor>();
        }
        return null;
    }

    public static bool Invoke(CutsceneActor actor, ScriptCall call)
    {
        bool flag = false;
        if (call.Data.FullName.Contains("ActorMoodNeutral") && (call.Data.Parameters.Count >= 1))
        {
            flag = ActorMoodNeutral(call.Data.Parameters[0]);
        }
        if (call.Data.FullName.Contains("ActorMoodConcerned") && (call.Data.Parameters.Count >= 1))
        {
            flag = ActorMoodConcerned(call.Data.Parameters[0]);
        }
        return flag;
    }

    [Serializable]
    public enum BrowserType
    {
        None,
        GlobalVariable,
        Conversation,
        Quest,
        ObjectGuid
    }
}

