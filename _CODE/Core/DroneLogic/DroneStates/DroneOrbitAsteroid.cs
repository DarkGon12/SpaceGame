using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Custom_State", menuName = "Drone/States/OrbitAsteroid")]
public class DroneOrbitAsteroid : DroneState
{
    private AsteroidData _asteroid;
    private DroneInventory _inventory;

    private Transform _drone;
    private Transform _target;
    private Transform _point;

    private bool _isAttack = false;

    [Inject] private DroneConfig _droneConfig;


    public override void Init()
    {
        _drone = MainLogic.transform;
        _target = MainLogic.Target;
        _point = MainLogic.Point;
        _inventory = MainLogic.GetInventory();

        _asteroid = _point.GetComponent<AsteroidData>();

        if (_droneConfig.DrawMoveLine)
        {
            LineRenderer line = MainLogic.GetLineRenderer();
            line.SetPosition(0, _drone.position);
            line.SetPosition(1, _target.position);
        }

        if (_target == null) IsFinished = true;
    }

    public override void StartAI()
    {        
        Vector3 direction = (_target.position - _drone.position);
        float distance = direction.magnitude;

        if (distance > _droneConfig.StopDistance())
        {
            Vector3 moveDir = direction.normalized;
            _drone.position += moveDir * _droneConfig.GetSpeed() * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            _drone.rotation = Quaternion.Slerp(_drone.rotation, targetRotation, _droneConfig.GetRotationSpeed() * Time.deltaTime);
        }
        else
        {
            MainLogic.GetLeftEffect().SetActive(true);
            MainLogic.GetRightEffect().SetActive(true);

            _drone.LookAt(_point.position);

            DamageAsteroid().Forget();
        }
    }

    private async UniTask DamageAsteroid()
    {
        if (_isAttack) return;
        _isAttack = true;

        await UniTask.Delay(_droneConfig.GetMiningSpeed() * 1000);

        _asteroid.Damage(25);
        _inventory.AddResource(_asteroid.GetResource());

        if (_asteroid.GetHealth() <= 0)
        {
            IsFinished = true;
            MainLogic.SetWork();
            MainLogic.GetLeftEffect().SetActive(false);
            MainLogic.GetRightEffect().SetActive(false);

            MainLogic.SetReturning();
        }

        _isAttack = false;
    }
}