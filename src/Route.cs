using System;
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
            RunStepNumberGenerator();
        }

        public int RouteId { get; set; }
        public int Lenght => RouteSteps.Count;
        internal bool Current { get; set; }
        internal List<StepReference> RouteSteps { get; }

        private void RunStepNumberGenerator()
        {
            foreach (var i in RouteSteps)
            {
                i.StepNumber = RouteSteps.IndexOf(i) + 1;
            }
        }
    }
}