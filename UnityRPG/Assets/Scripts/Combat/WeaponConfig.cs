using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName ="Weapons/Make new weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController weaponOverride = null;
        [SerializeField] GameObject primaryWeaponPrefab = null;
        [SerializeField] GameObject secondaryWeaponPrefab = null;
        [SerializeField] float weaponRange = 3.5f;
        [SerializeField] float damage = 10.0f;
        [SerializeField] float percentageBonus = 0;
        [SerializeField] bool IsRightHand = true;
        [SerializeField] Projectile projectile = null;
   
        
        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            GameObject obj = null;
            if(rightHand.childCount != 0)
            {
                foreach (Transform child in rightHand)
                    Destroy(child.gameObject);
            }

            if(leftHand.childCount != 0)
            {
                foreach (Transform child in leftHand)
                    Destroy(child.gameObject);
            }

            if (primaryWeaponPrefab != null)
            {
                if (IsRightHand)
                {
                    obj = Instantiate(primaryWeaponPrefab, rightHand);
                    if (secondaryWeaponPrefab)
                        Instantiate(secondaryWeaponPrefab, leftHand);
                }
                else
                {
                    obj = Instantiate(primaryWeaponPrefab, leftHand);
                    if (secondaryWeaponPrefab)
                        Instantiate(secondaryWeaponPrefab, rightHand);
                }
            }

            animator.runtimeAnimatorController = weaponOverride;

            if (obj)
                return obj.GetComponent<Weapon>();
            else
                return null;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject owner, float factor)
        {
            CapsuleCollider capsuleCollider = target.GetComponent<CapsuleCollider>();
            
            Projectile projectileInstance;
            if (IsRightHand)
            {
                projectileInstance = Instantiate(projectile, rightHand.transform.position + rightHand.transform.forward * capsuleCollider.radius, Quaternion.identity);
                
            }
            else
            {
                projectileInstance = Instantiate(projectile, leftHand.transform.position + leftHand.transform.forward * capsuleCollider.radius, Quaternion.identity);
            }

            projectileInstance.SetTarget(target, damage + factor, owner);
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public float GetWeaponDamage()
        {
            return damage;
        }
    }
}
