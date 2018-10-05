using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using lbaseDotNetCustomControls.Exceptions;

namespace lbaseDotNetCustomControls.Reflection
{
    public class C1RibbonWrapper: IWrappedObject
    {
        Control ctrl;
        /* 20151022 wian: Statt "Control ribbon" jetzt "Control lribbon" - siehe Issue TSCRUM-1245
         */
        public C1RibbonWrapper(Control lribbon)
        {
            ctrl = lribbon;
        }

        public RibbonTabCollectionWrapper Tabs 
        {
            get { return new RibbonTabCollectionWrapper(ReflectionHelper.ReflectionHelper.GetProperty(ctrl, "Tabs")); }
        }

        public RibbonApplicationMenuWrapper ApplicationMenu
        {
            get { return new RibbonApplicationMenuWrapper(ReflectionHelper.ReflectionHelper.GetProperty(ctrl, "ApplicationMenu")); }
        }

        public RibbonTabGroupCollectionWrapper ContextualTabGroups
        {
            get { return new RibbonTabGroupCollectionWrapper(ReflectionHelper.ReflectionHelper.GetProperty(ctrl, "ContextualTabGroups")); }
        }

        public RibbonTabWrapper SelectedTab 
        {
            get {
                Object selectedTab = ReflectionHelper.ReflectionHelper.GetProperty(ctrl, "SelectedTab");
                if (selectedTab != null)
                    return new RibbonTabWrapper(selectedTab);
                return null;
            }
            set {
                if (!value.Enabled)
                    throw new NotEnabledException("Der Tab " + value.Text + " ist nicht enabled.");
                ReflectionHelper.ReflectionHelper.SetProperty(ctrl, "SelectedTab", value.WrappedObject);
            }
        }

        public object WrappedObject
        {
            get { return ctrl; }
        }
    }
}
