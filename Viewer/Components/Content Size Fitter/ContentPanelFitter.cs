using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace NVYVE.MVC
{
    public class ContentPanelFitter : ContentSizeFitter
    {
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private float startY = 0f;
        [SerializeField]
        protected override void OnRectTransformDimensionsChange()
        {
            List<GameObject> children = gameObject.GetAllChildrenAsList();
            children.RemoveAt(0);
            float height = startY;
            height += GetComponent<RectTransform>().sizeDelta.y;

            if (rectTransform != null)
            {
                rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, height);
            }
            //Debug.Log("Hierarchy Changed");
            base.OnRectTransformDimensionsChange();
        } // protected override void OnRectTransformDimensionsChange()
    } // public class ContentPanelFitter : ContentSizeFitter
} // namespace NVYVE.MVC