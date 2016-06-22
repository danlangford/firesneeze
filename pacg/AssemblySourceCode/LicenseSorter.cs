using System;
using System.Collections.Generic;

public class LicenseSorter : IComparer<LicenseProduct>
{
    public int Compare(LicenseProduct a, LicenseProduct b)
    {
        int num = -1 * string.CompareOrdinal(a.Price, b.Price);
        if (num == 0)
        {
            num = a.Title.CompareTo(b.Title);
        }
        return num;
    }

    public static IComparer<LicenseProduct> SortByPrice() => 
        new LicenseSorter();
}

