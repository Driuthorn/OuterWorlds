using Configuration;
using Database.Interface;
using Serilog;
using Serilog.Events;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace Planetary.Producer
{
    public class PlanetaryProducer
    {
        private readonly Timer _timer = new Timer();
        private readonly Container _container;
        private readonly ILogger _logger;
        private readonly int _executionFrequencyMinutes = 20;
        private IRedisCache _redisCache;

        public PlanetaryProducer(Container container)
        {
            _container = container;
            _logger = _container.GetInstance<ILogger>();
            _redisCache = _container.GetInstance<IRedisCache>();
        }

        public void Start()
        {
            Log("Start");

            _timer.Elapsed += async (sender, e) => await Execute();
            _timer.AutoReset = false;
            _timer.Start();

            Log("Started");
        }

        private async Task Execute()
        {
            try
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    
                }
            }
            catch (Exception ex)
            {
                Log($"Unexpected Error. Exception {ex.Message}", LogEventLevel.Error, ex);
            }
            finally
            {
                Log("End Execution service.");
                SetNextExceution();
            }
        }

        private void SetNextExceution()
        {
            TimeSpan timeSpanToNextExecution = TimeSpan.FromMinutes(_executionFrequencyMinutes);
            var dateTimeToNextExecution = DateTime.UtcNow.AddSeconds(timeSpanToNextExecution.TotalSeconds);

            Log($"Next execution: {dateTimeToNextExecution:yyyy-MM-dd HH:mm:ss}");

            _timer.Stop();
            _timer.Interval = timeSpanToNextExecution.TotalMilliseconds;
            _timer.Start();
        }

        private void Log(string message, LogEventLevel level = LogEventLevel.Information, Exception ex = null)
        {
            var logMessage = "{ApplicationName} -> {message}";
            switch (level)
            {
                case LogEventLevel.Information:
                    {
                        _logger.Information(logMessage, AppSettings.Settings.ApplicationName, message);
                        break;
                    }
                case LogEventLevel.Error:
                    {
                        _logger.Error(ex, logMessage, AppSettings.Settings.ApplicationName, message);
                        break;
                    }
                case LogEventLevel.Fatal:
                    {
                        _logger.Fatal(logMessage, AppSettings.Settings.ApplicationName, message);
                        break;
                    }
                default:
                    {
                        _logger.Information(logMessage, AppSettings.Settings.ApplicationName, message);
                        break;
                    }
            }
        }
    }
}
