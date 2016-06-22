namespace GooglePlayGames.BasicApi
{
    using GooglePlayGames;
    using GooglePlayGames.BasicApi.Multiplayer;
    using GooglePlayGames.OurUtils;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct PlayGamesClientConfiguration
    {
        public static readonly PlayGamesClientConfiguration DefaultConfiguration;
        private readonly bool mEnableSavedGames;
        private readonly bool mRequireGooglePlus;
        private readonly InvitationReceivedDelegate mInvitationDelegate;
        private readonly GooglePlayGames.BasicApi.Multiplayer.MatchDelegate mMatchDelegate;
        private readonly string mPermissionRationale;
        private PlayGamesClientConfiguration(Builder builder)
        {
            this.mEnableSavedGames = builder.HasEnableSaveGames();
            this.mInvitationDelegate = builder.GetInvitationDelegate();
            this.mMatchDelegate = builder.GetMatchDelegate();
            this.mPermissionRationale = builder.GetPermissionRationale();
            this.mRequireGooglePlus = builder.HasRequireGooglePlus();
        }

        static PlayGamesClientConfiguration()
        {
            DefaultConfiguration = new Builder().Build();
        }

        public bool EnableSavedGames =>
            this.mEnableSavedGames;
        public bool RequireGooglePlus =>
            this.mRequireGooglePlus;
        public InvitationReceivedDelegate InvitationDelegate =>
            this.mInvitationDelegate;
        public GooglePlayGames.BasicApi.Multiplayer.MatchDelegate MatchDelegate =>
            this.mMatchDelegate;
        public string PermissionRationale =>
            this.mPermissionRationale;
        public class Builder
        {
            [CompilerGenerated]
            private static InvitationReceivedDelegate <>f__am$cache5;
            [CompilerGenerated]
            private static MatchDelegate <>f__am$cache6;
            private bool mEnableSaveGames;
            private InvitationReceivedDelegate mInvitationDelegate;
            private MatchDelegate mMatchDelegate;
            private string mRationale;
            private bool mRequireGooglePlus;

            public Builder()
            {
                if (<>f__am$cache5 == null)
                {
                    <>f__am$cache5 = new InvitationReceivedDelegate(PlayGamesClientConfiguration.Builder.<mInvitationDelegate>m__1);
                }
                this.mInvitationDelegate = <>f__am$cache5;
                if (<>f__am$cache6 == null)
                {
                    <>f__am$cache6 = new MatchDelegate(PlayGamesClientConfiguration.Builder.<mMatchDelegate>m__2);
                }
                this.mMatchDelegate = <>f__am$cache6;
            }

            [CompilerGenerated]
            private static void <mInvitationDelegate>m__1(Invitation, bool)
            {
            }

            [CompilerGenerated]
            private static void <mMatchDelegate>m__2(TurnBasedMatch, bool)
            {
            }

            public PlayGamesClientConfiguration Build()
            {
                this.mRequireGooglePlus = GameInfo.RequireGooglePlus();
                return new PlayGamesClientConfiguration(this);
            }

            public PlayGamesClientConfiguration.Builder EnableSavedGames()
            {
                this.mEnableSaveGames = true;
                return this;
            }

            internal InvitationReceivedDelegate GetInvitationDelegate() => 
                this.mInvitationDelegate;

            internal MatchDelegate GetMatchDelegate() => 
                this.mMatchDelegate;

            internal string GetPermissionRationale() => 
                this.mRationale;

            internal bool HasEnableSaveGames() => 
                this.mEnableSaveGames;

            internal bool HasRequireGooglePlus() => 
                this.mRequireGooglePlus;

            public PlayGamesClientConfiguration.Builder RequireGooglePlus()
            {
                this.mRequireGooglePlus = true;
                return this;
            }

            public PlayGamesClientConfiguration.Builder WithInvitationDelegate(InvitationReceivedDelegate invitationDelegate)
            {
                this.mInvitationDelegate = Misc.CheckNotNull<InvitationReceivedDelegate>(invitationDelegate);
                return this;
            }

            public PlayGamesClientConfiguration.Builder WithMatchDelegate(MatchDelegate matchDelegate)
            {
                this.mMatchDelegate = Misc.CheckNotNull<MatchDelegate>(matchDelegate);
                return this;
            }

            public PlayGamesClientConfiguration.Builder WithPermissionRationale(string rationale)
            {
                this.mRationale = rationale;
                return this;
            }
        }
    }
}

