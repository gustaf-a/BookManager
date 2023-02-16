using Contracts;
using NLog;

namespace LoggerService;

public class LoggerManager : ILoggerManager
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public LoggerManager()
    {

    }

    public void LogDebug(string message)
        => _logger.Debug(message);

    public void LogError(string message)
        => _logger.Error(message);

    public void LogError(Exception ex, string message)
        => _logger.Error(ex, message);

    public void LogInfo(string message)
    => _logger.Info(message);

    public void LogWarning(string message)
    => _logger.Warn(message);
}
