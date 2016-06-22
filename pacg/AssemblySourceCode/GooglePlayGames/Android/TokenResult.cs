namespace GooglePlayGames.Android
{
    using Com.Google.Android.Gms.Common.Api;
    using Google.Developers;
    using System;

    internal class TokenResult : JavaObjWrapper, Result
    {
        public TokenResult(IntPtr ptr) : base(ptr)
        {
        }

        public string getAccessToken() => 
            base.InvokeCall<string>("getAccessToken", "()Ljava/lang/String;", new object[0]);

        public string getEmail() => 
            base.InvokeCall<string>("getEmail", "()Ljava/lang/String;", new object[0]);

        public string getIdToken() => 
            base.InvokeCall<string>("getIdToken", "()Ljava/lang/String;", new object[0]);

        public Status getStatus() => 
            new Status(base.InvokeCall<IntPtr>("getStatus", "()Lcom/google/android/gms/common/api/Status;", new object[0]));
    }
}

