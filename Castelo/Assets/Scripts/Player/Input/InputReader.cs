using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerInput
{
    [CreateAssetMenu(menuName = "InputReader")]
    public class InputReader : UnityEngine.ScriptableObject, PlayerInput.IMouvementActions
    {
        private PlayerInput _playerInput;
        
        public event Action<Vector2> moveEvent;
        
        public event Action JumpEvent;
        public event Action JumpCancelledEvent;

        private void OnEnable()
        {
            if (_playerInput == null)
            {
                _playerInput = new PlayerInput();
                _playerInput.Mouvement.SetCallbacks(this);
                
                SetGameplay();
            }
        }

        public void SetGameplay()
        {
            _playerInput.Mouvement.Enable();
        }
        
        public void SetPauseMenu()
        {
            _playerInput.Mouvement.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                JumpEvent?.Invoke();
            }
            if (context.phase == InputActionPhase.Canceled)
            {
                JumpCancelledEvent?.Invoke();
            }
        }
    }
}
