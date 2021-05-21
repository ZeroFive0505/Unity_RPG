using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameDevTV.Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 10)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpVFX = null;
        [SerializeField] bool useModifier = false;
        public Action onLevelUp;
        public float currentDamageFactor;
        Experience experience;

        public LazyValue<int> currentLevel;


        private void Awake()
        {
            currentLevel = new LazyValue<int>(CalculateLevel);
            experience = GetComponent<Experience>();      
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (experience)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if(newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                GameObject vfx = Instantiate(levelUpVFX, transform);
                onLevelUp();
                Destroy(vfx, 3f);
            }
        }

        public float GetStat(Stat stat)
        {
            return (GerBaseStat(stat) + GetAddtiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!useModifier)
                return 0;
            IModifierProvider[] providers = GetComponents<IModifierProvider>();
            float sum = 0;
            foreach(IModifierProvider provider in providers)
            {
                foreach(float percentage in provider.GetPercentageModifiers(stat))
                {
                    sum += percentage;
                }
            }

            return sum;
        }

        private float GerBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, CalculateLevel());
        }

        private float GetAddtiveModifier(Stat stat)
        {
            if (!useModifier)
                return 0;
            IModifierProvider[] providers = GetComponents<IModifierProvider>();
            float sum = 0;
            foreach(IModifierProvider provider in providers)
            {
                foreach(float modifier in provider.GetAddtiveModifiers(stat))
                {
                    sum += modifier;
                }
            }

            return sum;
        }

        private int GetLevel()
        {
            return currentLevel.value;
        }

        public int CalculateLevel()
        {
            if (GetComponent<Experience>())
            {
                float currentXP = GetComponent<Experience>().GetExpPoint();
                int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
                for (int level = 1; level <= penultimateLevel; level++)
                {
                    float XpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                    if (currentXP < XpToLevelUp)
                        return level;
                }

                return penultimateLevel + 1;
            }
            else
                return startingLevel;
            
        }
    }
}
