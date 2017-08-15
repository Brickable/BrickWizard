namespace BrickWizard
{
    public class StepReference
    {
        public StepReference(string actionName, int stepNumber, int disableStepAfter=-1)
        {
            ActionName = actionName;
            StepNumber = stepNumber;
            DisableStepAfter = disableStepAfter;
        }
        internal string ActionName { get; }
        public int StepNumber { get; set; }
        public int DisableStepAfter { get; set; }
    }
}