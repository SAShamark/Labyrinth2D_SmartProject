using System;
using Gameplay.Environments;
using UI.Variables;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private MainMenuScreen _mainMenu;
        [SerializeField] private SettingsPopup _settingsPopup;
        [SerializeField] private GameplayScreen _gameplay;
        [SerializeField] private ResultPopup _resultPopup;

        private BaseWindow _currentWindow;

        public MainMenuScreen MainMenu => _mainMenu;
        public SettingsPopup SettingsPopup => _settingsPopup;
        public GameplayScreen Gameplay => _gameplay;
        public ResultPopup ResultPopup => _resultPopup;

        public event Action OnStartGameplay;

        public void Initialize(MazeDataService mazeDataService)
        {
            _settingsPopup.Initialize(mazeDataService);
            ShowWindow(_mainMenu);
            _mainMenu.OnPlay += PlayClicked;
        }

        private void OnDestroy()
        {
            _mainMenu.OnPlay -= PlayClicked;
        }

        private void PlayClicked()
        {
            OnStartGameplay?.Invoke();
            ShowWindow(_gameplay);
        }

        public void ShowWindow(WindowType type)
        {
            BaseWindow window = type switch
            {
                WindowType.MainMenu => _mainMenu,
                WindowType.Gameplay => _gameplay,
                _ => null
            };

            if (window != null)
            {
                ShowWindow(window);
            }
        }

        private void ShowWindow(BaseWindow window)
        {
            _currentWindow?.Hide();
            _currentWindow = window;
            _currentWindow.Show();
        }
    }
}