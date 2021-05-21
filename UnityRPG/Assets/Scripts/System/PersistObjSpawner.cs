using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Core
{
    public class PersistObjSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persisObjPrefab;

        static bool hasSpawned = false;

        private void Awake()
        {
            if (hasSpawned)
                return;

            SpawnPersisObj();

            hasSpawned = true;
        }

        private void SpawnPersisObj()
        {
            GameObject obj = Instantiate(persisObjPrefab);
            DontDestroyOnLoad(obj);
        }
    }

}