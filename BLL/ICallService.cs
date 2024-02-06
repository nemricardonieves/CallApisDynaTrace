using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface ICallService
    {
        Task<IEnumerable<Entity.Report>> CallMetricResponse(MetricsRequest metricsRequest);
    }
}
