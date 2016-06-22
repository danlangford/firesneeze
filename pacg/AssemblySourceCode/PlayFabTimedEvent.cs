using Facebook.MiniJSON;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayFabTimedEvent
{
    public PlayFabTimedEvent(string str)
    {
        object obj2;
        Dictionary<string, object> dictionary = Json.Deserialize(str) as Dictionary<string, object>;
        if (dictionary.TryGetValue("UniqueID", out obj2))
        {
            this.UniqueID = (string) obj2;
        }
        if (dictionary.TryGetValue("EventName", out obj2))
        {
            this.EventName = (string) obj2;
        }
        if (dictionary.TryGetValue("EventNameID", out obj2))
        {
            this.EventNameID = (long) obj2;
        }
        if (dictionary.TryGetValue("EventDescription", out obj2))
        {
            this.EventDescription = (string) obj2;
        }
        if (dictionary.TryGetValue("EventDescriptionID", out obj2))
        {
            this.EventDescriptionID = (long) obj2;
        }
        if (dictionary.TryGetValue("RewardType", out obj2))
        {
            this.RewardType = (string) obj2;
        }
        if (dictionary.TryGetValue("RewardValue", out obj2))
        {
            if (obj2 is string)
            {
                this.RewardValueString = (string) obj2;
            }
            else if (obj2 is long)
            {
                this.RewardValueInt = (long) obj2;
            }
        }
        if (dictionary.TryGetValue("Start", out obj2))
        {
            this.Start = this.ConvertStringToDate((string) obj2);
        }
        if (dictionary.TryGetValue("End", out obj2))
        {
            this.End = this.ConvertStringToDate((string) obj2);
        }
        if (dictionary.TryGetValue("PreviewStart", out obj2))
        {
            this.PreviewStart = this.ConvertStringToDate((string) obj2);
        }
    }

    private DateTime ConvertStringToDate(string incomingDate)
    {
        if (incomingDate == string.Empty)
        {
            return DateTime.MinValue;
        }
        try
        {
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            return DateTime.ParseExact(incomingDate, "MM/dd/yyyy H:mm", invariantCulture);
        }
        catch (FormatException exception)
        {
            Debug.Log("FormatException for " + incomingDate + ": " + exception.Message);
            return DateTime.MinValue;
        }
    }

    public string ToJson()
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object> {
            { 
                "UniqueID",
                this.UniqueID
            },
            { 
                "EventName",
                this.EventName
            },
            { 
                "EventNameID",
                this.EventNameID
            },
            { 
                "EventDescription",
                this.EventDescription
            },
            { 
                "EventDescriptionID",
                this.EventDescriptionID
            },
            { 
                "RewardType",
                this.RewardType
            },
            { 
                "RewardValue",
                this.RewardValueString
            },
            { 
                "Start",
                this.Start.ToShortDateString()
            },
            { 
                "End",
                this.End.ToShortDateString()
            },
            { 
                "PreviewStart",
                this.PreviewStart.ToShortDateString()
            }
        };
        return Json.Serialize(dictionary);
    }

    public bool CanPreview =>
        (this.PreviewStart != DateTime.MinValue);

    public DateTime End { get; set; }

    public string EventDescription { get; set; }

    public long EventDescriptionID { get; set; }

    public string EventName { get; set; }

    public long EventNameID { get; set; }

    public bool Future =>
        (this.Start.CompareTo(DateTime.Now.ToLocalTime()) > 0);

    public bool Past =>
        (this.End.CompareTo(DateTime.Now.ToLocalTime()) < 0);

    public bool Present =>
        ((this.Start.CompareTo(DateTime.Now.ToLocalTime()) < 0) && (this.End.CompareTo(DateTime.Now.ToLocalTime()) > 0));

    public DateTime PreviewStart { get; set; }

    public string RewardType { get; set; }

    public long RewardValueInt { get; set; }

    public string RewardValueString { get; set; }

    public bool ShouldPreview =>
        ((this.PreviewStart.CompareTo(DateTime.Now.ToLocalTime()) < 0) && (this.Start.CompareTo(DateTime.Now.ToLocalTime()) > 0));

    public DateTime Start { get; set; }

    public string UniqueID { get; set; }

    public bool ValidDates =>
        ((this.Start != DateTime.MinValue) && (this.End != DateTime.MinValue));
}

