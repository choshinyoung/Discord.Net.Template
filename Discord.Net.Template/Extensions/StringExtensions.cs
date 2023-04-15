using System.Text;

namespace Discord.Net.Template.Extensions;

public static class StringExtensions
{
    public static string Slice(this string content, int length = 2000)
    {
        return content.Length > length ? $"{content.AsSpan(0, length - 3)}..." : content;
    }

    public static string Slice(this string content, out bool isApplied, int length = 2000)
    {
        if (content.Length > length)
        {
            isApplied = true;

            return $"{content.AsSpan(0, length - 3)}...";
        }

        isApplied = false;
        return content;
    }

    public static MemoryStream ToStream(this string content)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }
}