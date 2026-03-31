using Discord;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Template.Utils;

public static class LogUtil
{
    public static LogLevel MapLogLevel(LogSeverity severity) =>
        severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => LogLevel.Information,
        };

    public static LogLevel MapLogSeverity(LogSeverity severity)
    {
        return severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => LogLevel.Information,
        };
    }

    public static void Log(this ILogger logger, LogMessage message)
    {
        var logLevel = MapLogLevel(message.Severity);

        if (logger.IsEnabled(logLevel))
        {
            if (message.Exception != null)
            {
                logger.Log(
                    logLevel,
                    message.Exception,
                    "{Source}: {Message}",
                    message.Source,
                    message.Exception.Message
                );
            }
            else
            {
                logger.Log(logLevel, "{Source}: {Message}", message.Source, message.Message);
            }
        }
    }
}
