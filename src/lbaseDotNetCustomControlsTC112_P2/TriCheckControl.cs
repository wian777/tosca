using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TricentisLibs;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using lbaseDotNetCustomControls.Helpers;

namespace lbaseDotNetCustomControls
{
    public class TriCheckControl : OperatorGenericCtrl
    {
        private System.Windows.Forms.Control ctrl;


        public TriCheckControl(System.Windows.Forms.Control ctrl)
        {
            this.ctrl = ctrl;
        }

        public override string getProperty(string name, string key)
        {
            switch (name.ToLower())
            {
                case "list":
                case "valuerange":
                    return String.Join(";", GetStateTexts());
                case "text":
                case "value":
                    return ReflectionHelper.ReflectionHelper.GetProperty(ctrl, "Text").ToString();
                //.InvokeMethod(ctrl, "PalTranslate").ToString();
            }
            return base.getProperty(name, key);
        }

        public override string getValue(string val, string param)
        {
            return getProperty("value", "");
        }

        public override string setValue(string val, string steering)
        {
            int clickCnt = 0;
            while (!StringHelpers.DoStringsMatch(getValue("", ""), val))
            {
                if (clickCnt++ >= 3)
                    return "Der Eingabewert >" + val + "< konnte nicht in der Checkbox gefunden werden!";
                Point clickPoint = ctrl.PointToScreen(new Point(ctrl.Width / 2, ctrl.Height / 2));
                TricentisLibs.MouseSteering.Click(clickPoint);
                Application.DoEvents();
                Thread.Sleep(10);
                Application.DoEvents();
            }
            return ReturnOK;
        }

        public override System.Windows.Forms.Control Ctrl
        {
            get { return ctrl; }
        }

        public override string RobotClass
        {
            get { return RobotClassComboBox; }
        }

        private List<String> GetStateTexts()
        {
            List<String> stateTexts = new List<String>();
            stateTexts.Add(ReflectionHelper.ReflectionHelper.GetField(ctrl, "sTextUnchecked").ToString());
            // ReflectionHelper.ReflectionHelper.InvokeMethod(
            stateTexts.Add(ReflectionHelper.ReflectionHelper.GetField(ctrl, "sTextChecked").ToString());
            stateTexts.Add(ReflectionHelper.ReflectionHelper.GetField(ctrl, "sTextUndefined").ToString());
            return stateTexts;
        }
    }
}
