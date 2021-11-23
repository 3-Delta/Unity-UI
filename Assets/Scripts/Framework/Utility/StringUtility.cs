using System.Text.RegularExpressions;

public static class StringUtility {
    public static bool IsChineseLetter(char ch) {
        Regex reg = new Regex(@"[\u4e00-\u9fa5]");
        return reg.IsMatch(ch.ToString());
    }

    public static int ChineseLetterCount(string input) {
        Regex reg = new Regex(@"[\u4e00-\u9fa5]");
        return reg.Matches(input).Count;
    }

    public static bool HasChineseLetter(string input) {
        return ChineseLetterCount(input) > 0;
    }
}
