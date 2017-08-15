using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickWizard
{
    public class Map
    {
        public Map(List<Route> routes)
        {
            if (routes == null || !routes.Any())
                throw new FieldAccessException("The Map must have at least one route");
            Routes = routes;
            Routes.First().Current = true;
        }
        internal List<Route> Routes;
        internal Route CurrentRoute => Routes.FirstOrDefault(x => x.Current);
        internal Route GetRoute(int routeId) => Routes.First(x => x.RouteId == routeId);

        internal Map FreezeSteps(string[] frozenSteps)
        {
            var routes = new List<Route>();
            foreach(var route in Routes)
            {
                routes.Add(new Route(route.RouteId, route.RouteSteps.Where(x => !frozenSteps.Contains(x.ActionName)).ToList()));
            }
            return new Map(routes);
        }
    }
}
