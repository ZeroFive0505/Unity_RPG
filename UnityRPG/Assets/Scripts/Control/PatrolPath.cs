using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            for(int i = 0; i < transform.childCount; i++)
            {
                Vector3 from = GetWayPoint(i);
                Vector3 to = GetWayPoint((i + 1) % transform.childCount);
                Gizmos.DrawSphere(transform.GetChild(i).transform.position, 0.5f);
                Gizmos.DrawLine(from, to);
            }
        }

        public Vector3 GetWayPoint(int index)
        {
            return transform.GetChild(index).position;
        }
    }
}
