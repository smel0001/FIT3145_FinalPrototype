using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACloud : MonoBehaviour, Ability
{ 

    private CloudState _cloudState;
    public float cloudCD = 1f;
    public float cloudTimer = 3f;

    public GameObject CloudPrefab;
    private SpriteRenderer cloudRenderer;
    private BoxCollider2D cloudCollider;

    private float _timer;

    private bool _groundReset;

    private enum CloudState
    {
        Ready,
        Active,
        Cooldown
    }

    void Awake()
    {
        cloudRenderer = CloudPrefab.GetComponent<SpriteRenderer>();
        cloudCollider = CloudPrefab.GetComponent<BoxCollider2D>();

        cloudRenderer.enabled = false;
        cloudCollider.enabled = false;

        _timer = cloudTimer;

        _cloudState = CloudState.Ready;
    }

    public void EnterAbility()
    {
    }

    public void ExitAbility()
    {  
        if (_cloudState == CloudState.Ready)
        {
            cloudRenderer.enabled = false;
        }
    }

    public void Activate(PlayerController player)
    {

        switch(_cloudState)
        {
            case CloudState.Ready:

                //get mouse position
                Vector3 mousePos2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos2D.z = 0;
                //set cloud position to mouse
                CloudPrefab.transform.position = mousePos2D;

                cloudRenderer.enabled = true;
                cloudRenderer.color = new Color(1, 1, 1, 0.5f);

                if (InputController.Instance.Ability.Down)
                {
                    
                    //change state to active
                    _cloudState = CloudState.Active;
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

        switch (_cloudState)
        {
            case CloudState.Active:

                cloudCollider.enabled = true;
                cloudRenderer.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, (_timer / 3.0f)));

                if (_timer > 0)
                {
                    _timer -= Time.deltaTime;

                }
                else
                {
                    _timer = cloudCD;
                    _cloudState = CloudState.Cooldown;
                }
                break;
            case CloudState.Cooldown:

                cloudRenderer.enabled = false;
                cloudCollider.enabled = false;

                if (_timer > 0)
                {
                    _timer -= Time.deltaTime;
                }
                else
                {
                    _timer = cloudTimer;
                    _cloudState = CloudState.Ready;
                }
                break;
        }
    }

    public void DeathReset()
    {
        cloudRenderer.enabled = false;
        cloudCollider.enabled = false;
        _timer = cloudTimer;
        _cloudState = CloudState.Ready;
    }

}
