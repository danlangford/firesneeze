namespace GooglePlayGames.Native
{
    using GooglePlayGames;
    using GooglePlayGames.BasicApi;
    using GooglePlayGames.BasicApi.Events;
    using GooglePlayGames.BasicApi.Multiplayer;
    using GooglePlayGames.BasicApi.Quests;
    using GooglePlayGames.BasicApi.SavedGame;
    using GooglePlayGames.Native.Cwrapper;
    using GooglePlayGames.Native.PInvoke;
    using GooglePlayGames.OurUtils;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.SocialPlatforms;

    public class NativeClient : IPlayGamesClient
    {
        [CompilerGenerated]
        private static Predicate<GooglePlayGames.BasicApi.Achievement> <>f__am$cache1A;
        [CompilerGenerated]
        private static Predicate<GooglePlayGames.BasicApi.Achievement> <>f__am$cache1B;
        private readonly object AuthStateLock = new object();
        private readonly IClientImpl clientImpl;
        private volatile bool friendsLoading;
        private readonly object GameServicesLock = new object();
        private volatile Dictionary<string, GooglePlayGames.BasicApi.Achievement> mAchievements;
        private volatile uint mAuthGeneration;
        private volatile AuthState mAuthState;
        private readonly PlayGamesClientConfiguration mConfiguration;
        private volatile IEventsClient mEventsClient;
        private volatile List<GooglePlayGames.BasicApi.Multiplayer.Player> mFriends;
        private volatile Action<Invitation, bool> mInvitationDelegate;
        private volatile Action<bool> mPendingAuthCallbacks;
        private volatile IQuestsClient mQuestsClient;
        private volatile NativeRealtimeMultiplayerClient mRealTimeClient;
        private volatile ISavedGameClient mSavedGameClient;
        private GooglePlayGames.Native.PInvoke.GameServices mServices;
        private volatile Action<bool> mSilentAuthCallbacks;
        private volatile bool mSilentAuthFailed;
        private volatile TokenClient mTokenClient;
        private volatile NativeTurnBasedMultiplayerClient mTurnBasedClient;
        private volatile GooglePlayGames.BasicApi.Multiplayer.Player mUser;
        private int needGPlusWarningCount;
        private int needGPlusWarningFreq = 0x186a0;
        private int noWebClientIdWarningCount;
        private string rationale;
        private int webclientWarningFreq = 0x186a0;

        internal NativeClient(PlayGamesClientConfiguration configuration, IClientImpl clientImpl)
        {
            PlayGamesHelperObject.CreateObject();
            this.mConfiguration = Misc.CheckNotNull<PlayGamesClientConfiguration>(configuration);
            this.clientImpl = clientImpl;
            this.rationale = configuration.PermissionRationale;
        }

        [CompilerGenerated]
        private static void <AsOnGameThreadCallback`1>m__1B<T>(T)
        {
        }

        private static Action<T> AsOnGameThreadCallback<T>(Action<T> callback)
        {
            <AsOnGameThreadCallback>c__AnonStoreyBC<T> ybc = new <AsOnGameThreadCallback>c__AnonStoreyBC<T> {
                callback = callback
            };
            if (ybc.callback == null)
            {
                return new Action<T>(NativeClient.<AsOnGameThreadCallback`1>m__1B<T>);
            }
            return new Action<T>(ybc.<>m__1C);
        }

        public void Authenticate(Action<bool> callback, bool silent)
        {
            object authStateLock = this.AuthStateLock;
            lock (authStateLock)
            {
                if (this.mAuthState == 1)
                {
                    InvokeCallbackOnGameThread<bool>(callback, true);
                    return;
                }
                if (this.mSilentAuthFailed && silent)
                {
                    InvokeCallbackOnGameThread<bool>(callback, false);
                    return;
                }
                if (callback != null)
                {
                    if (silent)
                    {
                        this.mSilentAuthCallbacks = (Action<bool>) Delegate.Combine(this.mSilentAuthCallbacks, callback);
                    }
                    else
                    {
                        this.mPendingAuthCallbacks = (Action<bool>) Delegate.Combine(this.mPendingAuthCallbacks, callback);
                    }
                }
            }
            this.InitializeGameServices();
            this.friendsLoading = false;
            if (!silent)
            {
                this.GameServices().StartAuthorizationUI();
            }
        }

        private GooglePlayGames.Native.PInvoke.GameServices GameServices()
        {
            object gameServicesLock = this.GameServicesLock;
            lock (gameServicesLock)
            {
                return this.mServices;
            }
        }

        [Obsolete("Use GetServerAuthCode() then exchange it for a token")]
        public string GetAccessToken()
        {
            if (!this.IsAuthenticated())
            {
                Debug.Log("Cannot get API client - not authenticated");
                return null;
            }
            if (!GameInfo.WebClientIdInitialized())
            {
                if ((this.noWebClientIdWarningCount++ % this.webclientWarningFreq) == 0)
                {
                    Debug.LogError("Web client ID has not been set, cannot request access token.");
                    this.noWebClientIdWarningCount = (this.noWebClientIdWarningCount / this.webclientWarningFreq) + 1;
                }
                return null;
            }
            this.mTokenClient.SetRationale(this.rationale);
            return this.mTokenClient.GetAccessToken();
        }

        public GooglePlayGames.BasicApi.Achievement GetAchievement(string achId)
        {
            if ((this.mAchievements != null) && this.mAchievements.ContainsKey(achId))
            {
                return this.mAchievements[achId];
            }
            return null;
        }

        public IntPtr GetApiClient() => 
            InternalHooks.InternalHooks_GetApiClient(this.mServices.AsHandle());

        public IEventsClient GetEventsClient()
        {
            object gameServicesLock = this.GameServicesLock;
            lock (gameServicesLock)
            {
                return this.mEventsClient;
            }
        }

        public IUserProfile[] GetFriends()
        {
            if ((this.mFriends == null) && !this.friendsLoading)
            {
                GooglePlayGames.OurUtils.Logger.w("Getting friends before they are loaded!!!");
                this.friendsLoading = true;
                this.LoadFriends(delegate (bool ok) {
                    GooglePlayGames.OurUtils.Logger.d(string.Concat(new object[] { "loading: ", ok, " mFriends = ", this.mFriends }));
                    if (!ok)
                    {
                        GooglePlayGames.OurUtils.Logger.e("Friends list did not load successfully.  Disabling loading until re-authenticated");
                    }
                    this.friendsLoading = !ok;
                });
            }
            return ((this.mFriends != null) ? ((IUserProfile[]) this.mFriends.ToArray()) : new IUserProfile[0]);
        }

        [Obsolete("Use GetServerAuthCode() then exchange it for a token")]
        public void GetIdToken(Action<string> idTokenCallback)
        {
            if (!this.IsAuthenticated())
            {
                Debug.Log("Cannot get API client - not authenticated");
                idTokenCallback(null);
            }
            if (!GameInfo.WebClientIdInitialized())
            {
                if ((this.noWebClientIdWarningCount++ % this.webclientWarningFreq) == 0)
                {
                    Debug.LogError("Web client ID has not been set, cannot request id token.");
                    this.noWebClientIdWarningCount = (this.noWebClientIdWarningCount / this.webclientWarningFreq) + 1;
                }
                idTokenCallback(null);
            }
            this.mTokenClient.SetRationale(this.rationale);
            this.mTokenClient.GetIdToken("1024220442272-32g9n7mlifgbmfna622vusdnts3nfr1h.apps.googleusercontent.com", idTokenCallback);
        }

        public void GetPlayerStats(Action<CommonStatusCodes, GooglePlayGames.BasicApi.PlayerStats> callback)
        {
            <GetPlayerStats>c__AnonStoreyC1 yc = new <GetPlayerStats>c__AnonStoreyC1 {
                callback = callback
            };
            this.mServices.StatsManager().FetchForPlayer(new Action<GooglePlayGames.Native.PInvoke.StatsManager.FetchForPlayerResponse>(yc.<>m__24));
        }

        public IQuestsClient GetQuestsClient()
        {
            object gameServicesLock = this.GameServicesLock;
            lock (gameServicesLock)
            {
                return this.mQuestsClient;
            }
        }

        public IRealTimeMultiplayerClient GetRtmpClient()
        {
            if (!this.IsAuthenticated())
            {
                return null;
            }
            object gameServicesLock = this.GameServicesLock;
            lock (gameServicesLock)
            {
                return this.mRealTimeClient;
            }
        }

        public ISavedGameClient GetSavedGameClient()
        {
            object gameServicesLock = this.GameServicesLock;
            lock (gameServicesLock)
            {
                return this.mSavedGameClient;
            }
        }

        public void GetServerAuthCode(string serverClientId, Action<CommonStatusCodes, string> callback)
        {
            <GetServerAuthCode>c__AnonStoreyBE ybe = new <GetServerAuthCode>c__AnonStoreyBE {
                callback = callback
            };
            this.mServices.FetchServerAuthCode(serverClientId, new Action<GooglePlayGames.Native.PInvoke.GameServices.FetchServerAuthCodeResponse>(ybe.<>m__1F));
        }

        public ITurnBasedMultiplayerClient GetTbmpClient()
        {
            object gameServicesLock = this.GameServicesLock;
            lock (gameServicesLock)
            {
                return this.mTurnBasedClient;
            }
        }

        public string GetToken()
        {
            if (this.mTokenClient != null)
            {
                return this.mTokenClient.GetAccessToken();
            }
            return null;
        }

        public string GetUserDisplayName() => 
            this.mUser?.userName;

        public string GetUserEmail()
        {
            if (!this.IsAuthenticated())
            {
                Debug.Log("Cannot get API client - not authenticated");
                return null;
            }
            if (!GameInfo.RequireGooglePlus())
            {
                if ((this.needGPlusWarningCount++ % this.needGPlusWarningFreq) == 0)
                {
                    Debug.LogError("RequiresGooglePlus not set, cannot request email.");
                    this.needGPlusWarningCount = (this.needGPlusWarningCount / this.needGPlusWarningFreq) + 1;
                }
                return null;
            }
            this.mTokenClient.SetRationale(this.rationale);
            return this.mTokenClient.GetEmail();
        }

        public string GetUserId() => 
            this.mUser?.id;

        public string GetUserImageUrl() => 
            this.mUser?.AvatarURL;

        private void HandleAuthTransition(GooglePlayGames.Native.Cwrapper.Types.AuthOperation operation, GooglePlayGames.Native.Cwrapper.CommonErrorStatus.AuthStatus status)
        {
            GooglePlayGames.OurUtils.Logger.d(string.Concat(new object[] { "Starting Auth Transition. Op: ", operation, " status: ", status }));
            object authStateLock = this.AuthStateLock;
            lock (authStateLock)
            {
                GooglePlayGames.Native.Cwrapper.Types.AuthOperation operation2 = operation;
                if (operation2 != GooglePlayGames.Native.Cwrapper.Types.AuthOperation.SIGN_IN)
                {
                    if (operation2 == GooglePlayGames.Native.Cwrapper.Types.AuthOperation.SIGN_OUT)
                    {
                        goto Label_019F;
                    }
                    goto Label_01AA;
                }
                if (status == GooglePlayGames.Native.Cwrapper.CommonErrorStatus.AuthStatus.VALID)
                {
                    <HandleAuthTransition>c__AnonStoreyC0 yc = new <HandleAuthTransition>c__AnonStoreyC0 {
                        <>f__this = this
                    };
                    if (this.mSilentAuthCallbacks != null)
                    {
                        this.mPendingAuthCallbacks = (Action<bool>) Delegate.Combine(this.mPendingAuthCallbacks, this.mSilentAuthCallbacks);
                        this.mSilentAuthCallbacks = null;
                    }
                    yc.currentAuthGeneration = this.mAuthGeneration;
                    this.mServices.AchievementManager().FetchAll(new Action<GooglePlayGames.Native.PInvoke.AchievementManager.FetchAllResponse>(yc.<>m__22));
                    this.mServices.PlayerManager().FetchSelf(new Action<GooglePlayGames.Native.PInvoke.PlayerManager.FetchSelfResponse>(yc.<>m__23));
                }
                else if (this.mAuthState == 2)
                {
                    this.mSilentAuthFailed = true;
                    this.mAuthState = 0;
                    Action<bool> mSilentAuthCallbacks = this.mSilentAuthCallbacks;
                    this.mSilentAuthCallbacks = null;
                    Debug.Log("Invoking callbacks, AuthState changed from silentPending to Unauthenticated.");
                    InvokeCallbackOnGameThread<bool>(mSilentAuthCallbacks, false);
                    if (this.mPendingAuthCallbacks != null)
                    {
                        Debug.Log("there are pending auth callbacks - starting AuthUI");
                        this.GameServices().StartAuthorizationUI();
                    }
                }
                else
                {
                    Debug.Log("AuthState == " + ((AuthState) this.mAuthState) + " calling auth callbacks with failure");
                    this.UnpauseUnityPlayer();
                    Action<bool> mPendingAuthCallbacks = this.mPendingAuthCallbacks;
                    this.mPendingAuthCallbacks = null;
                    InvokeCallbackOnGameThread<bool>(mPendingAuthCallbacks, false);
                }
                goto Label_01D0;
            Label_019F:
                this.ToUnauthenticated();
                goto Label_01D0;
            Label_01AA:
                GooglePlayGames.OurUtils.Logger.e("Unknown AuthOperation " + operation);
            Label_01D0:;
            }
        }

        internal void HandleInvitation(GooglePlayGames.Native.Cwrapper.Types.MultiplayerEvent eventType, string invitationId, GooglePlayGames.Native.PInvoke.MultiplayerInvitation invitation)
        {
            Action<Invitation, bool> mInvitationDelegate = this.mInvitationDelegate;
            if (mInvitationDelegate == null)
            {
                GooglePlayGames.OurUtils.Logger.d(string.Concat(new object[] { "Received ", eventType, " for invitation ", invitationId, " but no handler was registered." }));
            }
            else if (eventType == GooglePlayGames.Native.Cwrapper.Types.MultiplayerEvent.REMOVED)
            {
                GooglePlayGames.OurUtils.Logger.d("Ignoring REMOVED for invitation " + invitationId);
            }
            else
            {
                bool flag = eventType == GooglePlayGames.Native.Cwrapper.Types.MultiplayerEvent.UPDATED_FROM_APP_LAUNCH;
                mInvitationDelegate(invitation.AsInvitation(), flag);
            }
        }

        public void IncrementAchievement(string achId, int steps, Action<bool> callback)
        {
            <IncrementAchievement>c__AnonStoreyC6 yc = new <IncrementAchievement>c__AnonStoreyC6 {
                achId = achId,
                callback = callback,
                <>f__this = this
            };
            Misc.CheckNotNull<string>(yc.achId);
            yc.callback = AsOnGameThreadCallback<bool>(yc.callback);
            this.InitializeGameServices();
            GooglePlayGames.BasicApi.Achievement achievement = this.GetAchievement(yc.achId);
            if (achievement == null)
            {
                GooglePlayGames.OurUtils.Logger.e("Could not increment, no achievement with ID " + yc.achId);
                yc.callback(false);
            }
            else if (!achievement.IsIncremental)
            {
                GooglePlayGames.OurUtils.Logger.e("Could not increment, achievement with ID " + yc.achId + " was not incremental");
                yc.callback(false);
            }
            else if (steps < 0)
            {
                GooglePlayGames.OurUtils.Logger.e("Attempted to increment by negative steps");
                yc.callback(false);
            }
            else
            {
                this.GameServices().AchievementManager().Increment(yc.achId, Convert.ToUInt32(steps));
                this.GameServices().AchievementManager().Fetch(yc.achId, new Action<GooglePlayGames.Native.PInvoke.AchievementManager.FetchResponse>(yc.<>m__2B));
            }
        }

        private void InitializeGameServices()
        {
            object gameServicesLock = this.GameServicesLock;
            lock (gameServicesLock)
            {
                if (this.mServices == null)
                {
                    using (GameServicesBuilder builder = GameServicesBuilder.Create())
                    {
                        using (PlatformConfiguration configuration = this.clientImpl.CreatePlatformConfiguration())
                        {
                            this.RegisterInvitationDelegate(this.mConfiguration.InvitationDelegate);
                            builder.SetOnAuthFinishedCallback(new GameServicesBuilder.AuthFinishedCallback(this.HandleAuthTransition));
                            builder.SetOnTurnBasedMatchEventCallback((eventType, matchId, match) => this.mTurnBasedClient.HandleMatchEvent(eventType, matchId, match));
                            builder.SetOnMultiplayerInvitationEventCallback(new Action<GooglePlayGames.Native.Cwrapper.Types.MultiplayerEvent, string, GooglePlayGames.Native.PInvoke.MultiplayerInvitation>(this.HandleInvitation));
                            if (this.mConfiguration.EnableSavedGames)
                            {
                                builder.EnableSnapshots();
                            }
                            if (this.mConfiguration.RequireGooglePlus)
                            {
                                builder.RequireGooglePlus();
                            }
                            Debug.Log("Building GPG services, implicitly attempts silent auth");
                            this.mAuthState = 2;
                            this.mServices = builder.Build(configuration);
                            this.mEventsClient = new NativeEventClient(new GooglePlayGames.Native.PInvoke.EventManager(this.mServices));
                            this.mQuestsClient = new NativeQuestClient(new GooglePlayGames.Native.PInvoke.QuestManager(this.mServices));
                            this.mTurnBasedClient = new NativeTurnBasedMultiplayerClient(this, new TurnBasedManager(this.mServices));
                            this.mTurnBasedClient.RegisterMatchDelegate(this.mConfiguration.MatchDelegate);
                            this.mRealTimeClient = new NativeRealtimeMultiplayerClient(this, new RealtimeManager(this.mServices));
                            if (this.mConfiguration.EnableSavedGames)
                            {
                                this.mSavedGameClient = new NativeSavedGameClient(new GooglePlayGames.Native.PInvoke.SnapshotManager(this.mServices));
                            }
                            else
                            {
                                this.mSavedGameClient = new UnsupportedSavedGamesClient("You must enable saved games before it can be used. See PlayGamesClientConfiguration.Builder.EnableSavedGames.");
                            }
                            this.mAuthState = 2;
                            this.mTokenClient = this.clientImpl.CreateTokenClient(false);
                        }
                    }
                }
            }
        }

        private static void InvokeCallbackOnGameThread<T>(Action<T> callback, T data)
        {
            <InvokeCallbackOnGameThread>c__AnonStoreyBD<T> ybd = new <InvokeCallbackOnGameThread>c__AnonStoreyBD<T> {
                callback = callback,
                data = data
            };
            if (ybd.callback != null)
            {
                PlayGamesHelperObject.RunOnGameThread(new Action(ybd.<>m__1D));
            }
        }

        public bool IsAuthenticated()
        {
            object authStateLock = this.AuthStateLock;
            lock (authStateLock)
            {
                return (this.mAuthState == 1);
            }
        }

        public int LeaderboardMaxResults() => 
            this.GameServices().LeaderboardManager().LeaderboardMaxResults;

        public void LoadAchievements(Action<GooglePlayGames.BasicApi.Achievement[]> callback)
        {
            GooglePlayGames.BasicApi.Achievement[] array = new GooglePlayGames.BasicApi.Achievement[this.mAchievements.Count];
            this.mAchievements.Values.CopyTo(array, 0);
            callback(array);
        }

        public void LoadFriends(Action<bool> callback)
        {
            <LoadFriends>c__AnonStoreyBF ybf = new <LoadFriends>c__AnonStoreyBF {
                callback = callback,
                <>f__this = this
            };
            if (!this.IsAuthenticated())
            {
                GooglePlayGames.OurUtils.Logger.d("Cannot loadFriends when not authenticated");
                ybf.callback(false);
            }
            else if (this.mFriends != null)
            {
                ybf.callback(true);
            }
            else
            {
                this.mServices.PlayerManager().FetchFriends(new Action<GooglePlayGames.BasicApi.ResponseStatus, List<GooglePlayGames.BasicApi.Multiplayer.Player>>(ybf.<>m__20));
            }
        }

        public void LoadMoreScores(ScorePageToken token, int rowCount, Action<LeaderboardScoreData> callback)
        {
            this.GameServices().LeaderboardManager().LoadScorePage(null, rowCount, token, callback);
        }

        public void LoadScores(string leaderboardId, GooglePlayGames.BasicApi.LeaderboardStart start, int rowCount, GooglePlayGames.BasicApi.LeaderboardCollection collection, GooglePlayGames.BasicApi.LeaderboardTimeSpan timeSpan, Action<LeaderboardScoreData> callback)
        {
            this.GameServices().LeaderboardManager().LoadLeaderboardData(leaderboardId, start, rowCount, collection, timeSpan, this.mUser.id, callback);
        }

        public void LoadUsers(string[] userIds, Action<IUserProfile[]> callback)
        {
            <LoadUsers>c__AnonStoreyC2 yc = new <LoadUsers>c__AnonStoreyC2 {
                callback = callback
            };
            this.mServices.PlayerManager().FetchList(userIds, new Action<NativePlayer[]>(yc.<>m__25));
        }

        private void MaybeFinishAuthentication()
        {
            Action<bool> callback = null;
            object authStateLock = this.AuthStateLock;
            lock (authStateLock)
            {
                if ((this.mUser == null) || (this.mAchievements == null))
                {
                    GooglePlayGames.OurUtils.Logger.d(string.Concat(new object[] { "Auth not finished. User=", this.mUser, " achievements=", this.mAchievements }));
                    return;
                }
                GooglePlayGames.OurUtils.Logger.d("Auth finished. Proceeding.");
                callback = this.mPendingAuthCallbacks;
                this.mPendingAuthCallbacks = null;
                this.mAuthState = 1;
            }
            if (callback != null)
            {
                GooglePlayGames.OurUtils.Logger.d("Invoking Callbacks: " + callback);
                InvokeCallbackOnGameThread<bool>(callback, true);
            }
        }

        private void PopulateAchievements(uint authGeneration, GooglePlayGames.Native.PInvoke.AchievementManager.FetchAllResponse response)
        {
            if (authGeneration != this.mAuthGeneration)
            {
                GooglePlayGames.OurUtils.Logger.d("Received achievement callback after signout occurred, ignoring");
            }
            else
            {
                GooglePlayGames.OurUtils.Logger.d("Populating Achievements, status = " + response.Status());
                object authStateLock = this.AuthStateLock;
                lock (authStateLock)
                {
                    if ((response.Status() != GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.VALID) && (response.Status() != GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.VALID_BUT_STALE))
                    {
                        GooglePlayGames.OurUtils.Logger.e("Error retrieving achievements - check the log for more information. Failing signin.");
                        Action<bool> mPendingAuthCallbacks = this.mPendingAuthCallbacks;
                        this.mPendingAuthCallbacks = null;
                        if (mPendingAuthCallbacks != null)
                        {
                            InvokeCallbackOnGameThread<bool>(mPendingAuthCallbacks, false);
                        }
                        this.SignOut();
                        return;
                    }
                    Dictionary<string, GooglePlayGames.BasicApi.Achievement> dictionary = new Dictionary<string, GooglePlayGames.BasicApi.Achievement>();
                    IEnumerator<NativeAchievement> enumerator = response.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            NativeAchievement current = enumerator.Current;
                            using (current)
                            {
                                dictionary[current.Id()] = current.AsAchievement();
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
                    GooglePlayGames.OurUtils.Logger.d("Found " + dictionary.Count + " Achievements");
                    this.mAchievements = dictionary;
                }
                GooglePlayGames.OurUtils.Logger.d("Maybe finish for Achievements");
                this.MaybeFinishAuthentication();
            }
        }

        private void PopulateUser(uint authGeneration, GooglePlayGames.Native.PInvoke.PlayerManager.FetchSelfResponse response)
        {
            GooglePlayGames.OurUtils.Logger.d("Populating User");
            if (authGeneration != this.mAuthGeneration)
            {
                GooglePlayGames.OurUtils.Logger.d("Received user callback after signout occurred, ignoring");
            }
            else
            {
                object authStateLock = this.AuthStateLock;
                lock (authStateLock)
                {
                    if ((response.Status() != GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.VALID) && (response.Status() != GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.VALID_BUT_STALE))
                    {
                        GooglePlayGames.OurUtils.Logger.e("Error retrieving user, signing out");
                        Action<bool> mPendingAuthCallbacks = this.mPendingAuthCallbacks;
                        this.mPendingAuthCallbacks = null;
                        if (mPendingAuthCallbacks != null)
                        {
                            InvokeCallbackOnGameThread<bool>(mPendingAuthCallbacks, false);
                        }
                        this.SignOut();
                        return;
                    }
                    this.mUser = response.Self().AsPlayer();
                    this.mFriends = null;
                }
                GooglePlayGames.OurUtils.Logger.d("Found User: " + this.mUser);
                GooglePlayGames.OurUtils.Logger.d("Maybe finish for User");
                this.MaybeFinishAuthentication();
            }
        }

        public void RegisterInvitationDelegate(InvitationReceivedDelegate invitationDelegate)
        {
            <RegisterInvitationDelegate>c__AnonStoreyCA yca = new <RegisterInvitationDelegate>c__AnonStoreyCA {
                invitationDelegate = invitationDelegate
            };
            if (yca.invitationDelegate == null)
            {
                this.mInvitationDelegate = null;
            }
            else
            {
                this.mInvitationDelegate = Callbacks.AsOnGameThreadCallback<Invitation, bool>(new Action<Invitation, bool>(yca.<>m__2F));
            }
        }

        public void RevealAchievement(string achId, Action<bool> callback)
        {
            <RevealAchievement>c__AnonStoreyC4 yc = new <RevealAchievement>c__AnonStoreyC4 {
                achId = achId,
                <>f__this = this
            };
            if (<>f__am$cache1B == null)
            {
                <>f__am$cache1B = a => a.IsRevealed;
            }
            this.UpdateAchievement("Reveal", yc.achId, callback, <>f__am$cache1B, new Action<GooglePlayGames.BasicApi.Achievement>(yc.<>m__29));
        }

        public void SetStepsAtLeast(string achId, int steps, Action<bool> callback)
        {
            <SetStepsAtLeast>c__AnonStoreyC7 yc = new <SetStepsAtLeast>c__AnonStoreyC7 {
                achId = achId,
                callback = callback,
                <>f__this = this
            };
            Misc.CheckNotNull<string>(yc.achId);
            yc.callback = AsOnGameThreadCallback<bool>(yc.callback);
            this.InitializeGameServices();
            GooglePlayGames.BasicApi.Achievement achievement = this.GetAchievement(yc.achId);
            if (achievement == null)
            {
                GooglePlayGames.OurUtils.Logger.e("Could not increment, no achievement with ID " + yc.achId);
                yc.callback(false);
            }
            else if (!achievement.IsIncremental)
            {
                GooglePlayGames.OurUtils.Logger.e("Could not increment, achievement with ID " + yc.achId + " is not incremental");
                yc.callback(false);
            }
            else if (steps < 0)
            {
                GooglePlayGames.OurUtils.Logger.e("Attempted to increment by negative steps");
                yc.callback(false);
            }
            else
            {
                this.GameServices().AchievementManager().SetStepsAtLeast(yc.achId, Convert.ToUInt32(steps));
                this.GameServices().AchievementManager().Fetch(yc.achId, new Action<GooglePlayGames.Native.PInvoke.AchievementManager.FetchResponse>(yc.<>m__2C));
            }
        }

        public void ShowAchievementsUI(Action<GooglePlayGames.BasicApi.UIStatus> cb)
        {
            <ShowAchievementsUI>c__AnonStoreyC8 yc = new <ShowAchievementsUI>c__AnonStoreyC8 {
                cb = cb
            };
            if (this.IsAuthenticated())
            {
                Action<GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus> noopUICallback = Callbacks.NoopUICallback;
                if (yc.cb != null)
                {
                    noopUICallback = new Action<GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus>(yc.<>m__2D);
                }
                noopUICallback = AsOnGameThreadCallback<GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus>(noopUICallback);
                this.GameServices().AchievementManager().ShowAllUI(noopUICallback);
            }
        }

        public void ShowLeaderboardUI(string leaderboardId, GooglePlayGames.BasicApi.LeaderboardTimeSpan span, Action<GooglePlayGames.BasicApi.UIStatus> cb)
        {
            <ShowLeaderboardUI>c__AnonStoreyC9 yc = new <ShowLeaderboardUI>c__AnonStoreyC9 {
                cb = cb
            };
            if (this.IsAuthenticated())
            {
                Action<GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus> noopUICallback = Callbacks.NoopUICallback;
                if (yc.cb != null)
                {
                    noopUICallback = new Action<GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus>(yc.<>m__2E);
                }
                noopUICallback = AsOnGameThreadCallback<GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus>(noopUICallback);
                if (leaderboardId == null)
                {
                    this.GameServices().LeaderboardManager().ShowAllUI(noopUICallback);
                }
                else
                {
                    this.GameServices().LeaderboardManager().ShowUI(leaderboardId, span, noopUICallback);
                }
            }
        }

        public void SignOut()
        {
            this.ToUnauthenticated();
            if (this.GameServices() != null)
            {
                this.GameServices().SignOut();
            }
        }

        public void SubmitScore(string leaderboardId, long score, Action<bool> callback)
        {
            callback = AsOnGameThreadCallback<bool>(callback);
            if (!this.IsAuthenticated())
            {
                callback(false);
            }
            this.InitializeGameServices();
            if (leaderboardId == null)
            {
                throw new ArgumentNullException("leaderboardId");
            }
            this.GameServices().LeaderboardManager().SubmitScore(leaderboardId, score, null);
            callback(true);
        }

        public void SubmitScore(string leaderboardId, long score, string metadata, Action<bool> callback)
        {
            callback = AsOnGameThreadCallback<bool>(callback);
            if (!this.IsAuthenticated())
            {
                callback(false);
            }
            this.InitializeGameServices();
            if (leaderboardId == null)
            {
                throw new ArgumentNullException("leaderboardId");
            }
            this.GameServices().LeaderboardManager().SubmitScore(leaderboardId, score, metadata);
            callback(true);
        }

        private void ToUnauthenticated()
        {
            object authStateLock = this.AuthStateLock;
            lock (authStateLock)
            {
                this.mUser = null;
                this.mFriends = null;
                this.mAchievements = null;
                this.mAuthState = 0;
                this.mTokenClient = this.clientImpl.CreateTokenClient(true);
                this.mAuthGeneration++;
            }
        }

        public void UnlockAchievement(string achId, Action<bool> callback)
        {
            <UnlockAchievement>c__AnonStoreyC3 yc = new <UnlockAchievement>c__AnonStoreyC3 {
                achId = achId,
                <>f__this = this
            };
            if (<>f__am$cache1A == null)
            {
                <>f__am$cache1A = a => a.IsUnlocked;
            }
            this.UpdateAchievement("Unlock", yc.achId, callback, <>f__am$cache1A, new Action<GooglePlayGames.BasicApi.Achievement>(yc.<>m__27));
        }

        private void UnpauseUnityPlayer()
        {
        }

        private void UpdateAchievement(string updateType, string achId, Action<bool> callback, Predicate<GooglePlayGames.BasicApi.Achievement> alreadyDone, Action<GooglePlayGames.BasicApi.Achievement> updateAchievment)
        {
            <UpdateAchievement>c__AnonStoreyC5 yc = new <UpdateAchievement>c__AnonStoreyC5 {
                achId = achId,
                callback = callback,
                <>f__this = this
            };
            yc.callback = AsOnGameThreadCallback<bool>(yc.callback);
            Misc.CheckNotNull<string>(yc.achId);
            this.InitializeGameServices();
            GooglePlayGames.BasicApi.Achievement achievement = this.GetAchievement(yc.achId);
            if (achievement == null)
            {
                GooglePlayGames.OurUtils.Logger.d("Could not " + updateType + ", no achievement with ID " + yc.achId);
                yc.callback(false);
            }
            else if (alreadyDone(achievement))
            {
                GooglePlayGames.OurUtils.Logger.d("Did not need to perform " + updateType + ": on achievement " + yc.achId);
                yc.callback(true);
            }
            else
            {
                GooglePlayGames.OurUtils.Logger.d("Performing " + updateType + " on " + yc.achId);
                updateAchievment(achievement);
                this.GameServices().AchievementManager().Fetch(yc.achId, new Action<GooglePlayGames.Native.PInvoke.AchievementManager.FetchResponse>(yc.<>m__2A));
            }
        }

        [CompilerGenerated]
        private sealed class <AsOnGameThreadCallback>c__AnonStoreyBC<T>
        {
            internal Action<T> callback;

            internal void <>m__1C(T result)
            {
                NativeClient.InvokeCallbackOnGameThread<T>(this.callback, result);
            }
        }

        [CompilerGenerated]
        private sealed class <GetPlayerStats>c__AnonStoreyC1
        {
            internal Action<CommonStatusCodes, GooglePlayGames.BasicApi.PlayerStats> callback;

            internal void <>m__24(GooglePlayGames.Native.PInvoke.StatsManager.FetchForPlayerResponse playerStatsResponse)
            {
                CommonStatusCodes codes = ConversionUtils.ConvertResponseStatusToCommonStatus(playerStatsResponse.Status());
                if ((codes != CommonStatusCodes.Success) && (codes != CommonStatusCodes.SuccessCached))
                {
                    GooglePlayGames.OurUtils.Logger.e("Error loading PlayerStats: " + playerStatsResponse.Status().ToString());
                }
                if (this.callback != null)
                {
                    if (playerStatsResponse.PlayerStats() != null)
                    {
                        this.callback(codes, playerStatsResponse.PlayerStats().AsPlayerStats());
                    }
                    else
                    {
                        this.callback(codes, new GooglePlayGames.BasicApi.PlayerStats());
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetServerAuthCode>c__AnonStoreyBE
        {
            internal Action<CommonStatusCodes, string> callback;

            internal void <>m__1F(GooglePlayGames.Native.PInvoke.GameServices.FetchServerAuthCodeResponse serverAuthCodeResponse)
            {
                CommonStatusCodes codes = ConversionUtils.ConvertResponseStatusToCommonStatus(serverAuthCodeResponse.Status());
                if ((codes != CommonStatusCodes.Success) && (codes != CommonStatusCodes.SuccessCached))
                {
                    GooglePlayGames.OurUtils.Logger.e("Error loading server auth code: " + serverAuthCodeResponse.Status().ToString());
                }
                if (this.callback != null)
                {
                    this.callback(codes, serverAuthCodeResponse.Code());
                }
            }
        }

        [CompilerGenerated]
        private sealed class <HandleAuthTransition>c__AnonStoreyC0
        {
            internal NativeClient <>f__this;
            internal uint currentAuthGeneration;

            internal void <>m__22(GooglePlayGames.Native.PInvoke.AchievementManager.FetchAllResponse results)
            {
                this.<>f__this.PopulateAchievements(this.currentAuthGeneration, results);
            }

            internal void <>m__23(GooglePlayGames.Native.PInvoke.PlayerManager.FetchSelfResponse results)
            {
                this.<>f__this.PopulateUser(this.currentAuthGeneration, results);
            }
        }

        [CompilerGenerated]
        private sealed class <IncrementAchievement>c__AnonStoreyC6
        {
            internal NativeClient <>f__this;
            internal string achId;
            internal Action<bool> callback;

            internal void <>m__2B(GooglePlayGames.Native.PInvoke.AchievementManager.FetchResponse rsp)
            {
                if (rsp.Status() == GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.VALID)
                {
                    this.<>f__this.mAchievements.Remove(this.achId);
                    this.<>f__this.mAchievements.Add(this.achId, rsp.Achievement().AsAchievement());
                    this.callback(true);
                }
                else
                {
                    GooglePlayGames.OurUtils.Logger.e(string.Concat(new object[] { "Cannot refresh achievement ", this.achId, ": ", rsp.Status() }));
                    this.callback(false);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <InvokeCallbackOnGameThread>c__AnonStoreyBD<T>
        {
            internal Action<T> callback;
            internal T data;

            internal void <>m__1D()
            {
                GooglePlayGames.OurUtils.Logger.d("Invoking user callback on game thread");
                this.callback(this.data);
            }
        }

        [CompilerGenerated]
        private sealed class <LoadFriends>c__AnonStoreyBF
        {
            internal NativeClient <>f__this;
            internal Action<bool> callback;

            internal void <>m__20(GooglePlayGames.BasicApi.ResponseStatus status, List<GooglePlayGames.BasicApi.Multiplayer.Player> players)
            {
                if ((status == GooglePlayGames.BasicApi.ResponseStatus.Success) || (status == GooglePlayGames.BasicApi.ResponseStatus.SuccessWithStale))
                {
                    this.<>f__this.mFriends = players;
                    this.callback(true);
                }
                else
                {
                    this.<>f__this.mFriends = new List<GooglePlayGames.BasicApi.Multiplayer.Player>();
                    GooglePlayGames.OurUtils.Logger.e("Got " + status + " loading friends");
                    this.callback(false);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <LoadUsers>c__AnonStoreyC2
        {
            internal Action<IUserProfile[]> callback;

            internal void <>m__25(NativePlayer[] nativeUsers)
            {
                IUserProfile[] profileArray = new IUserProfile[nativeUsers.Length];
                for (int i = 0; i < profileArray.Length; i++)
                {
                    profileArray[i] = nativeUsers[i].AsPlayer();
                }
                this.callback(profileArray);
            }
        }

        [CompilerGenerated]
        private sealed class <RegisterInvitationDelegate>c__AnonStoreyCA
        {
            internal InvitationReceivedDelegate invitationDelegate;

            internal void <>m__2F(Invitation invitation, bool autoAccept)
            {
                this.invitationDelegate(invitation, autoAccept);
            }
        }

        [CompilerGenerated]
        private sealed class <RevealAchievement>c__AnonStoreyC4
        {
            internal NativeClient <>f__this;
            internal string achId;

            internal void <>m__29(GooglePlayGames.BasicApi.Achievement a)
            {
                a.IsRevealed = true;
                this.<>f__this.GameServices().AchievementManager().Reveal(this.achId);
            }
        }

        [CompilerGenerated]
        private sealed class <SetStepsAtLeast>c__AnonStoreyC7
        {
            internal NativeClient <>f__this;
            internal string achId;
            internal Action<bool> callback;

            internal void <>m__2C(GooglePlayGames.Native.PInvoke.AchievementManager.FetchResponse rsp)
            {
                if (rsp.Status() == GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.VALID)
                {
                    this.<>f__this.mAchievements.Remove(this.achId);
                    this.<>f__this.mAchievements.Add(this.achId, rsp.Achievement().AsAchievement());
                    this.callback(true);
                }
                else
                {
                    GooglePlayGames.OurUtils.Logger.e(string.Concat(new object[] { "Cannot refresh achievement ", this.achId, ": ", rsp.Status() }));
                    this.callback(false);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ShowAchievementsUI>c__AnonStoreyC8
        {
            internal Action<GooglePlayGames.BasicApi.UIStatus> cb;

            internal void <>m__2D(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus result)
            {
                this.cb((GooglePlayGames.BasicApi.UIStatus) result);
            }
        }

        [CompilerGenerated]
        private sealed class <ShowLeaderboardUI>c__AnonStoreyC9
        {
            internal Action<GooglePlayGames.BasicApi.UIStatus> cb;

            internal void <>m__2E(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus result)
            {
                this.cb((GooglePlayGames.BasicApi.UIStatus) result);
            }
        }

        [CompilerGenerated]
        private sealed class <UnlockAchievement>c__AnonStoreyC3
        {
            internal NativeClient <>f__this;
            internal string achId;

            internal void <>m__27(GooglePlayGames.BasicApi.Achievement a)
            {
                a.IsUnlocked = true;
                this.<>f__this.GameServices().AchievementManager().Unlock(this.achId);
            }
        }

        [CompilerGenerated]
        private sealed class <UpdateAchievement>c__AnonStoreyC5
        {
            internal NativeClient <>f__this;
            internal string achId;
            internal Action<bool> callback;

            internal void <>m__2A(GooglePlayGames.Native.PInvoke.AchievementManager.FetchResponse rsp)
            {
                if (rsp.Status() == GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.VALID)
                {
                    this.<>f__this.mAchievements.Remove(this.achId);
                    this.<>f__this.mAchievements.Add(this.achId, rsp.Achievement().AsAchievement());
                    this.callback(true);
                }
                else
                {
                    GooglePlayGames.OurUtils.Logger.e(string.Concat(new object[] { "Cannot refresh achievement ", this.achId, ": ", rsp.Status() }));
                    this.callback(false);
                }
            }
        }

        private enum AuthState
        {
            Unauthenticated,
            Authenticated,
            SilentPending
        }
    }
}

