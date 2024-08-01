using WebApplication2.Models.Base;

namespace WebApplication1.Models.Base
{
    public abstract class SoccerEntity : BaseEntity
    {
        public string Url { get; set; } = string.Empty;
    }
}
