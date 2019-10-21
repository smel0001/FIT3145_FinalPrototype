using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int numHorizontalRays = 5;
    public int numVerticalRays = 5;
    public float rayOffset = 0.1f;

    private Transform _transform;
    private BoxCollider2D _collider;
    private Vector3 _localScale;

    private Vector3 _RayTopRight;
    private Vector3 _RayBottomRight;
    private Vector3 _RayBottomLeft;
    private Vector3 _RayTopLeft;
    private RaycastHit2D _raycastHit;
    private float _horizontalRayGaps;
    private float _verticalRayGaps;

    public Vector2 velocity;

    public bool grounded;
    public bool ceiling;
    public bool sidecollide;

    public LayerMask PlatformsMask;


    void Awake()
    {
        _transform = transform;
        _collider = GetComponent<BoxCollider2D>();
        _localScale = transform.localScale;
        grounded = false;
        ceiling = false;
        sidecollide = false;

        _horizontalRayGaps = (_collider.size.y * Mathf.Abs(_localScale.y) - rayOffset) / (numHorizontalRays - 1);
        _verticalRayGaps = (_collider.size.x * Mathf.Abs(_localScale.x) - rayOffset) / (numVerticalRays - 1);
    }

    //Expectation is that this is called every frame in player
    public void Move()
    {
        grounded = false;
        ceiling = false;
        sidecollide = false;

        Vector2 deltaVelocity = velocity * Time.deltaTime;

        //Collision Cast
        RayOrigins();
        HorizontalRays(ref deltaVelocity);
        VerticalRays(ref deltaVelocity);

        _transform.Translate(deltaVelocity);

        if (grounded)
            velocity.y = 0;

        if (ceiling)
            if (velocity.y > 0)
                velocity.y = 0;
    }

    #region Rays
    void RayOrigins()
    {
        //no account for offset atm so 0 offset is required
        Vector2 size = new Vector2(_collider.size.x * Mathf.Abs(_localScale.x), _collider.size.y * Mathf.Abs(_localScale.y));

        _RayTopRight = _transform.position + new Vector3(size.x/2, size.y/2, 0f);
        _RayBottomRight = _transform.position + new Vector3(size.x/2, size.y/-2, 0f);
        _RayBottomLeft = _transform.position + new Vector3(size.x/-2, size.y/-2, 0f);
        _RayTopLeft = _transform.position + new Vector3(size.x/-2, size.y/2, 0f);
    }

    
    void HorizontalRays(ref Vector2 deltaVelocity)
    {
        //direction
        bool movingRight = true;
        if (deltaVelocity.x < 0f)
        {
            movingRight = false;
        }

        Vector3 rayOrigin = movingRight ? _RayBottomRight : _RayBottomLeft;
        Vector2 rayDir = movingRight ? Vector2.right : -Vector2.right;
        //temp ray dist needs a buffer?
        float rayDist = Mathf.Abs(deltaVelocity.x);

        rayOrigin.y += rayOffset/2;

        for (int i = 0; i < numHorizontalRays; i++) 
        {
            Vector3 ray = new Vector3 (rayOrigin.x, rayOrigin.y + i * _horizontalRayGaps, 0f);

            Debug.DrawRay(ray, rayDir * rayDist, Color.green);
            _raycastHit = Physics2D.Raycast(ray, rayDir, rayDist, PlatformsMask);
            if (_raycastHit)
            {
                //Shrink delta movement to new dist
                deltaVelocity.x = _raycastHit.point.x - rayOrigin.x;

                sidecollide = true;
            }
        }
    }

    void VerticalRays(ref Vector2 deltaVelocity)
    {
        bool movingUp = true;
        if (deltaVelocity.y < 0f)
        {
            movingUp = false;
        }

        Vector3 rayOrigin = movingUp ? _RayTopRight : _RayBottomRight;
        rayOrigin.x -= rayOffset / 2;
        rayOrigin.x += deltaVelocity.x;
        Vector2 rayDir = movingUp ? Vector2.up : -Vector2.up;
        //temp ray dist needs a buffer?
        float rayDist = Mathf.Abs(deltaVelocity.y);


        for (int i = 0; i < numVerticalRays; i++)
        {
            Vector3 ray = new Vector3(rayOrigin.x - i * _verticalRayGaps, rayOrigin.y, 0f);

            //Debug.DrawRay(ray, rayDir * rayDist, Color.red);
            _raycastHit = Physics2D.Raycast(ray, rayDir, rayDist, PlatformsMask);
            if (_raycastHit)
            {
                //Shrink delta movement to new dist
                deltaVelocity.y = _raycastHit.point.y - rayOrigin.y;

                if (!movingUp)
                {
                    grounded = true;
                }
                else
                {
                    ceiling = true;
                }
            }
        }
    }
    #endregion

    #region VelocityMutators
    public void SetVelocity(Vector2 v)
    {
        velocity = v;
    }

    public void AddForce(Vector2 f)
    {
        velocity += f * Time.deltaTime;
    }

    public void SetHorizontalVelocity(float v)
    {
        velocity.x = v;
    }

    public void AddHorizontalForce(float f)
    {
        velocity.x += f * Time.deltaTime;
    }

    public void SetVerticalVelocity(float v)
    {
        velocity.y = v;
    }

    public void AddVerticalForce(float f)
    {
        velocity.y += f * Time.deltaTime;
    }

    public void ApplyGravity(float f)
    {
        velocity.y += f * Time.deltaTime;
    }

    public void HorizontalDecelerate(float f)
    {
        if (velocity.x > 0f)
        {
            velocity.x -= f * Time.deltaTime;
            if (velocity.x < 0f)
            {
                velocity.x = 0f;
            }
        }
        else if (velocity.x < 0f)
        {
            velocity.x += f * Time.deltaTime;
            if (velocity.x > 0f)
            {
                velocity.x = 0f;
            }
        }
    }
    #endregion

}
