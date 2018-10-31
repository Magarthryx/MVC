using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NVYVE.MVC
{
    public class ToggleViewerButton : ViewerButton
    {
        [Header("-[ Toggle ]-")]
        public bool isToggleActive;
        
        private void Start()
        {
            // Set active state on Start
            UpdateState();
        }

        public void Toggle()
        {
            // Flip the state
            isToggleActive = !isToggleActive;
            // Update the state
            UpdateState();
        }

        private void UpdateState()
        {
            if (isToggleActive)
            {
                // We use 'Focus' for active
                SetFocus();
            }
            else
            {
                // We use 'Active' for inactive..... don't ask
                SetActive();
            }
        }
    }
}
