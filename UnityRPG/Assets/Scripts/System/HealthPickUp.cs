using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;

namespace RPG.Attributes
{
    public class HealthPickUp : MonoBehaviour
    {

        [SerializeField] float restoreAmount = 40.0f;
        [SerializeField] float respawnTime = 0.0f;
        [SerializeField] float timeToRespawn = 5.0f;
        [SerializeField] bool hasBeenEaten = false;
        [SerializeField] MeshCollider meshCollider;
        [SerializeField] MeshRenderer meshRenderer;

        private void Start()
        {
            meshCollider = GetComponent<MeshCollider>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            if(hasBeenEaten)
            {
                respawnTime += Time.deltaTime;

                if(respawnTime >= timeToRespawn)
                {
                    hasBeenEaten = false;
                    meshCollider.enabled = true;
                    meshRenderer.enabled = true;
                    respawnTime = 0.0f;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player" && !hasBeenEaten)
            {
                Health health = other.GetComponent<Health>();
                if (health.GetCurrentHealth() <= other.gameObject.GetComponent<BaseStats>().GetStat(Stat.Health) - restoreAmount)
                {
                    health.heal(restoreAmount);
                    hasBeenEaten = true;
                    meshCollider.enabled = false;
                    meshRenderer.enabled = false;
                }
            }
        }
    }
}
