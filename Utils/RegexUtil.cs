using System.Text.RegularExpressions;

namespace Discord.Net.Template.Utils;

public static partial class RegexUtil
{
    [GeneratedRegex("^\\s*(```(cs)?\\s*(?<block_code>.+)\\s*```)|(?<code>.+)\\s*$")]
    public static partial Regex CodeRegex();
}
