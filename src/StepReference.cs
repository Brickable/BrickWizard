namespace BrickWizard
{
    public class StepReference
    {
        public StepReference(string actionName, int disableStepAfter)
        {
            ActionName = actionName;
            DisableStepAfter = disableStepAfter;
        }
        public StepReference(string actionName)
        {
            ActionName = actionName;
            DisableStepAfter = -1;
        }
        internal string ActionName { get; }
        public int StepNumber { get; set; }
        public int DisableStepAfter { get;}
    }
}