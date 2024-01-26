using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private enum PlayerState
    {
        Idle,
        Accelerating,
        Reversing
    }

    private PlayerState _currentState = PlayerState.Idle;

    private float _steer = 0.0f;
    private float _forwardVelocity = 0.0f;
    private float _totalVelocity = 0.0f;

    private bool _acceleratePressed;
    private bool _reversePressed;

    private Rigidbody _rb;

    private GameManager _gameManager;

    [SerializeField] private float steerSpeed = 10f;
    [SerializeField] private float accelerationForce = 10f;
    [SerializeField] private float brakeForce = 10f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float decelerationRate = 1f;
    [SerializeField] private float reverseForce = 10f;
    [SerializeField] private float maxAngularVelocity = 10f;
    [SerializeField] private float bounceFactor = 0.5f;

    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();
        _gameManager = GameManager.Instance;

        _gameManager.OnLoadNextCourse += ResetPlayer;
    }

    private void OnDisable()
    {
        _gameManager.OnLoadNextCourse -= ResetPlayer;
    }

    private void Update()
    {
        _forwardVelocity = Vector3.Dot(_rb.velocity, transform.forward);

        switch (_currentState)
        {
            case PlayerState.Idle:
                Idle();
                break;
            case PlayerState.Accelerating:
                Accelerate();
                break;
            case PlayerState.Reversing:
                Reverse();
                break;
        }

        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, maxSpeed);
        _totalVelocity = _rb.velocity.magnitude;

        if (_totalVelocity < 0.1f)
            _rb.velocity = Vector3.zero;

        if (_steer != 0)
            Steer();
    }

    private void UpdatePlayerState()
    {
        if (_acceleratePressed && _reversePressed)
        {
            _currentState = PlayerState.Idle;
        }
        else if (_acceleratePressed)
        {
            _currentState = PlayerState.Accelerating;
        }
        else if (_reversePressed)
        {
            _currentState = PlayerState.Reversing;
        }
        else
        {
            _currentState = PlayerState.Idle;
        }
    }

    private void Idle()
    {
        if (_forwardVelocity <= 0)
            return;

        Vector3 decelerationForce = -_rb.velocity.normalized * decelerationRate * Time.deltaTime * 100;
        _rb.AddForce(decelerationForce, ForceMode.Acceleration);
    }

    private void Accelerate()
    {
        if (_rb.velocity.magnitude < maxSpeed)
        {
            _rb.AddForce(transform.forward * accelerationForce * Time.deltaTime * 100, ForceMode.Acceleration);
        }
    }

    private void Reverse()
    {
        if (_forwardVelocity > 0)
        {
            _rb.AddForce(-_rb.velocity.normalized * brakeForce * Time.deltaTime * 100, ForceMode.Acceleration);
        }
        else
        {
            _rb.AddForce(-transform.forward * reverseForce * Time.deltaTime * 100, ForceMode.Acceleration);
        }
    }

    private void Steer()
    {
        if (_totalVelocity < 0.1f)
            return;

        float torque = _steer * steerSpeed;

        float currentAngularVelocity = _rb.angularVelocity.y;

        float newAngularVelocity = _rb.angularVelocity.y + torque;

        if (Mathf.Abs(newAngularVelocity) <= maxAngularVelocity || Mathf.Sign(torque) != Mathf.Sign(currentAngularVelocity))
        {
            _rb.AddTorque(Vector3.up * torque * Time.deltaTime * 100, ForceMode.Acceleration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 bounceDirection = -collision.contacts[0].normal;

        _rb.velocity = Vector3.Reflect(_rb.velocity, bounceDirection) * bounceFactor;
    }

    private void ResetPlayer(Course course)
    {
        _rb.velocity = Vector3.zero;
        _steer = 0.0f;
        _acceleratePressed = false;
        _reversePressed = false;

        transform.position = course.SpawnPoint.position + (GetComponent<PlayerInput>().playerIndex > 0 ? new Vector3(_gameManager.Player2Offset, 0, 0) : Vector3.zero);
        transform.rotation = course.SpawnPoint.rotation;
    }

    public void OnAccelerate(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _acceleratePressed = true;
        }
        else if (ctx.canceled)
        {
            _acceleratePressed = false;
        }

        UpdatePlayerState();
    }

    public void OnSteer(InputAction.CallbackContext ctx)
    {
        _steer = ctx.ReadValue<float>();
    }

    public void OnReverse(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _reversePressed = true;
        }
        else if (ctx.canceled)
        {
            _reversePressed = false;
        }

        UpdatePlayerState();
    }
}
