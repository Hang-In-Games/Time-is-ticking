using UnityEngine;

/// <summary>
/// TimeBouncer 게임의 중앙 관리 매니저
/// 
/// 설계 철학: 중앙 관리 (Manager가 모든 오브젝트를 직접 관리)
/// - 각 오브젝트에 개별 스크립트 추가 X
/// - Manager에 오브젝트를 Public으로 할당
/// - Manager가 모든 로직 처리 (초기화, 업데이트, 충돌 등)
/// 
/// 게임 구조:
/// 1. ClockBorder: 시각적 표현만 (Collider 없음)
/// 2. Paddle: Manager가 입력 처리 및 회전 제어
/// 3. Ball: Manager가 경계 체크 및 물리 처리
/// 
/// 충돌 처리:
/// - Ball ↔ Paddle: Unity Physics (CircleCollider2D ↔ BoxCollider2D)
/// - Ball ↔ 시계 외곽: Manager가 수학적으로 경계 체크
/// </summary>
public class GameManager_TimeBouncer : MonoBehaviour
{
    [Header("Scene Objects")]
    [Tooltip("시계의 중심점")]
    public Transform clockCenter;
    
    [Tooltip("시계 테두리 스프라이트 (시각적 표현)")]
    public GameObject clockBorder;
    
    [Tooltip("플레이어 패들 (시침)")]
    public GameObject paddle;
    
    [Tooltip("공")]
    public GameObject ball;
    
    [Header("Clock Settings")]
    [Tooltip("시계 테두리 반지름")]
    public float clockRadius = 180f;
    
    [Tooltip("Ball 경계 오프셋 (양수: 안쪽, 음수: 바깥쪽)")]
    public float ballBoundaryOffset = 0f;  // 0으로 설정하면 정확히 clockRadius까지 감
    
    [Header("Paddle Settings")]
    [Tooltip("패들 회전 속도 (도/초)")]
    public float paddleRotationSpeed = 180f;
    
    [Tooltip("플레이어 입력 사용 여부")]
    public bool usePlayerInput = true;
    
    [Header("Ball Settings")]
    [Tooltip("공 초기 속도")]
    public float ballInitialSpeed = 300f;
    
    [Tooltip("공 최소 속도 (이 속도 아래로 떨어지지 않음)")]
    public float ballMinSpeed = 250f;
    
    [Tooltip("공 최대 속도 (이 속도 위로 올라가지 않음)")]
    public float ballMaxSpeed = 500f;
    
    [Tooltip("속도 감쇄율 (0~1, 0: 감쇄 없음, 1: 매우 빠른 감쇄)")]
    [Range(0f, 1f)]
    public float speedDecayRate = 0f;  // 기본값 0 (감쇄 없음)
    
    [Tooltip("경계 반사 계수 (0~1)")]
    [Range(0f, 1f)]
    public float boundaryBounciness = 1f;  // 1로 변경 (에너지 손실 없음)
    
    [Header("Score System")]
    [Tooltip("스코어 시스템 사용 여부")]
    public bool useScoreSystem = true;
    
    [Tooltip("스코어 매니저 (자동 생성됨)")]
    public ScoreManager scoreManager;
    
    // Private 변수
    private Rigidbody2D ballRb;
    private Rigidbody2D paddleRb;
    private CircleCollider2D ballCollider;
    private float ballRadius;
    private float currentPaddleAngle = 0f;
    private UnityEngine.InputSystem.InputAction moveAction;
    private bool wasBeyondBoundary = false;  // 경계 넘었는지 추적

    void Start()
    {
        ValidateReferences();
        InitializeComponents();
        InitializeBall();
        SetupInputSystem();
        SetupCollisionDetection();
        SetupClockBorderCollider();  // ClockBorder Collider 설정
        SetupScoreSystem();  // 스코어 시스템 설정
    }

    void Update()
    {
        if (usePlayerInput)
        {
            HandlePaddleInput();
        }
        
        // ConstrainBallToBoundary();  // Unity Physics로 대체 (ClockBorder EdgeCollider2D 사용)
        MaintainBallSpeed();
    }
    
    /// <summary>
    /// Inspector에서 할당된 오브젝트들 검증
    /// </summary>
    void ValidateReferences()
    {
        if (clockCenter == null)
        {
            clockCenter = transform;
            Debug.LogWarning("ClockCenter가 할당되지 않아 GameManager 위치를 사용합니다.");
        }
        
        if (paddle == null)
        {
            Debug.LogError("Paddle이 할당되지 않았습니다!");
        }
        
        if (ball == null)
        {
            Debug.LogError("Ball이 할당되지 않았습니다!");
        }
    }
    
    /// <summary>
    /// 컴포넌트 초기화 및 참조 저장
    /// </summary>
    void InitializeComponents()
    {
        // Ball 컴포넌트
        if (ball != null)
        {
            ballRb = ball.GetComponent<Rigidbody2D>();
            if (ballRb == null)
            {
                Debug.LogError("Ball에 Rigidbody2D가 없습니다!");
            }
            else
            {
                // 물리 설정 강제 적용
                ballRb.linearDamping = 0f;      // 선형 감쇠 없음 (공기저항 제거)
                ballRb.angularDamping = 0f;     // 회전 감쇠 없음
                ballRb.gravityScale = 0f;        // 중력 없음
                ballRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }
            
            ballCollider = ball.GetComponent<CircleCollider2D>();
            if (ballCollider != null)
            {
                ballRadius = ballCollider.radius * Mathf.Max(ball.transform.localScale.x, ball.transform.localScale.y);
            }
            else
            {
                Debug.LogWarning("Ball에 CircleCollider2D가 없어 기본 반지름 사용");
                ballRadius = 5f;
            }
            
            Debug.Log($"Ball 초기화 완료 - Radius: {ballRadius}, Linear Damping: {ballRb.linearDamping}");
        }
        
        // Paddle 컴포넌트
        if (paddle != null)
        {
            paddleRb = paddle.GetComponent<Rigidbody2D>();
            if (paddleRb == null)
            {
                Debug.LogWarning("Paddle에 Rigidbody2D가 없어 추가합니다.");
                paddleRb = paddle.AddComponent<Rigidbody2D>();
            }
            
            // Paddle 물리 설정 - 반드시 Kinematic!
            if (paddleRb.bodyType != RigidbodyType2D.Kinematic)
            {
                Debug.LogWarning($"Paddle Body Type이 {paddleRb.bodyType}였습니다. Kinematic으로 강제 변경합니다!");
                paddleRb.bodyType = RigidbodyType2D.Kinematic;
            }
            
            paddleRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            paddleRb.linearDamping = 0f;
            paddleRb.angularDamping = 0f;
            
            currentPaddleAngle = paddle.transform.eulerAngles.z;
            Debug.Log($"Paddle 초기화 완료 - Body Type: {paddleRb.bodyType}");
        }
    }
    
    /// <summary>
    /// Ball 초기 속도 설정
    /// </summary>
    void InitializeBall()
    {
        if (ballRb != null)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            ballRb.linearVelocity = direction * ballInitialSpeed;
            Debug.Log($"Ball 초기 속도 설정: {ballRb.linearVelocity}");
        }
    }
    
    /// <summary>
    /// 충돌 감지 설정
    /// </summary>
    void SetupCollisionDetection()
    {
        if (ball != null)
        {
            // Ball에 충돌 리스너 추가
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
            
            // 반지름 설정
            colliderSetup.radius = clockRadius;
            colliderSetup.SetupEdgeCollider();
            
            Debug.Log("ClockBorder EdgeCollider2D 설정 완료");
        }
        else
        {
            Debug.LogWarning("ClockBorder가 할당되지 않아 EdgeCollider2D를 생성할 수 없습니다.");
        }
    }
    
    /// <summary>
    /// 스코어 시스템 설정
    /// </summary>
    void SetupScoreSystem()
    {
        if (!useScoreSystem) return;
        
        // ScoreManager 생성 또는 가져오기
        if (scoreManager == null)
        {
            GameObject scoreObj = new GameObject("ScoreManager");
            scoreObj.transform.parent = transform;
            scoreObj.transform.localPosition = Vector3.zero;
            scoreManager = scoreObj.AddComponent<ScoreManager>();
        }
        
        // ScoreManager 초기화
        scoreManager.Initialize(clockCenter);
        
        // 이벤트 구독
        scoreManager.OnScoreChanged += OnScoreChanged;
        scoreManager.OnTargetReached += OnTargetScoreReached;
        
        Debug.Log("스코어 시스템 초기화 완료");
    }
    
    /// <summary>
    /// 점수 변경 이벤트 핸들러
    /// </summary>
    void OnScoreChanged(ScoreEventArgs args)
    {
        Debug.Log($"점수 획득! +{args.scoreGained} ({args.itemType}) - 총점: {args.totalScore}");
        // TODO: UI 업데이트
    }
    
    /// <summary>
    /// 목표 점수 달성 이벤트 핸들러
    /// </summary>
    void OnTargetScoreReached(int finalScore)
    {
        Debug.Log($"목표 점수 달성! 최종 점수: {finalScore}");
        // TODO: 게임 클리어 처리
    }
    
    /// <summary>
    /// Ball이 Paddle과 충돌했을 때 호출됨
    /// </summary>
    public void OnBallHitPaddle(Collision2D collision)
    {
        if (ballRb == null || paddle == null) return;
        
        // Paddle의 회전 속도 계산 (각속도를 선속도로 변환)
        // 접촉점에서의 Paddle 속도를 Ball에 추가
        Vector2 contactPoint = collision.contacts[0].point;
        Vector2 paddleCenter = paddle.transform.position;
        Vector2 radiusVector = contactPoint - paddleCenter;
        
        // 각속도를 라디안으로 변환 (현재 회전 속도 사용)
        float angularVelocity = paddleRotationSpeed * Mathf.Deg2Rad;
        
        // 접촉점에서의 선속도 = 각속도 × 반지름 (수직 방향)
        Vector2 paddleVelocityAtContact = new Vector2(-radiusVector.y, radiusVector.x) * angularVelocity;
        
        // Ball의 현재 속도에 Paddle 속도 일부 추가
        Vector2 newVelocity = ballRb.linearVelocity + paddleVelocityAtContact * 0.5f;
        
        // 속도 제한 적용
        float speed = newVelocity.magnitude;
        if (speed > ballMaxSpeed)
        {
            newVelocity = newVelocity.normalized * ballMaxSpeed;
        }
        
        ballRb.linearVelocity = newVelocity;
        Debug.Log($"Paddle 충돌 - 속도: {ballRb.linearVelocity.magnitude:F1}");
    }
    
    /// <summary>
    /// Input System 설정
    /// </summary>
    void SetupInputSystem()
    {
        // var inputActions = new UnityEngine.InputSystem.InputActionAsset();
        //
        // // InputSystem_Actions 에셋 찾기
        // var existingActions = Resources.LoadAll<UnityEngine.InputSystem.InputActionAsset>("");
        // if (existingActions.Length > 0)
        // {
        //     inputActions = existingActions[0];
        // }
        // else
        // {
        //     // Assets 폴더에서 찾기
        //     string[] guids = UnityEditor.AssetDatabase.FindAssets("InputSystem_Actions t:InputActionAsset");
        //     if (guids.Length > 0)
        //     {
        //         string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
        //         inputActions = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>(path);
        //     }
        // }
        //
        // if (inputActions != null)
        // {
        //     var playerActionMap = inputActions.FindActionMap("Player");
        //     if (playerActionMap != null)
        //     {
        //         moveAction = playerActionMap.FindAction("Move");
        //         if (moveAction != null)
        //         {
        //             moveAction.Enable();
        //             Debug.Log("Input System 설정 완료!");
        //         }
        //     }
        // }
        // else
        // {
        //     Debug.LogWarning("InputSystem_Actions를 찾을 수 없습니다. 키보드 입력으로 대체합니다.");
        // }
    }
    
    /// <summary>
    /// 패들 입력 처리 및 회전
    /// </summary>
    void HandlePaddleInput()
    {
        if (paddle == null) return;
        
        float input = 0f;
        
        // Input System 사용
        if (moveAction != null)
        {
            input = moveAction.ReadValue<Vector2>().x;
        }
        // Fallback: 기본 키보드 입력
        else
        {
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.A) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftArrow))
                input = -1f;
            else if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.D) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightArrow))
                input = 1f;
        }
        
        // 회전 적용
        if (input != 0f)
        {
            currentPaddleAngle += input * paddleRotationSpeed * Time.deltaTime;
            paddle.transform.rotation = Quaternion.Euler(0, 0, currentPaddleAngle);
        }
    }
    
    /// <summary>
    /// Ball을 원형 경계 내로 제약
    /// </summary>
    void ConstrainBallToBoundary()
    {
        if (ball == null || ballRb == null) return;
        
        Vector2 centerPosition = clockCenter.position;
        Vector2 ballPosition = ball.transform.position;
        Vector2 directionFromCenter = ballPosition - centerPosition;
        float distanceFromCenter = directionFromCenter.magnitude;
        
        // 유효 반지름 (Ball 크기 고려)
        float effectiveRadius = clockRadius - ballRadius;
        
        // 경계를 벗어났는지 확인
        if (distanceFromCenter > effectiveRadius)
        {
            // 경계 안으로 위치 보정
            Vector2 normalizedDirection = directionFromCenter.normalized;
            Vector2 boundaryPosition = centerPosition + normalizedDirection * effectiveRadius;
            ball.transform.position = boundaryPosition;
            
            // 속도 반사
            Vector2 velocity = ballRb.linearVelocity;
            Vector2 normal = -normalizedDirection; // 경계의 법선 (안쪽 방향)
            
            // 반사 공식: v' = v - 2(v·n)n
            Vector2 reflectedVelocity = velocity - 2 * Vector2.Dot(velocity, normal) * normal;
            ballRb.linearVelocity = reflectedVelocity * boundaryBounciness;
            
            Debug.Log($"경계 반사 - 속도: {ballRb.linearVelocity.magnitude:F1}");
        }
    }
    
    /// <summary>
    /// Ball 속도를 최소/최대 범위 내로 유지
    /// </summary>
    void MaintainBallSpeed()
    {
        if (ballRb == null) return;
        
        float currentSpeed = ballRb.linearVelocity.magnitude;
        
        // 속도 감쇄 적용
        if (speedDecayRate > 0f && currentSpeed > ballMinSpeed)
        {
            float decayedSpeed = currentSpeed * (1f - speedDecayRate * Time.deltaTime);
            
            // 최소 속도 아래로 떨어지지 않도록
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
        
        // 속도가 최소값보다 낮으면 증가
        if (currentSpeed < ballMinSpeed && currentSpeed > 0.1f)
        {
            Vector2 direction = ballRb.linearVelocity.normalized;
            ballRb.linearVelocity = direction * ballMinSpeed;
            // Debug.Log($"속도 보정 (최소): {currentSpeed:F1} → {ballMinSpeed:F1}");
        }
        // 속도가 최대값보다 높으면 감소
        else if (currentSpeed > ballMaxSpeed)
        {
            Vector2 direction = ballRb.linearVelocity.normalized;
            ballRb.linearVelocity = direction * ballMaxSpeed;
            // Debug.Log($"속도 보정 (최대): {currentSpeed:F1} → {ballMaxSpeed:F1}");
        }
    }
    
    /// <summary>
    /// 디버그용 기즈모
    /// - 시안색: ClockBorder 실제 크기 (에디터/런타임 동일)
    /// - 노란색: 시계 경계 (clockRadius)
    /// </summary>
    void OnDrawGizmos()
    {
        if (clockCenter == null) return;
        
        Vector3 center = clockCenter.position;
        
        // ClockBorder 실제 크기 (시안색) - 에디터/런타임 동일
        if (clockBorder != null)
        {
            SpriteRenderer borderRenderer = clockBorder.GetComponent<SpriteRenderer>();
            if (borderRenderer != null && borderRenderer.sprite != null)
            {
                // 실제 렌더링되는 크기 계산
                float actualRadius = borderRenderer.sprite.bounds.extents.x * clockBorder.transform.localScale.x;
                Gizmos.color = Color.cyan;
                DrawCircle(center, actualRadius, 64);
            }
        }
        
        // 시계 경계 (노란색) - clockRadius 설정값
        Gizmos.color = Color.yellow;
        DrawCircle(center, clockRadius, 64);
        
        // Ball 유효 경계는 제거 (보기 복잡해서)
        // 필요하면 아래 주석 해제:
        /*
        if (Application.isPlaying && ball != null && ballRadius > 0)
        {
            Gizmos.color = Color.green;
            DrawCircle(center, clockRadius - ballRadius, 64);
        }
        */
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
    
    void OnDestroy()
    {
        if (moveAction != null)
        {
            moveAction.Disable();
        }
        
        // 이벤트 구독 해제
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= OnScoreChanged;
            scoreManager.OnTargetReached -= OnTargetScoreReached;
        }
    }
}
