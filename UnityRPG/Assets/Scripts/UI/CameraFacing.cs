using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class CameraFacing : MonoBehaviour
    {

        // Update is called once per frame
        void LateUpdate()
        {
            transform.LookAt(Camera.main.transform);
        }
    }

}