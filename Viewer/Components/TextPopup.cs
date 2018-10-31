using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NVYVE.MVC
{
    public class TextPopup : MonoBehaviour
    {
        public Text text;

        public UIObject uiObject;
        
        public float popupDuration;

        public void Popup(string text)
        {
            StartCoroutine(IPopup(text));
        } // public void Popup(string text)

        public IEnumerator IPopup(string text)
        {
            this.text.text = text;
            
            uiObject.SetState("in");

            yield return new WaitForSeconds(popupDuration);

            uiObject.SetState("out");

            Destroy(gameObject, 2.0f);

            yield return null;
        } // public IEnumerator IPopup(string text)
    } // public class TextPopup : MonoBehaviour
} // namespace NVYVE.MVC