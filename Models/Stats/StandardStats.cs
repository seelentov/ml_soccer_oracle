using WebApplication2.Models.Base;

namespace WebApplication2.Models.Stats
{
    public class StandardStats: BaseEntity
    {
        public float MatchesPlayed { get; set; }
        public float Starts { get; set; }
        public float Minutes { get; set; }
        public float Goals { get; set; }
        public float Assists { get; set; }
        public float NonPenaltyGoals { get; set; }
        public float PenaltyMade { get; set; }
        public float PenaltyAttempted { get; set; }
        public float YellowCards { get; set; }
        public float RedCards { get; set; }

    }
}
