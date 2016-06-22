namespace GooglePlayGames
{
    using GooglePlayGames.Native.PInvoke;
    using System;

    internal interface IClientImpl
    {
        PlatformConfiguration CreatePlatformConfiguration();
        TokenClient CreateTokenClient(bool reset);
    }
}

