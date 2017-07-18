using System.Collections.Generic;

namespace BrickWizard
{
    public abstract class WizardModelBaseClass
    {
        public NavBar NavBar { get; internal set; }
        public Step PreviousStep { get; internal set; }
        public string ActionName { get; internal set; }
        public string ControllerName { get; internal set; }
    }
}
