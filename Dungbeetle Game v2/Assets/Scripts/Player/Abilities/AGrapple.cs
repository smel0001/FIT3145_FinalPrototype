using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AGrapple : MonoBehaviour, Ability
{
    // An enumerator for the types of states the grapple can have
    private enum GrappleState
    {
        Ready,
        Firing,
        Connected,
        Cooldown
    }

    private enum Wrapping
    {
        Wrap,
        UnWrap,
        Float
    }

    //public
    public float maxGrappleDistance = 10.0f;
    public float SwingSpeed = 20.0f;
    public float GrappleCooldown = 0.2f;
    public float ThrowTime = 0.2f;
    public Sprite GrappleSprite;

    private GrappleState _grappleState;

    private bool _groundReset;

    //Rays
    private RaycastHit2D _rayCastGrapple;
    private bool _grappleSuccess;

    private float _swingTime;
    private float _swingDir;

    //Wrapping State
    private bool _wrapFlip;
    private Wrapping _wrap;

    //Wrapping Points + Radii
    private Vector3 _grapplePoint;
    private float _radius;
    private List<Vector3> _pivotPoints;
    private List<float> _radii;

    //Grapple Indicator
    public GameObject Indicator;
    private SpriteRenderer IndicatorRenderer;

    // Grapple Render
    private GameObject grappleSprite;
    private SpriteRenderer grappleSpriteRender;
    private Vector3 _grappSpriteMove;
    private LineRenderer _lineRender;

    void Awake()
    {
        _grappleState = GrappleState.Ready;
        _grappleSuccess = false;

        _swingTime = ThrowTime;

        _wrap = Wrapping.Wrap;

        _pivotPoints = new List<Vector3>();
        _radii = new List<float>();

        Indicator = Instantiate(Indicator, transform.position, transform.rotation);
        IndicatorRenderer = Indicator.GetComponent<SpriteRenderer>();

        grappleSprite = new GameObject("GrappleSprite");
        grappleSpriteRender = grappleSprite.AddComponent<SpriteRenderer>();
        grappleSpriteRender.sprite = GrappleSprite;
        grappleSpriteRender.enabled = false;

        _lineRender = gameObject.AddComponent<LineRenderer>();
        _lineRender.startColor = Color.white;
        _lineRender.endColor = Color.green;
        _lineRender.startWidth = 0.1f;
        _lineRender.endWidth = 0.1f;
        _lineRender.enabled = false;
    }

    public void EnterAbility()
    {
        _grappleState = GrappleState.Ready;
        _swingTime = ThrowTime;
        _grappleSuccess = false;
        IndicatorRenderer.enabled = true;
    }

    public void ExitAbility()
    {
        _pivotPoints.Clear();
        _radii.Clear();
        _lineRender.positionCount = 0;
        _lineRender.enabled = false;
        grappleSpriteRender.enabled = false;

        _grappleState = GrappleState.Ready;
        IndicatorRenderer.enabled = false;
    }

    public void Activate(PlayerController player)
    {
        //Debug.DrawRay(player.transform.position, MousePosition(player).normalized * maxGrappleDistance, Color.red);
        Indicator.transform.position = player.transform.position;
        switch (_grappleState)
        {
            case GrappleState.Ready:

                //Ray
                Vector3 mousePos = new Vector3(InputController.Instance.Mouse.mouseX, InputController.Instance.Mouse.mouseY, 0f);
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                mousePos.x = mousePos.x - player.transform.position.x;
                mousePos.y = mousePos.y - player.transform.position.y;

                float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
                Indicator.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                


                _rayCastGrapple = Physics2D.Raycast(player.transform.position, MousePosition(player).normalized, maxGrappleDistance, LayerMask.GetMask("Default"));
                if (_rayCastGrapple)
                {
                    //Render sprite at X
                    grappleSpriteRender.enabled = true;
                    grappleSprite.transform.position = _rayCastGrapple.point;

                    //Draw sprite in?
                }
                else
                {
                    grappleSpriteRender.enabled = false;
                }



                if (InputController.Instance.Ability.Down)
                {
                    IndicatorRenderer.enabled = false;
                    _rayCastGrapple = Physics2D.Raycast(player.transform.position, MousePosition(player).normalized, maxGrappleDistance, LayerMask.GetMask("Default"));
                    if (_rayCastGrapple)
                    {
                        _grapplePoint = _rayCastGrapple.point;
                        _grappleSuccess = true;

                        //set radius on connection
                        _radius = Vector3.Distance(player.transform.position, _grapplePoint);

                        if (_rayCastGrapple.transform.CompareTag("Respawn"))
                        {
                            _grappleSuccess = false;
                        }
                    }
                    else
                    {
                        _radius = maxGrappleDistance;
                        _grapplePoint = MousePosition(player).normalized * maxGrappleDistance;
                        _grapplePoint += player.transform.position;
                        _grappleSuccess = false;
                    }


                    if (_grapplePoint.x >= player.transform.position.x)
                    {
                        _swingDir = 1f;
                    }
                    else
                    {
                        _swingDir = -1f;
                    }

                    grappleSprite.transform.position = player.transform.position;
                    grappleSpriteRender.enabled = true;
                    _grappSpriteMove = (_grapplePoint - player.transform.position).normalized * _radius / ThrowTime;

                    //linerender temp and jank
                    _lineRender.enabled = true;
                    _lineRender.positionCount = 2;
                    _lineRender.SetPosition(0, grappleSprite.transform.position);
                    _lineRender.SetPosition(1, player.transform.position);

                    _grappleState = GrappleState.Firing;
                }
                break;

            case GrappleState.Firing:

                // Simulate the movement of the grapple heading towards the fire direction
                if (_swingTime > 0)
                {
                    _swingTime -= Time.deltaTime;

                    //move sprite
                    grappleSprite.transform.position += _grappSpriteMove * Time.deltaTime;

                    //lineRender temp and jank
                    _lineRender.SetPosition(0, grappleSprite.transform.position);
                    _lineRender.SetPosition(1, player.transform.position);

                }
                else
                {
                    //temp line render cleanup
                    _lineRender.positionCount = 0;


                    _wrap = Wrapping.Wrap;
                    //adjust for y
                    if (_grapplePoint.y <= player.transform.position.y)
                    {
                        _swingDir *= -1;
                        _wrap = Wrapping.Float;
                    }

                    //Set Line Points
                    _pivotPoints.Add(_grapplePoint);
                    _pivotPoints.Add(player.transform.position);
                    SetLinePos();

                    _radius = Vector3.Distance(player.transform.position, _grapplePoint);
                    _radii.Add(_radius);

                    grappleSpriteRender.enabled = false;

                    if (player.grounded)
                    {
                        _grappleSuccess = false;
                    }

                    if (_grappleSuccess)
                    {
                        _lineRender.enabled = true;
                        grappleSpriteRender.enabled = true;
                    }
                    
                    _swingTime = GrappleCooldown;
                    
                    _grappleState = GrappleState.Connected;
                }

                break;

            case GrappleState.Connected:

                // Player travels along ray direction to object if raycast successfully hits
                if (_grappleSuccess)
                {   
                    //swing one way and then the other
                    switch(_wrap)
                    {
                        case Wrapping.Wrap:
                        
                            player.SetVelocity(Swing(player) * SwingSpeed);

                            //wrap
                            RaycastHit2D pivotHit = Physics2D.Linecast(player.transform.position, _grapplePoint, LayerMask.GetMask("Default"));
                            if(pivotHit)
                            {
                                if (new Vector3(pivotHit.point.x, pivotHit.point.y, 0f) != _grapplePoint)
                                {
                                    _grapplePoint = pivotHit.point;
                                    _radius = Vector3.Distance(player.transform.position, _grapplePoint);
                                    _radii.Add(_radius);
                                    _pivotPoints.Insert(_pivotPoints.Count - 1, pivotHit.point);
                                    SetLinePos();
                                }
                            }   

                            //always exits to float
                            
                            if (player.transform.position.y > _grapplePoint.y)
                            {
                                _wrap = Wrapping.Float;
                            }
                            break;

                        case Wrapping.UnWrap:

                            player.SetVelocity(Swing(player) * SwingSpeed);

                            //if moving left
                            if (_swingDir < 0)
                            {
                                if (player.transform.position.y < _pivotPoints[_pivotPoints.Count - 2].y && _pivotPoints.Count > 2)
                                {
                                    //drop point
                                    _pivotPoints.RemoveAt(_pivotPoints.Count - 2);
                                    _grapplePoint = _pivotPoints[_pivotPoints.Count - 2];
                                    _radii.RemoveAt(_radii.Count - 1);
                                    _radius = _radii[_radii.Count - 1];

                                    SetLinePos();
                                }
                               
                                if (player.transform.position.x < _pivotPoints[0].x)
                                {
                                    _wrap = Wrapping.Wrap;
                                }

                            }
                            else if (_swingDir > 0)
                            {
                                if (player.transform.position.y < _pivotPoints[_pivotPoints.Count - 2].y && _pivotPoints.Count > 2)
                                {
                                    //drop point
                                    _pivotPoints.RemoveAt(_pivotPoints.Count - 2);
                                    _grapplePoint = _pivotPoints[_pivotPoints.Count - 2];
                                    _radii.RemoveAt(_radii.Count - 1);
                                    _radius = _radii[_radii.Count - 1];

                                    SetLinePos();
                                }

                                if (player.transform.position.x > _pivotPoints[0].x)
                                {
                                    _wrap = Wrapping.Wrap;
                                }
                            }

                            break;
                            
                        case Wrapping.Float:

                            //Clamp to walls
                            float wallmax = _grapplePoint.x + _radius;
                            float wallmin = _grapplePoint.x - _radius;
                            Vector2 move = player.velocity * Time.deltaTime;
                            if (player.transform.position.x + move.x < wallmin || player.transform.position.x + move.x > wallmax)
                            {
                                player.SetHorizontalVelocity(0);
                            }

                            //Passing over top pivot cuts line
                            //if coming from right
                            if (_swingDir > 0)
                            {
                                if (player.transform.position.x < _pivotPoints[0].x)
                                {
                                    //break
                                    _lineRender.enabled = false;
                                    grappleSpriteRender.enabled = false;
                                    _grappleState = GrappleState.Cooldown;
                                }
                            }
                            else
                            {
                                if (player.transform.position.x > _pivotPoints[0].x)
                                {
                                    //break
                                    _lineRender.enabled = false;
                                    grappleSpriteRender.enabled = false;
                                    _grappleState = GrappleState.Cooldown;
                                }
                            }


                            //always exits to unwrap
                            if (player.transform.position.y < _grapplePoint.y)
                            {
                                _swingDir *= -1;
                                _wrap = Wrapping.UnWrap;
                            }
                            break;
                    }

                    _lineRender.SetPosition(_pivotPoints.Count-1, player.transform.position);

                    if (InputController.Instance.Jump.Down || InputController.Instance.Ability.Down || player.grounded || player.sidecollide || player.ceiling)
                    {
                        _lineRender.enabled = false;
                        grappleSpriteRender.enabled = false;
                        _grappleState = GrappleState.Cooldown;
                    }
                    
                }
                else
                {
                    _pivotPoints.Clear();
                    _radii.Clear();

                    _lineRender.enabled = false;
                    _swingTime = ThrowTime;
                    IndicatorRenderer.enabled = true;
                    _grappleState = GrappleState.Ready;
                }
                break;

            case GrappleState.Cooldown:
                if (_swingTime > 0)
                {
                    _swingTime -= Time.deltaTime;
                }
                else
                {
                    _pivotPoints.Clear();
                    _radii.Clear();

                    _swingTime = ThrowTime;
                    IndicatorRenderer.enabled = true;
                    _grappleState = GrappleState.Ready;
                }
                break;
        }
    }

    public void GroundCheck(PlayerController controller)
    {
        if (controller.grounded)
        {
            _groundReset = true;
        }
    }


    private Vector3 Swing(PlayerController player)
    {
        //Perpendicular to Attached Point
        Vector2 perpendicular = new Vector2(_grapplePoint.x, _grapplePoint.y) - new Vector2(player.transform.position.x, player.transform.position.y);
        Vector2 move = (Vector2.Perpendicular(perpendicular) * -_swingDir).normalized;
        //Debug.DrawRay(player.transform.position, new Vector3(move.x, move.y, 0f), Color.cyan);

        //Adjust for Drift (i.e. pull player in to fixed distance)
        //Calculate Direction back to Pivot Point
        Vector3 distAdjust = _grapplePoint - (new Vector3(move.x, move.y, 0f) + player.transform.position);
        //Debug.DrawRay(new Vector3(move.x, move.y, 0f) + player.transform.position, distAdjust.normalized, Color.green);
                        
        //Calculate Actual Point
        Vector3 pointToActual = -distAdjust.normalized * _radius;
        //Debug.DrawRay(_grapplePoint, pointToActual.normalized, Color.yellow);
        
        //Direction from Player to Actual Point
        Vector3 actualMove = pointToActual + _grapplePoint - player.transform.position ;
        //Debug.DrawRay(player.transform.position, actualMove * SwingSpeed, Color.white);

        return actualMove;
    }

    void SetLinePos()
    {
        _lineRender.positionCount = _pivotPoints.Count;
        _lineRender.SetPositions(_pivotPoints.ToArray());
    }

    private Vector2 MousePosition(PlayerController controller)
    {
        Vector3 mouse = new Vector3(InputController.Instance.Mouse.mouseX, InputController.Instance.Mouse.mouseY, 0f);
        mouse = Camera.main.ScreenToWorldPoint(mouse);
        mouse.z = 0f;

        Vector3 dir = (mouse - controller.transform.position);

        return dir;
    }


    public void DeathReset()
    {}
    
}
