using UnityEngine;

[CreateAssetMenu(fileName = "DroneConfig", menuName = "Config/Drone")]
public class DroneConfig : ConfigBase
{
    [SerializeField, Range(1, 300)] private int _droneLimite = 12;

    [SerializeField, Range(1,30)] private float _droneSpeed = 20f;
    [SerializeField, Range(1, 10)] private float _rotationSpeed = 5f;
    [SerializeField, Range(0, 10)] private float _stopDistance = 2f;

    [SerializeField] private int _homeWaitTime = 4;
    [SerializeField] private int _unloadTime = 1;
    [SerializeField] private int _miningTime = 2;

    [SerializeField] private Vector3 _homeVector;

    public bool DrawMoveLine;

    public float GetSpeed() => _droneSpeed;

    public float GetRotationSpeed() => _rotationSpeed;

    public float StopDistance() => _stopDistance;

    public Vector3 GetHomePosition() => _homeVector;

    public int GetHomeWaitTime() => _homeWaitTime;
    public int GetUnloadTime() => _unloadTime;

    public int GetDroneLimit() => _droneLimite;

    public int GetMiningSpeed() => _miningTime;


    public void SetDroneSpeed(int droneSpeed) => _droneSpeed = droneSpeed;
    public void SetDroneLimit(int limit) => _droneLimite = limit;
}