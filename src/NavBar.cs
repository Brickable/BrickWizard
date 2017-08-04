using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickWizard
{
    public class NavBar
    {
        public NavBar(int maxTabs, IEnumerable<Tab> tabs,string controllerName,string areaName)
        {
            Tabs = tabs;
            MaxTabs = maxTabs;
            ControllerName = controllerName;
            AreaName = areaName;
        }
        public IEnumerable<Tab> Tabs {get;set;}
        public int MaxTabs { get; }
        public string ControllerName { get; private set; }
        public string AreaName { get; private set; }
    }
}
