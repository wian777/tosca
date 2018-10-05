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
// using lbaseWebCustomControls;

namespace lbaseWebCustomControls.Html.Adapter
{
    [SupportedTechnical(typeof(IHtmlInputElementTechnical))]
    internal class lbaseWebCheckBoxByDivAdapter : HtmlCheckBoxAdapter
    {
        public lbaseWebCheckBoxByDivAdapter(IHtmlInputElementTechnical htmlTechnical, Validator validator)
            : base(htmlTechnical, validator)
        {
            /*
            if (!Directory.Exists(System.Environment.SystemDirectory + @"\..\Temp"))
            {
                Directory.CreateDirectory(System.Environment.SystemDirectory + @"\..\Temp");
            }
            String Logfile = System.Environment.SystemDirectory + @"\..\Temp\lbaseWebCheckBoxByDivAdapter.log";
            // _" + DateTime.Now.ToString("ddMMyyy_HHmm") + ".log";
            TextDatei c_textdatei = new TextDatei();
            c_textdatei.WriteFile(Logfile, "BEGIN lbaseWebCheckBoxByDivAdapter DATE and TIME: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") +
                                       "\r\n---------------------------------------------------" + "\r\n");
            */
            try
            {
                // Bei class="ButtonBase-StateButtonContent"
                validator.AssertTrue(() => htmlTechnical.ClassName == "ButtonBase-StateButtonContent");

                IHtmlDivTechnical parentDiv = GetParentNode<IHtmlDivTechnical>(htmlTechnical);
                /*
                while (parentDiv.GetAttribute("data-cuid") == "")
                {
                    parentDiv = GetParentNode<IHtmlDivTechnical>(parentDiv);
                }
                */
                // c_textdatei.Append(Logfile, "lbaseWebCheckBoxByDivAdapter suche data-cuid: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\r\n");
                int deep = 0;
                while (parentDiv != null && parentDiv.GetAttribute("data-cuid") == null && deep < 2)
                {
                    parentDiv = GetParentNode<IHtmlDivTechnical>(parentDiv);
                    deep++;
                }

                if (parentDiv != null)
                {
                    a_DataCuid = parentDiv.GetAttribute("data-cuid");
                    // c_textdatei.Append(Logfile, "lbaseWebCheckBoxByDivAdapter data-cuid ist: " + a_DataCuid + " - " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\r\n");
                }
                /*
                c_textdatei.Append(Logfile, "parentDiv.ClassName: " + parentDiv.ClassName);
                a_DataCuid = parentDiv.GetAttribute("data-cuid");
                c_textdatei.Append(Logfile, "a_DataCuid data-cuid: " + a_DataCuid);
                */
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
        // */
    }

}