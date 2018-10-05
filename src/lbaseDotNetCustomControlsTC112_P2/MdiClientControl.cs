using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TricentisLibs;
using System.Windows.Forms;
using lbaseDotNetCustomControls.Helpers;

namespace lbaseDotNetCustomControls
{
    public class MdiClientControl : OperatorGenericCtrl
    {
        private MdiClient mdiClient;

        public MdiClientControl(Control ctrl)
        {
            mdiClient = (MdiClient)ctrl;
        }

        public override string setValue(string val, string steering)
        {
            foreach (Control currChild in mdiClient.Controls)
            {
                if (StringHelpers.DoStringsMatch(currChild.Text, val))
                {
                    currChild.BringToFront();
                    if (currChild is Form && (currChild as Form).WindowState == FormWindowState.Minimized)
                        (currChild as Form).WindowState = FormWindowState.Normal;
                    return ReturnOK;
                }
            }
            return "Konnte das Fenster >" + val + "< nicht finden!";
        }

        public override string getValue(string val, string param)
        {
            if (mdiClient.Controls.Count > 0)
                return mdiClient.Controls[0].Text;
            return String.Empty;
        }

        public override string getProperty(string name, string key)
        {
            switch (name.ToLower())
            {
                case "value":
                    return getValue("", "");
                default:
                    return base.getProperty(name, key);
            }
        }

        public override System.Windows.Forms.Control Ctrl
        {
            get { return mdiClient; }
        }

        public override string RobotClass
        {
            get { return RobotClassTabControl; }
        }
    }
}
