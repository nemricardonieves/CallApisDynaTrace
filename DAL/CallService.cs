using Entity;
using System.Text.Json;

namespace DAL
{
    public class CallService : ICallService
    {
        public async Task<MetricResponse> CallMetricService(MetricsRequest request)
        {
            MetricResponse response = new();
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Api-Token " + "dt0c01.HC64WEWL3FKZOBESWHLXCDNH.KYESEJYU4B6NXGTPLKVTSAUDCMBQ34TBCVGZ5SYKC66WUDQPC2NQ3SIFDCMHAA2Q");
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
                        response.Total = result.result?.FirstOrDefault()?.data?.FirstOrDefault()?.values?.FirstOrDefault();
                        response.UnixDate = result.result?.FirstOrDefault()?.data?.FirstOrDefault()?.timestamps?.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                response.Exception = ex;
                response.ExceptionMessage = ex.Message;
            }
            return response;
        }
    }
}