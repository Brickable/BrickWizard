using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BrickWizard
{
    public abstract class WizardModelBaseClass
    {
        [IgnoreDataMember]
        public NavBar NavBar { get; internal set; }
        [IgnoreDataMember]
        public Step PreviousStep { get; internal set; }
        [IgnoreDataMember]
        public string ActionName { get; internal set; }
        [IgnoreDataMember]
        public string ControllerName { get; internal set; }
    }
}
