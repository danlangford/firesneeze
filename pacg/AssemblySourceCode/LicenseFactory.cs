using System;

public class LicenseFactory
{
    public static License Create(string productIdentifier) => 
        new License(productIdentifier);
}

