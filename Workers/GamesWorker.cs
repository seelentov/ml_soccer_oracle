﻿
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WebApplication2.Data;
using WebApplication2.Services;
using static WebApplication2.Services.Soccer365Parser;

namespace WebApplication2.Workers
{
    public class GamesWorker : IHostedService
    {
        private readonly ILogger<GamesWorker> _logger;
        private readonly Soccer365Parser _soccer365parser;
        private readonly IWebDriver _driver;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly MethodOptions _options;

        public GamesWorker(ILogger<GamesWorker> logger, Soccer365Parser soccer365parser, SeleniumFactory selenium, IServiceScopeFactory scopeFactory)
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
                            var gamesService = scope.ServiceProvider.GetRequiredService<GamesService>();

                            var leagues = (await leaguesService.GetAll()).ToList();

                            leagues.Reverse();

                            foreach (var league in leagues)
                            {
                                if (league.Parsed && league.Year != null && league.Year < DateTime.Now.Year)
                                {
                                    continue;
                                }

                                int? year = null!;

                                var gameLinks = await _soccer365parser.GetGameLinks(league, _options);

                                foreach (var gameLink in gameLinks)
                                {
                                    var gameChecker = await gamesService.Get(g => g.Url == gameLink);

                                    if (gameChecker != null && (gameChecker.MatchDate.Year < DateTime.Now.Year || gameChecker.UpdatedAt > DateTime.UtcNow.AddHours(-1)))
                                    {
                                        _logger.LogInformation("Skip " + gameChecker.Url, Microsoft.Extensions.Logging.LogLevel.Information);
                                        continue;
                                    }

                                    var game = await _soccer365parser.GetGameByLink(gameLink, _options);

                                    if (game != null)
                                    {
                                        game.League = league;

                                        if (year == null || year < game.MatchDate.Year)
                                        {
                                            year = game.MatchDate.Year;
                                        }

                                        await gamesService.UpdateOrAdd(game);
                                        _logger.LogInformation(game.Team1.Name + " | " + game.Team2.Name + " add in DB (" + game.Url + ")", Microsoft.Extensions.Logging.LogLevel.Information);
                                    }
                                }

                                leaguesService.CheckParsed(l => l.Url == league.Url);

                                leaguesService.UpdateYear(l => l.Url == league.Url, year);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation(ex, ex.Message, Microsoft.Extensions.Logging.LogLevel.Error);
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