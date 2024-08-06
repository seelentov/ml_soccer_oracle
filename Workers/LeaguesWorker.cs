
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WebApplication2.Data;
using WebApplication2.Services;
using static WebApplication2.Services.Soccer365Parser;

namespace WebApplication2.Workers
{
    public class LeaguesWorker : IHostedService
    {
        private readonly ILogger<LeaguesWorker> _logger;
        private readonly Soccer365Parser _soccer365parser;
        private IWebDriver _driver;
        private readonly IServiceScopeFactory _scopeFactory;
        private MethodOptions _options;
        private readonly TelegramService _telegramService;
        private readonly SeleniumFactory _seleniumFactory;

        public LeaguesWorker(ILogger<LeaguesWorker> logger, Soccer365Parser soccer365parser, SeleniumFactory selenium, IServiceScopeFactory scopeFactory, TelegramService telegramService)
        {
            _logger = logger;
            _soccer365parser = soccer365parser;
            _driver = selenium.Get();
            _scopeFactory = scopeFactory;
            _seleniumFactory = selenium;

            _options = new MethodOptions()
            {
                driver = _driver,
                wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10))
            };
            _telegramService = telegramService;
        }

        public void ReBuildDriver()
        {
            if (_driver != null)
            {
                _driver.Quit();
            }

            _driver = _seleniumFactory.Get();

            _options = new MethodOptions()
            {
                wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10)),
                driver = _driver
            };
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _ = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        try
                        {
                            var leaguesService = scope.ServiceProvider.GetRequiredService<LeaguesService>();

                            var parsedLeagues = await _soccer365parser.GetLeagues(_options);

                            foreach (var parsedLeague in parsedLeagues)
                            {
                                var league = await leaguesService.Get(l=>l.Url == parsedLeague.Url);

                                if (!(league != null && league.ParsedNested))
                                {
                                    var nestedLeagues = await _soccer365parser.GetNestedLeagues(parsedLeague, _options);

                                    foreach (var nestedLeague in nestedLeagues)
                                    {
                                        await leaguesService.UpdateOrAdd(nestedLeague);
                                        _logger.LogInformation("Add nested league in DB " + nestedLeague.Name, Microsoft.Extensions.Logging.LogLevel.Information);
                                    }
                                }

                                await leaguesService.UpdateOrAdd(parsedLeague);

                                leaguesService.CheckParsedNested(l => l.Url == parsedLeague.Url);

                                _logger.LogInformation("Add league in DB " + parsedLeague.Name, Microsoft.Extensions.Logging.LogLevel.Information);
                            }

                            await Task.Delay(TimeSpan.FromDays(1));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation(ex, ex.Message, Microsoft.Extensions.Logging.LogLevel.Information);
                            await _telegramService.SendMessage(ex.Message, "LeaguesWorkerError");
                            ReBuildDriver();
                        }
                    }

                }
            }, cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_driver != null)
            {
                _driver.Quit();
            }

            return Task.CompletedTask;
        }
    }
}
