using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BrickWizard
{
    public class Step
    {
        public Step(string actionName, string viewName, string name, IEnumerable<string> propertyNamesToBind)
        {
            ActionName = actionName;
            ViewName = viewName;
            Name = name;
            PropertiesToBind = propertyNamesToBind;
        }
        public Step(string actionName, string viewName, string name, string description, IEnumerable<string> propertyNamesToBind)
        {
            ActionName = actionName;
            ViewName = viewName;
            Name = name;
            PropertiesToBind = propertyNamesToBind;
            Description = description;
        }
        public Step(string actionName, string viewName, string name, string descrption, Func<int> triggerPointRule, IEnumerable<string> propertyNamesToBind)
            : this(actionName, viewName, name, propertyNamesToBind)
        {
            Description = descrption;
            TriggerPointRule = triggerPointRule;
        }
        public Step(string postMethodName, string viewName, string name, Func<int> triggerPointRule, IEnumerable<string> propertyNamesToBind)
            : this(postMethodName, viewName, name, propertyNamesToBind)
        {
            TriggerPointRule = triggerPointRule;
        }

        public string ActionName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        protected internal bool Current { get; set; }
        public string ViewName { get; set; }

        public IEnumerable<string> PropertiesToBind { get; set; }
        public Func<int> TriggerPointRule;
    }
}