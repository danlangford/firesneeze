﻿namespace GooglePlayGames
{
    using System;

    internal interface TokenClient
    {
        string GetAccessToken();
        string GetEmail();
        void GetIdToken(string serverClientId, Action<string> idTokenCallback);
        void SetRationale(string rationale);
    }
}

