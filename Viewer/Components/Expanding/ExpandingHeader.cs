using UnityEngine;
using System.Collections;

namespace NVYVE.MVC
{
    /// <summary>
    /// The header element for an expanding panel
    /// </summary>
    public class ExpandingHeader : Viewer
    {
        /// <summary>
        /// The necessary space required for the header
        /// </summary>
        public float dimension;

        /// <summary>
        /// The toggle button for the whether the expending area is open or not
        /// </summary>
        public ViewerButton areaToggle;
    } // public class ExpandingHeader : Viewer
} // namespace NVYVE.MVC