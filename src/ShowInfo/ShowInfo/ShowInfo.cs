using Tricentis.Automation.AutomationInstructions.Dynamic.Values;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;

namespace Toolbox
{
    [SpecialExecutionTaskName("CustomedShowInfo")]
    public class ShowInfo : SpecialExecutionTask
    {
        private const string c_ShowInfo = "ShowInfo";
        
        public ShowInfo(Validator validator) : base(validator)
        {
        }

        public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
        {
            IInputValue showinfo = testAction.GetParameterAsInputValue(c_ShowInfo, false);

            /*
            if (showinfo == null || string.IsNullOrEmpty(showinfo.Value))
                throw new ArgumentException(string.Format("Mandatory parameter '{0}' not set.", c_ShowInfo));
            */

            return new PassedActionResult("Info: " + showinfo.Value );
        }
    }
}
