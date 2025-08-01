using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Variables
{
    public class ResultPopup : BaseWindow
    {
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private TMP_Text _stepsCountText;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private Animation _starsAnimation;

        public event Action OnClose;

        private void Start()
        {
            _mainMenuButton.onClick.AddListener(CloseTrigger);
        }

        public void Draw(float elapsedTime, int stepsCount)
        {
            _starsAnimation.Play();
            _stepsCountText.text = stepsCount.ToString();
            UpdateTime(elapsedTime);
        }

        private void UpdateTime(float elapsedTime)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            _timeText.text = $"{minutes:00}:{seconds:00}";
        }

        private void CloseTrigger()
        {
            OnClose?.Invoke();
            Hide();
        }
    }
}