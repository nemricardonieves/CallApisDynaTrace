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
    public class CallServices
    {
        public CommonResponse CallMetricResponse(MetricsRequest metricsRequest)
        {
            CommonResponse response = new();
            string url = metricsRequest.typeReport == TypeReport.PRESTAMOS ? getUrlPrestamos(metricsRequest.typeData, metricsRequest.AppsByPrestamos) : getUrlAbonos(metricsRequest.typeData, metricsRequest.AppsByAbonos);
            metricsRequest.FromUnixDate = ConverDateToUnixTime(metricsRequest.FromDate.Value);
            metricsRequest.ToUnixDate = ConverDateToUnixTime(metricsRequest.ToDate.Value);
            url = string.Format(url, metricsRequest.FromUnixDate, metricsRequest.ToUnixDate);
            Uri uri = new Uri(url);
            return  response;
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

        private string getUrlPrestamos(TypeData typeData, AppsByPrestamos appsByPrestamos)
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
            response = url[appsByPrestamos];
            return response;
        }

        private string getUrlAbonos(TypeData typeData, AppsByAbonos appsByAbonos)
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
            response = url[appsByAbonos];
            return response;
        }
    }
}