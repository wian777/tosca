using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace lbaseDotNetCustomControls.Reflection
{
    class RibbonComboBoxWrapper : RibbonItemWrapper, IClickableRibbonItem
    {
        public static string Value = "";
        public RibbonComboBoxWrapper(object item)
            : base(item)
        {
        }

        public override String Text
        {
            get { return (String)ReflectionHelper.ReflectionHelper.GetProperty(item, "Text"); }
        }


        public void Click()
        {
            //ReflectionHelper.ReflectionHelper.InvokeMethod(item, "j");//die Methode welche beim ändern des Checked Wertes aufgerufen wird.
            ((C1.Win.C1Ribbon.RibbonComboBox)this.item).SelectedIndex = ((C1.Win.C1Ribbon.RibbonComboBox)this.item).Items.IndexOf(Value);
            Thread.Sleep(300);
        }
    }
}
