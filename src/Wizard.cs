using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BrickWizard.ExtensionMethods;

namespace BrickWizard
{
    public abstract class Wizard<T> : IWizard<T> where T : WizardModelBaseClass, new()
    {
        protected Wizard(string controllerName, string areaName = "", string[] frozenSteps = null)
        {
            _controllerName = controllerName;
            _areaName = areaName;
            _steps = (frozenSteps == null) ? Steps : Steps.FreezeSteps(frozenSteps);
            _map = (frozenSteps == null) ? Map : Map.FreezeSteps(frozenSteps);
            BaseModelSync();
        }
        protected Wizard(string controllerName, int startAtRouteId, string startAtStepActionName, string areaName = "", string[] frozenSteps = null)
        {
            _controllerName = controllerName;
            _areaName = areaName;
            _steps = (frozenSteps == null) ? Steps : Steps.FreezeSteps(frozenSteps);
            _map = (frozenSteps == null) ? Map : Map.FreezeSteps(frozenSteps);
            SetCoordinatesAt(startAtRouteId, startAtStepActionName);
            BaseModelSync();
        }

        
        //PRIVATE FIELDS
        private string _controllerName { get; }
        private string _areaName { get; }

        //PRIVATE PROPERTIES
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

        //OVERRIDABLE MEMBERS
        protected virtual int MaxTabs { get; } = 5;
        protected abstract Steps Steps { get; }
        protected abstract Map Map { get; }

        //PUBLIC MEMBERS
        public Route CurrentRoute => _map.CurrentRoute;
        public Steps CurrentRouteSteps => new Steps(_currentRouteSteps);
        public Step CurrentStep => _steps.Current;
        public bool IsStepAvailable(string stepActionName)=> _steps.steps.Any(x => x.ActionName == stepActionName);
        public T Model { get; set; } = new T();

        //PUBLIC COMMANDS
        public void AutoSync()
        {
            if (TryBackwardsMove(new StackTrace().GetFrame(1).GetMethod().Name))
            {
                MoveNext();
            }            
        }
        public bool TryMoveBackwards() => TryBackwardsMove(new StackTrace().GetFrame(1).GetMethod().Name);
        public void TryForceCommit(params object[] objs)
        {
            if (IsMoonWalkNeeded(new StackTrace().GetFrame(1).GetMethod().Name) && objs != null)
            {                
                foreach (var obj in objs)
                {
                    var propertyInfo = typeof(T).GetProperties().First(x => x.PropertyType.FullName == obj.GetType().FullName);
                    var property = typeof(T).GetProperty(propertyInfo.Name);
                    property.SetValue(Model, obj);
                }      
            }
        }
        public void TryCommit(T model)
        {
            if (IsMoonWalkNeeded(new StackTrace().GetFrame(1).GetMethod().Name))
            {
                foreach(var i in CurrentStep.PropertiesToBind ?? Enumerable.Empty<string>())
                {

                }

                //foreach (var step in _steps.steps)
                //{
                //    if (_currentRouteSteps.FirstOrDefault(x => x.ActionName == step.ActionName) == null)
                //    {
                //        foreach (var prop in typeof(T).GetProperties())
                //        {
                //            foreach (var i in step.PropertiesToBind)
                //            {
                //                if (i.ToString() == prop.Name)
                //                {
                //                    var property = typeof(T).GetProperty(prop.Name);
                //                    property.SetValue(Model, null);
                //                }
                //            }
                //        }
                //    }
                //}
                //foreach (var obj in objs)
                //{
                //    var propertyInfo = typeof(T).GetProperties().First(x => x.PropertyType.FullName == obj.GetType().FullName);
                //    var property = typeof(T).GetProperty(propertyInfo.Name);
                //    property.SetValue(Model, obj);
                //}
            }
        }
        public void CommitAndAutoSync(T model)
        {
            TryCommit(model);
            AutoSync();
        }
        public void CommitAndAutoSync(params object[] objs)
        {
            TryForceCommit(objs);
            AutoSync();
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

        //PRIVATE MEMBERS & COMMANDS
        private bool TryBackwardsMove(string callerMethodName)
        {
            var isMoonWalkNeeded = IsMoonWalkNeeded(callerMethodName);
            if (IsMoonWalkNeeded(callerMethodName))
            {
                MoonWalkTill(callerMethodName);
                BaseModelSync();
            }
            return isMoonWalkNeeded;
        }
        private bool IsMoonWalkNeeded(string callerMethodName) => (callerMethodName != CurrentStep.ActionName);
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
        private void CleanRoutes() => _map.Routes.ForEach(x => x.Current = false);
        private void SetCoordinatesAt(int routeId, string actionName)
        {
            SetCurrentRoute(routeId);
            _steps.SetCurrentStep(actionName);
        }
        private void SetCurrentRoute(int routeId)
        {
            CleanRoutes();
            _map.GetRoute(routeId).Current = true;
        }
        private void FollowTheRoute(int routeId)
        {
            if (CurrentRoute.RouteId == routeId)
            {
                TryMoveNextStep();
                return;
            }
            SetCoordinatesAt(routeId, CurrentStep.ActionName);
            TryMoveNextStep();
        }
        private void MoveNext()
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
        private void BaseModelSync()
        {
            this.Model.NavBar = GetNavBar();
            this.Model.ActionName = CurrentStep.ActionName;
            this.Model.CurrentRouteId = CurrentRoute.RouteId;
            this.Model.PreviousStepActionName = PreviousStep?.ActionName;
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
        private int GetNavBarStatingPointIndex()
        {
            var currentStep = CurrentRoute.RouteSteps.FirstOrDefault(x => x.ActionName == CurrentStep.ActionName);
            var lastStep = CurrentRoute.RouteSteps.Last();

            var currentStepIndex = CurrentRoute.RouteSteps.IndexOf(currentStep);
            var lastStepIndex = CurrentRoute.RouteSteps.IndexOf(lastStep);
            var middleIndex = Convert.ToInt16(MaxTabs / 2) + 1;

            //Navbar has 3 behaviors: {GoingTowardsMiddle, Rolling, GoingTowardsEnd}
            var GoingTowardsEnd = currentStepIndex + middleIndex > lastStepIndex;
            var GoingTowardsMiddle = currentStepIndex < middleIndex;

            if (GoingTowardsEnd)
            {
                return lastStepIndex - (MaxTabs - 1);
            }
            else if (GoingTowardsMiddle)
            {
                return 0;
            }
            else
            {
                return currentStepIndex - (middleIndex - 1);
            }
        }
        private Step NextStep => GetNeighbors(1);
        private Step PreviousStep => GetNeighbors(-1);
        private Step GetNeighbors(int i) => _currentRouteSteps.ElementAtOrDefault(_currentRouteSteps.FindIndex(x => x.Current) + i);
        private List<StepReference> StepsToShow()
        {
            if (CurrentRoute.Lenght <= MaxTabs)
            {
                return CurrentRoute.RouteSteps;
            }
            else
            {
                var stepsToShow = new List<StepReference>();
                int i = GetNavBarStatingPointIndex();
                var stepsTaken = 0;
                while (stepsTaken < MaxTabs)
                {
                    stepsToShow.Add(CurrentRoute.RouteSteps[i]);
                    stepsTaken++;
                    i++;
                }
                return stepsToShow;
            }
        }
        private NavBar GetNavBar()
        {
            var navBarList = new List<Tab>();
            var currentStepNumber = CurrentRoute.RouteSteps.FirstOrDefault(x => x.ActionName == CurrentStep.ActionName).StepNumber;

            foreach (var i in StepsToShow())
            {
                var step = _steps.GetStepByActionName(i.ActionName);

                var pastTabRule = i.StepNumber <= currentStepNumber;
                var blockPastTabRule = i.DisableStepAfter > 0 && currentStepNumber > i.DisableStepAfter;

                navBarList.Add(
                new Tab
                {
                    Current = step.Current,
                    Name = step.Name,
                    Number = i.StepNumber,
                    Action = step.ActionName,
                    Description = step.Description,
                    Enable = (pastTabRule && !blockPastTabRule) ? true : false
                });
            }
            var maxTabs = (MaxTabs >= CurrentRoute.Lenght) ? MaxTabs : CurrentRoute.Lenght;
            return new NavBar(maxTabs, navBarList, _controllerName, _areaName);
        }
    }
}