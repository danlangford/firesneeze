using System;

public abstract class LicenseAdapter
{
    protected LicenseAdapter()
    {
    }

    public virtual bool IsPurchasePossible() => 
        false;

    public virtual void Purchase(string productIdentifier)
    {
    }

    public virtual void Restore()
    {
    }

    public virtual void Start()
    {
    }
}

