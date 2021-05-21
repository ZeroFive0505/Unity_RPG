using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.Playables;
using RPG.Control;
using RPG.Core;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] int sceneNum = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeInTime = 1.5f;
        [SerializeField] float fadeOutTime = 3.0f;
        [SerializeField] float fadeWaitTime = 0.8f;
        [SerializeField] GameObject player;

        public Action onLoad;

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                StartCoroutine(AsyncLoadScene());
            }
        }

  

        private IEnumerator AsyncLoadScene()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (sceneNum < 0)
            {
                Debug.LogError("Scene to load is not set");
                yield break;
            }
            Fader fader = FindObjectOfType<Fader>();
            DontDestroyOnLoad(this);


            //Remove Player Control
            PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;
            yield return fader.FadeOut(fadeOutTime);


            //Save Current Level
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Save();
            yield return SceneManager.LoadSceneAsync(sceneNum);
            //Remove control
            PlayerController newPlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;

            //Load Current Level
            wrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            wrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);

            fader.FadeIn(fadeInTime);


            //Restore Control
            newPlayerController.enabled = true;
            Destroy(this);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.rotation = spawnPoint.transform.rotation;
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
        }

        private Portal GetOtherPortal()
        {
            foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this)
                    continue;
                if (portal.destination != destination)
                    continue;

                return portal;
            }

            return null;
        }
    }
}


