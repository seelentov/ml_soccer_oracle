using WebApplication1.Models.Base;
using WebApplication2.Models.Stats;

namespace WebApplication2.Models
{
    public class Team: SoccerEntity
    {
        public string Name { get; set; }
        public MinStats MinStats {  get; set; } 
        public Player TopTeamScorer { get; set; }
        public Player Goalkeeper { get; set; }

    }
}
