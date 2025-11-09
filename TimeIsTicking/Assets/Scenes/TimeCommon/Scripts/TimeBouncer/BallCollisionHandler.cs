using UnityEngine;

/// <summary>
/// Ball의 충돌 이벤트를 GameManager에 전달하는 헬퍼 컴포넌트
/// GameManager가 자동으로 추가함 (수동 추가 불필요)
/// </summary>
public class BallCollisionHandler : MonoBehaviour
{
    [Tooltip("GameManager 참조 (자동 설정됨)")]
    public TimeBouncer_GameManager gameManager;
    
    //distanceThreshold
    private float distanceThreshold;

    public void Initialize(float threshold)
    {
        distanceThreshold = threshold;
    }
    
    public void Initialize(TimeBouncer_GameManager manager)
    {
        gameManager = manager;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Clock boundary와 Paddle 충돌만 GameManager에 전달
        GameObject collisionObject = collision.gameObject;
        
        // Paddle 충돌이거나 Clock border 충돌인 경우만 처리
        if (collisionObject.CompareTag("Player") || 
            collisionObject.name.Contains("ClockBorder") || 
            collisionObject.name.Contains("Border"))
        {
            if (gameManager != null)
            {
                gameManager.OnBallCollision(collision);
            }
        }
        
        // ScoreItem과의 충돌은 ScoreItem.OnTriggerEnter2D에서 처리됨
    }
}
