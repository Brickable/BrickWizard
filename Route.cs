using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BrickWizard
{
    public class Route
    {
        public Route(int routeId,List<StepReference> routeSteps)
        {
            RouteId = routeId;
            RouteSteps = routeSteps;
        }
        public int RouteId { get; set; }
        internal bool Current { get; set; }
        internal List<StepReference> RouteSteps { get; }
        public int Lenght => RouteSteps.Count();
    }
}