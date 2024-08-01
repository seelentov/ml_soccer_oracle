using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;
using WebApplication2.Services;

namespace WebApplication2.Workers
{
    public class GamesWorker: IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly FbrefParser _fbrefParser;
        private readonly ILogger<GamesWorker> _logger;
        private readonly ChromeDriver _driver;
        public GamesWorker(IServiceScopeFactory scopeFactory, ILogger<GamesWorker> logger, FbrefParser fbrefParser, SeleniumFactory seleniumFactory)
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
                            await Cycle(scope);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex, ex.Message);
                    }

                }
            }, cancellationToken);


            return Task.CompletedTask;
        }

        private async Task Cycle(IServiceScope scope)
        {
            var leaguesService = scope.ServiceProvider.GetRequiredService<LeaguesService>();
            var gamesService = scope.ServiceProvider.GetRequiredService<GamesService>();

            _logger.LogInformation("Start GamesWorker Cycle", LogLevel.Information);

            var leagues = (await leaguesService.GetAll()).ToList();
            var games = (await gamesService.GetAll()).ToList();

            _logger.LogInformation("Get leagues", LogLevel.Information);

            foreach (var league in leagues)
            {
                if(league.Parsed && !league.Year.Contains(DateTime.UtcNow.Year.ToString()))
                {
                    continue;
                }

                var gameLinks = await _fbrefParser.GetGamesWithLinksAndResultsByLeagueLink(league.Url, _driver);

                foreach(var gameLink in gameLinks)
                {
                    if(games.FirstOrDefault(g=>g.Url == gameLink.Url) != null)
                    {
                        continue;
                    }

                    _logger.LogInformation("Parse game " + gameLink.Url, LogLevel.Information);
                    var game = await _fbrefParser.GetGameByLinkAndResult(gameLink, _driver);
                    
                    game.League = league;

                    _logger.LogInformation("Parse team " + game.Team1.Url, LogLevel.Information);

                    var team1 = await _fbrefParser.GetTeam(game.Team1, league, _driver);

                    if(team1.TopTeamScorer == null || team1.Goalkeeper == null)
                    {
                        continue;
                    }

                    _logger.LogInformation("Parse player " + team1.TopTeamScorer.Url, LogLevel.Information);

                    team1.TopTeamScorer = await _fbrefParser.GetPlayer(team1.TopTeamScorer, league, _driver);

                    _logger.LogInformation("Parse player " + team1.Goalkeeper.Url, LogLevel.Information);

                    team1.Goalkeeper = await _fbrefParser.GetPlayer(team1.Goalkeeper, league, _driver);

                    game.Team1 = team1;

                    _logger.LogInformation("Parse team " + game.Team2.Url, LogLevel.Information);

                    var team2 = await _fbrefParser.GetTeam(game.Team2, league, _driver);

                    if (team2.TopTeamScorer == null || team2.Goalkeeper == null)
                    {
                        continue;
                    }

                    _logger.LogInformation("Parse player " + team2.TopTeamScorer.Url, LogLevel.Information);

                    team2.TopTeamScorer = await _fbrefParser.GetPlayer(team2.TopTeamScorer, league, _driver);

                    _logger.LogInformation("Parse player " + team2.Goalkeeper.Url, LogLevel.Information);

                    team2.Goalkeeper = await _fbrefParser.GetPlayer(team2.Goalkeeper, league, _driver);

                    game.Team2 = team2;

                    await gamesService.UpdateOrAdd(game);

                    _logger.LogInformation("Add game " + game.Team1.Name + "|" + game.Team2.Name, LogLevel.Information);
                }

                leaguesService.CheckParsed(l => l.Url == league.Url);
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
