using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Combat;
using RPG.Attributes;
using RPG.Movement;
using GameDevTV.Utils;
using System;

namespace RPG.Control
{

    public class AIController : MonoBehaviour
    {
        private GameObject player;
        private NavMeshAgent agent;
        private Mover mover;
        private Animator animator;
        public float timeSinceLastSawPlayer = 0.0f;

        private int wayPointsCount;

        private LazyValue<Vector3> guardLocation;

        public int currentWaypoint;
        public bool bPatrol = false;
        [SerializeField] float suspicionTime = 3.0f;
        [SerializeField] float chaseDistance = 5.0f;
        [SerializeField] PatrolPath patrolPath = null;
        [SerializeField] float waypointTolerance = 1.0f;
        [SerializeField] float dwellTime = 0.0f;
        [SerializeField] float maxDwellTime = 2.0f;
        [Range(0, 1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        [SerializeField] float aggrevateTime = 0.0f;
        [SerializeField] bool IsAggrevated = false;
        [SerializeField] float aggroCooldown = 5.0f;
        [SerializeField] float shoutDistance = 10.0f;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
            agent = GetComponent<NavMeshAgent>();
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
            guardLocation = new LazyValue<Vector3>(SetGuardLocation);
            
            currentWaypoint = 0;

            if (GetComponent<Fighter>().GetWeapon().HasProjectile())
            {
                chaseDistance = GetComponent<Fighter>().GetWeapon().GetWeaponRange();
            }

            if (patrolPath)
            {
                wayPointsCount = patrolPath.transform.childCount;
                bPatrol = true;
            }
        }


        private Vector3 SetGuardLocation()
        {
            return transform.position;
        }

        private void Start()
        {
            guardLocation.ForceInit();
        }

        private void Update()
        {
            if (!GetComponent<Health>().Died())
            {
                if(patrolPath && bPatrol)
                    PatrolBehavior();



                if ((Vector3.Distance(player.transform.position, transform.position) < chaseDistance
                && player.GetComponent<Fighter>().CanAttack() || IsAggrevated))
                {
                    GetComponent<Fighter>().Attack(player);
                    timeSinceLastSawPlayer = 0.0f;
                    AggrevateNearbyEnemies();
                    bPatrol = false;   
                }
                else if(Vector3.Distance(player.transform.position, transform.position) > chaseDistance)
                {
                    timeSinceLastSawPlayer += Time.deltaTime;
                    GetComponent<Fighter>().Cancel();
                }


                if (timeSinceLastSawPlayer > suspicionTime)
                {
                    if (patrolPath)
                    {
                        mover.StartAction(GetCurrentWayPoints(), patrolSpeedFraction);
                        bPatrol = true;
                    }
                    else
                        mover.StartAction(guardLocation.value, patrolSpeedFraction);
                    animator.SetBool("Unarmed Idle", false);
                }
            }
            else
                GetComponent<Fighter>().Cancel();

            if(IsAggrevated)
            {
                aggrevateTime += Time.deltaTime;
                if(aggrevateTime > aggroCooldown)
                {
                    aggrevateTime = 0.0f;
                    IsAggrevated = false;
                }
            }
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);

            foreach(RaycastHit hit in hits)
            {
                AIController ai = hit.transform.GetComponent<AIController>();

                if (ai == null)
                    continue;
                ai.Aggrevate();
            }
        }

        public void Aggrevate()
        {
            IsAggrevated = true;
        }


        private void PatrolBehavior()
        {
            Vector3 nextPos = guardLocation.value;
            if (AtWaypoint())
            {
                dwellTime += Time.deltaTime;
                if(dwellTime > maxDwellTime)
                {
                    CycleWaypoint();
                    dwellTime = 0.0f;
                }
            }

            nextPos = GetCurrentWayPoints();

            mover.StartAction(nextPos, patrolSpeedFraction);
        }

        private bool AtWaypoint()
        {
            float distToWaypoint = Vector3.Distance(transform.position, GetCurrentWayPoints());

            return distToWaypoint < waypointTolerance;

        }

        private void CycleWaypoint()
        {
            currentWaypoint = (currentWaypoint + 1) % wayPointsCount;
        }

        private Vector3 GetCurrentWayPoints()
        {
            return patrolPath.transform.GetChild(currentWaypoint).position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.transform.position, chaseDistance);
        }
    }

}