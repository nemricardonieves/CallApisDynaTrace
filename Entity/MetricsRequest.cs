namespace Entity
{
    public class MetricsRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? FromUnixDate { get; set; }
        public long? ToUnixDate { get; set; }
        public long? IdSelector { get; set; }
        public int? RowsCount { get; set; }
        public string? Service { get; set; }
        public int? ServiceId { get; set; }
        public int? SelectorApp { get; set; }
        public TypeReport typeReport { get; set; }
        public TypeData typeData { get; set; }
        public AppsByAbonos AppsByAbonos { get; set; }
        public AppsByPrestamos AppsByPrestamos { get; set; }
    }
}