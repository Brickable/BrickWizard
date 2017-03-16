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

        public Step Current => steps.FirstOrDefault(x => x.Current);

        public int Length => steps.Count();
        private List<Step> steps;

        internal bool TryMoveStep() => TryIterateStep(true);
        internal bool TryBackStep() => TryIterateStep(false);
        internal Step NextStep => GetNeighbors(1);
        internal Step PreviousStep => GetNeighbors(-1);

        private bool TryIterateStep(bool isToIncrement)
        {
            Step active = (isToIncrement) ? NextStep : PreviousStep;
            if (active == null)
            {
                return false;
            }
            CleanSteps();
            active.Current = true;
            return true;
        }
        private Step GetNeighbors(int i) => steps.ElementAtOrDefault(steps.FindIndex(x => x.Current) + i);

        internal List<Tab> GetNavBar()
        {
            var NavBar = new List<Tab>();
            steps.ForEach(x =>
                NavBar.Add(
                new Tab
                {
                    Current = x.Current,
                    Name = x.Name,
                    Number = x.StepNumber,
                    Action = x.ActionName,
                    Description = x.Description
                }));
            return NavBar;
        }

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
