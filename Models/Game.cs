using WebApplication1.Models.Base;
using WebApplication2.Models.Stats;

namespace WebApplication2.Models
{
    public class Game: SoccerEntity
    {
        public League League { get; set; }
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
        public DateTime MatchDate { get; set; }
        public float? Result { get; set; } = null!;

    }
}
