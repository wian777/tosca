using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TricentisLibs;
using System.Windows.Forms;

namespace lbaseDotNetCustomControls
{
    public class TOSCAOperator : ITOSCAOperator
    {
        public OperatorGenericCtrl getCustomControl(paramAction pa)
        {
            Control ctrl = Control.FromHandle(pa.PtHwnd);
            if (ctrl != null)
            {
                if (ctrl is MdiClient)
                    return new MdiClientControl(ctrl);
                // Find Reflection Controls
                Type currType = ctrl.GetType();
                do
                {
                    Logger.Instance.Log(this, "Searching Custom Control for " + currType.FullName);
                    switch (currType.FullName)
                    {
                        case "lbase.ppj.common.Controls.clsTriCheck":
                            return new TriCheckControl(ctrl);
                        case "PPJ.Runtime.Windows.C1RibbonBarEx":
                        case "C1.Win.C1Ribbon.C1Ribbon":
                            return new C1Ribbon(ctrl);
                        //case "PPJ.Runtime.Windows.C1RibbonBarEx":
                        //  return new C1RibbonBarEx(ctrl);
                        case "C1.Win.C1FlexGrid.C1FlexGrid":
                            return new C1FlexGrid(ctrl);
                        default:
                            Logger.Instance.Log(this, "No Control found for " + currType.FullName);
                            break;
                    }
                    currType = currType.BaseType;
                } while (currType != null && currType != typeof(Control));
            }
            return null;
        }
    }
}
