namespace BrickWizard
{
    public interface IWizard<T> where T : WizardModelBaseClass, new()
    {
        Route CurrentRoute { get; }
        Steps CurrentRouteSteps { get; }
        Step CurrentStep { get; }
        T Model { get; set; }

        void ClearUnusedSteps();
        void Sync(params object[] objs);
        void Sync<T1>(T1 obj);
    }
}