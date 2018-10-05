using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Adapters.Attributes;
using Tricentis.Automation.Engines.Adapters.Html.Generic;
using Tricentis.Automation.Engines.Adapters.Lists;
using Tricentis.Automation.Engines.Technicals.Html;

namespace Customer.DevExpress.Html.Adapter
{
    [SupportedTechnical(typeof(IHtmlCellTechnical))]
    public class DevExpressComboBoxItemAdapter : AbstractHtmlDomNodeAdapter<IHtmlCellTechnical>, IListItemAdapter
    {
        private IHtmlInputElementTechnical htmlInputElementTechnical;
        private static readonly Regex regex = new Regex("ContentHolder_(?<idpart>.*)_DDD.*");

        public DevExpressComboBoxItemAdapter(IHtmlCellTechnical technical,
                                             Validator validator)
            : base(technical, validator)
        {
            validator.AssertTrue(() => CheckTechnical(technical));
        }

        private bool CheckTechnical(IHtmlCellTechnical technical)
        {
            string className = technical.ClassName;
            string id = technical.Id;
            if (string.IsNullOrEmpty(className) || !className.StartsWith("dxeListBoxItem") || string.IsNullOrEmpty(id) ||
                !id.StartsWith("ContentHolder"))
            {
                return false;
            }

            Match match = regex.Match(id);
            if (!match.Success)
            {
                return false;
            }
            string valueHolderId = "ContentHolder_" + match.Groups["idpart"].Value + "_I";

            htmlInputElementTechnical =
                Technical.Document.Get<IHtmlDocumentTechnical>().GetById(valueHolderId).Get<IHtmlInputElementTechnical>();
            return htmlInputElementTechnical != null;
        }

        public override bool IsSteerable
        {
            get { return true; }
        }

        public bool Selected
        {
            get { return htmlInputElementTechnical.Value == Text; }
            set
            {
                if (value)
                {
                    htmlInputElementTechnical.Value = Text;
                    htmlInputElementTechnical.FireEvent("change");
                }
            }
        }

        public string Text
        {
            get { return Technical.InnerText; }
        }
    }
}