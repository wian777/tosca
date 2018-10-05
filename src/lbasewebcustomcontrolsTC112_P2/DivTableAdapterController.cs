using System;
using System.Collections.Generic;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.AutomationInstructions.TestActions.Associations;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Adapters.Controllers;
using Tricentis.Automation.Engines.Representations.Attributes;

namespace HtmlTable.Html.Adapter.Controller
{
    // The Controller resolves the relation between single elements that belong to each other logically. In this case the Controller
    // is derived from TableContextAdapterController, which describes how to get from the table element (that is the HTML element that 
    // contains the complete table) to the rows inside the tables.
    // The TableContextAdapterController takes an Adapter as generic parameter. It's the same as the SupportedAdapter.

    // The SupportedAdapter describes the Adapter type which this controller needs as input source. It represents the starting point.
    // Therefore it must implement an ITableAdapter!
    [SupportedAdapter(typeof(DivTableAdapter))]
    public class DivTableAdapterController : TableContextAdapterController<DivTableAdapter>
    {

        // A controller will get several input parameters: 
        //  * The actual Adapter object that it should use as a starting point, 
        //  * an ISearchQuery: this carries all the parameter of the module, which are required by the framework to find the 
        //      matching HTML elements. The ISearchQuery is very rarely used in customized Controllers.
        //  * and a validator. The validator can be used to express if the Controller is applicable for the input parameters,
        //      however it is not needed in this case because this Controller can deal with all DivTableAdapters.
        public DivTableAdapterController(DivTableAdapter contextAdapter, ISearchQuery query, Validator validator)
            : base(contextAdapter, query, validator)
        {
        }

        #region Overrides of ContextAdapterController<DivTableAdapter>
        // Several methods have to be overridden from the (Table)ContextAdapterController. The following three methods describe how to
        // get from the given adapter to its 
        //  + direct children in the HTML structure  
        //  + to all its descendants in the HTML structure
        //  + to its parent
        //  + to its logical children, the row nodes

        // The implementation of each of these methods is pretty straight forward in this case. The Technical associations can be used.
        // A TechnicalAssociation tells the framework to search for a property on the HTML Technical (its name is passed in as a string).
        // That property is invoked and the result is the collection of the according nodes passed back to the framework
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

        #region Overrides of TableContextAdapterController<DivTableAdapter>
        // These methods are the most important ones for this Controller - it tells the framework how to get to the rows (or columns if you
        // have a column based table). 
        // But since this table has an easy comprehensible structure, it is just necessary to ask the framework to get all direct children.
        // The Technical Association 'Children' can be used.
        protected override IEnumerable<IAssociation> ResolveAssociation(RowsBusinessAssociation businessAssociation)
        {
            yield return new TechnicalAssociation("Children");
        }

        protected override IEnumerable<IAssociation> ResolveAssociation(ColumnsBusinessAssociation businessAssociation)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}