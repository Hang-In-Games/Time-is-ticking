using UnityEngine;

/// <summary>
/// ClockBorder에 원형 EdgeCollider2D를 자동으로 생성하는 유틸리티
/// GameManager가 Start에서 호출하거나, ClockBorder에 직접 추가 가능
/// </summary>
public class ClockBorderColliderSetup : MonoBehaviour
{
    [Header("Collider Settings")]
    [Tooltip("EdgeCollider2D의 점 개수 (많을수록 부드러운 원)")]
    public int segments = 64;
    
    [Tooltip("Collider 반지름 (0이면 Sprite 크기 사용)")]
    public float radius = 0f;
    
    [Tooltip("Physics Material 2D (Ball과 충돌 시 반발력)")]
    public PhysicsMaterial2D physicsMaterial;
    
    void Start()
    {
        SetupEdgeCollider();
    }
    
    public void SetupEdgeCollider()
    {
        // 기존 EdgeCollider2D 확인
        EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
        if (edgeCollider == null)
        {
            edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
            Debug.Log("ClockBorder에 EdgeCollider2D 추가");
        }
        
        // 반지름 결정
        float colliderRadius = radius;
        if (colliderRadius <= 0f)
        {
            // Sprite 크기에서 자동 계산
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                colliderRadius = spriteRenderer.sprite.bounds.extents.x * transform.localScale.x;
            }
            else
            {
                colliderRadius = 180f; // 기본값
                Debug.LogWarning("Sprite를 찾을 수 없어 기본 반지름 180 사용");
            }
        }
        
        // 원형 EdgeCollider2D 점 생성
        Vector2[] points = new Vector2[segments + 1];
        float angleStep = 360f / segments;
        
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            points[i] = new Vector2(
                Mathf.Cos(angle) * colliderRadius,
                Mathf.Sin(angle) * colliderRadius
            );
        }
        
        edgeCollider.points = points;
        
        // Physics Material 적용
        if (physicsMaterial != null)
        {
            edgeCollider.sharedMaterial = physicsMaterial;
        }
        else
        {
            Debug.LogWarning("ClockBorder Physics Material이 할당되지 않았습니다. Bounciness가 0일 수 있습니다.");
        }
        
        Debug.Log($"ClockBorder EdgeCollider2D 설정 완료 - Radius: {colliderRadius}, Segments: {segments}");
    }
}