namespace GooglePlayGames.Native
{
    using GooglePlayGames.BasicApi;
    using GooglePlayGames.BasicApi.Events;
    using GooglePlayGames.Native.PInvoke;
    using GooglePlayGames.OurUtils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal class NativeEventClient : IEventsClient
    {
        private readonly EventManager mEventManager;

        internal NativeEventClient(EventManager manager)
        {
            this.mEventManager = Misc.CheckNotNull<EventManager>(manager);
        }

        public void FetchAllEvents(DataSource source, Action<ResponseStatus, List<IEvent>> callback)
        {
            <FetchAllEvents>c__AnonStoreyCB ycb = new <FetchAllEvents>c__AnonStoreyCB {
                callback = callback
            };
            Misc.CheckNotNull<Action<ResponseStatus, List<IEvent>>>(ycb.callback);
            ycb.callback = CallbackUtils.ToOnGameThread<ResponseStatus, List<IEvent>>(ycb.callback);
            this.mEventManager.FetchAll(ConversionUtils.AsDataSource(source), new Action<EventManager.FetchAllResponse>(ycb.<>m__30));
        }

        public void FetchEvent(DataSource source, string eventId, Action<ResponseStatus, IEvent> callback)
        {
            <FetchEvent>c__AnonStoreyCC ycc = new <FetchEvent>c__AnonStoreyCC {
                callback = callback
            };
            Misc.CheckNotNull<string>(eventId);
            Misc.CheckNotNull<Action<ResponseStatus, IEvent>>(ycc.callback);
            this.mEventManager.Fetch(ConversionUtils.AsDataSource(source), eventId, new Action<EventManager.FetchResponse>(ycc.<>m__31));
        }

        public void IncrementEvent(string eventId, uint stepsToIncrement)
        {
            Misc.CheckNotNull<string>(eventId);
            this.mEventManager.Increment(eventId, stepsToIncrement);
        }

        [CompilerGenerated]
        private sealed class <FetchAllEvents>c__AnonStoreyCB
        {
            internal Action<ResponseStatus, List<IEvent>> callback;

            internal void <>m__30(EventManager.FetchAllResponse response)
            {
                ResponseStatus status = ConversionUtils.ConvertResponseStatus(response.ResponseStatus());
                if (!response.RequestSucceeded())
                {
                    this.callback(status, new List<IEvent>());
                }
                else
                {
                    this.callback(status, response.Data().Cast<IEvent>().ToList<IEvent>());
                }
            }
        }

        [CompilerGenerated]
        private sealed class <FetchEvent>c__AnonStoreyCC
        {
            internal Action<ResponseStatus, IEvent> callback;

            internal void <>m__31(EventManager.FetchResponse response)
            {
                ResponseStatus status = ConversionUtils.ConvertResponseStatus(response.ResponseStatus());
                if (!response.RequestSucceeded())
                {
                    this.callback(status, null);
                }
                else
                {
                    this.callback(status, response.Data());
                }
            }
        }
    }
}

