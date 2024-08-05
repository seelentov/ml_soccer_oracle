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
            MLGame.Day = game.MatchDate.DayOfWeek.ToString();  
            MLGame.Month = game.MatchDate.Month.ToString();  
            MLGame.Year = game.MatchDate.Year.ToString();

            MLGame.Result = game.Result ?? 0.0f;

            MLGame.Team1Name = game.Team1.Name;
            MLGame.Team1Matches = game.Team1.HeadToHeadBase.Matches;
            MLGame.Team1RestHours = game.Team1.HeadToHeadBase.RestHours;
            MLGame.Team1Wins = game.Team1.HeadToHeadBase.Wins;
            MLGame.Team1Draws = game.Team1.HeadToHeadBase.Draws;
            MLGame.Team1Loses = game.Team1.HeadToHeadBase.Loses;
            MLGame.Team1Goals = game.Team1.HeadToHeadBase.Goals;
            MLGame.Team1GoalsLost = game.Team1.HeadToHeadBase.GoalsLost;
            MLGame.Team1GoalsGame = game.Team1.HeadToHeadBase.GoalsGame;
            MLGame.Team1GoalsGameLost = game.Team1.HeadToHeadBase.GoalsGameLost;
            MLGame.Team1DryMatches = game.Team1.HeadToHeadBase.DryMatches;
            MLGame.Team1GoalBoth = game.Team1.HeadToHeadBase.GoalBoth;
            MLGame.Team1TotalMore25 = game.Team1.HeadToHeadBase.TotalMore25;
            MLGame.Team1TotalLess25 = game.Team1.HeadToHeadBase.TotalLess25;

            MLGame.Team1Strikes = game.Team1.HeadToHeadInGame.Strikes;
            MLGame.Team1ShotsOnTarget = game.Team1.HeadToHeadInGame.ShotsOnTarget;
            MLGame.Team1Possession = game.Team1.HeadToHeadInGame.Possession;
            MLGame.Team1Corner = game.Team1.HeadToHeadInGame.Corner;
            MLGame.Team1Violations = game.Team1.HeadToHeadInGame.Violations;
            MLGame.Team1Offsides = game.Team1.HeadToHeadInGame.Offsides;
            MLGame.Team1RedCards = game.Team1.HeadToHeadInGame.RedCards;
            MLGame.Team1YellowCards = game.Team1.HeadToHeadInGame.YellowCards;

            MLGame.Team1StrikesOpponent = game.Team1.HeadToHeadInGameOpponent.Strikes;
            MLGame.Team1ShotsOnTargetOpponent = game.Team1.HeadToHeadInGameOpponent.ShotsOnTarget;
            MLGame.Team1PossessionOpponent = game.Team1.HeadToHeadInGameOpponent.Possession;
            MLGame.Team1CornerOpponent = game.Team1.HeadToHeadInGameOpponent.Corner;
            MLGame.Team1ViolationsOpponent = game.Team1.HeadToHeadInGameOpponent.Violations;
            MLGame.Team1OffsidesOpponent = game.Team1.HeadToHeadInGameOpponent.Offsides;
            MLGame.Team1RedCardsOpponent = game.Team1.HeadToHeadInGameOpponent.RedCards;
            MLGame.Team1YellowCardsOpponent = game.Team1.HeadToHeadInGameOpponent.YellowCards;


            MLGame.Team2Name = game.Team2.Name;
            MLGame.Team2Matches = game.Team2.HeadToHeadBase.Matches;
            MLGame.Team2RestHours = game.Team2.HeadToHeadBase.RestHours;
            MLGame.Team2Wins = game.Team2.HeadToHeadBase.Wins;
            MLGame.Team2Draws = game.Team2.HeadToHeadBase.Draws;
            MLGame.Team2Loses = game.Team2.HeadToHeadBase.Loses;
            MLGame.Team2Goals = game.Team2.HeadToHeadBase.Goals;
            MLGame.Team2GoalsLost = game.Team2.HeadToHeadBase.GoalsLost;
            MLGame.Team2GoalsGame = game.Team2.HeadToHeadBase.GoalsGame;
            MLGame.Team2GoalsGameLost = game.Team2.HeadToHeadBase.GoalsGameLost;
            MLGame.Team2DryMatches = game.Team2.HeadToHeadBase.DryMatches;
            MLGame.Team2GoalBoth = game.Team2.HeadToHeadBase.GoalBoth;
            MLGame.Team2TotalMore25 = game.Team2.HeadToHeadBase.TotalMore25;
            MLGame.Team2TotalLess25 = game.Team2.HeadToHeadBase.TotalLess25;

            MLGame.Team2Strikes = game.Team2.HeadToHeadInGame.Strikes;
            MLGame.Team2ShotsOnTarget = game.Team2.HeadToHeadInGame.ShotsOnTarget;
            MLGame.Team2Possession = game.Team2.HeadToHeadInGame.Possession;
            MLGame.Team2Corner = game.Team2.HeadToHeadInGame.Corner;
            MLGame.Team2Violations = game.Team2.HeadToHeadInGame.Violations;
            MLGame.Team2Offsides = game.Team2.HeadToHeadInGame.Offsides;
            MLGame.Team2RedCards = game.Team2.HeadToHeadInGame.RedCards;
            MLGame.Team2YellowCards = game.Team2.HeadToHeadInGame.YellowCards;

            MLGame.Team2StrikesOpponent = game.Team2.HeadToHeadInGameOpponent.Strikes;
            MLGame.Team2ShotsOnTargetOpponent = game.Team2.HeadToHeadInGameOpponent.ShotsOnTarget;
            MLGame.Team2PossessionOpponent = game.Team2.HeadToHeadInGameOpponent.Possession;
            MLGame.Team2CornerOpponent = game.Team2.HeadToHeadInGameOpponent.Corner;
            MLGame.Team2ViolationsOpponent = game.Team2.HeadToHeadInGameOpponent.Violations;
            MLGame.Team2OffsidesOpponent = game.Team2.HeadToHeadInGameOpponent.Offsides;
            MLGame.Team2RedCardsOpponent = game.Team2.HeadToHeadInGameOpponent.RedCards;
            MLGame.Team2YellowCardsOpponent = game.Team2.HeadToHeadInGameOpponent.YellowCards;


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
