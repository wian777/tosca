using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lbaseDotNetCustomControls.Reflection
{
    class RibbonGalleryWrapper : RibbonItemWrapper
    {
        public RibbonGalleryWrapper(object item)
            : base(item)
        {
            // 
        }

        public override string Text
        {
            get { throw new NotImplementedException(); }
        }
    }
}
