using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NVYVE.MVC
{
    public class ExpandingPanel : Viewer
    {
        public bool clearToggle;

        public ExpandingHeader header;

        public RectTransform headerArea;
        public RectTransform bodyArea;
        public RectTransform contentArea;

        public LayoutElement layoutElement;

        public float maxLength;

        float previousDimension = 0f;
        
        /// <summary>
        /// target dimensions
        /// </summary>
        public virtual float dimenion
        {
            get
            {
                try
                {
                    previousDimension = headerDimension + (contentArea != null ? Mathf.Min(maxLength, bodyTransistionDimension) : bodyTransistionDimension);
                    return previousDimension;
                }
                catch { }
                try
                {
                    return headerDimension;
                }
                catch { }
                return 0f;
            }
        } // public virtual float dimenion

        /// <summary>
        /// area needed for the head
        /// </summary>        
        public virtual float headerDimension
        {
            get
            {
                if (header != null)
                {
                    return headerArea.rect.height;
                }
                else
                {
                    return 0;
                }
            }
        } // public float headerDimension

        /// <summary>
        /// area needed for the body
        /// </summary>        
        public virtual float bodyDimension
        {
            get
            {
                return bodyArea.rect.height;
            }
        } // public float bodyDimension

        /// <summary>
        /// The current area needed for the body modified by the transition area
        /// </summary>        
        public virtual float bodyTransistionDimension
        {
            get
            {
                return bodyDimension * bodyTransistion;
            }
        } // public float bodyDimension

        /// <summary>
        /// The area needed for the body scaled by the current percent of the visibility transistion 
        /// </summary>        
        public virtual float bodyTransistion
        {
            get
            {
                return currentTime / transistionTime;
            }
        } // public float bodyDimension

        /// <summary>
        /// The storage value for the IsOpen property
        /// </summary>        
        bool _isOpen;
        
        /// <summary>
        /// The property for whether the body of this panel is open or not
        /// </summary>
        public virtual bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                _isOpen = value;
                if (header != null)
                {
                    try
                    {
                        header.areaToggle.SetViewerState(_isOpen ? ViewerState.Focus : ViewerState.Active);
                    }
                    catch { }                        
                }
            }
        } // public virtual bool IsOpen

        /// <summary>
        /// The duration of time permitted for the transistion the body open and closed
        /// </summary>        
        public float transistionTime = 0.2f;
        
        /// <summary>
        /// The current time of the transistion area of the body
        /// </summary>
        float currentTime;
        
        /// <summary>
        /// The axis the body transistions open along
        /// </summary>
        public RectTransform.Axis axis;
        
        /// <summary>
        /// The main driven control of the passage of tick of this panel
        /// </summary>
        /// <param name="timeTick"></param>
        public override void Tick(float timeTick)
        {
            currentTime = Mathf.Clamp(currentTime + (timeTick * (IsOpen ? 1.0f : -1.0f)), 0.0f, transistionTime);
            if (previousDimension != dimenion)
            {
                layoutElement.preferredHeight = dimenion;
                rectTransform.SetSizeWithCurrentAnchors(axis, dimenion);
            }
        } // public virtual void CollapseControl(float timeTick)

        /// <summary>
        /// The activate function size the content area and sets up the area control button on the header
        /// </summary>
        /// <param name="list">The sharing of element through out the system</param>        
        public override void Activate(params Object[] list)
        {
            base.Activate(list);

            if (contentArea != null)
            {
                contentArea.SetSizeWithCurrentAnchors(axis, maxLength);
            }

            if (header != null)
            {
                try
                {
                    header.areaToggle.selectionButton.onClick.RemoveListener(Toggle);
                    header.areaToggle.selectionButton.onClick.AddListener(Toggle);
                }
                catch { }
            }
        } // public override void Activate(params Object[] list)
   
        void Toggle()
        {
            IsOpen = !IsOpen;
        }

        /// <summary>
        /// Clean up expanding panel
        /// </summary>
        /// <param name="list">The sharing of element through out the system</param>
        
        public override void Deactivate(params Object[] list)
        {
            base.Deactivate(list);

            if (header != null && header.areaToggle != null)
            {
                header.areaToggle.selectionButton.onClick.RemoveAllListeners();
            }
        } // public override void Deactivate(params Object[] list)
    } // public class ExpandingPanel : Viewer
} // namespace NVYVE.MVC