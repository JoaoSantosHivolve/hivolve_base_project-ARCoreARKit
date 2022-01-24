using UnityEngine;

namespace Common
{
    public enum SideToAdjust
    {
        Width,
        Height
    }
    [ExecuteInEditMode]
    public class RuleOfThreeImage : MonoBehaviour
    {
        public float originalWidth;
        public float originalHeight;

        public SideToAdjust sideToAdjust;

        private void Awake()
        {
            Canvas.ForceUpdateCanvases();

            var t = GetComponent<RectTransform>();
            float newLength = 0;
            switch (sideToAdjust)
            {
                case SideToAdjust.Width:
                    newLength = (t.rect.height * originalWidth) / originalHeight;
                    t.sizeDelta = new Vector2(newLength, 0);
                    break;
                case SideToAdjust.Height:
                    newLength = (t.rect.width * originalHeight) / originalWidth;
                    t.sizeDelta = new Vector2(0, newLength);
                    break;
            }

            Canvas.ForceUpdateCanvases();
        }

        #if UNITY_EDITOR
        private void Update()
        {
            Canvas.ForceUpdateCanvases();

            var t = GetComponent<RectTransform>();
            float newLength = 0;
            switch (sideToAdjust)
            {
                case SideToAdjust.Width:
                    newLength = (t.rect.height * originalWidth) / originalHeight;
                    t.sizeDelta = new Vector2(newLength, 0);
                    break;
                case SideToAdjust.Height:
                    newLength = (t.rect.width * originalHeight) / originalWidth;
                    t.sizeDelta = new Vector2(0, newLength);
                    break;
            }

            Canvas.ForceUpdateCanvases();
        }
        #endif
    }
}