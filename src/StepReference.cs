namespace BrickWizard
{
    public class StepReference
    {
        public StepReference(string actionName, int stepNumber)
        {
            ActionName = actionName;
            StepNumber = stepNumber;
        }
        internal string ActionName { get; }
        public int StepNumber { get; set; }
    }
}