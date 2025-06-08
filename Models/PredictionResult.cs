namespace Dermainsight.Models
{
    public class PredictionResult
    {
        public string Class { get; set; }
        public double Probability { get; set; }
    }

    public class PredictionResponse
    {
        public List<PredictionResult> Predictions { get; set; }
    }


}
