using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencPoints = 0;

        public Action onExperienceGained;

        public object CaptureState()
        {
            return experiencPoints;
        }

        public void EarnExp(float exp)
        {
            experiencPoints += exp;
            onExperienceGained();
        }

        public void RestoreState(object state)
        {
            experiencPoints = (float)state;
        }

        public float GetExpPoint()
        {
            return experiencPoints;
        }
    }

}