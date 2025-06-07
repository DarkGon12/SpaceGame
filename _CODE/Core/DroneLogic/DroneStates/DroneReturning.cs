using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "DroneReturning", menuName = "Drone/States/ReturnToBase")]
public class DroneReturning : DroneState
{
    private Transform _drone;
    private DroneInventory _droneInventory;
    private Vector3 _homePosition;

    [Inject] private IPublisher<ReturnDroneWorkEvent> _returnDroneWorkEventPub;
    [Inject] private IPublisher<StopDroneWorkEvent> _stopDroneWorkEventPub;
    [Inject] private IPublisher<UpdateUserInventoryEvent> _updateUserInventoryEventPub;

    [Inject] private Inventory _inventory;
    [Inject] private DroneConfig _droneConfig;

    public override void Init()
    {
        _drone = MainLogic.transform;
        _homePosition = _droneConfig.GetHomePosition();
        _droneInventory = MainLogic.GetInventory();

        _returnDroneWorkEventPub.Publish(new() { DroneName = MainLogic.GetName() });
    }

    public override void StartAI()
    {
        if (IsFinished) return;

        Vector3 direction = (_homePosition - _drone.position);
        Vector3 moveDir = direction.normalized;

        _drone.position += moveDir * _droneConfig.GetSpeed() * Time.deltaTime;

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            _drone.rotation = Quaternion.Slerp(_drone.rotation, targetRotation, _droneConfig.GetRotationSpeed() * Time.deltaTime);
        }

        if (direction.magnitude <= _droneConfig.StopDistance())
        {        
            if (_droneInventory != null && _droneInventory.GetCount() > 0)
            {
                Debug.Log("Drone returned and unloaded inventory.");
                WaitHomePosition().Forget();
            }

            IsFinished = true;
        }
    }

    private async UniTaskVoid WaitHomePosition()
    {
        _stopDroneWorkEventPub.Publish(new() { DroneName = MainLogic.GetName() });

        while (_droneInventory.GetCount() > 0)
        {
            InventoryData resource = _droneInventory.GetFirstResource();

            _inventory.AddResource(resource);
            _updateUserInventoryEventPub.Publish(new() { ResourceData = resource });

            _droneInventory.RemoveFirstResource();
            await UniTask.Delay(_droneConfig.GetUnloadTime() * 1000);
        }

        MainLogic.SetWork();
    }
}
