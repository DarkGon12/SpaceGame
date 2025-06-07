using UnityEngine;
using OctoTree;
using Zenject;
using MessagePipe;

[CreateAssetMenu(fileName = "Custom_State", menuName = "Drone/States/DroneWorker")]
public class DroneWorker : DroneState
{
    private Transform _drone;
    private GameObject _findAsteroid;

    private bool _destroyed, _find;

    private TreeFinder _treeFinder;

    [Inject] private IPublisher<StartDroneWorkEvent> _startDroneWorkEventPub;
    [Inject] private DroneConfig _droneConfig;

    public override void Init()
    {
        _startDroneWorkEventPub.Publish(new() { DroneName = MainLogic.GetName()} );

        _find = false;
        _treeFinder = FindAnyObjectByType<TreeFinder>();
        _drone = MainLogic.transform;
        _treeFinder.RegenerateTree();
    }

    public override void StartAI()
    {
        if (IsFinished) return;

        if (_findAsteroid == null)
            _findAsteroid = FindTreeDesteny();

        if (_find)
            FlyToAsteroid();
        
        if (_droneConfig.DrawMoveLine && _findAsteroid != null)
        {
            LineRenderer line = MainLogic.GetLineRenderer();
            line.positionCount = 2;
            line.SetPosition(0, _drone.position);
            line.SetPosition(1, _findAsteroid.transform.position);
        }
        else
        {
            LineRenderer line = MainLogic.GetLineRenderer();
            line.positionCount = 0;
        }
    }

    public void FlyToAsteroid()
    {
        Vector3 direction = (_findAsteroid.transform.position - _drone.position);
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
            float angle = 0 * 360 * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (10);
            Vector3 pos = _findAsteroid.transform.position + offset;
            Transform target = CreatePoint(pos);

            MainLogic.SetAsteroid(target, _findAsteroid.transform);
        }
    }

    private Transform CreatePoint(Vector3 position)
    {
        GameObject point = new GameObject("TargetPoint");
        point.transform.position = position;
        return point.transform;
    }

    public GameObject FindTreeDesteny()
    {
        if (_treeFinder != null)
        {
            GameObject closestTree = _treeFinder.FindClosestTree(_drone.position);

            if (closestTree != null)
            {
                Debug.Log("Tree: " + closestTree.name + " Vector: " + closestTree.transform.position);
                _find = true;
                return closestTree;
            }
        }
        return null;
    }
}