using System;
using PlayerInput;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMouvement : MonoBehaviour
    {
        [SerializeField] private InputReader input;
        
        [SerializeField] private float speed;
        [SerializeField] private float jumpSpeed;

        private Vector2 _moveDirection;
        private bool _isJumping;
        private Rigidbody _rigidbody;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            input.moveEvent += HandleMove;
            input.JumpEvent += HandleJump;
            input.JumpCancelledEvent += HandleCancelledJump;
        }

        private void Update()
        {
            Move();
            Jump();
        }

        private void HandleCancelledJump()
        {
            _isJumping = false;
        }

        private void HandleJump()
        {
            _isJumping = true;
        }

        private void HandleMove(Vector2 dir)
        {
            _moveDirection = dir;
        }

        private void Move()
        {
            Vector3 vel = _rigidbody.velocity;
            Vector3 newVel = new Vector3(_moveDirection.x, 0f, _moveDirection.y) * speed * Time.deltaTime;
            newVel.y = vel.y;
            _rigidbody.velocity = newVel;
        }

        private void Jump()
        {
            if (_isJumping)
            {
                _rigidbody.AddForce(Vector2.up * jumpSpeed * Time.deltaTime, ForceMode.Impulse);
            }
        }
    }
}
