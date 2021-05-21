using UnityEngine;
using Cinemachine;

public class QuaterCamera
{
    private Transform _transform = null;
    private CameraSetting _settings = null;
    private CinemachineVirtualCamera _vcam;
    

    //가상 X축 각도(상하)
    private float _virtualPitchAngle = 0.0f;

    //가상 Y축  각도(좌우)
    private float _virtualRollAngle = 0.0f;

    //가상 거리
    private float _virtualDistance = 0.0f;

    public QuaterCamera(Transform cameraTransform, CameraSetting settings, CinemachineVirtualCamera vcam)
    {
        //CameraSetting 인스펙터의 값들을 이 클래스의 멤버 변수로 저장
        _transform = cameraTransform;
        _settings = settings;
        
        ///
        _vcam = vcam;

        //최대값, 최소값 설정
        _settings.Distance = Mathf.Clamp(_settings.Distance, _settings.LimitDistance.x, _settings.LimitDistance.y );

        _settings.PitchAngle = Mathf.Clamp(_settings.PitchAngle, _settings.LimitPitchAngle.x, _settings.LimitPitchAngle.y);

        //virtual? 마우스를 통해 시점 이동하는 값들을 저장할 변수
        _virtualDistance = _settings.Distance;
        _virtualRollAngle = _settings.RollAngle;
        _virtualPitchAngle = _settings.PitchAngle;
    }

    //외부에서 호출해줄 것임. MonoBehavior를 상속받지 않았기 때문에
    //자력으로 Update가 불가능함
    public void Update()
    {
        if(Input.GetMouseButton(1) == true)
        {
            //마우스 축 값을 누적시켜줌 -> virtual 변수에
            _virtualRollAngle += Input.GetAxis("Mouse X") * _settings.RollRotationSpeed;
            _virtualPitchAngle -= Input.GetAxis("Mouse Y") * _settings.PitchRotationSpeed;
        }

        //마우스 휠 값을 누적 시켜줌
        _virtualDistance -= Input.GetAxis("Mouse ScrollWheel") * _settings.ZoomSpeed;
    }

    //모든 객체(MonoBehavior를 상속받은 객체)의 Update가 끝난 후 호출
    //캐릭터를 먼저 Update에서 움직이고, 그 움직임이 끝난 후 카메라 업데이트
    public void LateUpdate()
    {
        //선형보간식 (1 - t) * A + t * B

        //Y축 회전 각도
        _settings.RollAngle = Mathf.LerpAngle(_settings.RollAngle, _virtualRollAngle, Time.smoothDeltaTime * 3.0f);

        //X축 회전 각도
        _settings.PitchAngle = Mathf.LerpAngle(_settings.PitchAngle, _virtualPitchAngle, Time.smoothDeltaTime * 3.0f);

        //가로, 세로 회전값이 360도를 넘으면 카메라가 뒤집힘 -> 이걸 방지
        if (_settings.RollAngle > 360.0f)
            _settings.RollAngle -= 360.0f; //원래 각도로 돌아오게 됨
        else if (_settings.RollAngle < 0.0f)
            _settings.RollAngle += 360.0f;

        if (_settings.PitchAngle > 360.0f)
            _settings.PitchAngle -= 360.0f; //원래 각도로 돌아오게 됨
        else if (_settings.PitchAngle < 0.0f)
            _settings.PitchAngle += 360.0f;

        //카메라 각도 제한
        _virtualPitchAngle = Mathf.Clamp
        ( 
            _virtualPitchAngle,
            _settings.LimitPitchAngle.x,
            _settings.LimitPitchAngle.y
        );

        //_virtualPitchAngle = Mathf.LerpAngle(_settings.PitchAngle, _virtualPitchAngle, Time.smoothDeltaTime * 3.0f);

        //확대 축소 거리 제한 업데이트
        _settings.Distance = Mathf.Lerp( _settings.Distance, _virtualDistance, Time.smoothDeltaTime);

        _settings.Distance = Mathf.Clamp(_virtualDistance, _settings.LimitDistance.x, _settings.LimitDistance.y);

        _vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = _settings.Distance;

        //Tartget Obj를 따라오는 Follow Camera
        Quaternion q = Quaternion.Euler(_settings.PitchAngle, _settings.RollAngle, 0.0f);
        _transform.position = _settings.TargetObj.transform.position;
        
        // 회전 시킨 방향을 전방 방향으로 향하도록
        _transform.position -= (q * Vector3.forward * _settings.Distance);
        _transform.LookAt(_settings.TargetObj.transform.position);

    }

}
