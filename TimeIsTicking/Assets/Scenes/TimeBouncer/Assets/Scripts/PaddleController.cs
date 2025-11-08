using UnityEngine;

/// <summary>
/// 플레이어가 조종하는 패들(시계 침)
/// 중심점을 기준으로 회전하며 공을 쳐냅니다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PaddleController : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("회전 속도")]
    public float rotationSpeed = 180f; // 초당 180도
    
    [Tooltip("키보드 입력 사용 (false면 AI용)")]
    public bool useKeyboardInput = true;
    
    [Header("References")]
    [Tooltip("회전 중심점 (시계 중심)")]
    public Transform rotationCenter;
    
    private Rigidbody2D rb;
    private float currentAngle = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Rigidbody2D 설정
        rb.bodyType = RigidbodyType2D.Kinematic; // 중력 영향 받지 않음
        rb.freezeRotation = false; // 회전 가능
        
        if (rotationCenter == null)
        {
            Debug.LogWarning($"{gameObject.name}: rotationCenter가 설정되지 않았습니다.");
        }
        
        // 현재 각도 초기화
        currentAngle = transform.eulerAngles.z;
    }

    void Update()
    {
        if (useKeyboardInput)
        {
            HandleKeyboardInput();
        }
    }

    void FixedUpdate()
    {
        // 회전 적용
        ApplyRotation();
    }

    /// <summary>
    /// 키보드 입력 처리
    /// </summary>
    void HandleKeyboardInput()
    {
        float input = 0f;
        
        // 방향키 또는 A/D 키
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            input = 1f; // 반시계방향
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            input = -1f; // 시계방향
        }
        
        // 각도 업데이트
        currentAngle += input * rotationSpeed * Time.deltaTime;
    }

    /// <summary>
    /// AI가 목표 각도를 설정할 때 사용
    /// </summary>
    public void SetTargetAngle(float targetAngle)
    {
        currentAngle = targetAngle;
    }

    /// <summary>
    /// AI가 회전 방향을 제어할 때 사용 (-1 ~ 1)
    /// </summary>
    public void SetRotationInput(float input)
    {
        currentAngle += Mathf.Clamp(input, -1f, 1f) * rotationSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 회전 적용
    /// </summary>
    void ApplyRotation()
    {
        // Z축 회전만 적용
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
        
        // 중심점이 설정되어 있다면 위치도 조정 (선택사항)
        if (rotationCenter != null)
        {
            // 침이 중심점을 기준으로 회전하도록 위치 조정
            // 이 부분은 침의 Pivot 위치에 따라 필요 없을 수도 있습니다
        }
    }

    /// <summary>
    /// 현재 각도 반환 (AI용)
    /// </summary>
    public float GetCurrentAngle()
    {
        return currentAngle;
    }

    /// <summary>
    /// 회전 속도 반환 (AI용)
    /// </summary>
    public float GetAngularVelocity()
    {
        return rb.angularVelocity;
    }

    // 디버그 정보
    void OnGUI()
    {
        if (!useKeyboardInput) return;
        
        GUIStyle style = new GUIStyle();
        style.fontSize = 16;
        style.normal.textColor = Color.white;
        
        GUI.Label(new Rect(10, 10, 300, 30), $"각도: {currentAngle:F1}°", style);
        GUI.Label(new Rect(10, 35, 300, 30), "조작: ← → 또는 A D", style);
    }
}
