using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lbaseDotNetCustomControls.Reflection
{
    public class RibbonLabelWrapper : RibbonItemWrapper
    {

        public RibbonLabelWrapper(object item)
            : base(item)
        {
        }

        public override String Text
        {
            get { return (String)ReflectionHelper.ReflectionHelper.GetProperty(item, "Text"); }
        }

    }
}
