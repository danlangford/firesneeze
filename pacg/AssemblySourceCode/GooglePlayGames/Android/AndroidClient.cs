namespace GooglePlayGames.Android
{
    using GooglePlayGames;
    using GooglePlayGames.Native.PInvoke;
    using GooglePlayGames.OurUtils;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class AndroidClient : IClientImpl
    {
        [CompilerGenerated]
        private static Action<IntPtr> <>f__am$cache1;
        internal const string BridgeActivityClass = "com.google.games.bridge.NativeBridgeActivity";
        private const string LaunchBridgeMethod = "launchBridgeIntent";
        private const string LaunchBridgeSignature = "(Landroid/app/Activity;Landroid/content/Intent;)V";
        private TokenClient tokenClient;

        public PlatformConfiguration CreatePlatformConfiguration()
        {
            AndroidPlatformConfiguration configuration = AndroidPlatformConfiguration.Create();
            using (AndroidJavaObject obj2 = AndroidTokenClient.GetActivity())
            {
                configuration.SetActivity(obj2.GetRawObject());
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = delegate (IntPtr intent) {
                        <CreatePlatformConfiguration>c__AnonStoreyB4 yb = new <CreatePlatformConfiguration>c__AnonStoreyB4 {
                            intentRef = AndroidJNI.NewGlobalRef(intent)
                        };
                        PlayGamesHelperObject.RunOnGameThread(new Action(yb.<>m__C));
                    };
                }
                configuration.SetOptionalIntentHandlerForUI(<>f__am$cache1);
            }
            return configuration;
        }

        public TokenClient CreateTokenClient(bool reset)
        {
            if ((this.tokenClient == null) || reset)
            {
                this.tokenClient = new AndroidTokenClient();
            }
            return this.tokenClient;
        }

        private static void LaunchBridgeIntent(IntPtr bridgedIntent)
        {
            object[] args = new object[2];
            jvalue[] jvalueArray = AndroidJNIHelper.CreateJNIArgArray(args);
            try
            {
                using (AndroidJavaClass class2 = new AndroidJavaClass("com.google.games.bridge.NativeBridgeActivity"))
                {
                    using (AndroidJavaObject obj2 = AndroidTokenClient.GetActivity())
                    {
                        IntPtr methodID = AndroidJNI.GetStaticMethodID(class2.GetRawClass(), "launchBridgeIntent", "(Landroid/app/Activity;Landroid/content/Intent;)V");
                        jvalueArray[0].l = obj2.GetRawObject();
                        jvalueArray[1].l = bridgedIntent;
                        AndroidJNI.CallStaticVoidMethod(class2.GetRawClass(), methodID, jvalueArray);
                    }
                }
            }
            catch (Exception exception)
            {
                GooglePlayGames.OurUtils.Logger.e("Exception launching bridge intent: " + exception.Message);
                GooglePlayGames.OurUtils.Logger.e(exception.ToString());
            }
            finally
            {
                AndroidJNIHelper.DeleteJNIArgArray(args, jvalueArray);
            }
        }

        [CompilerGenerated]
        private sealed class <CreatePlatformConfiguration>c__AnonStoreyB4
        {
            internal IntPtr intentRef;

            internal void <>m__C()
            {
                try
                {
                    AndroidClient.LaunchBridgeIntent(this.intentRef);
                }
                finally
                {
                    AndroidJNI.DeleteGlobalRef(this.intentRef);
                }
            }
        }
    }
}

