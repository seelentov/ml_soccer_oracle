using WebApplication1.Models.Base;
using WebApplication2.Models.Stats;

namespace WebApplication2.Models
{
    public class Player: SoccerEntity
    {
        public string Name { get; set; }
        public float Age { get; set; }
        public StandardStats StandardStats {  get; set; }
        public NineteenStats NineteenStats { get; set; }
    }
}
