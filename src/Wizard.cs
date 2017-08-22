using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BrickWizard.ExtensionMethods;
using System.Runtime.CompilerServices;

namespace BrickWizard
{
    public abstract class Wizard<T> where T : WizardModelBaseClass, new()
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
        protected abstract Steps Steps { get; }
        protected abstract Map Map { get; }
        protected virtual int MaxTabs { get; } = 5;    

        //PUBLIC MEMBERS
        public bool IsStepAvailable(string stepActionName) => _steps.steps.Any(x => x.ActionName == stepActionName);
        public Route CurrentRoute => _map.CurrentRoute;
        public Steps CurrentRouteSteps => new Steps(_currentRouteSteps);
        public Step CurrentStep => _steps.Current;
        public T Model { get; set; } = new T();

        //PUBLIC COMMANDS
<<<<<<< HEAD
        public void Sync([CallerMemberName] string callerMemberName = "")
        {
            if (!TryMoonWalkMove(callerMemberName))
            {
                Next();
            }
            BaseModelSync();
        }
        public void Commit(params object[] objs)
        {
            foreach (var obj in objs ?? Enumerable.Empty<object>())
            {
                var propertyInfo = typeof(T).GetProperties().First(x => x.PropertyType.FullName == obj.GetType().FullName);
                var property = typeof(T).GetProperty(propertyInfo.Name);
                property.SetValue(Model, obj);
            }
        }
        public void Commit(T model)
        {
            foreach (var i in CurrentStep.PropertiesToBind ?? Enumerable.Empty<string>())
            {
                var m = model.GetType().GetProperty(i).GetValue(model, null);
                typeof(T).GetProperty(i).SetValue(Model, m);
            }
=======
        public void Sync() => Sync(new StackTrace().GetFrame(1).GetMethod().Name);
        public void ForceCommit(params object[] objs) => ForceCommit(new StackTrace().GetFrame(1).GetMethod().Name, objs);
        public void Commit(T model) => Commit(new StackTrace().GetFrame(1).GetMethod().Name, model);     
        public void CommitAndSync(T model)
        {
            var callerMethodName = new StackTrace().GetFrame(1).GetMethod().Name;
            Commit(callerMethodName, model);
            Sync(callerMethodName);
        }
        public void ForceCommitAndSync(params object[] objs)
        {
            var callerMethodName = new StackTrace().GetFrame(1).GetMethod().Name;
            ForceCommit(callerMethodName, objs);
            Sync(callerMethodName);
>>>>>>> 290284b3346bca52bb40a99616ffa09f7b284a38
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
        public bool TryCommit([CallerMemberName] string callerMemberName = "", params object[] objs)
        {
            AssertIfCallerMemberNameIsValid(callerMemberName);
            var isMoonWalkNeeded = IsMoonWalkNeeded(callerMemberName);
            if (!isMoonWalkNeeded)
            {
                foreach (var obj in objs ?? Enumerable.Empty<object>())
                {
                    var propertyInfo = typeof(T).GetProperties().First(x => x.PropertyType.FullName == obj.GetType().FullName);
                    var property = typeof(T).GetProperty(propertyInfo.Name);
                    property.SetValue(Model, obj);
                }
            }
            return isMoonWalkNeeded;
        }
<<<<<<< HEAD
        public bool TryCommit(T model, [CallerMemberName] string callerMemberName = "")
        {
            AssertIfCallerMemberNameIsValid(callerMemberName);
            var isMoonWalkNeeded = IsMoonWalkNeeded(callerMemberName);
            if (!isMoonWalkNeeded)
=======
        private void ForceCommit(string callerMethodName, params object[] objs)
        {
            if (!IsMoonWalkNeeded(callerMethodName))
            {
                foreach (var obj in objs ?? Enumerable.Empty<object>())
                {
                    var propertyInfo = typeof(T).GetProperties().First(x => x.PropertyType.FullName == obj.GetType().FullName);
                    var property = typeof(T).GetProperty(propertyInfo.Name);
                    property.SetValue(Model, obj);
                }
            }
        }
        private void Commit(string callerMethodName, T model)
        {
            if (!IsMoonWalkNeeded(callerMethodName))
            {
                foreach (var i in CurrentStep.PropertiesToBind ?? Enumerable.Empty<string>())
                {
                    var m = model.GetType().GetProperty(i).GetValue(model, null);
                    typeof(T).GetProperty(i).SetValue(Model, m);
                }
            }
        }
        private bool TryMoveBackwards() => TryBackwardsMove(new StackTrace().GetFrame(1).GetMethod().Name);
        private bool TryBackwardsMove(string callerMethodName)
        {
            var isMoonWalkNeeded = IsMoonWalkNeeded(callerMethodName);
            if (isMoonWalkNeeded)
>>>>>>> 290284b3346bca52bb40a99616ffa09f7b284a38
            {
                foreach (var i in CurrentStep.PropertiesToBind ?? Enumerable.Empty<string>())
                {
                    var m = model.GetType().GetProperty(i).GetValue(model, null);
                    typeof(T).GetProperty(i).SetValue(Model, m);
                }
            }
            return isMoonWalkNeeded;
        }
        public bool TryCommitAndSync(T model, [CallerMemberName] string callerMemberName = "")
        {
            AssertIfCallerMemberNameIsValid(callerMemberName);
            var isMoonWalkNeeded = IsMoonWalkNeeded(callerMemberName);
            if (!isMoonWalkNeeded)
            {
                Commit(model);
            }
            Sync(callerMemberName);
            return isMoonWalkNeeded;
        }
        public bool TryCommitAndSync([CallerMemberName] string callerMemberName = "", params object[] objs)
        {
            AssertIfCallerMemberNameIsValid(callerMemberName);
            var isMoonWalkNeeded = IsMoonWalkNeeded(callerMemberName);
            if (!isMoonWalkNeeded)
            {
                Commit(objs);
            }
            Sync(callerMemberName);
            return isMoonWalkNeeded;
        }
        public bool TryMoonWalking([CallerMemberName] string callerMemberName = "") => TryMoonWalkMove(callerMemberName, true);
        public void MoveNext([CallerMemberName] string callerMemberName = "") => MoveNext(callerMemberName, true);

        //PRIVATE MEMBERS & COMMANDS
        private void AssertIfCallerMemberNameIsValid(string callerMemberName)
        {
            if (!IsCallerMemberNameValid(callerMemberName))
            {
                throw new ArgumentException($"callerMemberName argument with value {callerMemberName} passed in Sync Method is not valid.");
            }
        }
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
        private void Next()
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
<<<<<<< HEAD
        private void MoveNext(string callerMemberName, bool syncBaseModel = false)
        {
            AssertIfCallerMemberNameIsValid(callerMemberName);
            Next();
            if (syncBaseModel)
            {
                BaseModelSync();
=======
        private void MoonWalkTill(string methodName)
        {
            while (CurrentStep.ActionName != methodName)
            {
                if (!TryMovePreviousStep())
                {
                    break;
                } 
>>>>>>> 290284b3346bca52bb40a99616ffa09f7b284a38
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
<<<<<<< HEAD
        private void MoonWalkTill(string methodName)
        {
            while (CurrentStep.ActionName != methodName)
            {
                if (!TryMovePreviousStep())
                    break;
            }
        }
        private bool IsCallerMemberNameValid(string callerMemberName)
        {
            return _steps.steps.Any(x => x.ActionName == callerMemberName);
        }
        private bool TryMoonWalkMove([CallerMemberName] string callerMemberName = "", bool syncBaseModel = false)
        {
            AssertIfCallerMemberNameIsValid(callerMemberName);
            var isMoonWalkNeeded = IsMoonWalkNeeded(callerMemberName);
            if (isMoonWalkNeeded)
            {
                MoonWalkTill(callerMemberName);
                if (syncBaseModel)
                {
                    BaseModelSync();
                }
            }
            return isMoonWalkNeeded;
        }
        private bool IsMoonWalkNeeded(string callerMemberName) => (callerMemberName != CurrentStep.ActionName);
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
=======
>>>>>>> 290284b3346bca52bb40a99616ffa09f7b284a38
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