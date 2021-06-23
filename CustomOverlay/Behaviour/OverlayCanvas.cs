using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

namespace CustomOverlay.Behaviour
{
    public class OverlayCanvas : MonoBehaviour
    {
        public static GameObject instance { get; private set; }
        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            GameObject CanvasObject = new GameObject();
            instance = CanvasObject;
            DontDestroyOnLoad(CanvasObject);

            CanvasObject.transform.position = new Vector3(0, 0, 0);

            Canvas canvas = CanvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            CanvasScaler scalar = CanvasObject.AddComponent<CanvasScaler>();
            scalar.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scalar.referenceResolution = new Vector2(1920, 1080);

            CanvasObject.AddComponent<GraphicRaycaster>();
        }
    }
}