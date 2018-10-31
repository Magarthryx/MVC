using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NVYVE.MVC
{
    /// <summary>
    /// The interactable element of viewers
    /// </summary>
    public class ViewerButton : ViewerStateComponent
    {
        public delegate void OnClick();
        public static OnClick onClick = delegate { };

        /// <summary>
        /// The layout element associated with this button
        /// </summary>
        public LayoutElement layoutElement
        {
            get
            {
                return GetComponent<LayoutElement>();
            }
        }
        /// <summary>
        /// The rect transform associated with this button
        /// </summary>
        public RectTransform rectTransform
        {
            get
            {
                return GetComponent<RectTransform>();
            }
        }

        /// <summary>
        /// The UI.Button associated with this button
        /// </summary>
        //[SerializeField]
        public Button selectionButton;
        /// <summary>
        /// When needed a text element used to identify the content assocated with this element, which is usually a model
        /// </summary>
        public Text id;
        public Text value;
        /// <summary>
        /// The desired area that the button should occupy
        /// </summary>
        public int dimension;

        public float Height
        {
            get
            {
                return selectionButton.image.preferredHeight;
            }
        }

        //private void Awake()
        //{
        //    onClick.

        //    if (selectionButton != null)
        //    {
        //        selectionButton.onClick.AddListener(() => { onClick(); });
        //    }
        //}


        /// <summary>
        /// Size the button
        /// </summary>
        public virtual void Fit() { }

        /// <summary>
        /// Setting the viewer button to active and it's button to active
        /// </summary>
        public override void SetActive()
        {
            base.SetActive();
            try
            {
                selectionButton.interactable = true;
            }
            catch { }
        } // public override void SetActive()

        /// <summary>
        /// Setting the viewer button to inactive and it's button to inactive
        /// </summary>
        public override void SetInactive()
        {
            base.SetInactive();
            try
            {
                selectionButton.interactable = false;
            }
            catch { }
        } // public override void SetInactive()
    } // public class ViewerButton : ViewerStateComponent
} // namespace NVYVE.MVC