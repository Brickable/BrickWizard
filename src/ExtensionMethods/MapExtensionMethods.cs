using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickWizard.ExtensionMethods
{
    internal static class MapExtensionMethods
    {
        internal static Map FreezeSteps(this Map map, string[] frozenSteps)
        {
            var routes = new List<Route>();
            foreach (var route in map.Routes)
            {
                routes.Add(new Route(route.RouteId, route.RouteSteps.Where(x => !frozenSteps.Contains(x.ActionName)).ToList()));
            }
            return new Map(routes);
        }
    }
}
