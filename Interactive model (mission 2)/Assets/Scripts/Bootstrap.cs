using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private ObservablesController _observablesController;

    private void Awake()
    {
        _observablesController.Init();

        _cameraController.Init(_observablesController.FocalObjectHandler);
    }

    private void LateUpdate()
    {
        _cameraController.Process();
    }
}
