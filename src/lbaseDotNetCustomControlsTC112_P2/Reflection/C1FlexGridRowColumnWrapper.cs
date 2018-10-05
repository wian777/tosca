using System;
using System.Reflection;

namespace lbaseDotNetCustomControls.Reflection
{
    class C1FlexGridRowColumnWrapper
    {
        private Object rowColumn;

        public C1FlexGridRowColumnWrapper(Object columnOrRow)
        {
            this.rowColumn = columnOrRow;
        }

        public String GetCaption()
        {
            return ReflectionHelper.ReflectionHelper.GetProperty(rowColumn, "Caption").ToString();
        }

        public int GetIndex()
        {
            return (int)ReflectionHelper.ReflectionHelper.GetProperty(rowColumn, "Index");
        }

        public bool GetVisible()
        {
            return (bool)ReflectionHelper.ReflectionHelper.GetProperty(rowColumn, "Visible");
        }
    }
}
