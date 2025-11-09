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
    
    // 참조
    private CircleCollider2D itemCollider;
    private SpriteRenderer spriteRenderer;
    private ScoreManager scoreManager;
    
    public void Initialize(ScoreItemType type, int score, Color color, ScoreManager manager)
    {
        itemType = type;
        scoreValue = score;
        scoreManager = manager;
        
        // Collider 설정
        itemCollider = GetComponent<CircleCollider2D>();
        if (itemCollider == null)
        {
            itemCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        itemCollider.isTrigger = true;  // Trigger로 설정
        
        // 시각적 표현
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
        
        Debug.Log($"ScoreItem 초기화 - Type: {itemType}, Score: {scoreValue}");
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Ball과 충돌 확인
        if (other.CompareTag("Ball") && scoreManager != null)
        {
            // 점수 획득
            scoreManager.CollectItem(this);
            
            // 아이템 제거
            Destroy(gameObject);
        }
    }
}
