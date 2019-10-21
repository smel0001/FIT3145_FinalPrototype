using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADash : MonoBehaviour, Ability
{
    public float DashSpeed = 10f;
    public float DashLength = 0.2f;
    public float DashCD = 0.5f;

    private DashState _dashState;
    private float _dashTime;
    private Vector2 _dashDir;
    private float _initHorizontalV = 0f;

    private bool _groundReset;

    public GameObject IndicatorPrefab;
    private SpriteRenderer indicatorRenderer;

    private enum DashState
    {
        Ready,
        Dashing,
        Cooldown
    }

    void Awake()
    {
        indicatorRenderer = IndicatorPrefab.GetComponent<SpriteRenderer>();
        indicatorRenderer.enabled = false;
    }

    public void EnterAbility()
    {
        _dashState = DashState.Ready;
        _dashTime = DashLength;
        _dashDir = Vector2.zero;

        indicatorRenderer.enabled = true;

    }

    public void ExitAbility()
    {
        _dashState = DashState.Ready;
        _dashTime = DashLength;

        indicatorRenderer.enabled = false;
    }

    public void Activate(PlayerController controller)
    {
        //Draw cursor
        //Debug.DrawRay(controller.transform.position, MouseDirCalc(controller) * DashSpeed * DashLength, Color.red);
        switch (_dashState)
        {
            case DashState.Ready:
                if (_groundReset)
                {
                    if (InputController.Instance.Ability.Down)
                    {
                        if (controller.velocity.y < 0f)
                        {
                            controller.SetVerticalVelocity(0f);
                        }
                        _initHorizontalV = controller.velocity.x;
                        _dashDir = MouseDirCalc(controller);
                        _dashState = DashState.Dashing;
                        _groundReset = false;
                    }
                }
                break;

            case DashState.Dashing:
                if (_dashTime > 0)
                {

                    Vector2 temp = _dashDir * DashSpeed;

                    //reset vertical
                    controller.SetVerticalVelocity(temp.y);
                    //preserve horizontal
                    controller.SetHorizontalVelocity(temp.x + _initHorizontalV);

                    _dashTime -= Time.deltaTime;
                }
                else
                {
                    _dashTime = DashCD;
                    _dashState = DashState.Cooldown;
                }
                break;

            case DashState.Cooldown:
                if (_dashTime > 0)
                {
                    _dashTime -= Time.deltaTime;
                }
                else
                {
                    _dashTime = DashLength;
                    _dashState = DashState.Ready;
                }
                break;
        }

        Vector3 mousePos = new Vector3(InputController.Instance.Mouse.mouseX, InputController.Instance.Mouse.mouseY, 0f);
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        mousePos.x = mousePos.x - IndicatorPrefab.transform.position.x;
        mousePos.y = mousePos.y - IndicatorPrefab.transform.position.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        IndicatorPrefab.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        IndicatorPrefab.transform.position = controller.transform.position;
    }

    public void GroundCheck(PlayerController controller)
    {
        if (controller.grounded)
        {
            _groundReset = true;
        }
    }

    private Vector2 MouseDirCalc(PlayerController controller)
    {
        Vector3 mouse = new Vector3(InputController.Instance.Mouse.mouseX, InputController.Instance.Mouse.mouseY, 0f);
        mouse = Camera.main.ScreenToWorldPoint(mouse);
        mouse.z = 0f;

        Vector3 dir = (mouse - controller.transform.position).normalized;
        Vector2 dir2D = new Vector2(dir.x, dir.y);

        return dir2D;
    }

    public void DeathReset()
    {}
}


