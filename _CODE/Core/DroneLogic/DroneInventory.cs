using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DroneInventory : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryCanvas;
    [SerializeField] private TextMeshProUGUI _inventoryTitle;
    [SerializeField] private Image _inventoryIcon;
    [Space(20)]
    [SerializeField] private List<InventoryData> _inventoryData = new List<InventoryData>();

    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void FixedUpdate()
    {
        _inventoryCanvas.transform.forward = (_inventoryCanvas.transform.position - _cam.transform.position).normalized;
    }

    private void UpdateCanvas(InventoryData resource)
    {
        _inventoryTitle.text = resource.ResourceName + ":" + resource.ResourceCount.ToString();
        _inventoryIcon.sprite = resource.ResourceIcon;
    }

    public void AddResource(InventoryData resource)
    {
        bool found = false;

        for (int i = 0; i < _inventoryData.Count; i++)
        {
            if (_inventoryData[i].ResourceName == resource.ResourceName)
            {
                _inventoryData[i].ResourceCount++;
                found = true;
                UpdateCanvas(resource);
                break;
            }
        }

        if (!found)
        {
            _inventoryData.Add(resource);
            _inventoryCanvas.SetActive(true);
            UpdateCanvas(resource);
        }
    }

    public void RemoveOneResource(InventoryData resource)
    {
        bool found = false;

        for (int i = 0; i < _inventoryData.Count; i++)
        {
            if (_inventoryData[i].ResourceName == resource.ResourceName)
            {
                _inventoryData[i].ResourceCount--;
                found = true;
                break;
            }
        }

        if (!found)
            _inventoryData.Remove(resource);
    }

    public void RemoveFirstResource()
    {
        if (_inventoryData.Count > 0)
        {
            if (_inventoryData[0].ResourceCount > 1)
            {
                _inventoryData[0].ResourceCount--;
                _inventoryTitle.text = _inventoryData[0].ResourceName + ":" + _inventoryData[0].ResourceCount.ToString();
            }
            else
            {
                _inventoryData.RemoveAt(0);
                _inventoryCanvas.SetActive(false);
            }
        }
    }

    public InventoryData GetFirstResource()
    {
        if (_inventoryData.Count > 0)
            return _inventoryData[0];
        else
            return null;
    }

    public void RemoveAllResource() => _inventoryData.Clear();

    public int GetCount() => _inventoryData.Count; 
}