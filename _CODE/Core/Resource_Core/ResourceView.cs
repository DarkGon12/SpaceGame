using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using MessagePipe;
using Zenject;
using System;
using TMPro;

public class ResourceView : MonoBehaviour
{
    [SerializeField] private GameObject _gameResourcePrefab;

    private List<GameResourcesElement> _gameResourcesCollection = new List<GameResourcesElement>();
    private List<TextMeshProUGUI> _resourcesTextCollection = new List<TextMeshProUGUI>();

    [Inject] private ISubscriber<UpdateUserInventoryEvent> _updateUserInventoryEventSub;
    [Inject] private ResourceCollection _gameResources;
    [Inject] private Inventory _inventory;

    private IDisposable _disposable;

    private void Awake()
    {
        var bag = DisposableBag.CreateBuilder();
        _updateUserInventoryEventSub.Subscribe(UpdateUserInventory).AddTo(bag);
        _disposable = bag.Build();      
    }

    private void Start()
    {
        DrawResources();
    }

    private void DrawResources()
    {
        _gameResourcesCollection = _gameResources.GetCollection();

        for (int i = 0; i < _gameResourcesCollection.Count; i++)
        {
            GameObject element = Instantiate(_gameResourcePrefab, transform);
            element.transform.GetChild(0).GetComponent<Image>().sprite = _gameResourcesCollection[i].ResourceIcon;
            element.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _gameResourcesCollection[i].ResourceName + ": 0";

            _resourcesTextCollection.Add(element.transform.GetChild(1).GetComponent<TextMeshProUGUI>());
        }
    }

    private void UpdateUserInventory(UpdateUserInventoryEvent evt)
    {
        for (int i = 0; i < _gameResourcesCollection.Count; i++)
        {
            if (_gameResourcesCollection[i].ResourceName == evt.ResourceData.ResourceName)
            {
                _resourcesTextCollection[i].text = evt.ResourceData.ResourceName + ": " + _inventory.GetResourceCount(evt.ResourceData);
                break;
            }
        }
    }

    private void OnDestroy() => _disposable?.Dispose();
}