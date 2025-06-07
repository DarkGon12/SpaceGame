using UnityEngine;
using Zenject;

public class SpaceCameraController : MonoBehaviour
{
    [Inject] private CameraConfig _cameraConfig;

    private void Update()
    {
        Vector3 inputDir = GetKeyboardInput() + (_cameraConfig.EdgeScrollEnabled ? GetEdgeScrollInput() : Vector3.zero);
        MoveCamera(inputDir.normalized);
    }

    private Vector3 GetKeyboardInput()
    {
        return new Vector3(
            Input.GetAxisRaw("Horizontal"), 0f,
            Input.GetAxisRaw("Vertical")
        );
    }

    private Vector3 GetEdgeScrollInput()
    {
        Vector3 dir = Vector3.zero;
        Vector3 mouse = Input.mousePosition;

        if (mouse.x >= Screen.width - _cameraConfig.BorderThickness) dir += Vector3.right;
        if (mouse.x <= _cameraConfig.BorderThickness) dir += Vector3.left;
        if (mouse.y >= Screen.height - _cameraConfig.BorderThickness) dir += Vector3.forward;
        if (mouse.y <= _cameraConfig.BorderThickness) dir += Vector3.back;

        return dir;
    }

    private void MoveCamera(Vector3 direction)
    {
        Vector3 delta = new Vector3(direction.x, 0f, direction.z) * _cameraConfig.MoveSpeed * Time.deltaTime;
        Vector3 newPos = transform.position + delta;

        newPos.x = Mathf.Clamp(newPos.x, _cameraConfig.MapBoundsX.x, _cameraConfig.MapBoundsX.y);
        newPos.y = Mathf.Clamp(newPos.y, _cameraConfig.HeightClamp.x, _cameraConfig.HeightClamp.y);
        newPos.z = Mathf.Clamp(newPos.z, _cameraConfig.MapBoundsZ.x, _cameraConfig.MapBoundsZ.y);

        transform.position = newPos;
    }
}