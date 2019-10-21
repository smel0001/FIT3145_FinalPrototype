using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float gravity = -20f;
    public float speed = 8f;
    public float groundDamping = 20f;
    public float inAirDamping = 6f;
    public float deceleration = 40f;

    private Vector2 _velocity;
    private PlayerController _controller;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private bool _facingRight;

    public Ability CurAbility;

    //Jumping Numbers
    private enum JumpState
    {
        Ascend,
        Descend,
        Wait
    }
    private JumpState _jumpState;
    public float jumpForce = 40f;
    public float initialJumpForce = 5.0f;
    public float maxJumpTime = 0.2f;
    private float currentJumpTime = 0.0f;

    void Awake()
    {
        _controller = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _jumpState = JumpState.Wait;
    }

    void Update()
    {
        //Perform Normal Movement
        Jump();
        HorizontalMovement();
        _controller.ApplyGravity(gravity); //Note this should probably be in here not PlayerController

        //Activate Ability
        CurAbility.Activate(_controller);

        //Perform Final Movement 
        _controller.Move();

        //Animate
        Animate();
    }

    void Jump()
    {
        switch(_jumpState)
        {
            case JumpState.Wait:
                if (InputController.Instance.Jump.Down)
                {
                    if (_controller.grounded)
                    {
                        currentJumpTime = 0.0f;
                        _controller.SetVerticalVelocity(initialJumpForce);
                        _jumpState = JumpState.Ascend;
                    }
                }
                break;
            case JumpState.Ascend:
                if (currentJumpTime < maxJumpTime)
                {
                    if (InputController.Instance.Jump.Held)
                    {
                        currentJumpTime += Time.deltaTime;
                        _controller.AddVerticalForce(jumpForce * (maxJumpTime - currentJumpTime) * 10);
                    }
                    else
                    {
                        _jumpState = JumpState.Descend;
                    }
                }
                else
                {
                    _jumpState = JumpState.Descend;
                }

                break;
            case JumpState.Descend:
                if (_controller.grounded)
                {
                    _jumpState = JumpState.Wait;
                }
                break;

        }
    }

    void HorizontalMovement()
    {
        var smoothedMovementFactor = _controller.grounded ? groundDamping : inAirDamping;
        //Needs to have decay
        if (InputController.Instance.Horizontal.Value != 0f)
        {
            _controller.SetHorizontalVelocity(Mathf.Lerp(_controller.velocity.x, InputController.Instance.Horizontal.Value * speed, Time.deltaTime * smoothedMovementFactor));
        }
        else
        {
            _controller.HorizontalDecelerate(deceleration);
        }
    }

    public void SetCurrentAbility(Ability incAbility)
    {
        CurAbility.ExitAbility();
        CurAbility = incAbility;
        CurAbility.EnterAbility();
    }

    void Animate()
    {
        if (InputController.Instance.Horizontal.Value < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (InputController.Instance.Horizontal.Value > 0)
        {
            _spriteRenderer.flipX = false;
        }

        if (_controller.velocity.y > 0)
        {
            _animator.Play(Animator.StringToHash("CharUp"));
        }
        else if (_controller.velocity.y < 0)
        {
            _animator.Play(Animator.StringToHash("CharDown"));
        }
        else if (InputController.Instance.Horizontal.Value != 0)
        {
            _animator.Play(Animator.StringToHash("CharRun"));
        }
        else
        {
            _animator.Play(Animator.StringToHash("CharIdle"));
        }
    }
}
