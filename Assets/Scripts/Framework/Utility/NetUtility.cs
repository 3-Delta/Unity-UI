using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;

public static class NetUtility {
    public static readonly string MacRegex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
    public static readonly string MacReplace = "$1:$2:$3:$4:$5:$6";

    public static readonly double KBDivider = 1024;
    public static readonly double MBDivider = Math.Pow(KBDivider, 2);
    public static readonly double GBDivider = Math.Pow(KBDivider, 3);
    public static readonly double TBDivider = Math.Pow(KBDivider, 4);

    private static StringBuilder _bytesUnit;

    /// <summary>
    /// Format Physical Address to human readable string
    /// </summary>
    /// <param name="_input"></param>
    /// <returns></returns>
    public static string HumanReadable(this PhysicalAddress _input) {
        return Regex.Replace(_input.ToString(), MacRegex, MacReplace);
    }

    /// <summary>
    /// Add the multiplier according to iValue for sFormat
    /// </summary>
    /// <param name="iValue">Base value</param>
    /// <param name="sFormat">Measure unit</param>
    /// <returns></returns>
    public static string BytesFormat(this double iValue, string sFormat = "B") {
        _bytesUnit = new StringBuilder(sFormat, 5);

        if (iValue < 1) {
            return 0.ToString();
        }

        if (iValue >= TBDivider) // 1tb
        {
            iValue = iValue / TBDivider;
            _bytesUnit.Insert(0, "T");
        }
        else if (iValue >= GBDivider) // 1gb
        {
            iValue = iValue / GBDivider;
            _bytesUnit.Insert(0, "G");
        }
        else if (iValue >= MBDivider) // 1mb
        {
            iValue = iValue / MBDivider;
            _bytesUnit.Insert(0, "M");
        }
        else if (iValue >= KBDivider) // 1kb
        {
            iValue = iValue / KBDivider;
            _bytesUnit.Insert(0, "K");
        }

        _bytesUnit.Insert(0, " ");

        return iValue.ToString("#.##") + _bytesUnit.ToString();
    }

    public static string BytesFormat(this long iValue, string sFormat = "B") {
        return Convert.ToDouble(iValue).BytesFormat(sFormat);
    }

    /// <summary>
    /// int example = 152;
    /// Console.WriteLine(example.Round(100)); // round to the nearest 100
    /// Console.WriteLine(example.Round(10)); // round to the nearest 10
    /// </summary>
    /// <param name="i"></param>
    /// <param name="nearest"></param>
    /// <returns></returns>
    public static int Round(this int i, int nearest) {
        return (int)((long)i).Round(nearest);
    }

    public static long Round(this long i, int nearest) {
        if (nearest <= 0 || nearest % 10 != 0) {
            throw new ArgumentOutOfRangeException("nearest", "Must round to a positive multiple of 10");
        }

        return (i + 5 * nearest / 10) / nearest * nearest;
    }
}
