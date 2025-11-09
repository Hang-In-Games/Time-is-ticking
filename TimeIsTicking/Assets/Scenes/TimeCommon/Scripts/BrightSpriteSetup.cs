using UnityEngine;

/// <summary>
/// SVG Sprite가 어둡게 나오는 문제를 해결하기 위한 Material 설정
/// </summary>
public class BrightSpriteSetup : MonoBehaviour
{
    [Header("Auto Setup on Start")]
    [Tooltip("시작 시 자동으로 밝은 Material 적용")]
    public bool autoSetupOnStart = true;
    
    [Header("Manual Settings")]
    [Tooltip("사용할 Shader (비워두면 자동 선택)")]
    public Shader customShader;
    
    [Tooltip("밝기 배율 (1보다 크면 더 밝아짐)")]
    [Range(1f, 3f)]
    public float brightnessFactor = 1f;
    
    private SpriteRenderer spriteRenderer;
    private Material brightMaterial;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            Debug.LogError($"{gameObject.name}: SpriteRenderer가 없습니다!");
            return;
        }
        
        if (autoSetupOnStart)
        {
            SetupBrightMaterial();
        }
    }

    /// <summary>
    /// 밝은 Material을 생성하고 적용합니다
    /// </summary>
    public void SetupBrightMaterial()
    {
        if (spriteRenderer == null) return;
        
        // 새로운 Material 생성
        brightMaterial = new Material(GetAppropriateShader());
        
        // Material 설정
        brightMaterial.color = Color.white;
        
        // Emission 설정 (더 밝게)
        if (brightnessFactor > 1f)
        {
            brightMaterial.EnableKeyword("_EMISSION");
            brightMaterial.SetColor("_EmissionColor", Color.white * (brightnessFactor - 1f));
        }
        
        // SpriteRenderer에 적용
        spriteRenderer.material = brightMaterial;
        spriteRenderer.color = Color.white;
        
        Debug.Log($"{gameObject.name}: 밝은 Material 적용 완료!");
    }

    /// <summary>
    /// 적절한 Shader를 선택합니다
    /// </summary>
    Shader GetAppropriateShader()
    {
        if (customShader != null)
        {
            return customShader;
        }
        
        // Unlit Shader 사용 (광원 영향 받지 않음)
        Shader unlitShader = Shader.Find("Sprites/Default");
        if (unlitShader != null)
        {
            return unlitShader;
        }
        
        // 대체: Universal Render Pipeline
        unlitShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (unlitShader != null)
        {
            return unlitShader;
        }
        
        // 기본 Shader
        return Shader.Find("Sprites/Default");
    }

    /// <summary>
    /// 밝기를 즉시 변경 (런타임)
    /// </summary>
    public void SetBrightness(float brightness)
    {
        brightnessFactor = Mathf.Clamp(brightness, 1f, 3f);
        
        if (brightMaterial != null)
        {
            if (brightnessFactor > 1f)
            {
                brightMaterial.EnableKeyword("_EMISSION");
                brightMaterial.SetColor("_EmissionColor", Color.white * (brightnessFactor - 1f));
            }
            else
            {
                brightMaterial.DisableKeyword("_EMISSION");
            }
        }
    }
}