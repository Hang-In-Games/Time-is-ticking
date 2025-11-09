using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 게임 제한시간 관리 및 결과 표시 시스템
/// GameManager와 연동하여 시간 종료 시 게임 상태 변경
/// </summary>
public class TimerManager : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("게임 제한시간 (초) - 실시간 변경 가능")]
    [Range(10f, 300f)]  // 10초~5분 범위
    public float gameDuration = 60f;
    
    [Tooltip("타이머 활성화 여부")]
    public bool useTimer = true;
    
    [Header("UI References")]
    [Tooltip("타이머 텍스트 (TMP_Text 또는 Text)")]
    public TMP_Text timerText;
    
    [Tooltip("타이머 진행바 (Image의 fillAmount 사용)")]
    public Image timerProgressBar;
    
    [Tooltip("게임 결과 패널")]
    public GameObject resultPanel;
    
    [Tooltip("성공 메시지 오브젝트")]
    public GameObject successMessage;
    
    [Tooltip("실패 메시지 오브젝트")]
    public GameObject failMessage;
    
    [Header("Timer Visual Settings")]
    [Tooltip("타이머 색상 - 정상")]
    public Color normalTimerColor = Color.white;
    
    [Tooltip("타이머 색상 - 경고 (30초 이하)")]
    public Color warningTimerColor = Color.yellow;
    
    [Tooltip("타이머 색상 - 위험 (10초 이하)")]
    public Color dangerTimerColor = Color.red;
    
    // 타이머 상태
    private float currentTime;
    private bool isTimerRunning = false;
    private bool isGameEnded = false;
    private float previousGameDuration;  // gameDuration 변경 감지용
    
    // 이벤트
    public static event Action OnTimeUp;
    public static event Action<float> OnTimeChanged;
    
    // 참조
    private TimeBouncer_GameManager gameManager;
    
    void Start()
    {
        InitializeTimer();
    }
    
    void Update()
    {
        // gameDuration이 Inspector에서 변경되었는지 확인
        if (Mathf.Abs(gameDuration - previousGameDuration) > 0.01f)
        {
            OnGameDurationChanged();
            previousGameDuration = gameDuration;
        }
        
        if (useTimer && isTimerRunning && !isGameEnded)
        {
            UpdateTimer();
        }
    }
    
    /// <summary>
    /// 타이머 초기화
    /// </summary>
    public void InitializeTimer()
    {
        currentTime = gameDuration;
        previousGameDuration = gameDuration;  // 초기값 설정
        isTimerRunning = false;
        isGameEnded = false;
        
        // GameManager 참조 찾기
        gameManager = FindFirstObjectByType<TimeBouncer_GameManager>();
        
        // UI 초기화
        UpdateTimerDisplay();
        HideResult();
        
        Debug.Log($"TimerManager 초기화 - 제한시간: {gameDuration}초");
    }
    
    /// <summary>
    /// gameDuration이 변경되었을 때 처리
    /// </summary>
    private void OnGameDurationChanged()
    {
        // 게임이 시작되지 않았거나 종료된 상태에서만 적용
        if (!isTimerRunning || isGameEnded)
        {
            currentTime = gameDuration;
            UpdateTimerDisplay();
            Debug.Log($"제한시간 변경됨: {gameDuration}초");
        }
        else
        {
            // 게임 진행 중에는 진행률 유지하면서 총 시간만 변경
            float progressRatio = 1f - (currentTime / previousGameDuration);
            currentTime = gameDuration * (1f - progressRatio);
            UpdateTimerDisplay();
            Debug.Log($"게임 중 제한시간 변경: {gameDuration}초 (진행률 {progressRatio:P0} 유지)");
        }
    }
    
    /// <summary>
    /// 타이머 시작
    /// </summary>
    public void StartTimer()
    {
        if (!useTimer) return;
        
        isTimerRunning = true;
        isGameEnded = false;
        Debug.Log("타이머 시작!");
    }
    
    /// <summary>
    /// 타이머 정지
    /// </summary>
    public void StopTimer()
    {
        isTimerRunning = false;
        Debug.Log("타이머 정지");
    }
    
    /// <summary>
    /// 타이머 일시정지
    /// </summary>
    public void PauseTimer()
    {
        isTimerRunning = false;
    }
    
    /// <summary>
    /// 타이머 재개
    /// </summary>
    public void ResumeTimer()
    {
        if (!isGameEnded)
        {
            isTimerRunning = true;
        }
    }
    
    /// <summary>
    /// 타이머 업데이트
    /// </summary>
    private void UpdateTimer()
    {
        currentTime -= Time.deltaTime;
        
        // 시간 종료 체크
        if (currentTime <= 0f)
        {
            currentTime = 0f;
            OnTimeUp?.Invoke();
            CheckGameResult();
        }
        
        // UI 업데이트
        UpdateTimerDisplay();
        
        // 시간 변경 이벤트
        OnTimeChanged?.Invoke(currentTime);
    }
    
    /// <summary>
    /// 타이머 UI 업데이트
    /// </summary>
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
            
            // 색상 변경
            timerText.color = GetTimerColor();
        }
        
        if (timerProgressBar != null)
        {
            float progress = currentTime / gameDuration;
            timerProgressBar.fillAmount = progress;
            timerProgressBar.color = GetTimerColor();
        }
    }
    
    /// <summary>
    /// 시간에 따른 타이머 색상 반환
    /// </summary>
    private Color GetTimerColor()
    {
        if (currentTime <= 10f)
            return dangerTimerColor;
        else if (currentTime <= 30f)
            return warningTimerColor;
        else
            return normalTimerColor;
    }
    
    /// <summary>
    /// 게임 결과 확인 및 표시
    /// </summary>
    private void CheckGameResult()
    {
        if (isGameEnded) return;
        
        isGameEnded = true;
        isTimerRunning = false;
        
        // GameManager에서 목표 점수 달성 여부 확인
        bool isSuccess = false;
        if (gameManager != null && gameManager.scoreManager != null)
        {
            int currentScore = gameManager.scoreManager.CurrentScore;
            int targetScore = gameManager.scoreManager.TargetScore;
            
            // 목표 점수가 0이면 무제한 모드 (시간 종료 시 성공)
            // 목표 점수가 설정되어 있으면 달성 여부 확인
            isSuccess = (targetScore <= 0) || (currentScore >= targetScore);
            
            Debug.Log($"게임 종료 - 현재점수: {currentScore}, 목표점수: {targetScore}, 결과: {(isSuccess ? "SUCCESS" : "FAIL")}");
        }
        
        ShowResult(isSuccess);
        
        // GameManager에 게임 종료 알림
        if (gameManager != null)
        {
            gameManager.OnGameEnd();
        }
    }
    
    /// <summary>
    /// 결과 화면 표시
    /// </summary>
    public void ShowResult(bool isSuccess)
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }
        
        if (isSuccess)
        {
            if (successMessage != null) successMessage.SetActive(true);
            if (failMessage != null) failMessage.SetActive(false);
        }
        else
        {
            if (successMessage != null) successMessage.SetActive(false);
            if (failMessage != null) failMessage.SetActive(true);
        }
        
        Debug.Log($"결과 표시: {(isSuccess ? "SUCCESS" : "FAIL")}");
    }
    
    /// <summary>
    /// 결과 화면 숨김
    /// </summary>
    public void HideResult()
    {
        if (resultPanel != null) resultPanel.SetActive(false);
        if (successMessage != null) successMessage.SetActive(false);
        if (failMessage != null) failMessage.SetActive(false);
    }
    
    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void RestartGame()
    {
        InitializeTimer();
        HideResult();
        
        // GameManager에 재시작 알림
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
        
        StartTimer();
    }
    
    /// <summary>
    /// 강제로 게임 성공 처리 (목표 점수 달성 시 호출)
    /// </summary>
    public void ForceGameSuccess()
    {
        if (isGameEnded) return;
        
        Debug.Log("목표 점수 달성! 게임 성공");
        isGameEnded = true;
        isTimerRunning = false;
        ShowResult(true);
        
        if (gameManager != null)
        {
            gameManager.OnGameEnd();
        }
    }
    
    // 게임 상태 프로퍼티
    public float CurrentTime => currentTime;
    public float GameDuration => gameDuration;
    public bool IsTimerRunning => isTimerRunning;
    public bool IsGameEnded => isGameEnded;
    public float TimeProgress => 1f - (currentTime / gameDuration);
    
    void OnDestroy()
    {
        // 이벤트 정리
        OnTimeUp = null;
        OnTimeChanged = null;
    }
}