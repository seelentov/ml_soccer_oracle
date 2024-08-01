using WebApplication2.Models.Base;

namespace WebApplication2.Models.Stats
{
    public class MinStats: BaseEntity
    {
        public float MatchesPlayed { get; set; }
        public float Wins { get; set; }
        public float Draws { get; set; }
        public float Loses { get; set; }
        public float GoalsFor { get; set; }
        public float GoalsAgainst { get; set; }
        public float GoalsDifference { get; set; }
        public float Points { get; set; }

    }
}
