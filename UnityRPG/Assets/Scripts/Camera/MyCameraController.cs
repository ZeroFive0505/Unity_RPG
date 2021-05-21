using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

//굳이 이걸 나눠서 만든 이유?
//어떤 CameraSetting이 들어오더라도 실행
public class MyCameraController : MonoBehaviour
{
    private QuaterCamera _quater = null;

    private CinemachineVirtualCamera vcam;
  

    void Awake()
    {
        CameraSetting settings = this.GetComponent<CameraSetting>();
        vcam = GetComponent<CinemachineVirtualCamera>();
        _quater = new QuaterCamera(this.transform, settings, vcam);
    }

    // Update is called once per frame
    void Update()
    {
        _quater.Update();
    }

    private void LateUpdate()
    {
        _quater.LateUpdate();
    }
}
