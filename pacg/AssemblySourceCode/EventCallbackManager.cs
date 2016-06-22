using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EventCallbackManager
{
    private static readonly string ID = "_EventManager";
    private bool isEventPaused;
    private bool isEventRunning;
    private List<EventCallback> queue = new List<EventCallback>(0x19);

    public void Add(EventCallback callback)
    {
        this.queue.Add(callback);
        if (!this.isEventPaused && !this.isEventRunning)
        {
            this.Next();
        }
    }

    public void Add(EventCallbackType callbackType, string ownerID, EventType eventType, int position, bool stateless)
    {
        EventCallback callback = new EventCallback {
            CallbackType = callbackType,
            CallbackID = ownerID,
            CallbackEvent = eventType,
            CallbackPosition = position,
            Stateless = stateless,
            CallbackCardId = null
        };
        this.Add(callback);
    }

    public void Add(EventCallbackType callbackType, string ownerID, EventType eventType, int position, bool stateless, Card parm)
    {
        EventCallback callback = new EventCallback {
            CallbackType = callbackType,
            CallbackID = ownerID,
            CallbackEvent = eventType,
            CallbackPosition = position,
            Stateless = stateless
        };
        if (parm != null)
        {
            callback.CallbackCardId = parm.ID;
        }
        this.Add(callback);
    }

    public void Clear()
    {
        this.queue.Clear();
        this.isEventRunning = false;
        this.isEventPaused = false;
        this.Post = 0;
    }

    public bool Complete()
    {
        bool flag = true;
        if (!this.isEventPaused && (this.Post > 0))
        {
            this.Post--;
            flag = !this.ContainsStatefulEvent();
            this.Next();
        }
        return flag;
    }

    public bool ContainsStatefulEvent()
    {
        for (int i = 0; i < this.queue.Count; i++)
        {
            if (!this.queue[i].Stateless)
            {
                return true;
            }
        }
        return false;
    }

    public void Dump()
    {
        for (int i = 0; i < this.queue.Count; i++)
        {
            Debug.Log(i + ": " + this.queue[i].ToString());
        }
    }

    private void GlowTextHide()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.locationPanel.GlowText(TextHilightType.None);
            window.layoutLocation.GlowText(false);
        }
    }

    private void GlowTextShow(EventCallbackType callbackType)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (callbackType == EventCallbackType.Location)
            {
                if (!Location.Current.Closed)
                {
                    window.locationPanel.GlowText(TextHilightType.AtThisLocation);
                }
                else
                {
                    bool flag = false;
                    for (int i = 0; i < Location.Current.Powers.Count; i++)
                    {
                        if ((Location.Current.Powers[i].Situation == LocationPowerType.AtThisLocation) && Location.Current.Powers[i].UsefulWhenClosed)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        window.locationPanel.GlowText(TextHilightType.AtThisLocation);
                    }
                    else
                    {
                        window.locationPanel.GlowText(TextHilightType.WhenPermanentlyClosed);
                    }
                }
            }
            if (callbackType == EventCallbackType.Scenario)
            {
                window.locationPanel.GlowText(TextHilightType.DuringThisScenario);
            }
            if (callbackType == EventCallbackType.Card)
            {
                window.layoutLocation.GlowText(true);
            }
        }
    }

    public void Next()
    {
        if (!this.isEventPaused)
        {
            this.GlowTextHide();
            this.isEventRunning = false;
            if (this.queue.Count > 0)
            {
                this.isEventRunning = true;
                EventCallback callback = this.queue[0];
                this.queue.RemoveAt(0);
                this.GlowTextShow(callback.CallbackType);
                callback.Invoke();
            }
        }
    }

    public void OnLoadData()
    {
        byte[] buffer;
        this.queue.Clear();
        if (Game.GetObjectData(ID, out buffer))
        {
            ByteStream bs = new ByteStream(buffer);
            if (bs != null)
            {
                bs.ReadInt();
                this.isEventRunning = bs.ReadBool();
                this.Post = bs.ReadInt();
                int num = bs.ReadInt();
                for (int i = 0; i < num; i++)
                {
                    EventCallback item = EventCallback.FromStream(bs);
                    this.queue.Add(item);
                }
            }
        }
    }

    public void OnSaveData()
    {
        ByteStream bs = new ByteStream();
        if (bs != null)
        {
            bs.WriteInt(1);
            bs.WriteBool(this.isEventRunning);
            bs.WriteInt(this.Post);
            bs.WriteInt(this.queue.Count);
            for (int i = 0; i < this.queue.Count; i++)
            {
                this.queue[i].ToStream(bs);
            }
            Game.SetObjectData(ID, bs.ToArray());
        }
    }

    public void Pause(bool isPaused)
    {
        this.isEventPaused = isPaused;
    }

    public void Top(EventCallback callback)
    {
        this.queue.Insert(0, callback);
        if (!this.isEventPaused && !this.isEventRunning)
        {
            this.Next();
        }
    }

    public int Count =>
        this.queue.Count;

    public bool IsEventRunning =>
        this.isEventRunning;

    public int Post { get; set; }
}

