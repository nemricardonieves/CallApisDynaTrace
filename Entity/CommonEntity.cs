namespace Entity
{
    public class CommonEntity
    {
        public int Year { get; set; }   
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; } = 0;
        public long TotalRows { get; set; }
        public long Average { get; set; }
        public string? Channel {  get; set; }
    }
}