using WebApplication2.Models.Base;

namespace WebApplication2.Models.Stats
{
    public class NineteenStats: BaseEntity
    {
        public float Minutes { get; set; }
        public float Goals { get; set; }
        public float Assists { get; set; }
        public float NonPenaltyGoals { get; set; }
        public float PenaltyAssists { get; set; }
    }
}
