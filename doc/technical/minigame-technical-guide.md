# 미니게임 기술 구현 가이드

## 📋 문서 개요
- **프로젝트**: Time is Ticking
- **엔진**: Unity 6 (6000.2.10f1)
- **목적**: 미니게임 제작을 위한 기술적 가이드라인 제공

---

## 🏗️ 프로젝트 구조

### 기존 시스템 분석

#### 1. 타임 이벤트 시스템
```
TimeEventDefine.cs - 이벤트 정의 (ScriptableObject)
├─ TimeEventType (Show/Hide)
├─ targetName
└─ Trigger 상태 관리

TimeEventCollection.cs - 이벤트 컬렉션 관리
└─ Dictionary<int, TimeEventDefine>

TimeEventTarget.cs - 이벤트 수신 오브젝트
└─ InvokeTimeEvent() 메서드

TimeEventTargetManager.cs - 이벤트 관리자
├─ DigitalClock 연동
└─ 씬의 모든 TimeEventTarget 관리
```

**미니게임 활용 방안**:
- 미니게임 진입/종료를 TimeEvent로 처리
- 미니게임 보상을 TimeEvent로 메인 게임에 전달
- 특정 시간대에 미니게임 활성화

#### 2. 디지털 시계 시스템
```
DigitalClock.cs
├─ 시간 흐름 제어 (실시간 또는 가속)
├─ 타임루프 리셋 이벤트
└─ OnReset 이벤트 제공
```

**미니게임 활용 방안**:
- 미니게임 내 시간 제한 기능
- 시간 조작 메커니즘의 기반
- 타임루프와 동기화

---

## 🎮 미니게임 공통 아키텍처

### 권장 구조: Manager-Centric Design
기존 TimeBouncer가 사용하는 중앙 관리 방식을 따릅니다.

```
MiniGameBase (추상 클래스)
├─ Initialize() - 초기화
├─ StartGame() - 게임 시작
├─ PauseGame() - 일시정지
├─ ResumeGame() - 재개
├─ EndGame() - 종료
└─ GetGameResult() - 결과 반환

구체적 미니게임 (MiniGameBase 상속)
└─ GameManager_[게임명].cs
    ├─ [Scene Objects] - Inspector 할당
    ├─ [Game Settings] - 게임 설정값
    └─ [Game Logic] - 게임 로직 메서드
```

### 필수 컴포넌트

#### 1. MiniGameManager (씬 단위)
```csharp
public abstract class MiniGameManager : MonoBehaviour
{
    [Header("Scene Objects")]
    // 씬 오브젝트 참조 (Inspector 할당)
    
    [Header("Game Settings")]
    // 게임 설정값
    
    [Header("Game State")]
    protected GameState currentState;
    protected float gameTime;
    protected int score;
    
    // 추상 메서드 (하위 클래스에서 구현)
    protected abstract void InitializeGame();
    protected abstract void UpdateGameLogic();
    protected abstract bool CheckWinCondition();
    protected abstract bool CheckLoseCondition();
    
    // 공통 메서드
    public virtual void StartGame() { }
    public virtual void PauseGame() { }
    public virtual void EndGame() { }
}
```

#### 2. MiniGameLauncher (전역)
```csharp
public class MiniGameLauncher : MonoBehaviour
{
    public static MiniGameLauncher Instance { get; private set; }
    
    // 미니게임 씬 관리
    public void LoadMiniGame(string miniGameName) { }
    public void UnloadMiniGame() { }
    
    // 보상 처리
    public void ProcessReward(MiniGameResult result) { }
}
```

#### 3. MiniGameResult (데이터 구조)
```csharp
[System.Serializable]
public class MiniGameResult
{
    public string miniGameName;
    public bool isCleared;
    public int score;
    public float completionTime;
    public Dictionary<string, object> additionalData;
}
```

---

## 🔧 Unity 6 특화 기능 활용

### 1. Universal Render Pipeline (URP)
```
URP 설정 파일 위치:
TimeIsTicking/Assets/Settings/

활용:
- 2D Renderer 사용
- 2D Lights (Normal maps 활용)
- Post-processing (시간 조작 효과)
- Sprite Atlas 최적화
```

### 2. New Input System
```
기존 Input Actions:
TimeIsTicking/Assets/InputSystem_Actions.inputactions

확장 방법:
1. Input Actions 에셋 수정
2. 각 미니게임별 Action Map 추가
3. PlayerInput 컴포넌트 활용
```

**예시 - 시간 조작 입력**:
```csharp
[Header("Input Actions")]
public InputActionReference timeRewindAction;
public InputActionReference timeForwardAction;
public InputActionReference timeStopAction;

private void OnEnable()
{
    timeRewindAction.action.performed += OnTimeRewind;
    timeForwardAction.action.performed += OnTimeForward;
    timeStopAction.action.performed += OnTimeStop;
}
```

### 3. Scene Management
```csharp
// Additive Scene Loading (미니게임 비동기 로딩)
public IEnumerator LoadMiniGameAsync(string sceneName)
{
    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(
        sceneName, 
        LoadSceneMode.Additive
    );
    
    while (!asyncLoad.isDone)
    {
        // 로딩 진행률 표시
        float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
        UpdateLoadingUI(progress);
        yield return null;
    }
    
    // 씬 활성화
    SceneManager.SetActiveScene(
        SceneManager.GetSceneByName(sceneName)
    );
}
```

### 4. Addressables (선택적)
Unity 6에서 강화된 에셋 관리 시스템 활용
```
장점:
- 미니게임 에셋 동적 로딩
- 메모리 효율성
- 빌드 크기 최적화

구조:
Assets/MiniGames/
├─ Common/ (공통 리소스)
└─ [게임명]/
    ├─ Sprites/
    ├─ Prefabs/
    ├─ Audio/
    └─ Scenes/
```

---

## 🎯 타임루프 메커니즘 구현

### 1. 상태 저장 시스템
```csharp
public class TimelineStateRecorder
{
    private Dictionary<float, GameState> stateHistory;
    private float recordInterval = 0.1f; // 100ms마다 기록
    
    public void RecordState(float timestamp, GameState state)
    {
        if (!stateHistory.ContainsKey(timestamp))
        {
            stateHistory[timestamp] = state.Clone();
        }
    }
    
    public GameState GetStateAt(float timestamp)
    {
        // 가장 가까운 타임스탬프 찾기
        float closestTime = stateHistory.Keys
            .OrderBy(t => Mathf.Abs(t - timestamp))
            .FirstOrDefault();
            
        return stateHistory[closestTime];
    }
    
    public void ClearHistory()
    {
        stateHistory.Clear();
    }
}
```

### 2. 시간 조작 컨트롤러
```csharp
public class TimeManipulationController
{
    private float currentTime;
    private float timeScale = 1f;
    private bool isReversing = false;
    
    public void SetTimeScale(float scale)
    {
        timeScale = Mathf.Clamp(scale, -2f, 5f);
        Time.timeScale = Mathf.Abs(timeScale);
        isReversing = timeScale < 0;
    }
    
    public void StopTime()
    {
        Time.timeScale = 0;
    }
    
    public void ResumeTime()
    {
        Time.timeScale = Mathf.Abs(timeScale);
    }
    
    public void JumpToTime(float targetTime)
    {
        // 타임라인의 특정 시점으로 이동
        currentTime = targetTime;
        // 상태 복원 로직
    }
}
```

### 3. 오브젝트 상태 스냅샷
```csharp
[System.Serializable]
public class ObjectSnapshot
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public bool isActive;
    public Dictionary<string, object> customData;
    
    public static ObjectSnapshot Capture(GameObject obj)
    {
        return new ObjectSnapshot
        {
            position = obj.transform.position,
            rotation = obj.transform.rotation,
            scale = obj.transform.localScale,
            isActive = obj.activeInHierarchy
        };
    }
    
    public void Restore(GameObject obj)
    {
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.localScale = scale;
        obj.SetActive(isActive);
    }
}
```

---

## 🎨 물리 시뮬레이션 (Physics 2D)

### TimeBouncer 스타일 물리
```csharp
// 기본 설정
public class PhysicsSetup
{
    public static void ConfigureForTimeBouncer(GameObject obj)
    {
        Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0; // 중력 없음
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.linearDamping = 0; // 공기저항 없음
        rb.angularDamping = 0;
        
        PhysicsMaterial2D material = new PhysicsMaterial2D();
        material.bounciness = 1.0f; // 완전 탄성 충돌
        material.friction = 0f;
        
        CircleCollider2D collider = obj.AddComponent<CircleCollider2D>();
        collider.sharedMaterial = material;
    }
}
```

### 원형 경계 충돌 감지
```csharp
public class CircularBoundaryChecker
{
    private Vector3 centerPoint;
    private float radius;
    
    public bool IsInsideBoundary(Vector3 position, float objectRadius)
    {
        float distance = Vector3.Distance(position, centerPoint);
        return distance + objectRadius <= radius;
    }
    
    public Vector3 ClampToBoundary(Vector3 position, float objectRadius)
    {
        Vector3 direction = (position - centerPoint).normalized;
        float maxDistance = radius - objectRadius;
        return centerPoint + direction * Mathf.Min(
            Vector3.Distance(position, centerPoint), 
            maxDistance
        );
    }
    
    public Vector3 GetReflectionDirection(Vector3 position, Vector3 velocity)
    {
        Vector3 normal = (position - centerPoint).normalized;
        return Vector3.Reflect(velocity, normal);
    }
}
```

---

## 📊 성능 최적화

### 1. 오브젝트 풀링
```csharp
public class ObjectPool
{
    private GameObject prefab;
    private Queue<GameObject> pool = new Queue<GameObject>();
    private Transform poolParent;
    
    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return GameObject.Instantiate(prefab, poolParent);
    }
    
    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
```

### 2. 타임라인 히스토리 메모리 관리
```csharp
public class HistoryManager
{
    private int maxHistorySize = 300; // 5분 * 60초
    private LinkedList<StateSnapshot> history;
    
    public void AddState(StateSnapshot state)
    {
        history.AddLast(state);
        
        // 오래된 히스토리 제거
        while (history.Count > maxHistorySize)
        {
            history.RemoveFirst();
        }
    }
}
```

### 3. LOD (Level of Detail) - 선택적
복잡한 비주얼 효과를 거리에 따라 조절
```csharp
public class SimpleLOD : MonoBehaviour
{
    public ParticleSystem detailedEffect;
    public ParticleSystem simpleEffect;
    public float switchDistance = 10f;
    
    void Update()
    {
        float dist = Vector3.Distance(
            transform.position, 
            Camera.main.transform.position
        );
        
        if (dist > switchDistance)
        {
            detailedEffect.Stop();
            simpleEffect.Play();
        }
        else
        {
            simpleEffect.Stop();
            detailedEffect.Play();
        }
    }
}
```

---

## 🔒 데이터 저장 및 로드

### Save System
```csharp
[System.Serializable]
public class MiniGameSaveData
{
    public string miniGameName;
    public int highScore;
    public bool isUnlocked;
    public bool isCompleted;
    public float bestTime;
    public SerializableDictionary<string, object> customData;
}

public class SaveManager
{
    private const string SAVE_FILE = "minigame_progress.json";
    
    public void SaveProgress(MiniGameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(
            Application.persistentDataPath, 
            SAVE_FILE
        );
        File.WriteAllText(path, json);
    }
    
    public MiniGameSaveData LoadProgress()
    {
        string path = Path.Combine(
            Application.persistentDataPath, 
            SAVE_FILE
        );
        
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<MiniGameSaveData>(json);
        }
        
        return new MiniGameSaveData();
    }
}
```

---

## 🐛 디버깅 도구

### Gizmos를 활용한 시각화
```csharp
private void OnDrawGizmos()
{
    if (showDebugGizmos)
    {
        // 경계선 표시
        Gizmos.color = Color.yellow;
        DrawCircle(centerPoint, boundaryRadius, 64);
        
        // 오브젝트 위치
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(objectPosition, 0.5f);
        
        // 속도 벡터
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(objectPosition, objectPosition + velocity);
    }
}

private void DrawCircle(Vector3 center, float radius, int segments)
{
    float angleStep = 360f / segments;
    Vector3 prevPoint = center + new Vector3(radius, 0, 0);
    
    for (int i = 1; i <= segments; i++)
    {
        float angle = i * angleStep * Mathf.Deg2Rad;
        Vector3 newPoint = center + new Vector3(
            Mathf.Cos(angle) * radius,
            Mathf.Sin(angle) * radius,
            0
        );
        Gizmos.DrawLine(prevPoint, newPoint);
        prevPoint = newPoint;
    }
}
```

### 콘솔 로깅 시스템
```csharp
public static class GameLogger
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogGameState(string message)
    {
        Debug.Log($"[GameState] {message}");
    }
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogTimelineEvent(string message)
    {
        Debug.Log($"[Timeline] {message}");
    }
    
    public static void LogError(string message)
    {
        Debug.LogError($"[Error] {message}");
    }
}
```

---

## 📱 플랫폼 고려사항

### 터치 입력 지원
```csharp
public class InputHandler
{
    public Vector2 GetInputPosition()
    {
        #if UNITY_STANDALONE || UNITY_EDITOR
            return Input.mousePosition;
        #elif UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount > 0)
                return Input.GetTouch(0).position;
            return Vector2.zero;
        #endif
    }
    
    public bool GetInputDown()
    {
        #if UNITY_STANDALONE || UNITY_EDITOR
            return Input.GetMouseButtonDown(0);
        #elif UNITY_IOS || UNITY_ANDROID
            return Input.touchCount > 0 && 
                   Input.GetTouch(0).phase == TouchPhase.Began;
        #endif
    }
}
```

### 해상도 대응
```csharp
public class ResolutionManager : MonoBehaviour
{
    private void Start()
    {
        // 카메라 Orthographic Size 자동 조정
        float targetAspect = 16f / 9f;
        float currentAspect = (float)Screen.width / Screen.height;
        
        Camera mainCamera = Camera.main;
        float baseSize = 6f;
        
        if (currentAspect < targetAspect)
        {
            // 세로가 더 긴 경우 (모바일)
            mainCamera.orthographicSize = baseSize / (currentAspect / targetAspect);
        }
    }
}
```

---

## 🧪 테스트 가이드라인

### 유닛 테스트 (Unity Test Framework)
```csharp
[Test]
public void TimeRewind_RestoresCorrectState()
{
    // Arrange
    TimelineStateRecorder recorder = new TimelineStateRecorder();
    GameState initialState = new GameState { score = 100 };
    recorder.RecordState(0f, initialState);
    
    GameState changedState = new GameState { score = 200 };
    recorder.RecordState(1f, changedState);
    
    // Act
    GameState restoredState = recorder.GetStateAt(0f);
    
    // Assert
    Assert.AreEqual(100, restoredState.score);
}
```

### 통합 테스트
```csharp
[UnityTest]
public IEnumerator MiniGame_CompletesSuccessfully()
{
    // 씬 로드
    SceneManager.LoadScene("TestMiniGame");
    yield return null;
    
    // 게임 시작
    MiniGameManager manager = GameObject.FindObjectOfType<MiniGameManager>();
    manager.StartGame();
    
    // 게임 진행 시뮬레이션
    yield return new WaitForSeconds(5f);
    
    // 결과 확인
    Assert.IsTrue(manager.IsGameCompleted());
}
```

---

## 📚 참고 리소스

### Unity 6 공식 문서
- [URP 2D Setup Guide](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest)
- [New Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest)
- [Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@latest)

### 추천 에셋
- **Odin Inspector**: 이미 프로젝트에 포함됨, Inspector 확장
- **DOTween**: 트윈 애니메이션
- **TextMesh Pro**: UI 텍스트 (이미 포함)

---

## 🎯 체크리스트

미니게임 구현 시 확인할 사항:

- [ ] MiniGameManager 상속 및 추상 메서드 구현
- [ ] Scene Objects를 Inspector에서 할당 (None 없이)
- [ ] Input System Actions 설정
- [ ] Physics 설정 (Rigidbody2D, Collider)
- [ ] TimeEvent 연동 (메인 게임과 통신)
- [ ] 저장/로드 기능 구현
- [ ] Gizmos 디버깅 도구 활용
- [ ] 성능 프로파일링 (60 FPS 유지)
- [ ] 다양한 해상도 테스트
- [ ] 터치 입력 테스트 (모바일)
- [ ] 메모리 누수 확인
- [ ] 빌드 테스트

---

## 결론

이 기술 가이드는 Unity 6 기반 Time is Ticking 프로젝트에서 미니게임을 제작할 때 필요한 핵심 기술 사항들을 정리한 것입니다. 기존 프로젝트 구조와 호환성을 유지하면서 확장 가능한 아키텍처를 제공하는 것을 목표로 합니다.

**핵심 원칙**:
1. Manager-Centric Design 유지
2. 기존 TimeEvent 시스템 활용
3. Unity 6 기능 적극 활용
4. 성능 최적화 고려
5. 크로스 플랫폼 대응
