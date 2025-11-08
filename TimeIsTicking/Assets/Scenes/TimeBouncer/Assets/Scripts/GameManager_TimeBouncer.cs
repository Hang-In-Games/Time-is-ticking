using UnityEngine;

/// <summary>
/// TimeBouncer 게임의 메인 매니저
/// 시계 테두리 내에서 공과 시침이 움직이는 퐁 게임을 관리합니다.
/// </summary>
public class GameManager_TimeBouncer : MonoBehaviour
{
    [Header("Clock Settings")]
    [Tooltip("시계의 중심점 (모든 오브젝트가 이 중심을 기준으로 제약됨)")]
    public Transform clockCenter;
    
    [Tooltip("시계 테두리 반지름 (SVG 기준 180)")]
    public float clockRadius = 180f;
    
    [Header("Boundary Object Tags")]
    [Tooltip("원형 경계 제약을 받을 오브젝트의 태그들")]
    public string[] boundaryTags = new string[] { "Ball", "Paddle" };
    
    [Header("Boundary Settings")]
    [Tooltip("경계에서 튕기기 활성화")]
    public bool enableBounce = true;
    
    [Tooltip("반사 계수 (0~1)")]
    [Range(0f, 1f)]
    public float bounciness = 0.8f;

    void Start()
    {
        // 시계 중심점이 설정되지 않았다면 현재 오브젝트를 중심으로 사용
        if (clockCenter == null)
        {
            clockCenter = transform;
            Debug.LogWarning("Clock Center가 설정되지 않아 GameManager를 중심으로 사용합니다.");
        }
        
        SetupBoundaryConstraints();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 지정된 태그를 가진 모든 오브젝트에 CircleBoundary 컴포넌트를 추가합니다.
    /// </summary>
    void SetupBoundaryConstraints()
    {
        int constrainedObjectCount = 0;
        
        foreach (string tag in boundaryTags)
        {
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
            
            if (taggedObjects.Length == 0)
            {
                Debug.LogWarning($"태그 '{tag}'를 가진 오브젝트가 없습니다.");
                continue;
            }
            
            foreach (GameObject obj in taggedObjects)
            {
                // 이미 CircleBoundary 컴포넌트가 있는지 확인
                CircleBoundary boundary = obj.GetComponent<CircleBoundary>();
                
                if (boundary == null)
                {
                    // 새로 추가
                    boundary = obj.AddComponent<CircleBoundary>();
                    Debug.Log($"'{obj.name}'에 CircleBoundary 컴포넌트를 추가했습니다.");
                }
                
                // 설정 적용
                boundary.centerPoint = clockCenter;
                boundary.radius = clockRadius;
                boundary.bounceOnBoundary = enableBounce;
                boundary.bounciness = bounciness;
                
                // 오브젝트 크기에 따라 반지름 자동 설정
                SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    boundary.objectRadius = Mathf.Max(
                        spriteRenderer.bounds.extents.x,
                        spriteRenderer.bounds.extents.y
                    );
                }
                else
                {
                    // 기본값 사용
                    boundary.objectRadius = 10f;
                }
                
                constrainedObjectCount++;
            }
        }
        
        Debug.Log($"총 {constrainedObjectCount}개의 오브젝트에 원형 경계 제약을 적용했습니다.");
    }

    /// <summary>
    /// 런타임에 새로운 오브젝트에 경계 제약을 추가할 때 사용
    /// </summary>
    public void AddBoundaryConstraint(GameObject obj, float objectRadius = 10f)
    {
        CircleBoundary boundary = obj.GetComponent<CircleBoundary>();
        
        if (boundary == null)
        {
            boundary = obj.AddComponent<CircleBoundary>();
        }
        
        boundary.centerPoint = clockCenter;
        boundary.radius = clockRadius;
        boundary.objectRadius = objectRadius;
        boundary.bounceOnBoundary = enableBounce;
        boundary.bounciness = bounciness;
    }
}
