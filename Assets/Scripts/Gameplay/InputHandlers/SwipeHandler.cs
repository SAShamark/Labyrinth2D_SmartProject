using System;
using UnityEngine;

namespace Gameplay.InputHandlers
{
    [Serializable]
    public class SwipeHandler
    {
        [SerializeField] private float _minSwipeDistance = 50f;
        [SerializeField] private float _maxSwipeTime = 1f;

        private Vector2 _swipeStartPosition;
        private float _swipeStartTime;
        private bool _isSwipeStarted;

        public event Action<Vector2Int> OnSwiped;

        public void HandleSwipeInput()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                HandleSwipeTouch(touch.position, touch.phase);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                StartSwipe(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0) && _isSwipeStarted)
            {
                EndSwipe(Input.mousePosition);
            }
        }

        private void HandleSwipeTouch(Vector2 touchPos, TouchPhase phase)
        {
            switch (phase)
            {
                case TouchPhase.Began:
                    StartSwipe(touchPos);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (_isSwipeStarted)
                        EndSwipe(touchPos);
                    break;
            }
        }

        private void StartSwipe(Vector2 position)
        {
            _swipeStartPosition = position;
            _swipeStartTime = Time.time;
            _isSwipeStarted = true;
        }

        private void EndSwipe(Vector2 position)
        {
            if (!_isSwipeStarted) return;

            var swipeTime = Time.time - _swipeStartTime;
            var swipeDistance = Vector2.Distance(position, _swipeStartPosition);

            _isSwipeStarted = false;

            if (swipeTime <= _maxSwipeTime && swipeDistance >= _minSwipeDistance)
            {
                var swipeDirection = (position - _swipeStartPosition).normalized;
                var moveDirection = GetMoveDirectionFromSwipe(swipeDirection);

                if (moveDirection != Vector2Int.zero)
                {
                    OnSwiped?.Invoke(moveDirection);
                }
            }
        }

        private Vector2Int GetMoveDirectionFromSwipe(Vector2 swipeDirection)
        {
            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
            {
                return swipeDirection.x > 0 ? Vector2Int.up : Vector2Int.down;
            }
            else
            {
                return swipeDirection.y > 0 ? Vector2Int.right : Vector2Int.left;
            }
        }
    }
}