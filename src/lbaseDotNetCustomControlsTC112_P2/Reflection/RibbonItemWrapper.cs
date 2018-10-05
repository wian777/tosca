using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lbaseDotNetCustomControls.Reflection
{
    public abstract class RibbonItemWrapper : IWrappedObjectWithText
    {
        protected object item;

        public RibbonItemWrapper(object item)
        {
            this.item = item;
        }

        public bool Enabled
        {
            get
            {
                return (bool)ReflectionHelper.ReflectionHelper.GetProperty(item, "Enabled");
            }
        }
        
        public object WrappedObject
        {
            get { return item; }
        }

        public abstract String Text { get; }
    }
}
