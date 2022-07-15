using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class DragonManager : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints;

    [SerializeField] public LayerMask playerLayer;
    [SerializeField] public Transform playerTransform;
    [SerializeField] public Camera attackRaycastArea;
    [SerializeField] public GameObject lookPoint; // Todo: make dynamic
    [SerializeField] private GameObject bloodEffect;
    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float runningSpeed = 5f;
    [SerializeField] private float attackingRadius = 10f;
    [SerializeField] private float initHealth = 100f;
    [SerializeField] private float visionRadius = 20f;

    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float timeBetweenAttacks = 5f;

    private NavMeshAgent _navMeshAgent;

    private int _currentDragonPosition = 0;

    private float _waypointRadius = 2f;
    private float _currentHealth;

    private bool _isPreviousAttackActive;

    private bool _isPlayerInVisionRadius;
    private bool _isPlayerInAttackingRadius;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _currentHealth = initHealth;
    }


    private void Update()
    {
        var position = transform.position;
        _isPlayerInVisionRadius = Physics.CheckSphere(position, visionRadius, playerLayer);
        _isPlayerInAttackingRadius = Physics.CheckSphere(position, attackingRadius, playerLayer);

        if (!_isPlayerInVisionRadius && !_isPlayerInAttackingRadius)
        {
            CircleBetweenWaypoints();
        }

        if (_isPlayerInVisionRadius && !_isPlayerInAttackingRadius)
        {
            RunTowardsPlayer();
        }

        if (_isPlayerInVisionRadius && _isPlayerInAttackingRadius)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        _navMeshAgent.SetDestination(transform.position);
        if (!_isPreviousAttackActive)
        {
            RaycastHit hitInfo;
            
            if (Physics.Raycast(attackRaycastArea.transform.position, attackRaycastArea.transform.forward,
                    out hitInfo, attackingRadius))
            {
                Debug.Log("Attacking");

                Animator animator = GetComponent<Animator>();
                animator.CrossFade("Attack", 0.2f);

                PlayerManager playerManager = hitInfo.transform.GetComponent<PlayerManager>();
                if (playerManager != null)
                {
                    playerManager.OnPlayerDamage(damageAmount);
                    GameObject impactGo = Instantiate(bloodEffect, hitInfo.point,
                        Quaternion.LookRotation(hitInfo.normal));
                    Destroy(impactGo, 1f);
                }
            }

            _isPreviousAttackActive = true;
            Invoke(nameof(ActiveAttacking), timeBetweenAttacks); // Todo also for player like this 
        }
    }

    private void ActiveAttacking()
    {
        _isPreviousAttackActive = false;
    }

    private void RunTowardsPlayer()
    {
        var playerPosition = playerTransform.position;

        Animator animator = GetComponent<Animator>();
        animator.SetFloat("MoveAmount", runningSpeed * 2);

        _navMeshAgent.SetDestination(playerPosition);
        transform.LookAt(playerPosition);
    }

    private void CircleBetweenWaypoints()
    {
        if (Vector3.Distance(waypoints[_currentDragonPosition].transform.position, transform.position) <
            _waypointRadius)
        {
            Animator animator = GetComponent<Animator>();
            animator.SetFloat("MoveAmount", walkingSpeed);

            _currentDragonPosition = Random.Range(0, waypoints.Length);
            if (_currentDragonPosition >= waypoints.Length)
            {
                _currentDragonPosition = 0;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position,
            waypoints[_currentDragonPosition].transform.position, Time.deltaTime * walkingSpeed);
        transform.LookAt(waypoints[_currentDragonPosition].transform.position);
    }

    public void OnZombieDamage(float takeDamage)
    {
        _currentHealth -= takeDamage;
        if (_currentHealth <= 0)
        {
            DragonDie();
        }
    }

    private void DragonDie()
    {
        _navMeshAgent.SetDestination(transform.position);
        walkingSpeed = 0f;
        attackingRadius = 0f;
        visionRadius = 0f;
        _isPlayerInAttackingRadius = false;
        _isPlayerInVisionRadius = false;

        Animator animator = GetComponent<Animator>();
        animator.CrossFade("Death", .2f);
        Destroy(gameObject, 5f);
    }
}