using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Windows.Forms;

using lbaseDotNetCustomControls.Reflection;

using TricentisLibs;

namespace lbaseDotNetCustomControls
{
    public class C1FlexGrid : OperatorGenericCtrl
    {
        private readonly Control flexGrid;
        private readonly C1FlexGridWrapper wrapper;

        public String ParentName
        {
            get { return getProperty("ParentName", ""); }
        }

        public C1FlexGrid(Control ctrl)
        {
            Logger.Instance.Log(this, "Entered Constructor of flexGrid");
            flexGrid = ctrl;
            wrapper = new C1FlexGridWrapper(flexGrid);
        }

        public override PropertyInfo[] getProperties(string key)
        {
            List<PropertyInfo> currProps = new List<PropertyInfo>(base.getProperties(key));
            currProps.Add(this.GetType().GetProperty("ParentName"));
            return currProps.ToArray();
        }

        public override string getProperty(string name, string key)
        {
            switch (name.ToLower())
            {
                case "parentname":
                    return flexGrid.Parent.Name;
                default:
                    return base.getProperty(name, key);
            }
        }

        public override Control Ctrl
        {
            get { return flexGrid; }
        }

        public override string RobotClass
        {
            get { return RobotClassDataGrid; }
        }


        public override string setValue(string val, string steering)
        {
            String[] parameters = val.Split(";".ToCharArray());
            if (parameters.Length == 3)
            {
                String rowStr = parameters[0];
                String colStr = parameters[1];
                int visibleRowIndex, visibleColIndex;

                String value = parameters[2];
                bool hasValue = false;
                if (!String.IsNullOrEmpty(value))
                {
                    if (!value.Trim().StartsWith("{") && !value.Trim().EndsWith("}"))
                        hasValue = true;
                    else
                    {
                        value = value.ToLower().Substring(value.IndexOf('{') + 1);
                        value = value.Substring(0, value.IndexOf('}'));
                        if (value.EndsWith("click"))
                        {
                            steering = value;
                            value = String.Empty;
                        }
                    }
                }
                if (int.TryParse(rowStr, out visibleRowIndex) && int.TryParse(colStr, out visibleColIndex))
                {
                    // first we have to find the real indexes for rows and cols
                    if (flexGrid.Name.ToLower().Equals("splitgrid"))
                    {
                        visibleRowIndex--;
                    }
                    int realRowIndex = wrapper.GetRealRowIndex(visibleRowIndex);
                    int realColIndex = wrapper.GetRealColIndex(--visibleColIndex);

                    if (!hasValue)
                    {
                        // it is a mouseSteering
                        if (!String.IsNullOrEmpty(value))
                        {
                            // on drag or drop we always use the first col for a doubleClick
                            String expandCollapseValue = wrapper.GetHierarchieColValue(realRowIndex);
                            if (expandCollapseValue != null)
                            {
                                String errorMsg = "Unbekanntes Handling: {" + value + "}";
                                switch (expandCollapseValue.Trim().ToLower())
                                {
                                    case "+":
                                        if (value == "expand")
                                            realColIndex = 0;
                                        else if (value == "collapse")
                                            return ReturnOK;
                                        else return errorMsg;
                                        break;
                                    case "-":
                                        if (value == "collapse")
                                            realColIndex = 0;
                                        else if (value == "expand")
                                            return ReturnOK;
                                        else return errorMsg;
                                        break;
                                    case "":
                                        return ReturnOK;
                                }
                                steering = value;
                            }
                            else
                                return "Keine Spalte für Expand und Collapse gefunden";
                        }
                        Rectangle rect = wrapper.GetSteeringRectangle(realRowIndex, realColIndex, steering);
                        if (!rect.IsEmpty)
                            if (rect.Width > 0 && rect.Height > 0)
                            {
                                if (HandleSteering(rect, steering))
                                    if (steering.ToLower() == "doubleclick")
                                        return ReturnCheckOKRectangle(rect);
                                    else
                                    {
                                        return ReturnOK;
                                    }
                                else
                                    return "Unbekanntes Steering: >" + steering + "<";
                            }
                            else
                                return "Kein Rechteck zum Klicken vorhanden, Zelle ist unsichtbar";
                        return "Kein Rechteck zum Klicken vorhanden";
                    }
                    else
                    {
                        return wrapper.SetValue(realRowIndex, realColIndex, value);
                    }
                }
            }

            // Wenn keine Zeile oder keine Spalte im TestStep eingetragen wurde:
            // Wir ignorieren alles, was aus dem TestStep kommt
            // und droppen auf die Zeile 0 und die Spalte 1
            if (steering.ToLower() == "drop")
            {
                // Ermittle rect mit Zeile 0, Spalte 1: 
                Rectangle rect = wrapper.GetSteeringRectangle(0, 1, steering);
                if (HandleSteering(rect, steering))
                    return ReturnOK;
            }
            // da nichts mit SetValue in C1FlexGrid funktioniert hat: 
            return "Fehler im SetValue von C1FlexGrid";
        }


        public override string getContent(string addInfo, ref bool bHeader)
        {
            Logger.Instance.Log(this, "Entered getContent of flexGrid");
            String content = wrapper.GetContent();
            String header;
            if (flexGrid.Name.ToLower().Equals("splitgrid"))
            {
                Control mainGrid = flexGrid.Parent.Controls[0];
                C1FlexGridWrapper mainGridWrapper = new C1FlexGridWrapper(mainGrid);
                header = mainGridWrapper.GetHeader();
            }
            else
            {
                header = wrapper.GetHeader();
            }
            if (String.IsNullOrEmpty(header))
            {
                bHeader = false;
                header = "";
            }
            else
            {
                bHeader = true;
            }
            if (!String.IsNullOrEmpty(content))
            {
                return header + "\n" + content;
            }
            else
                return header;
        }

        private bool HandleSteering(Rectangle rect, String steering)
        {
            int x = rect.X + rect.Width / 2;
            int y = rect.Y + rect.Height / 2;
            x = Math.Min(flexGrid.Width - 1, x);
            y = Math.Min(flexGrid.Height - 1, y);

            Point p = new Point(x, y);
            p = flexGrid.PointToScreen(p);

            ArrayList modifier = new ArrayList();
            switch (steering.ToLower())
            {
                case "doubleclick":
                    TricentisLibs.MouseSteering.Click(p, MouseSteering.Mousebutton.Left, MouseSteering.Clicktype.Click);
                    return true;
                case "click":
                    TricentisLibs.MouseSteering.Click(p, MouseSteering.Mousebutton.Left, MouseSteering.Clicktype.Click);
                    return true;
                case "rightclick":
                    TricentisLibs.MouseSteering.Click(p, MouseSteering.Mousebutton.Right, MouseSteering.Clicktype.Click);
                    return true;
                case "ctrlclick":
                    modifier.Add(TricentisLibs.MouseSteering.Modifier.CONTROL);
                    TricentisLibs.MouseSteering.Click(p, MouseSteering.Mousebutton.Left, MouseSteering.Clicktype.Click, modifier);
                    return true;
                case "shiftclick":
                    modifier.Add(TricentisLibs.MouseSteering.Modifier.SHIFT);
                    TricentisLibs.MouseSteering.Click(p, MouseSteering.Mousebutton.Left, MouseSteering.Clicktype.Click, modifier);
                    return true;
                case "altclick":
                    modifier.Add(TricentisLibs.MouseSteering.Modifier.ALT);
                    TricentisLibs.MouseSteering.Click(p, MouseSteering.Mousebutton.Left, MouseSteering.Clicktype.Click, modifier);
                    return true;
                case "expand":
                case "drag":
                    TricentisLibs.MouseSteering.StartDrag(p);
                    return true;
                case "drop":
                    TricentisLibs.MouseSteering.Drop(p);
                    return true;
                case "collapse":
                    TricentisLibs.MouseSteering.Click(p, MouseSteering.Mousebutton.Left, MouseSteering.Clicktype.Doubleclick);
                    return true;
                default:
                    return false;
            }
        }
    }
}
