using System;
using System.Linq;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Adapters.Attributes;
using Tricentis.Automation.Engines.Adapters.Html;
using Tricentis.Automation.Engines.Technicals.Html;
using Tricentis.Automation.Engines.Technicals.References;

namespace lbaseWebCustomControls.Html.Adapter
{
    [SupportedTechnical(typeof(IHtmlInputElementTechnical))]
    internal class lbaseWebCheckBoxByLabelAdapter : HtmlCheckBoxAdapter
    {
        public lbaseWebCheckBoxByLabelAdapter(IHtmlInputElementTechnical htmlTechnical, Validator validator)
            : base(htmlTechnical, validator)
        {
            try
            {
                IHtmlDivTechnical parentDiv = GetParentNode<IHtmlDivTechnical>(htmlTechnical);

                parentDiv = GetParentNode<IHtmlDivTechnical>(parentDiv);

                IHtmlDivTechnical child = parentDiv.Children.Get<IHtmlDivTechnical>().FirstOrDefault();

                IHtmlLabelTechnical cuidparentLabel = child.Children.Get<IHtmlLabelTechnical>().FirstOrDefault();
                a_ByLabelId = cuidparentLabel.Id;
            }
            catch (NullReferenceException)
            {
                validator.Fail();
            }
        }

        private static T GetParentNode<T>(IHtmlElementTechnical element) where T : class, IHtmlElementTechnical
        {
            return element.GetTechnicalType().GetProperty("parentNode").Get<IObjectReference>(element).Get<T>();
        }

        public string a_ByLabelId
        {
            get;

            private set;
        }

        private IObjectReference GetParentNode(IHtmlElementTechnical element)
        {
            return element.GetTechnicalType().GetProperty("parentNode").Get<IObjectReference>(element);
        }
    }

}