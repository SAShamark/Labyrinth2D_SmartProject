

using System;
using UnityEngine;

namespace UI
{
    public abstract class BaseWindow : MonoBehaviour
    {
        [SerializeField] protected Canvas _canvas;
        
        public virtual void Show()
        {
            _canvas.enabled = true;
        }

        public virtual void Hide()
        {
            _canvas.enabled = false;
        }
    }
}