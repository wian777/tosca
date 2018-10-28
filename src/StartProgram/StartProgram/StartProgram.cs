using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Tricentis.Automation.AutomationInstructions.Dynamic.Values;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;

namespace Toolbox
{
    [SpecialExecutionTaskName("CustomedStartProgram")]
    public class StartProgram : SpecialExecutionTask
    {
        private const string Path = "Path";
        private const string Arguments = "Arguments";
        private const string Argument = "Argument";

        public StartProgram(Validator validator) : base(validator)
        {
        }

        public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
        {
            IInputValue path = testAction.GetParameterAsInputValue(Path, false);
            IParameter parameter = testAction.GetParameter(Arguments, true);
            string processArguments = string.Empty;

            if (path == null || string.IsNullOrEmpty(path.Value))
                throw new ArgumentException(string.Format("Mandatory parameter '{0}' not set.", Path));

            if (parameter != null)
            {
                IEnumerable<IParameter> arguments = parameter.GetChildParameters(Argument);

                foreach (IParameter argument in arguments)
                {
                    IInputValue processArgument = argument.Value as IInputValue;
                    processArguments += processArgument.Value + " ";
                }
            }

            try
            {
                Process.Start(path.Value, processArguments);
            }
            catch (Win32Exception)
            {
                return new UnknownFailedActionResult("Could not start program",
                                                     string.Format(
                                                         "Failed while trying to start:\nPath: {0}\r\nArguments: {1}",
                                                         path.Value, processArguments),
                                                     "");
            }

            return new PassedActionResult("Started successfully");
        }
    }
}
