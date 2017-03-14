using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;



namespace BrickWizard
{
    public class Wizard<T> where T :WizardModelBaseClass, new()
    {
        public Wizard(IWorkflow<T> wf)
        {
            WF = wf;
        }

        public IWorkflow<T> WF { get; set; }

        public bool MoonWalkPerformed { get; private set; }

        public virtual void Sync<T1>(T1 obj)
        {
            var callerMethodName = new StackTrace().GetFrame(1).GetMethod().Name;
            var currentStep = WF.CurrentRoute.CurrentStep.ActionName;

            if (callerMethodName != currentStep)
            {
                MoonWalkPerformed = true;
                MoonWalkTill(callerMethodName);
            }
            else if (callerMethodName == currentStep)
            {
                MoonWalkPerformed = (obj==null);
                if (obj != null)
                {
                    var propertyInfo = typeof(T).GetProperties().First(x => x.PropertyType.FullName == obj.GetType().FullName);
                    var property = typeof(T).GetProperty(propertyInfo.Name);
                    property.SetValue(WF.Model, obj);
                }
                WF.Orchestrate();
            }
            WF.BaseModelSync();
        }
        //public virtual void Sync(params object[] objs)
        //{
        //    var callerMethodName = new StackTrace().GetFrame(1).GetMethod().Name;
        //    var currentStep = WF.CurrentRoute.CurrentStep.ActionName;

        //    if (callerMethodName == currentStep)
        //    {
        //        HasWorkflowGetBackInLastSincronization = false;
        //        foreach (var obj in objs)
        //        {
        //            var propertyInfo = typeof(T).GetProperties().First(x => x.PropertyType.FullName == obj.GetType().FullName);
        //            var property = typeof(T).GetProperty(propertyInfo.Name);
        //            property.SetValue(WF.Model, obj);
        //        }
        //    }
        //    else
        //    {
        //        HasWorkflowGetBackInLastSincronization = true;
        //        StepRouteBackTo(callerMethodName);
        //    }
        //    WF.Orchestrate();
        //}
        private void MoonWalkTill(string methodName)
        {
            while (WF.CurrentRoute.CurrentStep.ActionName != methodName)
            {
                    if(!WF.TryMovePreviousStep())
                        break;
            }
        }
    }
}