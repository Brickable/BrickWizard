using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickWizard
{
    public class NavBar
    {
        public NavBar(int maxTabs, IEnumerable<Tab> tabs)
        {
            Tabs = tabs;
            MaxTabs = maxTabs;
        }
        public IEnumerable<Tab> Tabs {get;set;}
        public int MaxTabs { get; }
    }
}
