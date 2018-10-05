using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using lbaseDotNetCustomControls.Exceptions;
using lbaseDotNetCustomControls.Helpers;

namespace lbaseDotNetCustomControls.Reflection
{
    public class RibbonGroupCollectionWrapper : RibbonCollectionWrapper<RibbonGroupWrapper>
    {

        public RibbonGroupCollectionWrapper(object coll):base(coll)
        {
        }
        
        protected override RibbonGroupWrapper WrapChildElement(object obj)
        {
            return new RibbonGroupWrapper(obj);
        }
    }
}
