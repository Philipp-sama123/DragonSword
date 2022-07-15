using UnityEngine;

// [RequireComponent(typeof(Rigidbody), typeof(PlayerManager), typeof(AnimatorManager))]
// [RequireComponent(typeof(InputManager))]
public class LocomotionManager : MonoBehaviour
{
    // ToDo: Think of Required Field 
    public Rigidbody playerRigidbody;

    private PlayerManager _playerManager;
    private AnimatorManager _animatorManager;
    private InputManager _inputManager;
    private Transform _cameraTransform;

    Vector3 _moveDirection = Vector3.zero;

    [Header("Falling")] public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffset = 0.5f;
    public LayerMask groundLayer;

    [Header("Movement Flags")] public bool isSprinting;

    public bool isJumping;

    public bool isGrounded;
    // ToDo: think about it: Brackeys isGroundedCheck: isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask); 


    [Header("Movement Speeds")] public float walkingSpeed = 1.5f;
    public float runningSpeed = 5f;
    public float sprintingSpeed = 7f;
    public float crouchingSpeedReducer = 5f;
    public float rotationSpeed = 15f;

    public float turnTime = .1f;
    public float turnVelocity;


    [Header("Jump Speeds")] public float jumpHeight = 3;
    public float gravityIntensity = -15;

    private bool _isCrouching = false;
    private static readonly int IsCrouching = Animator.StringToHash("IsCrouching");
    private static readonly int IsUsingRootMotion = Animator.StringToHash("IsUsingRootMotion");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");


    private void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
        _animatorManager = GetComponent<AnimatorManager>();
        _inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        if (Camera.main != null) _cameraTransform = Camera.main.transform;
    }

    public void HandleAllMovements()
    {
        HandleFallingAndLanding();
        if (_playerManager.isInteracting || isJumping)
        {
            return;
        }

        HandleFreeMovement();
        HandleFreeMovementRotation();
    }

    private void HandleFreeMovement()
    {
        _isCrouching = _animatorManager.animator.GetBool(IsCrouching);

        _moveDirection = _cameraTransform.forward * _inputManager.verticalInput;
        _moveDirection = _moveDirection + _cameraTransform.right * _inputManager.horizontalInput;
        _moveDirection.Normalize();
        _moveDirection.y = 0; // prevent going up

        if (isSprinting)
        {
            _moveDirection *= sprintingSpeed;
        }
        else
        {
            if (_inputManager.moveAmount >= 0.5f)
            {
                _moveDirection *= runningSpeed;
            }
            else
            {
                _moveDirection *= walkingSpeed;
            }
        }

        if (_isCrouching)
        {
            _moveDirection /= crouchingSpeedReducer;
        }

        Vector3 movementVelocity = _moveDirection;
        playerRigidbody.velocity = movementVelocity;
    }

    private void HandleMovement()
    {
        _isCrouching = _animatorManager.animator.GetBool(IsCrouching);

        var playerTransform = transform;
        _moveDirection = playerTransform.forward * _inputManager.verticalInput;
        _moveDirection += playerTransform.right * _inputManager.horizontalInput;

        _moveDirection.Normalize();
        _moveDirection.y = 0; // prevent going up

        if (isSprinting)
        {
            _moveDirection *= sprintingSpeed;
        }
        else
        {
            if (_moveDirection.z >= 0.5f)
            {
                _moveDirection *= runningSpeed;
            }
            else
            {
                _moveDirection *= walkingSpeed;
            }
        }

        if (_isCrouching)
        {
            _moveDirection /= crouchingSpeedReducer;
        }

        playerRigidbody.velocity = _moveDirection;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        var position = transform.position;
        Vector3 raycastOrigin = position;
        Vector3 targetPosition = position; // for the feet
        raycastOrigin.y = raycastOrigin.y + rayCastHeightOffset;

        if (!isGrounded && !isJumping)
        {
            if (!_playerManager.isInteracting)
            {
                _animatorManager.PlayTargetAnimation("Falling",
                    true); // ToDo -- maybe find something for the Animator Stuff string references
            }

            _animatorManager.animator.SetBool(IsUsingRootMotion, false);

            inAirTimer += Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity); // step of the ledge
            playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
            //  -Vector3.up     --      means it pulls you downwards 
            //  * inAirTimer    --      the longer you are in the air the quicker you fall
        }

        if (Physics.SphereCast(raycastOrigin, 0.2f, -Vector3.up, out hit, groundLayer))
        {
            if (!isGrounded && !_playerManager.isInteracting)
            {
                _animatorManager.PlayTargetAnimation("Land",
                    true); // ToDo -- maybe find something for the Animator Stuff string references
            }

            Vector3 raycastHitPoint = hit.point; // where the raycast hits the ground 
            targetPosition.y =
                raycastHitPoint.y; // assign the point where the raycast hits the ground to target position

            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        // this is responsible for setting the feet over the ground when the player is grounded
        if (isGrounded && !isJumping)
        {
            if (_playerManager.isInteracting /*|| _inputManager.moveAmount > 0*/)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }

    public void HandleJumping()
    {
        if (!isGrounded) return;
        _animatorManager.animator.SetBool(IsJumping, true);
        _animatorManager.PlayTargetAnimation("Jump", false);
        float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
        Vector3 playerVelocity = _moveDirection;

        playerVelocity.y = jumpingVelocity; // adds jumping velocity to movement
        playerRigidbody.velocity = playerVelocity;
    }

    public void HandleDodge()
    {
        if (_playerManager.isInteracting)
        {
            return;
        }

        _animatorManager.PlayTargetAnimation("Dodge Forward", true, true);
        // toggle invulnerable bool here
    }
    
    private void HandleFreeMovementRotation()
    {
        Vector3 direction = new Vector3(_inputManager.horizontalInput, 0f, _inputManager.verticalInput).normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnTime);

        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
}