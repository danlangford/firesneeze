namespace GooglePlayGames.BasicApi
{
    using System;

    public class ScorePageToken
    {
        private LeaderboardCollection mCollection;
        private string mId;
        private object mInternalObject;
        private LeaderboardTimeSpan mTimespan;

        internal ScorePageToken(object internalObject, string id, LeaderboardCollection collection, LeaderboardTimeSpan timespan)
        {
            this.mInternalObject = internalObject;
            this.mId = id;
            this.mCollection = collection;
            this.mTimespan = timespan;
        }

        public LeaderboardCollection Collection =>
            this.mCollection;

        internal object InternalObject =>
            this.mInternalObject;

        public string LeaderboardId =>
            this.mId;

        public LeaderboardTimeSpan TimeSpan =>
            this.mTimespan;
    }
}

