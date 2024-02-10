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
                DateTime dayBefore = DateTime.Now.AddDays(-1);
                metricsRequest.FromDate = new DateTime(dayBefore.Year, dayBefore.Month, dayBefore.Day, 0, 0, 0);
                metricsRequest.ToDate = new DateTime(dayBefore.Year, dayBefore.Month, dayBefore.Day, 23, 59, 59);
            }

            List<string> urlFormats = metricsRequest.typeReport == TypeReport.PRESTAMOS ? getUrlPrestamos(metricsRequest.appsByPrestamos) : getUrlAbonos(metricsRequest.appsByAbonos);
            DateTime startDate = metricsRequest.FromDate.Value;
            List<MetricResponse> metricResponse = new();


            do
            {
                DateTime finalDate = startDate.AddMinutes(59);
                string urlAmount = string.Format(urlFormats[0], ConverDateToUnixTime(startDate), ConverDateToUnixTime(finalDate));
                string urlMoveemnt = string.Format(urlFormats[1], ConverDateToUnixTime(startDate), ConverDateToUnixTime(finalDate));

                metricsRequest.UriRequest = new Uri(urlAmount);
                metricsRequest.DateData = finalDate;

                MetricResponse responseAmmount = await dataApi.CallMetricServiceAmmount(metricsRequest);
                metricsRequest.UriRequest = new Uri(urlMoveemnt);
                MetricResponse responseMoveemnt = await dataApi.CallMetricServiceMovemments(metricsRequest);

                responseAmmount.TotalMovements = responseMoveemnt.TotalMovements;
                metricResponse.Add(responseAmmount);

                startDate = startDate.AddHours(1);
            } while (startDate < metricsRequest.ToDate.Value);
            List<Entity.Report> listResult = new ();
            if (metricResponse.Any())
            {
                listResult = (from item in metricResponse
                                                  where item.ExceptionMessage == null
                                                  select new Entity.Report
                                                  {
                                                      Reporte = metricsRequest.typeReport == TypeReport.PRESTAMOS ? TypeReport.PRESTAMOS.ToString() : TypeReport.ABONOS.ToString(),
                                                      Servicio = item.apps.ToString(),
                                                      Anio = item.DateData.Value.Year.ToString(),
                                                      Mes = item.DateData.Value.Month.ToString(),
                                                      Dia = item.DateData.Value.Day.ToString(),
                                                      Hora = item.DateData.Value.Hour,
                                                      Monto = getMoneyFormat(item.TotalAmmount.GetValueOrDefault(0)),
                                                      MontoSinFormato = item.TotalAmmount.HasValue ? item.TotalAmmount.Value : 0,
                                                      Movimientos = item.TotalMovements.HasValue ? item.TotalMovements.Value : 0,
                                                  }).ToList();
            }
            return listResult;
        } 

        private string getMoneyFormat(long data)
        {
            decimal value = 0;
            decimal.TryParse(data.ToString(), out value);
            string response = string.Format("{0:C}", value);
            return string.Format("{0:C}", value);
        }

        private string getDecimalFormat(long data)
        {
            decimal value = 0;
            decimal.TryParse(data.ToString(), out value);
            string response = value.ToString();
            return response;
        }

        private long ConverDateToUnixTime(DateTime time)
        {
            long unixTime = ((DateTimeOffset)time).ToUnixTimeMilliseconds();
            return unixTime;
        }

        private DateTime convertUnixTimeToDateTime(long  unixTime)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddMilliseconds(unixTime);
        }

        private List<string> getUrlPrestamos(AppsByPrestamos? appsByPrestamos)
        {
            Dictionary<AppsByPrestamos, string> urlAmmount;
            Dictionary<AppsByPrestamos, string> urlMoveemts;
            List<string> responseList = new();

            urlAmmount = new Dictionary<AppsByPrestamos, string>
                {
                    {AppsByPrestamos.APP,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeprestamo_wsprestamos:filter(and(or(eq(Dimension,\"6609 - 1\")))):splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByPrestamos.COM,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeprestamo_wsprestamos:filter(and(or(eq(Dimension,6609 - 3\")))):splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByPrestamos.WHATAPP,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeprestamo_wsprestamos:filter(and(or(eq(Dimension,\"7687 - 1\")))):splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByPrestamos.PPAU,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeprestamo_wsprestamos:filter(and(or(eq(Dimension,\"7905 - 20\")))):splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                };

            urlMoveemts = new Dictionary<AppsByPrestamos, string>
                {
                    {AppsByPrestamos.APP,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeprestamos_wsprestamos:filter(and(or(eq(Dimension,\"6609 - 1\")))):splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByPrestamos.COM,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeprestamos_wsprestamos:filter(and(or(eq(Dimension,\"6609 - 3\")))):splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByPrestamos.WHATAPP,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeprestamos_wsprestamos:filter(and(or(eq(Dimension,\"7687 - 1\")))):splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByPrestamos.PPAU,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeprestamos_wsprestamos:filter(and(or(eq(Dimension,\"7905 - 20\")))):splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                };

            responseList.Add(urlAmmount[appsByPrestamos.Value]);
            responseList.Add(urlMoveemts[appsByPrestamos.Value]);

            return responseList;
        }

        private List<string> getUrlAbonos(AppsByAbonos? appsByAbonos)
        {
            Dictionary<AppsByAbonos, string> urlAmmount;
            Dictionary<AppsByAbonos, string> urlMoveemts;
            List<string> responseList = new();


            urlAmmount = new Dictionary<AppsByAbonos, string>
                {
                    {AppsByAbonos.APP,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeabonos_app_wsabonos:splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.COM,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeabonos_ecommerce_wsabonos:splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.WHATAPP,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeabonos_whatsapp_wsabonos:splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.ATM,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeabonos_abonosatm_wsabonos:splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.OXXO,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeabonos_oxxo_wsabonos:splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.COBRANZA,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.abonos_importe_tienda_cobranza:splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.TELECOM,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeabonos_abonostelecom_wsabonos:splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.REDEFECTIVA,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.importeabonos_redefectiva_wsabonos:splitBy():sum:sort(value(sum,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                };

            urlMoveemts = new Dictionary<AppsByAbonos, string>
                {
                    {AppsByAbonos.APP,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeabonos_app_wsabonos:splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.COM,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeabonos_ecommerce_wsabonos:splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.WHATAPP,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeabonos_whatsapp_wsabonos:splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.ATM,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeabonos_abonosatm_wsabonos:splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.OXXO,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeabonos_oxxo_wsabonos:splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.COBRANZA,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numero_abonos_tienda_cobranza:splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.TELECOM,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeabonos_abonostelecom_wsabonos:splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                    {AppsByAbonos.REDEFECTIVA,"https://sxf54568.live.dynatrace.com/api/v2/metrics/query?metricSelector=(calc:service.numerodeabonos_redefectiva_wsabonos:splitBy():sort(value(auto,descending)):limit(20)):limit(100):names&from={0}&to={1}&resolution=Inf&mzSelector=mzId(-2939186880565774010)" },
                };

            responseList.Add(urlAmmount[appsByAbonos.Value]);
            responseList.Add(urlMoveemts[appsByAbonos.Value]);
            return responseList;
        }
    }
}