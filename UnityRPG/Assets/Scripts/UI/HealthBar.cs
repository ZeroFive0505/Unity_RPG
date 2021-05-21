
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health health;
        [SerializeField] Vector3 scale;
        [SerializeField] Canvas canvas;

        // Start is called before the first frame update
        void Start()
        {
            health = GetComponentInParent<Health>();
            canvas.enabled = false;
            scale = transform.localScale;
        }

        public void UpdateHealthBar()
        {
            if(health.GetCurrentHealth() > 0.0f)
            {
                canvas.enabled = true;
                scale.x = health.GetFraction();
                transform.localScale = scale;
            }
            else if(health.GetCurrentHealth() <= 0.0f)
            {
                scale.x = 0;
                canvas.enabled = false;
                transform.localScale = scale;
            }
        }
    }


}
