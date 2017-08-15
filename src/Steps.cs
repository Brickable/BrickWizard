using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickWizard
{
    public class Steps
    {
        public Steps(List<Step> steps)
        {
            this.steps = steps;
            this.steps.First().Current = true;
        }
        internal List<Step> steps;
        internal Step Current => steps.FirstOrDefault(x => x.Current);
        internal Step GetStepByActionName(string actionName) => steps.Find(x => x.ActionName == actionName);

        internal void CleanSteps()
        {
            steps.ForEach(x => x.Current = false);
        }
        internal void SetCurrentStep(string actionName)
        {
            CleanSteps();
            steps.First(x => x.ActionName == actionName).Current = true;
        }
    }
}
