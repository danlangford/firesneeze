namespace GooglePlayGames.Native.PInvoke
{
    using GooglePlayGames.BasicApi.Quests;
    using GooglePlayGames.Native.Cwrapper;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class NativeQuestMilestone : BaseReferenceHolder, IQuestMilestone
    {
        internal NativeQuestMilestone(IntPtr selfPointer) : base(selfPointer)
        {
        }

        protected override void CallDispose(HandleRef selfPointer)
        {
            QuestMilestone.QuestMilestone_Dispose(selfPointer);
        }

        internal static NativeQuestMilestone FromPointer(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
            {
                return null;
            }
            return new NativeQuestMilestone(pointer);
        }

        public override string ToString() => 
            $"[NativeQuestMilestone: Id={this.Id}, EventId={this.EventId}, QuestId={this.QuestId}, CurrentCount={this.CurrentCount}, TargetCount={this.TargetCount}, State={this.State}]";

        internal bool Valid() => 
            QuestMilestone.QuestMilestone_Valid(base.SelfPtr());

        public byte[] CompletionRewardData =>
            PInvokeUtilities.OutParamsToArray<byte>((out_bytes, out_size) => QuestMilestone.QuestMilestone_CompletionRewardData(base.SelfPtr(), out_bytes, out_size));

        public ulong CurrentCount =>
            QuestMilestone.QuestMilestone_CurrentCount(base.SelfPtr());

        public string EventId =>
            PInvokeUtilities.OutParamsToString((out_string, out_size) => QuestMilestone.QuestMilestone_EventId(base.SelfPtr(), out_string, out_size));

        public string Id =>
            PInvokeUtilities.OutParamsToString((out_string, out_size) => QuestMilestone.QuestMilestone_Id(base.SelfPtr(), out_string, out_size));

        public string QuestId =>
            PInvokeUtilities.OutParamsToString((out_string, out_size) => QuestMilestone.QuestMilestone_QuestId(base.SelfPtr(), out_string, out_size));

        public MilestoneState State
        {
            get
            {
                Types.QuestMilestoneState state = QuestMilestone.QuestMilestone_State(base.SelfPtr());
                switch (state)
                {
                    case Types.QuestMilestoneState.NOT_STARTED:
                        return MilestoneState.NotStarted;

                    case Types.QuestMilestoneState.NOT_COMPLETED:
                        return MilestoneState.NotCompleted;

                    case Types.QuestMilestoneState.COMPLETED_NOT_CLAIMED:
                        return MilestoneState.CompletedNotClaimed;

                    case Types.QuestMilestoneState.CLAIMED:
                        return MilestoneState.Claimed;
                }
                throw new InvalidOperationException("Unknown state: " + state);
            }
        }

        public ulong TargetCount =>
            QuestMilestone.QuestMilestone_TargetCount(base.SelfPtr());
    }
}

