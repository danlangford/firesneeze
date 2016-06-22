namespace GooglePlayGames.Native.PInvoke
{
    using GooglePlayGames.Native.Cwrapper;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class TurnBasedMatchConfig : BaseReferenceHolder
    {
        internal TurnBasedMatchConfig(IntPtr selfPointer) : base(selfPointer)
        {
        }

        protected override void CallDispose(HandleRef selfPointer)
        {
            GooglePlayGames.Native.Cwrapper.TurnBasedMatchConfig.TurnBasedMatchConfig_Dispose(selfPointer);
        }

        internal long ExclusiveBitMask() => 
            GooglePlayGames.Native.Cwrapper.TurnBasedMatchConfig.TurnBasedMatchConfig_ExclusiveBitMask(base.SelfPtr());

        internal uint MaximumAutomatchingPlayers() => 
            GooglePlayGames.Native.Cwrapper.TurnBasedMatchConfig.TurnBasedMatchConfig_MaximumAutomatchingPlayers(base.SelfPtr());

        internal uint MinimumAutomatchingPlayers() => 
            GooglePlayGames.Native.Cwrapper.TurnBasedMatchConfig.TurnBasedMatchConfig_MinimumAutomatchingPlayers(base.SelfPtr());

        private string PlayerIdAtIndex(UIntPtr index)
        {
            <PlayerIdAtIndex>c__AnonStorey11E storeye = new <PlayerIdAtIndex>c__AnonStorey11E {
                index = index,
                <>f__this = this
            };
            return PInvokeUtilities.OutParamsToString(new PInvokeUtilities.OutStringMethod(storeye.<>m__E8));
        }

        internal IEnumerator<string> PlayerIdsToInvite() => 
            PInvokeUtilities.ToEnumerator<string>(GooglePlayGames.Native.Cwrapper.TurnBasedMatchConfig.TurnBasedMatchConfig_PlayerIdsToInvite_Length(base.SelfPtr()), new Func<UIntPtr, string>(this.PlayerIdAtIndex));

        internal uint Variant() => 
            GooglePlayGames.Native.Cwrapper.TurnBasedMatchConfig.TurnBasedMatchConfig_Variant(base.SelfPtr());

        [CompilerGenerated]
        private sealed class <PlayerIdAtIndex>c__AnonStorey11E
        {
            internal GooglePlayGames.Native.PInvoke.TurnBasedMatchConfig <>f__this;
            internal UIntPtr index;

            internal UIntPtr <>m__E8(StringBuilder out_string, UIntPtr size) => 
                GooglePlayGames.Native.Cwrapper.TurnBasedMatchConfig.TurnBasedMatchConfig_PlayerIdsToInvite_GetElement(this.<>f__this.SelfPtr(), this.index, out_string, size);
        }
    }
}

