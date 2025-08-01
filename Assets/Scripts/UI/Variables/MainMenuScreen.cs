using System;
using Services;
using Services.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Variables
{
    public class MainMenuScreen : BaseWindow
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private TMP_Text _stepsCountText;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private SettingsPopup _settingsPopup;

        public event Action OnPlay;

        private void Start()
        {
            _playButton.onClick.AddListener(StartGame);
            _settingsButton.onClick.AddListener(ShowSettings);
        }

        public override void Show()
        {
            base.Show();
            UpdateTime(StorageService.LoadData(StorageConstants.BEST_TIME, TimeTracker.Instance.ElapsedTime));
            _stepsCountText.text = 
                StorageService.LoadData(StorageConstants.BEST_STEP_COUNT, StepCounter.CurrentSteps).ToString();
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(StartGame);
            _settingsButton.onClick.RemoveListener(ShowSettings);
        }

        private void StartGame()
        {
            OnPlay?.Invoke();
        }

        private void ShowSettings()
        {
            _settingsPopup.Show();
        }

        private void UpdateTime(float elapsedTime)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            _timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}