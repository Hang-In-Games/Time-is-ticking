using System;
using UnityEngine;
using UnityEngine.UI;

public class MashButtonMIniGame : MonoBehaviour, IMiniGame
{
    [SerializeField] private Slider _guage;
    [SerializeField] private float _maxValue = 100f;
    [SerializeField] private float _initialValue = 20f;
    [SerializeField] private float _increasePerMash = 2f;
    [SerializeField] private float _decreasePerSecond = 2f;
    [SerializeField] private AudioSource _mashSound;
    
    private bool _isRunning;
    private bool _hasEnded;

    public void StartGame()
    {
        GameManager.Instance.StartMiniGame(this);
    }
    public void Init()
    {
        _guage.maxValue = _maxValue;
    }
    
    public void StartMiniGame()
    {
        _guage.value = _initialValue;
        _isRunning = true;
        _hasEnded = false;
        gameObject.SetActive(true);
    }

    public GimmickType GimmickType { get; } = GimmickType.MashMiniGame;

    private void EndMiniGame(bool isWin)
    {
        _hasEnded = true;
        _isRunning = false;
        OnMiniGameEnd?.Invoke(isWin);
        gameObject.SetActive(false);
    }
    
    public Action<bool> OnMiniGameEnd { get; set; }

    private void Update()
    {
        if (!_isRunning || _hasEnded) return;

        // 시간에 따라 감소
        _guage.value -= _decreasePerSecond * Time.deltaTime;

        if (_guage.value <= 0f)
        {
            _guage.value = 0f;
            EndMiniGame(false);
        }
    }

    public void MashButton()
    {
        if (!_isRunning || _hasEnded) return;

        _mashSound.Play();
        _guage.value = Mathf.Min(_guage.maxValue, _guage.value + _increasePerMash);

        if (_guage.value >= _guage.maxValue)
        {
            _guage.value = _guage.maxValue;
            EndMiniGame(true);
        }
    }
}