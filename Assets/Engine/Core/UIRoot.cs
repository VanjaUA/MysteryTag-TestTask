#region

using UnityEngine;

#endregion

namespace MergeGridGame.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UIRoot : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform screensContainer;
        [SerializeField] private RectTransform dragLayer;

        public RectTransform ScreensContainer => screensContainer;
        public RectTransform DragLayer => dragLayer;
        public Camera UICamera { get; private set; }

        public void Initialize(Camera uiCamera)
        {
            UICamera = uiCamera;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = uiCamera;
            canvas.planeDistance = 10f;
        }
    }
}
