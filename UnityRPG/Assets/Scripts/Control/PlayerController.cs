using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Mover mover;
        private Fighter fighter;

       

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Vector2 hotSpot;
            public Texture2D texture;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavmeshProjectionDistance = 2.0f;
        [SerializeField] float raycastRadius = 0.5f;
 

        private void Awake()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (InteractWithUI())
            {
                SetCursor(CursorType.UI);
                return;
            }

            

            if(!GetComponent<Health>().Died())
            {
                if (InteractWithComponent())
                    return;
                if (InteractWithCombat())
                    return;
                if (InteractWithMovement())
                    return;
                SetCursor(CursorType.None);
            }
            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach(RaycastHit hit in hits)
            {
                IRayCastable[] rayCastables = hit.transform.GetComponents<IRayCastable>();
                
                foreach(IRayCastable ray in rayCastables)
                {
                    if(ray.HandleRaycast(this))
                    {
                        SetCursor(ray.GetCursorType());
                        return true;
                    }
                }
            }

            return false;
        }

        private bool InteractWithUI()
        {
            return EventSystem.current.IsPointerOverGameObject(); //UI object
        }

        private bool InteractWithMovement()
        {


            //RaycastHit hit;
            //bool hashit = Physics.Raycast(GetMouseRay(), out hit);

            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);

            if (hasHit)
            {
                if (GetComponent<Mover>().CanMoveTo(target))
                {
                    if (Input.GetMouseButton(0))
                    {
                        mover.StartAction(target, 1.0f);
                    }
                    SetCursor(CursorType.Movement);
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            RaycastHit raycastHit;
            target = new Vector3();
            bool hasHit = Physics.Raycast(GetMouseRay(), out raycastHit);
            NavMeshHit navmeshHit;
            if(hasHit)
            {
                if (NavMesh.SamplePosition(raycastHit.point, out navmeshHit, maxNavmeshProjectionDistance, NavMesh.AllAreas))
                {
                    target = navmeshHit.position;

                    if (GetComponent<Mover>().CanMoveTo(target))
                        return true;
                    else
                        return false;
                }
            }
            
            return false;
        }

        

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);

            float[] distances = new float[hits.Length];

            for (int i = 0; i < distances.Length; i++)
                distances[i] = hits[i].distance;

            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = RaycastAllSorted();
            
            foreach (RaycastHit hit in hits)
            {

                IRayCastable[] rayCastables = hit.transform.GetComponents<IRayCastable>();

                foreach(IRayCastable ray in rayCastables)
                {
                    if(ray.HandleRaycast(this))
                    {
                        SetCursor(ray.GetCursorType());
                        return true;
                    }
                }
               
            }
            return false;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotSpot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach(CursorMapping mapping in cursorMappings)
            {
                if(mapping.type == type)
                {
                    return mapping;
                }
            }

            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
