using UnityEngine;

/// <summary>
/// TimeBouncer 게임 전용 매니저
/// GameManagerBase를 상속받아 TimeBouncer 게임 로직만 구현
/// 
/// 게임 구조:
/// 1. ClockBorder: 시각적 표현 + EdgeCollider2D로 경계 충돌
/// 2. Paddle: 플레이어가 조작하는 시침 (Kinematic Rigidbody2D)
/// 3. Ball: 물리 기반으로 움직이는 공 (Dynamic Rigidbody2D)
/// 
/// 충돌 처리:
/// - Ball ↔ Paddle: Unity Physics (CircleCollider2D ↔ BoxCollider2D)
/// - Ball ↔ 시계 외곽: EdgeCollider2D로 물리 반사
/// </summary>
public class TimeBouncer_GameManager : GameManagerBase
{
    [Header("TimeBouncer Scene Objects")]
    [Tooltip("플레이어 패들 (시침)")]
    public GameObject paddle;
    
    [Tooltip("공")]
    public GameObject ball;
    
    [Header("TimeBouncer Paddle Settings")]
    [Tooltip("패들 회전 속도 (도/초)")]
    public float paddleRotationSpeed = 180f;
    
    [Header("TimeBouncer Ball Settings")]
    [Tooltip("공 초기 속도")]
    public float ballInitialSpeed = 300f;
    
    [Tooltip("공 최소 속도")]
    public float ballMinSpeed = 250f;
    
    [Tooltip("공 최대 속도")]
    public float ballMaxSpeed = 500f;
    
    [Tooltip("속도 감쇄율 (0~1)")]
    [Range(0f, 1f)]
    public float speedDecayRate = 0f;
    
    [Tooltip("경계 반사 계수 (0~1)")]
    [Range(0f, 1f)]
    public float boundaryBounciness = 1f;
    
    //TODO ball-ScoreOBject간 Trigger시 거리검증용 변수
    [Tooltip("공과 스코어 오브젝트 간 트리거 거리 임계값")]
    [Range(0f, 5f)]
    public float ballScoreTriggerDistanceThreshold = 1.0f;
    
    [Header("TimeBouncer Score System")]
    [Tooltip("스코어 매니저 (자동 생성됨)")]
    public ScoreManager scoreManager;
    
    /// <summary>
    /// 목표 점수 - ScoreManager가 있으면 ScoreManager의 값 사용
    /// </summary>
    public override int targetScore
    {
        get
        {
            if (scoreManager != null)
            {
                return scoreManager.TargetScore;
            }
            return fallbackTargetScore;
        }
    }
    
    // TimeBouncer 전용 변수
    private Rigidbody2D ballRb;
    private Rigidbody2D paddleRb;
    private CircleCollider2D ballCollider;
    private float ballRadius;
    private float currentPaddleAngle = 0f;
    
    // 베이스 클래스에서 요구하는 추상 메서드 구현
    protected override void InitializeGameSpecific()
    {
        // TimeBouncer는 점수 시스템 사용
        useScoreSystem = true;
        
        InitializeTimeBounceComponents();

        //거리값을 ball에 전달
        InitializeBall(ballScoreTriggerDistanceThreshold);
        SetupCollisionDetection();
        SetupClockBorderCollider();
        SetupScoreSystem();
        
        // 공통 경계 처리를 위해 Ball 등록
        RegisterBallForBoundary(ball);
        
    // 검증/디버그 출력 제거됨
    }
    
    protected override void UpdateGameSpecific()
    {
        MaintainBallSpeed();
        UpdateTimer();
    }
    
    protected override void HandleGameSpecificInput()
    {
        HandlePaddleInput();
    }
    
    protected override void OnGameEndSpecific()
    {
        // TimeBouncer 게임 종료 처리
        if (ballRb != null)
        {
            ballRb.linearVelocity = Vector2.zero;
        }
    }

    protected override void RestartGameSpecific()
    {
        // TimeBouncer 게임 재시작 처리
        InitializeBall(ballScoreTriggerDistanceThreshold);
        if (scoreManager != null)
        {
            scoreManager.ResetScore();
        }
    }
    

    
    
    
    /// <summary>
    /// TimeBouncer 컴포넌트 초기화
    /// </summary>
    void InitializeTimeBounceComponents()
    {
        // Ball 컴포넌트
        if (ball != null)
        {
            ballRb = ball.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                // 물리 설정
                ballRb.linearDamping = 0f;
                ballRb.angularDamping = 0f;
                ballRb.gravityScale = 0f;
                ballRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }
            
            ballCollider = ball.GetComponent<CircleCollider2D>();
            if (ballCollider != null)
            {
                // Ball은 반드시 일반 Collider여야 함 (Trigger 아님)
                ballCollider.isTrigger = false;
                ballRadius = ballCollider.radius * Mathf.Max(ball.transform.localScale.x, ball.transform.localScale.y);
                
            }
            else
            {
                ballRadius = 0.5f;
            }
            
            
        }
        
        // Paddle 컴포넌트
        if (paddle != null)
        {
            paddleRb = paddle.GetComponent<Rigidbody2D>();
            if (paddleRb == null)
            {
                paddleRb = paddle.AddComponent<Rigidbody2D>();
            }
            
            // Paddle은 반드시 Kinematic
            paddleRb.bodyType = RigidbodyType2D.Kinematic;
            paddleRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            
            // Paddle Collider는 일반 Collider여야 함
            Collider2D paddleCollider = paddle.GetComponent<Collider2D>();
            if (paddleCollider != null)
            {
                paddleCollider.isTrigger = false;
                
            }
            
            currentPaddleAngle = paddle.transform.eulerAngles.z;
            
        }
    }
    
    /// <summary>
    /// Ball 초기 속도 설정
    /// </summary>
    void InitializeBall(float distanceThreshold)
    {
        if (ballRb != null)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            ballRb.linearVelocity = direction * ballInitialSpeed;
            
        }
    }
    
    /// <summary>
    /// 충돌 감지 설정
    /// </summary>
    void SetupCollisionDetection()
    {
        if (ball != null)
        {
            var collisionHandler = ball.GetComponent<BallCollisionHandler>();
            if (collisionHandler == null)
            {
                collisionHandler = ball.AddComponent<BallCollisionHandler>();
            }
            collisionHandler.Initialize(this);
        }
    }
    
    /// <summary>
    /// ClockBorder에 EdgeCollider2D 설정
    /// </summary>
    void SetupClockBorderCollider()
    {
        if (clockBorder != null)
        {
            var colliderSetup = clockBorder.GetComponent<ClockBorderColliderSetup>();
            if (colliderSetup == null)
            {
                colliderSetup = clockBorder.AddComponent<ClockBorderColliderSetup>();
            }
            
            colliderSetup.radius = clockRadius;
            colliderSetup.SetupEdgeCollider();
            
            
        }
    }
    
    /// <summary>
    /// 스코어 시스템 설정
    /// </summary>
    void SetupScoreSystem()
    {
        if (!useScoreSystem) return;
        
        if (scoreManager == null)
        {
            GameObject scoreObj = new GameObject("ScoreManager");
            scoreObj.transform.parent = transform;
            scoreObj.transform.localPosition = Vector3.zero;
            scoreManager = scoreObj.AddComponent<ScoreManager>();
        }
        
        scoreManager.Initialize(clockCenter);
        scoreManager.OnScoreChanged += OnScoreChanged;
        scoreManager.OnTargetReached += OnTargetScoreReached;
        
        
    }
    
    /// <summary>
    /// 점수 변경 이벤트 핸들러
    /// </summary>
    void OnScoreChanged(ScoreEventArgs args)
    {
        currentScore = args.totalScore;
        
    }
    
    /// <summary>
    /// 목표 점수 달성 이벤트 핸들러
    /// </summary>
    void OnTargetScoreReached(int finalScore)
    {
        currentScore = finalScore;
        OnGameEnd();
    }
    
    /// <summary>
    /// Ball 충돌 처리 (BallCollisionHandler에서 호출)
    /// Clock boundary와 Paddle 충돌만 처리
    /// </summary>
    public void OnBallCollision(Collision2D collision)
    {
        GameObject collisionObject = collision.gameObject;
        
        
        // Paddle과 충돌 시 특별 처리
        if (collisionObject == paddle && ballRb != null)
        {
            Vector2 contactPoint = collision.contacts[0].point;
            Vector2 paddleCenter = paddle.transform.position;
            Vector2 radiusVector = contactPoint - paddleCenter;
            
            float angularVelocity = paddleRotationSpeed * Mathf.Deg2Rad;
            Vector2 paddleVelocityAtContact = new Vector2(-radiusVector.y, radiusVector.x) * angularVelocity;
            
            Vector2 newVelocity = ballRb.linearVelocity + paddleVelocityAtContact * 0.5f;
            
            float speed = newVelocity.magnitude;
            if (speed > ballMaxSpeed)
            {
                newVelocity = newVelocity.normalized * ballMaxSpeed;
            }
            
            ballRb.linearVelocity = newVelocity;
            
        }
        // Clock boundary 충돌 (일반적인 물리 반사)
        else if (collisionObject.name.Contains("ClockBorder") || collisionObject.name.Contains("Border"))
        {
            // Unity의 기본 물리 반사를 사용 (별도 처리 불필요)
        }
    }
    
    /// <summary>
    /// 패들 입력 처리
    /// </summary>
    private void HandlePaddleInput()
    {
        if (paddle == null) return;
        
        float input = ReadInputValue();
        
        if (Mathf.Abs(input) > 0.05f)
        {
            currentPaddleAngle += input * paddleRotationSpeed * Time.deltaTime;
            paddle.transform.rotation = Quaternion.Euler(0, 0, currentPaddleAngle);
        }
    }
    
    /// <summary>
    /// Ball 속도 유지
    /// </summary>
    private void MaintainBallSpeed()
    {
        if (ballRb == null) return;
        
        float currentSpeed = ballRb.linearVelocity.magnitude;
        
        // 속도 감쇄
        if (speedDecayRate > 0f && currentSpeed > ballMinSpeed)
        {
            float decayedSpeed = currentSpeed * (1f - speedDecayRate * Time.deltaTime);
            if (decayedSpeed < ballMinSpeed)
            {
                decayedSpeed = ballMinSpeed;
            }
            
            if (decayedSpeed != currentSpeed)
            {
                Vector2 direction = ballRb.linearVelocity.normalized;
                ballRb.linearVelocity = direction * decayedSpeed;
                currentSpeed = decayedSpeed;
            }
        }
        
        // 속도 제한
        if (currentSpeed < ballMinSpeed && currentSpeed > 0.1f)
        {
            Vector2 direction = ballRb.linearVelocity.normalized;
            ballRb.linearVelocity = direction * ballMinSpeed;
        }
        else if (currentSpeed > ballMaxSpeed)
        {
            Vector2 direction = ballRb.linearVelocity.normalized;
            ballRb.linearVelocity = direction * ballMaxSpeed;
        }
    }
    
    /// <summary>
    /// 타이머 업데이트
    /// </summary>
    private void UpdateTimer()
    {
        // TimeBouncer는 일반적으로 시간 제한이 없지만, 필요시 구현
        if (timerText != null)
        {
            timerText.text = $"Time: {Time.time:F1}s";
        }
    }
    

#if UNITY_EDITOR
    /// <summary>
    /// TimeBouncer 전용 디버그 기즈모
    /// </summary>
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos(); // 공통 기즈모 그리기
        
        if (clockCenter == null) return;
        Vector3 center = clockCenter.position;
        
        // ClockBorder 실제 크기 (시안색)
        if (clockBorder != null)
        {
            SpriteRenderer borderRenderer = clockBorder.GetComponent<SpriteRenderer>();
            if (borderRenderer != null && borderRenderer.sprite != null)
            {
                float actualRadius = borderRenderer.sprite.bounds.extents.x * clockBorder.transform.localScale.x;
                Gizmos.color = Color.cyan;
                DrawCircle(center, actualRadius, 64);
            }
        }
        
        // Paddle 방향 (녹색)
        if (Application.isPlaying && paddle != null)
        {
            Gizmos.color = Color.green;
            float angle = currentPaddleAngle * Mathf.Deg2Rad;
            Vector3 paddleEnd = center + new Vector3(Mathf.Cos(angle) * clockRadius * 0.8f, Mathf.Sin(angle) * clockRadius * 0.8f, 0);
            Gizmos.DrawLine(center, paddleEnd);
        }
    }
#endif
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        // 이벤트 구독 해제
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= OnScoreChanged;
            scoreManager.OnTargetReached -= OnTargetScoreReached;
        }
    }
}