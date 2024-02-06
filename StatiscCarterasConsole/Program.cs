// See https://aka.ms/new-console-template for more information

using Entity;
using System.Net.Http.Headers;

try
{
    Console.WriteLine("Comienza proceso, "+DateTime.Now.ToLongDateString());
    List<Entity.Report> response = new();
    IEnumerable<Entity.Report> result;
    BLL.CallServices callService = new (new DAL.CallService());
    foreach (TypeReport tReport in  Enum.GetValues(typeof(TypeReport)))
    {
        foreach (TypeData tData in Enum.GetValues(typeof(TypeData)))
        {
            if (tReport== TypeReport.PRESTAMOS)
            {
                foreach (AppsByPrestamos tapp in Enum.GetValues(typeof(AppsByPrestamos)))
                {
                    result = await callService.CallMetricResponse(new MetricsRequest
                    {
                        typeReport = tReport,
                        typeData = tData,
                        appsByPrestamos = tapp,
                        appsByAbonos = null,
                        FromDate = null, //new DateTime(2024, 1, 1, 12, 0, 0),
                        ToDate = null // new DateTime(2024, 1, 31, 11, 59, 59),
                    });

                    response.AddRange(result);
                    result = null;
                }
            }
            else
            {
                foreach (AppsByAbonos tapp in Enum.GetValues(typeof(AppsByAbonos)))
                {
                    result = await callService.CallMetricResponse(new MetricsRequest
                    {
                        typeReport = tReport,
                        typeData = tData,
                        appsByPrestamos = null,
                        appsByAbonos = tapp,
                        FromDate = null, // new DateTime(2024, 1, 1, 12, 0, 0),
                        ToDate = null // new DateTime(2024, 1, 31, 11, 59, 59),
                    });

                    response.AddRange(result);
                    result = null;
                }
            }
        }
    }

    string path = Environment.CurrentDirectory;
    BLL.ExcelGenerate.GenerateReport(path, "reporte.xls", "reporte", response);
    Console.WriteLine("Termina Proceso, " + DateTime.Now.ToLongDateString());
}
catch (Exception ex)
{
    Console.WriteLine("Error : Ocurrio un error al generar el reporte");
    Console.WriteLine("Error Message: "+ ex.Message);
    Console.WriteLine("Error StackTrace: " + ex.StackTrace);
    Console.WriteLine("Error full Exception: " + ex.ToString());
}