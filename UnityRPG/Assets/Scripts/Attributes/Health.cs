using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using GameDevTV.Utils;
using UnityEngine.Events;
using RPG.UI;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        public bool death = false;
        [SerializeField] float regenPercentage = 70;
        [SerializeField] LazyValue<float> health;
        [SerializeField] TakeDamageEvent damageEvent;
        [SerializeField] HealthBar healthBar;
        [SerializeField] UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>{}


        public BaseStats baseStats;

        public bool HasBeenRestored = false;


        private void Awake()
        {
            if (!HasBeenRestored)
            {
                health = new LazyValue<float>(GetInitialHealth);
            }
            baseStats = GetComponent<BaseStats>();
            healthBar = GetComponentInChildren<HealthBar>();
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void OnEnable()
        {
            if (baseStats)
                baseStats.onLevelUp += RestoreHealth;
        }

        private void OnDisable()
        {
            if (baseStats)
                baseStats.onLevelUp -= RestoreHealth;
        }

        private void Start()
        {
            health.ForceInit();
        }

        private void RestoreHealth()
        {
            float regen = GetComponent<BaseStats>().GetStat(Stat.Health) * regenPercentage / 100;
            health.value = Mathf.Max(regen, health.value);
        }

        public void TakeDamage(float damage, GameObject instigator)
        {
           

            if (health.value >= 0.0f)
            {
                health.value -= damage;
                damageEvent.Invoke(damage);
               
            }
            
            if(health.value <= 0.0f && !Died())
            {
                Die();
                onDie.Invoke();
                if(instigator.GetComponent<Experience>())
                    instigator.GetComponent<Experience>().EarnExp(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
                else
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    player.GetComponent<Experience>().EarnExp(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
                }
            }
            if(healthBar)
                healthBar.UpdateHealthBar();
        }

        private void Update()
        {
            
        }

        public float GetFraction()
        {
            return health.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Die()
        {
            GetComponent<Animator>().SetTrigger("Death");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            death = true;
        }

        public bool Died()
        {
            return death;
        }

        public object CaptureState()
        {
            return health.value;
        }

        public void RestoreState(object state)
        {
            health.value = (float)state;
            if (health.value <= 0)
                Die();

            HasBeenRestored = true;
        }

      

        public float GetPrecentage()
        {
            return 100 * (health.value / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        public float GetCurrentHealth()
        {
            return health.value;
        }

        public void heal(float amount)
        {
            health.value += amount;
        }
    }

}