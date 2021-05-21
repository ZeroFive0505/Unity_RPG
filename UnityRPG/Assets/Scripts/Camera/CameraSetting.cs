using UnityEngine;

public class CameraSetting : MonoBehaviour
{
    [SerializeField]
    private GameObject _targetObj = null; //큐브가 들어올 자리.. 3rd 카메라로 움직일 대상(게임오브젝트 타입은 다 들어옴)
    public GameObject TargetObj
    {
        get { return _targetObj; }
    }

    private Camera _camera = null;
    public Camera Camera //메인 카메라에 보면 카메라 컴포넌트가 있는데 그걸 가져다 쓰려고 함
    {
        get { return _camera; }
    }

    /// <summary>
    /// 시작 Y축 각도 -> 즉, 좌우 카메라
    /// </summary>
    [SerializeField]
    private float _rollAngle = 0.0f;
    public float RollAngle
    {
        get { return _rollAngle; }
        set { _rollAngle = value; }
    }

    /// <summary>
    /// 시작 X축 각도 -> 즉, 상하 카메라
    /// </summary>
    [SerializeField]
    private float _pitchAngle = 65.0f;
    public float PitchAngle
    {
        get { return _pitchAngle; }
        set { _pitchAngle = value; }
    }

    /// <summary>
    /// 타겟까지의 거리
    /// </summary>
    [SerializeField]
    private float _distance = 5.0f;
    public float Distance
    {
        get { return _distance; }
        set { _distance = value; }
    }

    /// <summary>
    /// 확대 축소 속도
    /// </summary>
    private float _distanceLerpSpeed = 4.5f;
    public float DistanceLerpSpeed
    {
        get { return _distanceLerpSpeed; }
    }

    /// <summary>
    /// 카메라 확대 축소
    /// </summary>
    [SerializeField]
    private float _zoomSpeed = 25.0f;
    public float ZoomSpeed
    {
        get { return _zoomSpeed; }
    }

    /// <summary>
    /// Y축 회전속도
    /// </summary>
    [SerializeField]
    private float _rollRotationSpeed = 3.0f;
    public float RollRotationSpeed
    {
        get { return _rollRotationSpeed; }
        set { _rollRotationSpeed = value; }
    }

    /// <summary>
    /// X축 회전속도
    /// </summary>
    [SerializeField]
    private float _pitchRotationSpeed = 20.0f;
    public float PitchRotationSpeed
    {
        get { return _pitchRotationSpeed; }
        set { _pitchRotationSpeed = value; }
    }

    /// <summary>
    /// 최소 최대 거리 
    /// </summary>
    [SerializeField]
    private Vector2 _limitDistance = new Vector2(1.0f, 10.0f); //카메라로 다가갈 수 있는 최대 거리(말만 x, y지 min, max 용도로 사용)
    public Vector2 LimitDistance
    {
        get { return _limitDistance; }
    }

    /// <summary>
    /// 최소 최대 회전각도 -> 너무 위, 아래로 카메라가 들어도록 하지 않도록 제한
    /// </summary>
    [SerializeField]
    private Vector2 _limitPitchAngle = new Vector2(10.0f, 90.0f); //카메라가 멀어질 수 있는 최대 거리
    public Vector2 LimitPitchAngle
    {
        get { return _limitPitchAngle; }
    }


    private void Awake() //MonoBehavior 상속 받으면 생성자 사용 불가능
    {
        _camera = this.GetComponent<Camera>(); //컴포넌트에 카메라를 붙여놓으면 GetComponent를 통해 객체를 가져올 수 있음
    }
}
