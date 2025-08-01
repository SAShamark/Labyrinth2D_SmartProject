using System;
using System.Collections;
using Gameplay.Environments;
using Gameplay.InputHandlers;
using Services;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gameplay.Character
{
    public class CharacterControl : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private bool _smoothMovement = true;

        [SerializeField] private bool _enableWasd = true;
        [SerializeField] private bool _enableCellClick = true;
        [SerializeField] private bool _enableSwipe = true;
        [SerializeField] private ParticleSystem _winParticle;

        [SerializeField] private SwipeHandler _swipeHandler;
        
        private ClickHandler _clickHandler;
        private WasdHandler _wasdHandler;
        private Camera _mainCamera;

        private CellType[,] _maze;
        private int _mazeWidth = 21;
        private int _mazeHeight = 21;
        private Tilemap _groundTilemap;
        private bool _isMoving;
        private Vector2Int _currentMazePosition;
        private Vector3 _targetWorldPosition;
        private bool _inputEnabled = true;
        private Coroutine _mazePassedCoroutine;

        public event Action OnMazePassed;

        private void Start()
        {
            _mainCamera = Camera.main;
            _clickHandler = new ClickHandler(_mainCamera);
            _wasdHandler = new WasdHandler();
        }
        
        private void Update()
        {
            if (!_inputEnabled) return;
            if (!_isMoving)
            {
                if (_enableWasd) _wasdHandler.HandleWasdInput();
                if (_enableCellClick) _clickHandler.HandleClickInput();
                if (_enableSwipe) _swipeHandler.HandleSwipeInput();
            }

            if (_smoothMovement && _isMoving)
            {
                MoveTowardsTarget();
            }
        }

        private void OnDestroy()
        {
            UnsubscribeInput();

            if (_mazePassedCoroutine != null)
            {
                StopCoroutine(_mazePassedCoroutine);
            }
        }

        public void Initialize(CellType[,] maze, int mazeWidth, int mazeHeight, Tilemap groundTilemap)
        {
            _maze = maze;
            _mazeWidth = mazeWidth;
            _mazeHeight = mazeHeight;
            _groundTilemap = groundTilemap;

            _currentMazePosition = WorldToMazePosition(transform.position);
            _targetWorldPosition = transform.position;
            _inputEnabled = true;

            _swipeHandler.OnSwiped += TryMove;
            _clickHandler.OnClicked += TryMoveByClick;
            _wasdHandler.OnPressed += TryMove;
        }

        private void TryMove(Vector2Int direction)
        {
            var targetPosition = _currentMazePosition + direction;

            if (IsWalkable(targetPosition))
            {
                _currentMazePosition = targetPosition;
                _targetWorldPosition = MazeToWorldPosition(_currentMazePosition);

                if (_smoothMovement)
                {
                    _isMoving = true;
                }
                else
                {
                    transform.position = _targetWorldPosition;
                }

                CharacterMoved();
            }
        }

        private void TryMoveByClick(Vector3 mouseWorldPos)
        {
            var clickedMazePos = WorldToMazePosition(mouseWorldPos);

            if (IsValidClickTarget(clickedMazePos, _currentMazePosition))
            {
                var direction = clickedMazePos - _currentMazePosition;
                TryMove(direction);
            }
        }

        private bool IsValidClickTarget(Vector2Int targetPos, Vector2Int currentMazePosition)
        {
            if (!IsWalkable(targetPos))
                return false;

            var difference = targetPos - currentMazePosition;
            var manhattanDistance = Mathf.Abs(difference.x) + Mathf.Abs(difference.y);

            return manhattanDistance == 1;
        }

        private bool IsWalkable(Vector2Int position)
        {
            if (position.x < 0 || position.x >= _mazeWidth || position.y < 0 || position.y >= _mazeHeight)
                return false;

            return _maze[position.x, position.y] == CellType.Path || _maze[position.x, position.y] == CellType.Exit;
        }

        private void MoveTowardsTarget()
        {
            transform.position =
                Vector3.MoveTowards(transform.position, _targetWorldPosition, _moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _targetWorldPosition) < 0.01f)
            {
                transform.position = _targetWorldPosition;
                _isMoving = false;
            }
        }

        private Vector3 MazeToWorldPosition(Vector2Int mazePos)
        {
            Vector3Int cellPos = new Vector3Int(mazePos.x, mazePos.y, 0);
            return _groundTilemap.CellToWorld(cellPos) + _groundTilemap.tileAnchor;
        }

        private Vector2Int WorldToMazePosition(Vector3 worldPos)
        {
            Vector3Int cellPos = _groundTilemap.WorldToCell(worldPos);
            return new Vector2Int(cellPos.x, cellPos.y);
        }


        private void CharacterMoved()
        {
            StepCounter.AddStep();

            if (_maze[_currentMazePosition.x, _currentMazePosition.y] == CellType.Exit)
            {
                _inputEnabled = false;
                _mazePassedCoroutine = StartCoroutine(MazePassed());
            }
        }

        private IEnumerator MazePassed()
        {
            UnsubscribeInput();
            TimeTracker.Instance.StopTimer();
            
            _winParticle.Play();
            var duration = _winParticle.main.duration;
            yield return new WaitForSeconds(duration);
            OnMazePassed?.Invoke();
        }
        private void UnsubscribeInput()
        {
            if (_swipeHandler != null)
                _swipeHandler.OnSwiped -= TryMove;

            if (_clickHandler != null)
                _clickHandler.OnClicked -= TryMoveByClick;

            if (_wasdHandler != null)
                _wasdHandler.OnPressed -= TryMove;
        }
    }
}