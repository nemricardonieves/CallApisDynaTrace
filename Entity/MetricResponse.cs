
namespace Entity
{
    public class MetricResponse          :CommonResponse
    {
        public long? UnixDate { get; set; }
        public long? Total { get; set; }
        public TypeReport typeReport { get; set; }
        public TypeData typeData { get; set; }
        public Apps apps { get; set; }
        public DateTime? DateData { get; set; }
        public string? Hour { get; set; }
    }
}