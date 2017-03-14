using System.Collections.Generic;

namespace BrickWizard
{
    public abstract class WizardModelBaseClass
    {
        public List<Tab> NavBar { get; set; }
        public Step PreviousStep { get; set; }
        public string ActionName { get; set; }
    }
}
