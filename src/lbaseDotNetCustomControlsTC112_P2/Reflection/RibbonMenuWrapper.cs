using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lbaseDotNetCustomControls.Reflection
{
    public class RibbonMenuWrapper: RibbonDropDownBaseWrapper
    {
        public RibbonMenuWrapper(object item)
            : base(item)
        {
        }

        public override String Text
        {
            get { return (String)ReflectionHelper.ReflectionHelper.GetProperty(item, "Text"); }
        }

    }
}
