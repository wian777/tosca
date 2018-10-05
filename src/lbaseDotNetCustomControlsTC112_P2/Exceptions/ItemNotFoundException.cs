using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lbaseDotNetCustomControls.Exceptions
{
    public class ItemNotFoundException : Exception  
    {
        public ItemNotFoundException(String msg)
            : base(msg)
        { }
    }
}
