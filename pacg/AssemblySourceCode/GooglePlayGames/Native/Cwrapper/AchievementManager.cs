﻿namespace GooglePlayGames.Native.Cwrapper
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal static class AchievementManager
    {
        [DllImport("gpg")]
        internal static extern void AchievementManager_Fetch(HandleRef self, Types.DataSource data_source, string achievement_id, FetchCallback callback, IntPtr callback_arg);
        [DllImport("gpg")]
        internal static extern void AchievementManager_FetchAll(HandleRef self, Types.DataSource data_source, FetchAllCallback callback, IntPtr callback_arg);
        [DllImport("gpg")]
        internal static extern void AchievementManager_FetchAllResponse_Dispose(HandleRef self);
        [DllImport("gpg")]
        internal static extern IntPtr AchievementManager_FetchAllResponse_GetData_GetElement(HandleRef self, UIntPtr index);
        [DllImport("gpg")]
        internal static extern UIntPtr AchievementManager_FetchAllResponse_GetData_Length(HandleRef self);
        [DllImport("gpg")]
        internal static extern GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus AchievementManager_FetchAllResponse_GetStatus(HandleRef self);
        [DllImport("gpg")]
        internal static extern void AchievementManager_FetchResponse_Dispose(HandleRef self);
        [DllImport("gpg")]
        internal static extern IntPtr AchievementManager_FetchResponse_GetData(HandleRef self);
        [DllImport("gpg")]
        internal static extern GooglePlayGames.Native.Cwrapper.CommonErrorStatus.ResponseStatus AchievementManager_FetchResponse_GetStatus(HandleRef self);
        [DllImport("gpg")]
        internal static extern void AchievementManager_Increment(HandleRef self, string achievement_id, uint steps);
        [DllImport("gpg")]
        internal static extern void AchievementManager_Reveal(HandleRef self, string achievement_id);
        [DllImport("gpg")]
        internal static extern void AchievementManager_SetStepsAtLeast(HandleRef self, string achievement_id, uint steps);
        [DllImport("gpg")]
        internal static extern void AchievementManager_ShowAllUI(HandleRef self, ShowAllUICallback callback, IntPtr callback_arg);
        [DllImport("gpg")]
        internal static extern void AchievementManager_Unlock(HandleRef self, string achievement_id);

        internal delegate void FetchAllCallback(IntPtr arg0, IntPtr arg1);

        internal delegate void FetchCallback(IntPtr arg0, IntPtr arg1);

        internal delegate void ShowAllUICallback(GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus arg0, IntPtr arg1);
    }
}

