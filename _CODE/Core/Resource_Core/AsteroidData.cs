using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Services.ObjectPool;
using UnityEngine;

public class AsteroidData : MonoBehaviour
{
    [SerializeField] private InventoryData _resourceInfo;
    [Space(15)]
    [SerializeField] private List<Rigidbody> _asteroidExplosive = new List<Rigidbody>();
    [SerializeField] private GameObject _asteroidEffect;
   
    [SerializeField] private int _asteroidHealth;
    [SerializeField] private int _destroyTime = 5;

    private List<Vector3> _localPositionAsteroidElements = new List<Vector3>();
    private MeshRenderer _mesh;

    private void Start()
    {
        _mesh = GetComponent<MeshRenderer>();

        for (int i = 0; i < _asteroidExplosive.Count; i++)
            _localPositionAsteroidElements.Add(_asteroidExplosive[i].transform.localPosition);
    }

    public int GetHealth() => _asteroidHealth;

    public void Damage(int damage)
    {
        _asteroidHealth -= damage;

        if (_asteroidHealth <= 0)
        {
            _asteroidEffect.SetActive(true);

            for (int i = 0; i < _asteroidExplosive.Count; i++)
            {
                _asteroidExplosive[i].isKinematic = false;
                _asteroidExplosive[i].gameObject.SetActive(true);
            }

            HideMainAsteroid();

            DestroyAsteroid(_destroyTime).Forget();
            return;
        }
    }

    private async UniTaskVoid DestroyAsteroid(int time)
    {
        await UniTask.Delay(time * 1000);

        ShowMainAsteroid();
        ReturnAsteroidElementToOrigin();

        _asteroidEffect.SetActive(false);
        ObjectPoolManager.ReturnObjectToPool("asteroidPool", gameObject);
    }

    private void ReturnAsteroidElementToOrigin()
    {
        for (int i = 0; i < _localPositionAsteroidElements.Count; i++)
        {
            _asteroidExplosive[i].isKinematic = true;
            _asteroidExplosive[i].gameObject.SetActive(false);
            _asteroidExplosive[i].transform.localPosition = _localPositionAsteroidElements[i];
        }
    }

    private void ShowMainAsteroid() => _mesh.enabled = true;

    private void HideMainAsteroid() => _mesh.enabled = false;

    public InventoryData GetResource() => _resourceInfo;
}