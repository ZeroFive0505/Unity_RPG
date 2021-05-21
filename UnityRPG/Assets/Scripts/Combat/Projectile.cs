using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 3.0f;
        [SerializeField] Health target;
        [SerializeField] float destroyRange = 50.0f;
        [SerializeField] bool IsHoming = false;
        [SerializeField] GameObject hitVFX = null;
        [SerializeField] GameObject[] destroyonHit = null;
        [SerializeField] float lifeAfterImpact = 2f;
        [SerializeField] UnityEvent onDestroy;
        public GameObject player;
        public GameObject instigator;
        float damage = 0.0f;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {


            transform.position += transform.forward * Time.deltaTime * speed;

            if (target == null)
                return;

            if (IsHoming && !target.GetComponent<Health>().Died())
                transform.LookAt(GetAimLocation());
      
            

            if (Vector3.Distance(transform.position, player.transform.position) > destroyRange)
                Destroy(gameObject);
        }

        public void SetTarget(Health target, float damage, GameObject instigator)
        {
            if (target)
            {
                this.target = target;
                this.damage = damage;
                this.instigator = instigator;
                transform.LookAt(GetAimLocation());
            }
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
                return target.transform.position;
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == instigator)
                return;
            Health target = other.GetComponent<Health>();
            if (target && !target.GetComponent<Health>().Died())
            {
                target.TakeDamage(damage, instigator);
                speed = 0;
                if (hitVFX)
                {
                    GameObject vfx = Instantiate(hitVFX, transform.position, transform.rotation);
                    onDestroy.Invoke();
                    Destroy(vfx, 2f);
                }

                foreach (GameObject obj in destroyonHit)
                    Destroy(obj);

                Destroy(gameObject, lifeAfterImpact);
            }
        }
    }

}