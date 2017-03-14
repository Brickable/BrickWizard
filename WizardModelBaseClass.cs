using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickWizard
{
    public abstract class WizardModelBaseClass
    {
        public List<Tab> NavBar { get; set; }
        public Step PreviousStep { get; set; }
        public string ActionName { get; set; }
    }
}
