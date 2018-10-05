using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lbaseDotNetCustomControls.Reflection
{
    public class RibbonContextualTabGroupWrapper:IWrappedObjectWithText
    {
        private object obj;

        public RibbonContextualTabGroupWrapper(object obj)
        {
            this.obj = obj;
        }

        public string Text
        {
            get { return (String)ReflectionHelper.ReflectionHelper.GetProperty(obj, "Text"); }
        }

        public RibbonTabCollectionWrapper Tabs { get { return new RibbonTabCollectionWrapper(ReflectionHelper.ReflectionHelper.GetProperty(obj, "Tabs")); } }

        public object WrappedObject
        {
            get { return obj; }
        }
    }
}
