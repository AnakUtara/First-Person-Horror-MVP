using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{ 
    public class EnemyAI : MonoBehaviour
    {
        [Header("Pathfinding Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private float waitTime = 5f;
        [SerializeField] private float catchRadius;
        [SerializeField] private Animator animator;
        
        private FieldOfView _fov;
        private float _distanceToTarget;
        private Vector3 _directionToTarget;
        private int _currentPatrolPointIndex;
        private bool _isMoving, _isCaught;
        private float _chaseRadius;

        private enum State
        {
            Idle,
            Patrol,
            Chase
        };
        
        private State _currentState;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, catchRadius);
            Gizmos.color = Color.yellow;
        }

        private void Awake()
        {
            _fov = GetComponent<FieldOfView>();
            _currentState = State.Patrol;
            _isMoving = true;
            _isCaught = false;
            _chaseRadius = _fov.viewRadius;
        }

        private void Update()
        {
            ApplyMoveState();
            UpdateState();
            RunStateMachine();
        }

        private void ApplyMoveState()
        {
            if (_isMoving)
            {
                Move();
            }
            else
            {
                Idle();
            }
        }

        private void UpdateState()
        {
            if (_isMoving)
            {
                if (_fov.isPlayerVisible)
                {
                    TransitionToState(State.Chase, "Chase");
                } else if(_distanceToTarget > _chaseRadius)
                {
                    TransitionToState(State.Patrol, "Walk");
                }
            }
            else
            {
                TransitionToState(State.Idle, "Idle");
            }
        }

        private void TransitionToState(State newState, string animationTrigger)
        {
            if (_currentState == newState) return;

            _currentState = newState;
            SetAnimatorTrigger(animationTrigger);
        }

        private void SetAnimatorTrigger(string trigger)
        {
            animator.ResetTrigger("Idle");
            animator.ResetTrigger("Walk");
            animator.ResetTrigger("Chase");
            animator.SetTrigger(trigger);
        }

        private void RunStateMachine()
        {
            switch (_currentState)
            {
                case State.Patrol:
                    Patrol();
                    break;
                case State.Chase:
                    Chase();
                    break;
                case State.Idle:
                    Idle();
                    break;
            }
        }

        private void Patrol()
        {
            _fov.isPlayerVisible = false;
            _isMoving = true;
            agent.speed = 5f;
            //Make agent move to assigned destination by default
            agent.SetDestination(patrolPoints[_currentPatrolPointIndex].position);
            //Check if agent has finished calculating path and if remaining distance is less than equal to stop at target distance
            //Make stop area a little bit larger by adding 0.5f to stop distance
            if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
            {
                StartCoroutine(HoldPatrol());
            }
        }
        
        private IEnumerator HoldPatrol()
        {
            _isMoving = false;
            //Wait for (waitTime)th of second
            yield return new WaitForSeconds(waitTime);
            //Modulo index by the total number of points to move agent back to starting point once finish visited all points
            _currentPatrolPointIndex = (_currentPatrolPointIndex + 1) % patrolPoints.Length;
            _isMoving = true;
        }

        private void Chase()
        {
            agent.speed = 12;
            _distanceToTarget = Vector3.Distance(target.position, transform.position);
            if (_distanceToTarget < _chaseRadius)
            {
                _fov.isPlayerVisible = true;
                _isMoving = true;
                agent.SetDestination(target.position);
                if (_distanceToTarget < catchRadius)
                {
                    _isMoving = false;
                    _isCaught = true;
                }
                else
                {
                    _isMoving = true;
                    _isCaught = false;
                }
            }
            Debug.Log($"Player visible: {_fov.isPlayerVisible} | Player is caught: {_isCaught}");
        }
        
        private void Idle()
        {
            agent.isStopped = true;
        }

        private void Move()
        {
            agent.isStopped = false;
        }
    }
}
