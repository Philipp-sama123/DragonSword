using UnityEngine;

public class ResetBool : StateMachineBehaviour
{
    public string isInteractingBool;
    public string isUsingRootMotionBool;
    public string isJumpingBool;

    public bool isInteractingStatus;
    public bool isUsingRootMotionStatus;
    public bool isJumpingBoolStatus;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isInteractingBool.Length > 0)
            animator.SetBool(isInteractingBool, isInteractingStatus);
        if (isUsingRootMotionBool.Length > 0)
            animator.SetBool(isUsingRootMotionBool, isUsingRootMotionStatus);
        if (isJumpingBool.Length > 0)
            animator.SetBool(isJumpingBool, isJumpingBoolStatus);
    }
}