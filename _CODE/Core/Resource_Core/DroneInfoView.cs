using System;
using System.Collections.Generic;
using MessagePipe;
using TMPro;
using UnityEngine;
using Zenject;

public class DroneInfoView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _flyingDroneText;
    [SerializeField] private TextMeshProUGUI _returningDroneText;
    [SerializeField] private TextMeshProUGUI _idleDroneText;
    [SerializeField] private TextMeshProUGUI _droneLimitText;

    [Inject] private ISubscriber<StartDroneWorkEvent> _startDroneEventSub;
    [Inject] private ISubscriber<ReturnDroneWorkEvent> _returnDroneEventSub;
    [Inject] private ISubscriber<StopDroneWorkEvent> _stopDroneEventSub;
    [Inject] private ISubscriber<BuyDroneEvent> _buyDroneEventSub;

    [Inject] private DroneConfig _droneConfig;

    private HashSet<string> _flyingDrones = new();
    private HashSet<string> _returningDrones = new();
    private HashSet<string> _idleDrones = new();

    private IDisposable _disposable;

    private void Awake()
    {
        var bag = DisposableBag.CreateBuilder();
        _startDroneEventSub.Subscribe(OnDroneStart).AddTo(bag);
        _returnDroneEventSub.Subscribe(OnDroneReturn).AddTo(bag);
        _stopDroneEventSub.Subscribe(OnDroneStop).AddTo(bag);
        _buyDroneEventSub.Subscribe(OnDroneBuy).AddTo(bag);
        _disposable = bag.Build();
    }

    private void OnDroneStart(StartDroneWorkEvent evt)
    {
        string droneName = evt.DroneName;

        _idleDrones.Remove(droneName);
        _returningDrones.Remove(droneName);

        if (_flyingDrones.Add(droneName))
            UpdateTexts();
    }

    private void OnDroneReturn(ReturnDroneWorkEvent evt)
    {
        string droneName = evt.DroneName;

        _flyingDrones.Remove(droneName);
        _idleDrones.Remove(droneName);

        if (_returningDrones.Add(droneName))
            UpdateTexts();
    }

    private void OnDroneStop(StopDroneWorkEvent evt)
    {
        string droneName = evt.DroneName;

        _flyingDrones.Remove(droneName);
        _returningDrones.Remove(droneName);

        if (_idleDrones.Add(droneName))
            UpdateTexts();
    }

    private void OnDroneBuy(BuyDroneEvent evt)
    {
        if(evt.DroneCount == null || evt.DroneCount == 0)
            _droneLimitText.text = 1 + "/" + _droneConfig.GetDroneLimit();

        _droneLimitText.text = evt.DroneCount + "/" + _droneConfig.GetDroneLimit();
    }

    private void UpdateTexts()
    {
        _flyingDroneText.text = $"Flying: {_flyingDrones.Count}";
        _returningDroneText.text = $"Returning: {_returningDrones.Count}";
        _idleDroneText.text = $"Idle: {_idleDrones.Count}";
    }

    private void OnDestroy() => _disposable?.Dispose();
}
