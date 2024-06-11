using System;
using PlayerInput;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(MeshFilter))]
    public class PlayerMouvement : MonoBehaviour
    {
        [SerializeField] private InputReader input;
        
        [Header("Movement")]
        [SerializeField] private float speed;
        [SerializeField] private float jumpForce;
        [SerializeField] private float jumpCooldown;
        [SerializeField] private float airMultiplier;
        [SerializeField] private float groundDrag;
        
        [Header("Ground Check")]
        [SerializeField] private float playerHeight;
        [SerializeField] private LayerMask whatIsGround;
        private bool _grounded = false;

        private MeshFilter _meshFilter; 
            
        private Vector2 _moveDirection;
        private bool _jumpPressed;
        private Rigidbody _rigidbody;
        private bool _readyToJump = true;

        void Start()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _rigidbody = GetComponent<Rigidbody>();
            playerHeight = _meshFilter.mesh.bounds.size.y;
            input.moveEvent += HandleMove;
            input.JumpEvent += HandleJump;
            input.JumpCancelledEvent += HandleCancelledJump;
        }

        private void Update()
        {
            GetDrag();
            SpeedControl();
        }

        private void FixedUpdate()
        {
            Move();

            if (_jumpPressed && _readyToJump && _grounded)
            {
                _readyToJump = false;
                Jump();
                
                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }

        private void GetDrag()
        {
            _grounded = Physics.Raycast(transform.position, Vector3.down, 
                playerHeight * 0.5f + 0.2f, whatIsGround);
            if (_grounded)
            {
                _rigidbody.drag = groundDrag;
            }
            else
            {
                _rigidbody.drag = 0f;
            }
        }

        private void HandleCancelledJump()
        {
            _jumpPressed = false;
        }

        private void HandleJump()
        {
            _jumpPressed = true;
        }

        private void HandleMove(Vector2 dir)
        {
            _moveDirection = dir;
        }

        private void Move()
        {
            Vector3 newVel = (transform.forward * _moveDirection.y + transform.right * _moveDirection.x).normalized;
            newVel *= speed * Time.deltaTime;

            if (_grounded)
            {
                _rigidbody.AddForce(newVel, ForceMode.Force);
            }
            else
            {
                _rigidbody.AddForce(newVel * airMultiplier, ForceMode.Force);
            }
            
        }

        private void SpeedControl()
        {
            Vector3 vel = _rigidbody.velocity;
            vel.y = 0f;

            if (vel.magnitude > speed)
            {
                vel = vel.normalized * speed;
                _rigidbody.velocity = new Vector3(vel.x, _rigidbody.velocity.y, vel.z);
            }
        }

        private void Jump()
        {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
            
            _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
        }

        private void ResetJump()
        {
            _readyToJump = true;
        }
    }
}
