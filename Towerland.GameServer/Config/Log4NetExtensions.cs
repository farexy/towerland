using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Logging;

namespace Towerland.GameServer.Config
{
  public static class Log4NetExtensions
  {
    public static ILoggingBuilder AddLog4Net(this ILoggingBuilder loggingBuilder, string logConfigFile = "log.config")
    {
      XmlConfigurator.ConfigureAndWatch(LogManager.GetRepository(Assembly.GetEntryAssembly()), new FileInfo(logConfigFile));
      return loggingBuilder.AddProvider(new Log4NetLoggerProvider());
    }
  }

  public class Log4NetLoggerProvider : ILoggerProvider, IDisposable
  {
    private readonly ConcurrentDictionary<string, Log4NetAppStartErrorLogger> _loggers = new ConcurrentDictionary<string, Log4NetAppStartErrorLogger>();

    public ILogger CreateLogger(string categoryName)
    {
      return _loggers.GetOrAdd(categoryName, new Log4NetAppStartErrorLogger(categoryName));
    }

    public void Dispose()
    {
      this._loggers.Clear();
    }
  }

  public class Log4NetAppStartErrorLogger : ILogger
  {
    private readonly ILog _logger;

    public Log4NetAppStartErrorLogger(string name)
    {
      _logger = LogManager.GetLogger(Assembly.GetEntryAssembly(), name);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter = null)
    {
      if (!this.IsEnabled(logLevel))
        return;
      string str = formatter == null ? state.ToString() : formatter(state, exception);
      if (string.IsNullOrWhiteSpace(str) || logLevel != LogLevel.Critical)
        return;
      _logger.Fatal(str, exception);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
      if (logLevel == LogLevel.Critical)
        return _logger.IsFatalEnabled;
      return false;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
      return null;
    }
  }
}