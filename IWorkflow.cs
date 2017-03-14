using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BrickWizard
{
    public interface IWorkflow<T> where T : class, new()
    {
        T Model { get; set; }
        void Orchestrate();
        void BaseModelSync();
        List<string> TriggerPoints { get; }
        bool IsTriggerPointStep { get; }
        bool IsWorkflowInThisStep(string postMethodName);
        bool IsWorkflowInThisStep(int routeId, string postMethodName);
        List<Route> Routes { get;}
        Route CurrentRoute { get; }
        Step CurrentStep { get; }
        Step PreviousStep { get; }
        void FollowTheRoute(int routeId);
        bool TryMoveNextStep();
        bool TryMovePreviousStep();
        Route NextRoute { get; }
        Route PreviousRoute { get; }
    }
}
