using UnityEngine;

[CreateAssetMenu(fileName = "CameraConfig", menuName = "Config/Controller/Camera")]
public class CameraConfig : ConfigBase
{
    [Header("Movement Settings")]
    public float MoveSpeed = 50f;
    public float BorderThickness = 10f;

    [Header("Clamp")]
    public Vector2 HeightClamp = new(30f, 100f);
    public Vector2 MapBoundsX = new(-500f, 500f);
    public Vector2 MapBoundsZ = new(-500f, 500f);

    public bool EdgeScrollEnabled = true;
}