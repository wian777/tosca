using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lbaseDotNetCustomControls.Reflection
{
    public interface IWrappedObjectWithText : IWrappedObject
    {
        String Text { get; }
    }
}
