using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NVYVE.MVC
{
    /// <summary>
    /// The types of viewers
    /// </summary>
    public enum ViewerType
    {
        /// <summary>
        /// The splash, idle, start, etc. screen
        /// </summary>
        Title = 0,
        /// <summary>
        /// Holds elements that are persistent throughout all the menu
        /// </summary>
        Persistent = 1,
        /// <summary>
        /// A type of viewer that can be opened in conjecture with other viewers
        /// </summary>
        Overlay = 2,
        /// <summary>
        /// A stand alone area of interaction for the user
        /// </summary>
        State = 3,
        /// <summary>
        /// Makes the screen black, or faded, or whatever to block the view
        /// </summary>
        BlackGround = 4,
    } // public enum ViewerType

    /// <summary>
    /// The viewer component for the MVC pattern
    /// Allows the user to interact with the system and the system to reflect the internal state of the system
    /// </summary>
    public class Viewer : MonoBehaviour
    {
        #region Adjust

        /// <summary>
        /// Adjust the element under a given transform to a target count
        /// </summary>
        /// <typeparam name="T">The type of element you want adjusted to a particuler count</typeparam>
        /// <param name="prefab">The given element you want to be populated</param>
        /// <param name="parent">The root element for the generated elements</param>
        /// <param name="target">The number of elements desired</param>
        /// <returns>A list of the elements created</returns>
        static public List<T> AdjustList<T>(T prefab, Transform parent, int target) where T : MonoBehaviour
        {
            while (parent.childCount > target)
            {
                Transform c = parent.GetChild(0);
                c.SetParent(null);

                if (Application.isPlaying)
                    Destroy(c.gameObject);
                else
                    DestroyImmediate(c.gameObject);
            }

            while (parent.childCount < target)
            {
                Transform t = (Instantiate(prefab.gameObject) as GameObject).transform;
                t.SetParent(parent);
                t.gameObject.ResetLocalTransforms();
            }

            List<T> returnList = new List<T>();

            for (int i = 0; i < parent.childCount; i++)
            {
                returnList.Add(parent.GetChild(i).GetComponent<T>());
            }

            return returnList;
        } // static public List<T> AdjustList<T> (T prefab, Transform parent, int target) where T : MonoBehaviour

        /// <summary>
        /// Destroy's a list that inherits from MonoBehaviour
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        static public void DestroyList<T>(ref List<T> objects)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                Transform c = (objects[i] as MonoBehaviour) != null ? (objects[i] as MonoBehaviour).transform : null;
                if (c == null)
                    continue;
                c.SetParent(null);
                Destroy(c.gameObject);
            }
            objects.Clear();
        } // static public void DestroyList<T>(ref List<T> objects)

        /// <summary>
        /// Sizes an given area by the desired space required
        /// </summary>
        /// <param name="elements">The number of element that populate the area</param>
        /// <param name="layoutGroup">Layout group that needs sizing</param>
        /// <param name="dimension">the dimension desired by each element</param>
        static public void AdjustArea(int elements, LayoutGroup layoutGroup, float dimension = 0)
        {
            if (layoutGroup as HorizontalLayoutGroup != null)
            {
                layoutGroup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, elements * dimension);
            }

            if (layoutGroup as VerticalLayoutGroup != null)
            {
                layoutGroup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, elements * dimension);
            }

            if (layoutGroup as GridLayoutGroup != null)
            {
                GridLayoutGroup grid = layoutGroup as GridLayoutGroup;
                float x = Mathf.Ceil(Mathf.Sqrt(elements));
                float y = Mathf.Round(Mathf.Sqrt(elements));

                grid.cellSize = new Vector2(grid.GetComponent<RectTransform>().rect.width / x, grid.GetComponent<RectTransform>().rect.height / y);
            }
        } // static public void AdjustArea(int elements, LayoutGroup layoutGroup, int dimenion = 0)

        #endregion

        [Header("Viewer")]
        /// <summary>
        /// The type of viewer this is
        /// </summary>
        public ViewerType viewerType;
        /// <summary>
        /// Debug toggle used to test viewer
        /// </summary>
        public bool m_debug;
        /// <summary>
        /// The rect transform associated with the viewer
        /// </summary>
        public RectTransform rectTransform
        {
            get { return this.GetComponent<RectTransform>(); }
        } // public RectTransform rectTransform

        #region Viewers

        /// <summary>
        /// The viewer set in with the viewer is set in
        /// </summary>
        public List<Viewer> activeViewers;
        /// <summary>
        /// The viewer's assocated with this viewer but not set in when this viewer is set in
        /// </summary>
        public List<Viewer> inactiveViewers;
        /// <summary>
        /// References to viewers that aren't affected by anything in this viewer
        /// </summary>
        public List<Viewer> referencedViewers;
        /// <summary>
        /// A composite list of all the Active and Inactive Viewers;
        /// </summary>
        public virtual List<Viewer> allViewers
        {
            get
            {
                List<Viewer> returnScreens = new List<Viewer>();
                returnScreens.AddRange(activeViewers);
                returnScreens.AddRange(inactiveViewers);
                return returnScreens;
            }
        } // public List<Screen> allViewers

        #endregion

        #region Canvas

        /// <summary>
        /// The during time to wait before the canvas should turn off
        /// *Note
        /// This does not calculate UIOjects with multiple durations not 
        /// </summary>
        public float closeDuration
        {
            get
            {
                float time = 0;
#if UI_OBJECT
                foreach (UIObject uiObject in GetComponentsInChildren<UIObject>())
                {
                    foreach (UIObjectState state in uiObject.m_states)
                    {
                        time = Mathf.Max(state.duration / uiObject.m_keyframeDuration, time);
                    }
                }
#endif

                time += 0.5f;

                return time;
            }
        } // public float closeDuration

        /// <summary>
        /// The Canvas associated with the viewer
        /// </summary>
        public Canvas canvas
        {
            get { return this.GetComponent<Canvas>(); }
        } // public Canvas canvas

        /// <summary>
        /// The delays canvas control Coroutine.
        /// </summary>
        Coroutine delayCanvas;
        /// <summary>
        /// An iEnumerator that is a delayed control of the canvas' visibility state. This is used to turn of the visibility of the viewer when it's not active
        /// </summary>
        /// <param name="isCanvas">The target state of the canvas</param>
        /// <param name="delay">The duration of time to elapse before setting the canvas' visilbity state</param>
        /// <returns>Nothing of use returns from this function at this point</returns>
        IEnumerator DelayCanvas(bool isCanvas, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (canvas != null)
            {
                canvas.enabled = isCanvas;
            }
            yield return 0;
        } // IEnumerator DelayCanvas(bool isCanvas, float delay)

        /// <summary>
        /// The drive class to the delayCanvas coroutinue
        /// </summary>
        /// <param name="isCanvas">The target state of the canvas</param>
        /// <param name="delay">The duration of time to elapse before setting the canvas' visilbity state</param>
        public void SetCanvas(bool isCanvas, float delay = 0.0f)
        {
            if (delayCanvas != null)
            {
                StopCoroutine(delayCanvas);
            }
            if (canvas != null)
            {
                delayCanvas = StartCoroutine(DelayCanvas(isCanvas, delay));
            }
        } // public void SetCanvas(bool isCanvas, float delay = 0.0f)

        #endregion

        #region UIObject
#if UI_OBJECT
        /// <summary>
        /// The UIObject associated with the viewer
        /// </summary>
        public UIObject uiObject
        {
            get { return this.GetComponent<UIObject>(); }
        } // public UIObject uiObject

        /// <summary>
        /// ViewerState components associated with the viewer
        /// </summary>
        public List<ViewerStateComponent> viewerStateComponents;
        /// <summary>
        /// UIObjects associated with the viewer
        /// </summary>
        public List<UIObject> uiObjects;

        public void SetUIObjects(string state)
        {
            try //Sanity Check
            {
                uiObject.SetState(state);
            }
            catch { }
            //Loop through UIObjects in list to run in
            foreach (UIObject ui in uiObjects)
            {
                try //Sanity Check
                {
                    ui.SetState(state);
                }
                catch { }
            }//foreach (UIObject ui in uiObjects)
        }
#endif
        #endregion

        #region Raycasting
        /// <summary>
        /// Whether external systems can control the raycasting state of this system
        /// </summary>
        public bool manualControlRaycasting;

        /// <summary>
        /// The internal variable for isRayCasting;
        /// </summary>
        bool _isRayCasting;
        /// <summary>
        /// Controls the graphic raycaster's state on the canvas
        /// </summary>
        public bool isRayCasting
        {
            get { return _isRayCasting; }
            set
            {
                _isRayCasting = value;
                if (canvas != null)
                {
                    try
                    {
                        canvas.GetComponent<GraphicRaycaster>().enabled = _isRayCasting;
                    } catch { }
                }
            }
        } // public bool isRayCasting
        #endregion

        #region Event

        public delegate void CloseEvent();
        public event CloseEvent OnClose;

        public delegate void OpenEvent();
        public event OpenEvent OnOpen;

        #endregion Event

        #region Control
        /// <summary>
        /// The current state of the viewer
        /// </summary>
        public bool activated;
        /// <summary>
        /// A button mean to mimic windows close, minimize, and maximize system
        /// *Note this system should be expanded to full mimic the mimiced system
        /// </summary>
        public ViewerButton controlButton;

        /// <summary>
        /// Set the state of the viewer and all of the elements within this viewer
        /// </summary>
        /// <param name="activating">The new target state of the viewer</param>
        /// <param name="list">A list of elements that is passed throughout the system</param>
        public virtual void SetElements(bool activating, params UnityEngine.Object[] list)
        {
            if (canvas != null && this.activated == activating)
            {
                return;
            }
            // Previous Activated State
            bool oldActivated = activated;
            //Set activated bool
            activated = activating;
            //Set Graphic Raycaster
            if (!manualControlRaycasting)
            {
                isRayCasting = activating;
            }
            //Check for activating canvas to run in
            if (activating)
            {
#if UI_OBJECT
                SetUIObjects("in");
#endif
            }//if (activating)
            //For running out on Canvas
            else
            {
#if UI_OBJECT
                SetUIObjects("out");
#endif
            } // else
            foreach (ViewerStateComponent viewerStateComponent in viewerStateComponents)
            {
                try //Sanity Check
                {
                    if (activating) { viewerStateComponent.SetActive(); }
                    else { viewerStateComponent.SetInactive(); }
                }
                catch
                { Debug.LogWarning(this + " : " + viewerStateComponent + " could not have it's viewer state set", gameObject); }
            }//foreach (ViewerStateComponent viewerStateComponent in viewerStateComponents)

            List<Viewer> viewers = new List<Viewer>();
            viewers.AddRange(activeViewers);
            //Add to inactive list
            if (!activating)
            {
                viewers.AddRange(inactiveViewers);
                Deactivate(list);
                SetCanvas(false, closeDuration);
            }
            else
            {
                //Sanity Check
                if (canvas != null)
                {
                    canvas.enabled = true;
                }
                //add to active list
                Activate(list);
                SetCanvas(true, 0f);

                if (oldActivated != activated)
                {

                }
            }

            //Loop through all viewers
            foreach (Viewer viewer in viewers)
            {
                try //Sanity Check
                { viewer.SetElements(activating, list); }
                catch
                { Debug.Log(this + " : " + viewer + " failed to have it's elements set", gameObject); }
            }

            //try //Sanity Check
            //{ NVYVE.Input_Touch.UpdateCanvasList(); }
            //catch
            //{ Debug.LogWarning(this + " failed to update input touch canvas list", gameObject); }
        } // public virtual void SetElements(bool activating, params UnityEngine.Object[] list)

        /// <summary>
        /// Toggles the viewer
        /// </summary>
        public virtual void ToggleElements()
        {
            SetElements(!activated);
        } // public virtual void ToggleElements()

        /// <summary>
        /// Called when the viewer is activating
        /// </summary>
        /// <param name="list"></param>
        public virtual void Activate(params UnityEngine.Object[] list)
        {
            StartTimeOut();
        } // public virtual void Activate(params UnityEngine.Object[] list)
        /// <summary>
        /// Called when the viewer is deactivating
        /// </summary>
        /// <param name="list"></param>
        public virtual void Deactivate(params UnityEngine.Object[] list)
        {
            StopTimeOut();
        } // public virtual void Deactivate(params UnityEngine.Object[] list)

        /// <summary>
        /// The time it take to timeout this screen and for it to attempt it's time out function
        /// </summary>
        public float timeOut;

        /// <summary>
        /// A Coroutine to facility the timing out
        /// </summary>
        /// <returns></returns>
        IEnumerator DoTimeOut()
        {
            yield return new WaitForSeconds(timeOut);
            TimeOutEnded();
            yield return null;
        } // IEnumerator ITimeOut()

        /// <summary>
        /// This function starts the timeout Coroutine, but only if the time duration is greater than zero
        /// </summary>
        public void StartTimeOut()
        {
            if (activated && timeOut > 0f)
            {
                StartCoroutine("DoTimeOut");
            }
        } // public void StartTimeOut()

        /// <summary>
        /// This function ends the time out Coroutine
        /// </summary>
        public void StopTimeOut()
        {
            StopCoroutine("DoTimeOut");
        } // public void StopTimeOut()

        public void ResetTimeOut()
        {
            StopTimeOut();
            StartTimeOut();
        } // public void ResetTimeOut()

        /// <summary>
        /// The function called when a viewer times out
        /// </summary>
        public virtual void TimeOutEnded () { }

        #endregion

        #region Get/Set

        /// <summary>
        /// Fetches a viewer of a given type from the internal list of viewers
        /// </summary>
        /// <typeparam name="T">The tpye of viewer to be fetched</typeparam>
        /// <returns>If found returns the viewer, otherwise returns null</returns>
        public T GetViewer<T>() where T : Viewer
        {
            foreach (Viewer viewer in referencedViewers)
            {
                if (viewer as T != null)
                {
                    return viewer as T;
                }
            }
            foreach (Viewer viewer in allViewers)
            {
                if (viewer as T != null)
                {
                    return viewer as T;
                }
            }
            return null;
        } // public T GetViewer<T>() where T : class

        /// <summary>
        /// Fetches a viewer of a given type from the internal list of viewers
        /// </summary>
        /// <typeparam name="T">The tpye of viewer to be fetched</typeparam>
        /// <returns>If found returns the viewer, otherwise returns null</returns>
        public List<T> GetViewers<T>() where T : Viewer
        {
            List<T> viewers = new List<T>();
            foreach (Viewer viewer in referencedViewers)
            {
                if (viewer as T != null)
                {
                    viewers.Add(viewer as T);
                }
            }
            foreach (Viewer viewer in allViewers)
            {
                if (viewer as T != null)
                {
                    viewers.Add(viewer as T);
                }
            }
            return viewers;
        } // public T GetViewer<T>() where T : class

        /// <summary>
        /// Fetches a viewer state components of a given type from the internal list of viewer state components
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetViewerStateComponents<T>() where T : ViewerStateComponent
        {
            var components = (from x in viewerStateComponents
                              where x.GetType() is T
                              select x).ToList();
            return components as List<T>;
        } // public T GetViewerStateComponent<T>() where T : ViewerStateComponent

        public T GetViewerStateComponents<T>(string name) where T : ViewerStateComponent
        {
            foreach (ViewerStateComponent vsc in viewerStateComponents)
            {
                if (vsc as T != null && vsc.name == name)
                {
                    return vsc as T;
                }
            }
            return null;
        } // public T GetViewerStateComponents<T>(string name) where T : ViewerStateComponent

        #endregion

        #region Populate
        /// <summary>
        /// A function allowing a viewer to populate itself
        /// It's intended purpose is to allow viewers to do heavy population at start and not when it opens 
        /// </summary>
        public virtual void Populate()
        {
            foreach (Viewer viewer in allViewers)
            {
                if (viewer != null)
                    viewer.Populate();
            }
        } // public virtual void Populate()

        /// <summary>
        /// 
        /// </summary>
        public virtual void Reset()
        {

        }
        #endregion

        #region Shifted
        /// <summary>
        /// The current state of the shifted elements
        /// </summary>
        public bool shifted;
        /// <summary>
        /// A list of the elements to be controlled by a change in the shifted state
        /// </summary>
#if UI_OBJECT
        public List<UIObject> shiftables;
#endif
        /// <summary>
        /// The control functi8on for setting the shifted state of the shiftable elements
        /// </summary>
        /// <param name="shifted"></param>
        public virtual void SetShifted(bool shifted)
        {
            if (!activated)
            {
                return;
            }

            if (this.shifted != shifted)
            {
#if UI_OBJECT
                try
                { uiObject.SetState(shifted ? "shifted" : "unshifted"); }
                catch
                { Debug.LogWarning(this.gameObject.name + " has no uiObject to set shifted", gameObject); }

                foreach (UIObject shiftable in shiftables)
                {
                    try
                    { shiftable.SetState(shifted ? "shifted" : "unshifted"); }
                    catch
                    { Debug.LogWarning(this.gameObject.name + " : " + shiftable + " could not be shifted", gameObject); }
                }
#endif
            }

            //foreach (Viewer viewer in allViewers) - HACK TEST
            //{
            //    try
            //    { viewer.SetShifted(shifted); }
            //    catch
            //    { Debug.LogWarning(this.gameObject.name + " : " + viewer + " could not be shifted", gameObject); }
            //}
            this.shifted = shifted;
        } // public virtual void SetShifted(bool shifted)
        #endregion

        /// <summary>
        /// The update controlled by the main of the MVC pattern
        /// </summary>
        /// <param name="timeTick"></param>
        public virtual void Tick(float timeTick)
        {
            foreach (Viewer viewer in allViewers)
            {
                if (viewer != null)
                    viewer.Tick(timeTick);
            }
        } // public virtual void Tick(float timeTick)
    } // public class Viewer : MonoBehaviour
} // namespace NVYVE.MVC