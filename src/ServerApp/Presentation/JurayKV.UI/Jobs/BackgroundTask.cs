using JurayKV.Infrastructure.Services;

namespace JurayKV.UI.Jobs
{
    public class BackgroundTask : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;
        public BackgroundTask(ILogger<BackgroundTask> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            // Set the current time
            DateTime now = DateTime.Now;

            // Calculate the time until the next 6:00 AM
            DateTime nextSixAM = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);
            if (now > nextSixAM)
            {
                // If it's already past 6:00 AM today, schedule for tomorrow
                nextSixAM = nextSixAM.AddDays(1);
            }

            // Calculate the initial delay for the morning task
            TimeSpan initialMorningDelay = nextSixAM - now;

            // Create a Timer that executes DoWork every day at 6:00 AM
            //_timer = new Timer(DoWork, null, initialMorningDelay, TimeSpan.FromDays(1));
            _timer = new Timer(DoWork, null, TimeSpan.FromHours(6),
                TimeSpan.FromMinutes(10)); 
            _timer = new Timer(DoWorkMorningMail, null, initialMorningDelay, TimeSpan.FromDays(1));

            // Calculate the time until the next 6:00 PM
            DateTime nextSixPM = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0);
            if (now > nextSixPM)
            {
                // If it's already past 6:00 PM today, schedule for tomorrow
                nextSixPM = nextSixPM.AddDays(1);
            }

            // Calculate the initial delay for the evening task
            TimeSpan initialEveningDelay = nextSixPM - now;

            // Create a Timer that executes DoWorkEveningMail every day from 6:00 PM to 8:00 PM
            _timer = new Timer(DoWorkEveningMail, null, initialEveningDelay, TimeSpan.FromHours(2));

            return Task.CompletedTask;
        }
        private async void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");

            using (var scope = _scopeFactory.CreateScope())
            {
                var backgroundActivity = scope.ServiceProvider.GetRequiredService<BackgroundActivity>();
                await backgroundActivity.UpdateAllUserAdsAfterSix();
            }
        }
        private async void DoWorkEveningMail(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");

            using (var scope = _scopeFactory.CreateScope())
            {
                var backgroundActivity = scope.ServiceProvider.GetRequiredService<BackgroundActivity>();
                await backgroundActivity.SendEmailToEveningActiveAdsAsync();
            }
        }

        private async void DoWorkMorningMail(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");

            using (var scope = _scopeFactory.CreateScope())
            {
                var backgroundActivity = scope.ServiceProvider.GetRequiredService<BackgroundActivity>();
                await backgroundActivity.SendEmailToMorningReminderAsync();
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
