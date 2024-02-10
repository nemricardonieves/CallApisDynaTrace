using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface ICallService
    {
         Task<MetricResponse> CallMetricServiceAmmount(MetricsRequest request);
        Task<MetricResponse> CallMetricServiceMovemments(MetricsRequest request);
    }
}
