using WebApplication2.Exceptions;
using WebApplication2.Models;
using WebApplication2.Models.ML;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.Services
{
    public class MLGameAdapter : IAdapter<Game, MLGame>
    {
        public MLGameAdapter() { }

        public MLGame Adapt(Game game)
        {
            var MLGame = new MLGame();

            MLGame.LeagueName = game.League.Name;

            MLGame.MatchDate = game.MatchDate;  
            MLGame.Day = (int)game.MatchDate.DayOfWeek;  
            MLGame.Month = game.MatchDate.Month;  
            MLGame.Year = game.MatchDate.Year;

            MLGame.Result = game.Result ?? 0.0f;
            MLGame.HeadToHeadWinsTeam1 = game.HeadToHeadStats.WinsTeam1;
            MLGame.HeadToHeadWinsTeam2 = game.HeadToHeadStats.WinsTeam2;
            MLGame.HeadToHeadDraws = game.HeadToHeadStats.Draws;
            MLGame.AverageGoalsInHeadToHead = game.HeadToHeadStats.AverageGoals;
            MLGame.AverageGoalDifferenceInHeadToHead = game.HeadToHeadStats.AverageGoalDifference;

            MLGame.Team1Name = game.Team1.Name;
            MLGame.Team1MatchesPlayed = game.Team1.MinStats.MatchesPlayed;
            MLGame.Team1Wins = game.Team1.MinStats.Wins;
            MLGame.Team1Draws = game.Team1.MinStats.Draws;
            MLGame.Team1Loses = game.Team1.MinStats.Loses;
            MLGame.Team1GoalsFor = game.Team1.MinStats.GoalsFor;
            MLGame.Team1GoalsAgainst = game.Team1.MinStats.GoalsAgainst;
            MLGame.Team1GoalsDifference = game.Team1.MinStats.GoalsDifference;
            MLGame.Team1Points = game.Team1.MinStats.Points;

            MLGame.Team2Name = game.Team2.Name;
            MLGame.Team2MatchesPlayed = game.Team2.MinStats.MatchesPlayed;
            MLGame.Team2Wins = game.Team2.MinStats.Wins;
            MLGame.Team2Draws = game.Team2.MinStats.Draws;
            MLGame.Team2Loses = game.Team2.MinStats.Loses;
            MLGame.Team2GoalsFor = game.Team2.MinStats.GoalsFor;
            MLGame.Team2GoalsAgainst = game.Team2.MinStats.GoalsAgainst;
            MLGame.Team2GoalsDifference = game.Team2.MinStats.GoalsDifference;
            MLGame.Team2Points = game.Team2.MinStats.Points;


            MLGame.Team1TopTeamScorerName = game.Team1.TopTeamScorer.Name;
            MLGame.Team1TopTeamScorerAge = game.Team1.TopTeamScorer.Age;
            MLGame.Team1TopTeamScorerMatchesPlayed = game.Team1.TopTeamScorer.StandardStats.MatchesPlayed;
            MLGame.Team1TopTeamScorerStarts = game.Team1.TopTeamScorer.StandardStats.Starts;
            MLGame.Team1TopTeamScorerMinutes = game.Team1.TopTeamScorer.StandardStats.Minutes;
            MLGame.Team1TopTeamScorerNineteenPlayed = game.Team1.TopTeamScorer.NineteenStats.Minutes;
            MLGame.Team1TopTeamScorerGoals = game.Team1.TopTeamScorer.StandardStats.Goals;
            MLGame.Team1TopTeamScorerAssists = game.Team1.TopTeamScorer.StandardStats.Assists;
            MLGame.Team1TopTeamScorerNonPenaltyGoals = game.Team1.TopTeamScorer.StandardStats.NonPenaltyGoals;
            MLGame.Team1TopTeamScorerPenaltyMade = game.Team1.TopTeamScorer.StandardStats.PenaltyMade;
            MLGame.Team1TopTeamScorerPenaltyAttempted = game.Team1.TopTeamScorer.StandardStats.PenaltyAttempted;
            MLGame.Team1TopTeamScorerYellowCards = game.Team1.TopTeamScorer.StandardStats.YellowCards;
            MLGame.Team1TopTeamScorerRedCards = game.Team1.TopTeamScorer.StandardStats.RedCards;
            MLGame.Team1TopTeamScorerNineteenGoals = game.Team1.TopTeamScorer.NineteenStats.Goals;
            MLGame.Team1TopTeamScorerNineteenAssists = game.Team1.TopTeamScorer.NineteenStats.Assists;
            MLGame.Team1TopTeamScorerNineteenNonPenaltyGoals = game.Team1.TopTeamScorer.NineteenStats.NonPenaltyGoals;
            MLGame.Team1TopTeamScorerNineteenPenaltyAssists = game.Team1.TopTeamScorer.NineteenStats.PenaltyAssists;

            MLGame.Team1GoalkeeperName = game.Team1.Goalkeeper.Name;
            MLGame.Team1GoalkeeperAge = game.Team1.Goalkeeper.Age;
            MLGame.Team1GoalkeeperMatchesPlayed = game.Team1.Goalkeeper.StandardStats.MatchesPlayed;
            MLGame.Team1GoalkeeperStarts = game.Team1.Goalkeeper.StandardStats.Starts;
            MLGame.Team1GoalkeeperMinutes = game.Team1.Goalkeeper.StandardStats.Minutes;
            MLGame.Team1GoalkeeperNineteenPlayed = game.Team1.Goalkeeper.NineteenStats.Minutes;
            MLGame.Team1GoalkeeperGoals = game.Team1.Goalkeeper.StandardStats.Goals;
            MLGame.Team1GoalkeeperAssists = game.Team1.Goalkeeper.StandardStats.Assists;
            MLGame.Team1GoalkeeperNonPenaltyGoals = game.Team1.Goalkeeper.StandardStats.NonPenaltyGoals;
            MLGame.Team1GoalkeeperPenaltyMade = game.Team1.Goalkeeper.StandardStats.PenaltyMade;
            MLGame.Team1GoalkeeperPenaltyAttempted = game.Team1.Goalkeeper.StandardStats.PenaltyAttempted;
            MLGame.Team1GoalkeeperYellowCards = game.Team1.Goalkeeper.StandardStats.YellowCards;
            MLGame.Team1GoalkeeperRedCards = game.Team1.Goalkeeper.StandardStats.RedCards;
            MLGame.Team1GoalkeeperNineteenGoals = game.Team1.Goalkeeper.NineteenStats.Goals;
            MLGame.Team1GoalkeeperNineteenAssists = game.Team1.Goalkeeper.NineteenStats.Assists;
            MLGame.Team1GoalkeeperNineteenNonPenaltyGoals = game.Team1.Goalkeeper.NineteenStats.NonPenaltyGoals;
            MLGame.Team1GoalkeeperNineteenPenaltyAssists = game.Team1.Goalkeeper.NineteenStats.PenaltyAssists;

            MLGame.Team2TopTeamScorerName = game.Team2.TopTeamScorer.Name;
            MLGame.Team2TopTeamScorerAge = game.Team2.TopTeamScorer.Age;
            MLGame.Team2TopTeamScorerMatchesPlayed = game.Team2.TopTeamScorer.StandardStats.MatchesPlayed;
            MLGame.Team2TopTeamScorerStarts = game.Team2.TopTeamScorer.StandardStats.Starts;
            MLGame.Team2TopTeamScorerMinutes = game.Team2.TopTeamScorer.StandardStats.Minutes;
            MLGame.Team2TopTeamScorerNineteenPlayed = game.Team2.TopTeamScorer.NineteenStats.Minutes;
            MLGame.Team2TopTeamScorerGoals = game.Team2.TopTeamScorer.StandardStats.Goals;
            MLGame.Team2TopTeamScorerAssists = game.Team2.TopTeamScorer.StandardStats.Assists;
            MLGame.Team2TopTeamScorerNonPenaltyGoals = game.Team2.TopTeamScorer.StandardStats.NonPenaltyGoals;
            MLGame.Team2TopTeamScorerPenaltyMade = game.Team2.TopTeamScorer.StandardStats.PenaltyMade;
            MLGame.Team2TopTeamScorerPenaltyAttempted = game.Team2.TopTeamScorer.StandardStats.PenaltyAttempted;
            MLGame.Team2TopTeamScorerYellowCards = game.Team2.TopTeamScorer.StandardStats.YellowCards;
            MLGame.Team2TopTeamScorerRedCards = game.Team2.TopTeamScorer.StandardStats.RedCards;
            MLGame.Team2TopTeamScorerNineteenGoals = game.Team2.TopTeamScorer.NineteenStats.Goals;
            MLGame.Team2TopTeamScorerNineteenAssists = game.Team2.TopTeamScorer.NineteenStats.Assists;
            MLGame.Team2TopTeamScorerNineteenNonPenaltyGoals = game.Team2.TopTeamScorer.NineteenStats.NonPenaltyGoals;
            MLGame.Team2TopTeamScorerNineteenPenaltyAssists = game.Team2.TopTeamScorer.NineteenStats.PenaltyAssists;

            MLGame.Team2GoalkeeperName = game.Team2.Goalkeeper.Name;
            MLGame.Team2GoalkeeperAge = game.Team2.Goalkeeper.Age;
            MLGame.Team2GoalkeeperMatchesPlayed = game.Team2.Goalkeeper.StandardStats.MatchesPlayed;
            MLGame.Team2GoalkeeperStarts = game.Team2.Goalkeeper.StandardStats.Starts;
            MLGame.Team2GoalkeeperMinutes = game.Team2.Goalkeeper.StandardStats.Minutes;
            MLGame.Team2GoalkeeperNineteenPlayed = game.Team2.Goalkeeper.NineteenStats.Minutes;
            MLGame.Team2GoalkeeperGoals = game.Team2.Goalkeeper.StandardStats.Goals;
            MLGame.Team2GoalkeeperAssists = game.Team2.Goalkeeper.StandardStats.Assists;
            MLGame.Team2GoalkeeperNonPenaltyGoals = game.Team2.Goalkeeper.StandardStats.NonPenaltyGoals;
            MLGame.Team2GoalkeeperPenaltyMade = game.Team2.Goalkeeper.StandardStats.PenaltyMade;
            MLGame.Team2GoalkeeperPenaltyAttempted = game.Team2.Goalkeeper.StandardStats.PenaltyAttempted;
            MLGame.Team2GoalkeeperYellowCards = game.Team2.Goalkeeper.StandardStats.YellowCards;
            MLGame.Team2GoalkeeperRedCards = game.Team2.Goalkeeper.StandardStats.RedCards;
            MLGame.Team2GoalkeeperNineteenGoals = game.Team2.Goalkeeper.NineteenStats.Goals;
            MLGame.Team2GoalkeeperNineteenAssists = game.Team2.Goalkeeper.NineteenStats.Assists;
            MLGame.Team2GoalkeeperNineteenNonPenaltyGoals = game.Team2.Goalkeeper.NineteenStats.NonPenaltyGoals;
            MLGame.Team2GoalkeeperNineteenPenaltyAssists = game.Team2.Goalkeeper.NineteenStats.PenaltyAssists;

            return MLGame;
        }
        public IEnumerable<MLGame> AdaptRange(IEnumerable<Game> games)
        {
            var MLGame = new List<MLGame>();

            foreach (var game in games)
            {
                var adaptedGame = Adapt(game);
                MLGame.Add(adaptedGame);
            }

            return MLGame;
        }

    }
}
