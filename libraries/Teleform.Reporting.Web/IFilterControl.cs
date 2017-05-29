using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Teleform.Reporting.Web
{
	public interface IFilterControl
	{
		event EventHandler FilterApplied;
        event EventHandler FilterCanceled;

        string ID { get; set; }
        
        string TechPredicate { get; }

        object FilterData { get; set; }

        void RejectFilter();

		string AttributeID { get; set; }

        bool Active { get; }

        Style ActiveStyle { get; set; }

        TemplateField Field { get; set; }
	}
}
