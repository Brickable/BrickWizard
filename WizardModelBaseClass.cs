using System.Collections.Generic;

namespace BrickWizard
{
    public abstract class WizardModelBaseClass
    {
        public NavBar NavBar { get; set; }
        public Step PreviousStep { get; set; }
        public string ActionName { get; set; }
    }
}
