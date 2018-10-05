using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using lbaseDotNetCustomControls.Exceptions;
using lbaseDotNetCustomControls.Helpers;

namespace lbaseDotNetCustomControls.Reflection
{
    public class RibbonTabCollectionWrapper : RibbonCollectionWrapper<RibbonTabWrapper>
    {
        public RibbonTabCollectionWrapper(object coll):base (coll)
        {
        }
        
        protected override RibbonTabWrapper WrapChildElement(object obj)
        {
            return new RibbonTabWrapper(obj);
        }
    }
}
