using OpenQA.Selenium;
using WebApplication2.Exceptions;
using WebApplication2.Models;
using WebApplication2.Models.Stats;

namespace WebApplication2.Services
{
    public class FbrefParser
    {
        private readonly string _domain;
        private readonly string _leaguesListByYearPage;
        private readonly FormatService _formatService;
        public FbrefParser(HTMLDriverFactory htmlDriverFactory, FormatService formatService)
        {
            _domain = "https://fbref.com";
            _leaguesListByYearPage = _domain + "/en/comps";
            _formatService = formatService;
        }

        public async Task<List<string>> GetAllLeaguesLinks(WebDriver driver)
        {
            driver.Navigate().GoToUrl(_leaguesListByYearPage);

            var allLeaguesLinks = new List<string>();

            var yearLinks = GetYearsLinksList(driver);

            foreach (var yearLink in yearLinks)
            {
                var links = GetYearLeaguesLinksList(yearLink, driver);

                allLeaguesLinks.AddRange(links);
            }

            return allLeaguesLinks;

        }

        public List<string> GetYearLeaguesLinksList(string url, WebDriver driver)
        {
            driver.Navigate().GoToUrl(url);

            var leaguesLinksElems = driver.FindElements(By.XPath("//table[contains(@class, 'stats_table')]/tbody/tr/th/a"));

            var leaguesLinks = new List<string>();

            foreach (var leaguesLinkElem in leaguesLinksElems)
            {
                var link = leaguesLinkElem.GetAttribute("href");
                leaguesLinks.Add(link);
            }

            return leaguesLinks;
        }

        public List<string> GetYearsLinksList(WebDriver driver)
        {
            driver.Navigate().GoToUrl(_leaguesListByYearPage);

            var leagues = new List<League>();

            var yearLinksElems = driver.FindElements(By.XPath("//div[contains(@class, 'flexindex') and contains(@class, 'wrapable')]//a")).ToList();

            yearLinksElems.Sort((a, b) => b.Text.CompareTo(a.Text));

            var yearLinks = new List<string>();

            foreach (var yearLinkElem in yearLinksElems)
            {
                var link = yearLinkElem.GetAttribute("href");

                yearLinks.Add(link);
            }

            return yearLinks;

        }

        public async Task<League?> GetLeagueData(string url, WebDriver driver)
        {
            driver.Navigate().GoToUrl(url);

            var hoversmoothElem = driver.FindElement(By.Id("inner_nav")).GetAttribute("innerHTML");

            var isHaveScoresFixtures = hoversmoothElem.Contains("Scores &amp; Fixtures");

            if (!isHaveScoresFixtures)
            {
                return null;
            }

            var h1 = driver.FindElement(By.XPath("//h1")).Text;

            var year = h1.Split(" ")[0].Trim();
            var name = h1.Replace(year, "").Replace("Stats", "").Trim();

            var league = new League()
            {
                Name = name,
                Year = year,
                Url = url
            };

            return league;
        }

        public async Task<List<Game>> GetGamesWithLinksAndResultsByLeagueLink(string url, WebDriver driver)
        {
            driver.Navigate().GoToUrl(url);

            var gamesWithLinksAndResults = new List<Game>();

            var linkElem = driver.FindElement(By.XPath("//a[contains(.,'Scores & Fixtures')]"));

            var link = linkElem.GetAttribute("href");

            driver.Navigate().GoToUrl(link);

            var gameElems = driver.FindElements(By.XPath("//table[contains(@class,'stats_table') and contains(@id, 'sched_all')]/tbody/tr[not(contains(@class, 'spacer')) and not(contains(@class, 'thead'))]"));

            foreach (var gameElem in gameElems)
            {
                var scoreElem = gameElem.FindElement(By.CssSelector("[data-stat=\"score\"] a"));

                var matchReportElem = gameElem.FindElement(By.CssSelector("[data-stat=\"match_report\"] a"));

                var team1Elem = gameElem.FindElement(By.CssSelector("[data-stat=\"home_team\"] a"));
                var team2Elem = gameElem.FindElement(By.CssSelector("[data-stat=\"away_team\"] a"));

                var gameUrl = matchReportElem.GetAttribute("href");

                float? result = null;

                if (scoreElem.Text != "")
                {
                    var scores = scoreElem.Text.Split("–");
                    var score1 = _formatService.ToFloat(scores[0]);
                    var score2 = _formatService.ToFloat(scores[1]);

                    result = score1 == score2 ? 0.0f : score1 > score2 ? 1.0f : -1.0f;
                }

                var team1 = new Team()
                {
                    Name = team1Elem.Text,
                    Url = team1Elem.GetAttribute("href")
                };

                var team2 = new Team()
                {
                    Name = team2Elem.Text,
                    Url = team2Elem.GetAttribute("href")
                };

                var dateElem = gameElem.FindElement(By.CssSelector("[data-stat=\"date\"] a"));

                var dateString = dateElem.Text;

                var timeElemChecker = gameElem.FindElements(By.CssSelector(".venuetime"));

                var dateTime = _formatService.ParseDateTime(dateString + " " + "00:00", "yyyy-MM-dd HH:mm");

                if (timeElemChecker.Count > 0)
                {
                    dateTime = _formatService.ParseDateTime(dateString + " " + timeElemChecker[0].Text, "yyyy-MM-dd HH:mm");
                }

                var game = new Game()
                {
                    Result = result,
                    Url = gameUrl,
                    Team1 = team1,
                    Team2 = team2,
                    MatchDate = dateTime
                };

                gamesWithLinksAndResults.Add(game);
            }

            return gamesWithLinksAndResults;
        }

        public async Task<Game> GetGameByLinkAndResult(Game GameWithLinkResultAndTeamsLinks, WebDriver driver)
        {
            driver.Navigate().GoToUrl(GameWithLinkResultAndTeamsLinks.Url);

            var headToHeadLinkChecker = driver.FindElements(By.XPath("//div[contains(@class, 'scorebox_meta')]//a[contains(.,'History')]"));

            if (headToHeadLinkChecker.Count < 1)
            {
                headToHeadLinkChecker = driver.FindElements(By.XPath("//div[contains(@class, 'scorebox_meta')]//a[contains(.,'Head-to-Head')]"));
            }


            if (headToHeadLinkChecker.Count > 0)
            {
                var headToHeadLink = headToHeadLinkChecker[0].GetAttribute("href");

                driver.Navigate().GoToUrl(headToHeadLink);
            }


            var gamesHistoryTableElem = driver.FindElement(By.XPath("//div[contains(@id,'div_games_history_all')]"));

            if (gamesHistoryTableElem == null)
            {
                throw new ParserException("Cant get Head-To-Head table " + GameWithLinkResultAndTeamsLinks.Url);
            }

            var gamesHistoryTableMoreBtnCheck = driver.FindElements(By.XPath("//div[contains(@id,'all_games_history')]//button[contains(.,'Show hidden rows')]"));

            if(gamesHistoryTableMoreBtnCheck.Count > 0)
            {
                driver.ExecuteScript("document.querySelector('#all_games_history #div_games_history_all_control')?.click()");
                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            var gamesHistoryElems = gamesHistoryTableElem.FindElements(By.XPath("//div[contains(@id,'div_games_history_all')]//table/tbody/tr[not(contains(@class, 'spacer')) and not(contains(@class, 'thead'))]"));

            var gamesCount = gamesHistoryElems.Count;

            var headToHeadStats = new HeadToHeadStats();

            int goals = 0;
            List<int> differences = new();

            foreach (var gameElem in gamesHistoryElems)
            {
                var scoreElemCheck = gameElem.FindElements(By.CssSelector("[data-stat=\"score\"] a"));

                if(scoreElemCheck.Count < 1)
                {
                    continue;
                }

                var scoreElem = gameElem.FindElement(By.CssSelector("[data-stat=\"score\"] a"));

                if (scoreElem.Text == "")
                {
                    continue;
                }

                var scores = scoreElem.Text.Contains("–") ? scoreElem.Text.Split("–") : scoreElem.Text.Split("–");
                var score1 = _formatService.ToInt(scores[0]);
                var score2 = _formatService.ToInt(scores[1]);

                goals += score1 + score2;
                differences.Add(score1 - score2);

                var team1Elem = gameElem.FindElement(By.CssSelector("[data-stat=\"home_team\"] a"));
                var team2Elem = gameElem.FindElement(By.CssSelector("[data-stat=\"away_team\"] a"));

                var calcLeftTeam = await CalcLeftTeam(GameWithLinkResultAndTeamsLinks, team1Elem.Text, team2Elem.Text);

                var isLeftTeam1 = calcLeftTeam.isLeftTeam1;
                var isLeftTeam2 = calcLeftTeam.isLeftTeam2;

                if (!isLeftTeam1 && !isLeftTeam2)
                {
                    throw new ParserException("Cant get actual team in " + GameWithLinkResultAndTeamsLinks.Url);
                }

                if (score1 == score2)
                {
                    headToHeadStats.Draws++;
                }
                else if (score1 > score2)
                {
                    if (isLeftTeam1)
                    {
                        headToHeadStats.WinsTeam1++;
                    }
                    else
                    {
                        headToHeadStats.WinsTeam2++;
                    }
                }
                else
                {
                    if (isLeftTeam1)
                    {
                        headToHeadStats.WinsTeam2++;
                    }
                    else
                    {
                        headToHeadStats.WinsTeam1++;
                    }
                }
            }

            if (gamesCount != 0)
            {
                headToHeadStats.AverageGoals = (float)goals / gamesCount;
                headToHeadStats.AverageGoalDifference = (float)differences.Sum() / gamesCount;
            }

            GameWithLinkResultAndTeamsLinks.HeadToHeadStats = headToHeadStats;

            return GameWithLinkResultAndTeamsLinks;
        }


        public struct FbrefParserCalcPlayersResult
        {
            public bool isLeftTeam1;
            public bool isLeftTeam2;
        }
        public async Task<FbrefParserCalcPlayersResult> CalcLeftTeam(Game gameWithLinkResultAndTeamsLinks, string teamName, string team2Name)
        {
            var isLeftTeam1 = teamName.Contains(gameWithLinkResultAndTeamsLinks.Team1.Name) || gameWithLinkResultAndTeamsLinks.Team1.Name.Contains(teamName);
            var isLeftTeam2 = teamName.Contains(gameWithLinkResultAndTeamsLinks.Team2.Name) || gameWithLinkResultAndTeamsLinks.Team2.Name.Contains(teamName);

            if(isLeftTeam1 || isLeftTeam2)
            {
                return new FbrefParserCalcPlayersResult { isLeftTeam1 = isLeftTeam1, isLeftTeam2 = isLeftTeam2 };
            }

            isLeftTeam1 = team2Name.Contains(gameWithLinkResultAndTeamsLinks.Team1.Name) || gameWithLinkResultAndTeamsLinks.Team1.Name.Contains(team2Name);
            isLeftTeam2 = team2Name.Contains(gameWithLinkResultAndTeamsLinks.Team2.Name) || gameWithLinkResultAndTeamsLinks.Team2.Name.Contains(team2Name);

            if (isLeftTeam1 || isLeftTeam2)
            {
                return new FbrefParserCalcPlayersResult { isLeftTeam1 = isLeftTeam1, isLeftTeam2 = isLeftTeam2 };
            }

            isLeftTeam1 = teamName.Split(" ").Any(e=>gameWithLinkResultAndTeamsLinks.Team1.Name.Contains(e)) || gameWithLinkResultAndTeamsLinks.Team1.Name.Split(" ").Any(e => teamName.Contains(e));
            isLeftTeam2 = teamName.Split(" ").Any(e=>gameWithLinkResultAndTeamsLinks.Team2.Name.Contains(e)) || gameWithLinkResultAndTeamsLinks.Team2.Name.Split(" ").Any(e => teamName.Contains(e));

            if (isLeftTeam1 || isLeftTeam2)
            {
                return new FbrefParserCalcPlayersResult { isLeftTeam1 = isLeftTeam1, isLeftTeam2 = isLeftTeam2 };
            }

            isLeftTeam1 = team2Name.Split(" ").Any(e => gameWithLinkResultAndTeamsLinks.Team1.Name.Contains(e)) || gameWithLinkResultAndTeamsLinks.Team1.Name.Split(" ").Any(e => team2Name.Contains(e));
            isLeftTeam2 = team2Name.Split(" ").Any(e => gameWithLinkResultAndTeamsLinks.Team2.Name.Contains(e)) || gameWithLinkResultAndTeamsLinks.Team2.Name.Split(" ").Any(e => team2Name.Contains(e));

            if (isLeftTeam1 || isLeftTeam2)
            {
                return new FbrefParserCalcPlayersResult { isLeftTeam1 = isLeftTeam1, isLeftTeam2 = isLeftTeam2 };
            }

            var team1SplittedName = string.Join("", gameWithLinkResultAndTeamsLinks.Team1.Name.Split(" ").Select(e => e[0]));
            var team2SplittedName = string.Join("", gameWithLinkResultAndTeamsLinks.Team2.Name.Split(" ").Select(e => e[0]));

            isLeftTeam1 = teamName.Contains(team1SplittedName);
            isLeftTeam2 = teamName.Contains(team2SplittedName);

            if (isLeftTeam1 || isLeftTeam2)
            {
                return new FbrefParserCalcPlayersResult { isLeftTeam1 = isLeftTeam1, isLeftTeam2 = isLeftTeam2 };
            }

            isLeftTeam1 = team2Name.Contains(team1SplittedName);
            isLeftTeam2 = team2Name.Contains(team2SplittedName);

            return new FbrefParserCalcPlayersResult { isLeftTeam1 = isLeftTeam1, isLeftTeam2 = isLeftTeam2 };
        }

        public string GetSearchName(string leagueName)
        {
            var searchName = leagueName.ToLower().Contains("world cup") ? "World" : 
                leagueName.Contains("Major") ? "MLS" : 
                leagueName.Contains(" ") ? leagueName.Split(" ")[0] :
                leagueName;
            return searchName;
        }

        public IWebElement GetLeagueElem(string searchName, string leagueYear, IWebDriver driver, string xPath = "//table[contains(@class,'stats_table')]")
        {
            var leagueStatElems = driver.FindElements(By.XPath($"{xPath}/tbody/tr[contains(., '{leagueYear}') and contains(.,'{searchName}')]"));

            IWebElement leagueStatElem = null;

            foreach (var lse in leagueStatElems)
            {
                var year = lse.FindElement(By.CssSelector("[data-stat=\"year_id\"]")).Text;

                if (year == leagueYear)
                {
                    leagueStatElem = lse;
                    break;
                }
            }

            return leagueStatElem;
        }

        public async Task<Team> GetTeam(Team teamWithUrl, League league, WebDriver driver)
        {
            driver.Navigate().GoToUrl(teamWithUrl.Url);

            var scoresLink = driver.FindElement(By.XPath("//a[contains(.,'Stats & History')]")).GetAttribute("href");

            driver.Navigate().GoToUrl(scoresLink);

            var searchName = GetSearchName(league.Name);

            IWebElement leagueStatElem = GetLeagueElem(searchName, league.Year, driver);

            var matchesPlayed = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"games\"]")).Text;
            var wins = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"wins\"]")).Text;
            var draws = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"ties\"]")).Text;
            var losses = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"losses\"]")).Text;
            var goalsFor = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"goals_for\"]")).Text;
            var goalsAgainst = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"goals_against\"]")).Text;
            var goalsDifference = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"goal_diff\"]")).Text.Replace("+", "");
            var points = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"points\"]")).Text;

            var minStats = new MinStats()
            {
                MatchesPlayed = _formatService.ToFloat(matchesPlayed),
                Wins = _formatService.ToFloat(wins),
                Draws = _formatService.ToFloat(draws),
                Loses = _formatService.ToFloat(losses),
                GoalsFor = _formatService.ToFloat(goalsFor),
                GoalsAgainst = _formatService.ToFloat(goalsAgainst),
                GoalsDifference = _formatService.ToFloat(goalsDifference),
                Points = _formatService.ToFloat(points),
            };

            var topTeamScorerChecker = leagueStatElem.FindElements(By.CssSelector("[data-stat=\"top_team_scorers\"] a"));

            if (topTeamScorerChecker.Count > 0)
            {
                teamWithUrl.TopTeamScorer = new Player() { Url = topTeamScorerChecker[0].GetAttribute("href") };
            }

            var goalkeeperChecker = leagueStatElem.FindElements(By.CssSelector("[data-stat=\"top_keeper\"] a"));

            if (goalkeeperChecker.Count > 0)
            {
                teamWithUrl.Goalkeeper = new Player() { Url = goalkeeperChecker[0].GetAttribute("href") };
            }

            teamWithUrl.MinStats = minStats;

            return teamWithUrl;
        }

        public async Task<Player> GetPlayer(Player playerWithLink, League league, WebDriver driver)
        {
            driver.Navigate().GoToUrl(playerWithLink.Url);

            var scoresLinkChecher = driver.FindElements(By.XPath("//a[contains(.,'All Competitions')]"));

            IWebElement leagueStatElem = null;

            var name = playerWithLink.Url.Split("/").Last().Replace("-", " ");

            var searchName = GetSearchName(league.Name);

            if (scoresLinkChecher.Count > 0)
            {
                var scoresLink = scoresLinkChecher[0].GetAttribute("href");

                driver.Navigate().GoToUrl(scoresLink);
            }

            leagueStatElem = GetLeagueElem(searchName, league.Year, driver, "//div[contains(@id, \"all_stats_standard\")]//table[contains(@class,'stats_table')]");

            var age = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"age\"]")).Text;

            var matchesPlayed = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"games\"]")).Text;
            var starts = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"games_starts\"]")).Text;
            var minutes = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"minutes\"]")).Text;
            var goals = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"goals\"]")).Text;
            var assists = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"assists\"]")).Text;
            var nonPenaltyGoals = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"goals_pens\"]")).Text;
            var penaltyMade = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"pens_made\"]")).Text;
            var penaltyAttempted = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"pens_att\"]")).Text;
            var yellowCards = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"cards_yellow\"]")).Text;
            var redCards = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"cards_red\"]")).Text;

            var standardStats = new StandardStats()
            {
                MatchesPlayed = _formatService.ToFloat(matchesPlayed),
                Starts = _formatService.ToFloat(starts),
                Minutes = _formatService.ToFloat(minutes),
                Goals = _formatService.ToFloat(goals),
                Assists = _formatService.ToFloat(assists),
                NonPenaltyGoals = _formatService.ToFloat(nonPenaltyGoals),
                PenaltyMade = _formatService.ToFloat(penaltyMade),
                PenaltyAttempted = _formatService.ToFloat(penaltyAttempted),
                YellowCards = _formatService.ToFloat(yellowCards),
                RedCards = _formatService.ToFloat(redCards)
            };

            var nineteenMinutes = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"minutes_90s\"]")).Text;
            var nineteenGoals = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"goals_per90\"]")).Text;
            var nineteenAssists = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"assists_per90\"]")).Text;
            var nineteenNonPenaltyGoals = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"goals_pens_per90\"]")).Text;
            var nineteenPenaltyAssists = leagueStatElem.FindElement(By.CssSelector("[data-stat=\"goals_assists_pens_per90\"]")).Text;

            var nineteenStats = new NineteenStats()
            {
                Minutes = _formatService.ToFloat(nineteenMinutes),
                Goals = _formatService.ToFloat(nineteenGoals),
                Assists = _formatService.ToFloat(nineteenAssists),
                NonPenaltyGoals = _formatService.ToFloat(nineteenNonPenaltyGoals),
                PenaltyAssists = _formatService.ToFloat(nineteenPenaltyAssists),
            };

            playerWithLink.StandardStats = standardStats;
            playerWithLink.NineteenStats = nineteenStats;
            playerWithLink.Age = _formatService.ToFloat(age);
            playerWithLink.Name = name;

            return playerWithLink;
        }
    }
}
