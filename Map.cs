using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickWizard
{
    public class Map
    {
        private List<Route> routes;
        public Map(List<Route> routes)
        {
            if (routes == null || !routes.Any())
                throw new FieldAccessException("The Map must have at least one route");
            this.routes = routes;
            this.routes.First().Current = true;
        }

        internal Route CurrentRoute => routes.FirstOrDefault(x => x.Current);
        internal Step CurrentStep => CurrentRoute.CurrentStep;
        internal Route GetRoute(int routeId)=> routes.First(x => x.RouteId == routeId);
        internal void CleanRoutes()
        {
            routes.ForEach(x => x.Current = false);
        }
        internal bool TryMoveNextStep() => CurrentRoute.Steps.TryMoveStep();
        internal void FollowTheRoute(int routeId)
        {
            if (CurrentRoute.RouteId == routeId)
            {
                TryMoveNextStep();
                return;
            }
            var currentStep = CurrentStep;
            var routeToJump = GetRoute(routeId);
            CurrentRoute.Steps.CleanSteps();
            CleanRoutes();
            routeToJump.Current = true;
            routeToJump.Steps.SetCurrentStep(currentStep.ActionName);
            TryMoveNextStep();
        }
    }
}
