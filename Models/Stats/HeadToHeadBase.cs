using WebApplication2.Models.Base;

namespace WebApplication2.Models.Stats
{
    public class HeadToHeadBase : BaseEntity
    {
        public float Matches { get; set; } = 0.0f;
        public float RestHours { get; set; } = 0.0f;
        public float Wins { get; set; } = 0.0f;
        public float Draws { get; set; } = 0.0f;
        public float Loses { get; set; } = 0.0f;
        public float Goals { get; set; } = 0.0f;
        public float GoalsLost { get; set; } = 0.0f;
        public float GoalsGame { get; set; } = 0.0f;
        public float GoalsGameLost { get; set; } = 0.0f;
        public float DryMatches { get; set; } = 0.0f;
        public float GoalBoth { get; set; } = 0.0f;
        public float TotalMore25 { get; set; } = 0.0f;
        public float TotalLess25 { get; set; } = 0.0f;

    }
}
