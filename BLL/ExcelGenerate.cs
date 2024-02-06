using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BLL
{
    public class ExcelGenerate
    {
        public static void GenerateReport(string path, string reportName, string sheetName, IEnumerable<Entity.Report> reportData)
        {
            ExcelPackage excelPackage = new();
            excelPackage.Workbook.Worksheets.Add(sheetName);
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[sheetName];
            ExcelRangeBase excelRangeBase = worksheet.Cells["A1"].LoadFromCollection(reportData, PrintHeaders: true);
            
            reportName = Path.Combine(path, reportName);
            FileStream fileStream = new(reportName, FileMode.Create);
            excelPackage.SaveAs(fileStream);
        }
    }
}
//https://medium.com/geekculture/upload-files-to-google-drive-with-c-c32d5c8a7abc