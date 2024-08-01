using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;
using WebApplication2.Services;

namespace WebApplication2.Workers
{
    public class LeaguesWorker: IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly FbrefParser _fbrefParser;
        private readonly ILogger<LeaguesWorker> _logger;
        private readonly ChromeDriver _driver;
        public LeaguesWorker(IServiceScopeFactory scopeFactory, ILogger<LeaguesWorker> logger, FbrefParser fbrefParser, SeleniumFactory seleniumFactory)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _fbrefParser = fbrefParser;
            _driver = seleniumFactory.Get();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            _ = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            await Cycle(scope, cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex, ex.Message, LogLevel.Error);
                    }

                }
            }, cancellationToken);


            return Task.CompletedTask;
        }

        private async Task Cycle(IServiceScope scope, CancellationToken cancellationToken)
        {
            var leaguesService = scope.ServiceProvider.GetRequiredService<LeaguesService>();

            var leagues = (await leaguesService.GetAll()).ToList();

            _logger.LogInformation("Start LeaguesWorker Cycle", LogLevel.Information);

            var yearLinks = _fbrefParser.GetYearsLinksList(_driver);

            _logger.LogInformation("Load years links", LogLevel.Information);

            foreach (var yearLink in yearLinks)
            {
                var leaguesLinks = _fbrefParser.GetYearLeaguesLinksList(yearLink, _driver);

                _logger.LogInformation("Load leagues links by: " + yearLink, LogLevel.Information);

                foreach ( var leagueLink in leaguesLinks)
                {
                    var isLeagueExist = (await leaguesService.Get(l => l.Url == leagueLink)) != null;

                    if (isLeagueExist && !yearLink.Contains(DateTime.UtcNow.Year.ToString()))
                    {
                        _logger.LogInformation("Stop LeaguesWorker", LogLevel.Information);
                        await Task.Delay(10);
                        continue;
                    }

                    var league = await _fbrefParser.GetLeagueData(leagueLink, _driver);

                    if (league != null)
                    {
                        await leaguesService.UpdateOrAdd(league);

                        _logger.LogInformation("Save League in DB: " + league.Name, LogLevel.Information);
                    }
                    else
                    {
                        _logger.LogInformation("Cannot find League: " + leagueLink, LogLevel.Warning);
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if(_driver != null)
            {
                _driver.Quit();
            }
            return Task.CompletedTask;
        }
    }
}
