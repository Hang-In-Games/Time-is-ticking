using UnityEngine;

/// <summary>
/// Ball의 충돌 이벤트를 GameManager에 전달하는 헬퍼 컴포넌트
/// GameManager가 자동으로 추가함 (수동 추가 불필요)
/// </summary>
public class BallCollisionHandler : MonoBehaviour
{
    private GameManager_TimeBouncer gameManager;
    
    public void Initialize(GameManager_TimeBouncer manager)
    {
        gameManager = manager;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Paddle과 충돌했는지 확인
        if (collision.gameObject.CompareTag("Paddle") && gameManager != null)
        {
            gameManager.OnBallHitPaddle(collision);
        }
    }
}
