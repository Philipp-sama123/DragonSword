using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerInput _playerInput;

    private LocomotionManager _locomotionManager; // ToDo: Think of Required Field 

    private AnimatorManager _animatorManager; // ToDo: Think of Required Field 
    // PlayerCombatManager playerCombatManager; // ToDo: Think of Required Field 
    // SwitchVirtualCamera switchVirtual;

    public Vector2 movementInput;
    public Vector2 cameraInput;

    public float cameraInputX;
    public float cameraInputY;

    public float moveAmount;
    public float horizontalInput;
    public float verticalInput;

    public bool crouch_input;

    public bool sprintInput;
    public bool x_Input;
    public bool jump_input;

    public bool right_trigger_input;
    public bool left_trigger_input;

    public bool right_button_hold_input; // todo think of sth better
    public bool left_button_hold_input; // todo think of sth better

    private void Awake()
    {
        _locomotionManager = GetComponent<LocomotionManager>();
        _animatorManager = GetComponent<AnimatorManager>();

        // playerCombatManager = GetComponent<PlayerCombatManager>();
        // switchVirtual = FindObjectOfType<SwitchVirtualCamera>();
        // should be just one 
    }

    private void OnEnable()
    {
        if (_playerInput == null)
        {
            _playerInput = new PlayerInput();

            // ToDo: Describe this Shit!
            _playerInput.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            _playerInput.PlayerMovement.Camera.performed +=
                i => cameraInput =
                    i.ReadValue<Vector2>(); // if you move the mouse or the right joystick it will then send it to the camera input // more explain needed

            _playerInput.PlayerActions.B.performed += i => sprintInput = true; // b hit --> when holding it 
            _playerInput.PlayerActions.B.canceled += i => sprintInput = false;

            _playerInput.PlayerActions.X.performed += i => x_Input = true; // set true when pressed 
            _playerInput.PlayerActions.X.canceled += i => x_Input = false;

            _playerInput.PlayerActions.Jump.performed += i => jump_input = true; // set true when pressed 
            _playerInput.PlayerActions.Jump.canceled += i => jump_input = false;

            _playerInput.PlayerActions.RT.performed += i => right_trigger_input = true;
            _playerInput.PlayerActions.RT.canceled += i => right_trigger_input = false;

            _playerInput.PlayerActions.RB_Hold.performed += i => right_button_hold_input = true;
            _playerInput.PlayerActions.RB_Hold.canceled += i => right_button_hold_input = false;

            _playerInput.PlayerActions.LB_Hold.performed += i => left_button_hold_input = true;
            _playerInput.PlayerActions.LB_Hold.canceled += i => left_button_hold_input = false;

            _playerInput.PlayerMovement.ToggleCrouching.performed += i => crouch_input = true;
            _playerInput.PlayerMovement.ToggleCrouching.canceled += i => crouch_input = false;

            _playerInput.PlayerActions.LT.performed += _ => HandleAimingInput(true);
            _playerInput.PlayerActions.LT.canceled += _ => HandleAimingInput(false);
        }

        _playerInput.Enable();
    }

    private void OnDisable()
    {
        //    playerControls.PlayerActions.LT.performed -= _ => HandleAimingInput(true);
        //    playerControls.PlayerActions.LT.canceled -= _ => HandleAimingInput(false);

        _playerInput.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpingInput();
        HandleSlideInput();

        HandleAttackInput();
        HandleDefenseInput();
        HandleCrouchInput();

        //    HandleActionInput(); 
    }

    private void HandleMovementInput()
    {
        horizontalInput = movementInput.x;
        verticalInput = movementInput.y;

        cameraInputX = cameraInput.x; // take input from joystick and then pass it to move the camera 
        cameraInputY = cameraInput.y;

        moveAmount =
            Mathf.Clamp01(Mathf.Abs(horizontalInput) +
                          Mathf.Abs(verticalInput)); // clamp value between 0 and 1 // Abs - Absolute Value

        //ToDo: set animator values for rotation
        //ToDo: think on when to use moveAmount
     
    }

    private void HandleSprintingInput()
    {
        if (sprintInput == true && moveAmount > 0.5f)
        {
            _locomotionManager.isSprinting = true;
        }
        else
        {
            _locomotionManager.isSprinting = false;
        }
    }

    private void HandleJumpingInput()
    {
        if (jump_input)
        {
            jump_input = false;
            _locomotionManager.HandleJumping();
        }
    }

    private void HandleSlideInput()
    {
        if (x_Input)
        {
            _locomotionManager.HandleSlide();
        }
    }

    private void HandleAttackInput() // maybe wrap in action input#
    {
        // if (right_trigger_input || right_button_hold_input)
        // {
        //     playerCombatManager.HandleAttack(right_trigger_input, right_button_hold_input);
        // }
    }

    private void HandleDefenseInput()
    {
        // if (left_button_hold_input)
        // {
        //     playerCombatManager.HandleDefense();
        // }
    }

    private void HandleCrouchInput()
    {
        _locomotionManager.HandleCrouchInput(crouch_input);
    }

    private void HandleAimingInput(bool isAiming)
    {
        // if (isAiming)
        // {
        //     switchVirtual.StartAiming();
        // }
        // else
        // {
        //     switchVirtual.CancelAiming();
        // }
    }
}