using System;
using UnityEngine;

namespace Gameplay.InputHandlers
{
    internal class WasdHandler
    {
        public event Action<Vector2Int> OnPressed;

        internal void HandleWasdInput()
        {
            var moveDirection = Vector2Int.zero;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                moveDirection = new Vector2Int(1, 0);
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                moveDirection = new Vector2Int(-1, 0);
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                moveDirection = new Vector2Int(0, -1);
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                moveDirection = new Vector2Int(0, 1);

            if (moveDirection != Vector2Int.zero)
            {
                OnPressed?.Invoke(moveDirection);
            }
        }
    }
}