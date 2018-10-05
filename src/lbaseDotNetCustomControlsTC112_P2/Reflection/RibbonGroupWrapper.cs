using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lbaseDotNetCustomControls.Reflection
{
    public class RibbonGroupWrapper : IWrappedObjectWithText
    {
        private object group;

        public RibbonGroupWrapper(object group)
        {
            this.group = group;
        }

        public String Text
        {
            get { return (String) ReflectionHelper.ReflectionHelper.GetProperty(group, "Text"); }
        }

        public RibbonItemCollectionWrapper Items
        {
            get{
                return new RibbonItemCollectionWrapper(ReflectionHelper.ReflectionHelper.GetProperty(group, "Items"));
            }
        }

        public bool Enabled
        {
            get
            {
                return (bool)ReflectionHelper.ReflectionHelper.GetProperty(group, "Enabled");
            }
        }

        public object WrappedObject
        {
            get { return group; }
        }
    }
}
