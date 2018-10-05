using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Concurrent;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Adapters.Attributes;
using Tricentis.Automation.Engines.Adapters.Html;
using Tricentis.Automation.Engines.Technicals.Html;
using Tricentis.Automation.Engines.Technicals.References;
// using TricentisLibs;

namespace lbaseWebCustomControls.Html.Adapter
{
    // Implementiert am 20170831
    // Benutzer: wian, willi.andric@lbase.software
    // https://support.tricentis.com/community/manuals_detail.do?version=10.2.0&url=topic1.html&tcapi=tboxapi
    // https://documentation.tricentis.com/devcorner/1020/tboxapi/topic36.html
    [SupportedTechnical(typeof(IHtmlInputElementTechnical))]
    internal class lbaseWebTextBoxByDivAdapter : HtmlTextBoxAdapter
    {
        public lbaseWebTextBoxByDivAdapter(IHtmlInputElementTechnical htmlTechnical, Validator validator)
            : base(htmlTechnical, validator)
        {
			try
			{
				IHtmlDivTechnical parentDiv = GetParentNode<IHtmlDivTechnical>(htmlTechnical);

				int deep = 0;
				while (parentDiv != null && parentDiv.GetAttribute("data-cuid") == null && deep < 6)
				{
					parentDiv = GetParentNode<IHtmlDivTechnical>(parentDiv);
					deep++;
				}

				if (parentDiv != null)
				{
					a_DataCuid = parentDiv.GetAttribute("data-cuid");
				}
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

        public string a_DataCuid 
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