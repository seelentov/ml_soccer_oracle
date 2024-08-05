using Microsoft.Extensions.FileSystemGlobbing;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Text.RegularExpressions;
using WebApplication2.Models;
using WebApplication2.Models.Stats;

namespace WebApplication2.Services
{
    public class Soccer365Parser
    {
        private readonly string _domain;
        private readonly string _leaguesLink;
        private readonly string _tabLink;
        private readonly ILogger<Soccer365Parser> _logger;
        private readonly FormatService _formatService;
        public Soccer365Parser(ILogger<Soccer365Parser> logger, FormatService formatService)
        {
            _domain = "https://soccer365.ru";
            _leaguesLink = _domain + "/competitions";
            _tabLink = "&tab=form_teams";

            _logger = logger;
            _formatService = formatService;
        }

        public void Log(string message)
        {
            _logger.LogInformation(message, Microsoft.Extensions.Logging.LogLevel.Information);
        }

        public struct MethodOptions
        {
            public IWebDriver driver;
            public WebDriverWait wait;
        }

        public async Task<List<League>> GetLeagues(MethodOptions options)
        {
            var driver = options.driver;
            var wait = options.wait;

            Log("Load leagues page");

            driver.Navigate().GoToUrl(_leaguesLink);

            Log("Loaded");

            var leagues = new List<League>();

            int page = 1;

            while (true)
            {
                wait.Until(d => d.FindElements(By.CssSelector(".pager a")).Count > 0);

                var pager = driver.FindElements(By.CssSelector(".pager a")).Last();

                Log("Page " + page);

                wait.Until(d => d.FindElements(By.CssSelector(".season_item")).Count > 0);

                var seasonItems = driver.FindElements(By.CssSelector(".season_item"));

                foreach (var seasonItem in seasonItems)
                {
                    var url = seasonItem.FindElement(By.CssSelector("a.title")).GetAttribute("href");

                    var parseType = url.Contains("id") ? ParseType.OnePage : ParseType.TwoPage;

                    var name = seasonItem.FindElement(By.CssSelector("a.title span")).Text;

                    var league = new League()
                    {
                        Name = name,
                        Url = url,
                        ParseType = parseType
                    };

                    Log("Parse " + league.Name);

                    leagues.Add(league);
                }

                if (!pager.GetAttribute("innerHTML").Contains("»"))
                {
                    Log("Stop parse leagues");
                    break;
                }
                else
                {
                    pager.Click();
                    page++;
                }

            }

            return leagues;
        }

        public async Task<List<League>> GetNestedLeagues(League parentLeague, MethodOptions options)
        {
            Log("Parse nested" + parentLeague.Name);

            options.driver.Navigate().GoToUrl(parentLeague.Url);

            var leagues = new HashSet<League>();

            var selectboxesElems = options.driver.FindElements(By.CssSelector(".selectbox"));

            if (selectboxesElems.Count == 1)
            {
                var nestedLeaguesLinks = selectboxesElems[0].FindElements(By.CssSelector(".selectbox a"));

                foreach (var nestedLeaguesLink in nestedLeaguesLinks)
                {
                    var link = nestedLeaguesLink.GetAttribute("href");

                    var league = new League()
                    {
                        Url = link,
                        Name = parentLeague.Name,
                        ParseType = parentLeague.ParseType
                    };

                    Log("Parsed " + league.Name + league.Url);

                    leagues.Add(league);
                }
            }
            else if (selectboxesElems.Count == 2)
            {
                var tabbedLeagues = new HashSet<League>();

                var tabbedLeaguesLinks = selectboxesElems[0].FindElements(By.CssSelector(".selectbox a"));
                var nestedLeaguesLinks = selectboxesElems[1].FindElements(By.CssSelector(".selectbox a"));

                foreach (var nestedLeaguesLink in nestedLeaguesLinks)
                {
                    var link = nestedLeaguesLink.GetAttribute("href");

                    var league = new League()
                    {
                        Url = link,
                        Name = parentLeague.Name,
                        ParseType = parentLeague.ParseType
                    };

                    Log("Parsed " + league.Name + league.Url);

                    leagues.Add(league);
                }

                foreach (var nestedLeaguesLink in tabbedLeaguesLinks)
                {
                    var link = nestedLeaguesLink.GetAttribute("href");
                    var html = nestedLeaguesLink.GetAttribute("innerHTML");

                    var league = new League()
                    {
                        Url = link,
                        Name = _formatService.ClearString(html),
                        ParseType = parentLeague.ParseType
                    };

                    Log("Parsed " + league.Name + " " + league.Url);

                    tabbedLeagues.Add(league);
                    leagues.Add(league);
                }

                foreach (var tabbedLeague in tabbedLeagues)
                {
                    var tabbedNestedLeagues = await GetTabbedLeagues(tabbedLeague, options);

                    if(tabbedNestedLeagues != null)
                    {
                        foreach (var tabbedNestedLeague in tabbedNestedLeagues)
                        {
                            leagues.Add(tabbedNestedLeague);
                        }
                    }
                }
            }

            return leagues.ToList();

        }

        public async Task<List<League>?> GetTabbedLeagues(League parentLeague, MethodOptions options)
        {
            Log("Parse tabbed nested" + parentLeague.Name);

            options.driver.Navigate().GoToUrl(parentLeague.Url);

            var leagues = new HashSet<League>();


            var selectboxesElems = options.driver.FindElements(By.CssSelector(".selectbox"));
            var name = selectboxesElems[0].FindElement(By.CssSelector(".selectbox-label")).Text;

            if (selectboxesElems.Count < 2)
            {
                return null;
            }

            var nestedLeaguesLinks = selectboxesElems[1].FindElements(By.CssSelector(".selectbox a"));

            foreach (var nestedLeaguesLink in nestedLeaguesLinks)
            {
                var link = nestedLeaguesLink.GetAttribute("href");

                var league = new League()
                {
                    Url = link,
                    Name = name,
                    ParseType = parentLeague.ParseType
                };

                Log("Parsed " + league.Name + " " + league.Url);

                leagues.Add(league);
            }

            return leagues.ToList();
        }

        public async Task<List<string>> GetGameLinks(League league, MethodOptions options)
        {
            Log("Parse league links " + league.Name);

            var links = new List<string>();

            if (league.ParseType == ParseType.TwoPage)
            {
                var shedule = await GetGameLinksByPage(league.Url + "shedule/", options);

                Log("Parsed from shedule");

                var results = await GetGameLinksByPage(league.Url + "results/", options);

                Log("Parsed from results");

                foreach (var link in shedule.Concat(results))
                {
                    links.Add(link);
                }
            }
            else
            {
                var parsedLinks = await GetGameLinksByPage(league.Url, options);

                Log("Parsed from all");

                links.AddRange(parsedLinks);
            }

            Log("Games: " + links.Count.ToString());

            return links;

        }


        public async Task<List<string>> GetGameLinksByPage(string url, MethodOptions options)
        {
            options.driver.Navigate().GoToUrl(url);

            var links = new List<string>();

            var gameLinkElems = options.driver.FindElements(By.CssSelector(".game_block a"));
            var gameLinkElemsCount = options.driver.FindElements(By.CssSelector(".game_block a")).Count;

            foreach (var gameLinkElem in gameLinkElems)
            {
                var link = gameLinkElem.GetAttribute("href");

                links.Add(link);

                Log(link);
            }


            return links;
        }
        public enum GetGameDataType
        {
            onlyEnouth,
            withMissedData,
            all
        }

        public async Task<Game?> GetGameByLink(string url, MethodOptions options, GetGameDataType getGameDataType = GetGameDataType.onlyEnouth)
        {
            Log("Parse game " + url);

            options.driver.Navigate().GoToUrl(url + _tabLink);

            var gameEvents = options.driver.FindElements(By.CssSelector(".live_game"));


            var team1Name = gameEvents[0].FindElement(By.CssSelector("a")).Text;
            var team1ScoreElem = gameEvents[0].FindElement(By.CssSelector(".live_game_goal span")).Text;

            var team2Name = gameEvents[1].FindElement(By.CssSelector("a")).Text;
            var team2ScoreElem = gameEvents[1].FindElement(By.CssSelector(".live_game_goal span")).Text;


            float? result = null;

            if (!team1ScoreElem.Contains("-"))
            {
                var team1Score = _formatService.ToFloat(team1ScoreElem);
                var team2Score = _formatService.ToFloat(team2ScoreElem);

                result = team1Score > team2Score ? -1 : team1Score < team2Score ? 1 : 0;
            }

            var tables = options.driver.FindElements(By.CssSelector("table.tablesorter tbody"));

            if ((getGameDataType == GetGameDataType.onlyEnouth || getGameDataType == GetGameDataType.withMissedData) && tables.Count < 2)
            {
                Log("Error on tables in game (have less 2) " + url);

                return null!;
            }

            if (getGameDataType == GetGameDataType.onlyEnouth)
            {
                foreach (var table in tables)
                {
                    if (table.GetAttribute("innerHTML").Contains("?"))
                    {
                        Log("Not enouth data in " + url);
                        return null;
                    }
                }
            }

            var dateTimeElemChecker = options.driver.FindElements(By.CssSelector(".block_header h2"));

            var date = DateTime.Now;

            if (dateTimeElemChecker.Count > 0)
            {
                var dateTimeElem = dateTimeElemChecker[0];

                var dateString = dateTimeElem.Text.Split(',').Last();

                try
                {
                    date = _formatService.ParseDateTime(dateString.Trim(), "dd.MM.yyyy HH:mm");
                }
                catch
                {
                    date = DateTime.Now;
                    result = null;
                }

                try
                {
                    date = _formatService.ParseDateTime(dateString.Trim(), "dd.MM.yyyy");
                }
                catch
                {
                    date = DateTime.Now;
                    result = null;
                }
            }

            var headToHeadBases = (await GetHeadToHeadBase(tables[0]));

            Log("Parsed HeadToheadBase ");

            var team1headToHeadBase = headToHeadBases[0];
            var team2headToHeadBase = headToHeadBases[1];


            var team1headToHeadInGame = new HeadToHeadInGame();
            var team2headToHeadInGame = new HeadToHeadInGame();
            var team1headToHeadInGameOpponent = new HeadToHeadInGame();
            var team2headToHeadInGameOpponent = new HeadToHeadInGame();


            if (tables.Count == 2)
            {
                var headToHeadInGame = (await GetHeadToHeadInGame(tables[1]));

                Log("Parsed HeadToHeadInGame");

                team1headToHeadInGame = headToHeadInGame[0];
                team2headToHeadInGame = headToHeadInGame[1];

                var headToHeadInGameOpponent = (await GetHeadToHeadInGame(tables[1], GetHeadToHeadInGameType.Enemy));

                Log("Parsed HeadToHeadInGameOpponent");

                team1headToHeadInGameOpponent = headToHeadInGameOpponent[0];
                team2headToHeadInGameOpponent = headToHeadInGameOpponent[1];
            }



            Log("Parsed Game " + team1Name + " | " + team2Name);

            return new Game()
            {
                Team1 = new Team()
                {
                    Name = team1Name,
                    HeadToHeadBase = team1headToHeadBase,
                    HeadToHeadInGame = team1headToHeadInGame,
                    HeadToHeadInGameOpponent = team1headToHeadInGameOpponent,
                    Url = url,
                },
                Team2 = new Team()
                {
                    Name = team2Name,
                    HeadToHeadBase = team2headToHeadBase,
                    HeadToHeadInGame = team2headToHeadInGame,
                    HeadToHeadInGameOpponent = team2headToHeadInGameOpponent,
                    Url = url,
                },
                MatchDate = date,
                Result = result,
                Url = url,
            };

        }

        public async Task<HeadToHeadBase[]> GetHeadToHeadBase(IWebElement table)
        {
            var team1headToHeadBase = new HeadToHeadBase();
            var team2headToHeadBase = new HeadToHeadBase();

            var lines = table.FindElements(By.CssSelector("tr"));



            if (!lines[0].GetAttribute("innerHTML").Contains("?"))
            {
                var matches = lines[0];
                var matchesTds = matches.FindElements(By.CssSelector("td"));
                team1headToHeadBase.Matches = _formatService.ToFloat(matchesTds[0].Text);
                team2headToHeadBase.Matches = _formatService.ToFloat(matchesTds[2].Text);
            }

            if (!lines[1].GetAttribute("innerHTML").Contains("?"))
            {
                var rests = lines[1];
                var restsTds = rests.FindElements(By.CssSelector("td"));

                string pattern = @"\(([^)]+)\)";
                var rests1 = Regex.Match(restsTds[0].Text, pattern).Groups[0].Value.Replace("(", "").Replace(")", "");
                team1headToHeadBase.RestHours = _formatService.ToFloat(rests1);

                var rests2 = Regex.Match(restsTds[2].Text, pattern).Groups[0].Value.Replace("(", "").Replace(")", "");
                team2headToHeadBase.RestHours = _formatService.ToFloat(rests2);

            }

            if (!lines[2].GetAttribute("innerHTML").Contains("?"))
            {
                var wins = lines[2];
                var winsTds = wins.FindElements(By.CssSelector("td"));

                team1headToHeadBase.Wins = _formatService.ToFloat(winsTds[0].Text);
                team2headToHeadBase.Wins = _formatService.ToFloat(winsTds[2].Text);
            }

            if (!lines[3].GetAttribute("innerHTML").Contains("?"))
            {
                var draws = lines[3];
                var drawsTds = draws.FindElements(By.CssSelector("td"));

                team1headToHeadBase.Draws = _formatService.ToFloat(drawsTds[0].Text);
                team2headToHeadBase.Draws = _formatService.ToFloat(drawsTds[2].Text);
            }

            if (!lines[4].GetAttribute("innerHTML").Contains("?"))
            {
                var loses = lines[4];
                var losesTds = loses.FindElements(By.CssSelector("td"));

                team1headToHeadBase.Loses = _formatService.ToFloat(losesTds[0].Text);
                team2headToHeadBase.Loses = _formatService.ToFloat(losesTds[2].Text);
            }

            if (!lines[5].GetAttribute("innerHTML").Contains("?"))
            {
                var goals = lines[5];
                var goalsTds = goals.FindElements(By.CssSelector("td"));

                team1headToHeadBase.Goals = _formatService.ToFloat(goalsTds[0].Text);
                team2headToHeadBase.Goals = _formatService.ToFloat(goalsTds[2].Text);
            }

            if (!lines[6].GetAttribute("innerHTML").Contains("?"))
            {
                var goalsLosts = lines[6];
                var goalsLostsTds = goalsLosts.FindElements(By.CssSelector("td"));

                team1headToHeadBase.GoalsLost = _formatService.ToFloat(goalsLostsTds[0].Text);
                team2headToHeadBase.GoalsLost = _formatService.ToFloat(goalsLostsTds[2].Text);
            }

            if (!lines[7].GetAttribute("innerHTML").Contains("?"))
            {
                var goalsGame = lines[7];
                var goalsGameTds = goalsGame.FindElements(By.CssSelector("td"));

                team1headToHeadBase.GoalsGame = _formatService.ToFloat(goalsGameTds[0].Text);
                team2headToHeadBase.GoalsGame = _formatService.ToFloat(goalsGameTds[2].Text);
            }

            if (!lines[8].GetAttribute("innerHTML").Contains("?"))
            {
                var goalsGameLosts = lines[8];
                var goalsGameLostsTds = goalsGameLosts.FindElements(By.CssSelector("td"));

                team1headToHeadBase.GoalsGameLost = _formatService.ToFloat(goalsGameLostsTds[0].Text);
                team2headToHeadBase.GoalsGameLost = _formatService.ToFloat(goalsGameLostsTds[2].Text);
            }

            if (!lines[9].GetAttribute("innerHTML").Contains("?"))
            {
                var dryMatches = lines[9];
                var dryMatchesTds = dryMatches.FindElements(By.CssSelector("td"));

                team1headToHeadBase.DryMatches = _formatService.ToFloat(dryMatchesTds[0].Text);
                team2headToHeadBase.DryMatches = _formatService.ToFloat(dryMatchesTds[2].Text);
            }

            if (!lines[10].GetAttribute("innerHTML").Contains("?"))
            {
                var bothGoals = lines[10];
                var bothGoalsTds = bothGoals.FindElements(By.CssSelector("td"));

                team1headToHeadBase.GoalBoth = _formatService.ToFloat(bothGoalsTds[0].Text);
                team2headToHeadBase.GoalBoth = _formatService.ToFloat(bothGoalsTds[2].Text);
            }

            if (!lines[11].GetAttribute("innerHTML").Contains("?"))
            {
                var totalMore25 = lines[11];
                var totalMore25Tds = totalMore25.FindElements(By.CssSelector("td"));

                team1headToHeadBase.TotalMore25 = _formatService.ToFloat(totalMore25Tds[0].Text);
                team2headToHeadBase.TotalMore25 = _formatService.ToFloat(totalMore25Tds[2].Text);
            }

            if (!lines[12].GetAttribute("innerHTML").Contains("?"))
            {
                var totalLess25 = lines[12];
                var totalLess25Tds = totalLess25.FindElements(By.CssSelector("td"));

                team1headToHeadBase.TotalLess25 = _formatService.ToFloat(totalLess25Tds[0].Text);
                team2headToHeadBase.TotalLess25 = _formatService.ToFloat(totalLess25Tds[2].Text);
            }

            return [team1headToHeadBase, team2headToHeadBase];
        }

        public enum GetHeadToHeadInGameType
        {
            Own, Enemy
        }
        public async Task<HeadToHeadInGame[]?> GetHeadToHeadInGame(IWebElement table, GetHeadToHeadInGameType getHeadToHeadInGameType = GetHeadToHeadInGameType.Own)
        {
            var team1headToHeadInGame = new HeadToHeadInGame();
            var team2headToHeadInGame = new HeadToHeadInGame();

            var lines = table.FindElements(By.CssSelector("tr"));

            var isOwn = getHeadToHeadInGameType == GetHeadToHeadInGameType.Own;

            var start = isOwn ? 0 : 1;

            for (var i = start; i < lines.Count; i += 2)
            {
                var line = lines[i];

                var lineTds = line.FindElements(By.CssSelector("td"));

                var count1 = _formatService.ToFloat(lineTds[0].Text);
                var count2 = _formatService.ToFloat(lineTds[2].Text);

                if (lineTds.Any(line => line.Text == "?"))
                {
                    continue;
                }

                switch (isOwn ? i : i - 1)
                {
                    case 0:
                        team1headToHeadInGame.Strikes = count1;
                        team2headToHeadInGame.Strikes = count2;
                        break;
                    case 2:
                        team1headToHeadInGame.ShotsOnTarget = count1;
                        team2headToHeadInGame.ShotsOnTarget = count2;
                        break;
                    case 4:
                        team1headToHeadInGame.Possession = count1;
                        team2headToHeadInGame.Possession = count2;
                        break;
                    case 6:
                        team1headToHeadInGame.Corner = count1;
                        team2headToHeadInGame.Corner = count2;
                        break;
                    case 8:
                        team1headToHeadInGame.Violations = count1;
                        team2headToHeadInGame.Violations = count2;
                        break;
                    case 10:
                        team1headToHeadInGame.Offsides = count1;
                        team2headToHeadInGame.Offsides = count2;
                        break;
                    case 12:
                        team1headToHeadInGame.YellowCards = count1;
                        team2headToHeadInGame.YellowCards = count2;
                        break;
                    case 14:
                        team1headToHeadInGame.RedCards = count1;
                        team2headToHeadInGame.RedCards = count2;
                        break;
                }
            }

            return [team1headToHeadInGame, team2headToHeadInGame];
        }

    }
}
