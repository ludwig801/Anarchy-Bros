using UnityEngine;

public class GameCameraManager : MonoBehaviour
{
    public Camera GameCamera;
    public int Scale;
    public float ScrollSensitivity, MoveSensitivity;

    GameManager _gameManager;
    Vector3 _moveCameraTo;
    float _screenRatio;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _moveCameraTo = GameCamera.transform.position;

        _screenRatio = Screen.width / Screen.height;
    }

    void Update()
    {
        float sDelta = Input.GetAxis("Mouse ScrollWheel");
        float hDelta = Input.GetAxis("Horizontal");
        float vDelta = Input.GetAxis("Vertical");

        GameCamera.orthographicSize -= sDelta * ScrollSensitivity * Scale;

        _moveCameraTo += new Vector3(hDelta, vDelta, 0) * Time.deltaTime * MoveSensitivity * Scale;

        ClampCamera();

        GameCamera.transform.position = Vector3.Lerp(GameCamera.transform.position, _moveCameraTo, Time.deltaTime * MoveSensitivity * 2 * Scale);
    }

    void ClampCamera()
    {
        GameCamera.orthographicSize = Mathf.Max(Scale * 5, Mathf.Min(Scale * 10, GameCamera.orthographicSize));

        float width = GameCamera.orthographicSize * _screenRatio;
        float height = GameCamera.orthographicSize;

        Vector2 mapBottomLeft = _gameManager.Map.Center - 0.5f * _gameManager.Map.Size + new Vector2(width, height);
        Vector2 mapTopRight = _gameManager.Map.Center + 0.5f * _gameManager.Map.Size - new Vector2(width, height);

        _moveCameraTo.x = Mathf.Max(mapBottomLeft.x, Mathf.Min(mapTopRight.x, _moveCameraTo.x));
        _moveCameraTo.y = Mathf.Max(mapBottomLeft.y, Mathf.Min(mapTopRight.y, _moveCameraTo.y));
    }
}