using Cinemachine;
using Gameplay.Character;
using Gameplay.Environments;
using Services;
using UI;
using UnityEngine;

public class StartApplication : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private MazeGenerator _mazeGenerator;
    [SerializeField] private CinemachineVirtualCamera _camera;
    [SerializeField] private MazeConfig _mazeConfig;

    [SerializeField] private CharacterControl _characterControl;
    [SerializeField] private Transform _characterContainer;

    private CharacterControl _character;
    private GameStateController _gameStateController;

    private void Start()
    {
        _character = Instantiate(_characterControl, _characterContainer);
        InitCamera();

        var mazeDataService = new MazeDataService(_mazeConfig);
        var timeTracker = TimeTracker.Instance;

        _mazeGenerator.Initialize(mazeDataService, _character);
        _uiManager.Initialize(mazeDataService);
        _uiManager.ShowWindow(WindowType.MainMenu);

        _gameStateController = new GameStateController(_character, _mazeGenerator, timeTracker, _uiManager);
        Subscribes();
    }

    private void OnDestroy()
    {
        Unsubscribes();
    }

    private void Subscribes()
    {
        _uiManager.OnStartGameplay += _gameStateController.StartGameplay;
    }

    private void Unsubscribes()
    {
        _uiManager.OnStartGameplay -= _gameStateController.StartGameplay;
        _uiManager.ResultPopup.OnClose -= _gameStateController.ShowMainMenu;
        _character.OnMazePassed -= _gameStateController.EndGameplay;
    }

    private void InitCamera()
    {
        _camera.Follow = _character.transform;
        _camera.LookAt = _character.transform;
    }
}