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
        MetricResponse CallMetricService(MetricsRequest request);
    }
}
