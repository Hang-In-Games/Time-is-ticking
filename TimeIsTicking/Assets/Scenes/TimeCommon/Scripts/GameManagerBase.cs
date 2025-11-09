using UnityEngine;

/// <summary>
/// TimeBouncer와 TimeSeeker 게임의 공통 베이스 매니저 클래스
/// 
/// 공통 기능:
/// - Input System 설정
/// - 카메라 자동 설정
/// - 디버그 입력 처리
/// - 시계 설정 관리
/// - UI 기본 구조
/// 
/// 각 게임별 매니저는 이 클래스를 상속받아 게임별 로직 구현
/// </summary>
public abstract class GameManagerBase : MonoBehaviour
{
    [Header("Common Scene Objects")]
    [Tooltip("시계의 중심점")]
    public Transform clockCenter;
    
    [Tooltip("시계 테두리 스프라이트")]
    public GameObject clockBorder;
    
    [Header("Common Input Settings")]
    [Tooltip("Input System Actions 에셋 (선택사항)")]
    public UnityEngine.InputSystem.InputActionAsset inputActionAsset;
    
    [Tooltip("플레이어 입력 사용 여부")]
    public bool usePlayerInput = true;
    
    [Header("Common Clock Settings")]
    [Tooltip("시계 테두리 반지름")]
    public float clockRadius = 180f;
    
    [Header("Common Prefab Settings")]
    [Tooltip("프리팹으로 사용 시 부모 오브젝트 제어 여부")]
    public bool useParentControl = false;
    
    [Tooltip("카메라 자동 설정 사용 여부 (프리팹 사용 시 false 권장)")]
    public bool useCameraAutoSetup = false;
    
    [Header("Common UI References")]
    [Tooltip("점수 텍스트")]
    public UnityEngine.UI.Text scoreText;
    
    [Tooltip("타이머 텍스트 (초단위 표시)")]
    public UnityEngine.UI.Text timerText;
    
    [Tooltip("게임 상태 텍스트")]
    public UnityEngine.UI.Text statusText;
    
    [Tooltip("결과 메시지 텍스트 (GAME OVER/SUCCESS)")]
    public UnityEngine.UI.Text resultText;
    
    [Tooltip("결과 버튼 (재시작/다음단계 등)")]
    public UnityEngine.UI.Button resultButton;
    
    [Header("Common Score System")]
    [Tooltip("점수 시스템 사용 여부")]
    public bool useScoreSystem = false;
    
    [Tooltip("목표 점수 (0이면 무제한) - ScoreManager가 있으면 ScoreManager의 값 사용")]
    public int fallbackTargetScore = 100;
    
    /// <summary>
    /// 목표 점수 프로퍼티 - ScoreManager가 있으면 ScoreManager의 값을 우선 사용
    /// </summary>
    public virtual int targetScore
    {
        get
        {
            // 하위 클래스에서 ScoreManager를 사용하는 경우 오버라이드 가능
            return fallbackTargetScore;
        }
    }
    
    [Header("Common Timer System")]
    [Tooltip("타이머 시스템 사용 여부")]
    public bool useTimerSystem = false;
    
    [Tooltip("게임 제한시간 (초)")]
    [Range(10f, 300f)]
    public float gameTimeLimit = 60f;
    
    // 공통 내부 상태
    protected UnityEngine.InputSystem.InputAction moveAction;
    protected int currentScore = 0;
    protected float remainingTime = 0f;
    protected bool gameActive = true;
    
    // 타이머 시스템 상태
    protected float currentTimer = 0f;
    protected bool timerRunning = false;
    protected bool gameCompleted = false;  // 게임 완료 플래그
    protected bool gameCleared = false;    // 게임 클리어 플래그 (성공)
    
    // 게임 종료 상태 관리 (MashButtonMiniGame 패턴 적용)
    protected bool isRunning = false;      // 게임 진행 중 플래그
    protected bool hasEnded = false;       // 게임 종료됨 플래그
    
    // 경계 처리 관련
    protected GameObject ballObject;
    protected Rigidbody2D ballRigidbody;
    
    // 디버그 정보
    [Header("Debug Info (Runtime)")]
    [SerializeField] protected bool inputSystemActive = false;
    [SerializeField] protected string inputSystemStatus = "Not initialized";
    [SerializeField] protected float lastInputValue = 0f;
    [SerializeField] protected string gameType = "";
    
    // 추상 메서드 - 각 게임에서 구현해야 함
    protected abstract void InitializeGameSpecific();
    protected abstract void UpdateGameSpecific();
    protected abstract void HandleGameSpecificInput();
    protected abstract void OnGameEndSpecific();
    protected abstract void RestartGameSpecific();
    
    // Unity 생명주기
    protected virtual void Start()
    {
        gameType = GetType().Name;
        Debug.Log($"=== {gameType} 게임 시작 ===");
        
        ValidateCommonReferences();
        SetupInputSystem();
        SetupCamera();
        InitializeTimerSystem();
        InitializeGameSpecific();
        
        // 게임 시작
        isRunning = true;
        hasEnded = false;
    }
    
    protected virtual void Update()
    {
        // 게임이 종료되었거나 진행 중이 아니면 업데이트 중단 (MashButtonMiniGame 패턴)
        if (!isRunning || hasEnded) return;
        
        if (!gameActive) return;
        
        if (usePlayerInput)
        {
            HandleGameSpecificInput();
        }
        
        HandleCommonDebugInput();
        UpdateTimerSystem();
        UpdateGameSpecific();
        UpdateCommonUI();
        
        // 공통 경계 처리
        ConstrainBallToBoundary();
    }
    
    /// <summary>
    /// 공통 참조 검증
    /// </summary>
    protected virtual void ValidateCommonReferences()
    {
        if (clockCenter == null)
        {
            clockCenter = transform;
            Debug.LogWarning($"{gameType}: ClockCenter가 할당되지 않아 GameManager 위치를 사용합니다.");
        }
    }
    
    /// <summary>
    /// Input System 설정 (공통)
    /// </summary>
    protected virtual void SetupInputSystem()
    {
        // 방법 1: Inspector에서 할당된 InputActionAsset 사용
        if (inputActionAsset != null)
        {
            try
            {
                var playerActionMap = inputActionAsset.FindActionMap("Player");
                if (playerActionMap != null)
                {
                    moveAction = playerActionMap.FindAction("Move");
                    if (moveAction != null)
                    {
                        playerActionMap.Enable();
                        moveAction.Enable();
                        
                        inputSystemActive = true;
                        inputSystemStatus = $"Inspector 할당: {inputActionAsset.name}";
                        
                        Debug.Log($"{gameType}: Inspector에서 할당된 Input System 사용: {inputActionAsset.name}");
                        return;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"{gameType}: Inspector InputActionAsset 사용 실패: {e.Message}");
            }
        }
        
        // 실패 시 키보드 입력으로 대체
        inputSystemActive = false;
        inputSystemStatus = "로드 실패 - 키보드 입력 사용";
        
        Debug.LogWarning($"{gameType}: Input System을 찾을 수 없습니다. 키보드 입력으로 대체합니다.");
    }
    
    /// <summary>
    /// 카메라 자동 설정 (공통)
    /// </summary>
    protected virtual void SetupCamera()
    {
        // 프리팹 사용 시 카메라 자동 설정 스킵
        if (!useCameraAutoSetup)
        {
            Debug.Log($"{gameType}: 카메라 자동 설정 비활성화됨 (프리팹 모드)");
            return;
        }
        
        Camera mainCamera = Camera.main;
        if (mainCamera != null && clockCenter != null)
        {
            Vector3 cameraPosition = clockCenter.position + Vector3.back * 10f;
            mainCamera.transform.position = cameraPosition;
            mainCamera.transform.LookAt(clockCenter.position);
            
            if (mainCamera.orthographic)
            {
                mainCamera.orthographicSize = clockRadius * 1.5f;
            }
            
            Debug.Log($"{gameType}: 카메라 설정 완료 - Position: {cameraPosition}, Size: {mainCamera.orthographicSize}");
        }
    }
    
    /// <summary>
    /// 타이머 시스템 초기화
    /// </summary>
    protected virtual void InitializeTimerSystem()
    {
        if (useTimerSystem)
        {
            ResetTimer();
            StartTimer();
            
            // 결과 UI 초기화
            if (resultText != null)
            {
                resultText.gameObject.SetActive(false);
            }
            
            if (resultButton != null)
            {
                resultButton.gameObject.SetActive(false);
                resultButton.onClick.AddListener(OnResultButtonClicked);
            }
            
            Debug.Log($"{gameType}: 타이머 시스템 초기화 - 제한시간: {gameTimeLimit}초");
        }
    }
    
    /// <summary>
    /// 타이머 시스템 업데이트 - MashButtonMiniGame 패턴 적용
    /// </summary>
    protected virtual void UpdateTimerSystem()
    {
        // 게임이 종료되었거나 진행 중이 아니면 타이머 업데이트 중단
        if (!isRunning || hasEnded) return;
        
        if (useTimerSystem && timerRunning && !gameCompleted)
        {
            currentTimer -= Time.deltaTime;
            
            // 타이머 만료 체크
            if (currentTimer <= 0f)
            {
                currentTimer = 0f;
                OnTimerExpired();
            }
            
            UpdateTimerUI();
        }
    }
    
    /// <summary>
    /// 타이머 시작
    /// </summary>
    protected virtual void StartTimer()
    {
        if (useTimerSystem)
        {
            timerRunning = true;
            Debug.Log($"{gameType}: 타이머 시작!");
        }
    }
    
    /// <summary>
    /// 타이머 리셋
    /// </summary>
    protected virtual void ResetTimer()
    {
        currentTimer = gameTimeLimit;
        timerRunning = false;
        gameCompleted = false;
        gameCleared = false;
        
        UpdateTimerUI();
    }
    
    /// <summary>
    /// 타이머 UI 업데이트
    /// </summary>
    protected virtual void UpdateTimerUI()
    {
        if (timerText != null && useTimerSystem)
        {
            int seconds = Mathf.CeilToInt(currentTimer);
            timerText.text = $"Time: {seconds}s";
            
            // 시간에 따른 색상 변경
            if (currentTimer <= 10f)
            {
                timerText.color = Color.red;    // 위험: 빨간색
            }
            else if (currentTimer <= 30f)
            {
                timerText.color = Color.yellow; // 경고: 노란색
            }
            else
            {
                timerText.color = Color.white;  // 정상: 흰색
            }
        }
    }
    
    /// <summary>
    /// 공통 디버그 입력 처리
    /// </summary>
    protected virtual void HandleCommonDebugInput()
    {
        // Input System을 사용하여 키보드 입력 감지
        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard == null) return;
        
        // R: 게임 재시작
        if (keyboard.rKey.wasPressedThisFrame)
        {
            RestartGame();
            Debug.Log($"{gameType}: R 키 - 게임 재시작");
        }
        
        // I: Input System 재초기화
        if (keyboard.iKey.wasPressedThisFrame)
        {
            SetupInputSystem();
            Debug.Log($"{gameType}: I 키 - Input System 재초기화");
        }
        
        // P: 게임 일시정지/재개
        if (keyboard.pKey.wasPressedThisFrame)
        {
            TogglePause();
            Debug.Log($"{gameType}: P 키 - 게임 {(gameActive ? "재개" : "일시정지")}");
        }
    }
    
    /// <summary>
    /// 입력값 읽기 (공통 유틸리티)
    /// </summary>
    protected float ReadInputValue()
    {
        float input = 0f;
        bool inputSystemWorking = false;
        
        // Input System 시도
        if (moveAction != null)
        {
            try
            {
                if (moveAction.enabled && moveAction.actionMap != null && moveAction.actionMap.enabled)
                {
                    Vector2 moveValue = moveAction.ReadValue<Vector2>();
                    input = moveValue.x;
                    inputSystemWorking = true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"{gameType}: Input System 읽기 실패: {e.Message}");
                moveAction = null;
            }
        }
        
        // Fallback: Input System 키보드 직접 접근
        if (!inputSystemWorking)
        {
            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                    input = -1f;
                else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                    input = 1f;
            }
        }
        
        lastInputValue = input;
        return input;
    }
    
    /// <summary>
    /// 각도 정규화 (공통 유틸리티)
    /// </summary>
    protected float NormalizeAngle(float angle)
    {
        while (angle < 0) angle += 360f;
        while (angle >= 360) angle -= 360f;
        return angle;
    }
    
    /// <summary>
    /// Ball 등록 (경계 처리용)
    /// </summary>
    protected void RegisterBallForBoundary(GameObject ball)
    {
        ballObject = ball;
        if (ball != null)
        {
            ballRigidbody = ball.GetComponent<Rigidbody2D>();
        }
    }
    
    /// <summary>
    /// Ball을 원형 경계 내로 제약 (공통 유틸리티)
    /// </summary>
    protected void ConstrainBallToBoundary()
    {
        if (ballObject == null || clockCenter == null) return;
        
        Vector2 centerPosition = clockCenter.position;
        Vector2 ballPosition = ballObject.transform.position;
        Vector2 directionFromCenter = ballPosition - centerPosition;
        float distanceFromCenter = directionFromCenter.magnitude;
        
        // Ball 크기 계산
        float ballRadius = 0f;
        CircleCollider2D ballCollider = ballObject.GetComponent<CircleCollider2D>();
        if (ballCollider != null)
        {
            ballRadius = ballCollider.radius * Mathf.Max(ballObject.transform.localScale.x, ballObject.transform.localScale.y);
        }
        
        // 유효 반지름 (Ball 크기 고려)
        float effectiveRadius = clockRadius - ballRadius;
        
        // 경계를 벗어났는지 확인
        if (distanceFromCenter > effectiveRadius)
        {
            // 경계 안으로 위치 보정
            Vector2 normalizedDirection = directionFromCenter.normalized;
            Vector2 boundaryPosition = centerPosition + normalizedDirection * effectiveRadius;
            ballObject.transform.position = boundaryPosition;
            
            // 속도 반사 (물리 엔진 사용 시)
            if (ballRigidbody != null)
            {
                Vector2 velocity = ballRigidbody.linearVelocity;
                Vector2 normal = -normalizedDirection; // 경계의 법선 (안쪽 방향)
                
                // 반사 공식: v' = v - 2(v·n)n
                Vector2 reflectedVelocity = velocity - 2 * Vector2.Dot(velocity, normal) * normal;
                ballRigidbody.linearVelocity = reflectedVelocity * 0.9f; // 약간의 에너지 손실
                
                Debug.Log($"{gameType}: Ball 경계 반사 - 속도: {ballRigidbody.linearVelocity.magnitude:F1}");
            }
        }
    }
    
    /// <summary>
    /// 공통 UI 업데이트
    /// </summary>
    protected virtual void UpdateCommonUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
            if (targetScore > 0)
            {
                scoreText.text += $" / {targetScore}";
            }
        }
        
        if (statusText != null)
        {
            statusText.text = $"Game: {gameType} | Input: {(inputSystemActive ? "System" : "Keyboard")}";
        }
    }
    
    /// <summary>
    /// 점수 추가 (공통 유틸리티)
    /// </summary>
    protected virtual void AddScore(int points)
    {
        int previousScore = currentScore;
        currentScore += points;
        
        Debug.Log($"{gameType}: 점수 획득 +{points} (총: {currentScore})");
        
        // 목표 달성 확인
        if (targetScore > 0 && currentScore >= targetScore && previousScore < targetScore)
        {
            OnTargetScoreReached();
        }
    }
    
    /// <summary>
    /// 목표 점수 달성 처리
    /// </summary>
    protected virtual void OnTargetScoreReached()
    {
        Debug.Log($"{gameType}: 목표 점수 달성! {currentScore}/{targetScore}");
        
        // 타이머 시스템 사용 시 게임 클리어 처리
        if (useTimerSystem)
        {
            OnGameCleared();
        }
    }
    
    /// <summary>
    /// 타이머 만료 처리 (게임 오버)
    /// </summary>
    protected virtual void OnTimerExpired()
    {
        Debug.Log($"{gameType}: 타이머 만료! 게임 오버");
        OnGameOver();
    }
    
    /// <summary>
    /// 게임 클리어 처리 (성공) - MashButtonMiniGame 패턴 적용
    /// </summary>
    protected virtual void OnGameCleared()
    {
        // 이미 종료된 경우 중복 처리 방지
        if (hasEnded) return;
        
        hasEnded = true;
        isRunning = false;
        gameCompleted = true;
        gameCleared = true;
        timerRunning = false;
        gameActive = false;
        
        ShowResult("SUCCESS", "목표를 달성했습니다!");
        
        // 프리팹 사용 시 부모 오브젝트 비활성화
        if (useParentControl && transform.parent != null)
        {
            transform.parent.gameObject.SetActive(false);
        }
        
        Debug.Log($"{gameType}: 게임 클리어!");
    }
    
    /// <summary>
    /// 게임 오버 처리 (실패) - MashButtonMiniGame 패턴 적용
    /// </summary>
    protected virtual void OnGameOver()
    {
        // 이미 종료된 경우 중복 처리 방지
        if (hasEnded) return;
        
        hasEnded = true;
        isRunning = false;
        gameCompleted = true;
        gameCleared = false;
        timerRunning = false;
        gameActive = false;
        
        ShowResult("GAME OVER", "시간이 부족합니다!");
        
        // 프리팹 사용 시 부모 오브젝트 비활성화
        if (useParentControl && transform.parent != null)
        {
            transform.parent.gameObject.SetActive(false);
        }
        
        Debug.Log($"{gameType}: 게임 오버!");
    }
    
    /// <summary>
    /// 결과 화면 표시
    /// </summary>
    protected virtual void ShowResult(string resultMessage, string subMessage)
    {
        if (resultText != null)
        {
            resultText.text = $"{resultMessage}\n{subMessage}";
            resultText.gameObject.SetActive(true);
            
            // 결과에 따른 색상 변경
            if (gameCleared)
            {
                resultText.color = Color.green;  // 성공: 초록색
            }
            else
            {
                resultText.color = Color.red;    // 실패: 빨간색
            }
        }
        
        if (resultButton != null)
        {
            resultButton.gameObject.SetActive(true);
            
            // 버튼 텍스트 설정
            var buttonText = resultButton.GetComponentInChildren<UnityEngine.UI.Text>();
            if (buttonText != null)
            {
                if (gameCleared)
                {
                    buttonText.text = "다음 단계"; // 성공 시
                }
                else
                {
                    buttonText.text = "다시 시도"; // 실패 시
                }
            }
        }
    }
    
    /// <summary>
    /// 결과 화면 숨김
    /// </summary>
    protected virtual void HideResult()
    {
        if (resultText != null)
        {
            resultText.gameObject.SetActive(false);
        }
        
        if (resultButton != null)
        {
            resultButton.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 결과 버튼 클릭 처리
    /// </summary>
    protected virtual void OnResultButtonClicked()
    {
        Debug.Log($"{gameType}: 결과 버튼 클릭 - 클리어 상태: {gameCleared}");
        
        if (gameCleared)
        {
            // 성공 시: 다음 단계 (현재는 재시작)
            OnNextStage();
        }
        else
        {
            // 실패 시: 다시 시도
            RestartGame();
        }
    }
    
    /// <summary>
    /// 다음 단계 처리 (현재 미정 - 하위 클래스에서 구현)
    /// </summary>
    protected virtual void OnNextStage()
    {
        Debug.Log($"{gameType}: 다음 단계 요청 (현재는 재시작으로 대체)");
        RestartGame(); // 임시로 재시작
    }
    
    /// <summary>
    /// 게임 일시정지/재개
    /// </summary>
    public virtual void TogglePause()
    {
        gameActive = !gameActive;
        Time.timeScale = gameActive ? 1f : 0f;
    }
    
    /// <summary>
    /// 게임 재시작 (공통 + 게임별) - MashButtonMiniGame 패턴 적용
    /// </summary>
    public virtual void RestartGame()
    {
        // 상태 플래그 리셋
        isRunning = true;
        hasEnded = false;
        gameActive = true;
        gameCompleted = false;
        gameCleared = false;
        
        Time.timeScale = 1f;
        currentScore = 0;
        
        // 타이머 시스템 리셋
        if (useTimerSystem)
        {
            ResetTimer();
            StartTimer();
            HideResult();
        }
        
        // 프리팹 사용 시 부모 오브젝트 활성화
        if (useParentControl && transform.parent != null)
        {
            transform.parent.gameObject.SetActive(true);
        }
        
        RestartGameSpecific();
        
        Debug.Log($"{gameType}: 게임 재시작 - isRunning={isRunning}, hasEnded={hasEnded}");
    }
    
    /// <summary>
    /// 게임 종료 (공통 + 게임별)
    /// </summary>
    public virtual void OnGameEnd()
    {
        gameActive = false;
        Debug.Log($"{gameType}: 게임 종료! 최종 점수: {currentScore}");
        OnGameEndSpecific();
    }
    
    /// <summary>
    /// Input System 수동 설정을 위한 공개 메서드
    /// </summary>
    public void SetInputActionAsset(UnityEngine.InputSystem.InputActionAsset asset)
    {
        inputActionAsset = asset;
        SetupInputSystem();
        Debug.Log($"{gameType}: Input System 수동 설정 완료: {asset?.name ?? "null"}");
    }
    
    /// <summary>
    /// 게임 시작 (프리팹으로 사용 시 외부에서 호출) - MashButtonMiniGame 패턴 적용
    /// </summary>
    public virtual void StartMiniGame()
    {
        // 상태 플래그 리셋
        isRunning = true;
        hasEnded = false;
        gameActive = true;
        gameCompleted = false;
        gameCleared = false;
        
        Time.timeScale = 1f;
        currentScore = 0;
        
        // 타이머 시스템 리셋
        if (useTimerSystem)
        {
            ResetTimer();
            StartTimer();
            HideResult();
        }
        
        // 프리팹 사용 시 부모 오브젝트 활성화
        if (useParentControl && transform.parent != null)
        {
            transform.parent.gameObject.SetActive(true);
        }
        
        Debug.Log($"{gameType}: 미니게임 시작 - isRunning={isRunning}, hasEnded={hasEnded}");
    }
    
#if UNITY_EDITOR
    /// <summary>
    /// 디버그용 기즈모 (공통)
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        if (clockCenter == null) return;
        
        Vector3 center = clockCenter.position;
        
        // 시계 경계 (노란색)
        Gizmos.color = Color.yellow;
        DrawCircle(center, clockRadius, 64);
    }
    
    protected void DrawCircle(Vector3 center, float radius, int segments)
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
#endif
    
    protected virtual void OnDestroy()
    {
        if (moveAction != null)
        {
            moveAction.Disable();
        }
        
        // 버튼 이벤트 정리
        if (resultButton != null)
        {
            resultButton.onClick.RemoveAllListeners();
        }
    }
}