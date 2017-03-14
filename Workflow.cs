using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace BrickWizard
{
    public abstract class Workflow<T> : IWorkflow<T>  where T : WizardModelBaseClass, new()
    {
        protected Workflow()
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
            WorkflowInit();
            BaseModelSync();
        }
        private void WorkflowInit()
        {
            var current = Routes.First();
            current.Current = true;
            current.Steps.First().Current = true;
        }
        public void BaseModelSync()
        {
            Model.NavBar = NavBar;
            Model.ActionName = CurrentRoute.CurrentStep.ActionName;
            Model.PreviousStep = PreviousStep;
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
        public abstract void Orchestrate();
        public abstract List<Route> DefaultRoutes { get; }
        public abstract List<string> TriggerPoints { get; }
        public bool IsTriggerPointStep => TriggerPoints.Exists(x => x == CurrentRoute.CurrentStep.ActionName);
        public bool IsWorkflowInThisStep(string postMethodName) => CurrentRoute.CurrentStep.ActionName == postMethodName;
        public bool IsWorkflowInThisStep(int routeId, string actionName) => CurrentRoute.RouteId == routeId && IsWorkflowInThisStep(actionName);
        public List<Route> Routes { get; private set; }
        public Route CurrentRoute => Routes.FirstOrDefault(x => x.Current);
        public Step CurrentStep => CurrentRoute.CurrentStep;
        public Step PreviousStep=> CurrentRoute.PreviousStep;
        public bool TryMoveNextStep() { return CurrentRoute.TryIncrementRouteStep(); }
        public bool TryMovePreviousStep() { return CurrentRoute.TryDecrementRouteStep(); }
        public Route NextRoute => GetNeighbors(1);
        public Route PreviousRoute => GetNeighbors(-1);
        public void FollowTheRoute(int routeId)
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
            CurrentRoute.TryIncrementRouteStep();
        }
        private Route GetNeighbors(int i) => Routes.ElementAtOrDefault(Routes.FindIndex(x => x.Current) + i);






    }
}