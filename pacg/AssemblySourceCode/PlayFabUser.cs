using System;

public class PlayFabUser : User
{
    public PlayFabUser()
    {
        this.DisplayName = string.Empty;
        this.Id = string.Empty;
        this.Gold = 0;
        this.Chests = 0;
        this.GoldSubDaysRemaining = 0;
        this.GoldSubAvailable = false;
    }
}

