using System;

namespace Services
{
    public static class StepCounter
    {
        public static int CurrentSteps { get; private set; }
        public static event Action<int> OnAddedSteps;
        
        public static void AddStep()
        {
            CurrentSteps++;
            OnAddedSteps?.Invoke(CurrentSteps);
        }

        public static void ResetSteps()
        {
            CurrentSteps = 0;
        }
    }
}