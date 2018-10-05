using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lbaseDotNetCustomControls.Reflection
{
    public class RibbonControlHostWrapper : RibbonItemWrapper //, IClickableRibbonItem
    {
        // public object obj;

        public RibbonControlHostWrapper(object item)
            : base(item)
        {

        }
        
        public override String Text
        {
            get { return (String)ReflectionHelper.ReflectionHelper.GetProperty(item, "Text"); }
        }
    }
}
