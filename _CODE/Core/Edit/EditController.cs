using UnityEngine.UI;
using UnityEngine;
using Zenject;
using TMPro;
using MessagePipe;

public class EditController : MonoBehaviour
{
    [SerializeField] private GameObject _editPanel;
    [SerializeField] private Button _openEditPanel, _closeEditPanel;
    [Space(15)]
    [SerializeField] private Slider _droneCountSlider, _droneSpeedSlider;
    [SerializeField] private TMP_InputField _asteroidGenerationInput;
    [SerializeField] private Toggle _drawDroneLine;

    [SerializeField] private TextMeshProUGUI _droneCountText, _droneSpeedText, _asteroidTimeText;

    [Inject] private IPublisher<BuyDroneEvent> _buyDroneEventPub;
    [Inject] private AsteroidGeneratorConfig _asteroidConfig;
    [Inject] private DroneConfig _droneConfig;

    private bool _isOpen = false;
        
    private void Start()
    {
        _editPanel.SetActive(false);

        _openEditPanel.onClick.AddListener(OpenEditPanel);
        _closeEditPanel.onClick.AddListener(CloseEditPanel);
    }

    private void OpenEditPanel()
    {
        _editPanel.SetActive(true);
        _openEditPanel.onClick.AddListener(CloseEditPanel);

        _droneCountSlider.value = _droneConfig.GetDroneLimit();
        _droneSpeedSlider.value = _droneConfig.GetSpeed();
        _asteroidTimeText.text = "Время генерации: " + _asteroidConfig.RegenerateTime;
        _drawDroneLine.isOn = _droneConfig.DrawMoveLine;
    }

    private void CloseEditPanel()
    {
        _editPanel.SetActive(false);
        _openEditPanel.onClick.AddListener(OpenEditPanel);
    }

    public void UpdateDroneCountSlider()
    {
        _droneCountText.text = "Дронов: " + _droneCountSlider.value;
        _droneConfig.SetDroneLimit((int)_droneCountSlider.value);
        _buyDroneEventPub.Publish(new());
    }

    public void UpdateDroneSpeedSlider()
    {
        _droneSpeedText.text = "Скорость: " + _droneSpeedSlider.value;
        _droneConfig.SetDroneSpeed((int)_droneSpeedSlider.value);
    }

    public void UpdateAsteroidGenerationTime()
    {
        _asteroidTimeText.text = "Время генерации: " + _asteroidGenerationInput.text;
        _asteroidConfig.RegenerateTime = int.Parse(_asteroidGenerationInput.text);
    }

    public void UpdateDrawLineDrone()
    {
        _droneConfig.DrawMoveLine = _drawDroneLine.isOn;
    }
}