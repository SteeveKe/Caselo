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
        [SerializeField] private float walkSpeed;
        [SerializeField] private float sprintSpeed;
        [SerializeField] private float speed;
        [SerializeField] private float acceleration;
        [SerializeField] private float decceleration;
        [SerializeField] private float jumpForce;
        [SerializeField] private float jumpCooldown;
        [SerializeField] private float airMultiplier;
        [SerializeField] private float airSideMultiplier;
        //[SerializeField] private float groundDrag;
        
        [Header("Ground Check")]
        [SerializeField] private float playerHeight;
        [SerializeField] private LayerMask whatIsGround;
        private bool _grounded = false;

        private MeshFilter _meshFilter; 
            
        private Vector2 _moveDirection;
        private bool _jumpPressed;
        private bool _sprintPressed;
        private Rigidbody _rigidbody;
        private bool _readyToJump = true;

        public MovementState state;
        
        public enum MovementState
        {
            Walking,
            Sprinting,
            Air
        }
        
        private void HandleCancelledSprint()
        {
            _sprintPressed = false;
        }

        private void HandleSprint()
        {
            _sprintPressed = true;
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

        void Start()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _rigidbody = GetComponent<Rigidbody>();
            playerHeight = _meshFilter.mesh.bounds.size.y;
            input.MoveEvent += HandleMove;
            input.JumpEvent += HandleJump;
            input.JumpCancelledEvent += HandleCancelledJump;
            input.SprintEvent += HandleSprint;
            input.SprintCancelledEvent += HandleCancelledSprint;
        }

        private void Update()
        {
            StateHandler();
            GetDrag();
            SpeedControl();
        }

        private void FixedUpdate()
        {
            Move();
            JumpTest();
        }

        private void JumpTest()
        {
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
        }
        
        

        private void Move()
        {
            Vector3 currentVel = _rigidbody.velocity;
            currentVel.y = 0f;

            Vector3 dirVel;
            if (state != MovementState.Air)
            {
                if (_moveDirection.magnitude == 0)
                {
                    dirVel = currentVel;
                }
                else
                {
                    dirVel = currentVel.magnitude * (transform.forward * _moveDirection.y + transform.right * _moveDirection.x).normalized;
                }
                _rigidbody.velocity = new Vector3(dirVel.x, _rigidbody.velocity.y, dirVel.z);
            }
            else
            {
                dirVel = currentVel.magnitude * (transform.forward * _moveDirection.y + transform.right * _moveDirection.x * airSideMultiplier).normalized;
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y, dirVel.z);
            }
            
            float targetSpeed = _moveDirection.normalized.magnitude * speed;
            float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : decceleration;
            
            float speedDif = targetSpeed - currentVel.magnitude;
            float movement = speedDif * accelRate;
            
            Vector3 newVel;
            if (_moveDirection.magnitude == 0)
            {
                newVel = currentVel.normalized;
            }
            else
            {
                newVel = (transform.forward * _moveDirection.y + transform.right * _moveDirection.x).normalized;
            }

            if (_grounded)
            {
                _rigidbody.AddForce(movement * newVel, ForceMode.Force);
            }
            else
            {
                _rigidbody.AddForce(movement * newVel * airMultiplier, ForceMode.Force);
            }
        }

        private void StateHandler()
        {
            if (_grounded && _sprintPressed)
            {
                state = MovementState.Sprinting;
                speed = sprintSpeed;
            }
            else if (_grounded)
            {
                state = MovementState.Walking;
                speed = walkSpeed;
            }
            else
            {
                state = MovementState.Air;
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
