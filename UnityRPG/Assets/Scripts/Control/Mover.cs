using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;


namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        public Transform target;

        private NavMeshAgent agent;
        private Animator animator;


        [SerializeField] float maxNavPathLength = 20.0f;
        [SerializeField] float maxSpeed = 6.0f;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            agent.enabled = !GetComponent<Health>().Died();
            UpdateAnimator();
        }

        public void StartAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            animator.SetBool("Unarmed Idle", false);
            MoveToPoint(destination, speedFraction);
        }


        public void MoveToPoint(Vector3 destination, float speeedFraction)
        {
            agent.isStopped = false;
            agent.speed = maxSpeed * Mathf.Clamp01(speeedFraction);
            if (agent)
                agent.SetDestination(destination);
        }

     
        public void Cancel()
        {
            animator.ResetTrigger("StopAttacking");
            if(agent.isActiveAndEnabled)
                agent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            animator.SetFloat("Forwad Speed", speed);
        }


        //Animation Event
        public void FootL()
        {

        }


        //Animation Event
        public void FootR()
        {

        }

        public object CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> data = (Dictionary<string, object>) state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = ((SerializableVector3)data["position"]).ToVector();
            transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);

            if (!hasPath)
                return false;

            if (path.status != NavMeshPathStatus.PathComplete)
                return false;

            if (GetPathLength(path) > maxNavPathLength)
                return false;

            return true;
        }


        private float GetPathLength(NavMeshPath path)
        {
            Vector3 from = new Vector3();
            Vector3 to = new Vector3();
            float sumDistance = 0;

            if (path.corners.Length < 2)
                return sumDistance;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                from = path.corners[i];
                to = path.corners[i + 1];

                sumDistance += Vector3.Distance(from, to);
            }
            return sumDistance;
        }
    }

}
