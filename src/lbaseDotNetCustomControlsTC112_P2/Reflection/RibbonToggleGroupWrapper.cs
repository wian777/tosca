using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lbaseDotNetCustomControls.Exceptions;

namespace lbaseDotNetCustomControls.Reflection
{
    class RibbonToggleGroupWrapper : RibbonToggleButtonWrapper  // RibbonItemWrapper, IClickableRibbonItem
    {
        public static string Value = "";

        public RibbonToggleGroupWrapper(object item)
            : base(item)
        {
            // 
        }

        public override String Text
        {
            // wie erhalte ich bei OPTIONEN den Item Text rein?
            // OPTIONEN->Trace Level->Alles oder OPTIONEN->Trace Level->Error
            // Siehe RibbonCollectionWrapper
            get { return ""; }
        }

        public new void Click()
        {
            if (!Enabled)
                throw new NotEnabledException("Der Button >" + Text + "< ist nicht enabled!");
            // throw new NotImplementedException();
            // ReflectionHelper.ReflectionHelper.GetMethodNames(item)
            // ReflectionHelper.ReflectionHelper.GetMethodNames(item).Skip(100).ToArray()
            ReflectionHelper.ReflectionHelper.InvokeMethod(item, "j");

        }
    }
}
