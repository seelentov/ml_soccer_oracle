using WebApplication1.Models.Base;

namespace WebApplication2.Models
{
    public class League: SoccerEntity
    {
        public string Name { get; set; }
        public List<Game> Games { get; set; } = null!;
        public ParseType ParseType { get; set; }
        public bool Parsed { get; set; } = false;
        public int? Year { get; set; } = null!;
    }
    public enum ParseType
    {
        OnePage, TwoPage
    }
}
