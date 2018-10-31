using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace NVYVE.MVC
{
    /// <summary>
    /// The state of a viewer
    /// 
    /// *Note
    /// These names have been recognized not purely intuiative
    /// consider changing to Interactable, Uninteractable, Selected
    /// </summary>
    public enum ViewerState
    {
        /// <summary>
        /// The button has been selected 
        /// </summary>
        Focus,

        Unfocus,
        /// <summary>
        /// The button can be interactable with
        /// </summary>
        Active,
        /// <summary>
        /// The button can not be interacted with
        /// </summary>
        Inactive,
    } // public enum ViewerState

    public class ViewerStateComponent : MonoBehaviour
    {
        public UIObject uiObject
        {
            get
            {
                return GetComponent<UIObject>();
            }
        }

        public ViewerState viewerState;

        public virtual void SetViewerState(ViewerState viewerState)
        {
            switch (viewerState)
            {
                case ViewerState.Focus:
                    SetFocus();
                    break;
                case ViewerState.Unfocus:
                    SetUnfocus();
                    break;
                case ViewerState.Active:
                    SetActive();
                    break;
                case ViewerState.Inactive:
                    SetInactive();
                    break;
            }
        } // public virtual void SetViewerState(ViewerState viewerState)

        public virtual void SetFocus()
        {
            viewerState = ViewerState.Focus;
            SetState("focus");
        } // public virtual void SetSelected()

        public virtual void SetUnfocus()
        {
            viewerState = ViewerState.Unfocus;
            SetState("unfocus");
        } // public virtual void SetSelected()
        public virtual void SetActive()
        {
            viewerState = ViewerState.Active;
            SetState("active");
        } // public virtual void SetUnselected()
        //TODO the design pattern should move to include 
        public virtual void SetInactive()
        {
            viewerState = ViewerState.Inactive;
            SetState("inactive");
        } // public virtual void SetHide()


        public void SetState(string state)
        {
            try
            {
                uiObject.SetState(state);
            }
            catch { }
            foreach (UIObject uio in uiObjects)
            {
                try
                {
                    uio.SetState(state);
                }
                catch
                {
                }
            }
        }
        public List<UIObject> uiObjects;
        public void SelectToggle()
        {
            switch (viewerState)
            {
                case ViewerState.Inactive:
                    break;
                case ViewerState.Focus:
                    SetActive();
                    break;
                case ViewerState.Unfocus:
                    SetUnfocus();
                    break;
                case ViewerState.Active:
                    SetFocus();
                    break;
            }
        } // public void SelectToggle()
        public void VisibilityToggle()
        {
            switch (viewerState)
            {
                case ViewerState.Inactive:
                    SetActive();
                    break;
                case ViewerState.Active:
                case ViewerState.Focus:
                    SetInactive();
                    break;
            }
        } // public void VisibilityToggle()
    } // public class ViewerStateComponent : MonoBehaviour
} // namespace NVYVE.MVC