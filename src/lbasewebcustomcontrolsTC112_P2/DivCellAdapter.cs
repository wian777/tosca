using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Adapters.Attributes;
using Tricentis.Automation.Engines.Adapters.Generic;
using Tricentis.Automation.Engines.Adapters.Html.Generic;
using Tricentis.Automation.Engines.Technicals.Html;

namespace HtmlTable.Html.Adapter
{
    // This class represents (or interacts with) the cell. The class implements the ITableCellAdapter, allowing the framework to identify
    // this Adapter as a cell.

    // Supported Technical describes the type of HTML element that this table is represented by in the actual HTML structure. In this case 
    // the row is represented by a div-tag. 
    [SupportedTechnical(typeof(IHtmlDivTechnical))]
    public class DivCellAdapter : AbstractHtmlDomNodeAdapter<IHtmlDivTechnical>, ITableCellAdapter<IHtmlDivTechnical>
    {
        // The class inherits common functionality from the AbstractHtmlDomNodeAdapter, so that we do not have to implement all the common 
        // methods and properties that work the same for all HtmlDomNodes. The AbstactHtmlDomNodeAdapter needs to be parametrized with
        // the technical it interacts with - IHtmlDivTechnical in this case (the same as the SupportedTechnical!).

        #region Properties
        // Text is the property, that shows up in the scan result as the contents of the cell, for this we will use the InnerText
        public string Text
        {
            get { return Technical.InnerText; }
        }

        // RowSpan is the property that tells the framework how many rows this cells covers, which by default is 1 - the div table does not
        // use row spans.
        public int RowSpan
        {
            get { return 1; }
        }

        // ColSpan is the property that tells the framework how many column this cells covers, which by default is 1 - the div table does not
        // use row spans
        public int ColSpan
        {
            get { return 1; }
        }
        #endregion


        // A constructor is necessary since there is no default constructor. The technical that this adapter is responsible for will be
        // passed in by the framework. Additionally there is a validator. This validator should be used to determine if the adapter fits
        // this technical. In this case we need to validate that our div tag represents the cell. The validator is used to 
        // tell if the technical is a proper cell or not: cells that carry the div-table-col class, are cells of this tree.
        public DivCellAdapter(IHtmlDivTechnical technical, Validator validator)
            : base(technical, validator)
        {
            validator.AssertTrue(() => technical.ClassName == "div-table-col");
        }

    }
}