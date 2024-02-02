namespace Entity
{
    public class CommonResponse
    {
        public string? Message { get; set; }
        public string? ExceptionMessage { get; set; }
        public Exception? Exception { get; set; }
    }
}