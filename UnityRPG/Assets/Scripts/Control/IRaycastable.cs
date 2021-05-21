using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{ 
    public interface IRayCastable
    {
        CursorType GetCursorType();
        bool HandleRaycast(PlayerController playerController);
    }
}
