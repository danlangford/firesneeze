namespace GooglePlayGames.Native
{
    using GooglePlayGames.BasicApi;
    using GooglePlayGames.BasicApi.SavedGame;
    using GooglePlayGames.Native.Cwrapper;
    using GooglePlayGames.Native.PInvoke;
    using GooglePlayGames.OurUtils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    internal class NativeSavedGameClient : ISavedGameClient
    {
        private readonly GooglePlayGames.Native.PInvoke.SnapshotManager mSnapshotManager;
        private static readonly Regex ValidFilenameRegex = new Regex(@"\A[a-zA-Z0-9-._~]{1,100}\Z");

        internal NativeSavedGameClient(GooglePlayGames.Native.PInvoke.SnapshotManager manager)
        {
            this.mSnapshotManager = Misc.CheckNotNull<GooglePlayGames.Native.PInvoke.SnapshotManager>(manager);
        }

        private static Types.SnapshotConflictPolicy AsConflictPolicy(ConflictResolutionStrategy strategy)
        {
            switch (strategy)
            {
                case ConflictResolutionStrategy.UseLongestPlaytime:
                    return Types.SnapshotConflictPolicy.LONGEST_PLAYTIME;

                case ConflictResolutionStrategy.UseOriginal:
                    return Types.SnapshotConflictPolicy.LAST_KNOWN_GOOD;

                case ConflictResolutionStrategy.UseUnmerged:
                    return Types.SnapshotConflictPolicy.MOST_RECENTLY_MODIFIED;
            }
            throw new InvalidOperationException("Found unhandled strategy: " + strategy);
        }

        private static Types.DataSource AsDataSource(GooglePlayGames.BasicApi.DataSource source)
        {
            GooglePlayGames.BasicApi.DataSource source2 = source;
            if (source2 != GooglePlayGames.BasicApi.DataSource.ReadCacheOrNetwork)
            {
                if (source2 != GooglePlayGames.BasicApi.DataSource.ReadNetworkOnly)
                {
                    throw new InvalidOperationException("Found unhandled DataSource: " + source);
                }
                return Types.DataSource.NETWORK_ONLY;
            }
            return Types.DataSource.CACHE_OR_NETWORK;
        }

        private static NativeSnapshotMetadataChange AsMetadataChange(SavedGameMetadataUpdate update)
        {
            NativeSnapshotMetadataChange.Builder builder = new NativeSnapshotMetadataChange.Builder();
            if (update.IsCoverImageUpdated)
            {
                builder.SetCoverImageFromPngData(update.UpdatedPngCoverImage);
            }
            if (update.IsDescriptionUpdated)
            {
                builder.SetDescription(update.UpdatedDescription);
            }
            if (update.IsPlayedTimeUpdated)
            {
                builder.SetPlayedTime((ulong) update.UpdatedPlayedTime.Value.TotalMilliseconds);
            }
            return builder.Build();
        }

        private static SavedGameRequestStatus AsRequestStatus(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus status)
        {
            GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus status2 = status;
            switch ((status2 + 5))
            {
                case ~GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.ERROR_LICENSE_CHECK_FAILED:
                    return SavedGameRequestStatus.TimeoutError;

                case GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.VALID_BUT_STALE:
                    Logger.e("User was not authorized (they were probably not logged in).");
                    return SavedGameRequestStatus.AuthenticationError;

                case ~GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.ERROR_VERSION_UPDATE_REQUIRED:
                    return SavedGameRequestStatus.InternalError;

                case ~GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus.ERROR_TIMEOUT:
                    Logger.e("User attempted to use the game without a valid license.");
                    return SavedGameRequestStatus.AuthenticationError;

                case ((GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus) 6):
                case ((GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus) 7):
                    return SavedGameRequestStatus.Success;
            }
            Logger.e("Unknown status: " + status);
            return SavedGameRequestStatus.InternalError;
        }

        private static SavedGameRequestStatus AsRequestStatus(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.SnapshotOpenStatus status)
        {
            GooglePlayGames.Native.Cwrapper.CommonErrorStatus.SnapshotOpenStatus status2 = status;
            switch ((status2 + 5))
            {
                case ((GooglePlayGames.Native.Cwrapper.CommonErrorStatus.SnapshotOpenStatus) 0):
                    return SavedGameRequestStatus.TimeoutError;

                case ~GooglePlayGames.Native.Cwrapper.CommonErrorStatus.SnapshotOpenStatus.ERROR_NOT_AUTHORIZED:
                    return SavedGameRequestStatus.AuthenticationError;
            }
            if (status2 == GooglePlayGames.Native.Cwrapper.CommonErrorStatus.SnapshotOpenStatus.VALID)
            {
                return SavedGameRequestStatus.Success;
            }
            Logger.e("Encountered unknown status: " + status);
            return SavedGameRequestStatus.InternalError;
        }

        private static SelectUIStatus AsUIStatus(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus uiStatus)
        {
            GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus status = uiStatus;
            switch ((status + 6))
            {
                case ~(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus.VALID | GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus.ERROR_INTERNAL):
                    return SelectUIStatus.UserClosedUI;

                case GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus.VALID:
                    return SelectUIStatus.TimeoutError;

                case ~GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus.ERROR_VERSION_UPDATE_REQUIRED:
                    return SelectUIStatus.AuthenticationError;

                case ~GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus.ERROR_TIMEOUT:
                    return SelectUIStatus.InternalError;

                case ((GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus) 7):
                    return SelectUIStatus.SavedGameSelected;
            }
            Logger.e("Encountered unknown UI Status: " + uiStatus);
            return SelectUIStatus.InternalError;
        }

        public void CommitUpdate(ISavedGameMetadata metadata, SavedGameMetadataUpdate updateForMetadata, byte[] updatedBinaryData, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
        {
            <CommitUpdate>c__AnonStoreyF9 yf = new <CommitUpdate>c__AnonStoreyF9 {
                callback = callback
            };
            Misc.CheckNotNull<ISavedGameMetadata>(metadata);
            Misc.CheckNotNull<byte[]>(updatedBinaryData);
            Misc.CheckNotNull<Action<SavedGameRequestStatus, ISavedGameMetadata>>(yf.callback);
            yf.callback = ToOnGameThread<SavedGameRequestStatus, ISavedGameMetadata>(yf.callback);
            NativeSnapshotMetadata metadata2 = metadata as NativeSnapshotMetadata;
            if (metadata2 == null)
            {
                Logger.e("Encountered metadata that was not generated by this ISavedGameClient");
                yf.callback(SavedGameRequestStatus.BadInputError, null);
            }
            else if (!metadata2.IsOpen)
            {
                Logger.e("This method requires an open ISavedGameMetadata.");
                yf.callback(SavedGameRequestStatus.BadInputError, null);
            }
            else
            {
                this.mSnapshotManager.Commit(metadata2, AsMetadataChange(updateForMetadata), updatedBinaryData, new Action<GooglePlayGames.Native.PInvoke.SnapshotManager.CommitResponse>(yf.<>m__6D));
            }
        }

        public void Delete(ISavedGameMetadata metadata)
        {
            Misc.CheckNotNull<ISavedGameMetadata>(metadata);
            this.mSnapshotManager.Delete((NativeSnapshotMetadata) metadata);
        }

        public void FetchAllSavedGames(GooglePlayGames.BasicApi.DataSource source, Action<SavedGameRequestStatus, List<ISavedGameMetadata>> callback)
        {
            <FetchAllSavedGames>c__AnonStoreyFA yfa = new <FetchAllSavedGames>c__AnonStoreyFA {
                callback = callback
            };
            Misc.CheckNotNull<Action<SavedGameRequestStatus, List<ISavedGameMetadata>>>(yfa.callback);
            yfa.callback = ToOnGameThread<SavedGameRequestStatus, List<ISavedGameMetadata>>(yfa.callback);
            this.mSnapshotManager.FetchAll(AsDataSource(source), new Action<GooglePlayGames.Native.PInvoke.SnapshotManager.FetchAllResponse>(yfa.<>m__6E));
        }

        private void InternalManualOpen(string filename, GooglePlayGames.BasicApi.DataSource source, bool prefetchDataOnConflict, ConflictCallback conflictCallback, Action<SavedGameRequestStatus, ISavedGameMetadata> completedCallback)
        {
            <InternalManualOpen>c__AnonStoreyF5 yf = new <InternalManualOpen>c__AnonStoreyF5 {
                completedCallback = completedCallback,
                filename = filename,
                source = source,
                prefetchDataOnConflict = prefetchDataOnConflict,
                conflictCallback = conflictCallback,
                <>f__this = this
            };
            this.mSnapshotManager.Open(yf.filename, AsDataSource(yf.source), Types.SnapshotConflictPolicy.MANUAL, new Action<GooglePlayGames.Native.PInvoke.SnapshotManager.OpenResponse>(yf.<>m__6A));
        }

        internal static bool IsValidFilename(string filename)
        {
            if (filename == null)
            {
                return false;
            }
            return ValidFilenameRegex.IsMatch(filename);
        }

        public void OpenWithAutomaticConflictResolution(string filename, GooglePlayGames.BasicApi.DataSource source, ConflictResolutionStrategy resolutionStrategy, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
        {
            <OpenWithAutomaticConflictResolution>c__AnonStoreyF2 yf = new <OpenWithAutomaticConflictResolution>c__AnonStoreyF2 {
                resolutionStrategy = resolutionStrategy,
                callback = callback
            };
            Misc.CheckNotNull<string>(filename);
            Misc.CheckNotNull<Action<SavedGameRequestStatus, ISavedGameMetadata>>(yf.callback);
            yf.callback = ToOnGameThread<SavedGameRequestStatus, ISavedGameMetadata>(yf.callback);
            if (!IsValidFilename(filename))
            {
                Logger.e("Received invalid filename: " + filename);
                yf.callback(SavedGameRequestStatus.BadInputError, null);
            }
            else
            {
                this.OpenWithManualConflictResolution(filename, source, false, new ConflictCallback(yf.<>m__68), yf.callback);
            }
        }

        public void OpenWithManualConflictResolution(string filename, GooglePlayGames.BasicApi.DataSource source, bool prefetchDataOnConflict, ConflictCallback conflictCallback, Action<SavedGameRequestStatus, ISavedGameMetadata> completedCallback)
        {
            Misc.CheckNotNull<string>(filename);
            Misc.CheckNotNull<ConflictCallback>(conflictCallback);
            Misc.CheckNotNull<Action<SavedGameRequestStatus, ISavedGameMetadata>>(completedCallback);
            conflictCallback = this.ToOnGameThread(conflictCallback);
            completedCallback = ToOnGameThread<SavedGameRequestStatus, ISavedGameMetadata>(completedCallback);
            if (!IsValidFilename(filename))
            {
                Logger.e("Received invalid filename: " + filename);
                completedCallback(SavedGameRequestStatus.BadInputError, null);
            }
            else
            {
                this.InternalManualOpen(filename, source, prefetchDataOnConflict, conflictCallback, completedCallback);
            }
        }

        public void ReadBinaryData(ISavedGameMetadata metadata, Action<SavedGameRequestStatus, byte[]> completedCallback)
        {
            <ReadBinaryData>c__AnonStoreyF7 yf = new <ReadBinaryData>c__AnonStoreyF7 {
                completedCallback = completedCallback
            };
            Misc.CheckNotNull<ISavedGameMetadata>(metadata);
            Misc.CheckNotNull<Action<SavedGameRequestStatus, byte[]>>(yf.completedCallback);
            yf.completedCallback = ToOnGameThread<SavedGameRequestStatus, byte[]>(yf.completedCallback);
            NativeSnapshotMetadata metadata2 = metadata as NativeSnapshotMetadata;
            if (metadata2 == null)
            {
                Logger.e("Encountered metadata that was not generated by this ISavedGameClient");
                yf.completedCallback(SavedGameRequestStatus.BadInputError, null);
            }
            else if (!metadata2.IsOpen)
            {
                Logger.e("This method requires an open ISavedGameMetadata.");
                yf.completedCallback(SavedGameRequestStatus.BadInputError, null);
            }
            else
            {
                this.mSnapshotManager.Read(metadata2, new Action<GooglePlayGames.Native.PInvoke.SnapshotManager.ReadResponse>(yf.<>m__6B));
            }
        }

        public void ShowSelectSavedGameUI(string uiTitle, uint maxDisplayedSavedGames, bool showCreateSaveUI, bool showDeleteSaveUI, Action<SelectUIStatus, ISavedGameMetadata> callback)
        {
            <ShowSelectSavedGameUI>c__AnonStoreyF8 yf = new <ShowSelectSavedGameUI>c__AnonStoreyF8 {
                callback = callback
            };
            Misc.CheckNotNull<string>(uiTitle);
            Misc.CheckNotNull<Action<SelectUIStatus, ISavedGameMetadata>>(yf.callback);
            yf.callback = ToOnGameThread<SelectUIStatus, ISavedGameMetadata>(yf.callback);
            if (maxDisplayedSavedGames <= 0)
            {
                Logger.e("maxDisplayedSavedGames must be greater than 0");
                yf.callback(SelectUIStatus.BadInputError, null);
            }
            else
            {
                this.mSnapshotManager.SnapshotSelectUI(showCreateSaveUI, showDeleteSaveUI, maxDisplayedSavedGames, uiTitle, new Action<GooglePlayGames.Native.PInvoke.SnapshotManager.SnapshotSelectUIResponse>(yf.<>m__6C));
            }
        }

        private ConflictCallback ToOnGameThread(ConflictCallback conflictCallback)
        {
            <ToOnGameThread>c__AnonStoreyF3 yf = new <ToOnGameThread>c__AnonStoreyF3 {
                conflictCallback = conflictCallback
            };
            return new ConflictCallback(yf.<>m__69);
        }

        private static Action<T1, T2> ToOnGameThread<T1, T2>(Action<T1, T2> toConvert)
        {
            <ToOnGameThread>c__AnonStoreyFB<T1, T2> yfb = new <ToOnGameThread>c__AnonStoreyFB<T1, T2> {
                toConvert = toConvert
            };
            return new Action<T1, T2>(yfb.<>m__6F);
        }

        [CompilerGenerated]
        private sealed class <CommitUpdate>c__AnonStoreyF9
        {
            internal Action<SavedGameRequestStatus, ISavedGameMetadata> callback;

            internal void <>m__6D(GooglePlayGames.Native.PInvoke.SnapshotManager.CommitResponse response)
            {
                if (!response.RequestSucceeded())
                {
                    this.callback(NativeSavedGameClient.AsRequestStatus(response.ResponseStatus()), null);
                }
                else
                {
                    this.callback(SavedGameRequestStatus.Success, response.Data());
                }
            }
        }

        [CompilerGenerated]
        private sealed class <FetchAllSavedGames>c__AnonStoreyFA
        {
            internal Action<SavedGameRequestStatus, List<ISavedGameMetadata>> callback;

            internal void <>m__6E(GooglePlayGames.Native.PInvoke.SnapshotManager.FetchAllResponse response)
            {
                if (!response.RequestSucceeded())
                {
                    this.callback(NativeSavedGameClient.AsRequestStatus(response.ResponseStatus()), new List<ISavedGameMetadata>());
                }
                else
                {
                    this.callback(SavedGameRequestStatus.Success, response.Data().Cast<ISavedGameMetadata>().ToList<ISavedGameMetadata>());
                }
            }
        }

        [CompilerGenerated]
        private sealed class <InternalManualOpen>c__AnonStoreyF5
        {
            internal NativeSavedGameClient <>f__this;
            internal Action<SavedGameRequestStatus, ISavedGameMetadata> completedCallback;
            internal ConflictCallback conflictCallback;
            internal string filename;
            internal bool prefetchDataOnConflict;
            internal GooglePlayGames.BasicApi.DataSource source;

            internal void <>m__6A(GooglePlayGames.Native.PInvoke.SnapshotManager.OpenResponse response)
            {
                if (!response.RequestSucceeded())
                {
                    this.completedCallback(NativeSavedGameClient.AsRequestStatus(response.ResponseStatus()), null);
                }
                else if (response.ResponseStatus() == GooglePlayGames.Native.Cwrapper.CommonErrorStatus.SnapshotOpenStatus.VALID)
                {
                    this.completedCallback(SavedGameRequestStatus.Success, response.Data());
                }
                else if (response.ResponseStatus() == GooglePlayGames.Native.Cwrapper.CommonErrorStatus.SnapshotOpenStatus.VALID_WITH_CONFLICT)
                {
                    <InternalManualOpen>c__AnonStoreyF6 yf = new <InternalManualOpen>c__AnonStoreyF6 {
                        <>f__ref$245 = this,
                        original = response.ConflictOriginal(),
                        unmerged = response.ConflictUnmerged()
                    };
                    yf.resolver = new NativeSavedGameClient.NativeConflictResolver(this.<>f__this.mSnapshotManager, response.ConflictId(), yf.original, yf.unmerged, this.completedCallback, new Action(yf.<>m__71));
                    if (!this.prefetchDataOnConflict)
                    {
                        this.conflictCallback(yf.resolver, yf.original, null, yf.unmerged, null);
                    }
                    else
                    {
                        NativeSavedGameClient.Prefetcher prefetcher = new NativeSavedGameClient.Prefetcher(new Action<byte[], byte[]>(yf.<>m__72), this.completedCallback);
                        this.<>f__this.mSnapshotManager.Read(yf.original, new Action<GooglePlayGames.Native.PInvoke.SnapshotManager.ReadResponse>(prefetcher.OnOriginalDataRead));
                        this.<>f__this.mSnapshotManager.Read(yf.unmerged, new Action<GooglePlayGames.Native.PInvoke.SnapshotManager.ReadResponse>(prefetcher.OnUnmergedDataRead));
                    }
                }
                else
                {
                    Logger.e("Unhandled response status");
                    this.completedCallback(SavedGameRequestStatus.InternalError, null);
                }
            }

            private sealed class <InternalManualOpen>c__AnonStoreyF6
            {
                internal NativeSavedGameClient.<InternalManualOpen>c__AnonStoreyF5 <>f__ref$245;
                internal NativeSnapshotMetadata original;
                internal NativeSavedGameClient.NativeConflictResolver resolver;
                internal NativeSnapshotMetadata unmerged;

                internal void <>m__71()
                {
                    this.<>f__ref$245.<>f__this.InternalManualOpen(this.<>f__ref$245.filename, this.<>f__ref$245.source, this.<>f__ref$245.prefetchDataOnConflict, this.<>f__ref$245.conflictCallback, this.<>f__ref$245.completedCallback);
                }

                internal void <>m__72(byte[] originalData, byte[] unmergedData)
                {
                    this.<>f__ref$245.conflictCallback(this.resolver, this.original, originalData, this.unmerged, unmergedData);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <OpenWithAutomaticConflictResolution>c__AnonStoreyF2
        {
            internal Action<SavedGameRequestStatus, ISavedGameMetadata> callback;
            internal ConflictResolutionStrategy resolutionStrategy;

            internal void <>m__68(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData, ISavedGameMetadata unmerged, byte[] unmergedData)
            {
                switch (this.resolutionStrategy)
                {
                    case ConflictResolutionStrategy.UseLongestPlaytime:
                        if (original.TotalTimePlayed < unmerged.TotalTimePlayed)
                        {
                            resolver.ChooseMetadata(unmerged);
                            break;
                        }
                        resolver.ChooseMetadata(original);
                        break;

                    case ConflictResolutionStrategy.UseOriginal:
                        resolver.ChooseMetadata(original);
                        return;

                    case ConflictResolutionStrategy.UseUnmerged:
                        resolver.ChooseMetadata(unmerged);
                        return;

                    default:
                        Logger.e("Unhandled strategy " + this.resolutionStrategy);
                        this.callback(SavedGameRequestStatus.InternalError, null);
                        return;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ReadBinaryData>c__AnonStoreyF7
        {
            internal Action<SavedGameRequestStatus, byte[]> completedCallback;

            internal void <>m__6B(GooglePlayGames.Native.PInvoke.SnapshotManager.ReadResponse response)
            {
                if (!response.RequestSucceeded())
                {
                    this.completedCallback(NativeSavedGameClient.AsRequestStatus(response.ResponseStatus()), null);
                }
                else
                {
                    this.completedCallback(SavedGameRequestStatus.Success, response.Data());
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ShowSelectSavedGameUI>c__AnonStoreyF8
        {
            internal Action<SelectUIStatus, ISavedGameMetadata> callback;

            internal void <>m__6C(GooglePlayGames.Native.PInvoke.SnapshotManager.SnapshotSelectUIResponse response)
            {
                this.callback(NativeSavedGameClient.AsUIStatus(response.RequestStatus()), !response.RequestSucceeded() ? null : response.Data());
            }
        }

        [CompilerGenerated]
        private sealed class <ToOnGameThread>c__AnonStoreyF3
        {
            internal ConflictCallback conflictCallback;

            internal void <>m__69(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData, ISavedGameMetadata unmerged, byte[] unmergedData)
            {
                <ToOnGameThread>c__AnonStoreyF4 yf = new <ToOnGameThread>c__AnonStoreyF4 {
                    <>f__ref$243 = this,
                    resolver = resolver,
                    original = original,
                    originalData = originalData,
                    unmerged = unmerged,
                    unmergedData = unmergedData
                };
                Logger.d("Invoking conflict callback");
                PlayGamesHelperObject.RunOnGameThread(new Action(yf.<>m__70));
            }

            private sealed class <ToOnGameThread>c__AnonStoreyF4
            {
                internal NativeSavedGameClient.<ToOnGameThread>c__AnonStoreyF3 <>f__ref$243;
                internal ISavedGameMetadata original;
                internal byte[] originalData;
                internal IConflictResolver resolver;
                internal ISavedGameMetadata unmerged;
                internal byte[] unmergedData;

                internal void <>m__70()
                {
                    this.<>f__ref$243.conflictCallback(this.resolver, this.original, this.originalData, this.unmerged, this.unmergedData);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ToOnGameThread>c__AnonStoreyFB<T1, T2>
        {
            internal Action<T1, T2> toConvert;

            internal void <>m__6F(T1 val1, T2 val2)
            {
                <ToOnGameThread>c__AnonStoreyFC<T1, T2> yfc = new <ToOnGameThread>c__AnonStoreyFC<T1, T2> {
                    <>f__ref$251 = (NativeSavedGameClient.<ToOnGameThread>c__AnonStoreyFB<T1, T2>) this,
                    val1 = val1,
                    val2 = val2
                };
                PlayGamesHelperObject.RunOnGameThread(new Action(yfc.<>m__73));
            }

            private sealed class <ToOnGameThread>c__AnonStoreyFC
            {
                internal NativeSavedGameClient.<ToOnGameThread>c__AnonStoreyFB<T1, T2> <>f__ref$251;
                internal T1 val1;
                internal T2 val2;

                internal void <>m__73()
                {
                    this.<>f__ref$251.toConvert(this.val1, this.val2);
                }
            }
        }

        private class NativeConflictResolver : IConflictResolver
        {
            private readonly Action<SavedGameRequestStatus, ISavedGameMetadata> mCompleteCallback;
            private readonly string mConflictId;
            private readonly GooglePlayGames.Native.PInvoke.SnapshotManager mManager;
            private readonly NativeSnapshotMetadata mOriginal;
            private readonly Action mRetryFileOpen;
            private readonly NativeSnapshotMetadata mUnmerged;

            internal NativeConflictResolver(GooglePlayGames.Native.PInvoke.SnapshotManager manager, string conflictId, NativeSnapshotMetadata original, NativeSnapshotMetadata unmerged, Action<SavedGameRequestStatus, ISavedGameMetadata> completeCallback, Action retryOpen)
            {
                this.mManager = Misc.CheckNotNull<GooglePlayGames.Native.PInvoke.SnapshotManager>(manager);
                this.mConflictId = Misc.CheckNotNull<string>(conflictId);
                this.mOriginal = Misc.CheckNotNull<NativeSnapshotMetadata>(original);
                this.mUnmerged = Misc.CheckNotNull<NativeSnapshotMetadata>(unmerged);
                this.mCompleteCallback = Misc.CheckNotNull<Action<SavedGameRequestStatus, ISavedGameMetadata>>(completeCallback);
                this.mRetryFileOpen = Misc.CheckNotNull<Action>(retryOpen);
            }

            public void ChooseMetadata(ISavedGameMetadata chosenMetadata)
            {
                NativeSnapshotMetadata metadata = chosenMetadata as NativeSnapshotMetadata;
                if ((metadata != this.mOriginal) && (metadata != this.mUnmerged))
                {
                    Logger.e("Caller attempted to choose a version of the metadata that was not part of the conflict");
                    this.mCompleteCallback(SavedGameRequestStatus.BadInputError, null);
                }
                else
                {
                    this.mManager.Resolve(metadata, new NativeSnapshotMetadataChange.Builder().Build(), this.mConflictId, delegate (GooglePlayGames.Native.PInvoke.SnapshotManager.CommitResponse response) {
                        if (!response.RequestSucceeded())
                        {
                            this.mCompleteCallback(NativeSavedGameClient.AsRequestStatus(response.ResponseStatus()), null);
                        }
                        else
                        {
                            this.mRetryFileOpen();
                        }
                    });
                }
            }
        }

        private class Prefetcher
        {
            [CompilerGenerated]
            private static Action<SavedGameRequestStatus, ISavedGameMetadata> <>f__am$cache7;
            [CompilerGenerated]
            private static Action<SavedGameRequestStatus, ISavedGameMetadata> <>f__am$cache8;
            private Action<SavedGameRequestStatus, ISavedGameMetadata> completedCallback;
            private readonly Action<byte[], byte[]> mDataFetchedCallback;
            private readonly object mLock = new object();
            private byte[] mOriginalData;
            private bool mOriginalDataFetched;
            private byte[] mUnmergedData;
            private bool mUnmergedDataFetched;

            internal Prefetcher(Action<byte[], byte[]> dataFetchedCallback, Action<SavedGameRequestStatus, ISavedGameMetadata> completedCallback)
            {
                this.mDataFetchedCallback = Misc.CheckNotNull<Action<byte[], byte[]>>(dataFetchedCallback);
                this.completedCallback = Misc.CheckNotNull<Action<SavedGameRequestStatus, ISavedGameMetadata>>(completedCallback);
            }

            private void MaybeProceed()
            {
                if (this.mOriginalDataFetched && this.mUnmergedDataFetched)
                {
                    Logger.d("Fetched data for original and unmerged, proceeding");
                    this.mDataFetchedCallback(this.mOriginalData, this.mUnmergedData);
                }
                else
                {
                    Logger.d(string.Concat(new object[] { "Not all data fetched - original:", this.mOriginalDataFetched, " unmerged:", this.mUnmergedDataFetched }));
                }
            }

            internal void OnOriginalDataRead(GooglePlayGames.Native.PInvoke.SnapshotManager.ReadResponse readResponse)
            {
                object mLock = this.mLock;
                lock (mLock)
                {
                    if (!readResponse.RequestSucceeded())
                    {
                        Logger.e("Encountered error while prefetching original data.");
                        this.completedCallback(NativeSavedGameClient.AsRequestStatus(readResponse.ResponseStatus()), null);
                        if (<>f__am$cache7 == null)
                        {
                            <>f__am$cache7 = delegate {
                            };
                        }
                        this.completedCallback = <>f__am$cache7;
                    }
                    else
                    {
                        Logger.d("Successfully fetched original data");
                        this.mOriginalDataFetched = true;
                        this.mOriginalData = readResponse.Data();
                        this.MaybeProceed();
                    }
                }
            }

            internal void OnUnmergedDataRead(GooglePlayGames.Native.PInvoke.SnapshotManager.ReadResponse readResponse)
            {
                object mLock = this.mLock;
                lock (mLock)
                {
                    if (!readResponse.RequestSucceeded())
                    {
                        Logger.e("Encountered error while prefetching unmerged data.");
                        this.completedCallback(NativeSavedGameClient.AsRequestStatus(readResponse.ResponseStatus()), null);
                        if (<>f__am$cache8 == null)
                        {
                            <>f__am$cache8 = delegate {
                            };
                        }
                        this.completedCallback = <>f__am$cache8;
                    }
                    else
                    {
                        Logger.d("Successfully fetched unmerged data");
                        this.mUnmergedDataFetched = true;
                        this.mUnmergedData = readResponse.Data();
                        this.MaybeProceed();
                    }
                }
            }
        }
    }
}

