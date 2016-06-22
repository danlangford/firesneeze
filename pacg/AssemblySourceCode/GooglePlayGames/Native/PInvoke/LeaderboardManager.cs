namespace GooglePlayGames.Native.PInvoke
{
    using AOT;
    using GooglePlayGames.BasicApi;
    using GooglePlayGames.Native.Cwrapper;
    using GooglePlayGames.OurUtils;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class LeaderboardManager
    {
        private readonly GooglePlayGames.Native.PInvoke.GameServices mServices;

        internal LeaderboardManager(GooglePlayGames.Native.PInvoke.GameServices services)
        {
            this.mServices = Misc.CheckNotNull<GooglePlayGames.Native.PInvoke.GameServices>(services);
        }

        internal void HandleFetch(ScorePageToken token, GooglePlayGames.Native.PInvoke.FetchResponse response, string selfPlayerId, int maxResults, Action<LeaderboardScoreData> callback)
        {
            <HandleFetch>c__AnonStorey118 storey = new <HandleFetch>c__AnonStorey118 {
                selfPlayerId = selfPlayerId,
                maxResults = maxResults,
                token = token,
                callback = callback,
                <>f__this = this
            };
            storey.data = new LeaderboardScoreData(storey.token.LeaderboardId, (GooglePlayGames.BasicApi.ResponseStatus) response.GetStatus());
            if ((response.GetStatus() != GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.VALID) && (response.GetStatus() != GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.VALID_BUT_STALE))
            {
                Logger.w("Error returned from fetch: " + response.GetStatus());
                storey.callback(storey.data);
            }
            else
            {
                storey.data.Title = response.Leaderboard().Title();
                storey.data.Id = storey.token.LeaderboardId;
                GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_FetchScoreSummary(this.mServices.AsHandle(), Types.DataSource.CACHE_OR_NETWORK, storey.token.LeaderboardId, (Types.LeaderboardTimeSpan) storey.token.TimeSpan, (Types.LeaderboardCollection) storey.token.Collection, new GooglePlayGames.Native.Cwrapper.LeaderboardManager.FetchScoreSummaryCallback(GooglePlayGames.Native.PInvoke.LeaderboardManager.InternalFetchSummaryCallback), Callbacks.ToIntPtr<FetchScoreSummaryResponse>(new Action<FetchScoreSummaryResponse>(storey.<>m__A6), new Func<IntPtr, FetchScoreSummaryResponse>(FetchScoreSummaryResponse.FromPointer)));
            }
        }

        internal void HandleFetchScorePage(LeaderboardScoreData data, ScorePageToken token, FetchScorePageResponse rsp, Action<LeaderboardScoreData> callback)
        {
            data.Status = (GooglePlayGames.BasicApi.ResponseStatus) rsp.GetStatus();
            if ((rsp.GetStatus() != GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.VALID) && (rsp.GetStatus() != GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.VALID_BUT_STALE))
            {
                callback(data);
            }
            NativeScorePage scorePage = rsp.GetScorePage();
            if (!scorePage.Valid())
            {
                callback(data);
            }
            if (scorePage.HasNextScorePage())
            {
                data.NextPageToken = new ScorePageToken(scorePage.GetNextScorePageToken(), token.LeaderboardId, token.Collection, token.TimeSpan);
            }
            if (scorePage.HasPrevScorePage())
            {
                data.PrevPageToken = new ScorePageToken(scorePage.GetPreviousScorePageToken(), token.LeaderboardId, token.Collection, token.TimeSpan);
            }
            IEnumerator<NativeScoreEntry> enumerator = scorePage.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    NativeScoreEntry current = enumerator.Current;
                    data.AddScore(current.AsScore(data.Id));
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            callback(data);
        }

        internal void HandleFetchScoreSummary(LeaderboardScoreData data, FetchScoreSummaryResponse response, string selfPlayerId, int maxResults, ScorePageToken token, Action<LeaderboardScoreData> callback)
        {
            if ((response.GetStatus() != GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.VALID) && (response.GetStatus() != GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.VALID_BUT_STALE))
            {
                Logger.w("Error returned from fetchScoreSummary: " + response);
                data.Status = (GooglePlayGames.BasicApi.ResponseStatus) response.GetStatus();
                callback(data);
            }
            else
            {
                NativeScoreSummary scoreSummary = response.GetScoreSummary();
                data.ApproximateCount = scoreSummary.ApproximateResults();
                data.PlayerScore = scoreSummary.LocalUserScore().AsScore(data.Id, selfPlayerId);
                if (maxResults <= 0)
                {
                    callback(data);
                }
                else
                {
                    this.LoadScorePage(data, maxResults, token, callback);
                }
            }
        }

        [MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.LeaderboardManager.FetchCallback))]
        private static void InternalFetchCallback(IntPtr response, IntPtr data)
        {
            Callbacks.PerformInternalCallback("LeaderboardManager#InternalFetchCallback", Callbacks.Type.Temporary, response, data);
        }

        [MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.LeaderboardManager.FetchScorePageCallback))]
        private static void InternalFetchScorePage(IntPtr response, IntPtr data)
        {
            Callbacks.PerformInternalCallback("LeaderboardManager#InternalFetchScorePage", Callbacks.Type.Temporary, response, data);
        }

        [MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.LeaderboardManager.FetchScoreSummaryCallback))]
        private static void InternalFetchSummaryCallback(IntPtr response, IntPtr data)
        {
            Callbacks.PerformInternalCallback("LeaderboardManager#InternalFetchSummaryCallback", Callbacks.Type.Temporary, response, data);
        }

        public void LoadLeaderboardData(string leaderboardId, GooglePlayGames.BasicApi.LeaderboardStart start, int rowCount, GooglePlayGames.BasicApi.LeaderboardCollection collection, GooglePlayGames.BasicApi.LeaderboardTimeSpan timeSpan, string playerId, Action<LeaderboardScoreData> callback)
        {
            <LoadLeaderboardData>c__AnonStorey117 storey = new <LoadLeaderboardData>c__AnonStorey117 {
                playerId = playerId,
                rowCount = rowCount,
                callback = callback,
                <>f__this = this
            };
            NativeScorePageToken internalObject = new NativeScorePageToken(GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_ScorePageToken(this.mServices.AsHandle(), leaderboardId, (Types.LeaderboardStart) start, (Types.LeaderboardTimeSpan) timeSpan, (Types.LeaderboardCollection) collection));
            storey.token = new ScorePageToken(internalObject, leaderboardId, collection, timeSpan);
            GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_Fetch(this.mServices.AsHandle(), Types.DataSource.CACHE_OR_NETWORK, leaderboardId, new GooglePlayGames.Native.Cwrapper.LeaderboardManager.FetchCallback(GooglePlayGames.Native.PInvoke.LeaderboardManager.InternalFetchCallback), Callbacks.ToIntPtr<GooglePlayGames.Native.PInvoke.FetchResponse>(new Action<GooglePlayGames.Native.PInvoke.FetchResponse>(storey.<>m__A5), new Func<IntPtr, GooglePlayGames.Native.PInvoke.FetchResponse>(GooglePlayGames.Native.PInvoke.FetchResponse.FromPointer)));
        }

        public void LoadScorePage(LeaderboardScoreData data, int maxResults, ScorePageToken token, Action<LeaderboardScoreData> callback)
        {
            <LoadScorePage>c__AnonStorey119 storey = new <LoadScorePage>c__AnonStorey119 {
                data = data,
                token = token,
                callback = callback,
                <>f__this = this
            };
            if (storey.data == null)
            {
                storey.data = new LeaderboardScoreData(storey.token.LeaderboardId);
            }
            GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_FetchScorePage(this.mServices.AsHandle(), Types.DataSource.CACHE_OR_NETWORK, ((NativeScorePageToken) storey.token.InternalObject).AsPointer(), (uint) maxResults, new GooglePlayGames.Native.Cwrapper.LeaderboardManager.FetchScorePageCallback(GooglePlayGames.Native.PInvoke.LeaderboardManager.InternalFetchScorePage), Callbacks.ToIntPtr<FetchScorePageResponse>(new Action<FetchScorePageResponse>(storey.<>m__A7), new Func<IntPtr, FetchScorePageResponse>(FetchScorePageResponse.FromPointer)));
        }

        internal void ShowAllUI(Action<GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus> callback)
        {
            Misc.CheckNotNull<Action<GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus>>(callback);
            GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_ShowAllUI(this.mServices.AsHandle(), new GooglePlayGames.Native.Cwrapper.LeaderboardManager.ShowAllUICallback(Callbacks.InternalShowUICallback), Callbacks.ToIntPtr(callback));
        }

        internal void ShowUI(string leaderboardId, GooglePlayGames.BasicApi.LeaderboardTimeSpan span, Action<GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus> callback)
        {
            Misc.CheckNotNull<Action<GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus>>(callback);
            GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_ShowUI(this.mServices.AsHandle(), leaderboardId, (Types.LeaderboardTimeSpan) span, new GooglePlayGames.Native.Cwrapper.LeaderboardManager.ShowUICallback(Callbacks.InternalShowUICallback), Callbacks.ToIntPtr(callback));
        }

        internal void SubmitScore(string leaderboardId, long score, string metadata)
        {
            Misc.CheckNotNull<string>(leaderboardId, "leaderboardId");
            Logger.d(string.Concat(new object[] { "Native Submitting score: ", score, " for lb ", leaderboardId, " with metadata: ", metadata }));
            if (metadata == null)
            {
            }
            GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_SubmitScore(this.mServices.AsHandle(), leaderboardId, (ulong) score, string.Empty);
        }

        internal int LeaderboardMaxResults =>
            0x19;

        [CompilerGenerated]
        private sealed class <HandleFetch>c__AnonStorey118
        {
            internal GooglePlayGames.Native.PInvoke.LeaderboardManager <>f__this;
            internal Action<LeaderboardScoreData> callback;
            internal LeaderboardScoreData data;
            internal int maxResults;
            internal string selfPlayerId;
            internal ScorePageToken token;

            internal void <>m__A6(FetchScoreSummaryResponse rsp)
            {
                this.<>f__this.HandleFetchScoreSummary(this.data, rsp, this.selfPlayerId, this.maxResults, this.token, this.callback);
            }
        }

        [CompilerGenerated]
        private sealed class <LoadLeaderboardData>c__AnonStorey117
        {
            internal GooglePlayGames.Native.PInvoke.LeaderboardManager <>f__this;
            internal Action<LeaderboardScoreData> callback;
            internal string playerId;
            internal int rowCount;
            internal ScorePageToken token;

            internal void <>m__A5(GooglePlayGames.Native.PInvoke.FetchResponse rsp)
            {
                this.<>f__this.HandleFetch(this.token, rsp, this.playerId, this.rowCount, this.callback);
            }
        }

        [CompilerGenerated]
        private sealed class <LoadScorePage>c__AnonStorey119
        {
            internal GooglePlayGames.Native.PInvoke.LeaderboardManager <>f__this;
            internal Action<LeaderboardScoreData> callback;
            internal LeaderboardScoreData data;
            internal ScorePageToken token;

            internal void <>m__A7(FetchScorePageResponse rsp)
            {
                this.<>f__this.HandleFetchScorePage(this.data, this.token, rsp, this.callback);
            }
        }
    }
}

