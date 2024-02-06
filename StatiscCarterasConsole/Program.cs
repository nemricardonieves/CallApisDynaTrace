// See https://aka.ms/new-console-template for more information

using Entity;
using System.Net.Http.Headers;

try
{
    Console.WriteLine("Comienza proceso");
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
                        FromDate=new DateTime(2024,2,5,9,0,0),
                        ToDate = new DateTime(2024, 2, 5, 9, 59, 59),
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
                        FromDate = new DateTime(2024, 2, 5, 9, 0, 0),
                        ToDate = new DateTime(2024, 2, 5, 9, 59, 59),
                    });

                    response.AddRange(result);
                    result = null;
                }
            }
        }
    }

    string path = Environment.CurrentDirectory;
    BLL.ExcelGenerate.GenerateReport(path, "reporte.xls", "reporte", response);
    Console.WriteLine("Termina Proceso");
}
catch (Exception ex)
{
    Console.WriteLine("Error : Ocurrio un error al generar el reporte");
    Console.WriteLine("Error Message: "+ ex.Message);
    Console.WriteLine("Error StackTrace: " + ex.StackTrace);
    Console.WriteLine("Error full Exception: " + ex.ToString());
}