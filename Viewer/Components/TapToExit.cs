using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NVYVE.MVC
{
    public class TapToExit : ViewerButton
    {
        public int tapsToExit;
        int currentTaps;

        public float resetTapTime;

        public void Start()
        {
            ResetTaps();

            selectionButton.onClick.AddListener(() =>
            {
                currentTaps++;
                StopCoroutine("IResetTaps");

                if (currentTaps >= tapsToExit)
                {
                    Application.Quit();
                    Debug.Log("tap to exit");
                    ResetTaps();
                } else
                {
                    StartCoroutine("IResetTaps");
                }
            });
        } // public void Start()

        public void ResetTaps()
        {
            currentTaps = 0;
        } // public void ResetTaps()

        public IEnumerator IResetTaps()
        {
            yield return new WaitForSeconds(resetTapTime);

            ResetTaps();

            yield return null;
        } // public IEnumerator IResetTaps()
    } // public class TapToExit : ViewerButton
} // namespace NVYVE.MVC