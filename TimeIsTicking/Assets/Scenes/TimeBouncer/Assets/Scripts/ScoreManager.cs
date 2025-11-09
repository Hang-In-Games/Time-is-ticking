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
        if (selectedData == null || selectedData.prefab == null)
        {
            Debug.LogWarning("스폰할 아이템을 선택할 수 없습니다.");
            return null;
        }
        
        // 랜덤 위치 생성
        Vector2 spawnPosition = GetRandomSpawnPosition();
        
        // 아이템 생성
        GameObject item = Instantiate(selectedData.prefab, spawnPosition, Quaternion.identity, itemContainer);
        
        // ScoreItem 컴포넌트 설정
        ScoreItem scoreItem = item.GetComponent<ScoreItem>();
        if (scoreItem == null)
        {
            scoreItem = item.AddComponent<ScoreItem>();
        }
        scoreItem.Initialize(selectedData.type, selectedData.score, selectedData.itemColor, this);
        
        activeItems.Add(item);
        Debug.Log($"아이템 스폰 - Type: {selectedData.type}, Position: {spawnPosition}");
        
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
    /// 랜덤 스폰 위치 생성 (시계 내부)
    /// </summary>
    Vector2 GetRandomSpawnPosition()
    {
        Vector2 center = clockCenter != null ? (Vector2)clockCenter.position : Vector2.zero;
        
        // 랜덤 각도와 거리
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = Random.Range(spawnMinRadius, spawnRadius);
        
        // 원형 내부의 랜덤 위치
        Vector2 offset = new Vector2(
            Mathf.Cos(angle) * distance,
            Mathf.Sin(angle) * distance
        );
        
        return center + offset;
    }
    
    /// <summary>
    /// 아이템 획득 처리
    /// </summary>
    public void CollectItem(ScoreItem item)
    {
        if (item == null) return;
        
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
        
        Debug.Log($"점수 획득! +{item.scoreValue} (총: {currentScore})");
        
        // 목표 달성 확인
        if (targetScore > 0 && currentScore >= targetScore && previousScore < targetScore)
        {
            OnTargetReached?.Invoke(currentScore);
            Debug.Log($"목표 점수 달성! {currentScore}/{targetScore}");
        }
        
        // 활성 아이템 리스트에서 제거
        if (activeItems.Contains(item.gameObject))
        {
            activeItems.Remove(item.gameObject);
        }
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
    /// 특정 위치에 아이템 스폰
    /// </summary>
    public GameObject SpawnItemAt(Vector2 position, ScoreItemType type)
    {
        ScoreItemData data = itemTypes.Find(d => d.type == type);
        if (data == null || data.prefab == null)
        {
            Debug.LogWarning($"타입 {type}의 아이템 데이터를 찾을 수 없습니다.");
            return null;
        }
        
        GameObject item = Instantiate(data.prefab, position, Quaternion.identity, itemContainer);
        
        ScoreItem scoreItem = item.GetComponent<ScoreItem>();
        if (scoreItem == null)
        {
            scoreItem = item.AddComponent<ScoreItem>();
        }
        scoreItem.Initialize(data.type, data.score, data.itemColor, this);
        
        activeItems.Add(item);
        return item;
    }
    
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
}
