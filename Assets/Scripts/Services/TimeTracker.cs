using UnityEngine;

namespace Services
{
    public class TimeTracker : MonoSingleton<TimeTracker>
    {
        public float ElapsedTime { get; private set; }

        private bool _isTracking;

        private void Awake()
        {
            InitializeSingleton();
        }

        private void Update()
        {
            if (!_isTracking) return;

            ElapsedTime += Time.deltaTime;
        }

        public void StartTimer()
        {
            ElapsedTime = 0f;
            _isTracking = true;
        }

        public void StopTimer()
        {
            _isTracking = false;
        }

        public void ResetTimer()
        {
            ElapsedTime = 0f;
        }
    }
}