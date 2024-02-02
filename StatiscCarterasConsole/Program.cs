// See https://aka.ms/new-console-template for more information

try
{
    Console.WriteLine("Comienza proceso");
    BLL.Report.GetReport(Entity.ReportType.MovementsPerDay,DateTime.Now, DateTime.Now);
    Console.WriteLine("Termina Proceso");
}
catch (Exception ex)
{
    Console.WriteLine("Error : Ocurrio un error al generar el reporte");
    Console.WriteLine("Error Message: "+ ex.Message);
    Console.WriteLine("Error StackTrace: " + ex.StackTrace);
    Console.WriteLine("Error full Exception: " + ex.ToString());
}