using System.Text.RegularExpressions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Adapters.Attributes;
using Tricentis.Automation.Engines.Adapters.Html.Generic;
using Tricentis.Automation.Engines.Adapters.Lists;
using Tricentis.Automation.Engines.Technicals.Html;

namespace Customer.DevExpress.Html.Adapter
{
    // Adapting the Engine Layer -> Implementig a Combobox
    // Beispiel implementiert am 20170908
    // Benutzer: wian, willi.andric@lbase.software
    // Cascading Combo Boxes: https://demos.devexpress.com/aspxeditorsdemos/ASPxComboBox/ClientAPI.aspx
    // Beschreibung auf Tricentis.com:
    // https://support.tricentis.com/community/manuals_detail.do?version=10.2.0&url=topic1.html&tcapi=tboxapi
    // https://documentation.tricentis.com/devcorner/1020/tboxapi/topic39.html
    // Tosca Commander Testfall: UniqueId=12180993, NodePath=/lbaseWeb/w.Testfälle/TestCustomizedControls/ComboBox_identify
    [SupportedTechnical(typeof(IHtmlInputElementTechnical))]
    public class DevExpressComboBoxAdapter : AbstractHtmlDomNodeAdapter<IHtmlInputElementTechnical>, IComboBoxAdapter
    {
        private static readonly Regex idRegex = new Regex("ContentHolder_[^_]*?_I");

        public DevExpressComboBoxAdapter(IHtmlInputElementTechnical technical,
                                         Validator validator)
            : base(technical, validator)
        {
            validator.AssertTrue(() => CheckTechnical(technical));
        }

        private bool CheckTechnical(IHtmlInputElementTechnical technical)
        {
            string className = technical.ClassName;
            string id = technical.Id;
            return !string.IsNullOrEmpty(className) && className.StartsWith("dxeEditArea") &&
                   idRegex.IsMatch(id);
        }
    }
}