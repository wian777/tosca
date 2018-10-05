using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace lbaseDotNetCustomControls.Reflection
{
    public class RibbonCheckBoxWrapper : RibbonItemWrapper , IClickableRibbonItem
    {

        public RibbonCheckBoxWrapper(object item)
            : base(item)
        {
        }

        public override String Text
        {
            get { return (String)ReflectionHelper.ReflectionHelper.GetProperty(item, "Text"); }
        }


        public void Click()
        {
            // ReflectionHelper.ReflectionHelper.GetMethodNames(item)
            // ReflectionHelper.ReflectionHelper.GetMethodNames(item).Skip(100).ToArray()
            ReflectionHelper.ReflectionHelper.InvokeMethod(item, "j");//die Methode welche beim ändern des Checked Wertes aufgerufen wird.
            Thread.Sleep(300);
        }
    }
}
