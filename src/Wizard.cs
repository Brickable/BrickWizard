using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BrickWizard
{
    public abstract class Wizard<T> where T : WizardModelBaseClass, new()
    {
        protected Wizard(string controllerName,string areaName="")
        {
            _controllerName = controllerName;
            _areaName = areaName;
            _steps = Steps;
            _map = Map;
            BaseModelSync();
        }
        private string _controllerName { get;}
        private string _areaName { get; }

        private Steps _steps { get; set; }
        private Map _map { get; set; }
        private List<Step> _currentRouteSteps
        {
            get
            {
                var s = new List<Step>();
                foreach (var i in CurrentRoute.RouteSteps)
                {
                    s.Add(_steps.GetStepByActionName(i.ActionName));
                }
                return s;
            }
        }

        protected virtual int MaxTabs { get; } = 5;
        protected abstract Steps Steps { get; }
        protected abstract Map Map { get; }
        
        public Route CurrentRoute => _map.CurrentRoute;
        public Steps CurrentRouteSteps => new Steps(_currentRouteSteps);
        public Step CurrentStep => _steps.Current;
        public T Model { get; set; } = new T();
        /// <summary>
        /// Command that refresh the Model properties depending which object(s) you pass into. Analize your current wizard object state and decide what should be the next "move".
        /// </summary>
        /// <param name="obj">Source object to copy to the Model as destination</param>
        /// 
        public void Sync<T1>(T1 obj)
        {
            var callerMethodName = new StackTrace().GetFrame(1).GetMethod().Name;
            var currentStep = CurrentStep.ActionName;

            if (callerMethodName != currentStep)
            {
                MoonWalkPerformed = true;
                MoonWalkTill(callerMethodName);
            }
            else
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
        /// <summary>
        /// Command that refresh the Model properties depending which object(s) you pass into. Analize your current wizard object state and decide what should be the next "move".
        /// </summary>
        /// <param name="obj">set of Source objects to copy to the Model as destination</param>
        public void Sync(params object[] objs)
        {
            var callerMethodName = new StackTrace().GetFrame(1).GetMethod().Name;
            var currentStep = CurrentStep.ActionName;

            if (callerMethodName != currentStep)
            {
                MoonWalkPerformed = true;
                MoonWalkTill(callerMethodName);
            }

            else
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
        public void ClearUnusedSteps()
        {
            foreach (var step in _steps.steps)
            {
                if (_currentRouteSteps.FirstOrDefault(x => x.ActionName == step.ActionName) == null)
                {
                    foreach (var prop in typeof(T).GetProperties())
                    {
                        foreach (var i in step.PropertiesToBind)
                        {
                            if (i.ToString() == prop.Name)
                            {
                                var property = typeof(T).GetProperty(prop.Name);
                                property.SetValue(Model, null);
                            }
                        }
                    }
                }
            }
        }

        private Step NextStep => GetNeighbors(1);
        private Step PreviousStep => GetNeighbors(-1);
        private Step GetNeighbors(int i) => _currentRouteSteps.ElementAtOrDefault(_currentRouteSteps.FindIndex(x => x.Current) + i);
        private bool TryMoveNextStep() => TryIterateStep(true);
        private bool TryMovePreviousStep() => TryIterateStep(false);
        private bool TryIterateStep(bool isToIncrement)
        {
            Step active = (isToIncrement) ? NextStep : PreviousStep;
            if (active == null)
            {
                return false;
            }
            _steps.CleanSteps();
            active.Current = true;
            return true;
        }
        private bool IsTriggerPointStep => CurrentStep.TriggerPointRule != null;
        private bool MoonWalkPerformed { get; set; }
        private void CleanRoutes()=> _map.Routes.ForEach(x => x.Current = false);
        private void FollowTheRoute(int routeId)
        {
            if (CurrentRoute.RouteId == routeId)
            {
                TryMoveNextStep();
                return;
            }
            var currentStep = CurrentStep;
            var routeToJump = _map.GetRoute(routeId);
            _steps.CleanSteps();
            CleanRoutes();
            routeToJump.Current = true;
            _steps.SetCurrentStep(currentStep.ActionName);
            TryMoveNextStep();
        }
        private void Orchestrate()
        {
            if (!IsTriggerPointStep)
            {
                TryMoveNextStep();
            }
            else
            {
                FollowTheRoute(CurrentStep.TriggerPointRule.Invoke());
            }
        }
        private NavBar GetNavBar()
        {
            var navBarList = new List<Tab>();
            var currentStepNumber = CurrentRoute.RouteSteps.FirstOrDefault(x => x.ActionName == CurrentStep.ActionName).StepNumber;

            foreach (var i in CurrentRoute.RouteSteps)
            {
                var step = _steps.GetStepByActionName(i.ActionName);
                navBarList.Add(
                new Tab
                {
                    Current = step.Current,
                    Name = step.Name,
                    Number = i.StepNumber,
                    Action = step.ActionName,
                    Description = step.Description,
                    Enable = (i.StepNumber <= currentStepNumber) ? true : false
                });
            }
            var maxTabs = (MaxTabs >= CurrentRoute.Lenght) ? MaxTabs : CurrentRoute.Lenght;
            return new NavBar(maxTabs, navBarList,_controllerName,_areaName);
        }
        private void BaseModelSync()
        {
            this.Model.NavBar = GetNavBar();
            this.Model.ActionName = CurrentStep.ActionName;
            this.Model.PreviousStep = PreviousStep;
            this.Model.ControllerName = _controllerName;
            this.Model.AreaName = _areaName;
        }
        private void MoonWalkTill(string methodName)
        {
            while (CurrentStep.ActionName != methodName)
            {
                if (!TryMovePreviousStep())
                    break;
            }
        }
    }
}