// Assets/Script/Game/GameManager.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GimmickType
{
    MashMiniGame,
    TimeBouncer,
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private DigitalClock _clock = null;
    [SerializeField] private TimeEventCollection _timeEventCollection = null;
    [SerializeField] private AudioSource _BGMAudioSource;
    [SerializeField] private AudioSource _endGameAudioSource;
    [SerializeField] private GameObject _endGameUI;
    [SerializeField] private Animation _endGameAnimation;
    
    private List<TimeEventTarget> _timeEventTargets = new();
    private IMiniGame _currentMiniGame;
    private Dictionary<GimmickType, bool> _gimmickCleared = new();

    public IMiniGame CurrentMiniGame => _currentMiniGame;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        // 모든 GimmickType을 초기화(미클리어)
        foreach (GimmickType t in Enum.GetValues(typeof(GimmickType)))
            _gimmickCleared[t] = false;


        if (_clock != null)
        {
            _clock.OnReset += OnTimeReset;
            _clock.OnClear += OnClear;
        }

        _BGMAudioSource.Play();
        CollectTimeEventTargets();
        _timeEventCollection?.ResetAllTrigger();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (_clock != null)
        {
            _clock.OnReset -= OnTimeReset;
            _clock.OnClear -= OnClear;
        }
    }

    private void CollectTimeEventTargets()
    {
        _timeEventTargets.Clear();
        var roots = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var root in roots)
        {
            var targets = root.GetComponentsInChildren<TimeEventTarget>(true);
            _timeEventTargets.AddRange(targets);
        }
    }

    private void Update()
    {
        // 시간 이벤트 처리
        if (_clock == null || _timeEventCollection == null || _clock.IsPaused) return;

        int currentTime = Mathf.FloorToInt(_clock.ElapsedTimeAfterStart);
        foreach (var kv in _timeEventCollection.timeEventDefineDictionary)
        {
            var timeKey = kv.Key;
            var define = kv.Value;
            if (!define.IsTrigger && currentTime >= timeKey)
            {
                define.Trigger();
                ExecuteEvent(define);
            }
        }
    }

    private void ExecuteEvent(TimeEventDefine eventDefine)
    {
        foreach (var target in _timeEventTargets)
        {
            if (eventDefine.targetName.Equals(target.Key))
            {
                target.InvokeTimeEvent(eventDefine);
                return;
            }
        }
    }

    private void OnTimeReset()
    {
        StartCoroutine(EndGameCoroutine());
        _timeEventCollection?.ResetAllTrigger();
    }
    
    private void OnClear()
    {
        _endGameAnimation.Play();
        StartCoroutine(WinGameCoroutine());
        //SceneManager.LoadScene("");
        // 타임루프 클리어 시 처리 (필요시 구현)
    }

    private IEnumerator WinGameCoroutine()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Credit");
    }
    IEnumerator EndGameCoroutine()
    {
        _clock.Pause(PauseReason.EndGame);
        _BGMAudioSource.Stop();
        _endGameAudioSource.Play();
        _endGameUI.SetActive(true);
        yield return new WaitForSeconds(11f);
        _endGameUI.SetActive(false);
        _clock.ResetClock();
        _BGMAudioSource.Play();
        _endGameAudioSource.Stop();
        _clock.Resume(PauseReason.EndGame);
    }

    // IMiniGame 시작: 시계 일시정지, Init, Start, 종료 시 역순으로 정리
    public void StartMiniGame(IMiniGame miniGame)
    {
        if (miniGame == null) return;

        if (_currentMiniGame != null)
        {
            Debug.LogWarning("이미 실행중인 미니게임이 있습니다.");
            return;
        }

        _currentMiniGame = miniGame;

        // 시계 일시정지 (DigitalClock에 맞는 시그니처 사용)
        _clock?.Pause(PauseReason.MiniGame);
        _BGMAudioSource.Stop();
        
        _currentMiniGame.Init();

        _currentMiniGame.OnMiniGameEnd = (success) =>
        {
            if (success)
                MarkGimmickCleared(_currentMiniGame.GimmickType);
            
            _currentMiniGame.OnMiniGameEnd = null;
            _clock?.Resume(PauseReason.MiniGame);
            _BGMAudioSource.Play();
            _currentMiniGame = null;
        };

        _currentMiniGame.StartMiniGame();
    }
    
    // 특정 기믹 타입을 클리어 처리
    public void MarkGimmickCleared(GimmickType type)
    {
        if (!_gimmickCleared.ContainsKey(type)) return;

        if (_gimmickCleared[type]) return; // 이미 클리어됨

        _gimmickCleared[type] = true;

        if (AreAllGimmicksCleared())
        {
            _clock?.DisableLoop();
            Debug.Log("모든 기믹을 클리어했습니다. 타임루프 비활성화.");
        }
    }
    
    private bool AreAllGimmicksCleared()
    {
        foreach (var v in _gimmickCleared.Values)
        {
            if (!v) return false;
        }

        return true;
    }
}
