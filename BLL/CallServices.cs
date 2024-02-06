using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class CallServices   : ICallService
    {
        readonly DAL.ICallService dataApi;

        public CallServices(DAL.ICallService dataApi)
        {
            this.dataApi = dataApi;
        }

        public async Task<IEnumerable<Entity.Report>> CallMetricResponse(MetricsRequest metricsRequest)
        {
            IEnumerable<MetricResponse> response;

            if (!metricsRequest.FromDate.HasValue)
            {
                metricsRequest.FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                metricsRequest.ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            }

            string url = metricsRequest.typeReport == TypeReport.PRESTAMOS ? getUrlPrestamos(metricsRequest.typeData, metricsRequest.appsByPrestamos) : getUrlAbonos(metricsRequest.typeData, metricsRequest.appsByAbonos);
            DateTime startDate = metricsRequest.FromDate.Value;
            List<MetricResponse> metricResponse = new();
            do
            {
                DateTime finalDate = startDate.AddMinutes(59);
                url = string.Format(url, ConverDateToUnixTime(startDate), ConverDateToUnixTime(finalDate));
                metricsRequest.UriRequest = new Uri(url);
                metricsRequest.DateData = finalDate;
                metricResponse.Add(await dataApi.CallMetricService(metricsRequest));
                startDate= startDate.AddHours(1);
            } while (startDate < metricsRequest.ToDate.Value);

            List<Entity.Report> listResult = (from item in metricResponse
                                              where item.ExceptionMessage == null
                                              select new Entity.Report
                                              {
                                                  typeReport = metricsRequest.typeReport == TypeReport.PRESTAMOS ? TypeReport.PRESTAMOS.ToString() : TypeReport.ABONOS.ToString(),
                                                  typeData = metricsRequest.typeData == TypeData.MONTO ? "MONTO" : "MOVIMIENTO",
                                                  app = item.apps.ToString(),
                                                  hour = item.DateData.Value.Hour,
                                                  date = item.DateData.Value.ToShortDateString(),
                                                  valueData = item.Total
                                              }).ToList();
            return listResult;
        } 

        private long ConverDateToUnixTime(DateTime time)
        {               /*
                         *  DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    TimeSpan diff = (date ?? DateTime.Now) - origin;
    return Convert.ToDouble(Math.Floor(diff.TotalSeconds));
                         */
            long unixTime = ((DateTimeOffset)time).ToUnixTimeMilliseconds();
            return unixTime;
        }

        private DateTime convertUnixTimeToDateTime(long  unixTime)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddMilliseconds(unixTime);
        }

        private string getUrlPrestamos(TypeData typeData, AppsByPrestamos? appsByPrestamos)
        {
            string response = string.Empty;
            Dictionary<AppsByPrestamos, string> url;
            if (typeData== TypeData.MONTO)
            {
                url = new Dictionary<AppsByPrestamos, string>
                {
                    {AppsByPrestamos.APP,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeprestamo_wsprestamos:filter(and(or(eq(Dimension,\"6609 - 1\")))):splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByPrestamos.COM,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeprestamo_wsprestamos:filter(and(or(eq(Dimension,6609 - 3\")))):splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByPrestamos.WHATAPP,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeprestamo_wsprestamos:filter(and(or(eq(Dimension,\"7687 - 1\")))):splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByPrestamos.PPAU,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeprestamo_wsprestamos:filter(and(or(eq(Dimension,\"7905 - 20\")))):splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByPrestamos.TELECOM,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeabonos_abonostelecom_wsabonos:splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByPrestamos.REDEFECTIVA,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeabonos_redefectiva_wsabonos:splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                };
            }
            else
            {
                url = new Dictionary<AppsByPrestamos, string>
                {
                    {AppsByPrestamos.APP,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeprestamos_wsprestamos:filter(and(or(eq(Dimension,\"6609 - 1\")))):splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByPrestamos.COM,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeprestamos_wsprestamos:filter(and(or(eq(Dimension,\"6609 - 3\")))):splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByPrestamos.WHATAPP,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeprestamos_wsprestamos:filter(and(or(eq(Dimension,\"7687 - 1\")))):splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByPrestamos.PPAU,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeprestamos_wsprestamos:filter(and(or(eq(Dimension,\"7905 - 20\")))):splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByPrestamos.TELECOM,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeabonos_abonostelecom_wsabonos:splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByPrestamos.REDEFECTIVA,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeabonos_redefectiva_wsabonos:splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                };
            }
            response = url[appsByPrestamos.Value];
            return response;
        }

        private string getUrlAbonos(TypeData typeData, AppsByAbonos? appsByAbonos)
        {
            string response = string.Empty;
            Dictionary<AppsByAbonos, string> url;
            if (typeData == TypeData.MONTO)
            {
                url = new Dictionary<AppsByAbonos, string>
                {
                    {AppsByAbonos.APP,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeabonos_app_wsabonos:splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.COM,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeabonos_ecommerce_wsabonos:splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.WHATAPP,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeabonos_whatsapp_wsabonos:splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.ATM,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeabonos_abonosatm_wsabonos:splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.OXXO,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeabonos_oxxo_wsabonos:splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.COBRANZA,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.abonos_importe_tienda_cobranza:splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                };
            }
            else
            {
                url = new Dictionary<AppsByAbonos, string>
                {
                    {AppsByAbonos.APP,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeabonos_app_wsabonos:splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.COM,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeabonos_ecommerce_wsabonos:splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.WHATAPP,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeabonos_whatsapp_wsabonos:splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.ATM,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeabonos_abonosatm_wsabonos:splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.OXXO,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeabonos_oxxo_wsabonos:splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.COBRANZA,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numero_abonos_tienda_cobranza:splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                };
            }
            response = url[appsByAbonos.Value];
            return response;
        }
    }
}