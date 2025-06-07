using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<InventoryData> _inventoryData = new List<InventoryData>();

    public void AddResource(InventoryData resource)
    {
        for (int i = 0; i < _inventoryData.Count; i++)
        {
            if (_inventoryData[i].ResourceName == resource.ResourceName)
            {
                _inventoryData[i].ResourceCount++;
                break;
            }
        }
    }

    public int GetResourceCount(InventoryData resource)
    {
        for (int i = 0; i < _inventoryData.Count; i++)
        {
            if (_inventoryData[i].ResourceName == resource.ResourceName)
            {
                return _inventoryData[i].ResourceCount;
            }
        }
        return 0;
    }

    public InventoryData GetResource(string resourceName) => _inventoryData.Find(res => res.ResourceName == resourceName);
}