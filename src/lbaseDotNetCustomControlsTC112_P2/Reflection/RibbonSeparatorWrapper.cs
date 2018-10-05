using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lbaseDotNetCustomControls.Exceptions;

namespace lbaseDotNetCustomControls.Reflection
{
    public class RibbonSeparatorWrapper : RibbonItemWrapper
// public class RibbonLabelWrapper : RibbonItemWrapper
    {

        public RibbonSeparatorWrapper(object item)
            : base(item)
        {
        }

        public override String Text
        {
            // get { return (String)ReflectionHelper.ReflectionHelper.GetProperty(item, "Text"); }
            get { return ""; }
        }

    }
}
