using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace NVYVE.MVC
{
    public class ViewerController : Controller<ViewerController>
    {
        #region Viewer

        public List<Viewer> viewers;

        //initial setup method
        public override void Setup()
        {
            base.Setup();

            //create list
            viewers = new List<Viewer>();
            //loop throuhg project to find viewer types
            foreach (Viewer viewer in FindObjectsOfType<Viewer>())
            {
                //turn the viewer on
                viewer.activated = true;
                //set it's shifted state
                viewer.shifted = true;
                //if it has a canvas add it to the list
                if (viewer.canvas != null)
                {
                    viewers.Add(viewer);
                }
            }
            //Loop through to disable all canvas'
            foreach (Viewer viewer in viewers)
            {
                viewer.canvas.enabled = false;

                viewer.transform.SetParent(transform);
            }
        } // public override void Setup()

        public override void Populate()
        {
            //loop to populate the viewer
            foreach (Viewer viewer in viewers)
            {
                viewer.Populate();
            }
        } // public override void Populate()

        public override void Tick(float timeTick)
        {
            base.Tick(timeTick);
            //Loop to run custom update on viewers
            foreach (Viewer viewer in viewers)
            {
                if (viewer.canvas.enabled)
                    viewer.Tick(timeTick);
            }
        } // public override void Tick(float timeTick)

        #endregion
        
        #region Property

        public Viewer CurrentViewer { get; set; }
        public Viewer PreviousViewer { get; set; }

        public Viewer titleViewer
        {
            get
            {
                return GetViewerByType(ViewerType.Title)[0];
            }
        } // public Viewer titleViewer

        public Viewer persistentViewer
        {
            get
            {
                return GetViewerByType(ViewerType.Persistent)[0];
            }
        } // public Viewer persistentViewer

        public List<Viewer> stateViewers
        {
            get
            {
                return GetViewerByType(ViewerType.State);
            }
        } // public List<Viewer> stateViewers

        public List<Viewer> overlayViewers
        {
            get
            {
                return GetViewerByType(ViewerType.Overlay);
            }
        } // public List<Viewer> overlayViewers
        #endregion

        #region Raycast
        public List<Viewer> raycastControlledViewers
        {
            get
            {
                //create a new list
                List<Viewer> returnList = new List<Viewer>();
                //loop 
                foreach (Viewer viewer in GetViewerByType(ViewerType.State, ViewerType.Overlay))
                {
                    if (!viewer.manualControlRaycasting)
                    {
                        returnList.Add(viewer);
                    }
                }
                return returnList;
            }
        } // public List<Viewer> raycastControlledViewers

        public void SetRayCasting(bool isRayCasting)
        {
            foreach (Viewer viewer in raycastControlledViewers)
            {
                viewer.isRayCasting = isRayCasting;
            }
        } // public void SetRayCasting (bool isRayCasting)

        #endregion

        #region Get

        public List<Viewer> GetViewerByType(params ViewerType[] viewerTypes)
        {
            //new list
            List<Viewer> returnList = new List<Viewer>();
            //loop through the viewers to find a viewer
            foreach (Viewer viewer in viewers)
            {
                bool found = false;
                if (viewerTypes.Length > 0)
                {
                    foreach (ViewerType viewerType in viewerTypes)
                    {
                        if (viewer.viewerType == viewerType)
                        {
                            found = true;
                        }
                    }
                }
                else
                {
                    found = true;
                }
                if (found)
                {
                    returnList.Add(viewer);
                }
            }
            return returnList;
        } // public List<Viewer> GetViewerByType (params ViewerType[] viewerTypes)

        public virtual T GetViewer<T>() where T : class
        {
            foreach (Viewer viewer in viewers)
            {
                if (viewer as T != null)
                {
                    return viewer as T;
                }
            }

            return null;
        } // public virtual T GetViewer<T>() where T : class

        #endregion

        #region Shifted
        bool shifted = false;

        public void SetShifted(bool shifted)
        {
            foreach (Viewer viewer in viewers)
            {
                //Check if the Canvas is enabled before we call shifted on the viewer
                if (viewer.canvas.enabled)
                {
                    //Debug.Log("Viewer: " + viewer.name);
                    viewer.SetShifted(shifted);
                }
            }
        } // public void SetShifted (bool shifted)

        public void ToggleShifted()
        {
            SetShifted(!shifted);
        } // public void ToggleShifted()
        #endregion

        #region Control

        public delegate void ViewerOpened(Viewer viewer);
        public event ViewerOpened viewerOpened = delegate { };
        
        public delegate void ViewerClosed(Viewer viewer);
        public event ViewerClosed viewerClosed = delegate { };

        public virtual Viewer OpenViewer<T>(params UnityEngine.Object[] list) where T : Viewer
        {
            Viewer viewer = GetViewer<T>();

            if (viewer != null)
            {
                return OpenViewer(viewer, list);
            }

            return null;
        } //public virtual void OpenViewer<T>(params UnityEngine.Object[] list) where T : Viewer

        public virtual Viewer OpenViewer(Viewer viewer, params UnityEngine.Object[] list)
        {
            PreviousViewer = CurrentViewer;
            CurrentViewer = viewer;
            switch (viewer.viewerType)
            {
                case ViewerType.State:
                    foreach (Viewer toBeClosed in GetViewerByType(ViewerType.Overlay, ViewerType.State))
                    {
                        CloseViewer(toBeClosed, list);
                    }
                    break;
                case ViewerType.Overlay:
                case ViewerType.BlackGround:
                    break;
                default:
                    {
                        CloseAllViewers(list);
                        break;
                    }
            }

            if (viewer != null && viewers.Contains(viewer))
            {
                viewer.SetElements(true, list);
                SetShifted(shifted);

                // Let everyone know a viewer is open
                try
                {
                    viewerOpened(viewer);
                }
                catch { }

                return viewer;
            }

            return null;
        } // public virtual void OpenViewer(Viewer viewer, params UnityEngine.Object[] list)

        public virtual void CloseViewer<T>(params UnityEngine.Object[] list) where T : Viewer
        {
            Viewer viewer = GetViewer<T>();

            if (viewer != null)
            {
                CloseViewer(viewer, list);
            }
        } // public virtual void CloseViewer<T>(params UnityEngine.Object[] list) where T : Viewer

        public virtual void CloseViewer(Viewer viewer, params UnityEngine.Object[] list)
        {
            if (viewer != null && viewers.Contains(viewer))
            {
                //SetShifted(false);
                viewer.SetElements(false, list);

                // Let everyone know a viewer is open
                try
                {
                    viewerClosed(viewer);
                }
                catch { }
            }
        } // public virtual void CloseViewer(Viewer viewer, params UnityEngine.Object[] list)

        public virtual void CloseAllViewers(params UnityEngine.Object[] list)
        {
            foreach (Viewer viewer in viewers)
            {
                CloseViewer(viewer, list);
            }
        } // public virtual void CloseAllViewers(params UnityEngine.Object[] list)

        public virtual void SetBlackGroundScreen(bool v)
        {
            if (v)
            {
                OpenViewer<BlackGroundViewer>();
            } else
            {
                CloseViewer<BlackGroundViewer>();
            }
        }
        #endregion
    } // public class ViewerController : Controller<ViewerController>
} // namespace NVYVE.MVC