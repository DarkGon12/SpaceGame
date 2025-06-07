using System.Collections.Generic;
using UnityEngine;

public class DroneSelector : MonoBehaviour
{
    [Header("Настройки формации")]
    [SerializeField] private float formationSpacing = 5f;
    [SerializeField] private float stopDistance = 2f;
    [Space(10)]
    [SerializeField] private LayerMask _droneMask;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private LayerMask _asteroidMask;

    private List<DroneLogic> _selectedDrones = new();

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                if (((1 << hit.collider.gameObject.layer) & _droneMask) != 0)
                {
                    DroneLogic logic = hit.collider.GetComponent<DroneLogic>();
                    if (logic != null && !logic.CompareTag("Enemy"))
                    {
                        _selectedDrones.Add(logic);
                    }
                }
                else if (((1 << hit.collider.gameObject.layer) & _groundMask) != 0)
                {
                    AssignFormationTarget(hit.point);
                }
                else if (((1 << hit.collider.gameObject.layer) & _asteroidMask) != 0)
                {
                    AssignAsteroidTarget(hit.transform);
                }
            }
        }
    }

    private void AssignFormationTarget(Vector3 center)
    {
        int row = 0, col = 0;

        for (int i = 0; i < _selectedDrones.Count; i++)
        {
            Vector3 offset = new Vector3(col - row * 0.5f, 0, -row) * formationSpacing;
            Vector3 pos = center + offset;

            // Смещаем на stopDistance назад от центра
            Vector3 dir = (pos - center).normalized;
            if (dir != Vector3.zero)
                pos -= dir * stopDistance;

            Transform target = CreatePoint(pos);
            _selectedDrones[i].SetTarget(new DroneMove(), target);

            col++;
            if (col > row)
            {
                row++;
                col = 0;
            }
        }

        _selectedDrones.Clear();
    }


    private void AssignAsteroidTarget(Transform asteroid)
    {
        Vector3 center = asteroid.position;
        int count = _selectedDrones.Count;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (formationSpacing + stopDistance);
            Vector3 pos = center + offset;

            Transform target = CreatePoint(pos);
            _selectedDrones[i].SetAsteroid(target, asteroid);
        }

        _selectedDrones.Clear();
    }


    private Transform CreatePoint(Vector3 position)
    {
        GameObject point = new GameObject("TargetPoint");
        point.transform.position = position;
        return point.transform;
    }
}
