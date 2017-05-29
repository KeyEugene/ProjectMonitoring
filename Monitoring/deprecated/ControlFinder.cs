using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Monitoring.Reporting
{
    /// <summary>
    /// Finds all controls of type T stores them in FoundControls
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Obsolete("", true)]
    public class ControlFinder<T> where T : Control
    {
        private List<T> _foundControls = new List<T>();
        public IEnumerable<T> FoundControls
        {
            get { return _foundControls; }
        }

        public void FindChildControlsRecursive(Control control)
        {
            foreach (Control childControl in control.Controls)
            {
                if (childControl.GetType() == typeof(T))
                {
                    _foundControls.Add((T)childControl);
                }
                else
                {
                    FindChildControlsRecursive(childControl);
                }
            }
        }
    }
}