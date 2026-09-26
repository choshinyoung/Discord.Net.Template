namespace Discord.Net.Template.Extensions;

public static class AttributeExtensions
{
    public static bool HasAttribute<T>(this IEnumerable<Attribute> attributes)
        where T : Attribute
    {
        return attributes.OfType<T>().Any();
    }

    public static T? GetAttribute<T>(this IEnumerable<Attribute> attributes)
        where T : Attribute
    {
        return attributes.OfType<T>().FirstOrDefault();
    }
}
