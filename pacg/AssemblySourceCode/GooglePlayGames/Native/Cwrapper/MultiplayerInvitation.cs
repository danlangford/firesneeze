﻿namespace GooglePlayGames.Native.Cwrapper
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class MultiplayerInvitation
    {
        [DllImport("gpg")]
        internal static extern uint MultiplayerInvitation_AutomatchingSlotsAvailable(HandleRef self);
        [DllImport("gpg")]
        internal static extern ulong MultiplayerInvitation_CreationTime(HandleRef self);
        [DllImport("gpg")]
        internal static extern void MultiplayerInvitation_Dispose(HandleRef self);
        [DllImport("gpg")]
        internal static extern UIntPtr MultiplayerInvitation_Id(HandleRef self, StringBuilder out_arg, UIntPtr out_size);
        [DllImport("gpg")]
        internal static extern IntPtr MultiplayerInvitation_InvitingParticipant(HandleRef self);
        [DllImport("gpg")]
        internal static extern IntPtr MultiplayerInvitation_Participants_GetElement(HandleRef self, UIntPtr index);
        [DllImport("gpg")]
        internal static extern UIntPtr MultiplayerInvitation_Participants_Length(HandleRef self);
        [DllImport("gpg")]
        internal static extern Types.MultiplayerInvitationType MultiplayerInvitation_Type(HandleRef self);
        [return: MarshalAs(UnmanagedType.I1)]
        [DllImport("gpg")]
        internal static extern bool MultiplayerInvitation_Valid(HandleRef self);
        [DllImport("gpg")]
        internal static extern uint MultiplayerInvitation_Variant(HandleRef self);
    }
}

