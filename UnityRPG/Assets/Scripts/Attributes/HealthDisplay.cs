using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Stats;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        public Health health;
        public Text healthText;
        public BaseStats baseStats;


        private void Awake()
        {
            health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
            healthText = GetComponent<Text>();
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            healthText.text = "Health : " + (int)health.GetCurrentHealth() + " / " + (int)baseStats.GetStat(Stat.Health);
        }
    }

}