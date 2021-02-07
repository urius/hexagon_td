using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FormatExtensions
{
    public static string ToSpaceSeparatedAmount(this int amount)
    {
        return amount.ToString("# ### ##0");
    }
}
