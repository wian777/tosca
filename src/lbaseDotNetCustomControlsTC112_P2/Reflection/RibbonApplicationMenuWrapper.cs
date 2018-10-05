using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lbaseDotNetCustomControls.Reflection
{
    public class RibbonApplicationMenuWrapper : RibbonDropDownBaseWrapper
    {
        public RibbonApplicationMenuWrapper(object item)
            : base(item)
        {
        }

        public override string Text
        {
            get { return (String)ReflectionHelper.ReflectionHelper.GetProperty(item, "Text"); }
        }
    }
}
