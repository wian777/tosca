using System.Collections.Generic;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.AutomationInstructions.TestActions.Associations;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Adapters.Controllers;
using Tricentis.Automation.Engines.Representations.Attributes;
using Tricentis.Automation.Engines.Technicals;
using Tricentis.Automation.Engines.Technicals.Html;

namespace Customer.DevExpress.Html.Adapter.Controller
{
    [SupportedAdapter(typeof(DevExpressComboBoxAdapter))]
    public class DevExpressComboBoxAdapterController : ListAdapterController<DevExpressComboBoxAdapter>
    {
        public DevExpressComboBoxAdapterController(DevExpressComboBoxAdapter contextAdapter, ISearchQuery query,
                                                   Validator validator)
            : base(contextAdapter, query, validator)
        {
        }

        protected override IEnumerable<IAssociation> ResolveAssociation(ChildrenBusinessAssociation businessAssociation)
        {
            yield return new TechnicalAssociation("Children");
        }

        protected override IEnumerable<IAssociation> ResolveAssociation(
            DescendantsBusinessAssociation businessAssociation)
        {
            yield return new TechnicalAssociation("All");
        }

        protected override IEnumerable<IAssociation> ResolveAssociation(ParentBusinessAssociation businessAssociation)
        {
            yield return new TechnicalAssociation("ParentNode");
        }

        protected override IEnumerable<IAssociation> ResolveAssociation(ListItemsBusinessAssociation businessAssociation)
        {
            yield return new AlgorithmicAssociation("ListItems");
        }

        protected override IEnumerable<ITechnical> SearchTechnicals(
            IAlgorithmicAssociation algorithmicAssociation)
        {
            if (algorithmicAssociation.AlgorithmName == "ListItems")
            {
                return GetListItems();
            }
            return base.SearchTechnicals(algorithmicAssociation);
        }

        private IEnumerable<ITechnical> GetListItems()
        {
            List<ITechnical> items = new List<ITechnical>();

            string id = ContextAdapter.Technical.Id;
            string itemTableId = id.Substring(0, id.Length - 2) + "_DDD_L_LBT";
            IHtmlTableTechnical htmlTableTechnical =
                ContextAdapter.Technical.Document.Get<IHtmlDocumentTechnical>().GetById(itemTableId).Get
                    <IHtmlTableTechnical>();
            if (htmlTableTechnical == null) return items;
            foreach (IHtmlRowTechnical rowTechnical in htmlTableTechnical.Rows.Get<IHtmlRowTechnical>())
            {
                items.AddRange(rowTechnical.Cells.Get<ITechnical>());
            }

            return items;
        }
    }
}