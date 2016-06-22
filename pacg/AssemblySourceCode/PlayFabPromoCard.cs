using Facebook.MiniJSON;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayFabPromoCard
{
    public PlayFabPromoCard(string str)
    {
        object obj2;
        Dictionary<string, object> dictionary = Json.Deserialize(str) as Dictionary<string, object>;
        if (dictionary.TryGetValue("CardID", out obj2))
        {
            this.CardID = (string) obj2;
        }
        if (dictionary.TryGetValue("EventType", out obj2))
        {
            this.EventType = (string) obj2;
        }
        if (dictionary.TryGetValue("EventID", out obj2))
        {
            this.EventID = (string) obj2;
        }
        if (dictionary.TryGetValue("Start", out obj2))
        {
            this.Start = this.ConvertStringToDate((string) obj2);
        }
        if (dictionary.TryGetValue("End", out obj2))
        {
            this.End = this.ConvertStringToDate((string) obj2);
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
            return DateTime.ParseExact(incomingDate, "MM/dd/yyyy hh:mm", invariantCulture);
        }
        catch (FormatException exception)
        {
            Debug.Log("FormatException: " + exception.Message);
            return DateTime.MinValue;
        }
    }

    public string ToJson()
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object> {
            { 
                "CardID",
                this.CardID
            },
            { 
                "EventType",
                this.EventType
            },
            { 
                "EventID",
                this.EventID
            },
            { 
                "Start",
                this.Start.ToShortDateString()
            },
            { 
                "End",
                this.End.ToShortDateString()
            }
        };
        return Json.Serialize(dictionary);
    }

    public string CardID { get; set; }

    public DateTime End { get; set; }

    public string EventID { get; set; }

    public string EventType { get; set; }

    public bool Future =>
        (this.Start.CompareTo(DateTime.Now.ToLocalTime()) > 0);

    public bool Past =>
        (this.End.CompareTo(DateTime.Now.ToLocalTime()) < 0);

    public bool Present =>
        ((this.Start.CompareTo(DateTime.Now.ToLocalTime()) < 0) && (this.End.CompareTo(DateTime.Now.ToLocalTime()) > 0));

    public DateTime Start { get; set; }

    public bool ValidDates =>
        ((this.Start != DateTime.MinValue) && (this.End != DateTime.MinValue));
}

