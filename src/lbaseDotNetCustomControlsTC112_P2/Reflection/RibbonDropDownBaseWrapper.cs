using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lbaseDotNetCustomControls.Exceptions;
using System.Threading;

namespace lbaseDotNetCustomControls.Reflection
{
    public abstract class RibbonDropDownBaseWrapper : RibbonItemWrapper
    {
        public RibbonDropDownBaseWrapper(object item)
            : base(item)
        { 
        }

        public RibbonItemCollectionWrapper Items
        {
            get { return new RibbonItemCollectionWrapper(ReflectionHelper.ReflectionHelper.GetProperty(item, "Items")); }
        }

        public bool DroppedDown
        {
            get
            {
                return (bool)ReflectionHelper.ReflectionHelper.GetProperty(item, "DroppedDown");
            }
            set
            {
                if (!Enabled)
                {
                    throw new NotEnabledException("Das Item >" + Text + "< ist nicht enabled, um ein dropdown durchzufzuehren!");
                }
                else
                {
                    // Thread.Sleep(1000);
                    // Console.WriteLine("Habe 2 mal 1000 Milisekunden gewartet - wian");
                    ReflectionHelper.ReflectionHelper.SetProperty(item, "DroppedDown", value);
                    // Thread.Sleep(1000);
                }
            }
        }
    }
}
