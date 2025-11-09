using UnityEngine;

/// <summary>
/// Arc 오브젝트 타입 정의
/// </summary>
public enum ArcObjectType
{
    Point = 0,      // 점 (단일 위치)
    Line = 1        // 선 (Arc 형태)
}

/// <summary>
/// 난이도 레벨 정의
/// </summary>
public enum DifficultyLevel
{
    Easy = 0,       // 고정 위치
    Normal = 1,     // 일정한 회전
    Hard = 2        // 불규칙 움직임
}

/// <summary>
/// Arc 오브젝트 데이터 모델
/// </summary>
[System.Serializable]
public class ArcObjectData
{
    [Tooltip("오브젝트 타입")]
    public ArcObjectType type;
    
    [Tooltip("시작 각도 (도 단위)")]
    public float startAngle;
    
    [Tooltip("끝 각도 (도 단위, Line 타입만 사용)")]
    public float endAngle;
    
    [Tooltip("시계 반지름")]
    public float radius;
    
    [Tooltip("오브젝트 색상")]
    public Color color = Color.white;
    
    [Tooltip("움직임 패턴")]
    public MovementPattern movementPattern;
    
    public ArcObjectData(ArcObjectType type, float startAngle, float radius, Color color)
    {
        this.type = type;
        this.startAngle = startAngle;
        this.endAngle = startAngle + 15f; // 기본 15도 Arc
        this.radius = radius;
        this.color = color;
        this.movementPattern = new MovementPattern();
    }
}

/// <summary>
/// 오브젝트 움직임 패턴
/// </summary>
[System.Serializable]
public class MovementPattern
{
    [Tooltip("움직임 타입")]
    public MovementType type = MovementType.Static;
    
    [Tooltip("회전 속도 (도/초)")]
    public float rotationSpeed = 0f;
    
    [Tooltip("진동 진폭 (도)")]
    public float oscillationAmplitude = 0f;
    
    [Tooltip("진동 주기 (초)")]
    public float oscillationFrequency = 0f;
    
    [Tooltip("불규칙 움직임 강도")]
    [Range(0f, 1f)]
    public float randomness = 0f;
}

/// <summary>
/// 움직임 타입
/// </summary>
public enum MovementType
{
    Static = 0,         // 고정
    Rotating = 1,       // 일정한 회전
    Oscillating = 2,    // 진동
    Erratic = 3         // 불규칙
}

/// <summary>
/// 점수 이벤트 데이터
/// </summary>
public class ScoreEventArgs_TimeSeeker
{
    public ArcObjectType objectType;
    public int scoreGained;
    public int totalScore;
    public int comboCount;
    public Vector2 position;
    
    public ScoreEventArgs_TimeSeeker(ArcObjectType type, int gained, int total, int combo, Vector2 pos)
    {
        objectType = type;
        scoreGained = gained;
        totalScore = total;
        comboCount = combo;
        position = pos;
    }
}

/// <summary>
/// 난이도 설정
/// </summary>
[System.Serializable]
public class DifficultySettings
{
    [Tooltip("난이도 레벨")]
    public DifficultyLevel level = DifficultyLevel.Easy;
    
    [Tooltip("오브젝트 생성 주기 (초)")]
    public float spawnInterval = 5f;
    
    [Tooltip("바늘 허용 오차 (도)")]
    public float angleTolerance = 5f;
    
    [Tooltip("성공 판정 유지 시간 (초)")]
    public float dwellTime = 1f;
    
    [Tooltip("최대 동시 오브젝트 수")]
    public int maxActiveObjects = 3;
    
    [Tooltip("움직임 패턴")]
    public MovementPattern defaultMovementPattern;
    
    public DifficultySettings()
    {
        defaultMovementPattern = new MovementPattern();
    }
    
    /// <summary>
    /// Easy 난이도 설정 생성
    /// </summary>
    public static DifficultySettings CreateEasy()
    {
        var settings = new DifficultySettings
        {
            level = DifficultyLevel.Easy,
            spawnInterval = 5f,
            angleTolerance = 10f,
            dwellTime = 1f,
            maxActiveObjects = 2
        };
        settings.defaultMovementPattern.type = MovementType.Static;
        return settings;
    }
    
    /// <summary>
    /// Normal 난이도 설정 생성
    /// </summary>
    public static DifficultySettings CreateNormal()
    {
        var settings = new DifficultySettings
        {
            level = DifficultyLevel.Normal,
            spawnInterval = 3f,
            angleTolerance = 7f,
            dwellTime = 1f,
            maxActiveObjects = 3
        };
        settings.defaultMovementPattern.type = MovementType.Rotating;
        settings.defaultMovementPattern.rotationSpeed = 30f;
        return settings;
    }
    
    /// <summary>
    /// Hard 난이도 설정 생성
    /// </summary>
    public static DifficultySettings CreateHard()
    {
        var settings = new DifficultySettings
        {
            level = DifficultyLevel.Hard,
            spawnInterval = 2f,
            angleTolerance = 5f,
            dwellTime = 0.8f,
            maxActiveObjects = 4
        };
        settings.defaultMovementPattern.type = MovementType.Erratic;
        settings.defaultMovementPattern.rotationSpeed = 45f;
        settings.defaultMovementPattern.oscillationAmplitude = 15f;
        settings.defaultMovementPattern.oscillationFrequency = 2f;
        settings.defaultMovementPattern.randomness = 0.5f;
        return settings;
    }
}
