using Gameplay.Environments;
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

        private MazeDataService _mazeDataService;


        public void Initialize(MazeDataService mazeDataService)
        {
            _mazeDataService = mazeDataService;
            Draw();
            SetupListeners();
        }

        private void Draw()
        {
            var mazeData = _mazeDataService.MazeData;
            
            _widthInput.text = mazeData.MazeWidth.ToString();
            _heightInput.text = mazeData.MazeHeight.ToString();
            _exitsSlider.value = mazeData.NumberOfExits;
            _exitsSlider.value = mazeData.Complexity;
            _randomSeedToggle.isOn = mazeData.IsRandomSeed;
            _seedInput.text = mazeData.Seed.ToString();
            
            UpdateExitsLabel(_exitsSlider.value);
            UpdateComplexityLabel(_complexitySlider.value);
        }

        private void SetupListeners()
        {
            _closeButton.onClick.AddListener(CloseTrigger);
            
            _exitsSlider.onValueChanged.AddListener(UpdateExitsLabel);
            _complexitySlider.onValueChanged.AddListener(UpdateComplexityLabel);
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

            var mazeData = _mazeDataService.MazeData;
            mazeData.MazeWidth = width;
            mazeData.MazeHeight = height;
            mazeData.Seed = seed;
            
            mazeData.NumberOfExits = (int)_exitsSlider.value;
            mazeData.Complexity = _complexitySlider.value;
            mazeData.IsRandomSeed = _randomSeedToggle.isOn;

            _mazeDataService.SaveMazeData();
        }
        
        private void UpdateExitsLabel(float value)
        {
            _exitsLabel.text = value.ToString("0");
        }

        private void UpdateComplexityLabel(float value)
        {
            _complexityLabel.text = value.ToString("0.00");
        }
    }
}