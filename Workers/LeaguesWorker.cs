﻿
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
        private readonly IWebDriver _driver;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly MethodOptions _options;

        public LeaguesWorker(ILogger<LeaguesWorker> logger, Soccer365Parser soccer365parser, SeleniumFactory selenium, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _soccer365parser = soccer365parser;
            _driver = selenium.Get();
            _scopeFactory = scopeFactory;

            _options = new MethodOptions()
            {
                driver = _driver,
                wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10))
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
                                var nestedLeagues = await _soccer365parser.GetNestedLeagues(parsedLeague, _options);

                                foreach(var nestedLeague in nestedLeagues)
                                {
                                    await leaguesService.UpdateOrAdd(nestedLeague);
                                    _logger.LogInformation("Add nested league in DB " + nestedLeague.Name, Microsoft.Extensions.Logging.LogLevel.Information);

                                }

                                await leaguesService.UpdateOrAdd(parsedLeague);
                                _logger.LogInformation("Add league in DB " + parsedLeague.Name, Microsoft.Extensions.Logging.LogLevel.Information);
                            }

                            await Task.Delay(TimeSpan.FromDays(1));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation(ex, ex.Message, Microsoft.Extensions.Logging.LogLevel.Information);
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
                _driver.Close();
                _driver.Quit();
            }

            return Task.CompletedTask;
        }
    }
}