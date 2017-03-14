using System.Collections.Generic;
using System.Linq;

namespace BrickWizard
{
    public class Route
    {
        public int RouteId { get; set; }
        internal bool Current { get; set; }
        public List<Step> Steps { get; set; }
        public Step CurrentStep => Steps.FirstOrDefault(x => x.Current);

        internal Step NextStep => GetNeighbors(1);
        internal Step PreviousStep => GetNeighbors(-1);
        internal bool TryMoveStep() => TryIterateStep(true);
        internal bool TryBackStep() => TryIterateStep(false);

        private bool TryIterateStep(bool isToIncrement)
        {
            Step Active = (isToIncrement) ? NextStep : PreviousStep;
            if (Active == null)
                return false;
            Steps.ForEach(x => x.Current = false);
            Active.Current = true;
            return true;
        }
        private Step GetNeighbors(int i) => Steps.ElementAtOrDefault(Steps.FindIndex(x => x.Current) + i);
    }
}