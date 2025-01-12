using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float startWaitTime = 5f;
    [SerializeField] private float chaseRadius;
    private float _distanceToTarget;
    private int _currentPatrolPointIndex;
    private float _waitTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _waitTime = startWaitTime;
    }

    // Update is called once per frame
    void Update()
    {
       Patrol();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
        Gizmos.color = Color.yellow;
    }

    private void Patrol()
    {
        //Set agent patrol speed to slow
        agent.speed = 5f;
        //Make agent move to assigned destination by default
        agent.SetDestination(patrolPoints[_currentPatrolPointIndex].position);
        //Check if agent has finished calculating path and if remaining distance is less than equal to stop at target distance
        //Make stop area a little bit larger by adding 0.5f to stop distance
        if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
        {
            //If wait time has done
            if (_waitTime <= 0)
            {
                //Increment Patrol Point Index to move agent to next point
                //Modulo index by the total number of points to move agent back to starting point once finish visited all points
                _currentPatrolPointIndex = (_currentPatrolPointIndex + 1) % patrolPoints.Length;
                //Reset wait time
                _waitTime = startWaitTime;
                agent.isStopped = false;
            }
            else
            {
                agent.isStopped = true;
                //Decrement wait time per second of game time to stop agent for start wait time amount of seconds
                //to simulate agent waiting to search upon reaching patrol point
                _waitTime -= Time.deltaTime;
            }
        }
        // Debug.Log($"Moving to patrol point: {_currentPatrolPointIndex}");
        // Debug.Log($"Wait time: {_waitTime}");
    }

    private void Chase()
    {
        _distanceToTarget = Vector3.Distance(target.position, transform.position);
        if (_distanceToTarget < chaseRadius)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }
}
