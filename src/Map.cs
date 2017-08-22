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
            {
                throw new ArgumentException("The Map must have at least one route");
            }
            //check for unique Ids
            Routes = routes;
            Routes.First().Current = true;
        }
        internal List<Route> Routes;
        internal Route CurrentRoute => Routes.FirstOrDefault(x => x.Current);
        internal Route GetRoute(int routeId) => Routes.First(x => x.RouteId == routeId);

    }
}
