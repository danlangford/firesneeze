namespace GooglePlayGames.Native
{
    using GooglePlayGames.BasicApi;
    using GooglePlayGames.BasicApi.Multiplayer;
    using GooglePlayGames.Native.Cwrapper;
    using GooglePlayGames.Native.PInvoke;
    using GooglePlayGames.OurUtils;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class NativeTurnBasedMultiplayerClient : ITurnBasedMultiplayerClient
    {
        private volatile Action<GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch, bool> mMatchDelegate;
        private readonly NativeClient mNativeClient;
        private readonly TurnBasedManager mTurnBasedManager;

        internal NativeTurnBasedMultiplayerClient(NativeClient nativeClient, TurnBasedManager manager)
        {
            this.mTurnBasedManager = manager;
            this.mNativeClient = nativeClient;
        }

        public void AcceptFromInbox(Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback)
        {
            <AcceptFromInbox>c__AnonStorey103 storey = new <AcceptFromInbox>c__AnonStorey103 {
                callback = callback,
                <>f__this = this
            };
            storey.callback = Callbacks.AsOnGameThreadCallback<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch>(storey.callback);
            this.mTurnBasedManager.ShowInboxUI(new Action<TurnBasedManager.MatchInboxUIResponse>(storey.<>m__7D));
        }

        public void AcceptInvitation(string invitationId, Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback)
        {
            <AcceptInvitation>c__AnonStorey104 storey = new <AcceptInvitation>c__AnonStorey104 {
                invitationId = invitationId,
                callback = callback,
                <>f__this = this
            };
            storey.callback = Callbacks.AsOnGameThreadCallback<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch>(storey.callback);
            this.FindInvitationWithId(storey.invitationId, new Action<GooglePlayGames.Native.PInvoke.MultiplayerInvitation>(storey.<>m__7E));
        }

        public void AcknowledgeFinished(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, Action<bool> callback)
        {
            <AcknowledgeFinished>c__AnonStorey10C storeyc = new <AcknowledgeFinished>c__AnonStorey10C {
                callback = callback,
                <>f__this = this
            };
            storeyc.callback = Callbacks.AsOnGameThreadCallback<bool>(storeyc.callback);
            this.FindEqualVersionMatch(match, storeyc.callback, new Action<NativeTurnBasedMatch>(storeyc.<>m__86));
        }

        private Action<TurnBasedManager.TurnBasedMatchResponse> BridgeMatchToUserCallback(Action<GooglePlayGames.BasicApi.UIStatus, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> userCallback)
        {
            <BridgeMatchToUserCallback>c__AnonStorey102 storey = new <BridgeMatchToUserCallback>c__AnonStorey102 {
                userCallback = userCallback,
                <>f__this = this
            };
            return new Action<TurnBasedManager.TurnBasedMatchResponse>(storey.<>m__7C);
        }

        public void Cancel(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, Action<bool> callback)
        {
            <Cancel>c__AnonStorey10F storeyf = new <Cancel>c__AnonStorey10F {
                callback = callback,
                <>f__this = this
            };
            storeyf.callback = Callbacks.AsOnGameThreadCallback<bool>(storeyf.callback);
            this.FindEqualVersionMatch(match, storeyf.callback, new Action<NativeTurnBasedMatch>(storeyf.<>m__89));
        }

        public void CreateQuickMatch(uint minOpponents, uint maxOpponents, uint variant, Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback)
        {
            this.CreateQuickMatch(minOpponents, maxOpponents, variant, 0L, callback);
        }

        public void CreateQuickMatch(uint minOpponents, uint maxOpponents, uint variant, ulong exclusiveBitmask, Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback)
        {
            <CreateQuickMatch>c__AnonStoreyFD yfd = new <CreateQuickMatch>c__AnonStoreyFD {
                callback = callback
            };
            yfd.callback = Callbacks.AsOnGameThreadCallback<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch>(yfd.callback);
            using (GooglePlayGames.Native.PInvoke.TurnBasedMatchConfigBuilder builder = GooglePlayGames.Native.PInvoke.TurnBasedMatchConfigBuilder.Create())
            {
                builder.SetVariant(variant).SetMinimumAutomatchingPlayers(minOpponents).SetMaximumAutomatchingPlayers(maxOpponents).SetExclusiveBitMask(exclusiveBitmask);
                using (GooglePlayGames.Native.PInvoke.TurnBasedMatchConfig config = builder.Build())
                {
                    this.mTurnBasedManager.CreateMatch(config, this.BridgeMatchToUserCallback(new Action<GooglePlayGames.BasicApi.UIStatus, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch>(yfd.<>m__77)));
                }
            }
        }

        public void CreateWithInvitationScreen(uint minOpponents, uint maxOpponents, uint variant, Action<GooglePlayGames.BasicApi.UIStatus, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback)
        {
            <CreateWithInvitationScreen>c__AnonStoreyFF yff = new <CreateWithInvitationScreen>c__AnonStoreyFF {
                callback = callback,
                variant = variant,
                <>f__this = this
            };
            yff.callback = Callbacks.AsOnGameThreadCallback<GooglePlayGames.BasicApi.UIStatus, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch>(yff.callback);
            this.mTurnBasedManager.ShowPlayerSelectUI(minOpponents, maxOpponents, true, new Action<PlayerSelectUIResponse>(yff.<>m__79));
        }

        public void CreateWithInvitationScreen(uint minOpponents, uint maxOpponents, uint variant, Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback)
        {
            <CreateWithInvitationScreen>c__AnonStoreyFE yfe = new <CreateWithInvitationScreen>c__AnonStoreyFE {
                callback = callback
            };
            this.CreateWithInvitationScreen(minOpponents, maxOpponents, variant, new Action<GooglePlayGames.BasicApi.UIStatus, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch>(yfe.<>m__78));
        }

        public void DeclineInvitation(string invitationId)
        {
            this.FindInvitationWithId(invitationId, delegate (GooglePlayGames.Native.PInvoke.MultiplayerInvitation invitation) {
                if (invitation != null)
                {
                    this.mTurnBasedManager.DeclineInvitation(invitation);
                }
            });
        }

        private void FindEqualVersionMatch(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, Action<bool> onFailure, Action<NativeTurnBasedMatch> onVersionMatch)
        {
            <FindEqualVersionMatch>c__AnonStorey109 storey = new <FindEqualVersionMatch>c__AnonStorey109 {
                match = match,
                onFailure = onFailure,
                onVersionMatch = onVersionMatch
            };
            this.mTurnBasedManager.GetMatch(storey.match.MatchId, new Action<TurnBasedManager.TurnBasedMatchResponse>(storey.<>m__83));
        }

        private void FindEqualVersionMatchWithParticipant(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, string participantId, Action<bool> onFailure, Action<GooglePlayGames.Native.PInvoke.MultiplayerParticipant, NativeTurnBasedMatch> onFoundParticipantAndMatch)
        {
            <FindEqualVersionMatchWithParticipant>c__AnonStorey10A storeya = new <FindEqualVersionMatchWithParticipant>c__AnonStorey10A {
                participantId = participantId,
                onFoundParticipantAndMatch = onFoundParticipantAndMatch,
                match = match,
                onFailure = onFailure
            };
            this.FindEqualVersionMatch(storeya.match, storeya.onFailure, new Action<NativeTurnBasedMatch>(storeya.<>m__84));
        }

        private void FindInvitationWithId(string invitationId, Action<GooglePlayGames.Native.PInvoke.MultiplayerInvitation> callback)
        {
            <FindInvitationWithId>c__AnonStorey105 storey = new <FindInvitationWithId>c__AnonStorey105 {
                callback = callback,
                invitationId = invitationId
            };
            this.mTurnBasedManager.GetAllTurnbasedMatches(new Action<TurnBasedManager.TurnBasedMatchesResponse>(storey.<>m__7F));
        }

        public void Finish(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, byte[] data, MatchOutcome outcome, Action<bool> callback)
        {
            <Finish>c__AnonStorey10B storeyb = new <Finish>c__AnonStorey10B {
                outcome = outcome,
                callback = callback,
                data = data,
                <>f__this = this
            };
            storeyb.callback = Callbacks.AsOnGameThreadCallback<bool>(storeyb.callback);
            this.FindEqualVersionMatch(match, storeyb.callback, new Action<NativeTurnBasedMatch>(storeyb.<>m__85));
        }

        public void GetAllInvitations(Action<Invitation[]> callback)
        {
            <GetAllInvitations>c__AnonStorey100 storey = new <GetAllInvitations>c__AnonStorey100 {
                callback = callback
            };
            this.mTurnBasedManager.GetAllTurnbasedMatches(new Action<TurnBasedManager.TurnBasedMatchesResponse>(storey.<>m__7A));
        }

        public void GetAllMatches(Action<GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch[]> callback)
        {
            <GetAllMatches>c__AnonStorey101 storey = new <GetAllMatches>c__AnonStorey101 {
                callback = callback,
                <>f__this = this
            };
            this.mTurnBasedManager.GetAllTurnbasedMatches(new Action<TurnBasedManager.TurnBasedMatchesResponse>(storey.<>m__7B));
        }

        public int GetMaxMatchDataSize()
        {
            throw new NotImplementedException();
        }

        internal void HandleMatchEvent(Types.MultiplayerEvent eventType, string matchId, NativeTurnBasedMatch match)
        {
            <HandleMatchEvent>c__AnonStorey107 storey = new <HandleMatchEvent>c__AnonStorey107 {
                match = match,
                <>f__this = this,
                currentDelegate = this.mMatchDelegate
            };
            if (storey.currentDelegate != null)
            {
                if (eventType == Types.MultiplayerEvent.REMOVED)
                {
                    Logger.d("Ignoring REMOVE event for match " + matchId);
                }
                else
                {
                    storey.shouldAutolaunch = eventType == Types.MultiplayerEvent.UPDATED_FROM_APP_LAUNCH;
                    storey.match.ReferToMe();
                    Callbacks.AsCoroutine(this.WaitForLogin(new Action(storey.<>m__81)));
                }
            }
        }

        public void Leave(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, Action<bool> callback)
        {
            <Leave>c__AnonStorey10D storeyd = new <Leave>c__AnonStorey10D {
                callback = callback,
                <>f__this = this
            };
            storeyd.callback = Callbacks.AsOnGameThreadCallback<bool>(storeyd.callback);
            this.FindEqualVersionMatch(match, storeyd.callback, new Action<NativeTurnBasedMatch>(storeyd.<>m__87));
        }

        public void LeaveDuringTurn(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, string pendingParticipantId, Action<bool> callback)
        {
            <LeaveDuringTurn>c__AnonStorey10E storeye = new <LeaveDuringTurn>c__AnonStorey10E {
                callback = callback,
                <>f__this = this
            };
            storeye.callback = Callbacks.AsOnGameThreadCallback<bool>(storeye.callback);
            this.FindEqualVersionMatchWithParticipant(match, pendingParticipantId, storeye.callback, new Action<GooglePlayGames.Native.PInvoke.MultiplayerParticipant, NativeTurnBasedMatch>(storeye.<>m__88));
        }

        public void RegisterMatchDelegate(MatchDelegate del)
        {
            <RegisterMatchDelegate>c__AnonStorey106 storey = new <RegisterMatchDelegate>c__AnonStorey106 {
                del = del
            };
            if (storey.del == null)
            {
                this.mMatchDelegate = null;
            }
            else
            {
                this.mMatchDelegate = Callbacks.AsOnGameThreadCallback<GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch, bool>(new Action<GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch, bool>(storey.<>m__80));
            }
        }

        public void Rematch(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback)
        {
            <Rematch>c__AnonStorey110 storey = new <Rematch>c__AnonStorey110 {
                callback = callback,
                <>f__this = this
            };
            storey.callback = Callbacks.AsOnGameThreadCallback<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch>(storey.callback);
            this.FindEqualVersionMatch(match, new Action<bool>(storey.<>m__8A), new Action<NativeTurnBasedMatch>(storey.<>m__8B));
        }

        private static Types.MatchResult ResultToMatchResult(MatchOutcome.ParticipantResult result)
        {
            switch (result)
            {
                case MatchOutcome.ParticipantResult.None:
                    return Types.MatchResult.NONE;

                case MatchOutcome.ParticipantResult.Win:
                    return Types.MatchResult.WIN;

                case MatchOutcome.ParticipantResult.Loss:
                    return Types.MatchResult.LOSS;

                case MatchOutcome.ParticipantResult.Tie:
                    return Types.MatchResult.TIE;
            }
            Logger.e("Received unknown ParticipantResult " + result);
            return Types.MatchResult.NONE;
        }

        public void TakeTurn(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, byte[] data, string pendingParticipantId, Action<bool> callback)
        {
            <TakeTurn>c__AnonStorey108 storey = new <TakeTurn>c__AnonStorey108 {
                data = data,
                callback = callback,
                <>f__this = this
            };
            Logger.describe(storey.data);
            storey.callback = Callbacks.AsOnGameThreadCallback<bool>(storey.callback);
            this.FindEqualVersionMatchWithParticipant(match, pendingParticipantId, storey.callback, new Action<GooglePlayGames.Native.PInvoke.MultiplayerParticipant, NativeTurnBasedMatch>(storey.<>m__82));
        }

        [DebuggerHidden]
        private IEnumerator WaitForLogin(Action method) => 
            new <WaitForLogin>c__Iterator1 { 
                method = method,
                <$>method = method,
                <>f__this = this
            };

        [CompilerGenerated]
        private sealed class <AcceptFromInbox>c__AnonStorey103
        {
            internal NativeTurnBasedMultiplayerClient <>f__this;
            internal Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback;

            internal void <>m__7D(TurnBasedManager.MatchInboxUIResponse callbackResult)
            {
                using (NativeTurnBasedMatch match = callbackResult.Match())
                {
                    if (match == null)
                    {
                        this.callback(false, null);
                    }
                    else
                    {
                        GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match2 = match.AsTurnBasedMatch(this.<>f__this.mNativeClient.GetUserId());
                        Logger.d("Passing converted match to user callback:" + match2);
                        this.callback(true, match2);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <AcceptInvitation>c__AnonStorey104
        {
            internal NativeTurnBasedMultiplayerClient <>f__this;
            internal Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback;
            internal string invitationId;

            internal void <>m__7E(GooglePlayGames.Native.PInvoke.MultiplayerInvitation invitation)
            {
                if (invitation == null)
                {
                    Logger.e("Could not find invitation with id " + this.invitationId);
                    this.callback(false, null);
                }
                else
                {
                    this.<>f__this.mTurnBasedManager.AcceptInvitation(invitation, this.<>f__this.BridgeMatchToUserCallback((status, match) => this.callback(status == GooglePlayGames.BasicApi.UIStatus.Valid, match)));
                }
            }

            internal void <>m__8D(GooglePlayGames.BasicApi.UIStatus status, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match)
            {
                this.callback(status == GooglePlayGames.BasicApi.UIStatus.Valid, match);
            }
        }

        [CompilerGenerated]
        private sealed class <AcknowledgeFinished>c__AnonStorey10C
        {
            internal NativeTurnBasedMultiplayerClient <>f__this;
            internal Action<bool> callback;

            internal void <>m__86(NativeTurnBasedMatch foundMatch)
            {
                this.<>f__this.mTurnBasedManager.ConfirmPendingCompletion(foundMatch, response => this.callback(response.RequestSucceeded()));
            }

            internal void <>m__90(TurnBasedManager.TurnBasedMatchResponse response)
            {
                this.callback(response.RequestSucceeded());
            }
        }

        [CompilerGenerated]
        private sealed class <BridgeMatchToUserCallback>c__AnonStorey102
        {
            internal NativeTurnBasedMultiplayerClient <>f__this;
            internal Action<GooglePlayGames.BasicApi.UIStatus, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> userCallback;

            internal void <>m__7C(TurnBasedManager.TurnBasedMatchResponse callbackResult)
            {
                using (NativeTurnBasedMatch match = callbackResult.Match())
                {
                    if (match == null)
                    {
                        GooglePlayGames.BasicApi.UIStatus internalError = GooglePlayGames.BasicApi.UIStatus.InternalError;
                        switch ((callbackResult.ResponseStatus() + 5))
                        {
                            case ~(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID_BUT_STALE | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.ERROR_VERSION_UPDATE_REQUIRED):
                                internalError = GooglePlayGames.BasicApi.UIStatus.Timeout;
                                break;

                            case GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID:
                                internalError = GooglePlayGames.BasicApi.UIStatus.VersionUpdateRequired;
                                break;

                            case GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID_BUT_STALE:
                                internalError = GooglePlayGames.BasicApi.UIStatus.NotAuthorized;
                                break;

                            case ~GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.ERROR_VERSION_UPDATE_REQUIRED:
                                internalError = GooglePlayGames.BasicApi.UIStatus.InternalError;
                                break;

                            case ~GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.ERROR_MATCH_ALREADY_REMATCHED:
                                internalError = GooglePlayGames.BasicApi.UIStatus.Valid;
                                break;

                            case ~GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.ERROR_INACTIVE_MATCH:
                                internalError = GooglePlayGames.BasicApi.UIStatus.Valid;
                                break;
                        }
                        this.userCallback(internalError, null);
                    }
                    else
                    {
                        GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match2 = match.AsTurnBasedMatch(this.<>f__this.mNativeClient.GetUserId());
                        Logger.d("Passing converted match to user callback:" + match2);
                        this.userCallback(GooglePlayGames.BasicApi.UIStatus.Valid, match2);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <Cancel>c__AnonStorey10F
        {
            internal NativeTurnBasedMultiplayerClient <>f__this;
            internal Action<bool> callback;

            internal void <>m__89(NativeTurnBasedMatch foundMatch)
            {
                this.<>f__this.mTurnBasedManager.CancelMatch(foundMatch, status => this.callback(status > ~(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID_BUT_STALE | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.ERROR_VERSION_UPDATE_REQUIRED)));
            }

            internal void <>m__93(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus status)
            {
                this.callback(status > ~(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID_BUT_STALE | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.ERROR_VERSION_UPDATE_REQUIRED));
            }
        }

        [CompilerGenerated]
        private sealed class <CreateQuickMatch>c__AnonStoreyFD
        {
            internal Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback;

            internal void <>m__77(GooglePlayGames.BasicApi.UIStatus status, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match)
            {
                this.callback(status == GooglePlayGames.BasicApi.UIStatus.Valid, match);
            }
        }

        [CompilerGenerated]
        private sealed class <CreateWithInvitationScreen>c__AnonStoreyFE
        {
            internal Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback;

            internal void <>m__78(GooglePlayGames.BasicApi.UIStatus status, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match)
            {
                this.callback(status == GooglePlayGames.BasicApi.UIStatus.Valid, match);
            }
        }

        [CompilerGenerated]
        private sealed class <CreateWithInvitationScreen>c__AnonStoreyFF
        {
            internal NativeTurnBasedMultiplayerClient <>f__this;
            internal Action<GooglePlayGames.BasicApi.UIStatus, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback;
            internal uint variant;

            internal void <>m__79(PlayerSelectUIResponse result)
            {
                if (result.Status() != GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus.VALID)
                {
                    this.callback((GooglePlayGames.BasicApi.UIStatus) result.Status(), null);
                }
                using (GooglePlayGames.Native.PInvoke.TurnBasedMatchConfigBuilder builder = GooglePlayGames.Native.PInvoke.TurnBasedMatchConfigBuilder.Create())
                {
                    builder.PopulateFromUIResponse(result).SetVariant(this.variant);
                    using (GooglePlayGames.Native.PInvoke.TurnBasedMatchConfig config = builder.Build())
                    {
                        this.<>f__this.mTurnBasedManager.CreateMatch(config, this.<>f__this.BridgeMatchToUserCallback(this.callback));
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <FindEqualVersionMatch>c__AnonStorey109
        {
            internal GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match;
            internal Action<bool> onFailure;
            internal Action<NativeTurnBasedMatch> onVersionMatch;

            internal void <>m__83(TurnBasedManager.TurnBasedMatchResponse response)
            {
                using (NativeTurnBasedMatch match = response.Match())
                {
                    if (match == null)
                    {
                        Logger.e($"Could not find match {this.match.MatchId}");
                        this.onFailure(false);
                    }
                    else if (match.Version() != this.match.Version)
                    {
                        Logger.e($"Attempted to update a stale version of the match. Expected version was {this.match.Version} but current version is {match.Version()}.");
                        this.onFailure(false);
                    }
                    else
                    {
                        this.onVersionMatch(match);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <FindEqualVersionMatchWithParticipant>c__AnonStorey10A
        {
            internal GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match;
            internal Action<bool> onFailure;
            internal Action<GooglePlayGames.Native.PInvoke.MultiplayerParticipant, NativeTurnBasedMatch> onFoundParticipantAndMatch;
            internal string participantId;

            internal void <>m__84(NativeTurnBasedMatch foundMatch)
            {
                if (this.participantId == null)
                {
                    using (GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant = GooglePlayGames.Native.PInvoke.MultiplayerParticipant.AutomatchingSentinel())
                    {
                        this.onFoundParticipantAndMatch(participant, foundMatch);
                        return;
                    }
                }
                using (GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant2 = foundMatch.ParticipantWithId(this.participantId))
                {
                    if (participant2 == null)
                    {
                        Logger.e($"Located match {this.match.MatchId} but desired participant with ID {this.participantId} could not be found");
                        this.onFailure(false);
                    }
                    else
                    {
                        this.onFoundParticipantAndMatch(participant2, foundMatch);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <FindInvitationWithId>c__AnonStorey105
        {
            internal Action<GooglePlayGames.Native.PInvoke.MultiplayerInvitation> callback;
            internal string invitationId;

            internal void <>m__7F(TurnBasedManager.TurnBasedMatchesResponse allMatches)
            {
                if (allMatches.Status() <= ~(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID_BUT_STALE | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.ERROR_VERSION_UPDATE_REQUIRED))
                {
                    this.callback(null);
                }
                else
                {
                    IEnumerator<GooglePlayGames.Native.PInvoke.MultiplayerInvitation> enumerator = allMatches.Invitations().GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            GooglePlayGames.Native.PInvoke.MultiplayerInvitation current = enumerator.Current;
                            using (current)
                            {
                                if (current.Id().Equals(this.invitationId))
                                {
                                    this.callback(current);
                                    return;
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
                    this.callback(null);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <Finish>c__AnonStorey10B
        {
            internal NativeTurnBasedMultiplayerClient <>f__this;
            internal Action<bool> callback;
            internal byte[] data;
            internal MatchOutcome outcome;

            internal void <>m__85(NativeTurnBasedMatch foundMatch)
            {
                GooglePlayGames.Native.PInvoke.ParticipantResults results = foundMatch.Results();
                foreach (string str in this.outcome.ParticipantIds)
                {
                    Types.MatchResult result = NativeTurnBasedMultiplayerClient.ResultToMatchResult(this.outcome.GetResultFor(str));
                    uint placementFor = this.outcome.GetPlacementFor(str);
                    if (results.HasResultsForParticipant(str))
                    {
                        Types.MatchResult result2 = results.ResultsForParticipant(str);
                        uint num2 = results.PlacingForParticipant(str);
                        if ((result == result2) && (placementFor == num2))
                        {
                            continue;
                        }
                        Logger.e($"Attempted to override existing results for participant {str}: Placing {num2}, Result {result2}");
                        this.callback(false);
                        return;
                    }
                    GooglePlayGames.Native.PInvoke.ParticipantResults results2 = results;
                    results = results2.WithResult(str, placementFor, result);
                    results2.Dispose();
                }
                this.<>f__this.mTurnBasedManager.FinishMatchDuringMyTurn(foundMatch, this.data, results, response => this.callback(response.RequestSucceeded()));
            }

            internal void <>m__8F(TurnBasedManager.TurnBasedMatchResponse response)
            {
                this.callback(response.RequestSucceeded());
            }
        }

        [CompilerGenerated]
        private sealed class <GetAllInvitations>c__AnonStorey100
        {
            internal Action<Invitation[]> callback;

            internal void <>m__7A(TurnBasedManager.TurnBasedMatchesResponse allMatches)
            {
                Invitation[] invitationArray = new Invitation[allMatches.InvitationCount()];
                int num = 0;
                IEnumerator<GooglePlayGames.Native.PInvoke.MultiplayerInvitation> enumerator = allMatches.Invitations().GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        invitationArray[num++] = enumerator.Current.AsInvitation();
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
                this.callback(invitationArray);
            }
        }

        [CompilerGenerated]
        private sealed class <GetAllMatches>c__AnonStorey101
        {
            internal NativeTurnBasedMultiplayerClient <>f__this;
            internal Action<GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch[]> callback;

            internal void <>m__7B(TurnBasedManager.TurnBasedMatchesResponse allMatches)
            {
                int num = (allMatches.MyTurnMatchesCount() + allMatches.TheirTurnMatchesCount()) + allMatches.CompletedMatchesCount();
                GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch[] matchArray = new GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch[num];
                int num2 = 0;
                IEnumerator<NativeTurnBasedMatch> enumerator = allMatches.MyTurnMatches().GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        matchArray[num2++] = enumerator.Current.AsTurnBasedMatch(this.<>f__this.mNativeClient.GetUserId());
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
                IEnumerator<NativeTurnBasedMatch> enumerator2 = allMatches.TheirTurnMatches().GetEnumerator();
                try
                {
                    while (enumerator2.MoveNext())
                    {
                        matchArray[num2++] = enumerator2.Current.AsTurnBasedMatch(this.<>f__this.mNativeClient.GetUserId());
                    }
                }
                finally
                {
                    if (enumerator2 == null)
                    {
                    }
                    enumerator2.Dispose();
                }
                IEnumerator<NativeTurnBasedMatch> enumerator3 = allMatches.CompletedMatches().GetEnumerator();
                try
                {
                    while (enumerator3.MoveNext())
                    {
                        matchArray[num2++] = enumerator3.Current.AsTurnBasedMatch(this.<>f__this.mNativeClient.GetUserId());
                    }
                }
                finally
                {
                    if (enumerator3 == null)
                    {
                    }
                    enumerator3.Dispose();
                }
                this.callback(matchArray);
            }
        }

        [CompilerGenerated]
        private sealed class <HandleMatchEvent>c__AnonStorey107
        {
            internal NativeTurnBasedMultiplayerClient <>f__this;
            internal Action<GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch, bool> currentDelegate;
            internal NativeTurnBasedMatch match;
            internal bool shouldAutolaunch;

            internal void <>m__81()
            {
                this.currentDelegate(this.match.AsTurnBasedMatch(this.<>f__this.mNativeClient.GetUserId()), this.shouldAutolaunch);
                this.match.ForgetMe();
            }
        }

        [CompilerGenerated]
        private sealed class <Leave>c__AnonStorey10D
        {
            internal NativeTurnBasedMultiplayerClient <>f__this;
            internal Action<bool> callback;

            internal void <>m__87(NativeTurnBasedMatch foundMatch)
            {
                this.<>f__this.mTurnBasedManager.LeaveMatchDuringTheirTurn(foundMatch, status => this.callback(status > ~(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID_BUT_STALE | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.ERROR_VERSION_UPDATE_REQUIRED)));
            }

            internal void <>m__91(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus status)
            {
                this.callback(status > ~(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID_BUT_STALE | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.ERROR_VERSION_UPDATE_REQUIRED));
            }
        }

        [CompilerGenerated]
        private sealed class <LeaveDuringTurn>c__AnonStorey10E
        {
            internal NativeTurnBasedMultiplayerClient <>f__this;
            internal Action<bool> callback;

            internal void <>m__88(GooglePlayGames.Native.PInvoke.MultiplayerParticipant pendingParticipant, NativeTurnBasedMatch foundMatch)
            {
                this.<>f__this.mTurnBasedManager.LeaveDuringMyTurn(foundMatch, pendingParticipant, status => this.callback(status > ~(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID_BUT_STALE | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.ERROR_VERSION_UPDATE_REQUIRED)));
            }

            internal void <>m__92(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus status)
            {
                this.callback(status > ~(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID_BUT_STALE | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.ERROR_VERSION_UPDATE_REQUIRED));
            }
        }

        [CompilerGenerated]
        private sealed class <RegisterMatchDelegate>c__AnonStorey106
        {
            internal MatchDelegate del;

            internal void <>m__80(GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch match, bool autoLaunch)
            {
                this.del(match, autoLaunch);
            }
        }

        [CompilerGenerated]
        private sealed class <Rematch>c__AnonStorey110
        {
            internal NativeTurnBasedMultiplayerClient <>f__this;
            internal Action<bool, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch> callback;

            internal void <>m__8A(bool failed)
            {
                this.callback(false, null);
            }

            internal void <>m__8B(NativeTurnBasedMatch foundMatch)
            {
                this.<>f__this.mTurnBasedManager.Rematch(foundMatch, this.<>f__this.BridgeMatchToUserCallback((status, m) => this.callback(status == GooglePlayGames.BasicApi.UIStatus.Valid, m)));
            }

            internal void <>m__94(GooglePlayGames.BasicApi.UIStatus status, GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch m)
            {
                this.callback(status == GooglePlayGames.BasicApi.UIStatus.Valid, m);
            }
        }

        [CompilerGenerated]
        private sealed class <TakeTurn>c__AnonStorey108
        {
            internal NativeTurnBasedMultiplayerClient <>f__this;
            internal Action<bool> callback;
            internal byte[] data;

            internal void <>m__82(GooglePlayGames.Native.PInvoke.MultiplayerParticipant pendingParticipant, NativeTurnBasedMatch foundMatch)
            {
                this.<>f__this.mTurnBasedManager.TakeTurn(foundMatch, this.data, pendingParticipant, delegate (TurnBasedManager.TurnBasedMatchResponse result) {
                    if (result.RequestSucceeded())
                    {
                        this.callback(true);
                    }
                    else
                    {
                        Logger.d("Taking turn failed: " + result.ResponseStatus());
                        this.callback(false);
                    }
                });
            }

            internal void <>m__8E(TurnBasedManager.TurnBasedMatchResponse result)
            {
                if (result.RequestSucceeded())
                {
                    this.callback(true);
                }
                else
                {
                    Logger.d("Taking turn failed: " + result.ResponseStatus());
                    this.callback(false);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WaitForLogin>c__Iterator1 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action <$>method;
            internal NativeTurnBasedMultiplayerClient <>f__this;
            internal Action method;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        if (!string.IsNullOrEmpty(this.<>f__this.mNativeClient.GetUserId()))
                        {
                            break;
                        }
                        this.$current = null;
                        this.$PC = 1;
                        return true;

                    case 1:
                        break;

                    default:
                        goto Label_0060;
                }
                this.method();
                this.$PC = -1;
            Label_0060:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

