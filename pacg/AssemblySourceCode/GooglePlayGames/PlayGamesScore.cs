namespace GooglePlayGames
{
    using System;
    using UnityEngine.SocialPlatforms;

    public class PlayGamesScore : IScore
    {
        private DateTime mDate = new DateTime(0x7b2, 1, 1, 0, 0, 0);
        private string mLbId;
        private string mMetadata = string.Empty;
        private string mPlayerId = string.Empty;
        private ulong mRank;
        private long mValue;

        internal PlayGamesScore(DateTime date, string leaderboardId, ulong rank, string playerId, ulong value, string metadata)
        {
            this.mDate = date;
            this.mLbId = this.leaderboardID;
            this.mRank = rank;
            this.mPlayerId = playerId;
            this.mValue = (long) value;
            this.mMetadata = metadata;
        }

        public void ReportScore(Action<bool> callback)
        {
            PlayGamesPlatform.Instance.ReportScore(this.mValue, this.mLbId, this.mMetadata, callback);
        }

        public DateTime date =>
            this.mDate;

        public string formattedValue =>
            this.mValue.ToString();

        public string leaderboardID
        {
            get => 
                this.mLbId;
            set
            {
                this.mLbId = value;
            }
        }

        public int rank =>
            ((int) this.mRank);

        public string userID =>
            this.mPlayerId;

        public long value
        {
            get => 
                this.mValue;
            set
            {
                this.mValue = value;
            }
        }
    }
}

