namespace GooglePlayGames.Android
{
    using Com.Google.Android.Gms.Common.Api;
    using GooglePlayGames;
    using GooglePlayGames.OurUtils;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class AndroidTokenClient : TokenClient
    {
        private string accessToken;
        private string accountName;
        private bool apiAccessDenied;
        private int apiWarningCount;
        private int apiWarningFreq = 0x186a0;
        private bool fetchingAccessToken;
        private bool fetchingEmail;
        private bool fetchingIdToken;
        private const string FetchTokenMethod = "fetchToken";
        private const string FetchTokenSignature = "(Landroid/app/Activity;Ljava/lang/String;ZZZLjava/lang/String;)Lcom/google/android/gms/common/api/PendingResult;";
        private string idToken;
        private Action<string> idTokenCb;
        private string idTokenScope;
        private string rationale;
        private const string TokenFragmentClass = "com.google.games.bridge.TokenFragment";
        private int webClientWarningCount;
        private int webClientWarningFreq = 0x186a0;

        internal void Fetch(string scope, bool fetchEmail, bool fetchAccessToken, bool fetchIdToken, Action<bool> doneCallback)
        {
            <Fetch>c__AnonStoreyB5 yb = new <Fetch>c__AnonStoreyB5 {
                scope = scope,
                fetchEmail = fetchEmail,
                fetchAccessToken = fetchAccessToken,
                fetchIdToken = fetchIdToken,
                doneCallback = doneCallback,
                <>f__this = this
            };
            if (this.apiAccessDenied)
            {
                if ((this.apiWarningCount++ % this.apiWarningFreq) == 0)
                {
                    GooglePlayGames.OurUtils.Logger.w("Access to API denied");
                    this.apiWarningCount = (this.apiWarningCount / this.apiWarningFreq) + 1;
                }
                yb.doneCallback(false);
            }
            else
            {
                PlayGamesHelperObject.RunOnGameThread(new Action(yb.<>m__D));
            }
        }

        internal static void FetchToken(string scope, string rationale, bool fetchEmail, bool fetchAccessToken, bool fetchIdToken, Action<int, string, string, string> callback)
        {
            object[] args = new object[6];
            jvalue[] jvalueArray = AndroidJNIHelper.CreateJNIArgArray(args);
            try
            {
                using (AndroidJavaClass class2 = new AndroidJavaClass("com.google.games.bridge.TokenFragment"))
                {
                    using (AndroidJavaObject obj2 = GetActivity())
                    {
                        IntPtr methodID = AndroidJNI.GetStaticMethodID(class2.GetRawClass(), "fetchToken", "(Landroid/app/Activity;Ljava/lang/String;ZZZLjava/lang/String;)Lcom/google/android/gms/common/api/PendingResult;");
                        jvalueArray[0].l = obj2.GetRawObject();
                        jvalueArray[1].l = AndroidJNI.NewStringUTF(rationale);
                        jvalueArray[2].z = fetchEmail;
                        jvalueArray[3].z = fetchAccessToken;
                        jvalueArray[4].z = fetchIdToken;
                        jvalueArray[5].l = AndroidJNI.NewStringUTF(scope);
                        new PendingResult<TokenResult>(AndroidJNI.CallStaticObjectMethod(class2.GetRawClass(), methodID, jvalueArray)).setResultCallback(new TokenResultCallback(callback));
                    }
                }
            }
            catch (Exception exception)
            {
                GooglePlayGames.OurUtils.Logger.e("Exception launching token request: " + exception.Message);
                GooglePlayGames.OurUtils.Logger.e(exception.ToString());
            }
            finally
            {
                AndroidJNIHelper.DeleteJNIArgArray(args, jvalueArray);
            }
        }

        public string GetAccessToken()
        {
            if (string.IsNullOrEmpty(this.accessToken) && !this.fetchingAccessToken)
            {
                this.fetchingAccessToken = true;
                this.Fetch(this.idTokenScope, false, true, false, delegate (bool rc) {
                    this.fetchingAccessToken = false;
                });
            }
            return this.accessToken;
        }

        private string GetAccountName()
        {
            if (string.IsNullOrEmpty(this.accountName) && !this.fetchingEmail)
            {
                this.fetchingEmail = true;
                this.Fetch(this.idTokenScope, true, false, false, delegate (bool ok) {
                    this.fetchingEmail = false;
                });
            }
            return this.accountName;
        }

        public static AndroidJavaObject GetActivity()
        {
            using (AndroidJavaClass class2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                return class2.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }

        public string GetEmail() => 
            this.GetAccountName();

        [Obsolete("Use PlayGamesPlatform.GetServerAuthCode()")]
        public void GetIdToken(string serverClientId, Action<string> idTokenCallback)
        {
            if (string.IsNullOrEmpty(serverClientId))
            {
                if ((this.webClientWarningCount++ % this.webClientWarningFreq) == 0)
                {
                    GooglePlayGames.OurUtils.Logger.w("serverClientId is empty, cannot get Id Token");
                    this.webClientWarningCount = (this.webClientWarningCount / this.webClientWarningFreq) + 1;
                }
                idTokenCallback(null);
            }
            else
            {
                string str = "audience:server:client_id:" + serverClientId;
                if (string.IsNullOrEmpty(this.idToken) || (str != this.idTokenScope))
                {
                    if (!this.fetchingIdToken)
                    {
                        this.fetchingIdToken = true;
                        this.idTokenScope = str;
                        this.idTokenCb = idTokenCallback;
                        this.Fetch(this.idTokenScope, false, false, true, delegate (bool ok) {
                            this.fetchingIdToken = false;
                            if (!ok)
                            {
                                this.idTokenCb(null);
                            }
                            else
                            {
                                this.idTokenCb(this.idToken);
                            }
                        });
                    }
                }
                else
                {
                    idTokenCallback(this.idToken);
                }
            }
        }

        public void SetRationale(string rationale)
        {
            this.rationale = rationale;
        }

        [CompilerGenerated]
        private sealed class <Fetch>c__AnonStoreyB5
        {
            internal AndroidTokenClient <>f__this;
            internal Action<bool> doneCallback;
            internal bool fetchAccessToken;
            internal bool fetchEmail;
            internal bool fetchIdToken;
            internal string scope;

            internal void <>m__11(int rc, string access, string id, string email)
            {
                if (rc != 0)
                {
                    this.<>f__this.apiAccessDenied = rc == 0xbb9;
                    GooglePlayGames.OurUtils.Logger.w("Non-success returned from fetch: " + rc);
                    this.doneCallback(false);
                }
                else
                {
                    if (this.fetchAccessToken)
                    {
                        GooglePlayGames.OurUtils.Logger.d("a = " + access);
                    }
                    if (this.fetchEmail)
                    {
                        GooglePlayGames.OurUtils.Logger.d("email = " + email);
                    }
                    if (this.fetchIdToken)
                    {
                        GooglePlayGames.OurUtils.Logger.d("idt = " + id);
                    }
                    if (this.fetchAccessToken && !string.IsNullOrEmpty(access))
                    {
                        this.<>f__this.accessToken = access;
                    }
                    if (this.fetchIdToken && !string.IsNullOrEmpty(id))
                    {
                        this.<>f__this.idToken = id;
                        this.<>f__this.idTokenCb(this.<>f__this.idToken);
                    }
                    if (this.fetchEmail && !string.IsNullOrEmpty(email))
                    {
                        this.<>f__this.accountName = email;
                    }
                    this.doneCallback(true);
                }
            }

            internal void <>m__D()
            {
                AndroidTokenClient.FetchToken(this.scope, this.<>f__this.rationale, this.fetchEmail, this.fetchAccessToken, this.fetchIdToken, delegate (int rc, string access, string id, string email) {
                    if (rc != 0)
                    {
                        this.<>f__this.apiAccessDenied = rc == 0xbb9;
                        GooglePlayGames.OurUtils.Logger.w("Non-success returned from fetch: " + rc);
                        this.doneCallback(false);
                    }
                    else
                    {
                        if (this.fetchAccessToken)
                        {
                            GooglePlayGames.OurUtils.Logger.d("a = " + access);
                        }
                        if (this.fetchEmail)
                        {
                            GooglePlayGames.OurUtils.Logger.d("email = " + email);
                        }
                        if (this.fetchIdToken)
                        {
                            GooglePlayGames.OurUtils.Logger.d("idt = " + id);
                        }
                        if (this.fetchAccessToken && !string.IsNullOrEmpty(access))
                        {
                            this.<>f__this.accessToken = access;
                        }
                        if (this.fetchIdToken && !string.IsNullOrEmpty(id))
                        {
                            this.<>f__this.idToken = id;
                            this.<>f__this.idTokenCb(this.<>f__this.idToken);
                        }
                        if (this.fetchEmail && !string.IsNullOrEmpty(email))
                        {
                            this.<>f__this.accountName = email;
                        }
                        this.doneCallback(true);
                    }
                });
            }
        }
    }
}

