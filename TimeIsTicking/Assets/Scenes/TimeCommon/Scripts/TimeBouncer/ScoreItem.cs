using UnityEngine;

/// <summary>
/// 스코어 아이템 개별 오브젝트 (Ball이 충돌하면 점수 획득)
/// GameManager가 자동으로 추가하므로 수동 설정 불필요
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class ScoreItem : MonoBehaviour
{
    [Header("Item Settings")]
    [Tooltip("아이템 타입 (자동 설정됨)")]
    public ScoreItemType itemType = ScoreItemType.Normal;
    
    [Tooltip("획득 점수 (자동 설정됨)")]
    public int scoreValue = 1;
    
    [Header("Collision Settings")]
    [Tooltip("트리거 작동 최대 거리 (Collider 반지름 기준 배수)")]
    [Range(0.5f, 3.0f)]
    public float maxTriggerDistanceMultiplier = 1.2f;
    
    [Tooltip("고정 여유 거리 (추가 허용 거리)")]
    [Range(0f, 2f)]
    public float fixedMarginDistance = 0.3f;
    
    // 참조
    private CircleCollider2D itemCollider;
    private SpriteRenderer spriteRenderer;
    private ScoreManager scoreManager;
    
    public void Initialize(ScoreItemType type, int score, Color color, ScoreManager manager)
    {
        itemType = type;
        scoreValue = score;
        scoreManager = manager;
        
        // 기존 Collider 찾기 (프리팹에 있을 수 있음)
        itemCollider = GetComponent<CircleCollider2D>();
        
        if (itemCollider != null)
        {
            // 프리팹에 이미 CircleCollider2D가 있는 경우
            Debug.Log($"✅ 프리팹의 기존 CircleCollider2D 사용 - 반지름: {itemCollider.radius}");
        }
        else
        {
            // 다른 타입의 Collider2D가 있는지 확인
            Collider2D existingCollider = GetComponent<Collider2D>();
            if (existingCollider != null)
            {
                Debug.Log($"✅ 프리팹의 기존 {existingCollider.GetType().Name} 사용");
                // CircleCollider2D가 아닌 경우에도 활용 가능
            }
            else
            {
                // 아무 Collider도 없는 경우에만 새로 생성
                itemCollider = gameObject.AddComponent<CircleCollider2D>();
                itemCollider.radius = 0.5f;  // 기본 반지름 설정
                Debug.Log($"⚠️ 새 CircleCollider2D 생성 - 반지름: {itemCollider.radius}");
            }
        }
        
        // 모든 Collider2D를 Trigger로 설정
        Collider2D[] allColliders = GetComponents<Collider2D>();
        foreach (var collider in allColliders)
        {
            collider.isTrigger = true;
            Debug.Log($"🔧 {collider.GetType().Name} → Trigger 설정 완료");
        }
        
        // 최종 사용할 Collider 확정
        if (itemCollider == null)
        {
            itemCollider = GetComponent<CircleCollider2D>();
        }
        
        // 시각적 표현
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
            Debug.Log($"🎨 SpriteRenderer 색상 변경: {color}");
        }
        else
        {
            Debug.LogWarning($"⚠️ SpriteRenderer를 찾을 수 없습니다 - {gameObject.name}");
        }
        
        // 충돌 설정 검증
        Debug.Log($"🔍 ScoreItem 충돌 설정 검증:");
        Debug.Log($"   - Layer: {gameObject.layer} ({LayerMask.LayerToName(gameObject.layer)})");
        Debug.Log($"   - Tag: {gameObject.tag}");
        Debug.Log($"   - Position: {transform.position}");
        
        Debug.Log($"✅ ScoreItem 초기화 완료 - Type: {itemType}, Score: {scoreValue}, " +
                  $"Collider: {itemCollider?.GetType().Name}, Radius: {itemCollider?.radius}");
        Debug.Log($"   - 최대 트리거 거리: {CalculateMaxTriggerDistance():F2}");
    }
    
    /// <summary>
    /// 트리거 작동 최대 거리 계산
    /// </summary>
    private float CalculateMaxTriggerDistance()
    {
        float baseRadius = GetEffectiveColliderRadius();
        float maxDistance = (baseRadius * maxTriggerDistanceMultiplier) + fixedMarginDistance;
        
        // 최소 거리 보장 (너무 작으면 충돌이 어려움)
        return Mathf.Max(maxDistance, 0.5f);
    }
    
    /// <summary>
    /// 현재 오브젝트의 유효한 충돌 반지름 계산
    /// </summary>
    private float GetEffectiveColliderRadius()
    {
        // CircleCollider2D가 있는 경우
        if (itemCollider != null)
        {
            return itemCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y);
        }
        
        // 다른 Collider2D 타입들 처리
        Collider2D anyCollider = GetComponent<Collider2D>();
        if (anyCollider != null)
        {
            Bounds bounds = anyCollider.bounds;
            return Mathf.Max(bounds.size.x, bounds.size.y) * 0.5f;
        }
        
        // Collider가 없는 경우 기본값
        return 0.5f;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Ball과 충돌 확인
        if (other.CompareTag("Ball") && scoreManager != null)
        {
            // 거리 계산
            float distance = Vector2.Distance(transform.position, other.transform.position);
            float maxAllowedDistance = CalculateMaxTriggerDistance();
            
            Debug.Log($"🔍 Ball-ScoreItem 거리 체크:");
            Debug.Log($"   - 실제 거리: {distance:F2}");
            Debug.Log($"   - 최대 허용 거리: {maxAllowedDistance:F2}");
            Debug.Log($"   - ScoreItem 위치: {transform.position}");
            Debug.Log($"   - Ball 위치: {other.transform.position}");
            
            // 거리 판정
            if (distance <= maxAllowedDistance)
            {
                Debug.Log($"✅ Ball과 ScoreItem 정상 충돌 - 거리: {distance:F2} <= {maxAllowedDistance:F2}");
                
                // 점수 획득
                scoreManager.CollectItem(this);
                
                // 아이템 제거
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning($"⚠️ Ball과 ScoreItem 거리 초과로 무시됨 - 거리: {distance:F2} > {maxAllowedDistance:F2}");
            }
        }
        else if (other.CompareTag("Player"))
        {
            Debug.LogWarning($"⚠️ Paddle이 스코어 아이템에 충돌 - 무시됨");
        }
        else
        {
            Debug.Log($"ℹ️ 기타 오브젝트와 충돌 - {other.name} (태그: {other.tag})");
        }
    }
    

}
