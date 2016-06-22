﻿namespace GooglePlayGames.Native.Cwrapper
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal static class StatsManager
    {
        [DllImport("gpg")]
        internal static extern void StatsManager_FetchForPlayer(HandleRef self, Types.DataSource data_source, FetchForPlayerCallback callback, IntPtr callback_arg);
        [DllImport("gpg")]
        internal static extern void StatsManager_FetchForPlayerResponse_Dispose(HandleRef self);
        [DllImport("gpg")]
        internal static extern IntPtr StatsManager_FetchForPlayerResponse_GetData(HandleRef self);
        [DllImport("gpg")]
        internal static extern GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus StatsManager_FetchForPlayerResponse_GetStatus(HandleRef self);

        internal delegate void FetchForPlayerCallback(IntPtr arg0, IntPtr arg1);
    }
}

