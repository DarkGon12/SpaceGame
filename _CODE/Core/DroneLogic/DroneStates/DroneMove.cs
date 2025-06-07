using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "CustomState", menuName = "NPC/States/MoveToPoint")]
public class DroneMove : DroneState
{
    private Transform _drone;
    private Vector3 _destination;

    [Inject] private DroneConfig _droneConfig;

    public override void Init()
    {
        _drone = MainLogic.transform;
        _destination = MainLogic.Target ? MainLogic.Target.position : _drone.position;

        if(_droneConfig.DrawMoveLine)
        {
            LineRenderer line = MainLogic.GetLineRenderer();
            line.SetPosition(0, _drone.position);
            line.SetPosition(1, _destination);
        }
    }

    public override void StartAI()
    {
        if (IsFinished) return;

        Vector3 direction = (_destination - _drone.position);
        Vector3 moveDir = direction.normalized;

        _drone.position += moveDir * _droneConfig.GetSpeed() * Time.deltaTime;

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            _drone.rotation = Quaternion.Slerp(_drone.rotation, targetRotation, _droneConfig.GetRotationSpeed() * Time.deltaTime);
        }

        if (direction.magnitude <= _droneConfig.StopDistance())
            IsFinished = true;
    }
}
