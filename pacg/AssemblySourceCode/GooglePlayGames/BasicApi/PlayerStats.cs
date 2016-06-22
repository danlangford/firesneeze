namespace GooglePlayGames.BasicApi
{
    using System;
    using System.Runtime.CompilerServices;

    public class PlayerStats
    {
        private static float UNSET_VALUE = -1f;

        public PlayerStats()
        {
            this.Valid = false;
        }

        public bool HasAvgSessonLength() => 
            (this.AvgSessonLength != UNSET_VALUE);

        public bool HasChurnProbability() => 
            (this.ChurnProbability != UNSET_VALUE);

        public bool HasDaysSinceLastPlayed() => 
            (this.DaysSinceLastPlayed != ((int) UNSET_VALUE));

        public bool HasNumberOfPurchases() => 
            (this.NumberOfPurchases != ((int) UNSET_VALUE));

        public bool HasNumberOfSessions() => 
            (this.NumberOfSessions != ((int) UNSET_VALUE));

        public bool HasSessPercentile() => 
            (this.SessPercentile != UNSET_VALUE);

        public bool HasSpendPercentile() => 
            (this.SpendPercentile != UNSET_VALUE);

        public float AvgSessonLength { get; set; }

        public float ChurnProbability { get; set; }

        public int DaysSinceLastPlayed { get; set; }

        public int NumberOfPurchases { get; set; }

        public int NumberOfSessions { get; set; }

        public float SessPercentile { get; set; }

        public float SpendPercentile { get; set; }

        public bool Valid { get; set; }
    }
}

