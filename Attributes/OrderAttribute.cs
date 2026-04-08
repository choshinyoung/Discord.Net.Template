namespace Discord.Net.Template.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class OrderAttribute(int order) : Attribute
{
    public int Order { get; init; } = order;
}
