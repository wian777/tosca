using System;
using System.Text;
using System.Reflection;
using System.Windows.Forms;


using System.Threading;
using TricentisLibs;
using System.Drawing;

namespace lbaseDotNetCustomControls.Reflection
{
    public class C1FlexGridWrapper
    {
        private readonly Control flexGrid;
        private readonly System.Collections.IEnumerable columns;
        private readonly System.Collections.IEnumerable rows;
        private readonly int columnCount;
        private readonly int rowCount;

        public C1FlexGridWrapper(Control flexGrid)
        {
            this.flexGrid = flexGrid;
            Object cols = ReflectionHelper.ReflectionHelper.GetProperty(flexGrid, "Cols");
            columnCount = (int)ReflectionHelper.ReflectionHelper.GetProperty(cols, "Count");
            if (columnCount > 0)
                columns = cols as System.Collections.IEnumerable;
            else
                columns = null;
            Object rws = ReflectionHelper.ReflectionHelper.GetProperty(flexGrid, "Rows");
            rowCount = (int)ReflectionHelper.ReflectionHelper.GetProperty(rws, "Count");
            if (rowCount > 0)
                rows = rws as System.Collections.IEnumerable;
            else
                rows = null;
        }

        public String GetContent()
        {
            StringBuilder content = new StringBuilder();
            int lastVisibleRow = -1, lastVisibleColumn = -1;
            foreach (Object row in rows)
            {
                C1FlexGridRowColumnWrapper rowWrapper = new C1FlexGridRowColumnWrapper(row);
                if (rowWrapper.GetVisible())
                    lastVisibleRow = rowWrapper.GetIndex();
                
                if ((flexGrid.Name.ToLower().Equals("splitgrid")) && (lastVisibleRow == 0))
                    lastVisibleRow = 1;
                
            }
            foreach (Object col in columns)
            {
                C1FlexGridRowColumnWrapper colWrapper = new C1FlexGridRowColumnWrapper(col);
                if (colWrapper.GetVisible())
                    lastVisibleColumn = colWrapper.GetIndex();
            }
            if (lastVisibleRow >= 1 && lastVisibleColumn >= 0)  // We can build the Content
            {
                int currRow = 1;
                if (flexGrid.Name.ToLower().Equals("splitgrid")) 
                {
                    currRow = 0;
                }
                for (; currRow < rowCount; currRow++)
                {
                    C1FlexGridRowColumnWrapper rowWrapper = new C1FlexGridRowColumnWrapper(GetRow(currRow));
                    if (rowWrapper.GetVisible())
                    {
                        for (int currCol = 0; currCol < columnCount; currCol++)
                        {
                            C1FlexGridRowColumnWrapper colWrapper = new C1FlexGridRowColumnWrapper(GetColumn(currCol));
                            if (colWrapper.GetVisible())
                            {
                                content.Append(GetCellContent(currRow, currCol));
                                if (currCol < lastVisibleColumn)
                                    content.Append(";");
                            }
                        }
                        if (currRow < lastVisibleRow)
                            content.Append("\n");
                    }
                }
            }
            return content.ToString();
        }

        internal String SetValue(int row, int col, string value)
        {
            // -1 on rows and colors is not needed, as we don't use the offset when writing the content
            FinishEditing(true);
            FocusAndSelectCell(row, col);
            String retVal = SetValueWithEditor(row, col, value);
            FinishEditing(false);
            return retVal;
        }

        public String GetHeader()
        {
            StringBuilder header = new StringBuilder();
            int lastVisibleColumn = -1;
            foreach (Object col in columns)
            {
                C1FlexGridRowColumnWrapper colWrapper = new C1FlexGridRowColumnWrapper(col);
                if (colWrapper.GetVisible())
                    lastVisibleColumn = colWrapper.GetIndex();
            }
            foreach (Object column in columns)
            {
                C1FlexGridRowColumnWrapper colWrapper = new C1FlexGridRowColumnWrapper(column);
                if (colWrapper.GetVisible())
                {
                    header.Append(colWrapper.GetCaption().Replace("\r", "").Replace("\n", "").Replace("\t", ""));
                    if (colWrapper.GetIndex() < lastVisibleColumn)
                        header.Append(";");
                }
            }
            return header.ToString();
        }

        public String GetHierarchieColValue(int row)
        {
            foreach (Object o in columns)
                if (ReflectionHelper.ReflectionHelper.GetProperty(o, "Name").ToString() == "colHierarchie")
                {
                    C1FlexGridRowColumnWrapper columnWrapper = new C1FlexGridRowColumnWrapper(o);
                    return GetCellContent(row, columnWrapper.GetIndex());
                }
            return null;
        }

        private String GetCellContent(int row, int col)
        {
            return ReflectionHelper.ReflectionHelper.InvokeMethod(flexGrid, "GetDataDisplay",
                new Object[] { row, col }).ToString().Replace("\r", "").Replace("\n", "").Replace("\t", "");
        }

        public bool HasTreeColumn()
        {
            Object tree = ReflectionHelper.ReflectionHelper.GetProperty(flexGrid, "Tree");
            int treeColumn = (int)ReflectionHelper.ReflectionHelper.GetProperty(tree, "Column");
            if (treeColumn == -1)
                return false;
            else
                return true;
        }

        public Rectangle GetSteeringRectangle(int row, int col, String steering)
        {
            if (!steering.ToLower().Equals("shiftclick") && !steering.ToLower().Equals("ctrlclick") && !steering.ToLower().Equals("drop"))
            {
                FocusAndSelectCell(row, col);
            }
            return (Rectangle)ReflectionHelper.ReflectionHelper.InvokeMethod(flexGrid, "GetCellRect", new Object[] { row, col, true });
        }

        private void FocusAndSelectCell(int row, int col)
        {
            Thread.Sleep(300);
            if (flexGrid.CanFocus)
                ReflectionHelper.ReflectionHelper.InvokeMethod(flexGrid, "Focus");

            Thread.Sleep(1000);
            
            if (flexGrid.CanSelect)
                ReflectionHelper.ReflectionHelper.InvokeMethod(flexGrid, "Select", new Object[] { row, col, true });

            Thread.Sleep(300);            
        }

        private void StartEditing(int row, int col)
        {
            ReflectionHelper.ReflectionHelper.InvokeMethod(flexGrid, "StartEditing", new Object[] { row, col });
        }

        private void FinishEditing(bool cancel)
        {
            ReflectionHelper.ReflectionHelper.InvokeMethod(flexGrid, "FinishEditing", new object[] { cancel });
        }

        private String SetValueWithEditor(int row, int col, String value)
        {

            MethodInfo setDataMethod = flexGrid.GetType().GetMethod("SetData",
                                                         new Type[] { typeof(int), typeof(int), typeof(object) });
            Object cellStyle = ReflectionHelper.ReflectionHelper.InvokeMethod(flexGrid, "GetCellStyle", new object[] { row, col });
            Type dataType = null;
            if (cellStyle == null)
            {
                Object column = GetColumn(col);
                dataType = (Type)ReflectionHelper.ReflectionHelper.GetProperty(column, "DataType");
            }
            else
            {
                dataType = (Type)ReflectionHelper.ReflectionHelper.GetProperty(cellStyle, "DataType"); ;
            }
            if (dataType == typeof(bool))
            {
                if (value.ToLower() == "x" || value.ToLower() == "true" || value.ToLower() == "wahr"
                        || value.ToLower().StartsWith("check"))
                {
                    ReflectionHelper.ReflectionHelper.InvokeMethod(flexGrid, "SetData", new object[] { row, col, true });
                    return OperatorGenericCtrl.ReturnOK;
                }
                if (value.ToLower() == " " || value.ToLower() == "false" || value.ToLower() == "falsch"
                    || value.ToLower().StartsWith("uncheck"))
                {
                    ReflectionHelper.ReflectionHelper.InvokeMethod(flexGrid, "SetData", new object[] { row, col, false });
                    return OperatorGenericCtrl.ReturnOK;
                }
                return "Der Wert >" + value + "< kann nicht auf eine CheckBox-Zelle gesetzt werden";
            }
            else
            {
                Control editor = ReflectionHelper.ReflectionHelper.GetProperty(flexGrid, "Editor") as Control;
                if (editor != null)
                {
                    paramAction paramAction = new paramAction();
                    paramAction.PtHwnd = editor.Handle;
                    TricentisWindowFormCtrls.Operator op = new TricentisWindowFormCtrls.Operator(paramAction);
                    OperatorGenericCtrl genericCtrl = op.getOperatorControl(paramAction);
                    if (genericCtrl != null)
                    {
                        genericCtrl.setValue(value, "");
                        return OperatorGenericCtrl.ReturnOK;
                    }
                }
            }
            // default solution if no editor was found
            ReflectionHelper.ReflectionHelper.InvokeMethod(flexGrid, "SetData", new object[] { row, col, value });
            return OperatorGenericCtrl.ReturnOK;
            //return "Fehler beim Editieren der Zelle des FlexGrid. Kann Zellen vom Typ >" + dataType.Name + "< nicht editieren";
        }

        private Object GetRow(int index)
        {
            int cnt = 0;
            foreach (Object o in rows)
            {
                if (cnt++ == index)
                    return o;
            }
            return null;
        }

        private Object GetColumn(int index)
        {
            int cnt = 0;
            foreach (Object o in columns)
            {
                if (cnt++ == index)
                    return o;
            }
            return null;
        }

        public int GetRealRowIndex(int visibleIndex)
        {
            int currVisibleIndex = 0;
            foreach (var row in rows)
            {
                C1FlexGridRowColumnWrapper rowWrapper = new C1FlexGridRowColumnWrapper(row);
                if (rowWrapper.GetIndex() >= 0 && rowWrapper.GetVisible())
                    if (currVisibleIndex++ == visibleIndex)
                        return rowWrapper.GetIndex();
            }
            return -1;
        }

        public int GetRealColIndex(int visibleIndex)
        {
            int currVisibleIndex = 0;
            foreach (var col in columns)
            {
                C1FlexGridRowColumnWrapper columnWrapper = new C1FlexGridRowColumnWrapper(col);
                if (columnWrapper.GetVisible())
                    if (currVisibleIndex++ == visibleIndex)
                        return columnWrapper.GetIndex();
            }
            return -1;
        }
    }
}
