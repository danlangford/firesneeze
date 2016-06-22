using System;
using System.Collections.Generic;

public class CatalogItem
{
    public string Id;
    public Dictionary<string, int> RealCurrencyPrices;
    public Dictionary<string, int> VirtualCurrencyPrices;

    public CatalogItem()
    {
        this.Id = string.Empty;
        this.VirtualCurrencyPrices = new Dictionary<string, int>();
        this.RealCurrencyPrices = new Dictionary<string, int>();
    }

    public CatalogItem(string id, Dictionary<string, int> virtualCurrency, Dictionary<string, int> realCurrency)
    {
        this.Id = id;
        this.VirtualCurrencyPrices = virtualCurrency;
        this.RealCurrencyPrices = realCurrency;
    }
}

