namespace Discord.Net.Template.Events;

public static class EventHandler
{
    public static void Register()
    {
        Bot.Client.Log += OnLog;
    }

    private static async Task OnLog(LogMessage message)
    {
        Console.WriteLine(message);

        await Task.CompletedTask;
    }
}