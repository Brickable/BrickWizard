namespace BrickWizard
{
    public class Step
    {
        public Step(string postMethodName, string viewName, string name,int stepNumber, string descrption="")
        {
            ActionName = postMethodName;
            ViewName = viewName;
            Name = name;
            StepNumber = stepNumber;
            Description = descrption;
        }
        public string ActionName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StepNumber { get; set; }
        protected internal bool Current { get; set; }
        public string ViewName { get; set; }
    }
}