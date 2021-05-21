using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RPG.Stats
{
    public class ExpDisplay : MonoBehaviour
    {
        public Experience experience;
        public Text ExpText;

        private void Awake()
        {
            ExpText = GetComponent<Text>();
            experience = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            ExpText.text = "EXP : " + experience.GetExpPoint();
        }
    }

}
