// See https://aka.ms/new-console-template for more information

using Entity;
using System.Net.Http.Headers;

try
{
    Console.WriteLine("Comienza proceso, "+DateTime.Now.ToLongDateString());
    List<Entity.Report> response = new();
    IEnumerable<Entity.Report> result;
    BLL.CallServices callService = new (new DAL.CallService());

    TypeReport report = TypeReport.UNKNOW;
    TypeData data= TypeData.UNKNOW;
    AppsByAbonos appsByAbonos = AppsByAbonos.UNKNOW;
    AppsByPrestamos appsByPrestamos= AppsByPrestamos.UNKNOW;

    if (report != TypeReport.UNKNOW & data != TypeData.UNKNOW & (appsByAbonos != AppsByAbonos.UNKNOW || appsByPrestamos != AppsByPrestamos.UNKNOW))
    {
        await callService.CallMetricResponse(new MetricsRequest
        {
            typeReport = report,
            typeData = data,
            appsByPrestamos = appsByPrestamos != AppsByPrestamos.UNKNOW ? appsByPrestamos : null,
            appsByAbonos = appsByAbonos != AppsByAbonos.UNKNOW ? appsByAbonos : null,
            FromDate = new DateTime(2024, 2, 7, 0, 0, 0),
            ToDate = new DateTime(2024, 2, 7, 23, 29, 59)
        });
    }
    else
    {
        foreach (TypeReport tReport in Enum.GetValues(typeof(TypeReport)))
        {

            if (tReport == TypeReport.PRESTAMOS)
            {
                foreach (AppsByPrestamos tapp in Enum.GetValues(typeof(AppsByPrestamos)))
                {
                    if (tapp != AppsByPrestamos.UNKNOW)
                    {
                        result = await callService.CallMetricResponse(new MetricsRequest
                        {
                            typeReport = tReport,
                            //typeData = tData,
                            appsByPrestamos = tapp,
                            appsByAbonos = null,
                            //FromDate = null, 
                            FromDate = new DateTime(2024, 1, 1, 7, 0, 0),
                            //ToDate = null 
                            ToDate = new DateTime(2024, 1, 31, 23, 59, 59),
                        });

                        response.AddRange(result);
                        result = null;
                    }
                }
            }
            else
            {
                foreach (AppsByAbonos tapp in Enum.GetValues(typeof(AppsByAbonos)))
                {
                    if (tapp != AppsByAbonos.UNKNOW)
                    {
                        result = await callService.CallMetricResponse(new MetricsRequest
                        {
                            typeReport = tReport,
                            //typeData = tData,
                            appsByPrestamos = null,
                            appsByAbonos = tapp,
                            //FromDate = null, 
                            FromDate = new DateTime(2024, 1, 1, 7, 0, 0),
                            //ToDate = null 
                            ToDate = new DateTime(2024, 1, 31, 23, 59, 59),
                        });

                        response.AddRange(result);
                        result = null;
                    }
                }
            }

        }
    }

    string path = Environment.CurrentDirectory;
    BLL.ExcelGenerate.GenerateReport(path, "reporte1.xls", "reporte", response);
    Console.WriteLine("Termina Proceso, " + DateTime.Now.ToLongDateString());
}
catch (Exception ex)
{
    Console.WriteLine("Error : Ocurrio un error al generar el reporte");
    Console.WriteLine("Error Message: "+ ex.Message);
    Console.WriteLine("Error StackTrace: " + ex.StackTrace);
    Console.WriteLine("Error full Exception: " + ex.ToString());
}