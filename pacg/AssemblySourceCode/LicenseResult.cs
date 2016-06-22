using System;

public class LicenseResult
{
    public string Message;
    public LicenseOperationType Operation;
    public string ProductIdentifier;
    public bool Result;

    public LicenseResult(LicenseOperationType type, bool result)
    {
        this.Operation = type;
        this.ProductIdentifier = null;
        this.Result = result;
        this.Message = null;
    }

    public LicenseResult(LicenseOperationType type, string id, bool result, string text)
    {
        this.Operation = type;
        this.ProductIdentifier = id;
        this.Result = result;
        this.Message = text;
    }
}

