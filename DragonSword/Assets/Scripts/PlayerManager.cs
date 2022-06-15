using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // ToDo: Think of Required Fields
    private Animator _animator;
    private InputManager _inputManager;
    private LocomotionManager _locomotionManager;

    public bool isInteracting;
    public bool isUsingRootMotion;

    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int IsUsingRootMotion = Animator.StringToHash("IsUsingRootMotion");
    private static readonly int IsInteracting = Animator.StringToHash("IsInteracting");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _inputManager = GetComponent<InputManager>();
        _locomotionManager = GetComponent<LocomotionManager>();
    }

    private void Update()
    {
        _inputManager.HandleAllInputs();
    }

    // When working with Rigidbodies everything behaves much nicer in fixedUpdate -- Unity specific 
    // everything moving related should happen here 
    private void FixedUpdate()
    {
        _locomotionManager.HandleAllMovements();
    }

    // called after Frame ended
    private void LateUpdate()
    {
        // camera.HandleAllCameraMovement();
        _animator.SetBool(IsGrounded, _locomotionManager.isGrounded);

        _locomotionManager.isJumping = _animator.GetBool(IsJumping);
        isInteracting = _animator.GetBool(IsInteracting);
        isUsingRootMotion = _animator.GetBool(IsUsingRootMotion);
    }
}