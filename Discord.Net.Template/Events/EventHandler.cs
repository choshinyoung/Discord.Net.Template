namespace Discord.Net.Template.Events;

public class EventHandler : IEventHandler
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