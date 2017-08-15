using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickWizard.ExtensionMethods
{
    internal static class StepsExtensionMethods
    {
        internal static Steps FreezeSteps(this Steps steps, string[] frozenSteps)
        {
            return new Steps(steps.steps.Where(x => !frozenSteps.Contains(x.ActionName)).ToList());
        }
    }
}
