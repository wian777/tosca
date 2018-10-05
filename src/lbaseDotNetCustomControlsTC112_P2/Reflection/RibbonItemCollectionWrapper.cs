using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using lbaseDotNetCustomControls.Exceptions;
using lbaseDotNetCustomControls.Helpers;

namespace lbaseDotNetCustomControls.Reflection
{
    public class RibbonItemCollectionWrapper : RibbonCollectionWrapper<RibbonItemWrapper>
    {
        private object ribbonItemCollection;

        public RibbonItemCollectionWrapper(object coll) : base(coll)
        {
            ribbonItemCollection = coll;
        }
        
        protected override RibbonItemWrapper WrapChildElement(object obj)
        {
            switch (obj.GetType().Name)
            {
                case "RibbonButton":
                    return new RibbonButtonWrapper(obj);
                case "RibbonCheckBox":
                    return new RibbonCheckBoxWrapper(obj);
                case "RibbonComboBox":
                    return new RibbonComboBoxWrapper(obj);
                case "RibbonGallery":
                    return new RibbonGalleryWrapper(obj);
                case "RibbonSplitButton":
                    return new RibbonSplitButtonWrapper(obj);
                case "RibbonMenu":
                    return new RibbonMenuWrapper(obj);
                case "RibbonToggleButton":
                    return new RibbonToggleButtonWrapper(obj);
                case "RibbonToggleGroup":
                    return new RibbonToggleGroupWrapper(obj);
                case "RibbonSeparator":
                    return new RibbonSeparatorWrapper(obj);
                case "RibbonLabel":
                    return new RibbonLabelWrapper(obj);
                case "RibbonControlHost":
                    return new RibbonControlHostWrapper(obj);
                default:
                    throw new NotSupportedException("Der Typ >" + obj.GetType().Name + "< wird als RibbonItem noch nicht unterstützt!");
            }
        }
    }
}
