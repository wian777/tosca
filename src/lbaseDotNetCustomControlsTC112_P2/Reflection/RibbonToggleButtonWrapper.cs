using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lbaseDotNetCustomControls.Exceptions;

namespace lbaseDotNetCustomControls.Reflection
{
    public class RibbonToggleButtonWrapper : RibbonItemWrapper, IClickableRibbonItem
    {
        public RibbonToggleButtonWrapper(object item)
            : base(item)
        {
        }

        public override String Text
        {
            get { return (String)ReflectionHelper.ReflectionHelper.GetProperty(item, "Text"); }
        }


        public void Click()
        {
            if (!Enabled)
                throw new NotEnabledException("Der Button >" + Text + "< ist nicht enabled!");

            var isPpjV45 = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == "PPJ.Runtime.45");
            // Logger.Instance.Log("Der Button >" + Text + "< wird geklickt.");

            if (!isPpjV45)
            {

                if (item.GetType() == typeof(C1.Win.C1Ribbon.RibbonToggleButton))
                {
                    ReflectionHelper.ReflectionHelper.InvokeMethod(item, "j"); // a OnClick is invoked with the "j" Method ;-) wian
                }
                else
                {
                    ReflectionHelper.ReflectionHelper.InvokeMethod(item, "k"); // a OnDoubleClick is invoked with the "k" Method ;-)

                    /*
                    * Mit folgendem Befehl während Debuggen im Direktfenster die Methoden von item ermitteln:
                    ReflectionHelper.ReflectionHelper.GetMethodNames(item)
                     * ...
                     * [31]: "OnClick"
                     * [32]: "k"
                     * [33]: "add_DoubleClick"
                     * [34]: "remove_DoubleClick"
                     * [35]: "OnDoubleClick"
                     * [36]: "h"
                     * ...
                     */
                }
            }
            else
            {
                // 20151022 wian:
                // Diese Änderung ist seit dem Wechsel von ppj framework auf PPJ.Runtime.45.dll mit Dateiversion 4.5.1056.646 ab lbase 6.4 nötig
                ReflectionHelper.ReflectionHelper.InvokeMethod(item, "j"); // a OnClick is invoked with the "j" Method ;-) wian
                /*
                 * Mit folgendem Befehl während Debuggen im Direktfenster die Methoden von item ermitteln:
                 ReflectionHelper.ReflectionHelper.GetMethodNames(item)
                 * ...
                 * [33]: "OnClick"
                 * [34]: "j"
                 * [35]: "add_DoubleClick"
                 * [36]: "remove_DoubleClick"
                 * [37]: "OnDoubleClick"
                 * [38]: "k"
                 * ...
                */
            }
        }
        /*
         * public RibbonToggleButtonWrapper(object obj)
        {
            // TODO: Complete member initialization
            this.obj = obj;
        }
        */

    }
}
