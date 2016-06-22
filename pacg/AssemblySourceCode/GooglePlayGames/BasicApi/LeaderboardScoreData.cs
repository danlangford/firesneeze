namespace GooglePlayGames.BasicApi
{
    using GooglePlayGames;
    using System;
    using System.Collections.Generic;
    using UnityEngine.SocialPlatforms;

    public class LeaderboardScoreData
    {
        private ulong mApproxCount;
        private string mId;
        private ScorePageToken mNextPage;
        private IScore mPlayerScore;
        private ScorePageToken mPrevPage;
        private List<PlayGamesScore> mScores;
        private ResponseStatus mStatus;
        private string mTitle;

        internal LeaderboardScoreData(string leaderboardId)
        {
            this.mScores = new List<PlayGamesScore>();
            this.mId = leaderboardId;
        }

        internal LeaderboardScoreData(string leaderboardId, ResponseStatus status)
        {
            this.mScores = new List<PlayGamesScore>();
            this.mId = leaderboardId;
            this.mStatus = status;
        }

        internal int AddScore(PlayGamesScore score)
        {
            this.mScores.Add(score);
            return this.mScores.Count;
        }

        public override string ToString() => 
            $"[LeaderboardScoreData: mId={this.mId},  mStatus={this.mStatus}, mApproxCount={this.mApproxCount}, mTitle={this.mTitle}]";

        public ulong ApproximateCount
        {
            get => 
                this.mApproxCount;
            internal set
            {
                this.mApproxCount = value;
            }
        }

        public string Id
        {
            get => 
                this.mId;
            internal set
            {
                this.mId = value;
            }
        }

        public ScorePageToken NextPageToken
        {
            get => 
                this.mNextPage;
            internal set
            {
                this.mNextPage = value;
            }
        }

        public IScore PlayerScore
        {
            get => 
                this.mPlayerScore;
            internal set
            {
                this.mPlayerScore = value;
            }
        }

        public ScorePageToken PrevPageToken
        {
            get => 
                this.mPrevPage;
            internal set
            {
                this.mPrevPage = value;
            }
        }

        public IScore[] Scores =>
            this.mScores.ToArray();

        public ResponseStatus Status
        {
            get => 
                this.mStatus;
            internal set
            {
                this.mStatus = value;
            }
        }

        public string Title
        {
            get => 
                this.mTitle;
            internal set
            {
                this.mTitle = value;
            }
        }

        public bool Valid =>
            ((this.mStatus == ResponseStatus.Success) || (this.mStatus == ResponseStatus.SuccessWithStale));
    }
}

