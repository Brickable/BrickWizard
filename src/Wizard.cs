﻿using System;
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
        /// <summary>
        /// Property that define the max number of tabs to render on wizard nav bar
        /// </summary>
        protected virtual int MaxTabs { get; } = 5;
        /// <summary>
        /// Abstract property to be setted on specialized class definition.
        /// Property will define all steps of the wizard, its properties, properties to bind on commit for each step and trigger points of each step.
        /// </summary>
        protected abstract Steps Steps { get; }
        /// <summary>
        /// Abstract property to be setted on specialized class definition.
        /// Property will define all routes available in wizard and the steps for each route.
        /// </summary>
        protected abstract Map Map { get; }

        //PUBLIC MEMBERS
        /// <summary>
        /// Check if the step is not Frozen. frozen steps are defined on instatiation stage and they are not available for its lifetime.
        /// </summary>
        public bool IsStepAvailable(string stepActionName) => _steps.steps.Any(x => x.ActionName == stepActionName);       
        /// <summary>
        /// Based on callerMemberName and current step method infere if the sync will procceed with next step or moonwalk n steps
        /// </summary>
        /// <param name="callerMemberName">By convention if you dont pass it will be filled with the name of this method </param>
        public bool IsMoonWalkNeeded([CallerMemberName] string callerMemberName = "")
        {
            AssertIfCallerMemberNameIsValid(callerMemberName);
            return (callerMemberName != CurrentStep.ActionName);
        }
        /// <summary>
        /// Read Property that retrieve the current route
        /// </summary>
        public Route CurrentRoute => _map.CurrentRoute;
        /// <summary>
        /// Read Property that retrieve the current step
        /// </summary>
        public Step CurrentStep => _steps.Current;
        /// <summary>
        /// Generic Model of the specialized wizard classes.
        /// </summary>
        public T Model { get; set; } = new T();

        //PUBLIC COMMANDS
        /// <summary>
        /// Based on caller, on data in T Model and initial instance configuration declaratons of the wizard instance, 
        /// Sync Command will reason if needs to step n steps back, move to the next step or change to another route available.
        /// </summary>
        /// <param name="callerMemberName">By convention if you dont pass it will be filled with the name of this method </param>
        public void Sync([CallerMemberName] string callerMemberName = "")
        {
            if (!TryMoonWalkMove(callerMemberName))
            {
                Next();
            }
            BaseModelSync();
        }
        /// <summary>
        /// Bind the passed param objects To the T Model Object of Wizard instance
        /// </summary>
        /// <param name="objs">Collection of Objects to be bind with T Model</param>
        public void Commit(params object[] objs)
        {
            foreach (var obj in objs ?? Enumerable.Empty<object>())
            {
                var propertyInfo = typeof(T).GetProperties().First(x => x.PropertyType.FullName == obj.GetType().FullName);
                var property = typeof(T).GetProperty(propertyInfo.Name);
                property.SetValue(Model, obj);
            }
        }
        /// <summary>
        /// Bind entire T Model Object of Wizard instance if the current Step has no PropertiesToBind.
        /// Otherwise bind just the properties setted on current step CurrentStep.PropertiesToBind.
        /// </summary>
        /// <param name="model">T object to be bind with T Model</param>
        public void Commit(T model)
        {
            if(CurrentStep.PropertiesToBind == null || !CurrentStep.PropertiesToBind.Any())
            {
                this.Model = model;
            }
            else
            {
                foreach (var i in CurrentStep.PropertiesToBind)
                {
                    var m = model.GetType().GetProperty(i).GetValue(model, null);
                    typeof(T).GetProperty(i).SetValue(Model, m);
                }
            }
        }
        /// <summary>
        /// Clear all properties of T Model that not attached in the steps of current route from propertyNamesToBind at step constructor.
        /// </summary>
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
        /// <summary>
        /// After checking that MoonWalking is not needed, Bind the passed param objects To the T Model Object of Wizard instance.
        /// </summary>
        /// <param name="callerMemberName">By convention if you dont pass it will be filled with the name of this method </param>
        /// <param name="objs">Collection of Objects to be bind with T Model</param>
        public bool TryCommit([CallerMemberName] string callerMemberName = "", params object[] objs)
        {
            var isMoonWalkNeeded = IsMoonWalkNeeded(callerMemberName);
            if (!isMoonWalkNeeded)
            {
                Commit(objs);
            }
            return isMoonWalkNeeded;
        }
        /// <summary>
        /// After checking that MoonWalking is not needed, Bind entire T Model Object of Wizard instance if the current Step has no PropertiesToBind.
        /// Otherwise bind just the properties setted on current step CurrentStep.PropertiesToBind.
        /// </summary>
        /// <param name="model">T object to be bind with T Model</param>
        /// <param name="callerMemberName">By convention if you dont pass it will be filled with the name of this method </param>
        public bool TryCommit(T model, [CallerMemberName] string callerMemberName = "")
        {
            var isMoonWalkNeeded = IsMoonWalkNeeded(callerMemberName);
            if (!isMoonWalkNeeded)
            {
                Commit(model);
            }
            return isMoonWalkNeeded;
        }
        /// <summary>
        /// Execute TryCommit and Sync commands 
        /// </summary>
        /// <param name="model">T object to be bind with T Model</param>
        /// <param name="callerMemberName">By convention if you dont pass it will be filled with the name of this method </param>
        public bool TryCommitAndSync(T model, [CallerMemberName] string callerMemberName = "")
        {
            var isMoonWalkNeeded = IsMoonWalkNeeded(callerMemberName);
            if (!isMoonWalkNeeded)
            {
                Commit(model);
            }
            Sync(callerMemberName);
            return isMoonWalkNeeded;
        }
        /// <summary>
        /// Execute TryCommit and Sync commands
        /// </summary>
        /// <param name="callerMemberName">By convention if you dont pass it will be filled with the name of this method </param>
        /// <param name="objs">Collection of Objects to be bind with T Model</param>
        public bool TryCommitAndSync([CallerMemberName] string callerMemberName = "", params object[] objs)
        {
            var isMoonWalkNeeded = IsMoonWalkNeeded(callerMemberName);
            if (!isMoonWalkNeeded)
            {
                Commit(objs);
            }
            Sync(callerMemberName);
            return isMoonWalkNeeded;
        }
        /// <summary>
        /// Try to Go back N steps until current step action name is equal to callerMemberName
        /// </summary>
        /// <param name="callerMemberName">By convention if you dont pass it will be filled with the name of this method </param>
        public bool TryMoonWalking([CallerMemberName] string callerMemberName = "") => TryMoonWalkMove(callerMemberName, true);
        /// <summary>
        /// Based on definition of wizard specialization class decide if move to the next step or jump to another route
        /// </summary>
        /// <param name="callerMemberName">By convention if you dont pass it will be filled with the name of this method </param>
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
        private void MoveNext(string callerMemberName, bool syncBaseModel = false)
        {
            AssertIfCallerMemberNameIsValid(callerMemberName);
            Next();
            if (syncBaseModel)
            {
                BaseModelSync();
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
                {
                    break;
                }     
            }
        }
        private bool IsCallerMemberNameValid(string callerMemberName)
        {
            return _steps.steps.Any(x => x.ActionName == callerMemberName);
        }
        private bool TryMoonWalkMove([CallerMemberName] string callerMemberName = "", bool syncBaseModel = false)
        {
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
        private int GetNavBarStatingPointIndex()
        {
            var currentStep = CurrentRoute.RouteSteps.FirstOrDefault(x => x.ActionName == CurrentStep.ActionName);
            var lastStep = CurrentRoute.RouteSteps.Last();

            var currentStepIndex = CurrentRoute.RouteSteps.IndexOf(currentStep);
            var lastStepIndex = CurrentRoute.RouteSteps.IndexOf(lastStep);
            var middleIndex = Convert.ToInt16(MaxTabs / 2) + 1;

            //Navbar has 3 behaviors: (GoingTowardsMiddle, Rolling, GoingTowardsEnd)
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