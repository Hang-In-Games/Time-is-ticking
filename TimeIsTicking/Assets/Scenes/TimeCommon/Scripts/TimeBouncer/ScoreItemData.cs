using UnityEngine;

/// <summary>
/// 스코어 아이템 타입 정의
/// </summary>
public enum ScoreItemType
{
    Normal = 0,      // 일반 아이템 (1점)
    Silver = 1,      // 은색 아이템 (3점)
    Gold = 2,        // 금색 아이템 (5점)
    Bonus = 3        // 보너스 아이템 (10점)
}

/// <summary>
/// 스코어 아이템 데이터 모델
/// </summary>
[System.Serializable]
public class ScoreItemData
{
    [Tooltip("아이템 타입")]
    public ScoreItemType type;
    
    [Tooltip("획득 점수")]
    public int score;
    
    [Tooltip("프리팹")]
    public GameObject prefab;
    
    [Tooltip("스폰 확률 (0~1)")]
    [Range(0f, 1f)]
    public float spawnProbability = 1f;
    
    [Tooltip("아이템 색상 (시각적 구분용)")]
    public Color itemColor = Color.white;
    
    public ScoreItemData(ScoreItemType type, int score, float probability, Color color)
    {
        this.type = type;
        this.score = score;
        this.spawnProbability = probability;
        this.itemColor = color;
    }
}

/// <summary>
/// 스코어 이벤트 데이터
/// </summary>
public class ScoreEventArgs
{
    public ScoreItemType itemType;
    public int scoreGained;
    public int totalScore;
    public Vector2 position;
    
    public ScoreEventArgs(ScoreItemType type, int gained, int total, Vector2 pos)
    {
        itemType = type;
        scoreGained = gained;
        totalScore = total;
        position = pos;
    }
}
