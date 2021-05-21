using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Stats
{

    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/Progression", order = 0)]

    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookUpTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {

            BuildLookUp();
            //foreach(ProgressionCharacterClass progressionCharacter in characterClasses)
            //{
            //    if (progressionCharacter.characterClass != characterClass)
            //        continue;

            //    foreach(ProgressionStat progressionStat in progressionCharacter.stats)
            //    {
            //        if (progressionStat.stat != stat)
            //            continue;

            //        if (progressionStat.levels.Length < level)
            //            continue;

            //        return progressionStat.levels[level - 1];
            //    }
            //}
            //return 0;
            float[] levels = lookUpTable[characterClass][stat];

            if (levels.Length < level)
                return 0;

            return levels[level - 1];
        }

        private void BuildLookUp()
        {
            if (lookUpTable != null)
                return;

            lookUpTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionCharacter in characterClasses)
            {
                var statLookUpTable = new Dictionary<Stat, float[]>();

                foreach(ProgressionStat stat in progressionCharacter.stats)
                {
                    statLookUpTable[stat.stat] = stat.levels;
                }

                lookUpTable[progressionCharacter.characterClass] = statLookUpTable;
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }


        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookUp();

            float[] levels = lookUpTable[characterClass][stat];

            return levels.Length;
        }

    }
}
