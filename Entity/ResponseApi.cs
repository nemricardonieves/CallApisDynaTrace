namespace Entity
{
    public class ResponseApi
    {
        public int? totalCount { get; set; }
        public int? nextPageKey { get; set; }
        public string? resolution { get; set; }
        public IEnumerable<result>? result { get; set; }
    }

    public class result
    {
        public string? metricId { get; set; }
        //public long? dataPointCountRatio { get; set; }
        //public long? dimensionCountRatio { get; set; }
        public IEnumerable<data>? data { get; set; }
    }
    public class data
    {
        //dimensions
        //  dimensionMap
        public IEnumerable<long>? timestamps { get; set; }
        public IEnumerable<long>? values { get; set; }
    }
}