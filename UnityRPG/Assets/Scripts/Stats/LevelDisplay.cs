using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {

        public Text lvText;
        public BaseStats baseStats;

        private void Awake()
        {
            lvText = GetComponent<Text>();
            baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            lvText.text = "Lv : " + baseStats.CalculateLevel();
        }
    }

}