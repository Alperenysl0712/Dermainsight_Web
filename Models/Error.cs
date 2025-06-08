namespace Dermainsight.Models
{
    public class Error
    {
        public string? Title { get; set; }
        public string? Message { get; set; }
        public int? StatusCode { get; set; }
        public string? RequestedPath { get; set; }
        public string? ExceptionDetails { get; set; }
    }

    public class ServiceHealthResult
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public int? StatusCode { get; set; }
    }

}
