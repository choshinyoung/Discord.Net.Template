using Microsoft.Extensions.Configuration;

namespace Discord.Net.Template.Utility;

public static class Config
{
    private static readonly IConfigurationRoot Root;

    static Config()
    {
        Root = new ConfigurationBuilder().AddJsonFile("Data/config.json").Build();
    }

    public static string Get(string key)
    {
        return Root.GetSection(key).Value!;
    }

    public static T Get<T>(string key) where T : IParsable<T>
    {
        return T.Parse(Root.GetSection(key).Value!, null);
    }

    public static bool GetBool(string key)
    {
        return Root.GetSection(key).Value == "True";
    }
}