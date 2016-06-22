﻿namespace GooglePlayGames.Native.PInvoke
{
    using AOT;
    using GooglePlayGames.Native.Cwrapper;
    using GooglePlayGames.OurUtils;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class TurnBasedManager
    {
        private readonly GooglePlayGames.Native.PInvoke.GameServices mGameServices;

        internal TurnBasedManager(GooglePlayGames.Native.PInvoke.GameServices services)
        {
            this.mGameServices = services;
        }

        internal void AcceptInvitation(GooglePlayGames.Native.PInvoke.MultiplayerInvitation invitation, Action<TurnBasedMatchResponse> callback)
        {
            Logger.d("Accepting invitation: " + invitation.AsPointer().ToInt64());
            TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_AcceptInvitation(this.mGameServices.AsHandle(), invitation.AsPointer(), new TurnBasedMultiplayerManager.TurnBasedMatchCallback(TurnBasedManager.InternalTurnBasedMatchCallback), ToCallbackPointer(callback));
        }

        internal void CancelMatch(NativeTurnBasedMatch match, Action<GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus> callback)
        {
            TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_CancelMatch(this.mGameServices.AsHandle(), match.AsPointer(), new TurnBasedMultiplayerManager.MultiplayerStatusCallback(TurnBasedManager.InternalMultiplayerStatusCallback), Callbacks.ToIntPtr(callback));
        }

        internal void ConfirmPendingCompletion(NativeTurnBasedMatch match, Action<TurnBasedMatchResponse> callback)
        {
            TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_ConfirmPendingCompletion(this.mGameServices.AsHandle(), match.AsPointer(), new TurnBasedMultiplayerManager.TurnBasedMatchCallback(TurnBasedManager.InternalTurnBasedMatchCallback), ToCallbackPointer(callback));
        }

        internal void CreateMatch(GooglePlayGames.Native.PInvoke.TurnBasedMatchConfig config, Action<TurnBasedMatchResponse> callback)
        {
            TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_CreateTurnBasedMatch(this.mGameServices.AsHandle(), config.AsPointer(), new TurnBasedMultiplayerManager.TurnBasedMatchCallback(TurnBasedManager.InternalTurnBasedMatchCallback), ToCallbackPointer(callback));
        }

        internal void DeclineInvitation(GooglePlayGames.Native.PInvoke.MultiplayerInvitation invitation)
        {
            TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_DeclineInvitation(this.mGameServices.AsHandle(), invitation.AsPointer());
        }

        internal void FinishMatchDuringMyTurn(NativeTurnBasedMatch match, byte[] data, GooglePlayGames.Native.PInvoke.ParticipantResults results, Action<TurnBasedMatchResponse> callback)
        {
            TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_FinishMatchDuringMyTurn(this.mGameServices.AsHandle(), match.AsPointer(), data, new UIntPtr((uint) data.Length), results.AsPointer(), new TurnBasedMultiplayerManager.TurnBasedMatchCallback(TurnBasedManager.InternalTurnBasedMatchCallback), ToCallbackPointer(callback));
        }

        internal void GetAllTurnbasedMatches(Action<TurnBasedMatchesResponse> callback)
        {
            TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_FetchMatches(this.mGameServices.AsHandle(), new TurnBasedMultiplayerManager.TurnBasedMatchesCallback(TurnBasedManager.InternalTurnBasedMatchesCallback), Callbacks.ToIntPtr<TurnBasedMatchesResponse>(callback, new Func<IntPtr, TurnBasedMatchesResponse>(TurnBasedMatchesResponse.FromPointer)));
        }

        internal void GetMatch(string matchId, Action<TurnBasedMatchResponse> callback)
        {
            TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_FetchMatch(this.mGameServices.AsHandle(), matchId, new TurnBasedMultiplayerManager.TurnBasedMatchCallback(TurnBasedManager.InternalTurnBasedMatchCallback), ToCallbackPointer(callback));
        }

        [MonoPInvokeCallback(typeof(TurnBasedMultiplayerManager.MatchInboxUICallback))]
        internal static void InternalMatchInboxUICallback(IntPtr response, IntPtr data)
        {
            Callbacks.PerformInternalCallback("TurnBasedManager#MatchInboxUICallback", Callbacks.Type.Temporary, response, data);
        }

        [MonoPInvokeCallback(typeof(TurnBasedMultiplayerManager.MultiplayerStatusCallback))]
        internal static void InternalMultiplayerStatusCallback(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus status, IntPtr data)
        {
            Logger.d("InternalMultiplayerStatusCallback: " + status);
            Action<GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus> action = Callbacks.IntPtrToTempCallback<Action<GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus>>(data);
            try
            {
                action(status);
            }
            catch (Exception exception)
            {
                Logger.e("Error encountered executing InternalMultiplayerStatusCallback. Smothering to avoid passing exception into Native: " + exception);
            }
        }

        [MonoPInvokeCallback(typeof(TurnBasedMultiplayerManager.PlayerSelectUICallback))]
        internal static void InternalPlayerSelectUIcallback(IntPtr response, IntPtr data)
        {
            Callbacks.PerformInternalCallback("TurnBasedManager#PlayerSelectUICallback", Callbacks.Type.Temporary, response, data);
        }

        [MonoPInvokeCallback(typeof(TurnBasedMultiplayerManager.TurnBasedMatchCallback))]
        internal static void InternalTurnBasedMatchCallback(IntPtr response, IntPtr data)
        {
            Callbacks.PerformInternalCallback("TurnBasedManager#InternalTurnBasedMatchCallback", Callbacks.Type.Temporary, response, data);
        }

        [MonoPInvokeCallback(typeof(TurnBasedMultiplayerManager.TurnBasedMatchesCallback))]
        internal static void InternalTurnBasedMatchesCallback(IntPtr response, IntPtr data)
        {
            Callbacks.PerformInternalCallback("TurnBasedManager#TurnBasedMatchesCallback", Callbacks.Type.Temporary, response, data);
        }

        internal void LeaveDuringMyTurn(NativeTurnBasedMatch match, GooglePlayGames.Native.PInvoke.MultiplayerParticipant nextParticipant, Action<GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus> callback)
        {
            TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_LeaveMatchDuringMyTurn(this.mGameServices.AsHandle(), match.AsPointer(), nextParticipant.AsPointer(), new TurnBasedMultiplayerManager.MultiplayerStatusCallback(TurnBasedManager.InternalMultiplayerStatusCallback), Callbacks.ToIntPtr(callback));
        }

        internal void LeaveMatchDuringTheirTurn(NativeTurnBasedMatch match, Action<GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus> callback)
        {
            TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_LeaveMatchDuringTheirTurn(this.mGameServices.AsHandle(), match.AsPointer(), new TurnBasedMultiplayerManager.MultiplayerStatusCallback(TurnBasedManager.InternalMultiplayerStatusCallback), Callbacks.ToIntPtr(callback));
        }

        internal void Rematch(NativeTurnBasedMatch match, Action<TurnBasedMatchResponse> callback)
        {
            TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_Rematch(this.mGameServices.AsHandle(), match.AsPointer(), new TurnBasedMultiplayerManager.TurnBasedMatchCallback(TurnBasedManager.InternalTurnBasedMatchCallback), ToCallbackPointer(callback));
        }

        internal void ShowInboxUI(Action<MatchInboxUIResponse> callback)
        {
            TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_ShowMatchInboxUI(this.mGameServices.AsHandle(), new TurnBasedMultiplayerManager.MatchInboxUICallback(TurnBasedManager.InternalMatchInboxUICallback), Callbacks.ToIntPtr<MatchInboxUIResponse>(callback, new Func<IntPtr, MatchInboxUIResponse>(MatchInboxUIResponse.FromPointer)));
        }

        internal void ShowPlayerSelectUI(uint minimumPlayers, uint maxiumPlayers, bool allowAutomatching, Action<PlayerSelectUIResponse> callback)
        {
            TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_ShowPlayerSelectUI(this.mGameServices.AsHandle(), minimumPlayers, maxiumPlayers, allowAutomatching, new TurnBasedMultiplayerManager.PlayerSelectUICallback(TurnBasedManager.InternalPlayerSelectUIcallback), Callbacks.ToIntPtr<PlayerSelectUIResponse>(callback, new Func<IntPtr, PlayerSelectUIResponse>(PlayerSelectUIResponse.FromPointer)));
        }

        internal void TakeTurn(NativeTurnBasedMatch match, byte[] data, GooglePlayGames.Native.PInvoke.MultiplayerParticipant nextParticipant, Action<TurnBasedMatchResponse> callback)
        {
            TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TakeMyTurn(this.mGameServices.AsHandle(), match.AsPointer(), data, new UIntPtr((uint) data.Length), match.Results().AsPointer(), nextParticipant.AsPointer(), new TurnBasedMultiplayerManager.TurnBasedMatchCallback(TurnBasedManager.InternalTurnBasedMatchCallback), ToCallbackPointer(callback));
        }

        private static IntPtr ToCallbackPointer(Action<TurnBasedMatchResponse> callback) => 
            Callbacks.ToIntPtr<TurnBasedMatchResponse>(callback, new Func<IntPtr, TurnBasedMatchResponse>(TurnBasedMatchResponse.FromPointer));

        internal class MatchInboxUIResponse : BaseReferenceHolder
        {
            internal MatchInboxUIResponse(IntPtr selfPointer) : base(selfPointer)
            {
            }

            protected override void CallDispose(HandleRef selfPointer)
            {
                TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_MatchInboxUIResponse_Dispose(selfPointer);
            }

            internal static TurnBasedManager.MatchInboxUIResponse FromPointer(IntPtr pointer)
            {
                if (pointer.Equals(IntPtr.Zero))
                {
                    return null;
                }
                return new TurnBasedManager.MatchInboxUIResponse(pointer);
            }

            internal NativeTurnBasedMatch Match()
            {
                if (this.UiStatus() != GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus.VALID)
                {
                    return null;
                }
                return new NativeTurnBasedMatch(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_MatchInboxUIResponse_GetMatch(base.SelfPtr()));
            }

            internal GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus UiStatus() => 
                TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_MatchInboxUIResponse_GetStatus(base.SelfPtr());
        }

        internal delegate void TurnBasedMatchCallback(TurnBasedManager.TurnBasedMatchResponse response);

        internal class TurnBasedMatchesResponse : BaseReferenceHolder
        {
            internal TurnBasedMatchesResponse(IntPtr selfPointer) : base(selfPointer)
            {
            }

            protected override void CallDispose(HandleRef selfPointer)
            {
                TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_Dispose(base.SelfPtr());
            }

            internal IEnumerable<NativeTurnBasedMatch> CompletedMatches() => 
                PInvokeUtilities.ToEnumerable<NativeTurnBasedMatch>(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetCompletedMatches_Length(base.SelfPtr()), index => new NativeTurnBasedMatch(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetCompletedMatches_GetElement(base.SelfPtr(), index)));

            internal int CompletedMatchesCount() => 
                ((int) TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetCompletedMatches_Length(base.SelfPtr()).ToUInt32());

            internal static TurnBasedManager.TurnBasedMatchesResponse FromPointer(IntPtr pointer)
            {
                if (PInvokeUtilities.IsNull(pointer))
                {
                    return null;
                }
                return new TurnBasedManager.TurnBasedMatchesResponse(pointer);
            }

            internal int InvitationCount() => 
                ((int) TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetInvitations_Length(base.SelfPtr()).ToUInt32());

            internal IEnumerable<GooglePlayGames.Native.PInvoke.MultiplayerInvitation> Invitations() => 
                PInvokeUtilities.ToEnumerable<GooglePlayGames.Native.PInvoke.MultiplayerInvitation>(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetInvitations_Length(base.SelfPtr()), index => new GooglePlayGames.Native.PInvoke.MultiplayerInvitation(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetInvitations_GetElement(base.SelfPtr(), index)));

            internal IEnumerable<NativeTurnBasedMatch> MyTurnMatches() => 
                PInvokeUtilities.ToEnumerable<NativeTurnBasedMatch>(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetMyTurnMatches_Length(base.SelfPtr()), index => new NativeTurnBasedMatch(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetMyTurnMatches_GetElement(base.SelfPtr(), index)));

            internal int MyTurnMatchesCount() => 
                ((int) TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetMyTurnMatches_Length(base.SelfPtr()).ToUInt32());

            internal GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus Status() => 
                TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetStatus(base.SelfPtr());

            internal IEnumerable<NativeTurnBasedMatch> TheirTurnMatches() => 
                PInvokeUtilities.ToEnumerable<NativeTurnBasedMatch>(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetTheirTurnMatches_Length(base.SelfPtr()), index => new NativeTurnBasedMatch(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetTheirTurnMatches_GetElement(base.SelfPtr(), index)));

            internal int TheirTurnMatchesCount() => 
                ((int) TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchesResponse_GetTheirTurnMatches_Length(base.SelfPtr()).ToUInt32());
        }

        internal class TurnBasedMatchResponse : BaseReferenceHolder
        {
            internal TurnBasedMatchResponse(IntPtr selfPointer) : base(selfPointer)
            {
            }

            protected override void CallDispose(HandleRef selfPointer)
            {
                TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchResponse_Dispose(selfPointer);
            }

            internal static TurnBasedManager.TurnBasedMatchResponse FromPointer(IntPtr pointer)
            {
                if (pointer.Equals(IntPtr.Zero))
                {
                    return null;
                }
                return new TurnBasedManager.TurnBasedMatchResponse(pointer);
            }

            internal NativeTurnBasedMatch Match()
            {
                if (!this.RequestSucceeded())
                {
                    return null;
                }
                return new NativeTurnBasedMatch(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchResponse_GetMatch(base.SelfPtr()));
            }

            internal bool RequestSucceeded() => 
                (this.ResponseStatus() > ~(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID_BUT_STALE | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.VALID | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus.ERROR_VERSION_UPDATE_REQUIRED));

            internal GooglePlayGames.Native.Cwrapper.CommonErrorStatus.MultiplayerStatus ResponseStatus() => 
                TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_TurnBasedMatchResponse_GetStatus(base.SelfPtr());
        }
    }
}

