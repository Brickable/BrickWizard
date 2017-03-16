using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BrickWizard
{
    public abstract class Wizard<T> where T :WizardModelBaseClass, new()
    {
        public Wizard()
        {
            BaseModelSync();
        }

        protected abstract void Orchestrate();
        protected abstract Map Map { get; }
        protected abstract List<string> TriggerPoints { get; }
        protected abstract int MaxNavSteps { get; }

        public void Sync<T1>(T1 obj)
        {
            var callerMethodName = new StackTrace().GetFrame(1).GetMethod().Name;
            var currentStep = CurrentRoute.CurrentStep.ActionName;

            if (callerMethodName != currentStep)
            {
                MoonWalkPerformed = true;
                MoonWalkTill(callerMethodName);
            }
            else if (callerMethodName == currentStep)
            {
                MoonWalkPerformed = (obj == null);
                if (obj != null)
                {
                    var propertyInfo = typeof(T).GetProperties().First(x => x.PropertyType.FullName == obj.GetType().FullName);
                    var property = typeof(T).GetProperty(propertyInfo.Name);
                    property.SetValue(Model, obj);
                }
                Orchestrate();
            }
            BaseModelSync();
        }
        public void Sync(params object[] objs)
        {
            var callerMethodName = new StackTrace().GetFrame(1).GetMethod().Name;
            var currentStep = CurrentRoute.CurrentStep.ActionName;

            if (callerMethodName != currentStep)
            {
                MoonWalkPerformed = true;
                MoonWalkTill(callerMethodName);
            }

            else if (callerMethodName == currentStep)
            {
                MoonWalkPerformed = (objs == null || objs.Length == 0);
                if (objs != null && objs.Length > 0)
                {
                    foreach (var obj in objs)
                    {
                        var propertyInfo = typeof(T).GetProperties().First(x => x.PropertyType.FullName == obj.GetType().FullName);
                        var property = typeof(T).GetProperty(propertyInfo.Name);
                        property.SetValue(Model, obj);
                    }
                }
                Orchestrate();
            }
            BaseModelSync();
        }

        public T Model { get; set; } = new T();
        public Route CurrentRoute => Map.CurrentRoute;
        public Step CurrentStep => Map.CurrentStep;
        private List<Tab> NavBar => CurrentRoute.Steps.GetNavBar();

        protected bool IsTriggerPointStep => TriggerPoints.Exists(x => x == CurrentRoute.CurrentStep.ActionName);
        protected bool TryMoveNextStep() => Map.TryMoveNextStep();
        protected void FollowTheRoute(int routeId)
        {
            Map.FollowTheRoute( routeId);
        }
        
        private bool MoonWalkPerformed { get; set; }
        private bool TryMovePreviousStep() { return CurrentRoute.Steps.TryBackStep(); }
        private void BaseModelSync()
        {
            Model.NavBar = NavBar;
            Model.ActionName = CurrentRoute.CurrentStep.ActionName;
            Model.PreviousStep = CurrentRoute.Steps.PreviousStep;
        }
        private void MoonWalkTill(string methodName)
        {
            while (CurrentRoute.CurrentStep.ActionName != methodName)
            {
                if (!TryMovePreviousStep())
                    break;
            }
        }
    }
}