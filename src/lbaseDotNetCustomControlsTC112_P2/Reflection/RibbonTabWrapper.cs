using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lbaseDotNetCustomControls.Reflection
{
    public class RibbonTabWrapper : IWrappedObjectWithText
    {
        private object tab;

        public RibbonTabWrapper(object tab)
        {
            this.tab = tab;
        }

        public String Text 
        {
            get { return (String)ReflectionHelper.ReflectionHelper.GetProperty(tab, "Text"); }
        }

        public RibbonGroupCollectionWrapper Groups
        {
            get { return new RibbonGroupCollectionWrapper(ReflectionHelper.ReflectionHelper.GetProperty(tab, "Groups")); }
        }

        public bool Enabled
        {
            get
            {
                return (bool)ReflectionHelper.ReflectionHelper.GetProperty(tab, "Enabled");
            }
        }

        public object WrappedObject
        {
            get { return tab; }
        }
    }
}
