using UnityEngine;

/// <summary>
/// 원형 경계 내에서 오브젝트를 제약하는 컴포넌트
/// 공, 시침 등의 오브젝트가 시계 테두리 밖으로 나가지 않도록 합니다.
/// </summary>
public class CircleBoundary : MonoBehaviour
{
    [Header("Boundary Settings")]
    [Tooltip("원의 중심점 (시계 중심)")]
    public Transform centerPoint;
    
    [Tooltip("원의 반지름 (시계 테두리까지의 거리)")]
    public float radius = 180f;
    
    [Tooltip("오브젝트의 반지름 (충돌 여유 공간)")]
    public float objectRadius = 10f;
    
    [Header("Physics Settings")]
    [Tooltip("경계에 닿았을 때 반사 처리")]
    public bool bounceOnBoundary = true;
    
    [Tooltip("반사 계수 (0~1, 1은 완전 탄성 충돌)")]
    [Range(0f, 1f)]
    public float bounciness = 0.8f;
    
    private Rigidbody2D rb;
    private Vector2 centerPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (centerPoint == null)
        {
            // 중심점이 설정되지 않았다면 (0,0)을 중심으로 사용
            centerPosition = Vector2.zero;
            Debug.LogWarning($"{gameObject.name}: centerPoint가 설정되지 않아 (0,0)을 중심으로 사용합니다.");
        }
        else
        {
            centerPosition = centerPoint.position;
        }
    }

    void Update()
    {
        // 중심점이 움직일 수 있는 경우를 대비
        if (centerPoint != null)
        {
            centerPosition = centerPoint.position;
        }
        
        ConstrainToCircle();
    }

    void ConstrainToCircle()
    {
        Vector2 currentPosition = transform.position;
        Vector2 directionFromCenter = currentPosition - centerPosition;
        float distanceFromCenter = directionFromCenter.magnitude;
        
        // 유효 반지름 (오브젝트 크기 고려)
        float effectiveRadius = radius - objectRadius;
        
        // 경계를 벗어났는지 확인
        if (distanceFromCenter > effectiveRadius)
        {
            // 경계 안으로 위치 보정
            Vector2 normalizedDirection = directionFromCenter.normalized;
            Vector2 boundaryPosition = centerPosition + normalizedDirection * effectiveRadius;
            transform.position = boundaryPosition;
            
            // 물리 엔진 사용 시 속도 반사 처리
            if (bounceOnBoundary && rb != null)
            {
                // 현재 속도를 경계 법선 벡터에 대해 반사
                Vector2 velocity = rb.linearVelocity;
                Vector2 normal = -normalizedDirection; // 경계의 법선 (안쪽 방향)
                
                // 반사 공식: v' = v - 2(v·n)n
                Vector2 reflectedVelocity = velocity - 2 * Vector2.Dot(velocity, normal) * normal;
                
                // 반사 계수 적용
                rb.linearVelocity = reflectedVelocity * bounciness;
            }
        }
    }

    // 디버그용 기즈모
    void OnDrawGizmos()
    {
        Vector3 center = centerPoint != null ? centerPoint.position : Vector3.zero;
        
        // 외부 경계선 (시계 테두리)
        Gizmos.color = Color.yellow;
        DrawCircle(center, radius, 64);
        
        // 유효 경계선 (오브젝트 중심이 갈 수 있는 최대 범위)
        Gizmos.color = Color.green;
        DrawCircle(center, radius - objectRadius, 64);
        
        // 현재 오브젝트 위치
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, objectRadius);
    }

    void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
