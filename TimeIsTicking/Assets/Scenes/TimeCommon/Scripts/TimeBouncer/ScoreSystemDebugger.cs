using UnityEngine;

/// <summary>
/// 스코어 시스템 디버그 도우미
/// 게임 오브젝트들의 태그와 충돌 설정을 확인
/// </summary>
public class ScoreSystemDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    [Tooltip("디버그 정보 자동 출력")]
    public bool autoDebug = true;
    
    [Tooltip("디버그 간격 (초)")]
    public float debugInterval = 5f;
    
    private float nextDebugTime;
    
    void Start()
    {
        if (autoDebug)
        {
            nextDebugTime = Time.time + debugInterval;
            DebugGameObjects();
        }
    }
    
    void Update()
    {
        if (autoDebug && Time.time >= nextDebugTime)
        {
            DebugGameObjects();
            nextDebugTime = Time.time + debugInterval;
        }
    }
    
    /// <summary>
    /// 게임 오브젝트들의 태그 및 설정 확인
    /// </summary>
    [ContextMenu("Debug Game Objects")]
    public void DebugGameObjects()
    {
        Debug.Log("=== 스코어 시스템 디버그 정보 ===");
        
        // Ball 오브젝트 확인
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        Debug.Log($"🏀 Ball 태그 오브젝트 수: {balls.Length}");
        
        foreach (var ball in balls)
        {
            var ballCollider = ball.GetComponent<Collider2D>();
            var ballRb = ball.GetComponent<Rigidbody2D>();
            
            Debug.Log($"   - {ball.name}: 위치={ball.transform.position}, " +
                     $"Collider={ballCollider?.GetType().Name}, " +
                     $"IsTrigger={ballCollider?.isTrigger}, " +
                     $"Rigidbody={ball.GetComponent<Rigidbody2D>() != null}");
        }
        
        // Paddle 오브젝트 확인 (Player 태그)
        GameObject[] paddles = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log($"🏓 Player 태그 오브젝트 수: {paddles.Length}");
        
        foreach (var paddle in paddles)
        {
            var paddleCollider = paddle.GetComponent<Collider2D>();
            Debug.Log($"   - {paddle.name}: 위치={paddle.transform.position}, " +
                     $"Collider={paddleCollider?.GetType().Name}, " +
                     $"IsTrigger={paddleCollider?.isTrigger}");
        }
        
        // ScoreItem 오브젝트 확인
        ScoreItem[] scoreItems = FindObjectsByType<ScoreItem>(FindObjectsSortMode.None);
        Debug.Log($"⭐ ScoreItem 오브젝트 수: {scoreItems.Length}");
        
        foreach (var item in scoreItems)
        {
            var allColliders = item.GetComponents<Collider2D>();
            var circleCollider = item.GetComponent<CircleCollider2D>();
            var spriteRenderer = item.GetComponent<SpriteRenderer>();
            
            Debug.Log($"   - {item.name}: 위치={item.transform.position}, " +
                     $"타입={item.itemType}, " +
                     $"점수={item.scoreValue}");
            Debug.Log($"     * Collider 수: {allColliders.Length}");
            
            foreach (var collider in allColliders)
            {
                if (collider is CircleCollider2D circle)
                {
                    Debug.Log($"       - CircleCollider2D: 반지름={circle.radius}, IsTrigger={circle.isTrigger}");
                }
                else
                {
                    Debug.Log($"       - {collider.GetType().Name}: Bounds={collider.bounds.size}, IsTrigger={collider.isTrigger}");
                }
            }
            
            Debug.Log($"     * SpriteRenderer: {spriteRenderer != null}, 색상={spriteRenderer?.color}");
        }
        
        // ScoreManager 확인
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            Debug.Log($"📊 ScoreManager: 현재점수={scoreManager.currentScore}, " +
                     $"목표점수={scoreManager.targetScore}, " +
                     $"스폰반지름={scoreManager.spawnRadius}");
        }
        else
        {
            Debug.LogWarning("⚠️ ScoreManager를 찾을 수 없습니다!");
        }
        
        Debug.Log("=== 디버그 정보 끝 ===");
    }
    
    /// <summary>
    /// 특정 위치 주변의 충돌체 확인
    /// </summary>
    [ContextMenu("Check Colliders Around Ball")]
    public void CheckCollidersAroundBall()
    {
        GameObject ball = GameObject.FindWithTag("Ball");
        if (ball == null)
        {
            Debug.LogWarning("Ball을 찾을 수 없습니다!");
            return;
        }
        
        Vector2 ballPosition = ball.transform.position;
        float checkRadius = 5f;
        
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(ballPosition, checkRadius);
        Debug.Log($"🔍 Ball 주변 {checkRadius}f 반지름 내 충돌체 수: {nearbyColliders.Length}");
        
        foreach (var collider in nearbyColliders)
        {
            float distance = Vector2.Distance(ballPosition, collider.transform.position);
            Debug.Log($"   - {collider.name}: 거리={distance:F2}, " +
                     $"태그={collider.tag}, " +
                     $"IsTrigger={collider.isTrigger}, " +
                     $"타입={collider.GetType().Name}");
        }
    }
    
#if UNITY_EDITOR
    /// <summary>
    /// Gizmo로 시각적 디버그 정보 표시
    /// </summary>
    void OnDrawGizmos()
    {
        // Ball 위치 표시
        GameObject ball = GameObject.FindWithTag("Ball");
        if (ball != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(ball.transform.position, 1f);
            
            // Ball 주변 체크 영역
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(ball.transform.position, 5f);
        }
        
        // ScoreItem들 위치 표시
        ScoreItem[] scoreItems = FindObjectsByType<ScoreItem>(FindObjectsSortMode.None);
        foreach (var item in scoreItems)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(item.transform.position, 0.5f);
            
            // 충돌 반지름 표시
            var collider = item.GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(item.transform.position, collider.radius);
            }
        }
    }
#endif
}