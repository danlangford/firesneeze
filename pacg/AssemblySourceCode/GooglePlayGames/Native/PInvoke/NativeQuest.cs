namespace GooglePlayGames.Native.PInvoke
{
    using GooglePlayGames.BasicApi.Quests;
    using GooglePlayGames.Native.Cwrapper;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class NativeQuest : BaseReferenceHolder, IQuest
    {
        private volatile NativeQuestMilestone mCachedMilestone;

        internal NativeQuest(IntPtr selfPointer) : base(selfPointer)
        {
        }

        protected override void CallDispose(HandleRef selfPointer)
        {
            Quest.Quest_Dispose(selfPointer);
        }

        internal static NativeQuest FromPointer(IntPtr pointer)
        {
            if (pointer.Equals(IntPtr.Zero))
            {
                return null;
            }
            return new NativeQuest(pointer);
        }

        public override string ToString()
        {
            if (base.IsDisposed())
            {
                return "[NativeQuest: DELETED]";
            }
            return $"[NativeQuest: Id={this.Id}, Name={this.Name}, Description={this.Description}, BannerUrl={this.BannerUrl}, IconUrl={this.IconUrl}, State={this.State}, StartTime={this.StartTime}, ExpirationTime={this.ExpirationTime}, AcceptedTime={this.AcceptedTime}]";
        }

        internal bool Valid() => 
            Quest.Quest_Valid(base.SelfPtr());

        public DateTime? AcceptedTime
        {
            get
            {
                long millisSinceEpoch = Quest.Quest_AcceptedTime(base.SelfPtr());
                if (millisSinceEpoch == 0)
                {
                    return null;
                }
                return new DateTime?(PInvokeUtilities.FromMillisSinceUnixEpoch(millisSinceEpoch));
            }
        }

        public string BannerUrl =>
            PInvokeUtilities.OutParamsToString((out_string, out_size) => Quest.Quest_BannerUrl(base.SelfPtr(), out_string, out_size));

        public string Description =>
            PInvokeUtilities.OutParamsToString((out_string, out_size) => Quest.Quest_Description(base.SelfPtr(), out_string, out_size));

        public DateTime ExpirationTime =>
            PInvokeUtilities.FromMillisSinceUnixEpoch(Quest.Quest_ExpirationTime(base.SelfPtr()));

        public string IconUrl =>
            PInvokeUtilities.OutParamsToString((out_string, out_size) => Quest.Quest_IconUrl(base.SelfPtr(), out_string, out_size));

        public string Id =>
            PInvokeUtilities.OutParamsToString((out_string, out_size) => Quest.Quest_Id(base.SelfPtr(), out_string, out_size));

        public IQuestMilestone Milestone
        {
            get
            {
                if (this.mCachedMilestone == null)
                {
                    this.mCachedMilestone = NativeQuestMilestone.FromPointer(Quest.Quest_CurrentMilestone(base.SelfPtr()));
                }
                return this.mCachedMilestone;
            }
        }

        public string Name =>
            PInvokeUtilities.OutParamsToString((out_string, out_size) => Quest.Quest_Name(base.SelfPtr(), out_string, out_size));

        public DateTime StartTime =>
            PInvokeUtilities.FromMillisSinceUnixEpoch(Quest.Quest_StartTime(base.SelfPtr()));

        public GooglePlayGames.BasicApi.Quests.QuestState State
        {
            get
            {
                Types.QuestState state = Quest.Quest_State(base.SelfPtr());
                switch (state)
                {
                    case Types.QuestState.UPCOMING:
                        return GooglePlayGames.BasicApi.Quests.QuestState.Upcoming;

                    case Types.QuestState.OPEN:
                        return GooglePlayGames.BasicApi.Quests.QuestState.Open;

                    case Types.QuestState.ACCEPTED:
                        return GooglePlayGames.BasicApi.Quests.QuestState.Accepted;

                    case Types.QuestState.COMPLETED:
                        return GooglePlayGames.BasicApi.Quests.QuestState.Completed;

                    case Types.QuestState.EXPIRED:
                        return GooglePlayGames.BasicApi.Quests.QuestState.Expired;

                    case Types.QuestState.FAILED:
                        return GooglePlayGames.BasicApi.Quests.QuestState.Failed;
                }
                throw new InvalidOperationException("Unknown state: " + state);
            }
        }
    }
}

