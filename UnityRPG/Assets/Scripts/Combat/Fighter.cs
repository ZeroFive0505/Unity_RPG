using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Attributes;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using GameDevTV.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    { 
        [SerializeField] float weaponOffset = 5f;
        [SerializeField] float timeBetweenAttack = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig[] weapons = null;
        [SerializeField] int weaponIndex = 0;


        LazyValue<Weapon> currentWeapon;



        private Transform target;
        float timeSinceLastAttck = 0;

        private void Start()
        {
            Equip(weapons, weaponIndex);
            currentWeapon = new LazyValue<Weapon>(SetUpDefaultWeapon);
        }

        private Weapon SetUpDefaultWeapon()
        {
            Weapon weapon = Equip(weapons, weaponIndex);

            return weapon;
        }

        private void Update()
        {

            if(gameObject.tag == "Player")
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    currentWeapon.value = Equip(weapons, 0);
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                    currentWeapon.value = Equip(weapons, 1);
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                    currentWeapon.value = Equip(weapons, 2);
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                    currentWeapon.value = Equip(weapons, 3);
            }

            if(target != null)
            {
                if (target.GetComponent<Health>().Died())    
                    return;

                if ((Vector3.Distance(transform.position, target.transform.position) > weapons[weaponIndex].GetWeaponRange() + weaponOffset))
                    GetComponent<Animator>().SetBool("Unarmed Idle", false);
                else
                    GetComponent<Animator>().SetBool("Unarmed Idle", true);
                if ((Vector3.Distance(transform.position, target.transform.position) > weapons[weaponIndex].GetWeaponRange()))
                {
                    GetComponent<Mover>().MoveToPoint(target.position, 1.0f);
                }
                else
                {
                    GetComponent<Mover>().Cancel();
                    AttackBehavior();
                }
            }
      
            timeSinceLastAttck += Time.deltaTime;
        }

        private Weapon Equip(WeaponConfig[] weapons, int index)
        {
            if (weapons[index] == null)
                return null;
            weaponIndex = index;
            Animator animator = GetComponent<Animator>();

            Weapon weapon = weapons[index].Spawn(rightHandTransform, leftHandTransform, animator);

            return weapon;
        }

     

        private void AttackBehavior()
        {

            Vector3 LookAtGoal = new Vector3(target.position.x, transform.position.y, target.position.z);
            Vector3 Dir = LookAtGoal - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Dir), 3.0f);

            if (timeSinceLastAttck > timeBetweenAttack)
            {
                ChooseUnarmedAnim();
                timeSinceLastAttck = 0;
            }
        }

        private void ChooseUnarmedAnim()
        {
            int randomMovement = UnityEngine.Random.Range(1, 10);
            switch (randomMovement)
            {
                case 1:
                    GetComponent<Animator>().SetTrigger("Left Kick 1");
                    break;
                case 2:
                    GetComponent<Animator>().SetTrigger("Left Kick 2");
                    break;
                case 3:
                    GetComponent<Animator>().SetTrigger("Right Kick 1");
                    break;
                case 4:
                    GetComponent<Animator>().SetTrigger("Right Kick 2");
                    break;
                case 5:
                    GetComponent<Animator>().SetTrigger("Right Punch 1");
                    break;
                case 6:
                    GetComponent<Animator>().SetTrigger("Right Punch 2");
                    break;
                case 7:
                    GetComponent<Animator>().SetTrigger("Right Punch 3");
                    break;
                case 8:
                    GetComponent<Animator>().SetTrigger("Left Punch 1");
                    break;
                case 9:
                    GetComponent<Animator>().SetTrigger("Left Punch 2");
                    break;
                case 10:
                    GetComponent<Animator>().SetTrigger("Left Punch 3");
                    break;
            }
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.transform;
        }

        //Animation Event
        public void Hit()
        {
            if(target)
            {

                if(currentWeapon.value != null)
                {
                    currentWeapon.value.OnHit();
                }

                float factor = GetComponent<BaseStats>().GetStat(Stat.Damage);
                if(weapons[weaponIndex].HasProjectile())
                {
                    weapons[weaponIndex].LaunchProjectile(rightHandTransform, leftHandTransform, target.GetComponent<Health>(), gameObject, factor);
                }
                else
                    target.GetComponent<Health>().TakeDamage(weapons[weaponIndex].GetWeaponDamage() + factor, gameObject);
            }
        }

        public void Shoot()
        {
            Hit();
        }

        public void Cancel()
        {
            GetComponent<Animator>().SetTrigger("StopAttacking");
            GetComponent<Mover>().Cancel();
            target = null;
        }

        public bool CanAttack()
        {

            if (GetComponent<Health>().Died() || 
                (!GetComponent<Mover>().CanMoveTo(transform.position) 
                && !(Vector3.Distance(target.transform.position, transform.position) < weapons[weaponIndex].GetWeaponRange())))
                return false;
            else
                return true;

        }

        public WeaponConfig GetWeapon()
        {
            return weapons[weaponIndex];
        }

        public object CaptureState()
        {
            return weaponIndex;
        }

        public void RestoreState(object state)
        {
            int index = (int)state;
            weaponIndex = index;
            //Equip(weapons, index);
        }

        public Transform GetTarget()
        {
            if (target)
                return target;
            else
                return null;
        }

        public IEnumerable<float> GetAddtiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return weapons[weaponIndex].GetWeaponDamage();
            }
        }

     

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return weapons[weaponIndex].GetPercentageBonus();
            }
        }
    }
}


