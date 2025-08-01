using Cinemachine;
using Gameplay.Character;
using Gameplay.Environments;
using Services;
using Services.Storage;
using UI;
using UI.Variables;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private MazeGenerator _mazeGenerator;
    [SerializeField] private CinemachineVirtualCamera _camera;
    [SerializeField] private MazeConfig _mazeConfig;

    [SerializeField] private CharacterControl _characterControl;
    [SerializeField] private Transform _characterContainer;

    private CharacterControl _character;
    private MazeDataService _mazeDataService;
    private TimeTracker _timeTracker;

    private void Start()
    {
        _mazeDataService = new MazeDataService(_mazeConfig);
        _uiManager.Initialzie(_mazeDataService);
        _uiManager.ShowWindow(WindowType.MainMenu);
        _character = Instantiate(_characterControl, _characterContainer);
        InitCamera();
        _mazeGenerator.Initialize(_mazeDataService, _character);
        _timeTracker = TimeTracker.Instance;

        _uiManager.OnStartGameplay += StartGameplay;
    }

    private void OnDestroy()
    {
        _uiManager.OnStartGameplay -= StartGameplay;
        _uiManager.ResultPopup.OnClose -= ShowMainMenu;
        _character.OnMazePassed -= EndGameplay;
    }

    private void InitCamera()
    {
        _camera.Follow = _character.transform;
        _camera.LookAt = _character.transform;
    }

    private void StartGameplay()
    {
        _character.OnMazePassed += EndGameplay;
        _mazeGenerator.GenerateMaze();

        _timeTracker.ResetTimer();
        _timeTracker.StartTimer();
        StepCounter.ResetSteps();
    }

    private void EndGameplay()
    {
        var currentTime = _timeTracker.ElapsedTime;
        var currentStepCount = StepCounter.CurrentSteps;

        var resultPopup = _uiManager.ResultPopup;
        resultPopup.OnClose += ShowMainMenu;
        resultPopup.Show();
        resultPopup.Draw(currentTime, currentStepCount);

        CheckAndUpdateBestRecords(currentTime, currentStepCount);

        _character.OnMazePassed -= EndGameplay;
    }

    private void ShowMainMenu()
    {
        _uiManager.ShowWindow(WindowType.MainMenu);
        _uiManager.ResultPopup.OnClose -= ShowMainMenu;
    }

    private void CheckAndUpdateBestRecords(float currentTime, int currentStepCount)
    {
        if (currentTime < StorageService.LoadData(StorageConstants.BEST_TIME, 0f))
        {
            StorageService.SaveData(StorageConstants.BEST_TIME, currentTime);
        }

        if (currentStepCount < StorageService.LoadData(StorageConstants.BEST_STEP_COUNT, 0))
        {
            StorageService.SaveData(StorageConstants.BEST_STEP_COUNT, currentStepCount);
        }
    }
}