using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NVYVE.MVC
{
    /// <summary>
    /// A special type of viewer the elements beneath it change their shape
    /// </summary>
    public class ExpandingViewer : Viewer
    {
        /// <summary>
        /// allows only a single panel to be expanded at a time
        /// </summary>
        public bool isolatePanels;
        /// <summary>
        /// The prefab panel to populate itself with
        /// </summary>
        public ExpandingPanel expandingPanelPrefab;
        /// <summary>
        /// The area to be size of the panels
        /// </summary>
        public RectTransform body;
        /// <summary>
        /// The area to be populated with panels
        /// </summary>
        public RectTransform panelArea;

        /// <summary>
        /// A list of the current expanding panels
        /// </summary>
        public List<ExpandingPanel> expandingPanels;

        /// <summary>
        /// The previous area that was needed for the expanding panels
        /// </summary>
        float previousDimension = 0f;
        /// <summary>
        /// An aggregated value for the area that is needed for the current panels 
        /// </summary>
        public float dimension
        {
            get
            {
                float d = 0;
                foreach (ExpandingPanel expandingPanel in expandingPanels)
                {
                    d += expandingPanel.dimenion;
                }
                return d;
            }
        } // public float dimension

        /// <summary>
        /// The direction the area expandings in
        /// </summary>
        public RectTransform.Axis axis;

        /// <summary>
        /// The passage of time defined by the MVC main
        /// </summary>
        /// <param name="timeTick">The elapsed time duration</param>
        public override void Tick(float timeTick)
        {
            base.Tick(timeTick);
            foreach (ExpandingPanel expandingPanel in expandingPanels)
            {
                expandingPanel.Tick(timeTick);
            }
            if (dimension != previousDimension)
            {
                body.SetSizeWithCurrentAnchors(axis, dimension);
            }
            previousDimension = dimension;
        } // public override void Tick(float timeTick)
        public override void SetShifted(bool shifted)
        {
            base.SetShifted(shifted);
            foreach (ExpandingPanel expandingPanel in expandingPanels)
            {
                expandingPanel.IsOpen = false;
            }
        } // public override void SetShifted(bool shifted)

        /// <summary>
        /// Toggles a given panel, and if in isolate mode then close all other panels
        /// </summary>
        /// <param name="expandingPanel"></param>
        protected void Isolate(int index)
        {
            if (!isolatePanels)
            {
                return;
            }
            for (int i = 0; i < expandingPanels.Count; i++)
            {
                if (i != index)
                {
                    Close(expandingPanels[i]);
                }
            }
        } // protected void Isolate(ExpandingPanel expandingPanel)

        /// <summary>
        /// The setup of the expanding panel
        /// </summary>
        /// <param name="list">A shared group of elements passed through the system</param>
        public override void Activate(params Object[] list)
        {
            base.Activate(list);

            for (int i = 0; i < expandingPanels.Count; i++)
            {
                int index = i;
                if (expandingPanels[index].header != null)
                {
                    expandingPanels[index].header.areaToggle.selectionButton.onClick.AddListener(() =>
                    {
                        Isolate(index);
                    });
                }
            }
        } // public override void Activate(params Object[] list)

        /// <summary>
        /// Deactivate expanding viewer
        /// </summary>
        /// <param name="list"></param>
        public override void Deactivate(params Object[] list)
        {
            base.Deactivate(list);
            SetIsOpen(false);
        } // public override void Deactivate(params Object[] list)

        /// <summary>
        /// When setting elements set all of the expanding panels, and close them
        /// </summary>
        /// <param name="activating">The desired activation state</param>
        /// <param name="list">A share group of elements passed through the system</param>
        public override void SetElements(bool activating, params Object[] list)
        {
            base.SetElements(activating, list);
            foreach (ExpandingPanel expandingPanel in expandingPanels)
            {
                expandingPanel.SetElements(activating, list);
            }
            if (activating)
            {
                for (int i = 0; i < expandingPanels.Count; i++)
                {
                    int index = i;
                    if (expandingPanels[index].header != null)
                    {
                        expandingPanels[index].header.areaToggle.selectionButton.onClick.AddListener(() =>
                        {
                            Isolate(index);
                        });
                    }
                }
            }
            //Close();
        } // public override void SetElements(bool activating, params Object[] list)

        /// <summary>
        /// Open all panels
        /// </summary>
        public virtual void Open()
        {
            SetIsOpen(true);
        } // public virtual void Open()

        /// <summary>
        /// Open a target panel
        /// </summary>
        /// <param name="expandingPanel"></param>
        public virtual void Open(ExpandingPanel expandingPanel)
        {
            SetIsOpen(true, expandingPanel);
        } // public virtual void Open (ExpandingPanel expandingPanel)

        /// <summary>
        /// Close all panels
        /// </summary>
        public virtual void Close()
        {
            SetIsOpen(false);
        } // public virtual void Close()

        /// <summary>
        /// Close a target panel
        /// </summary>
        /// <param name="expandingPanel"></param>
        public virtual void Close(ExpandingPanel expandingPanel)
        {
            SetIsOpen(false, expandingPanel);
        } // public virtual void Close(ExpandingPanel expandingPanel)

        /// <summary>
        /// Set a list of expanding panels opened or closed
        /// </summary>
        /// <param name="state">The target state of the elements</param>
        /// <param name="list">The list of expanding panels to set the state of</param>
        public virtual void SetIsOpen(bool state, params ExpandingPanel[] list)
        {
            if (list.Length == 0)
            {
                if (expandingPanels.Count > 0)
                {
                    SetIsOpen(state, expandingPanels.ToArray());
                }
            }
            else
            {
                foreach (ExpandingPanel expandingPanel in list)
                {
                    expandingPanel.IsOpen = state;
                }
            }
        } // public virtual void SetIsOpen(bool state, params ExpandingPanel[] list)
    } // public class ExpandingViewer : Viewer
} // namespace NVYVE.MVC