using System;
using UnityEngine;

namespace Gameplay.InputHandlers
{
    public class ClickHandler
    {
        private readonly Camera _mainCamera;

        public event Action<Vector3> OnClicked;

        public ClickHandler(Camera mainCamera)
        {
            _mainCamera = mainCamera;
        }
        public void HandleClickInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0;
                OnClicked?.Invoke(mouseWorldPos);
            }
        }
    }
}