using Entity;

namespace DAL
{
    public class CallService:ICallService
    {
        public MetricResponse CallMetricService(MetricsRequest request)
        {
            MetricResponse response = new MetricResponse();
            return response;
        }
    }
}