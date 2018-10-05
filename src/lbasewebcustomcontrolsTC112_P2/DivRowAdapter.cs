using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Adapters.Attributes;
using Tricentis.Automation.Engines.Adapters.Generic;
using Tricentis.Automation.Engines.Adapters.Html.Generic;
using Tricentis.Automation.Engines.Technicals.Html;

namespace HtmlTable.Html.Adapter
{
    // This class represents (or interacts with) the row. The class implements the ITableRowAdapter, allowing the framework to identify
    // this Adapter as the row.

    // Supported Technical describes the type of HTML element that this table is represented by in the actual HTML structure. In this case 
    // the row is represented by a div-tag. 
    [SupportedTechnical(typeof(IHtmlDivTechnical))]
    public class DivRowAdapter : AbstractHtmlDomNodeAdapter<IHtmlDivTechnical>, ITableRowAdapter<IHtmlDivTechnical>
    {
        // The class inherits common functionality from the AbstractHtmlDomNodeAdapter, so that we do not have to implement all the common 
        // methods and properties that work the same for all HtmlDomNodes. The AbstactHtmlDomNodeAdapter needs to be parametrized with
        // the technical it interacts with - IHtmlDivTechnical in this case (the same as the SupportedTechnical!).

        #region Constructors & Destructors

        // A constructor is necessary since there is no default constructor. The technical that this adapter is responsible for will be
        // passed in by the framework. Additionally there is a validator. This validator should be used to determine if the adapter fits
        // this technical. In this case we need to validate that our div tag represents the row. The validator is used to 
        // tell if the technical is the correct row element or not: div-table-row and div-table-caption are classes that rows are marked
        // with in this example. That means this class caters for 2 different types of rows.
        public DivRowAdapter(IHtmlDivTechnical technical, Validator validator)
            : base(technical, validator)
        {
            validator.AssertTrue(() => technical.ClassName == "div-table-row" || technical.ClassName == "div-table-caption");
        }

        #endregion

    }
}