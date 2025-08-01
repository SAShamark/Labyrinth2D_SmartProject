using Gameplay.Character;
using Gameplay.Environments;
using Services;
using Services.Storage;
using UI;

public class GameStateController
{
    private readonly CharacterControl _character;
    private readonly MazeGenerator _mazeGenerator;
    private readonly TimeTracker _timeTracker;
    private readonly UIManager _uiManager;

    public GameStateController(CharacterControl character, MazeGenerator mazeGenerator, TimeTracker timeTracker,
        UIManager uiManager)
    {
        _character = character;
        _mazeGenerator = mazeGenerator;
        _timeTracker = timeTracker;
        _uiManager = uiManager;
    }

    internal void StartGameplay()
    {
        _character.OnMazePassed += EndGameplay;

        _mazeGenerator.GenerateMaze();
        _timeTracker.ResetTimer();
        _timeTracker.StartTimer();
        StepCounter.ResetSteps();
    }

    internal void EndGameplay()
    {
        var currentTime = _timeTracker.ElapsedTime;
        var currentStepCount = StepCounter.CurrentSteps;

        ShowResultPopup(currentTime, currentStepCount);
        SaveBestRecordsIfNeeded(currentTime, currentStepCount);

        _character.OnMazePassed -= EndGameplay;
    }

    private void ShowResultPopup(float currentTime, int currentStepCount)
    {
        var resultPopup = _uiManager.ResultPopup;
        resultPopup.OnClose += ShowMainMenu;
        resultPopup.Show();
        resultPopup.Draw(currentTime, currentStepCount);
    }

    internal void ShowMainMenu()
    {
        _uiManager.ShowWindow(WindowType.MainMenu);
        _uiManager.ResultPopup.OnClose -= ShowMainMenu;
    }

    private void SaveBestRecordsIfNeeded(float currentTime, int currentStepCount)
    {
        if (currentTime < StorageService.LoadData(StorageConstants.BEST_TIME, float.MaxValue))
        {
            StorageService.SaveData(StorageConstants.BEST_TIME, currentTime);
        }

        if (currentStepCount < StorageService.LoadData(StorageConstants.BEST_STEP_COUNT, int.MaxValue))
        {
            StorageService.SaveData(StorageConstants.BEST_STEP_COUNT, currentStepCount);
        }
    }
}