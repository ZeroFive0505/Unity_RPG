using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyInfo : MonoBehaviour
    {
        public Health health;
        public Fighter fighter;
        public Text healthText;

        private void Awake()
        {
            fighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
            healthText = GetComponent<Text>();
        }

        private void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if(fighter.GetTarget())
            {
                health = fighter.GetTarget().GetComponent<Health>();
                if (health.Died())
                    healthText.text = "Died!";
                else
                    healthText.text = "Enemy : " + health.GetCurrentHealth();
            }
            else
            {
                healthText.text = "No Target";
            }
        }
    }

}