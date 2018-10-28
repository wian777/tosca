using System;
using Tricentis.Automation.AutomationInstructions.Dynamic.Values;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;

namespace Toolbox
{
    [SpecialExecutionTaskName("CustomedSetFailed")]
    public class SetFailed : SpecialExecutionTask
    {
        private const string c_SetFailed    = "SetFailed";
        private const string c_FailureTyp   = "FailureTyp";
        private const string c_Message      = "Message";
        private string t_setfailed  = " - ";
        private string t_failuretyp = " - ";
        private string t_message    = " - ";

        public SetFailed(Validator validator) : base(validator)
        {
        }

        public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
        {
            IInputValue setfailed  = testAction.GetParameterAsInputValue(c_SetFailed,  false);
            IInputValue failuretyp = testAction.GetParameterAsInputValue(c_FailureTyp, false);
            IInputValue message    = testAction.GetParameterAsInputValue(c_Message,    false);


            if (string.IsNullOrEmpty(setfailed.Value))
            { 
                t_setfailed = "Failed";
            }
            else
            {
                t_setfailed = setfailed.Value;
            }

            if (! string.IsNullOrEmpty(failuretyp.Value))
                t_failuretyp = failuretyp.Value;

            if (! string.IsNullOrEmpty(message.Value))
                t_message = message.Value;

            string message_all = "Failuretyp: " + t_failuretyp + "; Message: " + t_message;

            if (t_setfailed == "Failed")
            {
                try
                {
                    if (t_failuretyp == "Warn" || t_failuretyp == "Absence" || t_failuretyp == "Missing")
                        return new PassedActionResult("Test " + t_setfailed + " > " + message_all + " <");

                    if (t_failuretyp == "Other Failure")
                    {
                        return new PassedActionResult("Test " + t_setfailed + " > " + message_all + " <");
                    }
                    else
                    {
                        throw (new TestFailedException("Test " + t_setfailed + " > " + message_all + " <"));
                    }
                }
                catch (TestFailedException e)
                {
                    throw (new TestFailedException("Test " + t_setfailed + " > " + message_all + " <\r\n" , e));
                }
                finally
                {

                }
                
                // return new PassedActionResult("Test failed: " + message_all);                
            }

            return new PassedActionResult("Passed successfully: " + message_all);

        }
    }
}


[Serializable]
public class TestFailedException : Exception
{
    public TestFailedException() { }
    public TestFailedException(string message) : base(message) { }
    public TestFailedException(string message, Exception inner) : base(message, inner) { }
    protected TestFailedException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}