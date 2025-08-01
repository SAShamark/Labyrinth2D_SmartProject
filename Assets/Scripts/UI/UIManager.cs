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
        private MazeDataService _mazeDataService;

        public MainMenuScreen MainMenu => _mainMenu;
        public SettingsPopup SettingsPopup => _settingsPopup;
        public GameplayScreen Gameplay => _gameplay;
        public ResultPopup ResultPopup => _resultPopup;
        
        public event Action OnStartGameplay;

        public void Initialzie(MazeDataService mazeDataService)
        {
            _mazeDataService = mazeDataService;
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
            ShowWindow(_gameplay);
            OnStartGameplay?.Invoke();
        }

        public void ShowWindow(WindowType type)
        {
            switch (type)
            {
                case WindowType.MainMenu:
                    ShowWindow(_mainMenu);
                    break;
                case WindowType.Gameplay:
                    ShowWindow(_gameplay);
                    break;
            }
        }

        private void ShowWindow(BaseWindow window)
        {
            if (_currentWindow != null)
            {
                _currentWindow.Hide();
            }

            _currentWindow = window;
            _currentWindow.Show();
        }
    }
}