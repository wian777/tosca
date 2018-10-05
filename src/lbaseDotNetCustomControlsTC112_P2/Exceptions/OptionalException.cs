using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lbaseDotNetCustomControls.Exceptions
{
    public class OptionalException : Exception
    {
        public OptionalException(String msg)
            : base(msg)
        { }
    }
}
