using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    [RequireComponent(typeof (NavMeshAgent))]
    [RequireComponent(typeof (Animator))]
    public class Mover : MonoBehaviour, IAction
    {
        // GR: Configuration variables
        [SerializeField] float maxSpeed = 6f;

        // GR: State variables

        // GR: Referenced variables
        NavMeshAgent navMeshAgent;
        Animator animator;
        Health health;

        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            navMeshAgent.enabled = !health.IsDead(); // GR: This turns off the NavMeshAgent which turns off the thin navMeshCollider which you can sometimes bump into still once the character has died.
            
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedAdjustment)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedAdjustment);
        }

        public void MoveTo(Vector3 destination, float speedAdjustment)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedAdjustment);
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            animator.SetFloat("forwardSpeed", speed);
        }
    }
}


