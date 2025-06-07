using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class DroneLogic : MonoBehaviour
{
    [Header("Настройка")]
    [SerializeField] private DroneState _startState;
    [SerializeField] private GameObject _leftEffect;
    [SerializeField] private GameObject _rightEffect;
    [SerializeField] private DroneInventory _inventoryDrone;
    [Space(10)]
    [Header("Debag")]
    [SerializeField] private DroneState _currentState; // dinamic
    [SerializeField] private DroneState _workState; // dinamic
    [SerializeField] private LineRenderer _lineRenderer;

    private string _droneName;
    private Transform _npcPosition;

    [Inject] private DiContainer _diContainer;

    #region Gets
    public GameObject GetLeftEffect() => _leftEffect;
    public GameObject GetRightEffect() => _rightEffect;

    public DroneInventory GetInventory() => _inventoryDrone;

    public LineRenderer GetLineRenderer() => _lineRenderer;

    public Transform Target { get; private set; }
    public Transform Point { get; private set; }

    public string GetName() => _droneName;
    #endregion

    private void Awake()
    {
        _droneName = "drone_" + UnityEngine.Random.Range(0,10000);
    }

    public void SetTarget(DroneState state, Transform target)
    {
        Target = target;
        SetState(state); 
    }

    public void SetAsteroid(Transform target, Transform point)
    {
        Target = target;
        Point = point;

        DroneOrbitAsteroid orbit = _diContainer.Instantiate<DroneOrbitAsteroid>();
        _diContainer.Inject(orbit);
        SetState(orbit);
    }

    /*
    Debug void
    */
    [ContextMenu("Debug Work")]
    public void SetWork()
    {
        DroneWorker worker = _diContainer.Instantiate<DroneWorker>();      
        _diContainer.Inject(worker);
        SetState(worker);
    }

    public void SetReturning()
    {
        DroneReturning returning = _diContainer.Instantiate<DroneReturning>();
        _diContainer.Inject(returning);
        SetState(returning);
    }

    public void Start()
    {
        _currentState = null;
        _npcPosition = transform;

        SetState(_startState);
    }

    private void Update()
    {
        if (!_currentState) return;

        if (!_currentState.IsFinished)
            _currentState.StartAI();
        else
        {
            // new state
        }
    }

    public void SetState(DroneState state)
    {
        if (state.IsUnityNull()) return;

        _currentState = Instantiate(state);
        _diContainer.Inject(_currentState);

        _currentState.SetLogic(this);
        _currentState.Init();
    }

    public DroneState GetWorkState() => _workState;
}