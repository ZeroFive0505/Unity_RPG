using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRayCastable
    {
        public CursorType GetCursorType()
        {
            if (GetComponent<Fighter>().CanAttack())
                return CursorType.Combat;
            else
                return CursorType.Movement;
        }

        public bool HandleRaycast(PlayerController playerController)
        {
            if (Input.GetMouseButton(0))
            {
                if (GetComponent<Fighter>().CanAttack())
                {
                    playerController.GetComponent<Fighter>().Attack(transform.gameObject);
                    return true;
                }
                else
                    return false;
            }

            else
                return true;
        }
    }

}