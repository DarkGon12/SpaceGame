using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Zenject;
using TMPro;
using MessagePipe;
using System.Collections.Generic;

public class ShopInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private List<GameObject> _droneList = new List<GameObject>();

    [SerializeField] private InventoryData _buyElementData;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private GameObject _craftingDronePrefab;
    [Header("Info panel")]
    [SerializeField] private GameObject _infoPanel;
    [SerializeField] private TextMeshProUGUI _infoPanelText;
    [SerializeField] private Image _infoPanelIcon;
    [Space(20)]
    [SerializeField] private float _fadeDuration = 0.3f;
    [SerializeField] private CanvasGroup _infoCanvasGroup;
    [SerializeField] private Button _buyButton;

    [Inject] private IPublisher<BuyDroneEvent> _buyDroneEventPub;
    [Inject] private Inventory _inventory;
    [Inject] private DiContainer _diContainer;

    private void Start()
    {
        _infoPanel.SetActive(false);
        _infoCanvasGroup.alpha = 0;
        _buyButton.onClick.AddListener(TryBuyDrone);
        InitInfo();
    }

    private void InitInfo()
    {
        _infoPanelText.text = _buyElementData.ResourceName + "\n" + _buyElementData.ResourceCount;
        _infoPanelIcon.sprite = _buyElementData.ResourceIcon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _infoPanel.SetActive(true);
        _infoCanvasGroup.alpha = 0;
        _infoPanel.transform.localScale = Vector3.one * 0.8f;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_infoCanvasGroup.DOFade(1f, _fadeDuration));
        sequence.Join(_infoPanel.transform.DOScale(1f, _fadeDuration).SetEase(Ease.OutBack));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_infoCanvasGroup.DOFade(0f, _fadeDuration));
        sequence.Join(_infoPanel.transform.DOScale(0.8f, _fadeDuration).SetEase(Ease.InBack));
        sequence.OnComplete(() =>
        {
            _infoPanel.SetActive(false);
        });
    }


    private void TryBuyDrone()
    {
        var resource = _inventory.GetResource(_buyElementData.ResourceName);
        if (resource != null && resource.ResourceCount >= _buyElementData.ResourceCount)
        {
            resource.ResourceCount -= _buyElementData.ResourceCount;
            BuyDrone();
        }
        else
        {
            Debug.Log("Not enough Nikel!");
        }
    }

    private void BuyDrone()
    {
        GameObject drone = _diContainer.InstantiatePrefab(_craftingDronePrefab);
        _diContainer.Inject(drone);
        drone.transform.SetParent(null);
        drone.transform.position = _spawnPoint.localPosition;
        drone.GetComponent<DroneLogic>().SetWork();
        _droneList.Add(drone);

        _buyDroneEventPub.Publish(new() { DroneCount = _droneList.Count });
    }
}
