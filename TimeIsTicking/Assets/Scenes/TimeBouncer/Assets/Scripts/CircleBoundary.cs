using UnityEngine;

/// <summary>
/// 원형 경계 내에서 오브젝트를 제약하는 컴포넌트
/// 
/// 사용 목적:
/// - Ball이 시계 외곽(ClockBorder) 영역을 벗어나지 않도록 수학적으로 제약
/// - ClockBorder는 시각적 표현일 뿐 물리적 Collider 없음
/// - Unity Physics 대신 스크립트로 경계 충돌 처리 (간단하고 정확한 원형 경계)
/// 
/// 충돌 처리 구조:
/// - Ball ↔ Paddle: Unity Physics (CircleCollider2D ↔ BoxCollider2D)
/// - Ball ↔ 시계 외곽: 이 스크립트 (수학적 거리 계산 + 반사)
/// 
/// 적용 대상:
/// - Ball: 필수 (경계 내에서만 움직여야 함)
/// - Paddle: 불필요 (중심 축 회전이라 경계 벗어날 수 없음)
/// </summary>
public class CircleBoundary : MonoBehaviour
{
    [Header("Boundary Settings")]
    [Tooltip("원의 중심점 (시계 중심)")]
    public Transform centerPoint;
    
    [Tooltip("원의 반지름 (시계 테두리까지의 거리)")]
    public float radius = 180f;
    
    [Tooltip("오브젝트의 반지름 (충돌 여유 공간). 0이면 자동 감지")]
    public float objectRadius = 0f;
    
    [Tooltip("자동 감지 여부 (체크 시 Collider 크기를 자동으로 사용)")]
    public bool autoDetectSize = true;
    
    [Header("Physics Settings")]
    [Tooltip("경계에 닿았을 때 반사 처리")]
    public bool bounceOnBoundary = true;
    
    [Tooltip("반사 계수 (0~1, 1은 완전 탄성 충돌)")]
    [Range(0f, 1f)]
    public float bounciness = 0.8f;
    
    private Rigidbody2D rb;
    private Vector2 centerPosition;
    private float detectedRadius;

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
        
        // 오브젝트 크기 자동 감지
        if (autoDetectSize)
        {
            DetectObjectSize();
        }
        else
        {
            detectedRadius = objectRadius;
        }
        
        Debug.Log($"{gameObject.name}: CircleBoundary 설정 - Radius: {radius}, Object Radius: {detectedRadius}, Effective Radius: {radius - detectedRadius}");
    }
    
    void DetectObjectSize()
    {
        // CircleCollider2D가 있으면 그것을 사용
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider != null)
        {
            detectedRadius = circleCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y);
            Debug.Log($"{gameObject.name}: CircleCollider2D 감지 - Radius: {detectedRadius}");
            return;
        }
        
        // BoxCollider2D가 있으면 대각선의 절반을 사용
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Vector2 size = boxCollider.size;
            size.x *= transform.localScale.x;
            size.y *= transform.localScale.y;
            detectedRadius = size.magnitude / 2f;
            Debug.Log($"{gameObject.name}: BoxCollider2D 감지 - Size: {size}, Radius: {detectedRadius}");
            return;
        }
        
        // SpriteRenderer가 있으면 스프라이트 크기 사용
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
            spriteSize.x *= transform.localScale.x;
            spriteSize.y *= transform.localScale.y;
            detectedRadius = spriteSize.magnitude / 2f;
            Debug.Log($"{gameObject.name}: SpriteRenderer 감지 - Size: {spriteSize}, Radius: {detectedRadius}");
            return;
        }
        
        // 아무것도 없으면 수동 설정값 사용
        detectedRadius = objectRadius > 0 ? objectRadius : 10f;
        Debug.LogWarning($"{gameObject.name}: Collider를 찾을 수 없어 기본값 사용 - Radius: {detectedRadius}");
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
        float effectiveRadius = radius - detectedRadius;
        
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
        float displayRadius = (autoDetectSize && Application.isPlaying) ? detectedRadius : objectRadius;
        
        // 외부 경계선 (시계 테두리)
        Gizmos.color = Color.yellow;
        DrawCircle(center, radius, 64);
        
        // 유효 경계선 (오브젝트 중심이 갈 수 있는 최대 범위)
        Gizmos.color = Color.green;
        DrawCircle(center, radius - displayRadius, 64);
        
        // 현재 오브젝트 위치
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, displayRadius);
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
