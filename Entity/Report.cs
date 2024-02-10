namespace Entity
{
    public class Report
    {
        public string? Reporte { get; set; }        
        public string? Servicio { get; set; }
        public string Anio { get; set; }
        public string Mes { get; set; }
        public string Dia { get; set; }
        public int Hora { get; set; }
        public string Monto { get; set; }
        public long MontoSinFormato { get; set; }
        public long Movimientos { get; set; }
    }
}