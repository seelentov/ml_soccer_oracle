using WebApplication2.Models.Base;

namespace WebApplication2.Models.Stats
{
    public class HeadToHeadInGame: BaseEntity
    {
        public float Strikes { get; set; } = 0.0f;
        public float ShotsOnTarget { get; set; } = 0.0f;
        public float Possession { get; set; } = 0.0f;
        public float Corner { get; set; } = 0.0f;
        public float Violations { get; set; } = 0.0f;
        public float Offsides { get; set; } = 0.0f;

        public float RedCards { get; set; } = 0.0f;
        public float YellowCards { get; set; } = 0.0f;
    }
}
