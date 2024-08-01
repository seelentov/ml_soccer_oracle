using WebApplication2.Models.Base;

namespace WebApplication2.Models.Stats
{
    public class HeadToHeadStats: BaseEntity
    {
        public float WinsTeam1 { get; set; } = 0.0f;
        public float WinsTeam2 { get; set; } = 0.0f;
        public float Draws { get; set; } = 0.0f;
        public float AverageGoals { get; set; } = 0.0f;
        public float AverageGoalDifference { get; set; } = 0.0f;
    }
}
