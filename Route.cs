using System.Collections.Generic;
using System.Linq;
using System;

namespace BrickWizard
{
    public class Route
    {
        public int RouteId { get; set; }
        public bool Current { get; set; }
        public List<Step> Steps { get; set; }
        public Step CurrentStep => Steps.FirstOrDefault(x => x.Current);
        public Step NextStep => GetNeighbors(1);
        public Step PreviousStep => GetNeighbors(-1);
        public bool TryIncrementRouteStep() => TryIterateStep(true);
        public bool TryDecrementRouteStep() => TryIterateStep(false);
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