namespace GooglePlayGames
{
    using System;

    public static class GameInfo
    {
        public const string ApplicationId = "1024220442272";
        public const string IosClientId = "1024220442272-ifdvcs63qj2aqul3oofo41bh3cunage8.apps.googleusercontent.com";
        public const string NearbyConnectionServiceId = "";
        private const string UnescapedApplicationId = "APP_ID";
        private const string UnescapedIosClientId = "IOS_CLIENTID";
        private const string UnescapedNearbyServiceId = "NEARBY_SERVICE_ID";
        private const string UnescapedRequireGooglePlus = "REQUIRE_GOOGLE_PLUS";
        private const string UnescapedWebClientId = "WEB_CLIENTID";
        public const string WebClientId = "1024220442272-32g9n7mlifgbmfna622vusdnts3nfr1h.apps.googleusercontent.com";

        public static bool ApplicationIdInitialized() => 
            (!string.IsNullOrEmpty("1024220442272") && !"1024220442272".Equals(ToEscapedToken("APP_ID")));

        public static bool IosClientIdInitialized() => 
            (!string.IsNullOrEmpty("1024220442272-ifdvcs63qj2aqul3oofo41bh3cunage8.apps.googleusercontent.com") && !"1024220442272-ifdvcs63qj2aqul3oofo41bh3cunage8.apps.googleusercontent.com".Equals(ToEscapedToken("IOS_CLIENTID")));

        public static bool NearbyConnectionsInitialized() => 
            (!string.IsNullOrEmpty(string.Empty) && !string.Empty.Equals(ToEscapedToken("NEARBY_SERVICE_ID")));

        public static bool RequireGooglePlus() => 
            false;

        private static string ToEscapedToken(string token) => 
            $"__{token}__";

        public static bool WebClientIdInitialized() => 
            (!string.IsNullOrEmpty("1024220442272-32g9n7mlifgbmfna622vusdnts3nfr1h.apps.googleusercontent.com") && !"1024220442272-32g9n7mlifgbmfna622vusdnts3nfr1h.apps.googleusercontent.com".Equals(ToEscapedToken("WEB_CLIENTID")));
    }
}

