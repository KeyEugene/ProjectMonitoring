using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

using SortDirection = System.Web.UI.WebControls.SortDirection;
using System.Drawing;



namespace Teleform.Reporting.Web
{
    public partial class SortingControl : CompositeControl
    {
        public string FieldName { get; set; }

        private LinkButton SortingButton;

        public string AttributeID { get; set; }

        private string sortingDirectionToSession;

        private string sortingDirectionFromSession;

        private string techPredicate;

        public string TechPredicate
        {
            get
            {
                if (sortingDirectionToSession != null)
                {
                    var expression = string.Concat(AttributeID, " ", sortingDirectionToSession);
                    if (expression != string.Empty)
                        techPredicate = expression;
                }
                return techPredicate;
            }
            set
            {
                techPredicate = value;
            }
        }

        public object SortingData
        {
            get
            {
                var tuple = new Tuple<string, string>
                (
                    TechPredicate,
                    sortingDirectionToSession
                );
                return tuple;
            }
            set
            {
                if (value is Tuple<string, string>)
                {
                    var tuple = value as Tuple<string, string>;
                    TechPredicate = tuple.Item1;
                    sortingDirectionFromSession = tuple.Item2;

                }
            }
        }


        protected override void CreateChildControls()
        {
            SortingButton = new LinkButton
            //SortingButton = new TextBox
            {
                //Text = "↨↨↨",
                Text = FieldName,
                ID = "SortingButton"
            };
            SortingButton.Style.Add("white-space", "normal");
            SortingButton.Click += (ApplySorting_Click);

            Controls.Add(SortingButton);

            sortingDirectionToSession = "DESC";
            if (sortingDirectionFromSession == "DESC")
            {
                SortingButton.Text = string.Format("{0} {1}", "&#9660", FieldName);
                sortingDirectionToSession = "ASC ";
                //SortingButton.Text = "↑↑↑";
            }
            else if (sortingDirectionFromSession == "ASC ")
            {
                SortingButton.Text = string.Format("{0} {1}", "&#9650", FieldName);                
                sortingDirectionToSession = "DESC";
                //SortingButton.Text = "↓↓↓";
            }
        }

    }
}
