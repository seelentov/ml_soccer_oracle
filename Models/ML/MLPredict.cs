using Microsoft.ML.Data;

namespace WebApplication2.Models.ML
{
    public class MLPredict
    {
        [ColumnName("Score")]
        public string Result { get; set; }
    }
}


