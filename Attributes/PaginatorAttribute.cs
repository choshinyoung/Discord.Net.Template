namespace Discord.Net.Template.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class PaginatorAttribute(string id) : Attribute
{
    public string Id { get; init; } = id;
}
