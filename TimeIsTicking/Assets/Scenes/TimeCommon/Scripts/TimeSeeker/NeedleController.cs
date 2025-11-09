using UnityEngine;

/// <summary>
/// 바늘(시침) 컨트롤러
/// 중심점을 기준으로 회전하며 Arc 오브젝트를 추적합니다.
/// </summary>
public class NeedleController : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("회전 속도 (도/초)")]
    public float rotationSpeed = 180f;
    
    [Tooltip("플레이어 입력 사용")]
    public bool usePlayerInput = true;
    
    [Header("References")]
    [Tooltip("회전 중심점")]
    public Transform rotationCenter;
    
    private float currentAngle = 0f;
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // Rigidbody2D 설정
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = false;
        
        if (rotationCenter == null)
        {
            Debug.LogWarning($"{gameObject.name}: rotationCenter가 설정되지 않았습니다.");
        }
        
        // 현재 각도 초기화
        currentAngle = transform.eulerAngles.z;
    }
    
    /// <summary>
    /// 외부에서 각도 설정 (GameManager가 사용)
    /// </summary>
    public void SetAngle(float angle)
    {
        currentAngle = angle;
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
    
    /// <summary>
    /// 현재 각도 가져오기
    /// </summary>
    public float GetCurrentAngle()
    {
        return currentAngle;
    }
    
    /// <summary>
    /// 회전 입력 적용 (GameManager가 사용)
    /// </summary>
    public void ApplyRotationInput(float input)
    {
        currentAngle += input * rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
}
