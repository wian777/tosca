using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using lbaseDotNetCustomControls.Exceptions;

namespace lbaseDotNetCustomControls.Reflection
{
    public class RibbonSplitButtonWrapper : RibbonDropDownBaseWrapper, IClickableRibbonItem
    {
        public RibbonSplitButtonWrapper(object item)
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
                throw new NotEnabledException("Der SplitButton >" + Text + "< ist nicht enabled!");

            /*
             * 20171017 wian
             * ACHTUNG: seit 17.10. ist auf PPJ 46 gewechselt worden
             * ab lbase 7.2 nötig
             * 20151023 wian
             * ACHTUNG: seit dem Wechsel von ppj framework auf PPJ.Runtime.45.dll mit Dateiversion 4.5.1056.646 
             * var isPpjV45 = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == "PPJ.Runtime.45");
             * ab lbase 6.4 nötig
             * Es kann vorkommen, dass sich bei Wechsel von ppj framwork die InvokeMethode ändert (siehe z.B. RibbonButtonWrapper)!
             * Mit folgendem Befehl während Debuggen im Direktfenster die Methoden von item ermitteln:
             ReflectionHelper.ReflectionHelper.GetMethodNames(item)
             * ...
             * [36]: "OnClick"
             * [37]: "i"
             * ...
             */
            Thread.Sleep(300);
            ReflectionHelper.ReflectionHelper.InvokeMethod(item, "i");  // a Click is invoked with the "i" Method ;-)
            Thread.Sleep(300);
        }
    }
}
