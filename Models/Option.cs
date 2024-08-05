using WebApplication2.Models.Base;

namespace WebApplication2.Models
{
    public class Option:BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
