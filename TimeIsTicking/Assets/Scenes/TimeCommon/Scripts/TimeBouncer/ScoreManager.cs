using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스코어 관리 시스템
/// GameManager가 생성하고 관리함
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [Tooltip("현재 점수")]
    public int currentScore = 0;
    
    [Tooltip("목표 점수 (0이면 무제한)")]
    public int targetScore = 0;
    
    [Header("Item Spawn Settings")]
    [Tooltip("아이템 스폰 간격 (초)")]
    public float spawnInterval = 3f;
    
    [Tooltip("최대 동시 아이템 수")]
    public int maxActiveItems = 3;
    
    [Tooltip("스폰 가능 영역 반지름 (시계 중심 기준)")]
    public float spawnRadius = 150f;
    
    [Tooltip("스폰 최소 거리 (중심으로부터)")]
    public float spawnMinRadius = 50f;
    
    [Header("Collision Distance Settings")]
    [Tooltip("모든 ScoreItem의 트리거 거리 배수 (기본값 오버라이드)")]
    [Range(0.01f, 1.0f)]
    public float globalTriggerDistanceMultiplier = 0.05f;
    
    [Tooltip("모든 ScoreItem의 고정 여유 거리 (기본값 오버라이드)")]
    [Range(0f, 2f)]
    public float globalFixedMarginDistance = 0.3f;
    
    [Tooltip("거리 설정을 개별 ScoreItem에 적용할지 여부")]
    public bool useGlobalDistanceSettings = true;
    
    [Header("Item Types")]
    [Tooltip("스코어 아이템 타입 설정")]
    public List<ScoreItemData> itemTypes = new List<ScoreItemData>();
    
    // 참조
    private Transform clockCenter;
    private Transform itemContainer;  // 아이템들의 부모 오브젝트
    private List<GameObject> activeItems = new List<GameObject>();
    private float nextSpawnTime;
    
    // 이벤트
    public System.Action<ScoreEventArgs> OnScoreChanged;
    public System.Action<int> OnTargetReached;
    
    void Start()
    {
        InitializeDefaultItems();
        CreateItemContainer();
        nextSpawnTime = Time.time + spawnInterval;
    }
    
    void Update()
    {
        // 자동 스폰
        if (Time.time >= nextSpawnTime && activeItems.Count < maxActiveItems)
        {
            SpawnRandomItem();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }
    
    /// <summary>
    /// 초기화 (GameManager에서 호출)
    /// </summary>
    public void Initialize(Transform center)
    {
        clockCenter = center;
    }
    
    /// <summary>
    /// 기본 아이템 타입 초기화
    /// </summary>
    void InitializeDefaultItems()
    {
        if (itemTypes.Count == 0)
        {
            itemTypes.Add(new ScoreItemData(ScoreItemType.Normal, 1, 0.6f, Color.white));
            itemTypes.Add(new ScoreItemData(ScoreItemType.Silver, 3, 0.25f, Color.gray));
            itemTypes.Add(new ScoreItemData(ScoreItemType.Gold, 5, 0.1f, Color.yellow));
            itemTypes.Add(new ScoreItemData(ScoreItemType.Bonus, 10, 0.05f, new Color(1f, 0.5f, 0f)));
        }
    }
    
    /// <summary>
    /// 아이템 컨테이너 생성
    /// </summary>
    void CreateItemContainer()
    {
        GameObject container = new GameObject("ScoreItems");
        container.transform.parent = transform;
        container.transform.localPosition = Vector3.zero;
        itemContainer = container.transform;
    }
    
    /// <summary>
    /// 랜덤 아이템 스폰
    /// </summary>
    public GameObject SpawnRandomItem()
    {
        if (itemTypes.Count == 0) return null;
        
        // 확률에 따라 아이템 타입 선택
        ScoreItemData selectedData = SelectItemByProbability();
        if (selectedData == null)
        {
            Debug.LogWarning("스폰할 아이템을 선택할 수 없습니다.");
            return null;
        }
        
        // 랜덤 위치 생성
        Vector2 spawnPosition = GetRandomSpawnPosition();
        
        // 아이템 생성 (prefab이 있으면 사용, 없으면 기본 생성)
        GameObject item;
        bool usingPrefab = false;
        
        if (selectedData.prefab != null)
        {
            item = Instantiate(selectedData.prefab, spawnPosition, Quaternion.identity, itemContainer);
            usingPrefab = true;
            
            // 프리팹의 기존 컴포넌트 정보 출력
            var existingColliders = item.GetComponents<Collider2D>();
            var existingRenderer = item.GetComponent<SpriteRenderer>();
            
            Debug.Log($"✅ 프리팹 사용: {selectedData.prefab.name}");
            Debug.Log($"   - Collider 수: {existingColliders.Length}");
            foreach (var col in existingColliders)
            {
                Debug.Log($"     * {col.GetType().Name}: IsTrigger={col.isTrigger}");
            }
            Debug.Log($"   - SpriteRenderer: {existingRenderer != null}");
        }
        else
        {
            item = CreateDefaultScoreItem(selectedData, spawnPosition);
            Debug.Log($"⚠️ 기본 아이템 생성 (프리팹 없음): {selectedData.type}");
        }
        
        // ScoreItem 컴포넌트 확인 및 추가
        ScoreItem scoreItem = item.GetComponent<ScoreItem>();
        if (scoreItem == null)
        {
            scoreItem = item.AddComponent<ScoreItem>();
            Debug.Log($"🔧 ScoreItem 컴포넌트 추가: {item.name}");
        }
        else
        {
            Debug.Log($"✅ 기존 ScoreItem 컴포넌트 사용: {item.name}");
        }
        
        // 초기화 (프리팹의 기존 설정을 최대한 활용)
        scoreItem.Initialize(selectedData.type, selectedData.score, selectedData.itemColor, this);
        
        activeItems.Add(item);
        Debug.Log($"🎯 아이템 스폰 완료 - Type: {selectedData.type}, Position: {spawnPosition}, " +
                  $"프리팹사용: {usingPrefab}, 활성아이템수: {activeItems.Count}");
        
        return item;
    }
    
    /// <summary>
    /// 확률에 따라 아이템 선택
    /// </summary>
    ScoreItemData SelectItemByProbability()
    {
        float totalProbability = 0f;
        foreach (var data in itemTypes)
        {
            totalProbability += data.spawnProbability;
        }
        
        float randomValue = Random.Range(0f, totalProbability);
        float cumulative = 0f;
        
        foreach (var data in itemTypes)
        {
            cumulative += data.spawnProbability;
            if (randomValue <= cumulative)
            {
                return data;
            }
        }
        
        return itemTypes[0];  // 기본값
    }
    
    /// <summary>
    /// 랜덤 스폰 위치 생성 (시계 내부, 다른 오브젝트와 충돌 방지)
    /// </summary>
    Vector2 GetRandomSpawnPosition()
    {
        Vector2 center = clockCenter != null ? (Vector2)clockCenter.position : Vector2.zero;
        Vector2 spawnPosition = center;
        
        // 최대 10번 시도하여 적절한 위치 찾기
        for (int attempts = 0; attempts < 10; attempts++)
        {
            // 랜덤 각도와 거리
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(spawnMinRadius, spawnRadius);
            
            // 원형 내부의 랜덤 위치
            Vector2 offset = new Vector2(
                Mathf.Cos(angle) * distance,
                Mathf.Sin(angle) * distance
            );
            
            spawnPosition = center + offset;
            
            // 해당 위치에 다른 오브젝트가 있는지 확인
            Collider2D existingCollider = Physics2D.OverlapCircle(spawnPosition, 1f);
            
            if (existingCollider == null || existingCollider.CompareTag("ScoreItem"))
            {
                // 적절한 위치 발견
                Debug.Log($"스폰 위치 확정 - 위치: {spawnPosition}, 시도: {attempts + 1}");
                break;
            }
            else
            {
                Debug.Log($"스폰 위치 충돌 감지 - {existingCollider.name}, 재시도: {attempts + 1}");
            }
        }
        
        return spawnPosition;
    }
    
    /// <summary>
    /// 아이템 획득 처리
    /// </summary>
    public void CollectItem(ScoreItem item)
    {
        if (item == null) 
        {
            Debug.LogWarning("CollectItem: item이 null입니다!");
            return;
        }
        
        // 중복 획득 방지
        if (!activeItems.Contains(item.gameObject))
        {
            Debug.LogWarning($"CollectItem: {item.name}이 활성 아이템 목록에 없습니다! (중복 획득 시도?)");
            return;
        }
        
        // 점수 추가
        int previousScore = currentScore;
        currentScore += item.scoreValue;
        
        // 이벤트 발생
        var eventArgs = new ScoreEventArgs(
            item.itemType,
            item.scoreValue,
            currentScore,
            item.transform.position
        );
        OnScoreChanged?.Invoke(eventArgs);
        
        Debug.Log($"✅ 점수 획득 성공! 타입: {item.itemType}, +{item.scoreValue} (총: {currentScore}), 위치: {item.transform.position}");
        
        // 목표 달성 확인
        if (targetScore > 0 && currentScore >= targetScore && previousScore < targetScore)
        {
            OnTargetReached?.Invoke(currentScore);
            Debug.Log($"🎯 목표 점수 달성! {currentScore}/{targetScore}");
        }
        
        // 활성 아이템 리스트에서 제거
        activeItems.Remove(item.gameObject);
        Debug.Log($"활성 아이템 수: {activeItems.Count}");
    }
    
    /// <summary>
    /// 점수 초기화
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        
        // 모든 활성 아이템 제거
        foreach (var item in activeItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        activeItems.Clear();
        
        Debug.Log("점수 초기화");
    }
    
    /// <summary>
    /// 기본 스코어 아이템 생성 (prefab이 없을 때)
    /// </summary>
    GameObject CreateDefaultScoreItem(ScoreItemData data, Vector2 position)
    {
        // 기본 원형 오브젝트 생성
        GameObject item = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        item.name = $"ScoreItem_{data.type}";
        item.transform.position = position;
        item.transform.parent = itemContainer;
        
        // 크기 조정 (아이템 타입에 따라)
        float scale = data.type switch
        {
            ScoreItemType.Normal => 0.8f,
            ScoreItemType.Silver => 1.0f,
            ScoreItemType.Gold => 1.2f,
            ScoreItemType.Bonus => 1.5f,
            _ => 1.0f
        };
        item.transform.localScale = Vector3.one * scale;
        
        // Rigidbody 제거 (정적 아이템)
        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            DestroyImmediate(rb);
        }
        
        // SphereCollider를 CircleCollider2D로 교체
        if (item.TryGetComponent<SphereCollider>(out var sphereCol))
        {
            DestroyImmediate(sphereCol);
        }
        CircleCollider2D circleCol = item.AddComponent<CircleCollider2D>();
        circleCol.isTrigger = true;
        
        // 색상 설정
        Renderer renderer = item.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = data.itemColor;
        }
        
        Debug.Log($"기본 스코어 아이템 생성 - Type: {data.type}, Scale: {scale}");
        return item;
    }
    
    /// <summary>
    /// 특정 위치에 아이템 스폰
    /// </summary>
    public GameObject SpawnItemAt(Vector2 position, ScoreItemType type)
    {
        ScoreItemData data = itemTypes.Find(d => d.type == type);
        if (data == null)
        {
            Debug.LogWarning($"타입 {type}의 아이템 데이터를 찾을 수 없습니다.");
            return null;
        }
        
        GameObject item;
        if (data.prefab != null)
        {
            item = Instantiate(data.prefab, position, Quaternion.identity, itemContainer);
        }
        else
        {
            item = CreateDefaultScoreItem(data, position);
        }
        
        ScoreItem scoreItem = item.GetComponent<ScoreItem>();
        if (scoreItem == null)
        {
            scoreItem = item.AddComponent<ScoreItem>();
        }
        
        // 전역 거리 설정 적용
        if (useGlobalDistanceSettings)
        {
            scoreItem.maxTriggerDistanceMultiplier = globalTriggerDistanceMultiplier;
            scoreItem.fixedMarginDistance = globalFixedMarginDistance;
            Debug.Log($"🔧 ScoreItem 거리 설정 적용 - 배수: {globalTriggerDistanceMultiplier}, 여유거리: {globalFixedMarginDistance}");
        }
        
        scoreItem.Initialize(data.type, data.score, data.itemColor, this);
        
        activeItems.Add(item);
        return item;
    }
    
#if UNITY_EDITOR
    /// <summary>
    /// 디버그용 기즈모
    /// </summary>
    void OnDrawGizmos()
    {
        if (clockCenter == null) return;
        
        Vector3 center = clockCenter.position;
        
        // 스폰 가능 영역 (초록색)
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        DrawCircle(center, spawnRadius, 32);
        
        // 스폰 최소 거리 (빨간색)
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        DrawCircle(center, spawnMinRadius, 32);
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
#endif
    
    // 공개 속성들
    public int CurrentScore => currentScore;
    public int TargetScore => targetScore;
    public int ActiveItemCount => activeItems.Count;
}
