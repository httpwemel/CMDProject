using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class PlayerCharacter : KinematicObject
    {
        public float maxSpeed = 7;
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        public Collider2D collider2d;
        public int maxHealth;

        bool jumping;
        bool nearObject;
        Vector2 moveDirection;
        public SpriteRenderer spriteRenderer;
        internal Animator animator;

        public Bounds Bounds => collider2d.bounds;

        private PlayerControls playerControls;
        public InputAction jump,  move, interact;

        IInteractable currentObject;

        public ReactiveProperty<int> coins { get; set; }

        public ReactiveProperty<int> health { get; set; }
        public ReactiveProperty<string> status { get; set; }

        public Text healthText, coinsText, statusText;


        void Awake()
        {
            playerControls = new PlayerControls();
            collider2d = GetComponent<Collider2D>();

            health = new ReactiveProperty<int>(maxHealth);
            coins = new ReactiveProperty<int>(0);
            status = new ReactiveProperty<string>("Player Alive");

            coins.SubscribeToText(coinsText);

            health.SubscribeToText(healthText);

            status.SubscribeToText(statusText);
            
            move = playerControls.Player.Move;
            move.Enable();

            jump = playerControls.Player.Jump;
            jump.Enable();
            jump.performed += Jump;

            interact = playerControls.Player.Interact;
            interact.Enable();
            interact.performed += Interact;
        }

        protected override void Update()
        {
            moveDirection = move.ReadValue<Vector2>();
            UpdateJumpState();
            base.Update();
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (jumpState == JumpState.Grounded)
                jumpState = JumpState.PrepareToJump;
            else
            {
                stopJump = true;
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            currentObject = collision.gameObject.GetComponent<IInteractable>();
            if (currentObject != null &&  currentObject is IInteractable) 
            {
                nearObject = true;
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (currentObject != null &&  currentObject is IInteractable) 
            {
                nearObject = false;
                currentObject = null;
            }
        }

        private void Interact(InputAction.CallbackContext context)
        {
            if (nearObject && currentObject != null && currentObject is IInteractable)
            {
                currentObject.OnInteract(this);
            }    
        }

        void UpdateJumpState()
        {
            jumping = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jumping = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jumping && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * 1.5f;
                jumping = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * 0.5f;
                }
            }

            if (moveDirection.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (moveDirection.x < -0.01f)
                spriteRenderer.flipX = true;

            //animator.SetBool("grounded", IsGrounded);
            //animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = moveDirection * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }