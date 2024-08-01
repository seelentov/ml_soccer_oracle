using WebApplication1.Models.Base;

namespace WebApplication2.Models
{
    public class League: SoccerEntity
    {
        public string Name { get; set; }
        public string Year { get; set; }
        public List<Game> Games { get; set; } = null!;
        public bool Parsed { get; set; } = false;
    }
}
