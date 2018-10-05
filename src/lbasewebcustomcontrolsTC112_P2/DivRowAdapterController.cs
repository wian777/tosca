using System.Collections.Generic;

using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.AutomationInstructions.TestActions.Associations;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Adapters.Controllers;
using Tricentis.Automation.Engines.Representations.Attributes;

namespace HtmlTable.Html.Adapter.Controller
{
    // The Controller resolves the relation between single elements that belong to each other logically. In this case the Controller
    // is derived from TableRowContextAdapterController, which describes how to get from the row element to the cells in the row.
    // The TableRowContextAdapterController takes an Adapter as generic parameter. It's the same as the SupportedAdapter.

    // The SupportedAdapter describes the Adapter type which this controller needs as input source. It represents the starting point.
    // Therefore it also must implement an ITableRowAdapter!
    [SupportedAdapter(typeof(DivRowAdapter))]
    public class DivRowAdapterController : TableRowContextAdapterController<DivRowAdapter>
    {

        // A controller will get several input parameters: 
        //  * The actual Adapter object that it should use as a starting point, 
        //  * an ISearchQuery: this carries all the parameter of the module, which are required by the framework to find the 
        //      matching HTML elements. The ISearchQuery is very rarely used for customized Controllers.
        //  * and a validator. The validator can be used to express if the Controller is applicable for the input parameters,
        //      however it is not needed in this case because this Controller can deal with all DivRowAdapter.
        public DivRowAdapterController(DivRowAdapter contextAdapter, ISearchQuery query, Validator validator)
            : base(contextAdapter, query, validator)
        {
        }

        #region Overrides of ContextAdapterController<DivRowAdapter>
        // Several methods have to be overridden from the (TableRow)ContextAdapterController. The following three methods describe how to
        // get from the given adapter to its 
        //  + direct children in the HTML structure  
        //  + to all its descendants in the HTML structure
        //  + to its parent
        //  + to its logical children, the row nodes

        // The implementation of each of these methods is pretty straight forward in this case. The Technical associations can be used.
        // A TechnicalAssociation tells the framework to search for a property on the HTML Technical (its name is passed in as a string).
        // That property is invoked and the result is the collection of the according nodes.
        protected override IEnumerable<IAssociation> ResolveAssociation(ChildrenBusinessAssociation businessAssociation)
        {
            yield return new TechnicalAssociation("Children");
        }

        protected override IEnumerable<IAssociation> ResolveAssociation(DescendantsBusinessAssociation businessAssociation)
        {
            yield return new TechnicalAssociation("All");
        }

        protected override IEnumerable<IAssociation> ResolveAssociation(ParentBusinessAssociation businessAssociation)
        {
            yield return new TechnicalAssociation("ParentNode");
        }
        #endregion
        // This method is the most important one for this Controller - it tells the framework how to get to the cells. 
        // But since this row has an easy comprehensible structure, it is just necessary to ask the framework to get all childeren.
        // The children are then either div cells (in case of the header) and the DivCellAdapter will take care of them or
        // they are real cells and the framework uses the default Adapter
        protected override IEnumerable<IAssociation> ResolveAssociation(CellsBusinessAssociation businessAssociation)
        {
            yield return new TechnicalAssociation("Children");
        }
    }
}