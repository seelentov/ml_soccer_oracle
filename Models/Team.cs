using WebApplication1.Models.Base;
using WebApplication2.Models.Stats;

namespace WebApplication2.Models
{
    public class Team: SoccerEntity
    {
        public string Name { get; set; }

        public HeadToHeadBase HeadToHeadBase { get; set; }
        public HeadToHeadInGame HeadToHeadInGame { get; set; }
        public HeadToHeadInGame HeadToHeadInGameOpponent { get; set; }
    }
}
