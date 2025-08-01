using Services;
using TMPro;
using UnityEngine;

namespace UI.Variables
{
    public class GameplayScreen : BaseWindow
    {
        [SerializeField] private TMP_Text _stepsCountText;
        [SerializeField] private TMP_Text _timeText;

        private void Start()
        {
            StepCounter.OnAddedSteps += UpdateStepsCount;
        }

        public override void Show()
        {
            base.Show();
            UpdateTime(TimeTracker.Instance.ElapsedTime);
            UpdateStepsCount(StepCounter.CurrentSteps);
        }

        private void OnDestroy()
        {
            StepCounter.OnAddedSteps -= UpdateStepsCount;
        }

        private void Update()
        {
            UpdateTime(TimeTracker.Instance.ElapsedTime);
        }

        private void UpdateStepsCount(int stepsCount)
        {
            if (_stepsCountText == null) return;

            _stepsCountText.text = stepsCount.ToString();
        }

        private void UpdateTime(float elapsedTime)
        {
            var minutes = Mathf.FloorToInt(elapsedTime / 60f);
            var seconds = Mathf.FloorToInt(elapsedTime % 60f);
            _timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}