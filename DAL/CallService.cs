using Entity;
using System.Text.Json;

namespace DAL
{
    public class CallService : ICallService
    {
        public async Task<MetricResponse> CallMetricServiceAmmount(MetricsRequest request)
        {
            MetricResponse response = new();
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Api-Token " + "");
                HttpResponseMessage responseApi = await client.GetAsync(request.UriRequest);

                response.DateData = request.DateData;
                Apps app = Apps.Unk;
                if (request.appsByAbonos.HasValue)
                {
                    app = (Apps)Enum.Parse(typeof(Apps), request.appsByAbonos.ToString());
                }
                else
                {
                    app = (Apps)Enum.Parse(typeof(Apps), request.appsByPrestamos.ToString());
                }
                response.apps = app;

                if (responseApi.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ResponseApi>(responseApi.Content.ReadAsStringAsync().Result);
                    if (result is not null)
                    {
                        response.TotalAmmount = result.result?.FirstOrDefault()?.data?.FirstOrDefault()?.values?.FirstOrDefault();
                        response.UnixDate = result.result?.FirstOrDefault()?.data?.FirstOrDefault()?.timestamps?.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                response.Exception = ex;
                response.ExceptionMessage = ex.Message;
                response.TotalAmmount = -0;
            }
            return response;
        }

        public async Task<MetricResponse> CallMetricServiceMovemments(MetricsRequest request)
        {
            MetricResponse response = new();
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Api-Token " + "");
                HttpResponseMessage responseApi = await client.GetAsync(request.UriRequest);

                response.DateData = request.DateData;
                Apps app = Apps.Unk;
                if (request.appsByAbonos.HasValue)
                {
                    app = (Apps)Enum.Parse(typeof(Apps), request.appsByAbonos.ToString());
                }
                else
                {
                    app = (Apps)Enum.Parse(typeof(Apps), request.appsByPrestamos.ToString());
                }
                response.apps = app;

                if (responseApi.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ResponseApi>(responseApi.Content.ReadAsStringAsync().Result);
                    if (result is not null)
                    {
                        response.TotalMovements = result.result?.FirstOrDefault()?.data?.FirstOrDefault()?.values?.FirstOrDefault();
                        response.UnixDate = result.result?.FirstOrDefault()?.data?.FirstOrDefault()?.timestamps?.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                response.Exception = ex;
                response.ExceptionMessage = ex.Message;
                response.TotalMovements = -0;
            }
            return response;
        }
    }
}