using System.Text.RegularExpressions;

namespace Discord.Net.Template.Utils;

public static partial class RegexUtil
{
    [GeneratedRegex(
        "^\\s*```(?:(?:csharp|cs)\\s)?\\s*(?<block_code>.+?)\\s*```\\s*$|^\\s*(?<code>.+?)\\s*$",
        RegexOptions.Singleline
    )]
    public static partial Regex CodeRegex();
}
