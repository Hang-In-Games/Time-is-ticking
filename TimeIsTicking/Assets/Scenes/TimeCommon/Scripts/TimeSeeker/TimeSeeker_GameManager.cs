using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TimeSeeker 게임 전용 매니저
/// GameManagerBase를 상속받아 TimeSeeker 게임 로직만 구현
/// 
/// 게임 설명:
/// - Arc 형태의 오브젝트를 랜덤하게 생성
/// - 바늘(시침)이 오브젝트 위에 일정 시간 머물면 점수 획득
/// - 난이도에 따라 오브젝트가 움직이는 패턴 변경
/// </summary>
public class TimeSeeker_GameManager : GameManagerBase
{
    // IMiniGame 인터페이스 구현
    public override GimmickType GimmickType => GimmickType.TimeBouncer;
    
    [Header("TimeSeeker Scene Objects")]
    [Tooltip("바늘 (시침)")]
    public GameObject needle;
    
    [Header("TimeSeeker Needle Settings")]
    [Tooltip("바늘 회전 속도 (도/초)")]
    public float needleRotationSpeed = 180f;
    
    [Header("TimeSeeker Difficulty Settings")]
    [Tooltip("현재 난이도")]
    public DifficultyLevel currentDifficulty = DifficultyLevel.Easy;
    
    [Tooltip("난이도별 설정")]
    public DifficultySettings easySettings;
    public DifficultySettings normalSettings;
    public DifficultySettings hardSettings;
    
    [Header("TimeSeeker Score Settings")]
    [Tooltip("점 획득 점수")]
    public int pointScore = 10;
    
    [Tooltip("선 획득 점수")]
    public int lineScore = 10;
    
    [Tooltip("실패 페널티")]
    public int failurePenalty = -5;
    
    [Tooltip("콤보 추가 점수 (배율)")]
    public float comboMultiplier = 1.5f;
    
    [Header("TimeSeeker Game Settings")]
    [Tooltip("게임 시간 (초, 0이면 무제한)")]
    public float gameTime = 60f;
    
    [Header("TimeSeeker UI References")]
    [Tooltip("난이도 텍스트")]
    public UnityEngine.UI.Text difficultyText;
    
    [Tooltip("콤보 텍스트")]
    public UnityEngine.UI.Text comboText;
    
    // TimeSeeker 전용 변수
    private float currentNeedleAngle = 0f;
    private DifficultySettings currentSettings;
    private List<ArcObject> activeObjects = new List<ArcObject>();
    private Transform objectContainer;
    private float nextSpawnTime;
    private int currentCombo = 0;
    private ArcObject trackedObject = null;
    private float trackingTime = 0f;
    
    // 베이스 클래스에서 요구하는 추상 메서드 구현
    protected override void InitializeGameSpecific()
    {
        // TimeSeeker는 점수 시스템 사용
        useScoreSystem = true;
        
        ValidateTimeSeekerReferences();
        InitializeSettings();
        CreateObjectContainer();
        InitializeGame();
        
        // 첫 번째 오브젝트 생성
        if (activeObjects.Count == 0)
        {
            SpawnRandomObject();
        }
        
        // TimeSeeker는 needle이 중심에서 회전하므로 경계 처리 불필요
        // 하지만 베이스 클래스 호환성을 위해 needle 등록
        // RegisterBallForBoundary(needle);
    }
    
    protected override void UpdateGameSpecific()
    {
        UpdateTimer();
        CheckObjectSpawning();
        CheckNeedleCollision();
        UpdateTimeSeekerUI();
    }
    
    protected override void HandleGameSpecificInput()
    {
        HandleNeedleInput();
        HandleTimeSeekerDebugInput();
    }
    
    protected override void OnGameEndSpecific()
    {
        // 모든 오브젝트 제거
        foreach (var obj in activeObjects)
        {
            if (obj != null)
            {
                Destroy(obj.gameObject);
            }
        }
        activeObjects.Clear();
    }
    
    protected override void RestartGameSpecific()
    {
        // 모든 오브젝트 제거
        foreach (var obj in activeObjects)
        {
            if (obj != null)
            {
                Destroy(obj.gameObject);
            }
        }
        activeObjects.Clear();
        
        InitializeGame();
    }

    /// <summary>
    /// StartMiniGame 오버라이드 - 프리팹 모드에서 첫 오브젝트 생성
    /// </summary>
    public override void StartMiniGame()
    {
        base.StartMiniGame();

        // 프리팹 모드에서 게임 시작 시 첫 오브젝트 생성
        if (usePrefabMode && activeObjects.Count == 0)
        {
            Debug.Log($"{gameType}: 프리팹 모드 - 첫 오브젝트 생성");
            SpawnRandomObject();
        }
    }
    
    /// <summary>
    /// TimeSeeker 전용 참조 검증
    /// </summary>
    void ValidateTimeSeekerReferences()
    {
        if (needle == null)
        {
            Debug.LogError("TimeSeeker: Needle이 할당되지 않았습니다!");
        }
    }
    
    /// <summary>
    /// 난이도 설정 초기화
    /// </summary>
    void InitializeSettings()
    {
        if (easySettings.level == 0 && easySettings.spawnInterval == 0)
            easySettings = DifficultySettings.CreateEasy();
        
        if (normalSettings.level == 0 && normalSettings.spawnInterval == 0)
            normalSettings = DifficultySettings.CreateNormal();
        
        if (hardSettings.level == 0 && hardSettings.spawnInterval == 0)
            hardSettings = DifficultySettings.CreateHard();
        
        UpdateDifficulty(currentDifficulty);
    }
    
    /// <summary>
    /// 오브젝트 컨테이너 생성
    /// </summary>
    void CreateObjectContainer()
    {
        GameObject container = new GameObject("ArcObjects");
        container.transform.parent = transform;
        container.transform.localPosition = Vector3.zero;
        objectContainer = container.transform;
    }
    
    /// <summary>
    /// TimeSeeker 게임 초기화
    /// </summary>
    void InitializeGame()
    {
        currentCombo = 0;
        remainingTime = gameTime;
        nextSpawnTime = Time.time + currentSettings.spawnInterval;
        
        // 바늘 초기 각도
        if (needle != null)
        {
            currentNeedleAngle = needle.transform.eulerAngles.z;
        }
        
        Debug.Log($"TimeSeeker: 게임 초기화 완료 - 난이도: {currentDifficulty}");
    }
    
    /// <summary>
    /// 바늘 입력 처리
    /// </summary>
    void HandleNeedleInput()
    {
        if (needle == null) return;
        
        float input = ReadInputValue();
        
        if (Mathf.Abs(input) > 0.05f)
        {
            float rotationDelta = input * needleRotationSpeed * Time.deltaTime;
            currentNeedleAngle += rotationDelta;
            currentNeedleAngle = NormalizeAngle(currentNeedleAngle);
            
            needle.transform.rotation = Quaternion.Euler(0, 0, currentNeedleAngle);
            
            // 디버그 로그 (처음 5초만)
            if (Time.time < 5f)
            {
                Debug.Log($"TimeSeeker: 바늘 회전: input={input:F2}, 각도={currentNeedleAngle:F1}°");
            }
        }
    }
    
    /// <summary>
    /// TimeSeeker 전용 디버그 입력
    /// </summary>
    void HandleTimeSeekerDebugInput()
    {
        // Space: 즉시 오브젝트 생성
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space))
        {
            SpawnRandomObject();
            Debug.Log("TimeSeeker: Space 키 - 오브젝트 수동 생성");
        }
        
        // C: 모든 오브젝트 제거
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.C))
        {
            foreach (var obj in activeObjects)
            {
                if (obj != null)
                {
                    Destroy(obj.gameObject);
                }
            }
            activeObjects.Clear();
            Debug.Log("TimeSeeker: C 키 - 모든 오브젝트 제거");
        }
    }
    
    /// <summary>
    /// 타이머 업데이트
    /// </summary>
    void UpdateTimer()
    {
        if (gameTime <= 0) return;
        
        remainingTime -= Time.deltaTime;
        
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            OnGameEnd();
        }
    }
    
    /// <summary>
    /// 오브젝트 스폰 체크
    /// </summary>
    void CheckObjectSpawning()
    {
        if (Time.time >= nextSpawnTime && activeObjects.Count < currentSettings.maxActiveObjects)
        {
            SpawnRandomObject();
            nextSpawnTime = Time.time + currentSettings.spawnInterval;
        }
    }
    
    /// <summary>
    /// 랜덤 오브젝트 생성
    /// </summary>
    void SpawnRandomObject()
    {
        // 랜덤 타입 선택
        ArcObjectType type = Random.value > 0.5f ? ArcObjectType.Point : ArcObjectType.Line;
        
        // 랜덤 각도
        float startAngle = Random.Range(0f, 360f);
        float endAngle = type == ArcObjectType.Line ? startAngle + Random.Range(15f, 45f) : startAngle;
        
        // 밝고 선명한 색상 생성
        Color color = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.9f, 1f);
        
        // 오브젝트 생성
        GameObject obj = new GameObject($"ArcObject_{type}_{activeObjects.Count}");
        obj.transform.parent = objectContainer;
        obj.transform.position = clockCenter.position;
        
        ArcObject arcObject = obj.AddComponent<ArcObject>();
        
        // 데이터 설정
        ArcObjectData data = new ArcObjectData(type, startAngle, clockRadius, color);
        data.endAngle = endAngle;
        data.movementPattern = new MovementPattern
        {
            type = currentSettings.defaultMovementPattern.type,
            rotationSpeed = currentSettings.defaultMovementPattern.rotationSpeed,
            oscillationAmplitude = currentSettings.defaultMovementPattern.oscillationAmplitude,
            oscillationFrequency = currentSettings.defaultMovementPattern.oscillationFrequency,
            randomness = currentSettings.defaultMovementPattern.randomness
        };
        
        arcObject.Initialize(data, clockCenter);
        activeObjects.Add(arcObject);
        
        Debug.Log($"TimeSeeker: 오브젝트 생성 완료 - Type: {type}, Angle: {startAngle:F1}°~{endAngle:F1}°, Active Objects: {activeObjects.Count}");
    }
    
    /// <summary>
    /// 바늘 충돌 체크
    /// </summary>
    void CheckNeedleCollision()
    {
        float normalizedAngle = NormalizeAngle(currentNeedleAngle);
        bool foundOverlap = false;
        
        foreach (var obj in activeObjects)
        {
            if (obj == null) continue;
            
            bool isOverlapping = obj.IsOverlappingWithNeedle(normalizedAngle, currentSettings.angleTolerance);
            
            if (isOverlapping)
            {
                foundOverlap = true;
                
                if (trackedObject != obj)
                {
                    // 새로운 오브젝트 추적 시작
                    if (trackedObject != null)
                    {
                        trackedObject.SetActive(false);
                    }
                    trackedObject = obj;
                    trackingTime = 0f;
                }
                
                trackedObject.SetActive(true);
                trackingTime += Time.deltaTime;
                
                // 일정 시간 유지하면 점수 획득
                if (trackingTime >= currentSettings.dwellTime)
                {
                    CollectObject(trackedObject);
                    break;
                }
            }
            else
            {
                obj.SetActive(false);
            }
        }
        
        // 겹치는 오브젝트가 없으면 추적 초기화
        if (!foundOverlap)
        {
            if (trackedObject != null)
            {
                trackedObject.SetActive(false);
                trackedObject = null;
            }
            trackingTime = 0f;
        }
    }
    
    /// <summary>
    /// 오브젝트 수집 (점수 획득)
    /// </summary>
    void CollectObject(ArcObject obj)
    {
        if (obj == null) return;
        
        // 점수 계산
        int baseScore = obj.objectType == ArcObjectType.Point ? pointScore : lineScore;
        int finalScore = Mathf.RoundToInt(baseScore * (1 + currentCombo * (comboMultiplier - 1)));
        
        currentScore += finalScore;
        currentCombo++;
        
        Debug.Log($"TimeSeeker: 오브젝트 수집! +{finalScore} (콤보: {currentCombo}x) - 총점: {currentScore}");
        
        // 오브젝트 제거
        activeObjects.Remove(obj);
        Destroy(obj.gameObject);
        
        // 추적 초기화
        trackedObject = null;
        trackingTime = 0f;
        
        // 목표 점수 달성 확인
        if (targetScore > 0 && currentScore >= targetScore)
        {
            OnGameEnd();
        }
    }
    
    /// <summary>
    /// 난이도 변경
    /// </summary>
    public void UpdateDifficulty(DifficultyLevel newDifficulty)
    {
        currentDifficulty = newDifficulty;
        
        switch (currentDifficulty)
        {
            case DifficultyLevel.Easy:
                currentSettings = easySettings;
                break;
            case DifficultyLevel.Normal:
                currentSettings = normalSettings;
                break;
            case DifficultyLevel.Hard:
                currentSettings = hardSettings;
                break;
        }
        
        Debug.Log($"TimeSeeker: 난이도 변경: {currentDifficulty}");
    }
    
    /// <summary>
    /// TimeSeeker 전용 UI 업데이트
    /// </summary>
    void UpdateTimeSeekerUI()
    {
        if (difficultyText != null)
        {
            difficultyText.text = $"Difficulty: {currentDifficulty}";
        }
        
        if (timerText != null && gameTime > 0)
        {
            timerText.text = $"Time: {Mathf.CeilToInt(remainingTime)}s";
        }
        
        if (comboText != null)
        {
            if (currentCombo > 0)
            {
                comboText.text = $"Combo: {currentCombo}x";
                comboText.gameObject.SetActive(true);
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }
    }
    

#if UNITY_EDITOR
    /// <summary>
    /// TimeSeeker 전용 디버그 기즈모
    /// </summary>
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos(); // 공통 기즈모 그리기
        
        if (clockCenter == null) return;
        Vector3 center = clockCenter.position;
        
        // 바늘 방향 (빨간색)
        if (Application.isPlaying && needle != null)
        {
            Gizmos.color = Color.red;
            float angle = currentNeedleAngle * Mathf.Deg2Rad;
            Vector3 needleEnd = center + new Vector3(Mathf.Cos(angle) * clockRadius, Mathf.Sin(angle) * clockRadius, 0);
            Gizmos.DrawLine(center, needleEnd);
        }
        
        // 활성 오브젝트들 (매젠타색)
        if (Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            foreach (var obj in activeObjects)
            {
                if (obj != null)
                {
                    float objAngle = obj.GetCurrentAngle() * Mathf.Deg2Rad;
                    Vector3 objPos = center + new Vector3(Mathf.Cos(objAngle) * clockRadius, Mathf.Sin(objAngle) * clockRadius, 0);
                    Gizmos.DrawWireSphere(objPos, 10f);
                }
            }
        }
    }
#endif
}