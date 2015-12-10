using UnityEngine;

namespace AnarchyBros
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;

        public Camera MainCamera;
        public Transform Ground;
        public int Scale;
        public float ScrollSensitivity, MoveSensitivity;

        Vector3 _moveCameraTo;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            _moveCameraTo = MainCamera.transform.position;
        }

        void Update()
        {
            float sdelta = Input.GetAxis("Mouse ScrollWheel");
            float hdelta = Input.GetAxis("Horizontal");
            float vdelta = Input.GetAxis("Vertical");

            UpdateCamera(hdelta, vdelta, sdelta);
        }

        void UpdateCamera(float h, float v, float s)
        {
            MainCamera.orthographicSize -= s * ScrollSensitivity * Scale;

            _moveCameraTo += new Vector3(h, v, 0) * Time.deltaTime * MoveSensitivity * Scale;

            ClampCamera();

            MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, _moveCameraTo, Time.deltaTime * MoveSensitivity * Scale);
        }

        void ClampCamera()
        {
            MainCamera.orthographicSize = Mathf.Max(Scale * 5, Mathf.Min(Scale * 10, MainCamera.orthographicSize));

            float width = (MainCamera.orthographicSize * Screen.width) / Screen.height;
            float height = MainCamera.orthographicSize;

            Vector2 min = Ground.transform.position - 0.5f * Ground.transform.localScale + new Vector3(width, height, 0);
            Vector2 max = Ground.transform.position + 0.5f * Ground.transform.localScale - new Vector3(width, height, 0);

            _moveCameraTo.x = Mathf.Max(min.x, Mathf.Min(max.x, _moveCameraTo.x));
            _moveCameraTo.y = Mathf.Max(min.y, Mathf.Min(max.y, _moveCameraTo.y));
        }

        public void OnMapSizeChanged(float newVal)
        {
            Scale = (int)newVal;

            int width = Scale * 36;
            int height = Scale * 20;

            Ground.transform.localScale = new Vector3(width, height, 0);

            MainCamera.orthographicSize = Scale * 10;

            GraphManager.Instance.OnScaleChanged();
        }
    }
}