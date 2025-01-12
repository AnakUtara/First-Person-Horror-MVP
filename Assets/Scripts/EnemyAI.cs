using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float waitTime = 5f;
    [SerializeField] private float chaseRadius;
    private float _distanceToTarget;
    private int _currentPatrolPointIndex;
    private bool _isWaiting;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
    //
    // }

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
        
        //Make sure pathfinding and coroutine execution only when is waiting state is false.
        //It's to avoid over-execution of coroutine during frame updates.
        if (_isWaiting == false)
        {
            //Check if agent has finished calculating path and if remaining distance is less than equal to stop at target distance
            //Make stop area a little bit larger by adding 0.5f to stop distance
            if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
            {
                StartCoroutine(WaitAtPatrolPoint());
            }
        }
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

    private IEnumerator WaitAtPatrolPoint()
    {
        //Set is waiting state to true
        _isWaiting = true;
        //Stop agent from moving
        agent.isStopped = true;
        //Wait for (waitTime)th of second
        yield return new WaitForSeconds(waitTime);
        //Modulo index by the total number of points to move agent back to starting point once finish visited all points
        _currentPatrolPointIndex = (_currentPatrolPointIndex + 1) % patrolPoints.Length;
        //Allow agent to move
        agent.isStopped = false;
        //Set is waiting state to false, initiate wait loop
        _isWaiting = false;
    }
}
