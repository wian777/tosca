using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lbaseDotNetCustomControls.Exceptions
{
    public class NotEnabledException : Exception 
    {
        public NotEnabledException(String msg)
            : base(msg)
        { }
    }
}
