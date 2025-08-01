using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Variables
{
    public class SettingsPopup : BaseWindow
    {
        [SerializeField] private Button _closeButton;

        [SerializeField] private TMP_InputField _widthInput;
        [SerializeField] private TMP_InputField _heightInput;
        [SerializeField] private TMP_InputField _seedInput;
        [SerializeField] private Slider _exitsSlider;
        [SerializeField] private Slider _complexitySlider;
        [SerializeField] private Toggle _randomSeedToggle;

        [SerializeField] private TMP_Text _exitsLabel;
        [SerializeField] private TMP_Text _complexityLabel;
        [SerializeField] private TMP_Text _seedLabel;

        private MazeDataService _mazeDataService;


        public void Initialize(MazeDataService mazeDataService)
        {
            _mazeDataService = mazeDataService;
            Draw();
            SetupListeners();
        }

        private void Draw()
        {
            _widthInput.text = _mazeDataService.MazeData.MazeWidth.ToString();
            _heightInput.text = _mazeDataService.MazeData.MazeHeight.ToString();
            _exitsSlider.value = _mazeDataService.MazeData.NumberOfExits;
            _exitsSlider.value = _mazeDataService.MazeData.Complexity;
            _randomSeedToggle.isOn = _mazeDataService.MazeData.IsRandomSeed;
            _seedInput.text = _mazeDataService.MazeData.Seed.ToString();
            
            _exitsLabel.text = _exitsSlider.value.ToString("0");
            _complexityLabel.text = _complexitySlider.value.ToString("0.00");
        }

        private void SetupListeners()
        {
            _closeButton.onClick.AddListener(CloseTrigger);
            
            _exitsSlider.onValueChanged.AddListener(value => _exitsLabel.text = value.ToString("0"));
            _complexitySlider.onValueChanged.AddListener(value => _complexityLabel.text = value.ToString("0.00"));
        }

        private void CloseTrigger()
        {
            ApplySettings();
            Hide();
        }

        private void ApplySettings()
        {
            int.TryParse(_widthInput.text, out var width);
            int.TryParse(_heightInput.text, out var height);
            int.TryParse(_seedInput.text, out var seed);
            
            _mazeDataService.MazeData.MazeWidth = width;
            _mazeDataService.MazeData.MazeHeight = height;
            _mazeDataService.MazeData.NumberOfExits = (int)_exitsSlider.value;
            _mazeDataService.MazeData.Complexity = _exitsSlider.value;
            _mazeDataService.MazeData.IsRandomSeed = _randomSeedToggle.isOn;
            _mazeDataService.MazeData.Seed = seed;

            SaveSettings();
        }

        private void SaveSettings()
        {
            _mazeDataService.SaveMazeData();
        }
    }
}