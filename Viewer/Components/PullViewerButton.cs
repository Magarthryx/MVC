using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NVYVE.MVC
{
    public class PullViewerButton : ViewerButton
    {
        public float threshold;

        public RectTransform navigationTab;
        
        public delegate void Pull();
        public event Pull OnPulled = delegate { };

        bool isPulled = false;

        public void Update()
        {
            if (pulled && !isPulled)
            {
                if(OnPulled != null)
                    OnPulled.Invoke();
            }

            isPulled = pulled;
        } // public void Update()

        public bool pulled
        {
            get
            {
                return (Vector3.Magnitude(navigationTab.localPosition) > threshold);
            }
        } // public bool pulled
    } // public class PullViewerButton : ViewerButton
} // namespace NVYVE.MVC