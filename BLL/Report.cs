using Entity;

namespace BLL
{
    public class Report
    {
        public static Response GetReport(Entity.ReportType type, DateTime startDate, DateTime endDate)
        {
            Response response = new();
            List<CommonEntity> lst = new()
            {
                new CommonEntity { Channel = "APP",Year=2023,Day=1,Average=500 },
                new CommonEntity { Channel = "APP",Year=2023,Day=1,Average=500 } ,
                new CommonEntity { Channel = "APP",Year=2023,Day=1,Average=500 }   ,
                new CommonEntity { Channel = "APP",Year=2023,Day=1,Average=500 }       ,
                new CommonEntity { Channel = "APP",Year=2023,Day=1,Average=500 }               ,
            };
            response.ReportList = lst;
            string path = Environment.CurrentDirectory;
            string reportName = "StaticsCarteras.xlsx";
            string sheet = string.Concat(DateTime.Now.ToString("yyyMMdd"));
           // ExcelGenerate.GenerateReport(path, reportName, sheet, response.ReportList);
            return response;
        }
    }
}