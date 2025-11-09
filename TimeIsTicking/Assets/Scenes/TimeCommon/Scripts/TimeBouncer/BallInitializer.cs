using UnityEngine;

public class BallInitializer : MonoBehaviour
{
    [Header("Ball Settings")]
    [Tooltip("공의 초기 속도 (단위/초)")]
    public float initialSpeed = 300f;
    
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 랜덤한 방향으로 초기 속도 설정
            Vector2 direction = Random.insideUnitCircle.normalized;
            rb.linearVelocity = direction * initialSpeed;
            Debug.Log($"Ball 초기 속도: {rb.linearVelocity}");
        }
        else
        {
            Debug.LogError("Ball에 Rigidbody2D 컴포넌트가 없습니다!");
        }
    }
}
