using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BrickWizard
{
    public abstract class WizardModelBaseClass
    {
        public NavBar NavBar { get; internal set; }
        public string PreviousStepActionName { get; internal set; }
        public string ActionName { get; internal set; }
        public string ControllerName { get; internal set; }
        public string AreaName { get; internal set; }
        public int CurrentRouteId { get; internal set; }
    }
}
