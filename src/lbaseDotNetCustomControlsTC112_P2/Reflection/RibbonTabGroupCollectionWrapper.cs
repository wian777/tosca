using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lbaseDotNetCustomControls.Reflection
{
    public class RibbonTabGroupCollectionWrapper : RibbonCollectionWrapper<RibbonContextualTabGroupWrapper>
    {
        public RibbonTabGroupCollectionWrapper(object coll)
            : base(coll)
        {
        }

        protected override RibbonContextualTabGroupWrapper WrapChildElement(object obj)
        {
            return new RibbonContextualTabGroupWrapper(obj);
        }
    }
}
