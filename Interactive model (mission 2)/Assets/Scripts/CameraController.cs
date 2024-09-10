using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 2.0f;
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 100.0f;
    public float minZoom = 5.0f;
    public float maxZoom = 50.0f;

    private float _currentZoom = 20.0f;

    private Vector3 _lastMousePos;

    private bool _isRotating = false;

    public bool restoreStartPosAndRot = true;
    private Vector3 _startPos;
    private Quaternion _startRot;

    [SerializeField] private Camera _camera;

    private FocalObjectHandler _focalObjectHandler;

    private Transform _focalTarget;

    public void Init(FocalObjectHandler focalObjectHandler)
    {
        _focalObjectHandler = focalObjectHandler;
        _focalObjectHandler.focalObjectChanged += OnFocalChanged;

        _startPos = _camera.transform.position;
        _startRot = _camera.transform.rotation;
    }

    public void Process()
    {
        HandleVerticalMove();

        if (_focalObjectHandler.focalObject != null)
        {
            HandleRotation();
            HandleZoom();
        }
    }

    private void OnFocalChanged(GameObject focal)
    {
        if (focal == null && restoreStartPosAndRot)
        {
            _camera.transform.position = _startPos;
            _camera.transform.rotation = _startRot;
        }
    }

    private void HandleVerticalMove()
    {
        // Если зажата левая кнопка мыши
        if (Input.GetMouseButton(0))
        {
            Vector3 move = Vector3.up * -Input.GetAxis("Mouse Y");
            _camera.transform.position += move * moveSpeed * Time.deltaTime;
        }
    }

    private void HandleZoom()
    {
        _currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        _currentZoom = Mathf.Clamp(_currentZoom, minZoom, maxZoom);

        // Обновляем положение камеры, чтобы фокус оставался на цели
        _camera.transform.position = _focalTarget.position - _camera.transform.forward * _currentZoom;
    }

    private void HandleRotation()
    {
        _focalTarget = _focalObjectHandler.focalObject.transform;

        // Если правая кнопка зажата, вращаем камеру
        if (Input.GetMouseButtonDown(1))
        {
            _isRotating = true;
            _lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(1))
        {
            _isRotating = false;
        }

        if (_isRotating)
        {
            Vector3 delta = Input.mousePosition - _lastMousePos;
            float rotationY = delta.x * rotationSpeed * Time.deltaTime;
            float rotationX = -delta.y * rotationSpeed * Time.deltaTime;

            // Вращаем камеру на фокусе
            _camera.transform.RotateAround(_focalTarget.position, Vector3.up, rotationY);
            _camera.transform.RotateAround(_focalTarget.position, _camera.transform.right, rotationX);

            _lastMousePos = Input.mousePosition;
        }
    }
}
