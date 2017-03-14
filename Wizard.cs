using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BrickWizard
{
    public abstract class Wizard<T> where T :WizardModelBaseClass, new()
    {
        public Wizard()
        {
            if (DefaultRoutes == null || !DefaultRoutes.Any())
                throw new FieldAccessException("The Routes collection must be instanciated in InitializeRoute method");
            Routes = DefaultRoutes;
            var currentRoutes = Routes.Where(x => x.Current);
            if (currentRoutes.Any())
                throw new FieldAccessException("You cannot define Current Route in DefaultRoutes");
            var allCurrentSteps = currentRoutes.SelectMany(x => x.Steps.Where(z => z.Current));
            if (currentRoutes.Any())
                throw new FieldAccessException("You cannot define Current Steps in DefaultRoutes");
            WizardInit();
            BaseModelSync();
        }

        protected abstract void Orchestrate();
        protected abstract List<Route> DefaultRoutes { get; }
        protected abstract List<string> TriggerPoints { get; }

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
        public List<Tab> NavBar
        {
            get
            {
                var NavBar = new List<Tab>();

                CurrentRoute.Steps.ForEach(x =>
                    NavBar.Add(
                    new Tab
                    {
                        Current = x.Current,
                        Name = x.Name,
                        Number = x.StepNumber,
                        Action = x.ActionName
                    }));
                return NavBar;
            }
        }
        public Route CurrentRoute => Routes.FirstOrDefault(x => x.Current);
        public Step CurrentStep => CurrentRoute.CurrentStep;

        private bool MoonWalkPerformed { get; set; }
        protected bool IsTriggerPointStep => TriggerPoints.Exists(x => x == CurrentRoute.CurrentStep.ActionName);
        protected List<Route> Routes { get; private set; }
        protected bool TryMoveNextStep() { return CurrentRoute.TryMoveStep(); }
        protected void FollowTheRoute(int routeId)
        {
            if (CurrentRoute.RouteId == routeId)
            {
                TryMoveNextStep();
                return;
            }
            var currentStep = CurrentStep;
            var routeToJump = Routes.First(x => x.RouteId == routeId);
            Routes.ForEach(x => x.Current = false);
            routeToJump.Current = true;
            routeToJump.Steps.ForEach(x => x.Current = false);
            routeToJump.Steps.First(x => x.ActionName == currentStep.ActionName).Current = true;
            CurrentRoute.TryMoveStep();
        }

        private bool TryMovePreviousStep() { return CurrentRoute.TryBackStep(); }
        private void BaseModelSync()
        {
            Model.NavBar = NavBar;
            Model.ActionName = CurrentRoute.CurrentStep.ActionName;
            Model.PreviousStep = CurrentRoute.PreviousStep;
        }
        private void MoonWalkTill(string methodName)
        {
            while (CurrentRoute.CurrentStep.ActionName != methodName)
            {
                if (!TryMovePreviousStep())
                    break;
            }
        }
        private void WizardInit()
        {
            var current = Routes.First();
            current.Current = true;
            current.Steps.First().Current = true;
        }
    }
}