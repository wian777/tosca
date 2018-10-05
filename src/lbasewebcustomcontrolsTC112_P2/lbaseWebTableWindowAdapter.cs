using System;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Adapters;
using Tricentis.Automation.Engines.Adapters.Attributes;
using Tricentis.Automation.Engines.Adapters.Html.Generic;
using Tricentis.Automation.Engines.Technicals.Html;
using Tricentis.Automation.Engines.Technicals.References;

namespace HtmlTable.Html.Adapter
{
	// This class represents (or interacts with) the table itself. The class implements the ITableAdapter, allowing the framework to identify
    // this Adapter as the table. The interface creates the link between the Representation and the Adapter

    // Supported Technical describes the type of HTML element that this table is represented by in the actual HTML structure. In this case 
    // the table is represented by a div-tag. Since the DIV element is already known by the framework we can refer to it.
    [SupportedTechnical(typeof(IHtmlDivTechnical))]
    public class lbaseWebTableWindowAdapter : AbstractHtmlDomNodeAdapter<IHtmlDivTechnical>, ITableAdapter {
        // The class inherits common functionality from the AbstractHtmlDomNodeAdapter, so that we do not have to implement all the common 
        // methods and properties that work the same for all HtmlDomNodes. The AbstactHtmlDomNodeAdapter needs to be parametrized with
        // the technical it interacts with - IHtmlDivTechnical in this case (the same as the SupportedTechnical!).

        // A constructor is necessary since there is no default constructor. The technical that this adapter is responsible for will be
        // passed in by the framework. Additionally there is a validator. This validator should be used to determine if the adapter fits
        // this technical. In this case we need to validate that our div tag represents the table. The validator is used to 
        // tell if the technical is a proper table or not, e.g. by checking the classname for the correct value.
		public lbaseWebTableWindowAdapter(IHtmlDivTechnical technical,
							   Validator validator)
			: base(technical, validator)
		{

            try
            {
                if (technical.ClassName != null && technical.ClassName.Contains("DataGridView-Control"))
                {
                    validator.AssertTrue(() => technical.ClassName.Contains("DataGridView-Control"));

                    #region search data-cuid for table
                    IHtmlDivTechnical parentDiv = GetParentNode<IHtmlDivTechnical>(technical);
                    int deep = 0;
                    while (parentDiv != null && parentDiv.GetAttribute("data-cuid") == null && deep < 4)
                    {
                        parentDiv = GetParentNode<IHtmlDivTechnical>(parentDiv);
                        deep++;
                    }

                    if (parentDiv != null)
                    {
                        a_DataCuid = parentDiv.GetAttribute("data-cuid");
                    }
                    #endregion search data-cuid for table
                }
                else
                {
                    validator.AssertTrue(false);
                }

            }
            catch (NullReferenceException)
            {
                validator.Fail();
            }
        }

        //public LoadStrategy LoadStrategy { get; } = LoadStrategy.Default;

		public LoadStrategy LoadStrategy
        {
            get { return LoadStrategy.Default; }
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
