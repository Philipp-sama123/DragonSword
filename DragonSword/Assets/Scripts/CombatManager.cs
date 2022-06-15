using UnityEngine;

namespace DefaultNamespace
{
    public class CombatManager : MonoBehaviour
    {
        private AnimatorManager _animatorManager;
        private PlayerManager _playerManager;
        private LocomotionManager _locomotionManager;

        private void Awake()
        {
            _animatorManager = GetComponent<AnimatorManager>();
            _playerManager = GetComponent<PlayerManager>();
            _locomotionManager = GetComponent<LocomotionManager>();
        }

        public void HandlePrimaryAttack()
        {
            if (!_locomotionManager.isGrounded || _playerManager.isInteracting) return;
            // ToDo: Handle Attacks better when attacking or blocking
            _animatorManager.PlayTargetAnimation("Primary Attack 1", true, true);
        }

        public void HandleDefense()
        {
            // ToDo: Handle Attacks better when attacking or blocking
            if (_locomotionManager.isGrounded)
            {
                _animatorManager.PlayTargetAnimation("Defense", true, true);
            }
        }
    }
}