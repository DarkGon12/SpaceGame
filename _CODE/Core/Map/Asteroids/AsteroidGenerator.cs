using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Services.ObjectPool;
using UnityEngine;
using Zenject;

public class AsteroidGenerator : MonoBehaviour
{
    private readonly Dictionary<Vector3Int, List<Vector3>> _grid = new Dictionary<Vector3Int, List<Vector3>>();
    private readonly List<GameObject> _generatedAsteroid = new List<GameObject>();

    [Inject] private AsteroidGeneratorConfig _asteroidConfig;

    private void Awake()
    {
        ObjectPoolManager.CreatePool("asteroidPool", _asteroidConfig.AsteroidPrefabCollection, _asteroidConfig.AsteroidCount);

        GenerateAsteroids();
        CheckAsteroidCountLoop().Forget();
    }

    [ContextMenu("Generate")]
    private void GenerateAsteroids()
    {
        int placed = 0;
        int attempts = 0;

        while (placed < _asteroidConfig.AsteroidCount && attempts < _asteroidConfig.AsteroidCount * 10)
        {
            Vector3 candidate = GetRandomPosition();
            Vector3Int cell = WorldToGrid(candidate);

            if (!IsTooClose(candidate, cell))
            {
                GameObject asteroid = ObjectPoolManager.GetNextObjectFromPool("asteroidPool");
                if (asteroid == null)
                {
                    Debug.LogWarning("ѕул asteroidPool пуст Ч создание остановлено.");
                    return;
                }

                asteroid.layer = 8;
                asteroid.transform.position = candidate;
                asteroid.transform.rotation = Random.rotation;
                asteroid.transform.SetParent(transform);
                asteroid.SetActive(true);

                if (!_grid.ContainsKey(cell))
                    _grid[cell] = new List<Vector3>();
                _grid[cell].Add(candidate);
                _generatedAsteroid.Add(asteroid);

                placed++;
            }

            attempts++;
        }
    }


    [ContextMenu("Clear")]
    private void ClearAsteroids()
    {
        foreach (var asteroid in _generatedAsteroid)
        {
            if (asteroid != null)
                ObjectPoolManager.ReturnObjectToPool("asteroidPool", asteroid);
        }

        _generatedAsteroid.Clear();
        _grid.Clear();
    }


    private async UniTaskVoid CheckAsteroidCountLoop()
    {
        while (true)
        {
            await UniTask.Delay(_asteroidConfig.RegenerateTime * 1000);

            for (int i = _generatedAsteroid.Count - 1; i >= 0; i--)
            {
                if (_generatedAsteroid[i] == null || !_generatedAsteroid[i].activeSelf)
                {
                    RemovePositionFromGrid(i);
                    _generatedAsteroid.RemoveAt(i);
                }
            }

            int deficit = _asteroidConfig.AsteroidCount - _generatedAsteroid.Count;
            if (deficit > 0)
                GenerateMissingAsteroids(deficit);
        }
    }

    private void GenerateMissingAsteroids(int count)
    {
        int placed = 0;
        int attempts = 0;

        while (placed < count && attempts < count * 10)
        {
            Vector3 candidate = GetRandomPosition();
            Vector3Int cell = WorldToGrid(candidate);

            if (!IsTooClose(candidate, cell))
            {
                GameObject asteroid = ObjectPoolManager.GetNextObjectFromPool("asteroidPool");
                if (asteroid == null)
                {
                    Debug.LogWarning("ѕул asteroidPool пуст Ч невозможно восстановить астероиды.");
                    return;
                }

                asteroid.transform.position = candidate;
                asteroid.transform.rotation = Random.rotation;
                asteroid.transform.SetParent(transform);
                asteroid.SetActive(true);

                if (!_grid.ContainsKey(cell))
                    _grid[cell] = new List<Vector3>();
                _grid[cell].Add(candidate);
                _generatedAsteroid.Add(asteroid);

                placed++;
            }

            attempts++;
        }
    }


    private void RemovePositionFromGrid(int index)
    {
        if (index >= _generatedAsteroid.Count || _generatedAsteroid[index] == null)
            return;

        Vector3 pos = _generatedAsteroid[index].transform.position;
        Vector3Int cell = WorldToGrid(pos);

        if (_grid.TryGetValue(cell, out var list))
        {
            list.RemoveAll(p => Vector3.Distance(p, pos) < _asteroidConfig.AsteroidSpacing);
            if (list.Count == 0)
                _grid.Remove(cell);
        }
    }

    private bool IsTooClose(Vector3 pos, Vector3Int cell)
    {
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
                for (int z = -1; z <= 1; z++)
                {
                    Vector3Int neighbor = cell + new Vector3Int(x, y, z);
                    if (_grid.TryGetValue(neighbor, out var list))
                    {
                        foreach (var p in list)
                        {
                            if (Vector3.Distance(p, pos) < _asteroidConfig.AsteroidSpacing)
                                return true;
                        }
                    }
                }

        return false;
    }

    private Vector3 GetRandomPosition()
    {
        return transform.position + new Vector3(
            Random.Range(-_asteroidConfig.ZoneSize.x / 2, _asteroidConfig.ZoneSize.x / 2),
            Random.Range(-_asteroidConfig.ZoneSize.y / 2, _asteroidConfig.ZoneSize.y / 2),
            Random.Range(-_asteroidConfig.ZoneSize.z / 2, _asteroidConfig.ZoneSize.z / 2)
        );
    }

    private GameObject GetRandomAsteroidPrefab()
    {
        return _asteroidConfig.AsteroidPrefabCollection[
            Random.Range(0, _asteroidConfig.AsteroidPrefabCollection.Count)];
    }

    private Vector3Int WorldToGrid(Vector3 worldPos) => Vector3Int.RoundToInt((worldPos - transform.position) / _asteroidConfig.AsteroidSpacing);
}