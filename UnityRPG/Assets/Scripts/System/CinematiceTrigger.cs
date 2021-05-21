using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Saving;

namespace RPG.Cinematic
{
    
    public class CinematiceTrigger : MonoBehaviour, ISaveable
    {
        public bool bTrigger = false;

        public object CaptureState()
        {
            return bTrigger;
        }

        public void RestoreState(object state)
        {
            bTrigger = (bool)state;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                if (bTrigger == false)
                {
                    GetComponent<PlayableDirector>().Play();
                    bTrigger = true;
                }
            }
        }
    }


}
