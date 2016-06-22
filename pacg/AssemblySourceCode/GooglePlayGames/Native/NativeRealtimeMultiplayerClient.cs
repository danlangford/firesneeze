namespace GooglePlayGames.Native
{
    using GooglePlayGames.BasicApi.Multiplayer;
    using GooglePlayGames.Native.Cwrapper;
    using GooglePlayGames.Native.PInvoke;
    using GooglePlayGames.OurUtils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class NativeRealtimeMultiplayerClient : IRealTimeMultiplayerClient
    {
        private volatile RoomSession mCurrentSession;
        private readonly NativeClient mNativeClient;
        private readonly RealtimeManager mRealtimeManager;
        private readonly object mSessionLock = new object();

        internal NativeRealtimeMultiplayerClient(NativeClient nativeClient, RealtimeManager manager)
        {
            this.mNativeClient = Misc.CheckNotNull<NativeClient>(nativeClient);
            this.mRealtimeManager = Misc.CheckNotNull<RealtimeManager>(manager);
            this.mCurrentSession = this.GetTerminatedSession();
            PlayGamesHelperObject.AddPauseCallback(new Action<bool>(this.HandleAppPausing));
        }

        public void AcceptFromInbox(RealTimeMultiplayerListener listener)
        {
            object mSessionLock = this.mSessionLock;
            lock (mSessionLock)
            {
                <AcceptFromInbox>c__AnonStoreyE3 ye = new <AcceptFromInbox>c__AnonStoreyE3 {
                    <>f__this = this,
                    newRoom = new RoomSession(this.mRealtimeManager, listener)
                };
                if (this.mCurrentSession.IsActive())
                {
                    Logger.e("Received attempt to accept invitation without cleaning up active session.");
                    ye.newRoom.LeaveRoom();
                }
                else
                {
                    this.mCurrentSession = ye.newRoom;
                    this.mCurrentSession.ShowingUI = true;
                    this.mRealtimeManager.ShowRoomInboxUI(new Action<RealtimeManager.RoomInboxUIResponse>(ye.<>m__49));
                }
            }
        }

        public void AcceptInvitation(string invitationId, RealTimeMultiplayerListener listener)
        {
            <AcceptInvitation>c__AnonStoreyE7 ye = new <AcceptInvitation>c__AnonStoreyE7 {
                invitationId = invitationId,
                <>f__this = this
            };
            object mSessionLock = this.mSessionLock;
            lock (mSessionLock)
            {
                <AcceptInvitation>c__AnonStoreyE6 ye2 = new <AcceptInvitation>c__AnonStoreyE6 {
                    <>f__ref$231 = ye,
                    <>f__this = this,
                    newRoom = new RoomSession(this.mRealtimeManager, listener)
                };
                if (this.mCurrentSession.IsActive())
                {
                    Logger.e("Received attempt to accept invitation without cleaning up active session.");
                    ye2.newRoom.LeaveRoom();
                }
                else
                {
                    this.mCurrentSession = ye2.newRoom;
                    this.mRealtimeManager.FetchInvitations(new Action<RealtimeManager.FetchInvitationsResponse>(ye2.<>m__4A));
                }
            }
        }

        public void CreateQuickGame(uint minOpponents, uint maxOpponents, uint variant, RealTimeMultiplayerListener listener)
        {
            this.CreateQuickGame(minOpponents, maxOpponents, variant, 0L, listener);
        }

        public void CreateQuickGame(uint minOpponents, uint maxOpponents, uint variant, ulong exclusiveBitMask, RealTimeMultiplayerListener listener)
        {
            object mSessionLock = this.mSessionLock;
            lock (mSessionLock)
            {
                <CreateQuickGame>c__AnonStoreyDC ydc = new <CreateQuickGame>c__AnonStoreyDC {
                    <>f__this = this,
                    newSession = new RoomSession(this.mRealtimeManager, listener)
                };
                if (this.mCurrentSession.IsActive())
                {
                    Logger.e("Received attempt to create a new room without cleaning up the old one.");
                    ydc.newSession.LeaveRoom();
                }
                else
                {
                    this.mCurrentSession = ydc.newSession;
                    Logger.d("QuickGame: Setting MinPlayersToStart = " + minOpponents);
                    this.mCurrentSession.MinPlayersToStart = minOpponents;
                    using (RealtimeRoomConfigBuilder builder = RealtimeRoomConfigBuilder.Create())
                    {
                        <CreateQuickGame>c__AnonStoreyDA yda = new <CreateQuickGame>c__AnonStoreyDA {
                            <>f__this = this,
                            config = builder.SetMinimumAutomatchingPlayers(minOpponents).SetMaximumAutomatchingPlayers(maxOpponents).SetVariant(variant).SetExclusiveBitMask(exclusiveBitMask).Build()
                        };
                        using (yda.config)
                        {
                            <CreateQuickGame>c__AnonStoreyDB ydb = new <CreateQuickGame>c__AnonStoreyDB {
                                <>f__ref$220 = ydc,
                                <>f__ref$218 = yda,
                                <>f__this = this,
                                helper = HelperForSession(ydc.newSession)
                            };
                            try
                            {
                                ydc.newSession.StartRoomCreation(this.mNativeClient.GetUserId(), new Action(ydb.<>m__42));
                            }
                            finally
                            {
                                if (ydb.helper != null)
                                {
                                    ydb.helper.Dispose();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CreateWithInvitationScreen(uint minOpponents, uint maxOppponents, uint variant, RealTimeMultiplayerListener listener)
        {
            <CreateWithInvitationScreen>c__AnonStoreyDF ydf = new <CreateWithInvitationScreen>c__AnonStoreyDF {
                variant = variant,
                <>f__this = this
            };
            object mSessionLock = this.mSessionLock;
            lock (mSessionLock)
            {
                <CreateWithInvitationScreen>c__AnonStoreyDE yde = new <CreateWithInvitationScreen>c__AnonStoreyDE {
                    <>f__ref$223 = ydf,
                    <>f__this = this,
                    newRoom = new RoomSession(this.mRealtimeManager, listener)
                };
                if (this.mCurrentSession.IsActive())
                {
                    Logger.e("Received attempt to create a new room without cleaning up the old one.");
                    yde.newRoom.LeaveRoom();
                }
                else
                {
                    this.mCurrentSession = yde.newRoom;
                    this.mCurrentSession.ShowingUI = true;
                    this.mRealtimeManager.ShowPlayerSelectUI(minOpponents, maxOppponents, true, new Action<PlayerSelectUIResponse>(yde.<>m__47));
                }
            }
        }

        public void DeclineInvitation(string invitationId)
        {
            <DeclineInvitation>c__AnonStoreyEA yea = new <DeclineInvitation>c__AnonStoreyEA {
                invitationId = invitationId,
                <>f__this = this
            };
            this.mRealtimeManager.FetchInvitations(new Action<RealtimeManager.FetchInvitationsResponse>(yea.<>m__4B));
        }

        public void GetAllInvitations(Action<Invitation[]> callback)
        {
            <GetAllInvitations>c__AnonStoreyE2 ye = new <GetAllInvitations>c__AnonStoreyE2 {
                callback = callback
            };
            this.mRealtimeManager.FetchInvitations(new Action<RealtimeManager.FetchInvitationsResponse>(ye.<>m__48));
        }

        public List<Participant> GetConnectedParticipants() => 
            this.mCurrentSession.GetConnectedParticipants();

        public Invitation GetInvitation() => 
            this.mCurrentSession.GetInvitation();

        public Participant GetParticipant(string participantId) => 
            this.mCurrentSession.GetParticipant(participantId);

        public Participant GetSelf() => 
            this.mCurrentSession.GetSelf();

        private RoomSession GetTerminatedSession()
        {
            RoomSession session = new RoomSession(this.mRealtimeManager, new NoopListener());
            session.EnterState(new ShutdownState(session), false);
            return session;
        }

        private void HandleAppPausing(bool paused)
        {
            if (paused)
            {
                Logger.d("Application is pausing, which disconnects the RTMP  client.  Leaving room.");
                this.LeaveRoom();
            }
        }

        private static GooglePlayGames.Native.PInvoke.RealTimeEventListenerHelper HelperForSession(RoomSession session)
        {
            <HelperForSession>c__AnonStoreyDD ydd = new <HelperForSession>c__AnonStoreyDD {
                session = session
            };
            return GooglePlayGames.Native.PInvoke.RealTimeEventListenerHelper.Create().SetOnDataReceivedCallback(new Action<NativeRealTimeRoom, GooglePlayGames.Native.PInvoke.MultiplayerParticipant, byte[], bool>(ydd.<>m__43)).SetOnParticipantStatusChangedCallback(new Action<NativeRealTimeRoom, GooglePlayGames.Native.PInvoke.MultiplayerParticipant>(ydd.<>m__44)).SetOnRoomConnectedSetChangedCallback(new Action<NativeRealTimeRoom>(ydd.<>m__45)).SetOnRoomStatusChangedCallback(new Action<NativeRealTimeRoom>(ydd.<>m__46));
        }

        public bool IsRoomConnected() => 
            this.mCurrentSession.IsRoomConnected();

        public void LeaveRoom()
        {
            this.mCurrentSession.LeaveRoom();
        }

        public void SendMessage(bool reliable, string participantId, byte[] data)
        {
            this.mCurrentSession.SendMessage(reliable, participantId, data);
        }

        public void SendMessage(bool reliable, string participantId, byte[] data, int offset, int length)
        {
            this.mCurrentSession.SendMessage(reliable, participantId, data, offset, length);
        }

        public void SendMessageToAll(bool reliable, byte[] data)
        {
            this.mCurrentSession.SendMessageToAll(reliable, data);
        }

        public void SendMessageToAll(bool reliable, byte[] data, int offset, int length)
        {
            this.mCurrentSession.SendMessageToAll(reliable, data, offset, length);
        }

        public void ShowWaitingRoomUI()
        {
            object mSessionLock = this.mSessionLock;
            lock (mSessionLock)
            {
                this.mCurrentSession.ShowWaitingRoomUI();
            }
        }

        private static T WithDefault<T>(T presented, T defaultValue) where T: class => 
            ((presented == null) ? defaultValue : presented);

        [CompilerGenerated]
        private sealed class <AcceptFromInbox>c__AnonStoreyE3
        {
            internal NativeRealtimeMultiplayerClient <>f__this;
            internal NativeRealtimeMultiplayerClient.RoomSession newRoom;

            internal void <>m__49(RealtimeManager.RoomInboxUIResponse response)
            {
                <AcceptFromInbox>c__AnonStoreyE4 ye = new <AcceptFromInbox>c__AnonStoreyE4 {
                    <>f__ref$227 = this
                };
                this.<>f__this.mCurrentSession.ShowingUI = false;
                if (response.ResponseStatus() != GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus.VALID)
                {
                    Logger.d("User did not complete invitation screen.");
                    this.newRoom.LeaveRoom();
                }
                else
                {
                    ye.invitation = response.Invitation();
                    <AcceptFromInbox>c__AnonStoreyE5 ye2 = new <AcceptFromInbox>c__AnonStoreyE5 {
                        <>f__ref$227 = this,
                        <>f__ref$228 = ye,
                        helper = NativeRealtimeMultiplayerClient.HelperForSession(this.newRoom)
                    };
                    try
                    {
                        Logger.d("About to accept invitation " + ye.invitation.Id());
                        this.newRoom.StartRoomCreation(this.<>f__this.mNativeClient.GetUserId(), new Action(ye2.<>m__4D));
                    }
                    finally
                    {
                        if (ye2.helper != null)
                        {
                            ye2.helper.Dispose();
                        }
                    }
                }
            }

            private sealed class <AcceptFromInbox>c__AnonStoreyE4
            {
                internal NativeRealtimeMultiplayerClient.<AcceptFromInbox>c__AnonStoreyE3 <>f__ref$227;
                internal GooglePlayGames.Native.PInvoke.MultiplayerInvitation invitation;
            }

            private sealed class <AcceptFromInbox>c__AnonStoreyE5
            {
                internal NativeRealtimeMultiplayerClient.<AcceptFromInbox>c__AnonStoreyE3 <>f__ref$227;
                internal NativeRealtimeMultiplayerClient.<AcceptFromInbox>c__AnonStoreyE3.<AcceptFromInbox>c__AnonStoreyE4 <>f__ref$228;
                internal GooglePlayGames.Native.PInvoke.RealTimeEventListenerHelper helper;

                internal void <>m__4D()
                {
                    this.<>f__ref$227.<>f__this.mRealtimeManager.AcceptInvitation(this.<>f__ref$228.invitation, this.helper, delegate (RealtimeManager.RealTimeRoomResponse acceptResponse) {
                        using (this.<>f__ref$228.invitation)
                        {
                            this.<>f__ref$227.newRoom.HandleRoomResponse(acceptResponse);
                            this.<>f__ref$227.newRoom.SetInvitation(this.<>f__ref$228.invitation.AsInvitation());
                        }
                    });
                }

                internal void <>m__4E(RealtimeManager.RealTimeRoomResponse acceptResponse)
                {
                    using (this.<>f__ref$228.invitation)
                    {
                        this.<>f__ref$227.newRoom.HandleRoomResponse(acceptResponse);
                        this.<>f__ref$227.newRoom.SetInvitation(this.<>f__ref$228.invitation.AsInvitation());
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <AcceptInvitation>c__AnonStoreyE6
        {
            internal NativeRealtimeMultiplayerClient.<AcceptInvitation>c__AnonStoreyE7 <>f__ref$231;
            internal NativeRealtimeMultiplayerClient <>f__this;
            internal NativeRealtimeMultiplayerClient.RoomSession newRoom;

            internal void <>m__4A(RealtimeManager.FetchInvitationsResponse response)
            {
                if (!response.RequestSucceeded())
                {
                    Logger.e("Couldn't load invitations.");
                    this.newRoom.LeaveRoom();
                }
                else
                {
                    <AcceptInvitation>c__AnonStoreyE8 ye = new <AcceptInvitation>c__AnonStoreyE8 {
                        <>f__ref$231 = this.<>f__ref$231,
                        <>f__ref$230 = this
                    };
                    IEnumerator<GooglePlayGames.Native.PInvoke.MultiplayerInvitation> enumerator = response.Invitations().GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            ye.invitation = enumerator.Current;
                            using (ye.invitation)
                            {
                                if (ye.invitation.Id().Equals(this.<>f__ref$231.invitationId))
                                {
                                    this.<>f__this.mCurrentSession.MinPlayersToStart = ye.invitation.AutomatchingSlots() + ye.invitation.ParticipantCount();
                                    Logger.d("Setting MinPlayersToStart with invitation to : " + this.<>f__this.mCurrentSession.MinPlayersToStart);
                                    <AcceptInvitation>c__AnonStoreyE9 ye2 = new <AcceptInvitation>c__AnonStoreyE9 {
                                        <>f__ref$230 = this,
                                        <>f__ref$232 = ye,
                                        helper = NativeRealtimeMultiplayerClient.HelperForSession(this.newRoom)
                                    };
                                    try
                                    {
                                        this.newRoom.StartRoomCreation(this.<>f__this.mNativeClient.GetUserId(), new Action(ye2.<>m__4F));
                                        return;
                                    }
                                    finally
                                    {
                                        if (ye2.helper != null)
                                        {
                                            ye2.helper.Dispose();
                                        }
                                    }
                                }
                                continue;
                            }
                        }
                    }
                    finally
                    {
                        if (enumerator == null)
                        {
                        }
                        enumerator.Dispose();
                    }
                    Logger.e("Room creation failed since we could not find invitation with ID " + this.<>f__ref$231.invitationId);
                    this.newRoom.LeaveRoom();
                }
            }

            private sealed class <AcceptInvitation>c__AnonStoreyE8
            {
                internal NativeRealtimeMultiplayerClient.<AcceptInvitation>c__AnonStoreyE6 <>f__ref$230;
                internal NativeRealtimeMultiplayerClient.<AcceptInvitation>c__AnonStoreyE7 <>f__ref$231;
                internal GooglePlayGames.Native.PInvoke.MultiplayerInvitation invitation;
            }

            private sealed class <AcceptInvitation>c__AnonStoreyE9
            {
                internal NativeRealtimeMultiplayerClient.<AcceptInvitation>c__AnonStoreyE6 <>f__ref$230;
                internal NativeRealtimeMultiplayerClient.<AcceptInvitation>c__AnonStoreyE6.<AcceptInvitation>c__AnonStoreyE8 <>f__ref$232;
                internal GooglePlayGames.Native.PInvoke.RealTimeEventListenerHelper helper;

                internal void <>m__4F()
                {
                    this.<>f__ref$230.<>f__this.mRealtimeManager.AcceptInvitation(this.<>f__ref$232.invitation, this.helper, new Action<RealtimeManager.RealTimeRoomResponse>(this.<>f__ref$230.newRoom.HandleRoomResponse));
                }
            }
        }

        [CompilerGenerated]
        private sealed class <AcceptInvitation>c__AnonStoreyE7
        {
            internal NativeRealtimeMultiplayerClient <>f__this;
            internal string invitationId;
        }

        [CompilerGenerated]
        private sealed class <CreateQuickGame>c__AnonStoreyDA
        {
            internal NativeRealtimeMultiplayerClient <>f__this;
            internal RealtimeRoomConfig config;
        }

        [CompilerGenerated]
        private sealed class <CreateQuickGame>c__AnonStoreyDB
        {
            internal NativeRealtimeMultiplayerClient.<CreateQuickGame>c__AnonStoreyDA <>f__ref$218;
            internal NativeRealtimeMultiplayerClient.<CreateQuickGame>c__AnonStoreyDC <>f__ref$220;
            internal NativeRealtimeMultiplayerClient <>f__this;
            internal GooglePlayGames.Native.PInvoke.RealTimeEventListenerHelper helper;

            internal void <>m__42()
            {
                this.<>f__this.mRealtimeManager.CreateRoom(this.<>f__ref$218.config, this.helper, new Action<RealtimeManager.RealTimeRoomResponse>(this.<>f__ref$220.newSession.HandleRoomResponse));
            }
        }

        [CompilerGenerated]
        private sealed class <CreateQuickGame>c__AnonStoreyDC
        {
            internal NativeRealtimeMultiplayerClient <>f__this;
            internal NativeRealtimeMultiplayerClient.RoomSession newSession;
        }

        [CompilerGenerated]
        private sealed class <CreateWithInvitationScreen>c__AnonStoreyDE
        {
            internal NativeRealtimeMultiplayerClient.<CreateWithInvitationScreen>c__AnonStoreyDF <>f__ref$223;
            internal NativeRealtimeMultiplayerClient <>f__this;
            internal NativeRealtimeMultiplayerClient.RoomSession newRoom;

            internal void <>m__47(PlayerSelectUIResponse response)
            {
                this.<>f__this.mCurrentSession.ShowingUI = false;
                if (response.Status() != GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus.VALID)
                {
                    Logger.d("User did not complete invitation screen.");
                    this.newRoom.LeaveRoom();
                }
                else
                {
                    this.<>f__this.mCurrentSession.MinPlayersToStart = (uint) ((response.MinimumAutomatchingPlayers() + response.Count<string>()) + 1);
                    using (RealtimeRoomConfigBuilder builder = RealtimeRoomConfigBuilder.Create())
                    {
                        builder.SetVariant(this.<>f__ref$223.variant);
                        builder.PopulateFromUIResponse(response);
                        <CreateWithInvitationScreen>c__AnonStoreyE0 ye = new <CreateWithInvitationScreen>c__AnonStoreyE0 {
                            <>f__ref$222 = this,
                            config = builder.Build()
                        };
                        try
                        {
                            <CreateWithInvitationScreen>c__AnonStoreyE1 ye2 = new <CreateWithInvitationScreen>c__AnonStoreyE1 {
                                <>f__ref$222 = this,
                                <>f__ref$224 = ye,
                                helper = NativeRealtimeMultiplayerClient.HelperForSession(this.newRoom)
                            };
                            try
                            {
                                this.newRoom.StartRoomCreation(this.<>f__this.mNativeClient.GetUserId(), new Action(ye2.<>m__4C));
                            }
                            finally
                            {
                                if (ye2.helper != null)
                                {
                                    ye2.helper.Dispose();
                                }
                            }
                        }
                        finally
                        {
                            if (ye.config != null)
                            {
                                ye.config.Dispose();
                            }
                        }
                    }
                }
            }

            private sealed class <CreateWithInvitationScreen>c__AnonStoreyE0
            {
                internal NativeRealtimeMultiplayerClient.<CreateWithInvitationScreen>c__AnonStoreyDE <>f__ref$222;
                internal RealtimeRoomConfig config;
            }

            private sealed class <CreateWithInvitationScreen>c__AnonStoreyE1
            {
                internal NativeRealtimeMultiplayerClient.<CreateWithInvitationScreen>c__AnonStoreyDE <>f__ref$222;
                internal NativeRealtimeMultiplayerClient.<CreateWithInvitationScreen>c__AnonStoreyDE.<CreateWithInvitationScreen>c__AnonStoreyE0 <>f__ref$224;
                internal GooglePlayGames.Native.PInvoke.RealTimeEventListenerHelper helper;

                internal void <>m__4C()
                {
                    this.<>f__ref$222.<>f__this.mRealtimeManager.CreateRoom(this.<>f__ref$224.config, this.helper, new Action<RealtimeManager.RealTimeRoomResponse>(this.<>f__ref$222.newRoom.HandleRoomResponse));
                }
            }
        }

        [CompilerGenerated]
        private sealed class <CreateWithInvitationScreen>c__AnonStoreyDF
        {
            internal NativeRealtimeMultiplayerClient <>f__this;
            internal uint variant;
        }

        [CompilerGenerated]
        private sealed class <DeclineInvitation>c__AnonStoreyEA
        {
            internal NativeRealtimeMultiplayerClient <>f__this;
            internal string invitationId;

            internal void <>m__4B(RealtimeManager.FetchInvitationsResponse response)
            {
                if (!response.RequestSucceeded())
                {
                    Logger.e("Couldn't load invitations.");
                }
                else
                {
                    IEnumerator<GooglePlayGames.Native.PInvoke.MultiplayerInvitation> enumerator = response.Invitations().GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            GooglePlayGames.Native.PInvoke.MultiplayerInvitation current = enumerator.Current;
                            using (current)
                            {
                                if (current.Id().Equals(this.invitationId))
                                {
                                    this.<>f__this.mRealtimeManager.DeclineInvitation(current);
                                }
                                continue;
                            }
                        }
                    }
                    finally
                    {
                        if (enumerator == null)
                        {
                        }
                        enumerator.Dispose();
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetAllInvitations>c__AnonStoreyE2
        {
            internal Action<Invitation[]> callback;

            internal void <>m__48(RealtimeManager.FetchInvitationsResponse response)
            {
                if (!response.RequestSucceeded())
                {
                    Logger.e("Couldn't load invitations.");
                    this.callback(new Invitation[0]);
                }
                else
                {
                    List<Invitation> list = new List<Invitation>();
                    IEnumerator<GooglePlayGames.Native.PInvoke.MultiplayerInvitation> enumerator = response.Invitations().GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            GooglePlayGames.Native.PInvoke.MultiplayerInvitation current = enumerator.Current;
                            using (current)
                            {
                                list.Add(current.AsInvitation());
                                continue;
                            }
                        }
                    }
                    finally
                    {
                        if (enumerator == null)
                        {
                        }
                        enumerator.Dispose();
                    }
                    this.callback(list.ToArray());
                }
            }
        }

        [CompilerGenerated]
        private sealed class <HelperForSession>c__AnonStoreyDD
        {
            internal NativeRealtimeMultiplayerClient.RoomSession session;

            internal void <>m__43(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant, byte[] data, bool isReliable)
            {
                this.session.OnDataReceived(room, participant, data, isReliable);
            }

            internal void <>m__44(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant)
            {
                this.session.OnParticipantStatusChanged(room, participant);
            }

            internal void <>m__45(NativeRealTimeRoom room)
            {
                this.session.OnConnectedSetChanged(room);
            }

            internal void <>m__46(NativeRealTimeRoom room)
            {
                this.session.OnRoomStatusChanged(room);
            }
        }

        private class AbortingRoomCreationState : NativeRealtimeMultiplayerClient.State
        {
            private readonly NativeRealtimeMultiplayerClient.RoomSession mSession;

            internal AbortingRoomCreationState(NativeRealtimeMultiplayerClient.RoomSession session)
            {
                this.mSession = Misc.CheckNotNull<NativeRealtimeMultiplayerClient.RoomSession>(session);
            }

            internal override void HandleRoomResponse(RealtimeManager.RealTimeRoomResponse response)
            {
                if (!response.RequestSucceeded())
                {
                    this.mSession.EnterState(new NativeRealtimeMultiplayerClient.ShutdownState(this.mSession));
                    this.mSession.OnGameThreadListener().RoomConnected(false);
                }
                else
                {
                    this.mSession.EnterState(new NativeRealtimeMultiplayerClient.LeavingRoom(this.mSession, response.Room(), () => this.mSession.OnGameThreadListener().RoomConnected(false)));
                }
            }

            internal override bool IsActive() => 
                false;
        }

        private class ActiveState : NativeRealtimeMultiplayerClient.MessagingEnabledState
        {
            [CompilerGenerated]
            private static Func<GooglePlayGames.Native.PInvoke.MultiplayerParticipant, string> <>f__am$cache0;
            [CompilerGenerated]
            private static Func<GooglePlayGames.Native.PInvoke.MultiplayerParticipant, Participant> <>f__am$cache1;
            [CompilerGenerated]
            private static Func<Participant, string> <>f__am$cache2;
            [CompilerGenerated]
            private static Func<Participant, string> <>f__am$cache3;

            internal ActiveState(NativeRealTimeRoom room, NativeRealtimeMultiplayerClient.RoomSession session) : base(session, room)
            {
            }

            internal override Participant GetParticipant(string participantId)
            {
                if (!base.mParticipants.ContainsKey(participantId))
                {
                    Logger.e("Attempted to retrieve unknown participant " + participantId);
                    return null;
                }
                return base.mParticipants[participantId];
            }

            internal override Participant GetSelf()
            {
                foreach (Participant participant in base.mParticipants.Values)
                {
                    if ((participant.Player != null) && participant.Player.id.Equals(base.mSession.SelfPlayerId()))
                    {
                        return participant;
                    }
                }
                return null;
            }

            internal override void HandleConnectedSetChanged(NativeRealTimeRoom room)
            {
                <HandleConnectedSetChanged>c__AnonStoreyF1 yf = new <HandleConnectedSetChanged>c__AnonStoreyF1();
                List<string> source = new List<string>();
                List<string> list2 = new List<string>();
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = p => p.Id();
                }
                Dictionary<string, GooglePlayGames.Native.PInvoke.MultiplayerParticipant> dictionary = room.Participants().ToDictionary<GooglePlayGames.Native.PInvoke.MultiplayerParticipant, string>(<>f__am$cache0);
                foreach (string str in base.mNativeParticipants.Keys)
                {
                    GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant = dictionary[str];
                    GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant2 = base.mNativeParticipants[str];
                    if (!participant.IsConnectedToRoom())
                    {
                        list2.Add(str);
                    }
                    if (!participant2.IsConnectedToRoom() && participant.IsConnectedToRoom())
                    {
                        source.Add(str);
                    }
                }
                foreach (GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant3 in base.mNativeParticipants.Values)
                {
                    participant3.Dispose();
                }
                base.mNativeParticipants = dictionary;
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = p => p.AsParticipant();
                }
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = p => p.ParticipantId;
                }
                base.mParticipants = base.mNativeParticipants.Values.Select<GooglePlayGames.Native.PInvoke.MultiplayerParticipant, Participant>(<>f__am$cache1).ToDictionary<Participant, string>(<>f__am$cache2);
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = p => p.ToString();
                }
                Logger.d("Updated participant statuses: " + string.Join(",", base.mParticipants.Values.Select<Participant, string>(<>f__am$cache3).ToArray<string>()));
                if (list2.Contains(this.GetSelf().ParticipantId))
                {
                    Logger.w("Player was disconnected from the multiplayer session.");
                }
                yf.selfId = this.GetSelf().ParticipantId;
                source = source.Where<string>(new Func<string, bool>(yf.<>m__61)).ToList<string>();
                list2 = list2.Where<string>(new Func<string, bool>(yf.<>m__62)).ToList<string>();
                if (source.Count > 0)
                {
                    source.Sort();
                    base.mSession.OnGameThreadListener().PeersConnected(source.Where<string>(new Func<string, bool>(yf.<>m__63)).ToArray<string>());
                }
                if (list2.Count > 0)
                {
                    list2.Sort();
                    base.mSession.OnGameThreadListener().PeersDisconnected(list2.Where<string>(new Func<string, bool>(yf.<>m__64)).ToArray<string>());
                }
            }

            internal override bool IsRoomConnected() => 
                true;

            internal override void LeaveRoom()
            {
                base.mSession.EnterState(new NativeRealtimeMultiplayerClient.LeavingRoom(base.mSession, base.mRoom, () => base.mSession.OnGameThreadListener().LeftRoom()));
            }

            internal override void OnStateEntered()
            {
                if (this.GetSelf() == null)
                {
                    Logger.e("Room reached active state with unknown participant for the player");
                    this.LeaveRoom();
                }
            }

            [CompilerGenerated]
            private sealed class <HandleConnectedSetChanged>c__AnonStoreyF1
            {
                internal string selfId;

                internal bool <>m__61(string peerId) => 
                    !peerId.Equals(this.selfId);

                internal bool <>m__62(string peerId) => 
                    !peerId.Equals(this.selfId);

                internal bool <>m__63(string peer) => 
                    !peer.Equals(this.selfId);

                internal bool <>m__64(string peer) => 
                    !peer.Equals(this.selfId);
            }
        }

        private class BeforeRoomCreateStartedState : NativeRealtimeMultiplayerClient.State
        {
            private readonly NativeRealtimeMultiplayerClient.RoomSession mContainingSession;

            internal BeforeRoomCreateStartedState(NativeRealtimeMultiplayerClient.RoomSession session)
            {
                this.mContainingSession = Misc.CheckNotNull<NativeRealtimeMultiplayerClient.RoomSession>(session);
            }

            internal override void LeaveRoom()
            {
                Logger.d("Session was torn down before room was created.");
                this.mContainingSession.OnGameThreadListener().RoomConnected(false);
                this.mContainingSession.EnterState(new NativeRealtimeMultiplayerClient.ShutdownState(this.mContainingSession));
            }
        }

        private class ConnectingState : NativeRealtimeMultiplayerClient.MessagingEnabledState
        {
            private static readonly HashSet<Types.ParticipantStatus> FailedStatuses;
            private const float InitialPercentComplete = 20f;
            private HashSet<string> mConnectedParticipants;
            private float mPercentComplete;
            private float mPercentPerParticipant;

            static ConnectingState()
            {
                HashSet<Types.ParticipantStatus> set = new HashSet<Types.ParticipantStatus> {
                    Types.ParticipantStatus.DECLINED,
                    Types.ParticipantStatus.LEFT
                };
                FailedStatuses = set;
            }

            internal ConnectingState(NativeRealTimeRoom room, NativeRealtimeMultiplayerClient.RoomSession session) : base(session, room)
            {
                this.mConnectedParticipants = new HashSet<string>();
                this.mPercentComplete = 20f;
                this.mPercentPerParticipant = 80f / ((float) session.MinPlayersToStart);
            }

            internal override void HandleConnectedSetChanged(NativeRealTimeRoom room)
            {
                HashSet<string> set = new HashSet<string>();
                if (((room.Status() == Types.RealTimeRoomStatus.AUTO_MATCHING) || (room.Status() == Types.RealTimeRoomStatus.CONNECTING)) && (base.mSession.MinPlayersToStart <= room.ParticipantCount()))
                {
                    base.mSession.MinPlayersToStart += room.ParticipantCount();
                    this.mPercentPerParticipant = 80f / ((float) base.mSession.MinPlayersToStart);
                }
                IEnumerator<GooglePlayGames.Native.PInvoke.MultiplayerParticipant> enumerator = room.Participants().GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        GooglePlayGames.Native.PInvoke.MultiplayerParticipant current = enumerator.Current;
                        using (current)
                        {
                            if (current.IsConnectedToRoom())
                            {
                                set.Add(current.Id());
                            }
                            continue;
                        }
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
                if (this.mConnectedParticipants.Equals(set))
                {
                    Logger.w("Received connected set callback with unchanged connected set!");
                }
                else
                {
                    IEnumerable<string> source = this.mConnectedParticipants.Except<string>(set);
                    if (room.Status() == Types.RealTimeRoomStatus.DELETED)
                    {
                        Logger.e("Participants disconnected during room setup, failing. Participants were: " + string.Join(",", source.ToArray<string>()));
                        base.mSession.OnGameThreadListener().RoomConnected(false);
                        base.mSession.EnterState(new NativeRealtimeMultiplayerClient.ShutdownState(base.mSession));
                    }
                    else
                    {
                        IEnumerable<string> enumerable2 = set.Except<string>(this.mConnectedParticipants);
                        Logger.d("New participants connected: " + string.Join(",", enumerable2.ToArray<string>()));
                        if (room.Status() == Types.RealTimeRoomStatus.ACTIVE)
                        {
                            Logger.d("Fully connected! Transitioning to active state.");
                            base.mSession.EnterState(new NativeRealtimeMultiplayerClient.ActiveState(room, base.mSession));
                            base.mSession.OnGameThreadListener().RoomConnected(true);
                        }
                        else
                        {
                            this.mPercentComplete += this.mPercentPerParticipant * enumerable2.Count<string>();
                            this.mConnectedParticipants = set;
                            base.mSession.OnGameThreadListener().RoomSetupProgress(this.mPercentComplete);
                        }
                    }
                }
            }

            internal override void HandleParticipantStatusChanged(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant)
            {
                if (FailedStatuses.Contains(participant.Status()))
                {
                    base.mSession.OnGameThreadListener().ParticipantLeft(participant.AsParticipant());
                    if ((room.Status() != Types.RealTimeRoomStatus.CONNECTING) && (room.Status() != Types.RealTimeRoomStatus.AUTO_MATCHING))
                    {
                        this.LeaveRoom();
                    }
                }
            }

            internal override void LeaveRoom()
            {
                base.mSession.EnterState(new NativeRealtimeMultiplayerClient.LeavingRoom(base.mSession, base.mRoom, () => base.mSession.OnGameThreadListener().RoomConnected(false)));
            }

            internal override void OnStateEntered()
            {
                base.mSession.OnGameThreadListener().RoomSetupProgress(this.mPercentComplete);
            }

            internal override void ShowWaitingRoomUI(uint minimumParticipantsBeforeStarting)
            {
                base.mSession.ShowingUI = true;
                base.mSession.Manager().ShowWaitingRoomUI(base.mRoom, minimumParticipantsBeforeStarting, delegate (RealtimeManager.WaitingRoomUIResponse response) {
                    base.mSession.ShowingUI = false;
                    Logger.d("ShowWaitingRoomUI Response: " + response.ResponseStatus());
                    if (response.ResponseStatus() == GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus.VALID)
                    {
                        Logger.d(string.Concat(new object[] { "Connecting state ShowWaitingRoomUI: room pcount:", response.Room().ParticipantCount(), " status: ", response.Room().Status() }));
                        if (response.Room().Status() == Types.RealTimeRoomStatus.ACTIVE)
                        {
                            base.mSession.EnterState(new NativeRealtimeMultiplayerClient.ActiveState(response.Room(), base.mSession));
                        }
                    }
                    else if (response.ResponseStatus() == GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus.ERROR_LEFT_ROOM)
                    {
                        this.LeaveRoom();
                    }
                    else
                    {
                        base.mSession.OnGameThreadListener().RoomSetupProgress(this.mPercentComplete);
                    }
                });
            }
        }

        private class LeavingRoom : NativeRealtimeMultiplayerClient.State
        {
            private readonly Action mLeavingCompleteCallback;
            private readonly NativeRealTimeRoom mRoomToLeave;
            private readonly NativeRealtimeMultiplayerClient.RoomSession mSession;

            internal LeavingRoom(NativeRealtimeMultiplayerClient.RoomSession session, NativeRealTimeRoom room, Action leavingCompleteCallback)
            {
                this.mSession = Misc.CheckNotNull<NativeRealtimeMultiplayerClient.RoomSession>(session);
                this.mRoomToLeave = Misc.CheckNotNull<NativeRealTimeRoom>(room);
                this.mLeavingCompleteCallback = Misc.CheckNotNull<Action>(leavingCompleteCallback);
            }

            internal override bool IsActive() => 
                false;

            internal override void OnStateEntered()
            {
                this.mSession.Manager().LeaveRoom(this.mRoomToLeave, delegate (GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus status) {
                    this.mLeavingCompleteCallback();
                    this.mSession.EnterState(new NativeRealtimeMultiplayerClient.ShutdownState(this.mSession));
                });
            }
        }

        private abstract class MessagingEnabledState : NativeRealtimeMultiplayerClient.State
        {
            [CompilerGenerated]
            private static Func<GooglePlayGames.Native.PInvoke.MultiplayerParticipant, string> <>f__am$cache4;
            [CompilerGenerated]
            private static Func<GooglePlayGames.Native.PInvoke.MultiplayerParticipant, Participant> <>f__am$cache5;
            [CompilerGenerated]
            private static Func<Participant, string> <>f__am$cache6;
            [CompilerGenerated]
            private static Func<Participant, bool> <>f__am$cache7;
            protected Dictionary<string, GooglePlayGames.Native.PInvoke.MultiplayerParticipant> mNativeParticipants;
            protected Dictionary<string, Participant> mParticipants;
            protected NativeRealTimeRoom mRoom;
            protected readonly NativeRealtimeMultiplayerClient.RoomSession mSession;

            internal MessagingEnabledState(NativeRealtimeMultiplayerClient.RoomSession session, NativeRealTimeRoom room)
            {
                this.mSession = Misc.CheckNotNull<NativeRealtimeMultiplayerClient.RoomSession>(session);
                this.UpdateCurrentRoom(room);
            }

            internal sealed override List<Participant> GetConnectedParticipants()
            {
                if (<>f__am$cache7 == null)
                {
                    <>f__am$cache7 = p => p.IsConnectedToRoom;
                }
                List<Participant> list = this.mParticipants.Values.Where<Participant>(<>f__am$cache7).ToList<Participant>();
                list.Sort();
                return list;
            }

            internal virtual void HandleConnectedSetChanged(NativeRealTimeRoom room)
            {
            }

            internal virtual void HandleParticipantStatusChanged(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant)
            {
            }

            internal virtual void HandleRoomStatusChanged(NativeRealTimeRoom room)
            {
            }

            internal sealed override void OnConnectedSetChanged(NativeRealTimeRoom room)
            {
                this.HandleConnectedSetChanged(room);
                this.UpdateCurrentRoom(room);
            }

            internal override void OnDataReceived(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant sender, byte[] data, bool isReliable)
            {
                this.mSession.OnGameThreadListener().RealTimeMessageReceived(isReliable, sender.Id(), data);
            }

            internal sealed override void OnParticipantStatusChanged(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant)
            {
                this.HandleParticipantStatusChanged(room, participant);
                this.UpdateCurrentRoom(room);
            }

            internal sealed override void OnRoomStatusChanged(NativeRealTimeRoom room)
            {
                this.HandleRoomStatusChanged(room);
                this.UpdateCurrentRoom(room);
            }

            internal override void SendToAll(byte[] data, int offset, int length, bool isReliable)
            {
                byte[] buffer = Misc.GetSubsetBytes(data, offset, length);
                if (isReliable)
                {
                    foreach (string str in this.mNativeParticipants.Keys)
                    {
                        this.SendToSpecificRecipient(str, buffer, 0, buffer.Length, true);
                    }
                }
                else
                {
                    this.mSession.Manager().SendUnreliableMessageToAll(this.mRoom, buffer);
                }
            }

            internal override void SendToSpecificRecipient(string recipientId, byte[] data, int offset, int length, bool isReliable)
            {
                if (!this.mNativeParticipants.ContainsKey(recipientId))
                {
                    Logger.e("Attempted to send message to unknown participant " + recipientId);
                }
                else if (isReliable)
                {
                    this.mSession.Manager().SendReliableMessage(this.mRoom, this.mNativeParticipants[recipientId], Misc.GetSubsetBytes(data, offset, length), null);
                }
                else
                {
                    List<GooglePlayGames.Native.PInvoke.MultiplayerParticipant> recipients = new List<GooglePlayGames.Native.PInvoke.MultiplayerParticipant> {
                        this.mNativeParticipants[recipientId]
                    };
                    this.mSession.Manager().SendUnreliableMessageToSpecificParticipants(this.mRoom, recipients, Misc.GetSubsetBytes(data, offset, length));
                }
            }

            internal void UpdateCurrentRoom(NativeRealTimeRoom room)
            {
                if (this.mRoom != null)
                {
                    this.mRoom.Dispose();
                }
                this.mRoom = Misc.CheckNotNull<NativeRealTimeRoom>(room);
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = p => p.Id();
                }
                this.mNativeParticipants = this.mRoom.Participants().ToDictionary<GooglePlayGames.Native.PInvoke.MultiplayerParticipant, string>(<>f__am$cache4);
                if (<>f__am$cache5 == null)
                {
                    <>f__am$cache5 = p => p.AsParticipant();
                }
                if (<>f__am$cache6 == null)
                {
                    <>f__am$cache6 = p => p.ParticipantId;
                }
                this.mParticipants = this.mNativeParticipants.Values.Select<GooglePlayGames.Native.PInvoke.MultiplayerParticipant, Participant>(<>f__am$cache5).ToDictionary<Participant, string>(<>f__am$cache6);
            }
        }

        private class NoopListener : RealTimeMultiplayerListener
        {
            public void OnLeftRoom()
            {
            }

            public void OnParticipantLeft(Participant participant)
            {
            }

            public void OnPeersConnected(string[] participantIds)
            {
            }

            public void OnPeersDisconnected(string[] participantIds)
            {
            }

            public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
            {
            }

            public void OnRoomConnected(bool success)
            {
            }

            public void OnRoomSetupProgress(float percent)
            {
            }
        }

        private class OnGameThreadForwardingListener
        {
            private readonly RealTimeMultiplayerListener mListener;

            internal OnGameThreadForwardingListener(RealTimeMultiplayerListener listener)
            {
                this.mListener = Misc.CheckNotNull<RealTimeMultiplayerListener>(listener);
            }

            public void LeftRoom()
            {
                PlayGamesHelperObject.RunOnGameThread(() => this.mListener.OnLeftRoom());
            }

            public void ParticipantLeft(Participant participant)
            {
                <ParticipantLeft>c__AnonStoreyF0 yf = new <ParticipantLeft>c__AnonStoreyF0 {
                    participant = participant,
                    <>f__this = this
                };
                PlayGamesHelperObject.RunOnGameThread(new Action(yf.<>m__56));
            }

            public void PeersConnected(string[] participantIds)
            {
                <PeersConnected>c__AnonStoreyED yed = new <PeersConnected>c__AnonStoreyED {
                    participantIds = participantIds,
                    <>f__this = this
                };
                PlayGamesHelperObject.RunOnGameThread(new Action(yed.<>m__53));
            }

            public void PeersDisconnected(string[] participantIds)
            {
                <PeersDisconnected>c__AnonStoreyEE yee = new <PeersDisconnected>c__AnonStoreyEE {
                    participantIds = participantIds,
                    <>f__this = this
                };
                PlayGamesHelperObject.RunOnGameThread(new Action(yee.<>m__54));
            }

            public void RealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
            {
                <RealTimeMessageReceived>c__AnonStoreyEF yef = new <RealTimeMessageReceived>c__AnonStoreyEF {
                    isReliable = isReliable,
                    senderId = senderId,
                    data = data,
                    <>f__this = this
                };
                PlayGamesHelperObject.RunOnGameThread(new Action(yef.<>m__55));
            }

            public void RoomConnected(bool success)
            {
                <RoomConnected>c__AnonStoreyEC yec = new <RoomConnected>c__AnonStoreyEC {
                    success = success,
                    <>f__this = this
                };
                PlayGamesHelperObject.RunOnGameThread(new Action(yec.<>m__51));
            }

            public void RoomSetupProgress(float percent)
            {
                <RoomSetupProgress>c__AnonStoreyEB yeb = new <RoomSetupProgress>c__AnonStoreyEB {
                    percent = percent,
                    <>f__this = this
                };
                PlayGamesHelperObject.RunOnGameThread(new Action(yeb.<>m__50));
            }

            [CompilerGenerated]
            private sealed class <ParticipantLeft>c__AnonStoreyF0
            {
                internal NativeRealtimeMultiplayerClient.OnGameThreadForwardingListener <>f__this;
                internal Participant participant;

                internal void <>m__56()
                {
                    this.<>f__this.mListener.OnParticipantLeft(this.participant);
                }
            }

            [CompilerGenerated]
            private sealed class <PeersConnected>c__AnonStoreyED
            {
                internal NativeRealtimeMultiplayerClient.OnGameThreadForwardingListener <>f__this;
                internal string[] participantIds;

                internal void <>m__53()
                {
                    this.<>f__this.mListener.OnPeersConnected(this.participantIds);
                }
            }

            [CompilerGenerated]
            private sealed class <PeersDisconnected>c__AnonStoreyEE
            {
                internal NativeRealtimeMultiplayerClient.OnGameThreadForwardingListener <>f__this;
                internal string[] participantIds;

                internal void <>m__54()
                {
                    this.<>f__this.mListener.OnPeersDisconnected(this.participantIds);
                }
            }

            [CompilerGenerated]
            private sealed class <RealTimeMessageReceived>c__AnonStoreyEF
            {
                internal NativeRealtimeMultiplayerClient.OnGameThreadForwardingListener <>f__this;
                internal byte[] data;
                internal bool isReliable;
                internal string senderId;

                internal void <>m__55()
                {
                    this.<>f__this.mListener.OnRealTimeMessageReceived(this.isReliable, this.senderId, this.data);
                }
            }

            [CompilerGenerated]
            private sealed class <RoomConnected>c__AnonStoreyEC
            {
                internal NativeRealtimeMultiplayerClient.OnGameThreadForwardingListener <>f__this;
                internal bool success;

                internal void <>m__51()
                {
                    this.<>f__this.mListener.OnRoomConnected(this.success);
                }
            }

            [CompilerGenerated]
            private sealed class <RoomSetupProgress>c__AnonStoreyEB
            {
                internal NativeRealtimeMultiplayerClient.OnGameThreadForwardingListener <>f__this;
                internal float percent;

                internal void <>m__50()
                {
                    this.<>f__this.mListener.OnRoomSetupProgress(this.percent);
                }
            }
        }

        private class RoomCreationPendingState : NativeRealtimeMultiplayerClient.State
        {
            private readonly NativeRealtimeMultiplayerClient.RoomSession mContainingSession;

            internal RoomCreationPendingState(NativeRealtimeMultiplayerClient.RoomSession session)
            {
                this.mContainingSession = Misc.CheckNotNull<NativeRealtimeMultiplayerClient.RoomSession>(session);
            }

            internal override void HandleRoomResponse(RealtimeManager.RealTimeRoomResponse response)
            {
                if (!response.RequestSucceeded())
                {
                    this.mContainingSession.EnterState(new NativeRealtimeMultiplayerClient.ShutdownState(this.mContainingSession));
                    this.mContainingSession.OnGameThreadListener().RoomConnected(false);
                }
                else
                {
                    this.mContainingSession.EnterState(new NativeRealtimeMultiplayerClient.ConnectingState(response.Room(), this.mContainingSession));
                }
            }

            internal override bool IsActive() => 
                true;

            internal override void LeaveRoom()
            {
                Logger.d("Received request to leave room during room creation, aborting creation.");
                this.mContainingSession.EnterState(new NativeRealtimeMultiplayerClient.AbortingRoomCreationState(this.mContainingSession));
            }
        }

        private class RoomSession
        {
            private volatile string mCurrentPlayerId;
            private Invitation mInvitation;
            private readonly object mLifecycleLock = new object();
            private readonly NativeRealtimeMultiplayerClient.OnGameThreadForwardingListener mListener;
            private readonly RealtimeManager mManager;
            private uint mMinPlayersToStart;
            private volatile bool mShowingUI;
            private volatile NativeRealtimeMultiplayerClient.State mState;
            private volatile bool mStillPreRoomCreation;

            internal RoomSession(RealtimeManager manager, RealTimeMultiplayerListener listener)
            {
                this.mManager = Misc.CheckNotNull<RealtimeManager>(manager);
                this.mListener = new NativeRealtimeMultiplayerClient.OnGameThreadForwardingListener(listener);
                this.EnterState(new NativeRealtimeMultiplayerClient.BeforeRoomCreateStartedState(this), false);
                this.mStillPreRoomCreation = true;
            }

            internal void EnterState(NativeRealtimeMultiplayerClient.State handler)
            {
                this.EnterState(handler, true);
            }

            internal void EnterState(NativeRealtimeMultiplayerClient.State handler, bool fireStateEnteredEvent)
            {
                object mLifecycleLock = this.mLifecycleLock;
                lock (mLifecycleLock)
                {
                    this.mState = Misc.CheckNotNull<NativeRealtimeMultiplayerClient.State>(handler);
                    if (fireStateEnteredEvent)
                    {
                        Logger.d("Entering state: " + handler.GetType().Name);
                        this.mState.OnStateEntered();
                    }
                }
            }

            internal List<Participant> GetConnectedParticipants() => 
                this.mState.GetConnectedParticipants();

            public Invitation GetInvitation() => 
                this.mInvitation;

            internal virtual Participant GetParticipant(string participantId) => 
                this.mState.GetParticipant(participantId);

            internal virtual Participant GetSelf() => 
                this.mState.GetSelf();

            internal void HandleRoomResponse(RealtimeManager.RealTimeRoomResponse response)
            {
                object mLifecycleLock = this.mLifecycleLock;
                lock (mLifecycleLock)
                {
                    this.mState.HandleRoomResponse(response);
                }
            }

            internal bool IsActive() => 
                this.mState.IsActive();

            internal virtual bool IsRoomConnected() => 
                this.mState.IsRoomConnected();

            internal void LeaveRoom()
            {
                if (!this.ShowingUI)
                {
                    object mLifecycleLock = this.mLifecycleLock;
                    lock (mLifecycleLock)
                    {
                        this.mState.LeaveRoom();
                    }
                }
                else
                {
                    Logger.d("Not leaving room since showing UI");
                }
            }

            internal RealtimeManager Manager() => 
                this.mManager;

            internal void OnConnectedSetChanged(NativeRealTimeRoom room)
            {
                object mLifecycleLock = this.mLifecycleLock;
                lock (mLifecycleLock)
                {
                    this.mState.OnConnectedSetChanged(room);
                }
            }

            internal void OnDataReceived(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant sender, byte[] data, bool isReliable)
            {
                this.mState.OnDataReceived(room, sender, data, isReliable);
            }

            internal NativeRealtimeMultiplayerClient.OnGameThreadForwardingListener OnGameThreadListener() => 
                this.mListener;

            internal void OnParticipantStatusChanged(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant)
            {
                object mLifecycleLock = this.mLifecycleLock;
                lock (mLifecycleLock)
                {
                    this.mState.OnParticipantStatusChanged(room, participant);
                }
            }

            internal void OnRoomStatusChanged(NativeRealTimeRoom room)
            {
                object mLifecycleLock = this.mLifecycleLock;
                lock (mLifecycleLock)
                {
                    this.mState.OnRoomStatusChanged(room);
                }
            }

            internal string SelfPlayerId() => 
                this.mCurrentPlayerId;

            internal void SendMessage(bool reliable, string participantId, byte[] data)
            {
                this.SendMessage(reliable, participantId, data, 0, data.Length);
            }

            internal void SendMessage(bool reliable, string participantId, byte[] data, int offset, int length)
            {
                this.mState.SendToSpecificRecipient(participantId, data, offset, length, reliable);
            }

            internal void SendMessageToAll(bool reliable, byte[] data)
            {
                this.SendMessageToAll(reliable, data, 0, data.Length);
            }

            internal void SendMessageToAll(bool reliable, byte[] data, int offset, int length)
            {
                this.mState.SendToAll(data, offset, length, reliable);
            }

            public void SetInvitation(Invitation invitation)
            {
                this.mInvitation = invitation;
            }

            internal void ShowWaitingRoomUI()
            {
                this.mState.ShowWaitingRoomUI(this.MinPlayersToStart);
            }

            internal void StartRoomCreation(string currentPlayerId, Action createRoom)
            {
                object mLifecycleLock = this.mLifecycleLock;
                lock (mLifecycleLock)
                {
                    if (!this.mStillPreRoomCreation)
                    {
                        Logger.e("Room creation started more than once, this shouldn't happen!");
                    }
                    else if (!this.mState.IsActive())
                    {
                        Logger.w("Received an attempt to create a room after the session was already torn down!");
                    }
                    else
                    {
                        this.mCurrentPlayerId = Misc.CheckNotNull<string>(currentPlayerId);
                        this.mStillPreRoomCreation = false;
                        this.EnterState(new NativeRealtimeMultiplayerClient.RoomCreationPendingState(this));
                        createRoom();
                    }
                }
            }

            internal uint MinPlayersToStart
            {
                get => 
                    this.mMinPlayersToStart;
                set
                {
                    this.mMinPlayersToStart = value;
                }
            }

            internal bool ShowingUI
            {
                get => 
                    this.mShowingUI;
                set
                {
                    this.mShowingUI = value;
                }
            }
        }

        private class ShutdownState : NativeRealtimeMultiplayerClient.State
        {
            private readonly NativeRealtimeMultiplayerClient.RoomSession mSession;

            internal ShutdownState(NativeRealtimeMultiplayerClient.RoomSession session)
            {
                this.mSession = Misc.CheckNotNull<NativeRealtimeMultiplayerClient.RoomSession>(session);
            }

            internal override bool IsActive() => 
                false;

            internal override void LeaveRoom()
            {
                this.mSession.OnGameThreadListener().LeftRoom();
            }
        }

        internal abstract class State
        {
            protected State()
            {
            }

            internal virtual List<Participant> GetConnectedParticipants()
            {
                Logger.d(base.GetType().Name + ".GetConnectedParticipants: Returning empty connected participants");
                return new List<Participant>();
            }

            internal virtual Participant GetParticipant(string participantId)
            {
                Logger.d(base.GetType().Name + ".GetSelf: Returning null participant.");
                return null;
            }

            internal virtual Participant GetSelf()
            {
                Logger.d(base.GetType().Name + ".GetSelf: Returning null self.");
                return null;
            }

            internal virtual void HandleRoomResponse(RealtimeManager.RealTimeRoomResponse response)
            {
                Logger.d(base.GetType().Name + ".HandleRoomResponse: Defaulting to no-op.");
            }

            internal virtual bool IsActive()
            {
                Logger.d(base.GetType().Name + ".IsNonPreemptable: Is preemptable by default.");
                return true;
            }

            internal virtual bool IsRoomConnected()
            {
                Logger.d(base.GetType().Name + ".IsRoomConnected: Returning room not connected.");
                return false;
            }

            internal virtual void LeaveRoom()
            {
                Logger.d(base.GetType().Name + ".LeaveRoom: Defaulting to no-op.");
            }

            internal virtual void OnConnectedSetChanged(NativeRealTimeRoom room)
            {
                Logger.d(base.GetType().Name + ".OnConnectedSetChanged: Defaulting to no-op.");
            }

            internal virtual void OnDataReceived(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant sender, byte[] data, bool isReliable)
            {
                Logger.d(base.GetType().Name + ".OnDataReceived: Defaulting to no-op.");
            }

            internal virtual void OnParticipantStatusChanged(NativeRealTimeRoom room, GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant)
            {
                Logger.d(base.GetType().Name + ".OnParticipantStatusChanged: Defaulting to no-op.");
            }

            internal virtual void OnRoomStatusChanged(NativeRealTimeRoom room)
            {
                Logger.d(base.GetType().Name + ".OnRoomStatusChanged: Defaulting to no-op.");
            }

            internal virtual void OnStateEntered()
            {
                Logger.d(base.GetType().Name + ".OnStateEntered: Defaulting to no-op.");
            }

            internal virtual void SendToAll(byte[] data, int offset, int length, bool isReliable)
            {
                Logger.d(base.GetType().Name + ".SendToApp: Defaulting to no-op.");
            }

            internal virtual void SendToSpecificRecipient(string recipientId, byte[] data, int offset, int length, bool isReliable)
            {
                Logger.d(base.GetType().Name + ".SendToSpecificRecipient: Defaulting to no-op.");
            }

            internal virtual void ShowWaitingRoomUI(uint minimumParticipantsBeforeStarting)
            {
                Logger.d(base.GetType().Name + ".ShowWaitingRoomUI: Defaulting to no-op.");
            }
        }
    }
}

