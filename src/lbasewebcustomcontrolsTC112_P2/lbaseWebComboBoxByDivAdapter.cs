using System;
using System.Linq;
///*
using System.IO;
using System.Collections;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Adapters.Attributes;
using Tricentis.Automation.Engines.Adapters.Html;
using Tricentis.Automation.Engines.Technicals.Html;
using Tricentis.Automation.Engines.Technicals.References;
// using lbaseWebCustomControls;
/*
using TricentisLibs;
*/
namespace lbaseWebCustomControls.Html.Adapter
{
    [SupportedTechnical(typeof(IHtmlInputElementTechnical))]
    internal class lbaseWebComboBoxByDivAdapter : HtmlComboBoxAdapter
    {
        public lbaseWebComboBoxByDivAdapter(IHtmlSelectTechnical htmlTechnical, Validator validator)
            : base(htmlTechnical, validator)
        {
            /*
            if (!Directory.Exists(System.Environment.SystemDirectory + @"\..\Temp"))
            {
                Directory.CreateDirectory(System.Environment.SystemDirectory + @"\..\Temp");
            }
            String Logfile = System.Environment.SystemDirectory + @"\..\Temp\lbaseWebComboBoxByDivAdapter.log";
            // _" + DateTime.Now.ToString("ddMMyyy_HHmm") + ".log";
            TextDatei c_textdatei = new TextDatei();
            c_textdatei.WriteFile(Logfile, "BEGIN lbaseWebComboBoxByDivAdapter DATE and TIME: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") +
                                       "\r\n---------------------------------------------------" + "\r\n");
            */
            try
            {
                IHtmlDivTechnical parentDiv = GetParentNode<IHtmlDivTechnical>(htmlTechnical);
                parentDiv = GetParentNode<IHtmlDivTechnical>(GetParentNode<IHtmlDivTechnical>(GetParentNode<IHtmlDivTechnical>(GetParentNode<IHtmlDivTechnical>((GetParentNode<IHtmlDivTechnical>(parentDiv))))));
                /*
                parentDiv = GetParentNode<IHtmlDivTechnical>(parentDiv);
                parentDiv = GetParentNode<IHtmlDivTechnical>(parentDiv);
                c_textdatei.Append(Logfile, "parentDiv.ClassName: " + parentDiv.ClassName);
                parentDiv = GetParentNode<IHtmlDivTechnical>(parentDiv);
                c_textdatei.Append(Logfile, "parentDiv.ClassName: " + parentDiv.ClassName);
                parentDiv = GetParentNode<IHtmlDivTechnical>(parentDiv);
                c_textdatei.Append(Logfile, "parentDiv.ClassName: " + parentDiv.ClassName);
                */

                // parentDiv.GetAttribute("data-cuid")
                /*
                while (parentDiv.GetAttribute("data-cuid") == "")
                {
                    parentDiv = GetParentNode<IHtmlDivTechnical>(parentDiv);
                }
                */

                a_DataCuid = parentDiv.GetAttribute("data-cuid");
                // c_textdatei.Append(Logfile, "a_DataCuid data-cuid: " + a_DataCuid);
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