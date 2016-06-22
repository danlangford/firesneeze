namespace GooglePlayGames.Native
{
    using GooglePlayGames.BasicApi.Nearby;
    using GooglePlayGames.Native.PInvoke;
    using GooglePlayGames.OurUtils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal class NativeNearbyConnectionsClient : INearbyConnectionClient
    {
        private readonly NearbyConnectionsManager mManager;

        internal NativeNearbyConnectionsClient(NearbyConnectionsManager manager)
        {
            this.mManager = Misc.CheckNotNull<NearbyConnectionsManager>(manager);
        }

        public void AcceptConnectionRequest(string remoteEndpointId, byte[] payload, IMessageListener listener)
        {
            Misc.CheckNotNull<string>(remoteEndpointId, "remoteEndpointId");
            Misc.CheckNotNull<byte[]>(payload, "payload");
            Misc.CheckNotNull<IMessageListener>(listener, "listener");
            Logger.d("Calling AcceptConncectionRequest");
            this.mManager.AcceptConnectionRequest(remoteEndpointId, payload, ToMessageListener(listener));
            Logger.d("Called!");
        }

        public void DisconnectFromEndpoint(string remoteEndpointId)
        {
            this.mManager.DisconnectFromEndpoint(remoteEndpointId);
        }

        public string GetAppBundleId() => 
            this.mManager.AppBundleId;

        public string GetServiceId() => 
            NearbyConnectionsManager.ServiceId;

        private void InternalSend(List<string> recipientEndpointIds, byte[] payload, bool isReliable)
        {
            if (recipientEndpointIds == null)
            {
                throw new ArgumentNullException("recipientEndpointIds");
            }
            if (payload == null)
            {
                throw new ArgumentNullException("payload");
            }
            if (recipientEndpointIds.Contains(null))
            {
                throw new InvalidOperationException("Cannot send a message to a null recipient");
            }
            if (recipientEndpointIds.Count == 0)
            {
                Logger.w("Attempted to send a reliable message with no recipients");
            }
            else
            {
                if (isReliable)
                {
                    if (payload.Length > this.MaxReliableMessagePayloadLength())
                    {
                        throw new InvalidOperationException("cannot send more than " + this.MaxReliableMessagePayloadLength() + " bytes");
                    }
                }
                else if (payload.Length > this.MaxUnreliableMessagePayloadLength())
                {
                    throw new InvalidOperationException("cannot send more than " + this.MaxUnreliableMessagePayloadLength() + " bytes");
                }
                foreach (string str in recipientEndpointIds)
                {
                    if (isReliable)
                    {
                        this.mManager.SendReliable(str, payload);
                    }
                    else
                    {
                        this.mManager.SendUnreliable(str, payload);
                    }
                }
            }
        }

        public string LocalDeviceId() => 
            this.mManager.LocalDeviceId();

        public string LocalEndpointId() => 
            this.mManager.LocalEndpointId();

        public int MaxReliableMessagePayloadLength() => 
            0x1000;

        public int MaxUnreliableMessagePayloadLength() => 
            0x490;

        public void RejectConnectionRequest(string requestingEndpointId)
        {
            Misc.CheckNotNull<string>(requestingEndpointId, "requestingEndpointId");
            this.mManager.RejectConnectionRequest(requestingEndpointId);
        }

        public void SendConnectionRequest(string name, string remoteEndpointId, byte[] payload, Action<ConnectionResponse> responseCallback, IMessageListener listener)
        {
            <SendConnectionRequest>c__AnonStoreyCE yce = new <SendConnectionRequest>c__AnonStoreyCE {
                responseCallback = responseCallback
            };
            Misc.CheckNotNull<string>(remoteEndpointId, "remoteEndpointId");
            Misc.CheckNotNull<byte[]>(payload, "payload");
            Misc.CheckNotNull<Action<ConnectionResponse>>(yce.responseCallback, "responseCallback");
            Misc.CheckNotNull<IMessageListener>(listener, "listener");
            yce.responseCallback = Callbacks.AsOnGameThreadCallback<ConnectionResponse>(yce.responseCallback);
            using (NativeMessageListenerHelper helper = ToMessageListener(listener))
            {
                this.mManager.SendConnectionRequest(name, remoteEndpointId, payload, new Action<long, NativeConnectionResponse>(yce.<>m__34), helper);
            }
        }

        public void SendReliable(List<string> recipientEndpointIds, byte[] payload)
        {
            this.InternalSend(recipientEndpointIds, payload, true);
        }

        public void SendUnreliable(List<string> recipientEndpointIds, byte[] payload)
        {
            this.InternalSend(recipientEndpointIds, payload, false);
        }

        public void StartAdvertising(string name, List<string> appIdentifiers, TimeSpan? advertisingDuration, Action<AdvertisingResult> resultCallback, Action<ConnectionRequest> requestCallback)
        {
            <StartAdvertising>c__AnonStoreyCD ycd = new <StartAdvertising>c__AnonStoreyCD {
                resultCallback = resultCallback,
                requestCallback = requestCallback
            };
            Misc.CheckNotNull<List<string>>(appIdentifiers, "appIdentifiers");
            Misc.CheckNotNull<Action<AdvertisingResult>>(ycd.resultCallback, "resultCallback");
            Misc.CheckNotNull<Action<ConnectionRequest>>(ycd.requestCallback, "connectionRequestCallback");
            if (advertisingDuration.HasValue && (advertisingDuration.Value.Ticks < 0L))
            {
                throw new InvalidOperationException("advertisingDuration must be positive");
            }
            ycd.resultCallback = Callbacks.AsOnGameThreadCallback<AdvertisingResult>(ycd.resultCallback);
            ycd.requestCallback = Callbacks.AsOnGameThreadCallback<ConnectionRequest>(ycd.requestCallback);
            this.mManager.StartAdvertising(name, appIdentifiers.Select<string, NativeAppIdentifier>(new Func<string, NativeAppIdentifier>(NativeAppIdentifier.FromString)).ToList<NativeAppIdentifier>(), ToTimeoutMillis(advertisingDuration), new Action<long, NativeStartAdvertisingResult>(ycd.<>m__32), new Action<long, NativeConnectionRequest>(ycd.<>m__33));
        }

        public void StartDiscovery(string serviceId, TimeSpan? advertisingTimeout, IDiscoveryListener listener)
        {
            Misc.CheckNotNull<string>(serviceId, "serviceId");
            Misc.CheckNotNull<IDiscoveryListener>(listener, "listener");
            using (NativeEndpointDiscoveryListenerHelper helper = ToDiscoveryListener(listener))
            {
                this.mManager.StartDiscovery(serviceId, ToTimeoutMillis(advertisingTimeout), helper);
            }
        }

        public void StopAdvertising()
        {
            this.mManager.StopAdvertising();
        }

        public void StopAllConnections()
        {
            this.mManager.StopAllConnections();
        }

        public void StopDiscovery(string serviceId)
        {
            Misc.CheckNotNull<string>(serviceId, "serviceId");
            this.mManager.StopDiscovery(serviceId);
        }

        private static NativeEndpointDiscoveryListenerHelper ToDiscoveryListener(IDiscoveryListener listener)
        {
            <ToDiscoveryListener>c__AnonStoreyD0 yd = new <ToDiscoveryListener>c__AnonStoreyD0 {
                listener = listener
            };
            yd.listener = new OnGameThreadDiscoveryListener(yd.listener);
            NativeEndpointDiscoveryListenerHelper helper = new NativeEndpointDiscoveryListenerHelper();
            helper.SetOnEndpointFound(new Action<long, NativeEndpointDetails>(yd.<>m__37));
            helper.SetOnEndpointLostCallback(new Action<long, string>(yd.<>m__38));
            return helper;
        }

        private static NativeMessageListenerHelper ToMessageListener(IMessageListener listener)
        {
            <ToMessageListener>c__AnonStoreyCF ycf = new <ToMessageListener>c__AnonStoreyCF {
                listener = listener
            };
            ycf.listener = new OnGameThreadMessageListener(ycf.listener);
            NativeMessageListenerHelper helper = new NativeMessageListenerHelper();
            helper.SetOnMessageReceivedCallback(new NativeMessageListenerHelper.OnMessageReceived(ycf.<>m__35));
            helper.SetOnDisconnectedCallback(new Action<long, string>(ycf.<>m__36));
            return helper;
        }

        private static long ToTimeoutMillis(TimeSpan? span) => 
            (!span.HasValue ? 0L : PInvokeUtilities.ToMilliseconds(span.Value));

        [CompilerGenerated]
        private sealed class <SendConnectionRequest>c__AnonStoreyCE
        {
            internal Action<ConnectionResponse> responseCallback;

            internal void <>m__34(long localClientId, NativeConnectionResponse response)
            {
                this.responseCallback(response.AsResponse(localClientId));
            }
        }

        [CompilerGenerated]
        private sealed class <StartAdvertising>c__AnonStoreyCD
        {
            internal Action<ConnectionRequest> requestCallback;
            internal Action<AdvertisingResult> resultCallback;

            internal void <>m__32(long localClientId, NativeStartAdvertisingResult result)
            {
                this.resultCallback(result.AsResult());
            }

            internal void <>m__33(long localClientId, NativeConnectionRequest request)
            {
                this.requestCallback(request.AsRequest());
            }
        }

        [CompilerGenerated]
        private sealed class <ToDiscoveryListener>c__AnonStoreyD0
        {
            internal IDiscoveryListener listener;

            internal void <>m__37(long localClientId, NativeEndpointDetails endpoint)
            {
                this.listener.OnEndpointFound(endpoint.ToDetails());
            }

            internal void <>m__38(long localClientId, string lostEndpointId)
            {
                this.listener.OnEndpointLost(lostEndpointId);
            }
        }

        [CompilerGenerated]
        private sealed class <ToMessageListener>c__AnonStoreyCF
        {
            internal IMessageListener listener;

            internal void <>m__35(long localClientId, string endpointId, byte[] data, bool isReliable)
            {
                this.listener.OnMessageReceived(endpointId, data, isReliable);
            }

            internal void <>m__36(long localClientId, string endpointId)
            {
                this.listener.OnRemoteEndpointDisconnected(endpointId);
            }
        }

        protected class OnGameThreadDiscoveryListener : IDiscoveryListener
        {
            private readonly IDiscoveryListener mListener;

            public OnGameThreadDiscoveryListener(IDiscoveryListener listener)
            {
                this.mListener = Misc.CheckNotNull<IDiscoveryListener>(listener);
            }

            public void OnEndpointFound(EndpointDetails discoveredEndpoint)
            {
                <OnEndpointFound>c__AnonStoreyD3 yd = new <OnEndpointFound>c__AnonStoreyD3 {
                    discoveredEndpoint = discoveredEndpoint,
                    <>f__this = this
                };
                PlayGamesHelperObject.RunOnGameThread(new Action(yd.<>m__3B));
            }

            public void OnEndpointLost(string lostEndpointId)
            {
                <OnEndpointLost>c__AnonStoreyD4 yd = new <OnEndpointLost>c__AnonStoreyD4 {
                    lostEndpointId = lostEndpointId,
                    <>f__this = this
                };
                PlayGamesHelperObject.RunOnGameThread(new Action(yd.<>m__3C));
            }

            [CompilerGenerated]
            private sealed class <OnEndpointFound>c__AnonStoreyD3
            {
                internal NativeNearbyConnectionsClient.OnGameThreadDiscoveryListener <>f__this;
                internal EndpointDetails discoveredEndpoint;

                internal void <>m__3B()
                {
                    this.<>f__this.mListener.OnEndpointFound(this.discoveredEndpoint);
                }
            }

            [CompilerGenerated]
            private sealed class <OnEndpointLost>c__AnonStoreyD4
            {
                internal NativeNearbyConnectionsClient.OnGameThreadDiscoveryListener <>f__this;
                internal string lostEndpointId;

                internal void <>m__3C()
                {
                    this.<>f__this.mListener.OnEndpointLost(this.lostEndpointId);
                }
            }
        }

        protected class OnGameThreadMessageListener : IMessageListener
        {
            private readonly IMessageListener mListener;

            public OnGameThreadMessageListener(IMessageListener listener)
            {
                this.mListener = Misc.CheckNotNull<IMessageListener>(listener);
            }

            public void OnMessageReceived(string remoteEndpointId, byte[] data, bool isReliableMessage)
            {
                <OnMessageReceived>c__AnonStoreyD1 yd = new <OnMessageReceived>c__AnonStoreyD1 {
                    remoteEndpointId = remoteEndpointId,
                    data = data,
                    isReliableMessage = isReliableMessage,
                    <>f__this = this
                };
                PlayGamesHelperObject.RunOnGameThread(new Action(yd.<>m__39));
            }

            public void OnRemoteEndpointDisconnected(string remoteEndpointId)
            {
                <OnRemoteEndpointDisconnected>c__AnonStoreyD2 yd = new <OnRemoteEndpointDisconnected>c__AnonStoreyD2 {
                    remoteEndpointId = remoteEndpointId,
                    <>f__this = this
                };
                PlayGamesHelperObject.RunOnGameThread(new Action(yd.<>m__3A));
            }

            [CompilerGenerated]
            private sealed class <OnMessageReceived>c__AnonStoreyD1
            {
                internal NativeNearbyConnectionsClient.OnGameThreadMessageListener <>f__this;
                internal byte[] data;
                internal bool isReliableMessage;
                internal string remoteEndpointId;

                internal void <>m__39()
                {
                    this.<>f__this.mListener.OnMessageReceived(this.remoteEndpointId, this.data, this.isReliableMessage);
                }
            }

            [CompilerGenerated]
            private sealed class <OnRemoteEndpointDisconnected>c__AnonStoreyD2
            {
                internal NativeNearbyConnectionsClient.OnGameThreadMessageListener <>f__this;
                internal string remoteEndpointId;

                internal void <>m__3A()
                {
                    this.<>f__this.mListener.OnRemoteEndpointDisconnected(this.remoteEndpointId);
                }
            }
        }
    }
}

