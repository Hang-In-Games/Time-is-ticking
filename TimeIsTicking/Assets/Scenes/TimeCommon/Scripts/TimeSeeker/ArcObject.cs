using UnityEngine;

/// <summary>
/// 개별 Arc 오브젝트 (점 또는 선)
/// 시계 테두리에 배치되며 난이도에 따라 움직임
/// </summary>
public class ArcObject : MonoBehaviour
{
    [Header("Arc Settings")]
    [Tooltip("오브젝트 타입")]
    public ArcObjectType objectType = ArcObjectType.Point;
    
    [Tooltip("시작 각도")]
    public float startAngle = 0f;
    
    [Tooltip("끝 각도 (Line 타입만)")]
    public float endAngle = 15f;
    
    [Tooltip("시계 중심으로부터의 반지름")]
    public float radius = 180f;
    
    [Header("Movement Settings")]
    [Tooltip("움직임 패턴")]
    public MovementPattern movementPattern;
    
    [Header("Visual Settings")]
    [Tooltip("오브젝트 색상")]
    public Color objectColor = Color.white;
    
    [Tooltip("활성화 시 색상")]
    public Color activeColor = Color.green;
    
    [Tooltip("크기")]
    public float objectSize = 10f;
    
    // 내부 상태
    private Transform clockCenter;
    private SpriteRenderer spriteRenderer;
    private LineRenderer lineRenderer;
    private float initialAngle;
    private float currentOffset = 0f;
    private bool isActive = false;
    private float activeTime = 0f;
    
    void Start()
    {
        initialAngle = startAngle;
        SetupVisuals();
    }
    
    void Update()
    {
        UpdateMovement();
        UpdatePosition();
    }
    
    /// <summary>
    /// 초기화
    /// </summary>
    public void Initialize(ArcObjectData data, Transform center)
    {
        objectType = data.type;
        startAngle = data.startAngle;
        endAngle = data.endAngle;
        radius = data.radius;
        objectColor = data.color;
        movementPattern = data.movementPattern;
        clockCenter = center;
        
        initialAngle = startAngle;
        
        SetupVisuals();
        UpdatePosition();
    }
    
    /// <summary>
    /// 시각적 요소 설정
    /// </summary>
    void SetupVisuals()
    {
        if (objectType == ArcObjectType.Point)
        {
            // 점 표현 - SpriteRenderer 사용
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
            
            // LineRenderer가 있다면 제거 (충돌 방지)
            LineRenderer existingLine = GetComponent<LineRenderer>();
            if (existingLine != null)
            {
                DestroyImmediate(existingLine);
            }
            
            // 원형 스프라이트 생성
            CreateCircleSprite();
        }
        else if (objectType == ArcObjectType.Line)
        {
            // 선 표현 - LineRenderer 사용
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }
            
            // SpriteRenderer가 있다면 제거 (충돌 방지)
            SpriteRenderer existingSprite = GetComponent<SpriteRenderer>();
            if (existingSprite != null)
            {
                DestroyImmediate(existingSprite);
            }
            
            SetupLineRenderer();
        }
        
        Debug.Log($"ArcObject 시각화 설정 완료 - Type: {objectType}, Color: {objectColor}");
    }
    
    /// <summary>
    /// 원형 스프라이트 생성
    /// </summary>
    void CreateCircleSprite()
    {
        if (spriteRenderer == null) return;
        
        // 런타임에서 원형 스프라이트 생성
        int resolution = 64; // 해상도 증가
        Texture2D texture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[resolution * resolution];
        
        Vector2 center = new Vector2(resolution / 2f, resolution / 2f);
        float outerRadius = resolution / 2f - 2;
        float innerRadius = outerRadius - 4; // 테두리 효과
        
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                
                if (distance <= innerRadius)
                {
                    // 내부 - 완전한 색상
                    pixels[y * resolution + x] = Color.white;
                }
                else if (distance <= outerRadius)
                {
                    // 테두리 - 알파 그라데이션
                    float alpha = 1f - (distance - innerRadius) / (outerRadius - innerRadius);
                    pixels[y * resolution + x] = new Color(1f, 1f, 1f, alpha);
                }
                else
                {
                    // 외부 - 투명
                    pixels[y * resolution + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        // 픽셀당 유닛 크기 조정
        float pixelsPerUnit = resolution / objectSize;
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        spriteRenderer.sprite = sprite;
        
        // 추가 렌더링 설정
        spriteRenderer.sortingOrder = 2;
        spriteRenderer.color = objectColor;
        
        Debug.Log($"Point 스프라이트 생성 완료 - 크기: {objectSize}, 색상: {objectColor}");
    }
    
    /// <summary>
    /// LineRenderer 설정
    /// </summary>
    void SetupLineRenderer()
    {
        if (lineRenderer == null) return;
        
        lineRenderer.positionCount = 20;
        lineRenderer.startWidth = 8f;
        lineRenderer.endWidth = 8f;
        lineRenderer.useWorldSpace = true;
        
        // Material 설정 - 더 확실한 방법으로
        Material lineMaterial = null;
        
        // Unity 기본 Unlit Shader 사용
        Shader unlitShader = Shader.Find("Unlit/Color");
        if (unlitShader == null)
        {
            unlitShader = Shader.Find("Legacy Shaders/Unlit/Color");
        }
        if (unlitShader == null)
        {
            unlitShader = Shader.Find("Sprites/Default");
        }
        if (unlitShader == null)
        {
            unlitShader = Shader.Find("Standard");
        }
        
        if (unlitShader != null)
        {
            lineMaterial = new Material(unlitShader);
            lineMaterial.color = objectColor;
        }
        else
        {
            Debug.LogWarning("적절한 Shader를 찾을 수 없습니다. 기본 Material을 사용합니다.");
            lineMaterial = new Material(Shader.Find("Standard"));
        }
        
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = objectColor;
        lineRenderer.endColor = objectColor;
        
        // Z값 설정으로 카메라에 보이게 하기
        lineRenderer.sortingOrder = 1;
        
        UpdateLinePositions();
        
        Debug.Log($"LineRenderer 설정 완료 - Shader: {lineRenderer.material.shader.name}, Color: {objectColor}");
    }
    
    /// <summary>
    /// LineRenderer 위치 업데이트
    /// </summary>
    void UpdateLinePositions()
    {
        if (lineRenderer == null || clockCenter == null) return;
        
        Vector3 center = clockCenter.position;
        int segments = lineRenderer.positionCount;
        float angleStep = (endAngle - startAngle) / (segments - 1);
        
        for (int i = 0; i < segments; i++)
        {
            float angle = (startAngle + angleStep * i + currentOffset) * Mathf.Deg2Rad;
            Vector3 position = center + new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                -1f  // Z값을 약간 앞으로 이동시켜 카메라에 보이게 함
            );
            lineRenderer.SetPosition(i, position);
        }
        
        // 첫 번째 업데이트시 디버그 로그
        if (Time.time < 1f)
        {
            Debug.Log($"Line 위치 업데이트: {startAngle:F1}° ~ {endAngle:F1}°, Center: {center}, Radius: {radius}");
        }
    }
    
    /// <summary>
    /// 움직임 업데이트
    /// </summary>
    void UpdateMovement()
    {
        if (movementPattern == null) return;
        
        switch (movementPattern.type)
        {
            case MovementType.Static:
                // 고정 - 움직임 없음
                break;
                
            case MovementType.Rotating:
                // 일정한 회전
                currentOffset += movementPattern.rotationSpeed * Time.deltaTime;
                break;
                
            case MovementType.Oscillating:
                // 진동
                float oscillation = Mathf.Sin(Time.time * movementPattern.oscillationFrequency * Mathf.PI * 2f) 
                    * movementPattern.oscillationAmplitude;
                currentOffset = oscillation;
                break;
                
            case MovementType.Erratic:
                // 불규칙 (회전 + 진동 + 랜덤)
                currentOffset += movementPattern.rotationSpeed * Time.deltaTime;
                float erraticOscillation = Mathf.Sin(Time.time * movementPattern.oscillationFrequency * Mathf.PI * 2f) 
                    * movementPattern.oscillationAmplitude;
                float randomOffset = Random.Range(-movementPattern.randomness, movementPattern.randomness) * 10f;
                currentOffset += erraticOscillation * Time.deltaTime + randomOffset * Time.deltaTime;
                break;
        }
    }
    
    /// <summary>
    /// 위치 업데이트
    /// </summary>
    void UpdatePosition()
    {
        if (clockCenter == null) return;
        
        if (objectType == ArcObjectType.Point)
        {
            // 점의 위치 계산
            float currentAngle = (initialAngle + currentOffset) * Mathf.Deg2Rad;
            Vector3 position = clockCenter.position + new Vector3(
                Mathf.Cos(currentAngle) * radius,
                Mathf.Sin(currentAngle) * radius,
                0
            );
            transform.position = position;
        }
        else if (objectType == ArcObjectType.Line)
        {
            // 선의 위치들 업데이트
            UpdateLinePositions();
        }
    }
    
    /// <summary>
    /// 바늘이 이 오브젝트와 겹치는지 확인
    /// </summary>
    public bool IsOverlappingWithNeedle(float needleAngle, float tolerance)
    {
        float normalizedNeedleAngle = NormalizeAngle(needleAngle);
        
        if (objectType == ArcObjectType.Point)
        {
            float currentAngle = NormalizeAngle(initialAngle + currentOffset);
            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(normalizedNeedleAngle, currentAngle));
            return angleDifference <= tolerance;
        }
        else if (objectType == ArcObjectType.Line)
        {
            float currentStart = NormalizeAngle(startAngle + currentOffset);
            float currentEnd = NormalizeAngle(endAngle + currentOffset);
            
            // Arc가 0도를 넘어가는 경우 처리
            if (currentStart > currentEnd)
            {
                return (normalizedNeedleAngle >= currentStart || normalizedNeedleAngle <= currentEnd + tolerance) ||
                       (normalizedNeedleAngle <= currentEnd || normalizedNeedleAngle >= currentStart - tolerance);
            }
            else
            {
                return normalizedNeedleAngle >= currentStart - tolerance && 
                       normalizedNeedleAngle <= currentEnd + tolerance;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 각도를 0-360 범위로 정규화
    /// </summary>
    float NormalizeAngle(float angle)
    {
        while (angle < 0) angle += 360f;
        while (angle >= 360) angle -= 360f;
        return angle;
    }
    
    /// <summary>
    /// 활성화 상태 설정 (바늘이 겹쳤을 때)
    /// </summary>
    public void SetActive(bool active)
    {
        isActive = active;
        
        if (active)
        {
            activeTime += Time.deltaTime;
            
            // 색상 변경
            if (spriteRenderer != null)
            {
                spriteRenderer.color = activeColor;
            }
            if (lineRenderer != null)
            {
                lineRenderer.startColor = activeColor;
                lineRenderer.endColor = activeColor;
            }
        }
        else
        {
            activeTime = 0f;
            
            // 원래 색상으로 복원
            if (spriteRenderer != null)
            {
                spriteRenderer.color = objectColor;
            }
            if (lineRenderer != null)
            {
                lineRenderer.startColor = objectColor;
                lineRenderer.endColor = objectColor;
            }
        }
    }
    
    /// <summary>
    /// 활성화 유지 시간 가져오기
    /// </summary>
    public float GetActiveTime()
    {
        return activeTime;
    }
    
    /// <summary>
    /// 현재 각도 가져오기 (중심각)
    /// </summary>
    public float GetCurrentAngle()
    {
        if (objectType == ArcObjectType.Point)
        {
            return NormalizeAngle(initialAngle + currentOffset);
        }
        else
        {
            return NormalizeAngle((startAngle + endAngle) / 2f + currentOffset);
        }
    }
}
