using System.Collections.Generic;
using System.Linq;

namespace BrickWizard
{
    public class Route
    {
        public int RouteId { get; set; }
        internal bool Current { get; set; }
        public Steps Steps { get; set; }
        public Step CurrentStep => Steps.Current; 
        public int Lenght => Steps.Length;
    }
}