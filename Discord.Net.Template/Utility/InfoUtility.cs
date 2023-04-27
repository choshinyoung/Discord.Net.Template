using Discord.Commands;
using Discord.Interactions;
using ModuleInfo = Discord.Commands.ModuleInfo;
using PreconditionAttribute = Discord.Commands.PreconditionAttribute;

namespace Discord.Net.Template.Utility;

public static class InfoUtility
{
    public static bool HavePrecondition<T>(CommandInfo info)
    {
        return info.Preconditions.Any(p => p.GetType() == typeof(T));
    }

    public static T GetPrecondition<T>(CommandInfo info) where T : PreconditionAttribute
    {
        return (T)info.Preconditions.First(p => p.GetType() == typeof(T));
    }

    public static bool HavePrecondition<T>(ModuleInfo info)
    {
        return info.Preconditions.Any(p => p.GetType() == typeof(T));
    }

    public static T GetPrecondition<T>(ModuleInfo info) where T : PreconditionAttribute
    {
        return (T)info.Preconditions.First(p => p.GetType() == typeof(T));
    }

    public static bool HaveAttribute<T>(CommandInfo info)
    {
        return info.Attributes.Any(p => p.GetType() == typeof(T));
    }

    public static T GetAttribute<T>(CommandInfo info) where T : Attribute
    {
        return (T)info.Attributes.First(p => p.GetType() == typeof(T));
    }

    public static bool HaveAttribute<T>(ModuleInfo info)
    {
        return info.Attributes.Any(p => p.GetType() == typeof(T));
    }

    public static T GetAttribute<T>(ModuleInfo info) where T : Attribute
    {
        return (T)info.Attributes.First(p => p.GetType() == typeof(T));
    }

    public static bool HavePrecondition<T>(SlashCommandInfo info)
    {
        return info.Preconditions.Any(p => p.GetType() == typeof(T));
    }

    public static T GetPrecondition<T>(SlashCommandInfo info) where T : Discord.Interactions.PreconditionAttribute
    {
        return (T)info.Preconditions.First(p => p.GetType() == typeof(T));
    }

    public static bool HavePrecondition<T>(Discord.Interactions.ModuleInfo info)
    {
        return info.Preconditions.Any(p => p.GetType() == typeof(T));
    }

    public static T GetPrecondition<T>(Discord.Interactions.ModuleInfo info)
        where T : Discord.Interactions.PreconditionAttribute
    {
        return (T)info.Preconditions.First(p => p.GetType() == typeof(T));
    }

    public static bool HaveAttribute<T>(SlashCommandInfo info)
    {
        return info.Attributes.Any(p => p.GetType() == typeof(T));
    }

    public static T GetAttribute<T>(SlashCommandInfo info) where T : Attribute
    {
        return (T)info.Attributes.First(p => p.GetType() == typeof(T));
    }

    public static bool HaveAttribute<T>(Discord.Interactions.ModuleInfo info)
    {
        return info.Attributes.Any(p => p.GetType() == typeof(T));
    }

    public static T GetAttribute<T>(Discord.Interactions.ModuleInfo info) where T : Attribute
    {
        return (T)info.Attributes.First(p => p.GetType() == typeof(T));
    }
}