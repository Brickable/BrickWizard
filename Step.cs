using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrickWizard
{
    public class Step
    {
        public Step(string postMethodName, string viewName, string name,int stepNumber)
        {
            ActionName = postMethodName;
            ViewName = viewName;
            Name = name;
            StepNumber = stepNumber;
        }
        public string ActionName { get; set; }
        public string Name { get; set; }
        public int StepNumber { get; set; }
        protected internal bool Current { get; set; }
        public string ViewName { get; set; }
    }
}