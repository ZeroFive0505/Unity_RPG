using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageText;
        [SerializeField] float destroyTime = 2.5f;
        [SerializeField] Text text;
      

        public void Spawn(float damageAmount)
        {
            GameObject textFX = Instantiate(damageText.gameObject, transform);
            textFX.GetComponent<DamageText>().SetValue(damageAmount);
            Destroy(textFX, destroyTime);
        }
    }
}


