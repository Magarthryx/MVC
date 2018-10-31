using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NVYVE.MVC
{
    public class NavigationViewer : Viewer
    {
        public override void Activate(params Object[] list)
        {
            base.Activate(list);

            ViewerController.instance.viewerOpened -= UpdateNavigationButtons;
            ViewerController.instance.viewerOpened += UpdateNavigationButtons;

            ClearButtons();
            SetupButtons();
        } // public override void Activate(params Object[] list)

        public override void Deactivate(params Object[] list)
        {
            base.Deactivate(list);

            ViewerController.instance.viewerOpened -= UpdateNavigationButtons;

            CloseHeader();
            ClearButtons();
        } // public override void Deactivate(params Object[] list)

        #region Navigation

        public List<NavigationButton> navigationButtons;

        public virtual void SetupButtons()
        {
            foreach (NavigationButton navigationButton in navigationButtons)
            {
                navigationButton.selectionButton.onClick.AddListener(() =>
                {
                    ViewerController.instance.OpenViewer(navigationButton.navigatedViewer);
                });
            }
        } // public virtual void SetupButtons()

        public virtual void ClearButtons()
        {
            foreach (NavigationButton navigationButton in navigationButtons)
            {
                navigationButton.selectionButton.onClick.RemoveAllListeners();
            }
        } // public virtual void ClearButtons()
        
        public void UpdateNavigationButtons (Viewer openedViewer)
        {
            if (openedViewer.viewerType == ViewerType.State)
            {
                foreach (NavigationButton navigationButton in navigationButtons)
                {
                    if(navigationButton.navigatedViewer == openedViewer)
                    {
                        navigationButton.SetFocus();
                    } else
                    {
                        navigationButton.SetActive();
                    }
                }
            }
        } // public void UpdateNavigationButtons (Viewer openedViewer)

        #endregion Navigation

        #region Header

        public HeaderPanel headerPanelPrefab;

        HeaderPanel currentHeaderPanel;

        public RectTransform headerPanelArea;

        public void OpenHeader(string header)
        {
            if (currentHeaderPanel != null)
            {
                CloseHeader();
            }

            currentHeaderPanel = Instantiate<HeaderPanel>(headerPanelPrefab, headerPanelArea);

            currentHeaderPanel.SetElements(true);

            currentHeaderPanel.Populate(header);
        } // public void OpenHeader(string header)

        public void CloseHeader()
        {
            if (currentHeaderPanel == null)
            {
                return;
            }

            currentHeaderPanel.SetElements(false);

            Destroy(currentHeaderPanel.gameObject, 2f);
        } // public void CloseHeader()
        
        #endregion Header
    } // public class NavigationViewer : Viewer
} // namespace NVYVE.MVC