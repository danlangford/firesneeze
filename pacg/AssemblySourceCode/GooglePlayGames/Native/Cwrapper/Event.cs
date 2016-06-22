﻿namespace GooglePlayGames.Native.Cwrapper
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class Event
    {
        [DllImport("gpg")]
        internal static extern IntPtr Event_Copy(HandleRef self);
        [DllImport("gpg")]
        internal static extern ulong Event_Count(HandleRef self);
        [DllImport("gpg")]
        internal static extern UIntPtr Event_Description(HandleRef self, StringBuilder out_arg, UIntPtr out_size);
        [DllImport("gpg")]
        internal static extern void Event_Dispose(HandleRef self);
        [DllImport("gpg")]
        internal static extern UIntPtr Event_Id(HandleRef self, StringBuilder out_arg, UIntPtr out_size);
        [DllImport("gpg")]
        internal static extern UIntPtr Event_ImageUrl(HandleRef self, StringBuilder out_arg, UIntPtr out_size);
        [DllImport("gpg")]
        internal static extern UIntPtr Event_Name(HandleRef self, StringBuilder out_arg, UIntPtr out_size);
        [return: MarshalAs(UnmanagedType.I1)]
        [DllImport("gpg")]
        internal static extern bool Event_Valid(HandleRef self);
        [DllImport("gpg")]
        internal static extern Types.EventVisibility Event_Visibility(HandleRef self);
    }
}

